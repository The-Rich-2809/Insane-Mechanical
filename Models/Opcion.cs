using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Insane_Mechanical.Models
{
    public class Opcion
    {
        [Key]
        public int ID { get; set; }
        public string Texto { get; set; }
        public bool EsCorrecta { get; set; }
        public int idPregunta { get; set; }

        [ForeignKey("idPregunta")]
        public virtual Pregunta Pregunta { get; set; }
    }
}
