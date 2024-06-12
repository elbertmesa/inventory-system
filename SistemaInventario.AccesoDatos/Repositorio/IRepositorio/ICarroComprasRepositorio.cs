using SistemaInventario.Modelos;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaInventario.AccesoDatos.Repositorio.IRepositorio
{
    public interface ICarroComprasRepositorio :IRepositorio<CarroCompras> 
    {
        void Actualizar(CarroCompras carroCompras);
    }
}
