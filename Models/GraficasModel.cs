namespace Insane_Mechanical.Models
{
    public class GraficasModel
    {
        public List<PreguntaErroresViewModel> PreguntasMasErrores { get; set; }
        public List<PreguntaErroresViewModel> PreguntasMenosErrores { get; set; }
    }

    public class PreguntaErroresViewModel
    {
        public string Pregunta { get; set; }
        public int RespuestasIncorrectas { get; set; }
        public int RespuestasCorrectas { get; set; }
    }
}
