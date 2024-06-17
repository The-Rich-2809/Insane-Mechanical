using System.ComponentModel.DataAnnotations;

namespace Insane_Mechanical.Models
{
    public class Pregunta
    {
        [Key]
        public int ID { get; set; }
        public string Nombre { get; set; }
        public string Tipo { get; set; }
        public virtual ICollection<Opcion> Opciones { get; set; }
    }
}
