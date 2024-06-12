using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SistemaInventario.AccesoDatos.Data;
using SistemaInventario.Modelos;
using SistemaInventario.Modelos.ViewModels;
using SistemaInventario.Utilidades;

namespace SistemaInventario.Areas.Inventario.Controllers
{
    [Area("Inventario")]
    [Authorize(Roles =DS.Role_Admin+","+DS.Role_Inventario)]
    public class InventarioController : Controller
    {

        private readonly ApplicationDbContext _db;

        [BindProperty]
        public InventarioViewModel inventarioVM { get; set; }

        public InventarioController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            return View();
        }


        public IActionResult NuevoInventario(int? inventarioId)
        {
            inventarioVM = new InventarioViewModel();
            inventarioVM.BodegaLista = _db.Bodegas.ToList().Select(b => new SelectListItem
            {
                Text = b.Nombre,
                Value = b.Id.ToString()
            });
            inventarioVM.ProductoLista = _db.Productos.ToList().Select(p => new SelectListItem
            {
                Text = p.Descripcion,
                Value = p.Id.ToString()
            });

            inventarioVM.InventarioDetalles = new List<InventarioDetalle>();

            if (inventarioId!=null)
            {
                inventarioVM.Inventario = _db.Inventario.SingleOrDefault(i => i.Id == inventarioId);
                inventarioVM.InventarioDetalles = _db.InventarioDetalle.Include(p => p.Producto).Include(m=> m.Producto.Marca).
                    Where(d=> d.InventarioId== inventarioId).ToList();
            }

            return View(inventarioVM);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        //public IActionResult AgregarProductoPost(int producto, int cantidad, int inventarioId)
        public IActionResult NuevoInventario(int producto, int cantidad, int inventarioId)
        {
            inventarioVM.Inventario.Id = inventarioId;
            if (inventarioVM.Inventario.Id==0) // Graba el Registro en inventario
            {
                inventarioVM.Inventario.Estado = false;
                inventarioVM.Inventario.FechaInicial = DateTime.Now;
                // Capturar el Id del usuario conectado
                var claimIdentidad = (ClaimsIdentity)User.Identity;
                var claim = claimIdentidad.FindFirst(ClaimTypes.NameIdentifier);
                inventarioVM.Inventario.UsuarioAplicacionId = claim.Value;

                _db.Inventario.Add(inventarioVM.Inventario);
                _db.SaveChanges();
            }
            else
            {
                inventarioVM.Inventario = _db.Inventario.SingleOrDefault(i => i.Id == inventarioId);
            }
            var bodegaProducto = _db.BodegaProducto.Include(b => b.Producto).FirstOrDefault(b => b.ProductoId == producto &&
                                                                                            b.BodegaId == inventarioVM.Inventario.BodegaId);
            var detalle = _db.InventarioDetalle.Include(p => p.Producto).FirstOrDefault(d => d.ProductoId == producto &&
                                                                                         d.InventarioId == inventarioVM.Inventario.Id);
            if (detalle==null)
            {
                inventarioVM.InventarioDetalle = new InventarioDetalle();
                inventarioVM.InventarioDetalle.ProductoId = producto;
                inventarioVM.InventarioDetalle.InventarioId = inventarioVM.Inventario.Id;
                if (bodegaProducto!=null)
                {
                    inventarioVM.InventarioDetalle.StockAnterior = bodegaProducto.Cantidad;
                }
                else
                {
                    inventarioVM.InventarioDetalle.StockAnterior = 0;
                }
                inventarioVM.InventarioDetalle.Cantidad = cantidad;
                _db.InventarioDetalle.Add(inventarioVM.InventarioDetalle);
                _db.SaveChanges();

            }
            else
            {
                detalle.Cantidad += cantidad;
                _db.SaveChanges();
            }

            return RedirectToAction("NuevoInventario", new { inventarioId = inventarioVM.Inventario.Id });
        }


        public IActionResult Mas(int Id)
        {
            inventarioVM = new InventarioViewModel();
            var detalle = _db.InventarioDetalle.FirstOrDefault(d => d.Id == Id);
            inventarioVM.Inventario = _db.Inventario.FirstOrDefault(i => i.Id == detalle.InventarioId);

            detalle.Cantidad += 1;
            _db.SaveChanges();
            return RedirectToAction("NuevoInventario", new { inventarioId = inventarioVM.Inventario.Id });
        }

        public IActionResult Menos(int Id)
        {
            inventarioVM = new InventarioViewModel();
            var detalle = _db.InventarioDetalle.FirstOrDefault(d => d.Id == Id);
            inventarioVM.Inventario = _db.Inventario.FirstOrDefault(i => i.Id == detalle.InventarioId);
            if (detalle.Cantidad==1)
            {
                _db.InventarioDetalle.Remove(detalle);
                _db.SaveChanges();
            }
            else
            {
                detalle.Cantidad -= 1;
                _db.SaveChanges();
            }
            return RedirectToAction("NuevoInventario", new { inventarioId = inventarioVM.Inventario.Id });
        }


        public IActionResult GenerarStock(int Id)
        {
            var inventario = _db.Inventario.FirstOrDefault(i => i.Id == Id);
            var detalleLista = _db.InventarioDetalle.Where(d => d.InventarioId == Id).ToList();

            foreach (var item in detalleLista)
            {
   
                var bodegaProducto = _db.BodegaProducto.Include(p => p.Producto).FirstOrDefault(b => b.ProductoId == item.ProductoId &&
                                                                                                   b.BodegaId == inventario.BodegaId);

                if (bodegaProducto!=null)
                {
                    bodegaProducto.Cantidad += item.Cantidad;
                }
                else
                {
                    bodegaProducto = new BodegaProducto();
                    bodegaProducto.BodegaId = inventario.BodegaId;
                    bodegaProducto.ProductoId = item.ProductoId;
                    bodegaProducto.Cantidad = item.Cantidad;
                    _db.BodegaProducto.Add(bodegaProducto);
                }
                bodegaProducto = new BodegaProducto();
            }

            // Actualizar la Cabecera de Inventario
            inventario.Estado = true;
            inventario.FechaFinal = DateTime.Now;
            _db.SaveChanges();
            //return RedirectToAction(nameof(Index));
            return RedirectToAction("Index");
        }

       
        public IActionResult Historial()
        {
            return View();
        }

        public IActionResult DetalleHistorial(int id)
        {
            var detalleHistorial = _db.InventarioDetalle.Include(p => p.Producto).Include(m => m.Producto.Marca).Where(d => d.InventarioId == id);
            return View(detalleHistorial);
        }

        #region API

        [HttpGet]
        public IActionResult ObtenerTodos()
        {
            var todos = _db.BodegaProducto.Include(b => b.Bodega).Include(p => p.Producto).ToList();
            return Json(new { data = todos });
        }
       
        [HttpGet]
        public IActionResult ObtenerHistorial()
        {
            var todos = _db.Inventario.Include(b => b.Bodega).Include(u => u.UsuarioAplicacion).Where(i => i.Estado == true).ToList();
            return Json(new { data = todos });
        }
        #endregion

    }
}