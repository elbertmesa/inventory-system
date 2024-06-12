using SistemaInventario.AccesoDatos.Data;
using SistemaInventario.AccesoDatos.Repositorio.IRepositorio;
using SistemaInventario.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SistemaInventario.AccesoDatos.Repositorio
{
    public class CompaniaRepositorio : Repositorio<Compania>, ICompaniaRepositorio
    {
        private readonly ApplicationDbContext _db;

        public CompaniaRepositorio(ApplicationDbContext db):base(db)
        {
            _db = db;
        }

        public void Actualizar(Compania compania)
        {
            var companiaDb = _db.Compania.FirstOrDefault(c => c.Id == compania.Id);
            if (companiaDb!=null)
            {
                if (compania.LogoUrl!=null)
                {
                    companiaDb.LogoUrl = compania.LogoUrl;
                }
                companiaDb.Nombre = compania.Nombre;
                companiaDb.Descripcion = compania.Descripcion;
                companiaDb.Direccion = compania.Direccion;
                companiaDb.BodegaVentaId = compania.BodegaVentaId;
                companiaDb.Pais = compania.Pais;
                companiaDb.Ciudad = compania.Ciudad;
                companiaDb.Telefono = compania.Telefono;
            }
        }
    }
}
