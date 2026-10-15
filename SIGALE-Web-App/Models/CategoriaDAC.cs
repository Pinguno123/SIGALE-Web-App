namespace SIGALE_Web_App.Models
{
    public class CategoriaDAC
    {
        public int IdCategoria { get; set; }

        public string NombreCategoria { get; set; } = string.Empty;

        public string? Descripcion { get; set; }

        // Relaciones
        public ICollection<ActividadDAC> Actividades
        { get; set; } = new List<ActividadDAC>();
    }
}
