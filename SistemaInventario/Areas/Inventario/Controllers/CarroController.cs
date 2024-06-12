using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Rotativa.AspNetCore;
using SistemaInventario.AccesoDatos.Data;
using SistemaInventario.AccesoDatos.Repositorio.IRepositorio;
using SistemaInventario.Modelos;
using SistemaInventario.Modelos.ViewModels;
using SistemaInventario.Utilidades;
using Stripe;

namespace SistemaInventario.Areas.Inventario.Controllers
{
    [Area("Inventario")]
    public class CarroController : Controller
    {
        private readonly IUnidadTrabajo _unidadTrabajo;
        private readonly IEmailSender _emailSender;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _db;

        [BindProperty]
        public CarroComprasViewModel CarroComprasVM { get; set; }


        public CarroController(IUnidadTrabajo unidadTrabajo, IEmailSender emailSender, UserManager<IdentityUser> userManager, ApplicationDbContext db)
        {
            _unidadTrabajo = unidadTrabajo;
            _emailSender = emailSender;
            _userManager = userManager;
            _db = db;
        }

        public IActionResult Index()
        {
            var claimIdentidad = (ClaimsIdentity)User.Identity;
            var claim = claimIdentidad.FindFirst(ClaimTypes.NameIdentifier);

            CarroComprasVM = new CarroComprasViewModel()
            {
                Orden = new Modelos.Orden(),
                CarroComprasLista = _unidadTrabajo.CarroCompras.ObtenerTodos(u => u.UsuarioAplicacionId ==
                                                                claim.Value, incluirPropiedades: "Producto")
            };

            CarroComprasVM.Orden.TotalOrden = 0;
            CarroComprasVM.Orden.UsuarioAplicacion = _unidadTrabajo.UsuarioAplicacion.ObtenerPrimero(u => u.Id == claim.Value);

            foreach (var lista in CarroComprasVM.CarroComprasLista)
            {
                lista.Precio = lista.Producto.Precio;
                CarroComprasVM.Orden.TotalOrden += (lista.Precio * lista.Cantidad);
            }

            return View(CarroComprasVM);
        }


