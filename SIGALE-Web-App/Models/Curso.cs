namespace SIGALE_Web_App.Models
{
    public class Curso
    {
        public int IdCurso { get; set; }

        public string NombreCurso { get; set; } = string.Empty;

        public string? Descripcion { get; set; }

        public DateOnly FechaInicio { get; set; }

        public DateOnly FechaFin { get; set; }

        public string Horario { get; set; } = string.Empty;

        public int CupoMaximo { get; set; }

        public string? Estado { get; set; } = "Activo";

        public string? CarnetInstructor { get; set; }

        // Instructor del curso
        public Usuario? Instructor { get; set; }

        // Inscripciones del curso
        public ICollection<InscripcionCurso> Inscripciones
        { get; set; } = new List<InscripcionCurso>();
    }
}
