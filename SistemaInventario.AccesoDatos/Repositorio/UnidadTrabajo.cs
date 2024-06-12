using SistemaInventario.AccesoDatos.Data;
using SistemaInventario.AccesoDatos.Repositorio.IRepositorio;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaInventario.AccesoDatos.Repositorio
{
    public class UnidadTrabajo :IUnidadTrabajo
    {
        private readonly ApplicationDbContext _db;
        public IBodegaRepositorio Bodega { get; private set; }
        public ICategoriaRepositorio Categoria { get; private set; }
        public IMarcaRepositorio Marca { get; private set; }
        public IProductoRepositorio Producto { get; private set; }
        public IUsuarioAplicacionRepositorio UsuarioAplicacion { get; private set; }
        public ICompaniaRepositorio Compania { get; private set; }
        public ICarroComprasRepositorio CarroCompras { get; private set; }
        public IOrdenRepositorio Orden { get; private set; }
        public IOrdenDetalleRepositorio OrdenDetalle { get; private set; }

        public UnidadTrabajo(ApplicationDbContext db)
        {
            _db = db;
            Bodega = new BodegaRepositorio(_db); // Inicializamos
            Categoria = new CategoriaRepositorio(_db);
            Marca = new MarcaRepositorio(_db);
            Producto = new ProductoRepositorio(_db);
            UsuarioAplicacion = new UsuarioAplicacionRepositorio(_db);
            Compania = new CompaniaRepositorio(_db);
            CarroCompras = new CarroComprasRepositorio(_db);
            Orden = new OrdenRepositorio(_db);
            OrdenDetalle = new OrdenDetalleRepositorio(_db);

        }

        public void Guardar()
        {
            _db.SaveChanges();
        }

        public void Dispose()
        {
            _db.Dispose();
        }
    }
}
