using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SistemaInventario.Modelos
{
    public class Orden
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UsuarioAplicacionId { get; set; }

        [ForeignKey("UsuarioAplicacionId")]
        public UsuarioAplicacion UsuarioAplicacion { get; set; }

        [Required]
        public DateTime FechaOrden { get; set; }

        [Required]
        public DateTime FechaEnvio { get; set; }

        public string NumeroEnvio { get; set; }
        public string Carrier { get; set; }

        [Required]
        public double TotalOrden { get; set; }

        public string EstadoOrden { get; set; }

        public string EstadoPago { get; set; }

        public DateTime FechaPago { get; set; }

        public DateTime FechaMaximaPago { get; set; }

        public string TransaccionId { get; set; }

        public string Telefono { get; set; }

        public string Direccion { get; set; }

        public string Ciudad { get; set; }

        public string Pais { get; set; }

        public string NombresCliente { get; set; }
    }
}
