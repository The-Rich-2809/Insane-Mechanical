﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Insane_Mechanical.Models
{
    public class Usuario
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public string Correo { get; set; }
        public string Contrasena { get; set; }
        public string DireccionImagen { get; set; }
        public string Nombre { get; set; }
        public string TipoUsuario { get; set; }
        public string? Codigcadena { get; set; }
        public DateTime? FechadeExpiracion { get; set; }
        public bool Verificado { get; set; }
        public string Telefono { get; set; }
        public string Genero { get; set; }
        public virtual ICollection<RespuestasUsuario> Respuestas { get; set; }
    }
}
