using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SistemaInventario.Modelos
{
    public class BodegaProducto
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name ="Bodega")]
        public int BodegaId { get; set; }

        [ForeignKey("BodegaId")]
        public Bodega Bodega { get; set; }

        [Required]
        [Display(Name ="Producto")]
        public int ProductoId { get; set; }

        [ForeignKey("ProductoId")]
        public Producto Producto { get; set; }

        [Required]
        [Display(Name ="Cantidad")]
        public int Cantidad { get; set; }
    }
}
