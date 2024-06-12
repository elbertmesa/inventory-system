using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaInventario.Utilidades
{
    public static class DS
    {
        public const string Role_Cliente = "Cliente";
        public const string Role_Admin = "Admin";
        public const string Role_Inventario = "Inventario";
        public const string Role_Ventas = "Ventas";
        public const string ssCarroCompras = "Sesion Carro Compras";


        public const string EstadoPendiente = "Pendiente";
        public const string EstadoAprobado = "Aprobado";
        public const string EstadoEnProceso = "Procesando";
        public const string EstadoEnviado = "Enviado";
        public const string EstadoCancelado="Cancelado";
        public const string EstadoDevuelto = "Devuelto";

        public const string PagoEstadoPendiente = "Pendiente";
        public const string PagoEstadoAprobado = "Aprobado";
        public const string PagoEstadoRetrasado = "Retrasado";
        public const string EstadoRechazado = "Rechazado";

    }
}
