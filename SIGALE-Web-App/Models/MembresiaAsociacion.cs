namespace SIGALE_Web_App.Models
{
    public class MembresiaAsociacion
    {
        public int IdMembresia { get; set; }

        public string Carnet { get; set; } = string.Empty;

        public int IdAsociacion { get; set; }

        public string? Rol { get; set; } = "Miembro";

        public DateOnly? FechaSolicitud { get; set; }

        public string? EstadoValidacion { get; set; } = "Pendiente";

        // Relaciones
        public Usuario Usuario { get; set; } = null!;

        public Asociacion Asociacion { get; set; } = null!;
    }
}
