using SistemaInventario.Modelos;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaInventario.AccesoDatos.Repositorio.IRepositorio
{
    public interface ICompaniaRepositorio :IRepositorio<Compania>
    {
        void Actualizar(Compania compania);
    }
}
