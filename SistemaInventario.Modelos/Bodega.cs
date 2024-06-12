using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SistemaInventario.Modelos
{
    public class Bodega
    {
      
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage ="Nombre es Obligatorio")]
        [MaxLength(50)]
        [Display(Name ="Nombre")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "Descripcion es Obligatorio")]
        [MaxLength(100)]
        [Display(Name ="Descripcion")]
        public string Descripcion { get; set; }

        [Required]
        public bool Estado { get; set; }


    }
}
