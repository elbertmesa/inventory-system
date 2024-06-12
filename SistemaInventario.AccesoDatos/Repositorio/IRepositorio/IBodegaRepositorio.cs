using SistemaInventario.Modelos;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaInventario.AccesoDatos.Repositorio.IRepositorio
{
    public interface IBodegaRepositorio :IRepositorio<Bodega>
    {
        void Actualizar(Bodega bodega);
    }
}
