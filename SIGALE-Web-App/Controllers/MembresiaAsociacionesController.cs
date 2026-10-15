using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using SIGALE_Web_App.Models;

namespace SIGALE_Web_App.Controllers
{
    public class MembresiaAsociacionesController : Controller
    {
        private readonly string _connectionString;

        public MembresiaAsociacionesController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Cadena de conexión 'DefaultConnection' no encontrada.");
        }

        // GET: /MembresiaAsociaciones
        public async Task<IActionResult> Index()
        {
            var membresias = new List<MembresiaAsociacion>();

            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                const string query = @"
                    SELECT m.IdMembresia, m.Carnet, u.NombreCompleto, m.IdAsociacion, a.Nombre AS NombreAsociacion, 
                           m.Rol, m.FechaSolicitud, m.EstadoValidacion
                    FROM clubs.MembresiaAsociaciones m
                    INNER JOIN auth.Usuarios u ON m.Carnet = u.Carnet
                    INNER JOIN clubs.Asociaciones a ON m.IdAsociacion = a.IdAsociacion
                    ORDER BY m.IdMembresia DESC";

                using (var command = new SqlCommand(query, connection))
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        membresias.Add(new MembresiaAsociacion
                        {
                            IdMembresia = reader.GetInt32(0),
                            Carnet = reader.GetString(1),
                            Usuario = new Usuario
                            {
                                Carnet = reader.GetString(1),
                                NombreCompleto = reader.GetString(2)
                            },
                            IdAsociacion = reader.GetInt32(3),
                            Asociacion = new Asociacion
                            {
                                IdAsociacion = reader.GetInt32(3),
                                Nombre = reader.GetString(4)
                            },
                            Rol = reader.IsDBNull(5) ? "Miembro" : reader.GetString(5),
                            FechaSolicitud = reader.IsDBNull(6) ? null : DateOnly.FromDateTime(reader.GetDateTime(6)),
                            EstadoValidacion = reader.IsDBNull(7) ? "Pendiente" : reader.GetString(7)
                        });
                    }
                }

                ViewBag.Usuarios = await ObtenerUsuariosAsync(connection);
                ViewBag.Asociaciones = await ObtenerAsociacionesAsync(connection);
            }
            catch (SqlException ex)
            {
                TempData["ErrorMessage"] = "Error al consultar membresías: " + ex.Message;
                ViewBag.Usuarios = new List<Usuario>();
                ViewBag.Asociaciones = new List<Asociacion>();
            }

            return View(membresias);
        }

        // POST: /MembresiaAsociaciones/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string carnet, int idAsociacion, string rol, DateTime? fechaSolicitud, string estadoValidacion)
        {
            if (string.IsNullOrWhiteSpace(carnet) || idAsociacion <= 0)
            {
                TempData["ErrorMessage"] = "Usuario y Asociación son requeridos.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                const string query = @"
                    INSERT INTO clubs.MembresiaAsociaciones (Carnet, IdAsociacion, Rol, FechaSolicitud, EstadoValidacion)
                    VALUES (@Carnet, @IdAsociacion, @Rol, @FechaSolicitud, @EstadoValidacion)";

                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Carnet", carnet.Trim().ToUpper());
                command.Parameters.AddWithValue("@IdAsociacion", idAsociacion);
                command.Parameters.AddWithValue("@Rol", string.IsNullOrWhiteSpace(rol) ? "Miembro" : rol.Trim());
                command.Parameters.AddWithValue("@FechaSolicitud", (object?)fechaSolicitud?.Date ?? DateTime.Today);
                command.Parameters.AddWithValue("@EstadoValidacion", string.IsNullOrWhiteSpace(estadoValidacion) ? "Pendiente" : estadoValidacion.Trim());

                await command.ExecuteNonQueryAsync();
                TempData["SuccessMessage"] = "Membresía registrada correctamente.";
            }
            catch (SqlException ex)
            {
                TempData["ErrorMessage"] = "Error al registrar membresía: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: /MembresiaAsociaciones/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int idMembresia, string carnet, int idAsociacion, string rol, DateTime? fechaSolicitud, string estadoValidacion)
        {
            if (idMembresia <= 0 || string.IsNullOrWhiteSpace(carnet) || idAsociacion <= 0)
            {
                TempData["ErrorMessage"] = "Datos incompletos para actualizar la membresía.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                const string query = @"
                    UPDATE clubs.MembresiaAsociaciones
                    SET Carnet = @Carnet,
                        IdAsociacion = @IdAsociacion,
                        Rol = @Rol,
                        FechaSolicitud = @FechaSolicitud,
                        EstadoValidacion = @EstadoValidacion
                    WHERE IdMembresia = @IdMembresia";

                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@IdMembresia", idMembresia);
                command.Parameters.AddWithValue("@Carnet", carnet.Trim().ToUpper());
                command.Parameters.AddWithValue("@IdAsociacion", idAsociacion);
                command.Parameters.AddWithValue("@Rol", string.IsNullOrWhiteSpace(rol) ? "Miembro" : rol.Trim());
                command.Parameters.AddWithValue("@FechaSolicitud", (object?)fechaSolicitud?.Date ?? DBNull.Value);
                command.Parameters.AddWithValue("@EstadoValidacion", string.IsNullOrWhiteSpace(estadoValidacion) ? "Pendiente" : estadoValidacion.Trim());

                var rows = await command.ExecuteNonQueryAsync();
                if (rows > 0)
                {
                    TempData["SuccessMessage"] = "Membresía actualizada con éxito.";
                }
                else
                {
                    TempData["ErrorMessage"] = "No se encontró la membresía a actualizar.";
                }
            }
            catch (SqlException ex)
            {
                TempData["ErrorMessage"] = "Error al actualizar la membresía: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: /MembresiaAsociaciones/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int idMembresia)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                const string query = "DELETE FROM clubs.MembresiaAsociaciones WHERE IdMembresia = @IdMembresia";
                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@IdMembresia", idMembresia);

                var rows = await command.ExecuteNonQueryAsync();
                if (rows > 0)
                {
                    TempData["SuccessMessage"] = "Membresía eliminada exitosamente.";
                }
                else
                {
                    TempData["ErrorMessage"] = "No se encontró la membresía a eliminar.";
                }
            }
            catch (SqlException ex)
            {
                TempData["ErrorMessage"] = "Error al eliminar la membresía: " + ex.Message;
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

        private static async Task<List<Asociacion>> ObtenerAsociacionesAsync(SqlConnection connection)
        {
            var list = new List<Asociacion>();
            const string query = "SELECT IdAsociacion, Nombre FROM clubs.Asociaciones ORDER BY Nombre ASC";
            using var command = new SqlCommand(query, connection);
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(new Asociacion { IdAsociacion = reader.GetInt32(0), Nombre = reader.GetString(1) });
            }
            return list;
        }
    }
}
