using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SistemaInventario.Modelos
{
    public class Producto
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(30)]
        [Display(Name ="Numero de Serie")]
        public string NumeroSerie { get; set; }

        [Required]
        [MaxLength(60)]
        [Display(Name ="Descripcion")]
        public string Descripcion { get; set; }

        [Required]
        [Range(1, 10000)]
        [Display(Name ="Precio")]
        public double Precio { get; set; }

        [Required]
        [Range(1, 10000)]
        [Display(Name ="Costo")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public double Costo { get; set; }

        public string ImagenUrl { get; set; }

        // Foreign keys
        [Required(ErrorMessage = "Seleccione una Categoria Correcta")]
        public int CategoriaId { get; set; }

        [ForeignKey("CategoriaId")]
        public Categoria Categoria { get; set; }

        [Required]
        public int MarcaId { get; set; }

        [ForeignKey("MarcaId")]
        public Marca Marca { get; set; }

        // Recursividad

        public int? PadreId { get; set; }
        public virtual Producto Padre { get; set; }

    }
}
