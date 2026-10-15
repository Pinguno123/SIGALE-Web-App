namespace SIGALE_Web_App.Models
{
    public class SeleccionDeportiva
    {
        public int IdSeleccion { get; set; }

        public string Disciplina { get; set; } = string.Empty;

        public string Rama { get; set; } = string.Empty;

        public string NombreEquipo { get; set; } = string.Empty;

        public string? ImgUrl { get; set; }

        // Relaciones
        public ICollection<ParticipacionDeportiva> Participaciones
        { get; set; } = new List<ParticipacionDeportiva>();
    }
}