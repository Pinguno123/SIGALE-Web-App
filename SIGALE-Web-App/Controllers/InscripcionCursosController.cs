using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using SIGALE_Web_App.Models;

namespace SIGALE_Web_App.Controllers
{
    public class InscripcionCursosController : Controller
    {
        private readonly string _connectionString;

        public InscripcionCursosController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Cadena de conexión 'DefaultConnection' no encontrada.");
        }

        // GET: /InscripcionCursos
        public async Task<IActionResult> Index()
        {
            var inscripciones = new List<InscripcionCurso>();

            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                const string query = @"
                    SELECT i.IdInscripcion, i.Carnet, u.NombreCompleto, i.IdCurso, c.NombreCurso, 
                           i.Nota, i.FechaInscripcion, i.EstadoInscripcion
                    FROM courses.InscripcionCursos i
                    INNER JOIN auth.Usuarios u ON i.Carnet = u.Carnet
                    INNER JOIN courses.Cursos c ON i.IdCurso = c.IdCurso
                    ORDER BY i.IdInscripcion ASC";

                using (var command = new SqlCommand(query, connection))
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        inscripciones.Add(new InscripcionCurso
                        {
                            IdInscripcion = reader.GetInt32(0),
                            Carnet = reader.GetString(1),
                            Usuario = new Usuario
                            {
                                Carnet = reader.GetString(1),
                                NombreCompleto = reader.GetString(2)
                            },
                            IdCurso = reader.GetInt32(3),
                            Curso = new Curso
                            {
                                IdCurso = reader.GetInt32(3),
                                NombreCurso = reader.GetString(4)
                            },
                            Nota = reader.IsDBNull(5) ? null : reader.GetDecimal(5),
                            FechaInscripcion = reader.IsDBNull(6) ? null : reader.GetDateTime(6),
                            EstadoInscripcion = reader.IsDBNull(7) ? "Confirmada" : reader.GetString(7)
                        });
                    }
                }

                ViewBag.Usuarios = await ObtenerUsuariosAsync(connection);
                ViewBag.Cursos = await ObtenerCursosAsync(connection);
            }
            catch (SqlException ex)
            {
                TempData["ErrorMessage"] = "Error al consultar inscripciones de cursos: " + ex.Message;
                ViewBag.Usuarios = new List<Usuario>();
                ViewBag.Cursos = new List<Curso>();
            }

            return View(inscripciones);
        }

        // POST: /InscripcionCursos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string carnet, int idCurso, decimal? nota, DateTime? fechaInscripcion, string estadoInscripcion)
        {
            if (string.IsNullOrWhiteSpace(carnet) || idCurso <= 0)
            {
                TempData["ErrorMessage"] = "Usuario y Curso son obligatorios.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var fInscripcion = (fechaInscripcion ?? DateTime.Today).Date;
                const string queryCurso = "SELECT FechaInicio, NombreCurso FROM courses.Cursos WHERE IdCurso = @IdCurso";
                using (var cmdCurso = new SqlCommand(queryCurso, connection))
                {
                    cmdCurso.Parameters.AddWithValue("@IdCurso", idCurso);
                    using var readerCurso = await cmdCurso.ExecuteReaderAsync();
                    if (await readerCurso.ReadAsync())
                    {
                        var fechaInicioCurso = readerCurso.GetDateTime(0).Date;
                        var nombreCurso = readerCurso.GetString(1);
                        if (fInscripcion >= fechaInicioCurso)
                        {
                            TempData["ErrorMessage"] = $"Solo es posible inscribirse antes de que empiece el curso. El curso '{nombreCurso}' inicia el {fechaInicioCurso:dd/MM/yyyy}.";
                            return RedirectToAction(nameof(Index));
                        }
                    }
                }

                const string query = @"
                    INSERT INTO courses.InscripcionCursos (Carnet, IdCurso, Nota, FechaInscripcion, EstadoInscripcion)
                    VALUES (@Carnet, @IdCurso, @Nota, @FechaInscripcion, @EstadoInscripcion)";

                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Carnet", carnet.Trim().ToUpper());
                command.Parameters.AddWithValue("@IdCurso", idCurso);
                command.Parameters.AddWithValue("@Nota", (object?)nota ?? DBNull.Value);
                command.Parameters.AddWithValue("@FechaInscripcion", fInscripcion);
                command.Parameters.AddWithValue("@EstadoInscripcion", string.IsNullOrWhiteSpace(estadoInscripcion) ? "Confirmada" : estadoInscripcion.Trim());

                await command.ExecuteNonQueryAsync();
                TempData["SuccessMessage"] = "Inscripción a curso creada con éxito.";
            }
            catch (SqlException ex)
            {
                TempData["ErrorMessage"] = "Error al registrar la inscripción: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: /InscripcionCursos/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int idInscripcion, string carnet, int idCurso, decimal? nota, DateTime? fechaInscripcion, string estadoInscripcion)
        {
            if (idInscripcion <= 0 || string.IsNullOrWhiteSpace(carnet) || idCurso <= 0)
            {
                TempData["ErrorMessage"] = "Datos incompletos para actualizar la inscripción.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                if (fechaInscripcion.HasValue)
                {
                    const string queryCurso = "SELECT FechaInicio, NombreCurso FROM courses.Cursos WHERE IdCurso = @IdCurso";
                    using (var cmdCurso = new SqlCommand(queryCurso, connection))
                    {
                        cmdCurso.Parameters.AddWithValue("@IdCurso", idCurso);
                        using var readerCurso = await cmdCurso.ExecuteReaderAsync();
                        if (await readerCurso.ReadAsync())
                        {
                            var fechaInicioCurso = readerCurso.GetDateTime(0).Date;
                            var nombreCurso = readerCurso.GetString(1);
                            if (fechaInscripcion.Value.Date >= fechaInicioCurso)
                            {
                                TempData["ErrorMessage"] = $"Solo es posible inscribirse antes de que empiece el curso. El curso '{nombreCurso}' inicia el {fechaInicioCurso:dd/MM/yyyy}.";
                                return RedirectToAction(nameof(Index));
                            }
                        }
                    }
                }

                const string query = @"
                    UPDATE courses.InscripcionCursos
                    SET Carnet = @Carnet,
                        IdCurso = @IdCurso,
                        Nota = @Nota,
                        FechaInscripcion = @FechaInscripcion,
                        EstadoInscripcion = @EstadoInscripcion
                    WHERE IdInscripcion = @IdInscripcion";

                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@IdInscripcion", idInscripcion);
                command.Parameters.AddWithValue("@Carnet", carnet.Trim().ToUpper());
                command.Parameters.AddWithValue("@IdCurso", idCurso);
                command.Parameters.AddWithValue("@Nota", (object?)nota ?? DBNull.Value);
                command.Parameters.AddWithValue("@FechaInscripcion", (object?)fechaInscripcion?.Date ?? DBNull.Value);
                command.Parameters.AddWithValue("@EstadoInscripcion", string.IsNullOrWhiteSpace(estadoInscripcion) ? "Confirmada" : estadoInscripcion.Trim());

                var rows = await command.ExecuteNonQueryAsync();
                if (rows > 0)
                {
                    TempData["SuccessMessage"] = "Inscripción actualizada con éxito.";
                }
                else
                {
                    TempData["ErrorMessage"] = "No se encontró la inscripción a actualizar.";
                }
            }
            catch (SqlException ex)
            {
                TempData["ErrorMessage"] = "Error al actualizar la inscripción: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: /InscripcionCursos/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int idInscripcion)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                const string query = "DELETE FROM courses.InscripcionCursos WHERE IdInscripcion = @IdInscripcion";
                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@IdInscripcion", idInscripcion);

                var rows = await command.ExecuteNonQueryAsync();
                if (rows > 0)
                {
                    TempData["SuccessMessage"] = "Inscripción eliminada correctamente.";
                }
                else
                {
                    TempData["ErrorMessage"] = "No se encontró la inscripción a eliminar.";
                }
            }
            catch (SqlException ex)
            {
                TempData["ErrorMessage"] = "Error al eliminar la inscripción: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        private static async Task<List<Usuario>> ObtenerUsuariosAsync(SqlConnection connection)
        {
            var list = new List<Usuario>();
            const string query = "SELECT Carnet, NombreCompleto FROM auth.Usuarios ORDER BY NombreCompleto ASC";
            using var command = new SqlCommand(query, connection);
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(new Usuario { Carnet = reader.GetString(0), NombreCompleto = reader.GetString(1) });
            }
            return list;
        }

        private static async Task<List<Curso>> ObtenerCursosAsync(SqlConnection connection)
        {
            var list = new List<Curso>();
            const string query = "SELECT IdCurso, NombreCurso, FechaInicio FROM courses.Cursos ORDER BY NombreCurso ASC";
            using var command = new SqlCommand(query, connection);
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(new Curso 
                { 
                    IdCurso = reader.GetInt32(0), 
                    NombreCurso = reader.GetString(1),
                    FechaInicio = DateOnly.FromDateTime(reader.GetDateTime(2))
                });
            }
            return list;
        }
    }
}
