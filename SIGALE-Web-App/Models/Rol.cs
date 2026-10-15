namespace SIGALE_Web_App.Models
{
    public class Rol
    {
        public int IdRol { get; set; }

        public string NombreRol { get; set; } = string.Empty;

        public string? Descripcion { get; set; }

        // Relaciones
        public ICollection<Usuario> Usuarios { get; set; }
            = new List<Usuario>();
    }

}
