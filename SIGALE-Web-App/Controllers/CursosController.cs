using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using SIGALE_Web_App.Models;

namespace SIGALE_Web_App.Controllers
{
    public class CursosController : Controller
    {
        private readonly string _connectionString;

        public CursosController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Cadena de conexión 'DefaultConnection' no encontrada.");
        }

        // GET: /Cursos
        public async Task<IActionResult> Index()
        {
            var cursos = new List<Curso>();

            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                const string query = @"
                    SELECT c.IdCurso, c.NombreCurso, c.Descripcion, c.FechaInicio, c.FechaFin, 
                           c.Horario, c.CupoMaximo, c.Estado, c.CarnetInstructor, u.NombreCompleto AS NombreInstructor
                    FROM courses.Cursos c
                    LEFT JOIN auth.Usuarios u ON c.CarnetInstructor = u.Carnet
                    ORDER BY c.IdCurso ASC";

                using (var command = new SqlCommand(query, connection))
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var instructorCarnet = reader.IsDBNull(8) ? null : reader.GetString(8);
                        var instructorNombre = reader.IsDBNull(9) ? null : reader.GetString(9);

                        cursos.Add(new Curso
                        {
                            IdCurso = reader.GetInt32(0),
                            NombreCurso = reader.GetString(1),
                            Descripcion = reader.IsDBNull(2) ? null : reader.GetString(2),
                            FechaInicio = DateOnly.FromDateTime(reader.GetDateTime(3)),
                            FechaFin = DateOnly.FromDateTime(reader.GetDateTime(4)),
                            Horario = reader.GetString(5),
                            CupoMaximo = reader.GetInt32(6),
                            Estado = reader.IsDBNull(7) ? "Activo" : reader.GetString(7),
                            CarnetInstructor = instructorCarnet,
                            Instructor = instructorCarnet != null ? new Usuario { Carnet = instructorCarnet, NombreCompleto = instructorNombre ?? "" } : null
                        });
                    }
                }

                ViewBag.Instructores = await ObtenerInstructoresAsync(connection);
            }
            catch (SqlException ex)
            {
                TempData["ErrorMessage"] = "Error al consultar los cursos: " + ex.Message;
                ViewBag.Instructores = new List<Usuario>();
            }

            return View(cursos);
        }

        // POST: /Cursos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string nombreCurso, string? descripcion, DateTime fechaInicio, DateTime fechaFin, string horario, int cupoMaximo, string estado, string? carnetInstructor)
        {
            if (string.IsNullOrWhiteSpace(nombreCurso) || nombreCurso.Trim().Length < 5)
            {
                TempData["ErrorMessage"] = "El nombre del curso es obligatorio y debe tener al menos 5 caracteres.";
                return RedirectToAction(nameof(Index));
            }

            if (string.IsNullOrWhiteSpace(horario))
            {
                TempData["ErrorMessage"] = "El horario del curso es obligatorio.";
                return RedirectToAction(nameof(Index));
            }

            if (cupoMaximo <= 15)
            {
                TempData["ErrorMessage"] = "El cupo máximo debe ser mayor a 15 (mínimo 16).";
                return RedirectToAction(nameof(Index));
            }

            if (fechaFin.Date < fechaInicio.Date.AddDays(7))
            {
                TempData["ErrorMessage"] = "La fecha de fin del curso debe ser por lo menos una semana después de que inicie.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                if (!string.IsNullOrWhiteSpace(carnetInstructor))
                {
                    const string checkInstructor = "SELECT COUNT(1) FROM auth.Usuarios WHERE Carnet = @Carnet AND IdRol = 2";
                    using var checkCmd = new SqlCommand(checkInstructor, connection);
                    checkCmd.Parameters.AddWithValue("@Carnet", carnetInstructor.Trim().ToUpper());
                    var valido = (int)(await checkCmd.ExecuteScalarAsync() ?? 0) > 0;
                    if (!valido)
                    {
                        TempData["ErrorMessage"] = "El usuario seleccionado no cuenta con el rol de Instructor (IdRol = 2).";
                        return RedirectToAction(nameof(Index));
                    }
                }

                const string query = @"
                    INSERT INTO courses.Cursos (NombreCurso, Descripcion, FechaInicio, FechaFin, Horario, CupoMaximo, Estado, CarnetInstructor)
                    VALUES (@NombreCurso, @Descripcion, @FechaInicio, @FechaFin, @Horario, @CupoMaximo, @Estado, @CarnetInstructor)";

                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@NombreCurso", nombreCurso.Trim());
                command.Parameters.AddWithValue("@Descripcion", (object?)descripcion?.Trim() ?? DBNull.Value);
                command.Parameters.AddWithValue("@FechaInicio", fechaInicio.Date);
                command.Parameters.AddWithValue("@FechaFin", fechaFin.Date);
                command.Parameters.AddWithValue("@Horario", horario.Trim());
                command.Parameters.AddWithValue("@CupoMaximo", cupoMaximo);
                command.Parameters.AddWithValue("@Estado", string.IsNullOrWhiteSpace(estado) ? "Activo" : estado.Trim());
                command.Parameters.AddWithValue("@CarnetInstructor", string.IsNullOrWhiteSpace(carnetInstructor) ? DBNull.Value : carnetInstructor.Trim().ToUpper());

                await command.ExecuteNonQueryAsync();
                TempData["SuccessMessage"] = "Curso registrado con éxito.";
            }
            catch (SqlException ex)
            {
                TempData["ErrorMessage"] = "Error al registrar curso: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: /Cursos/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int idCurso, string nombreCurso, string? descripcion, DateTime fechaInicio, DateTime fechaFin, string horario, int cupoMaximo, string estado, string? carnetInstructor)
        {
            if (idCurso <= 0 || string.IsNullOrWhiteSpace(nombreCurso) || nombreCurso.Trim().Length < 5)
            {
                TempData["ErrorMessage"] = "El nombre del curso es obligatorio y debe tener al menos 5 caracteres.";
                return RedirectToAction(nameof(Index));
            }

            if (string.IsNullOrWhiteSpace(horario))
            {
                TempData["ErrorMessage"] = "El horario del curso es obligatorio.";
                return RedirectToAction(nameof(Index));
            }

            if (cupoMaximo <= 15)
            {
                TempData["ErrorMessage"] = "El cupo máximo debe ser mayor a 15 (mínimo 16).";
                return RedirectToAction(nameof(Index));
            }

            if (fechaFin.Date < fechaInicio.Date.AddDays(7))
            {
                TempData["ErrorMessage"] = "La fecha de fin del curso debe ser por lo menos una semana después de que inicie.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                if (!string.IsNullOrWhiteSpace(carnetInstructor))
                {
                    const string checkInstructor = "SELECT COUNT(1) FROM auth.Usuarios WHERE Carnet = @Carnet AND IdRol = 2";
                    using var checkCmd = new SqlCommand(checkInstructor, connection);
                    checkCmd.Parameters.AddWithValue("@Carnet", carnetInstructor.Trim().ToUpper());
                    var valido = (int)(await checkCmd.ExecuteScalarAsync() ?? 0) > 0;
                    if (!valido)
                    {
                        TempData["ErrorMessage"] = "El usuario seleccionado no cuenta con el rol de Instructor (IdRol = 2).";
                        return RedirectToAction(nameof(Index));
                    }
                }

                const string query = @"
                    UPDATE courses.Cursos
                    SET NombreCurso = @NombreCurso,
                        Descripcion = @Descripcion,
                        FechaInicio = @FechaInicio,
                        FechaFin = @FechaFin,
                        Horario = @Horario,
                        CupoMaximo = @CupoMaximo,
                        Estado = @Estado,
                        CarnetInstructor = @CarnetInstructor
                    WHERE IdCurso = @IdCurso";

                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@IdCurso", idCurso);
                command.Parameters.AddWithValue("@NombreCurso", nombreCurso.Trim());
                command.Parameters.AddWithValue("@Descripcion", (object?)descripcion?.Trim() ?? DBNull.Value);
                command.Parameters.AddWithValue("@FechaInicio", fechaInicio.Date);
                command.Parameters.AddWithValue("@FechaFin", fechaFin.Date);
                command.Parameters.AddWithValue("@Horario", horario.Trim());
                command.Parameters.AddWithValue("@CupoMaximo", cupoMaximo);
                command.Parameters.AddWithValue("@Estado", string.IsNullOrWhiteSpace(estado) ? "Activo" : estado.Trim());
                command.Parameters.AddWithValue("@CarnetInstructor", string.IsNullOrWhiteSpace(carnetInstructor) ? DBNull.Value : carnetInstructor.Trim().ToUpper());

                var rows = await command.ExecuteNonQueryAsync();
                if (rows > 0)
                {
                    TempData["SuccessMessage"] = "Curso actualizado con éxito.";
                }
                else
                {
                    TempData["ErrorMessage"] = "No se encontró el curso a actualizar.";
                }
            }
            catch (SqlException ex)
            {
                TempData["ErrorMessage"] = "Error al actualizar curso: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: /Cursos/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int idCurso)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                const string query = "DELETE FROM courses.Cursos WHERE IdCurso = @IdCurso";
                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@IdCurso", idCurso);

                var rows = await command.ExecuteNonQueryAsync();
                if (rows > 0)
                {
                    TempData["SuccessMessage"] = "Curso eliminado exitosamente.";
                }
                else
                {
                    TempData["ErrorMessage"] = "No se encontró el curso a eliminar.";
                }
            }
            catch (SqlException ex)
            {
                TempData["ErrorMessage"] = "Error al eliminar curso (puede tener inscripciones asociadas): " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        private static async Task<List<Usuario>> ObtenerInstructoresAsync(SqlConnection connection)
        {
            var list = new List<Usuario>();
            const string query = "SELECT Carnet, NombreCompleto FROM auth.Usuarios WHERE IdRol = 2 ORDER BY NombreCompleto ASC";
            using var command = new SqlCommand(query, connection);
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(new Usuario { Carnet = reader.GetString(0), NombreCompleto = reader.GetString(1) });
            }
            return list;
        }
    }
}
