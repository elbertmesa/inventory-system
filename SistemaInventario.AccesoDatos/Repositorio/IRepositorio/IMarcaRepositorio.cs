using SistemaInventario.Modelos;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaInventario.AccesoDatos.Repositorio.IRepositorio
{
    public interface IMarcaRepositorio :IRepositorio<Marca>
    {
        void Actualizar(Marca marca);

    }
}
