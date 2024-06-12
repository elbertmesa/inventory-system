using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SistemaInventario.Modelos
{
    public class Categoria
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        [Display(Name ="Nombre Categoria")]
        [RegularExpression(@"^[a-zA-Z''-'\s]{1,50}$",
         ErrorMessage = "Numeros y Caracteres especiales no estan permitidos.")]
        public string Nombre { get; set; }

        [Required]
        public bool Estado { get; set; }

    }
}
