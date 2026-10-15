namespace SIGALE_Web_App.Models
{
    public class InscripcionCurso
    {
        public int IdInscripcion { get; set; }

        public string Carnet { get; set; } = string.Empty;

        public int IdCurso { get; set; }

        public decimal? Nota { get; set; }

        public DateTime? FechaInscripcion { get; set; }

        public string? EstadoInscripcion { get; set; }
            = "Confirmada";

        // Relaciones
        public Usuario Usuario { get; set; } = null!;

        public Curso Curso { get; set; } = null!;
    }
}
