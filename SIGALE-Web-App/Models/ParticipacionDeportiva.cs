namespace SIGALE_Web_App.Models
{
    public class ParticipacionDeportiva
    {
        public int IdParticipacion { get; set; }

        public string Carnet { get; set; } = string.Empty;

        public int IdSeleccion { get; set; }

        public string? Modalidad { get; set; }

        public DateOnly? FechaIngreso { get; set; }

        // Relaciones
        public Usuario Usuario { get; set; } = null!;

        public SeleccionDeportiva SeleccionDeportiva
        { get; set; } = null!;
    }
}
