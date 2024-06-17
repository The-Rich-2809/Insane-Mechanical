using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Insane_Mechanical.Models
{
    public class RespuestasUsuario
    {
        [Key]
        public int Id { get; set; }
        public int idPregunta { get; set; }
        public string Respuesta { get; set; }
        public int idUsuario { get; set; }
        public int idOpcion { get; set; }


        [ForeignKey("idOpcion")]
        public virtual Opcion Opcion { get; set; }

        [ForeignKey("idPregunta")]
        public virtual Usuario Usuario { get; set; }

        [ForeignKey("idUsuario")]
        public virtual Pregunta Pregunta { get; set; }
    }
}
