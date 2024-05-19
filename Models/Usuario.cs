using System.ComponentModel.DataAnnotations.Schema;
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
    }
}
