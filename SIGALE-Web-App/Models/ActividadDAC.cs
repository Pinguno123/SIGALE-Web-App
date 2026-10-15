namespace SIGALE_Web_App.Models
{
    public class ActividadDAC
    {
        public int IdActividad { get; set; }

        public int IdCategoria { get; set; }

        public string NombreActividad { get; set; } = string.Empty;

        public string? Descripcion { get; set; }

        public string? Estado { get; set; } = "Activo";

        public string? CarnetInstructor { get; set; }

        // Categoría de la actividad
        public CategoriaDAC Categoria { get; set; } = null!;

        // Instructor asignado
        public Usuario? Instructor { get; set; }

        // Inscripciones a la actividad
        public ICollection<InscripcionDAC> Inscripciones
        { get; set; } = new List<InscripcionDAC>();
    }
}
