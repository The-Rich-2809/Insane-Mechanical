using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Insane_Mechanical.Models
{
    public class Articulo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public int CategoriaId { get; set; }
        public string Titulo { get; set;}
        public string Descripcion {  get; set; }
        public string RutaHTML { get; set; }
        public string RutaImagen { get; set; }
    }
}
