namespace SIGALE_Web_App.Models
{
    public class Usuario
    {
        public string Carnet { get; set; } = string.Empty;

        public string NombreCompleto { get; set; } = string.Empty;

        public string Carrera { get; set; } = string.Empty;

        public string Campus { get; set; } = string.Empty;

        public bool Activo { get; set; } = true;

        public string CorreoElectronico { get; set; } = string.Empty;

        public int IdRol { get; set; }

        // Navegación hacia Rol
        public Rol Rol { get; set; } = null!;

        // Asociaciones
        public ICollection<MembresiaAsociacion> MembresiasAsociaciones
        { get; set; } = new List<MembresiaAsociacion>();

        // Cursos impartidos
        public ICollection<Curso> CursosImpartidos
        { get; set; } = new List<Curso>();

        // Inscripciones a cursos
        public ICollection<InscripcionCurso> InscripcionesCursos
        { get; set; } = new List<InscripcionCurso>();

        // Participaciones deportivas
        public ICollection<ParticipacionDeportiva> ParticipacionesDeportivas
        { get; set; } = new List<ParticipacionDeportiva>();

        // Actividades DAC impartidas
        public ICollection<ActividadDAC> ActividadesDACImpartidas
        { get; set; } = new List<ActividadDAC>();

        // Inscripciones DAC
        public ICollection<InscripcionDAC> InscripcionesDAC
        { get; set; } = new List<InscripcionDAC>();
    }

}
