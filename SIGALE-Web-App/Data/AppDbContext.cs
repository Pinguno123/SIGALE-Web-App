using Microsoft.EntityFrameworkCore;
using SIGALE_Web_App.Models;

namespace SIGALE_Web_App.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // ==============================
        // ESQUEMA AUTH
        // ==============================
        public DbSet<Rol> Roles => Set<Rol>();
        public DbSet<Usuario> Usuarios => Set<Usuario>();

        // ==============================
        // ESQUEMA CLUBS
        // ==============================
        public DbSet<Asociacion> Asociaciones => Set<Asociacion>();
        public DbSet<MembresiaAsociacion> MembresiaAsociaciones
            => Set<MembresiaAsociacion>();

        // ==============================
        // ESQUEMA COURSES
        // ==============================
        public DbSet<Curso> Cursos => Set<Curso>();
        public DbSet<InscripcionCurso> InscripcionCursos
            => Set<InscripcionCurso>();

        // ==============================
        // ESQUEMA SPORTS
        // ==============================
        public DbSet<SeleccionDeportiva> SeleccionesDeportivas
            => Set<SeleccionDeportiva>();
        public DbSet<ParticipacionDeportiva> ParticipacionDeportiva
            => Set<ParticipacionDeportiva>();

        // ==============================
        // ESQUEMA DAC
        // ==============================
        public DbSet<CategoriaDAC> CategoriasDAC
            => Set<CategoriaDAC>();
        public DbSet<ActividadDAC> ActividadesDAC
            => Set<ActividadDAC>();
        public DbSet<InscripcionDAC> InscripcionesDAC
            => Set<InscripcionDAC>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ==========================================
            // AUTH.ROLES
            // ==========================================
            modelBuilder.Entity<Rol>(entity =>
            {
                entity.ToTable("Roles", "auth");

                entity.HasKey(e => e.IdRol);

                entity.Property(e => e.IdRol)
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.NombreRol)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .IsRequired();

                entity.Property(e => e.Descripcion)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.HasIndex(e => e.NombreRol);
            });

            // ==========================================
            // AUTH.USUARIOS
            // ==========================================
            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.ToTable("Usuarios", "auth");

                entity.HasKey(e => e.Carnet);

                entity.Property(e => e.Carnet)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .ValueGeneratedNever();

                entity.Property(e => e.NombreCompleto)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .IsRequired();

                entity.Property(e => e.Carrera)
                    .HasMaxLength(150)
                    .IsUnicode(false)
                    .IsRequired();

                entity.Property(e => e.Campus)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .IsRequired();

                entity.Property(e => e.Activo)
                    .HasDefaultValue(true)
                    .IsRequired();

                entity.Property(e => e.CorreoElectronico)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .IsRequired();

                entity.HasOne(e => e.Rol)
                    .WithMany(r => r.Usuarios)
                    .HasForeignKey(e => e.IdRol)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            // ==========================================
            // CLUBS.ASOCIACIONES
            // ==========================================
            modelBuilder.Entity<Asociacion>(entity =>
            {
                entity.ToTable("Asociaciones", "clubs");

                entity.HasKey(e => e.IdAsociacion);

                entity.Property(e => e.IdAsociacion)
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Acronimo)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .IsRequired();

                entity.Property(e => e.Nombre)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .IsRequired();

                entity.Property(e => e.Descripcion)
                    .HasColumnType("nvarchar(max)");

                entity.Property(e => e.Formulario)
                    .HasColumnType("nvarchar(max)");

                entity.Property(e => e.ImgUrl)
                    .HasColumnType("nvarchar(max)");
            });

            // ==========================================
            // CLUBS.MEMBRESIAASOCIACIONES
            // ==========================================
            modelBuilder.Entity<MembresiaAsociacion>(entity =>
            {
                entity.ToTable("MembresiaAsociaciones", "clubs", t =>
                {
                    t.HasCheckConstraint(
                        "CK_RolMembresia",
                        "[Rol] IN ('Miembro','Directiva','Presidente','Tesorero','Secretario')");

                    t.HasCheckConstraint(
                        "CK_EstadoValidacion",
                        "[EstadoValidacion] IN ('Pendiente','Aprobada','Rechazada')");
                });

                entity.HasKey(e => e.IdMembresia);

                entity.Property(e => e.IdMembresia)
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Carnet)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .IsRequired();

                entity.Property(e => e.Rol)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValue("Miembro");

                entity.Property(e => e.FechaSolicitud)
                    .HasColumnType("date")
                    .HasDefaultValueSql("GETDATE()");

                entity.Property(e => e.EstadoValidacion)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasDefaultValue("Pendiente");

                entity.HasIndex(e => new
                {
                    e.Carnet,
                    e.IdAsociacion
                })
                .IsUnique()
                .HasDatabaseName("UQ_Estudiante_Asociacion");

                entity.HasOne(e => e.Usuario)
                    .WithMany(u => u.MembresiasAsociaciones)
                    .HasForeignKey(e => e.Carnet)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(e => e.Asociacion)
                    .WithMany(a => a.Membresias)
                    .HasForeignKey(e => e.IdAsociacion)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            // ==========================================
            // COURSES.CURSOS
            // ==========================================
            modelBuilder.Entity<Curso>(entity =>
            {
                entity.ToTable("Cursos", "courses", t =>
                {
                    t.HasCheckConstraint(
                        "CK_FechasCurso",
                        "[FechaFin] >= [FechaInicio]");

                    t.HasCheckConstraint(
                        "CK_EstadoCurso",
                        "[Estado] IN ('Activo','Inactivo','Finalizado')");

                    t.HasCheckConstraint(
                        "CK_Curso_InstructorValido",
                        "[CarnetInstructor] IS NULL OR auth.fn_ValidarInstructor([CarnetInstructor]) = 1");
                });

                entity.HasKey(e => e.IdCurso);

                entity.Property(e => e.IdCurso)
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.NombreCurso)
                    .HasMaxLength(150)
                    .IsUnicode(false)
                    .IsRequired();

                entity.Property(e => e.Descripcion)
                    .HasMaxLength(500)
                    .IsUnicode(true);

                entity.Property(e => e.FechaInicio)
                    .HasColumnType("date")
                    .IsRequired();

                entity.Property(e => e.FechaFin)
                    .HasColumnType("date")
                    .IsRequired();

                entity.Property(e => e.Horario)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .IsRequired();

                entity.Property(e => e.CupoMaximo)
                    .IsRequired();

                entity.Property(e => e.Estado)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasDefaultValue("Activo");

                entity.Property(e => e.CarnetInstructor)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.HasOne(e => e.Instructor)
                    .WithMany(u => u.CursosImpartidos)
                    .HasForeignKey(e => e.CarnetInstructor)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            // ==========================================
            // COURSES.INSCRIPCIONCURSOS
            // ==========================================
            modelBuilder.Entity<InscripcionCurso>(entity =>
            {
                entity.ToTable("InscripcionCursos", "courses", t =>
                {
                    t.HasCheckConstraint(
                        "CK_EstadoInscripcion",
                        "[EstadoInscripcion] IN ('Confirmada','En Espera','Cancelada')");
                });

                entity.HasKey(e => e.IdInscripcion);

                entity.Property(e => e.IdInscripcion)
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Carnet)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .IsRequired();

                entity.Property(e => e.Nota)
                    .HasPrecision(4, 2);

                entity.Property(e => e.FechaInscripcion)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("GETDATE()");

                entity.Property(e => e.EstadoInscripcion)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasDefaultValue("Confirmada");

                entity.HasIndex(e => new
                {
                    e.Carnet,
                    e.IdCurso
                })
                .IsUnique()
                .HasDatabaseName("UQ_Estudiante_Curso");

                entity.HasOne(e => e.Usuario)
                    .WithMany(u => u.InscripcionesCursos)
                    .HasForeignKey(e => e.Carnet)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(e => e.Curso)
                    .WithMany(c => c.Inscripciones)
                    .HasForeignKey(e => e.IdCurso)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            // ==========================================
            // SPORTS.SELECCIONESDEPORTIVAS
            // ==========================================
            modelBuilder.Entity<SeleccionDeportiva>(entity =>
            {
                entity.ToTable("SeleccionesDeportivas", "sports");

                entity.HasKey(e => e.IdSeleccion);

                entity.Property(e => e.IdSeleccion)
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Disciplina)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .IsRequired();

                entity.Property(e => e.Rama)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .IsRequired();

                entity.Property(e => e.NombreEquipo)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .IsRequired();

                entity.Property(e => e.ImgUrl)
                    .HasColumnType("nvarchar(max)");
            });

            // ==========================================
            // SPORTS.PARTICIPACIONDEPORTIVA
            // ==========================================
            modelBuilder.Entity<ParticipacionDeportiva>(entity =>
            {
                entity.ToTable("ParticipacionDeportiva", "sports");

                entity.HasKey(e => e.IdParticipacion);

                entity.Property(e => e.IdParticipacion)
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Carnet)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .IsRequired();

                entity.Property(e => e.Modalidad)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.FechaIngreso)
                    .HasColumnType("date")
                    .HasDefaultValueSql("GETDATE()");

                entity.HasIndex(e => new
                {
                    e.Carnet,
                    e.IdSeleccion
                })
                .IsUnique()
                .HasDatabaseName("UQ_Estudiante_Seleccion");

                entity.HasOne(e => e.Usuario)
                    .WithMany(u => u.ParticipacionesDeportivas)
                    .HasForeignKey(e => e.Carnet)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(e => e.SeleccionDeportiva)
                    .WithMany(s => s.Participaciones)
                    .HasForeignKey(e => e.IdSeleccion)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            // ==========================================
            // DAC.CATEGORIASDAC
            // ==========================================
            modelBuilder.Entity<CategoriaDAC>(entity =>
            {
                entity.ToTable("CategoriasDAC", "dac");

                entity.HasKey(e => e.IdCategoria);

                entity.Property(e => e.IdCategoria)
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.NombreCategoria)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .IsRequired();

                entity.Property(e => e.Descripcion)
                    .HasMaxLength(500)
                    .IsUnicode(true);
            });

            // ==========================================
            // DAC.ACTIVIDADESDAC
            // ==========================================
            modelBuilder.Entity<ActividadDAC>(entity =>
            {
                entity.ToTable("ActividadesDAC", "dac", t =>
                {
                    t.HasCheckConstraint(
                        "CK_EstadoActividadDAC",
                        "[Estado] IN ('Activo','Inactivo')");

                    t.HasCheckConstraint(
                        "CK_Actividad_InstructorValido",
                        "[CarnetInstructor] IS NULL OR auth.fn_ValidarInstructor([CarnetInstructor]) = 1");
                });

                entity.HasKey(e => e.IdActividad);

                entity.Property(e => e.IdActividad)
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.NombreActividad)
                    .HasMaxLength(150)
                    .IsUnicode(false)
                    .IsRequired();

                entity.Property(e => e.Descripcion)
                    .HasColumnType("nvarchar(max)");

                entity.Property(e => e.Estado)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasDefaultValue("Activo");

                entity.Property(e => e.CarnetInstructor)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.HasOne(e => e.Categoria)
                    .WithMany(c => c.Actividades)
                    .HasForeignKey(e => e.IdCategoria)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(e => e.Instructor)
                    .WithMany(u => u.ActividadesDACImpartidas)
                    .HasForeignKey(e => e.CarnetInstructor)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            // ==========================================
            // DAC.INSCRIPCIONESDAC
            // ==========================================
            modelBuilder.Entity<InscripcionDAC>(entity =>
            {
                entity.ToTable("InscripcionesDAC", "dac", t =>
                {
                    t.HasCheckConstraint(
                        "CK_EstadoInscripcionDAC",
                        "[EstadoInscripcion] IN ('Activa','Finalizada','Retirada')");
                });

                entity.HasKey(e => e.IdInscripcionDAC);

                entity.Property(e => e.IdInscripcionDAC)
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Carnet)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .IsRequired();

                entity.Property(e => e.FechaInscripcion)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("GETDATE()");

                entity.Property(e => e.EstadoInscripcion)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasDefaultValue("Activa");

                entity.HasIndex(e => new
                {
                    e.Carnet,
                    e.IdActividad
                })
                .IsUnique()
                .HasDatabaseName("UQ_Estudiante_ActividadDAC");

                entity.HasOne(e => e.Usuario)
                    .WithMany(u => u.InscripcionesDAC)
                    .HasForeignKey(e => e.Carnet)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(e => e.Actividad)
                    .WithMany(a => a.Inscripciones)
                    .HasForeignKey(e => e.IdActividad)
                    .OnDelete(DeleteBehavior.NoAction);
            });
        }
    }
}
