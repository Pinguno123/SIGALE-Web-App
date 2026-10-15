namespace SIGALE_Web_App.Models
{
    public class InscripcionDAC
    {
        public int IdInscripcionDAC { get; set; }

        public string Carnet { get; set; } = string.Empty;

        public int IdActividad { get; set; }

        public DateTime? FechaInscripcion { get; set; }

        public string? EstadoInscripcion { get; set; }
            = "Activa";

        // Relaciones
        public Usuario Usuario { get; set; } = null!;

        public ActividadDAC Actividad { get; set; } = null!;
    }
}
