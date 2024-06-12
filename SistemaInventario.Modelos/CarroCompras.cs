using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SistemaInventario.Modelos
{
    public class CarroCompras
    {
        public CarroCompras()
        {
            Cantidad = 1;
        }

        [Key]
        public int Id { get; set; }

        [Required]
        public string UsuarioAplicacionId { get; set; }

        [ForeignKey("UsuarioAplicacionId")]
        public UsuarioAplicacion UsuarioAplicacion { get; set; }

        [Required]
        public int ProductoId { get; set; }

        [ForeignKey("ProductoId")]
        public Producto Producto { get; set; }

        [Required]
        [Range(1,1000,ErrorMessage ="Ingresar un valor de 1 al 1000")]
        public int Cantidad { get; set; }

        [NotMapped]
        public double Precio { get; set; }
    }
}
