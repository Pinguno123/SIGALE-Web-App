namespace SIGALE_Web_App.Models
{
    public class Asociacion
    {
        public int IdAsociacion { get; set; }

        public string Acronimo { get; set; } = string.Empty;

        public string Nombre { get; set; } = string.Empty;

        public int? AnioFundacion { get; set; }

        public string? Descripcion { get; set; }

        public string? Formulario { get; set; }

        public string? ImgUrl { get; set; }

        // Relaciones
        public ICollection<MembresiaAsociacion> Membresias
        { get; set; } = new List<MembresiaAsociacion>();
    }

}
