using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using SIGALE_Web_App.Models;

namespace SIGALE_Web_App.Controllers
{
    public class InscripcionesDACController : Controller
    {
        private readonly string _connectionString;

        public InscripcionesDACController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Cadena de conexión 'DefaultConnection' no encontrada.");
        }

        // GET: /InscripcionesDAC
        public async Task<IActionResult> Index()
        {
            var inscripciones = new List<InscripcionDAC>();

            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                const string query = @"
                    SELECT i.IdInscripcionDAC, i.Carnet, u.NombreCompleto, i.IdActividad, a.NombreActividad, 
                           i.FechaInscripcion, i.EstadoInscripcion
                    FROM dac.InscripcionesDAC i
                    INNER JOIN auth.Usuarios u ON i.Carnet = u.Carnet
                    INNER JOIN dac.ActividadesDAC a ON i.IdActividad = a.IdActividad
                    ORDER BY i.IdInscripcionDAC ASC";

                using (var command = new SqlCommand(query, connection))
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        inscripciones.Add(new InscripcionDAC
                        {
                            IdInscripcionDAC = reader.GetInt32(0),
                            Carnet = reader.GetString(1),
                            Usuario = new Usuario
                            {
                                Carnet = reader.GetString(1),
                                NombreCompleto = reader.GetString(2)
                            },
                            IdActividad = reader.GetInt32(3),
                            Actividad = new ActividadDAC
                            {
                                IdActividad = reader.GetInt32(3),
                                NombreActividad = reader.GetString(4)
                            },
                            FechaInscripcion = reader.IsDBNull(5) ? null : reader.GetDateTime(5),
                            EstadoInscripcion = reader.IsDBNull(6) ? "Activa" : reader.GetString(6)
                        });
                    }
                }

                ViewBag.Usuarios = await ObtenerUsuariosAsync(connection);
                ViewBag.Actividades = await ObtenerActividadesAsync(connection);
            }
            catch (SqlException ex)
            {
                TempData["ErrorMessage"] = "Error al consultar inscripciones DAC: " + ex.Message;
                ViewBag.Usuarios = new List<Usuario>();
                ViewBag.Actividades = new List<ActividadDAC>();
            }

            return View(inscripciones);
        }

        // POST: /InscripcionesDAC/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string carnet, int idActividad, DateTime? fechaInscripcion, string estadoInscripcion)
        {
            if (string.IsNullOrWhiteSpace(carnet) || idActividad <= 0)
            {
                TempData["ErrorMessage"] = "Usuario y Actividad son obligatorios.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                const string query = @"
                    INSERT INTO dac.InscripcionesDAC (Carnet, IdActividad, FechaInscripcion, EstadoInscripcion)
                    VALUES (@Carnet, @IdActividad, @FechaInscripcion, @EstadoInscripcion)";

                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Carnet", carnet.Trim().ToUpper());
                command.Parameters.AddWithValue("@IdActividad", idActividad);
                command.Parameters.AddWithValue("@FechaInscripcion", (object?)fechaInscripcion ?? DateTime.Now);
                command.Parameters.AddWithValue("@EstadoInscripcion", string.IsNullOrWhiteSpace(estadoInscripcion) ? "Activa" : estadoInscripcion.Trim());

                await command.ExecuteNonQueryAsync();
                TempData["SuccessMessage"] = "Inscripción DAC registrada correctamente.";
            }
            catch (SqlException ex)
            {
                TempData["ErrorMessage"] = "Error al registrar la inscripción: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: /InscripcionesDAC/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int idInscripcionDAC, string carnet, int idActividad, DateTime? fechaInscripcion, string estadoInscripcion)
        {
            if (idInscripcionDAC <= 0 || string.IsNullOrWhiteSpace(carnet) || idActividad <= 0)
            {
                TempData["ErrorMessage"] = "Datos incompletos para actualizar la inscripción DAC.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                const string query = @"
                    UPDATE dac.InscripcionesDAC
                    SET Carnet = @Carnet,
                        IdActividad = @IdActividad,
                        FechaInscripcion = @FechaInscripcion,
                        EstadoInscripcion = @EstadoInscripcion
                    WHERE IdInscripcionDAC = @IdInscripcionDAC";

                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@IdInscripcionDAC", idInscripcionDAC);
                command.Parameters.AddWithValue("@Carnet", carnet.Trim().ToUpper());
                command.Parameters.AddWithValue("@IdActividad", idActividad);
                command.Parameters.AddWithValue("@FechaInscripcion", (object?)fechaInscripcion ?? DBNull.Value);
                command.Parameters.AddWithValue("@EstadoInscripcion", string.IsNullOrWhiteSpace(estadoInscripcion) ? "Activa" : estadoInscripcion.Trim());

                var rows = await command.ExecuteNonQueryAsync();
                if (rows > 0)
                {
                    TempData["SuccessMessage"] = "Inscripción DAC actualizada con éxito.";
                }
                else
                {
                    TempData["ErrorMessage"] = "No se encontró la inscripción a actualizar.";
                }
            }
            catch (SqlException ex)
            {
                TempData["ErrorMessage"] = "Error al actualizar la inscripción DAC: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: /InscripcionesDAC/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int idInscripcionDAC)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                const string query = "DELETE FROM dac.InscripcionesDAC WHERE IdInscripcionDAC = @IdInscripcionDAC";
                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@IdInscripcionDAC", idInscripcionDAC);

                var rows = await command.ExecuteNonQueryAsync();
                if (rows > 0)
                {
                    TempData["SuccessMessage"] = "Inscripción DAC eliminada exitosamente.";
                }
                else
                {
                    TempData["ErrorMessage"] = "No se encontró la inscripción a eliminar.";
                }
            }
            catch (SqlException ex)
            {
                TempData["ErrorMessage"] = "Error al eliminar la inscripción DAC: " + ex.Message;
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

        private static async Task<List<ActividadDAC>> ObtenerActividadesAsync(SqlConnection connection)
        {
            var list = new List<ActividadDAC>();
            const string query = "SELECT IdActividad, NombreActividad FROM dac.ActividadesDAC ORDER BY NombreActividad ASC";
            using var command = new SqlCommand(query, connection);
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(new ActividadDAC { IdActividad = reader.GetInt32(0), NombreActividad = reader.GetString(1) });
            }
            return list;
        }
    }
}
