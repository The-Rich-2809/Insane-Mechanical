using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Insane_Mechanical.Models
{
    public class Categoria
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public string Titulo { get; set; }
        public string RutaImagen { get; set; }
        public string Descripcion {  get; set; }
    }
}
