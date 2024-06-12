using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SistemaInventario.AccesoDatos.Repositorio.IRepositorio;
using SistemaInventario.Modelos;
using SistemaInventario.Modelos.ViewModels;
using SistemaInventario.Utilidades;

namespace SistemaInventario.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = DS.Role_Admin)]
    public class CompaniaController : Controller
    {
        private readonly IUnidadTrabajo _unidadTrabajo;
        private readonly IWebHostEnvironment _hostEnvironment;

        public CompaniaController(IUnidadTrabajo unidadTrabajo, IWebHostEnvironment hostEnvironment)
        {
            _unidadTrabajo = unidadTrabajo;
            _hostEnvironment = hostEnvironment;
        }


        public IActionResult Index()
        {
            var compania = _unidadTrabajo.Compania.ObtenerTodos();
            return View(compania);
        }

        public IActionResult Upsert(int? id)
        {
            CompaniaVM companiaVM = new CompaniaVM() { 
                Compania = new Compania(),
                BodegaLista = _unidadTrabajo.Bodega.ObtenerTodos().Select(c=> new SelectListItem {
                  Text = c.Nombre,
                  Value =c.Id.ToString()
                }),
            };


            if (id == null)
            {
                // Esto es para Crear nuevo Registro
                return View(companiaVM);
            }
            // Esto es para Actualizar
            companiaVM.Compania = _unidadTrabajo.Compania.Obtener(id.GetValueOrDefault());
            if (companiaVM.Compania == null)
            {
                return NotFound();
            }

            return View(companiaVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(CompaniaVM companiaVM)
        {
            if (ModelState.IsValid)
            {

                // Cargar Imagenes
                string webRootPath = _hostEnvironment.WebRootPath;
                var files = HttpContext.Request.Form.Files;
                if (files.Count>0)
                {
                    string filename = Guid.NewGuid().ToString();
                    var uploads = Path.Combine(webRootPath, @"imagenes\compania");
                    var extension = Path.GetExtension(files[0].FileName);
                    if (companiaVM.Compania.LogoUrl!=null)
                    {
                        //Esto es para editar, necesitamos borrar la imagen anterior
                        var imagenPath = Path.Combine(webRootPath, companiaVM.Compania.LogoUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(imagenPath))
                        {
                            System.IO.File.Delete(imagenPath);
                        }
                    }

                    using (var filesStreams = new FileStream(Path.Combine(uploads, filename + extension), FileMode.Create))
                    {
                        files[0].CopyTo(filesStreams);
                    }
                    companiaVM.Compania.LogoUrl = @"\imagenes\compania\" + filename + extension;
                }
                else
                {
                    // Si en el Update el usuario no cambia la imagen
                    if (companiaVM.Compania.Id!=0)
                    {
                        Compania companiaDB = _unidadTrabajo.Compania.Obtener(companiaVM.Compania.Id);
                        companiaVM.Compania.LogoUrl = companiaDB.LogoUrl;
                    }
                }


                if (companiaVM.Compania.Id == 0)
                {
                    _unidadTrabajo.Compania.Agregar(companiaVM.Compania);
                }
                else
                {
                    _unidadTrabajo.Compania.Actualizar(companiaVM.Compania);
                }
                _unidadTrabajo.Guardar();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                companiaVM.BodegaLista = _unidadTrabajo.Bodega.ObtenerTodos().Select(c => new SelectListItem
                {
                    Text = c.Nombre,
                    Value = c.Id.ToString()
                });


                if (companiaVM.Compania.Id!=0)
                {
                    companiaVM.Compania = _unidadTrabajo.Compania.Obtener(companiaVM.Compania.Id);
                }

            }
            return View(companiaVM.Compania);
        }




        #region API
        [HttpGet]
        public IActionResult ObtenerTodos()
        {
            var todos = _unidadTrabajo.Compania.ObtenerTodos(incluirPropiedades: "Bodega");
            return Json(new { data = todos });
        }
       

        #endregion
    }
}