        public IActionResult mas(int carroId)
        {
            var carroCompras = _unidadTrabajo.CarroCompras.ObtenerPrimero(c => c.Id == carroId, incluirPropiedades: "Producto");
            carroCompras.Cantidad += 1;
            _unidadTrabajo.Guardar();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult menos(int carroId)
        {
            var carroCompras = _unidadTrabajo.CarroCompras.ObtenerPrimero(c => c.Id == carroId, incluirPropiedades: "Producto");
            if (carroCompras.Cantidad==1)
            {
                var numeroProductos = _unidadTrabajo.CarroCompras.ObtenerTodos(u => u.UsuarioAplicacionId ==
                                                                 carroCompras.UsuarioAplicacionId).ToList().Count();
                _unidadTrabajo.CarroCompras.Remover(carroCompras);
                _unidadTrabajo.Guardar();
                HttpContext.Session.SetInt32(DS.ssCarroCompras, numeroProductos - 1);
            }
            else
            {
                carroCompras.Cantidad -= 1;
                _unidadTrabajo.Guardar();
            }
            return RedirectToAction(nameof(Index));
        }

        public IActionResult remover(int carroId)
        {
            var carroCompras = _unidadTrabajo.CarroCompras.ObtenerPrimero(c => c.Id == carroId, incluirPropiedades: "Producto");
           
            var numeroProductos = _unidadTrabajo.CarroCompras.ObtenerTodos(u => u.UsuarioAplicacionId ==
                                                                 carroCompras.UsuarioAplicacionId).ToList().Count();
                _unidadTrabajo.CarroCompras.Remover(carroCompras);
                _unidadTrabajo.Guardar();
                HttpContext.Session.SetInt32(DS.ssCarroCompras, numeroProductos - 1);
            
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Proceder()
        {
            var claimIdentidad = (ClaimsIdentity)User.Identity;
            var claim = claimIdentidad.FindFirst(ClaimTypes.NameIdentifier);

            CarroComprasVM = new CarroComprasViewModel()
            {
                Orden = new Modelos.Orden(),
                CarroComprasLista = _unidadTrabajo.CarroCompras.ObtenerTodos(u => u.UsuarioAplicacionId ==
                                                                claim.Value, incluirPropiedades: "Producto"),
                Compania = _unidadTrabajo.Compania.ObtenerPrimero()
            };

            CarroComprasVM.Orden.TotalOrden = 0;
            CarroComprasVM.Orden.UsuarioAplicacion = _unidadTrabajo.UsuarioAplicacion.ObtenerPrimero(u => u.Id == claim.Value);

            foreach (var lista in CarroComprasVM.CarroComprasLista)
            {
                lista.Precio = lista.Producto.Precio;
                CarroComprasVM.Orden.TotalOrden += (lista.Precio * lista.Cantidad);
            }

            CarroComprasVM.Orden.NombresCliente = CarroComprasVM.Orden.UsuarioAplicacion.Nombres + " " +
                                                  CarroComprasVM.Orden.UsuarioAplicacion.Apellidos;
            CarroComprasVM.Orden.Telefono = CarroComprasVM.Orden.UsuarioAplicacion.PhoneNumber;
            CarroComprasVM.Orden.Direccion = CarroComprasVM.Orden.UsuarioAplicacion.Direccion;
            CarroComprasVM.Orden.Pais = CarroComprasVM.Orden.UsuarioAplicacion.Pais;
            CarroComprasVM.Orden.Ciudad = CarroComprasVM.Orden.UsuarioAplicacion.Ciudad;

            // Controlar Stock
            foreach (var item in CarroComprasVM.CarroComprasLista)
            {
                // Capturar el Stock de cada Producto
                var producto = _db.BodegaProducto.FirstOrDefault(b => b.ProductoId == item.ProductoId &&
                                                                     b.BodegaId == CarroComprasVM.Compania.BodegaVentaId);

                if (item.Cantidad>producto.Cantidad)
                {
                    TempData["Error"] = "La cantidad del producto " + item.Producto.Descripcion + " Excede al Stock actual";
                    return RedirectToAction(nameof(Index));
                }
            }


            return View(CarroComprasVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Proceder")]
        public IActionResult ProcederPost(string stripeToken)
        {
            var claimIdentidad = (ClaimsIdentity)User.Identity;
            var claim = claimIdentidad.FindFirst(ClaimTypes.NameIdentifier);
            CarroComprasVM.Orden.UsuarioAplicacion = _unidadTrabajo.UsuarioAplicacion.ObtenerPrimero(c => c.Id == claim.Value);
            CarroComprasVM.CarroComprasLista = _unidadTrabajo.CarroCompras.ObtenerTodos(
                                                    c => c.UsuarioAplicacionId == claim.Value, incluirPropiedades: "Producto");
            CarroComprasVM.Orden.EstadoOrden = DS.EstadoPendiente;
            CarroComprasVM.Orden.EstadoPago = DS.PagoEstadoPendiente;
            CarroComprasVM.Orden.UsuarioAplicacionId = claim.Value;
            CarroComprasVM.Orden.FechaOrden = DateTime.Now;
            CarroComprasVM.Compania = _unidadTrabajo.Compania.ObtenerPrimero();
            

            _unidadTrabajo.Orden.Agregar(CarroComprasVM.Orden);
            _unidadTrabajo.Guardar();

            foreach (var item in CarroComprasVM.CarroComprasLista)
            {
                OrdenDetalle ordenDetalle = new OrdenDetalle()
                {
                    ProductoId = item.ProductoId,
                    OrdenId = CarroComprasVM.Orden.Id,
                    Precio = item.Producto.Precio,
                    Cantidad = item.Cantidad
                };
                CarroComprasVM.Orden.TotalOrden += ordenDetalle.Cantidad * ordenDetalle.Precio;
                _unidadTrabajo.OrdenDetalle.Agregar(ordenDetalle);
                
            }
            // Remover los productos del carro de Compras
            _unidadTrabajo.CarroCompras.RemoverRango(CarroComprasVM.CarroComprasLista);
            _unidadTrabajo.Guardar();
            HttpContext.Session.SetInt32(DS.ssCarroCompras, 0);

            if (stripeToken == null)
            {

            }
            else
            {
                //Procesar el Pago
                var options = new ChargeCreateOptions
                {
                    Amount = Convert.ToInt32(CarroComprasVM.Orden.TotalOrden * 100),
                    Currency = "usd",
                    Description = "Numero de Orden: " + CarroComprasVM.Orden.Id,
                    Source = stripeToken
                };
                var service = new ChargeService();
                try
                {
                    Charge charge = service.Create(options);
                    if (charge.BalanceTransactionId == null)
                    {
                        CarroComprasVM.Orden.EstadoPago = DS.EstadoRechazado;
                    }
                    else
                    {
                        CarroComprasVM.Orden.TransaccionId = charge.Id;// charge..BalanceTransactionId;
                    }
                    if (charge.Status.ToLower() == "succeeded")
                    {
                        CarroComprasVM.Orden.EstadoPago = DS.PagoEstadoAprobado;
                        CarroComprasVM.Orden.EstadoOrden = DS.EstadoAprobado;
                        CarroComprasVM.Orden.FechaPago = DateTime.Now;


                        // Actualizar Stock del Inventario
                        foreach (var item in CarroComprasVM.CarroComprasLista)
                        {
                            var producto = _db.BodegaProducto.FirstOrDefault(b => b.ProductoId == item.ProductoId &&
                                                                                b.BodegaId == CarroComprasVM.Compania.BodegaVentaId);

                            producto.Cantidad -= item.Cantidad;  // Disminuye la Cantidad del Stock del Producto/Bodega
                        }

                    }
                }
                catch (Exception)
                {

                    return RedirectToAction("OrdenError", "Carro", new { id = CarroComprasVM.Orden.Id });
                }
                
                

            }
            _unidadTrabajo.Guardar();
            return RedirectToAction("OrdenConfirmacion", "Carro", new { id = CarroComprasVM.Orden.Id });
            
        }

        public IActionResult OrdenError(int id)
        {
            var orden = _unidadTrabajo.Orden.Obtener(id);
            orden.EstadoOrden = "Rechazado";
            _unidadTrabajo.Guardar();
            return View();
        }

        public IActionResult OrdenConfirmacion(int id)
        {
            return View(id);
        }

        public IActionResult ImprimirOrden(int id)
        {
            CarroComprasVM = new CarroComprasViewModel();
            CarroComprasVM.Compania = _unidadTrabajo.Compania.ObtenerPrimero();
            CarroComprasVM.Orden = _unidadTrabajo.Orden.ObtenerPrimero(o => o.Id == id, incluirPropiedades: "UsuarioAplicacion");
            CarroComprasVM.OrdenDetalleLista = _unidadTrabajo.OrdenDetalle.ObtenerTodos(d => d.OrdenId == id, incluirPropiedades: "Producto");

            return new ViewAsPdf("ImprimirOrden", CarroComprasVM)
            {
                FileName = "Orden#"+CarroComprasVM.Orden.Id+".pdf",
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait,
                PageSize = Rotativa.AspNetCore.Options.Size.A4,
                CustomSwitches = "--page-offset 0 --footer-center [page] --footer-font-size 12"                
            }; 

        }

    }
}