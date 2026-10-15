using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using SIGALE_Web_App.Models;

namespace SIGALE_Web_App.Controllers
{
    public class ParticipacionDeportivaController : Controller
    {
        private readonly string _connectionString;

        public ParticipacionDeportivaController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Cadena de conexión 'DefaultConnection' no encontrada.");
        }

        // GET: /ParticipacionDeportiva
        public async Task<IActionResult> Index()
        {
            var participaciones = new List<ParticipacionDeportiva>();

            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                const string query = @"
                    SELECT p.IdParticipacion, p.Carnet, u.NombreCompleto, p.IdSeleccion, s.NombreEquipo, 
                           p.Modalidad, p.FechaIngreso
                    FROM sports.ParticipacionDeportiva p
                    INNER JOIN auth.Usuarios u ON p.Carnet = u.Carnet
                    INNER JOIN sports.SeleccionesDeportivas s ON p.IdSeleccion = s.IdSeleccion
                    ORDER BY p.IdParticipacion DESC";

                using (var command = new SqlCommand(query, connection))
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        participaciones.Add(new ParticipacionDeportiva
                        {
                            IdParticipacion = reader.GetInt32(0),
                            Carnet = reader.GetString(1),
                            Usuario = new Usuario
                            {
                                Carnet = reader.GetString(1),
                                NombreCompleto = reader.GetString(2)
                            },
                            IdSeleccion = reader.GetInt32(3),
                            SeleccionDeportiva = new SeleccionDeportiva
                            {
                                IdSeleccion = reader.GetInt32(3),
                                NombreEquipo = reader.GetString(4)
                            },
                            Modalidad = reader.IsDBNull(5) ? null : reader.GetString(5),
                            FechaIngreso = reader.IsDBNull(6) ? null : DateOnly.FromDateTime(reader.GetDateTime(6))
                        });
                    }
                }

                ViewBag.Usuarios = await ObtenerUsuariosAsync(connection);
                ViewBag.Selecciones = await ObtenerSeleccionesAsync(connection);
            }
            catch (SqlException ex)
            {
                TempData["ErrorMessage"] = "Error al consultar participaciones deportivas: " + ex.Message;
                ViewBag.Usuarios = new List<Usuario>();
                ViewBag.Selecciones = new List<SeleccionDeportiva>();
            }

            return View(participaciones);
        }

        // POST: /ParticipacionDeportiva/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string carnet, int idSeleccion, string? modalidad, DateTime? fechaIngreso)
        {
            if (string.IsNullOrWhiteSpace(carnet) || idSeleccion <= 0)
            {
                TempData["ErrorMessage"] = "Usuario y Selección Deportiva son obligatorios.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                const string query = @"
                    INSERT INTO sports.ParticipacionDeportiva (Carnet, IdSeleccion, Modalidad, FechaIngreso)
                    VALUES (@Carnet, @IdSeleccion, @Modalidad, @FechaIngreso)";

                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Carnet", carnet.Trim().ToUpper());
                command.Parameters.AddWithValue("@IdSeleccion", idSeleccion);
                command.Parameters.AddWithValue("@Modalidad", (object?)modalidad?.Trim() ?? DBNull.Value);
                command.Parameters.AddWithValue("@FechaIngreso", (object?)fechaIngreso?.Date ?? DateTime.Today);

                await command.ExecuteNonQueryAsync();
                TempData["SuccessMessage"] = "Participación registrada correctamente.";
            }
            catch (SqlException ex)
            {
                TempData["ErrorMessage"] = "Error al registrar participación: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: /ParticipacionDeportiva/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int idParticipacion, string carnet, int idSeleccion, string? modalidad, DateTime? fechaIngreso)
        {
            if (idParticipacion <= 0 || string.IsNullOrWhiteSpace(carnet) || idSeleccion <= 0)
            {
                TempData["ErrorMessage"] = "Datos incompletos para actualizar la participación.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                const string query = @"
                    UPDATE sports.ParticipacionDeportiva
                    SET Carnet = @Carnet,
                        IdSeleccion = @IdSeleccion,
                        Modalidad = @Modalidad,
                        FechaIngreso = @FechaIngreso
                    WHERE IdParticipacion = @IdParticipacion";

                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@IdParticipacion", idParticipacion);
                command.Parameters.AddWithValue("@Carnet", carnet.Trim().ToUpper());
                command.Parameters.AddWithValue("@IdSeleccion", idSeleccion);
                command.Parameters.AddWithValue("@Modalidad", (object?)modalidad?.Trim() ?? DBNull.Value);
                command.Parameters.AddWithValue("@FechaIngreso", (object?)fechaIngreso?.Date ?? DBNull.Value);

                var rows = await command.ExecuteNonQueryAsync();
                if (rows > 0)
                {
                    TempData["SuccessMessage"] = "Participación actualizada correctamente.";
                }
                else
                {
                    TempData["ErrorMessage"] = "No se encontró la participación a actualizar.";
                }
            }
            catch (SqlException ex)
            {
                TempData["ErrorMessage"] = "Error al actualizar la participación: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: /ParticipacionDeportiva/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int idParticipacion)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                const string query = "DELETE FROM sports.ParticipacionDeportiva WHERE IdParticipacion = @IdParticipacion";
                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@IdParticipacion", idParticipacion);

                var rows = await command.ExecuteNonQueryAsync();
                if (rows > 0)
                {
                    TempData["SuccessMessage"] = "Participación eliminada exitosamente.";
                }
                else
                {
                    TempData["ErrorMessage"] = "No se encontró la participación a eliminar.";
                }
            }
            catch (SqlException ex)
            {
                TempData["ErrorMessage"] = "Error al eliminar participación: " + ex.Message;
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

        private static async Task<List<SeleccionDeportiva>> ObtenerSeleccionesAsync(SqlConnection connection)
        {
            var list = new List<SeleccionDeportiva>();
            const string query = "SELECT IdSeleccion, NombreEquipo, Disciplina FROM sports.SeleccionesDeportivas ORDER BY NombreEquipo ASC";
            using var command = new SqlCommand(query, connection);
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(new SeleccionDeportiva
                {
                    IdSeleccion = reader.GetInt32(0),
                    NombreEquipo = $"{reader.GetString(1)} ({reader.GetString(2)})"
                });
            }
            return list;
        }
    }
}
