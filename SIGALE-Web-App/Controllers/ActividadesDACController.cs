using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using SIGALE_Web_App.Models;

namespace SIGALE_Web_App.Controllers
{
    public class ActividadesDACController : Controller
    {
        private readonly string _connectionString;

        public ActividadesDACController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Cadena de conexión 'DefaultConnection' no encontrada.");
        }

        // GET: /ActividadesDAC
        public async Task<IActionResult> Index()
        {
            var actividades = new List<ActividadDAC>();

            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                const string query = @"
                    SELECT a.IdActividad, a.IdCategoria, c.NombreCategoria, a.NombreActividad, 
                           a.Descripcion, a.Estado, a.CarnetInstructor, u.NombreCompleto AS NombreInstructor
                    FROM dac.ActividadesDAC a
                    INNER JOIN dac.CategoriasDAC c ON a.IdCategoria = c.IdCategoria
                    LEFT JOIN auth.Usuarios u ON a.CarnetInstructor = u.Carnet
                    ORDER BY a.IdActividad ASC";

                using (var command = new SqlCommand(query, connection))
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var instructorCarnet = reader.IsDBNull(6) ? null : reader.GetString(6);
                        var instructorNombre = reader.IsDBNull(7) ? null : reader.GetString(7);

                        actividades.Add(new ActividadDAC
                        {
                            IdActividad = reader.GetInt32(0),
                            IdCategoria = reader.GetInt32(1),
                            Categoria = new CategoriaDAC
                            {
                                IdCategoria = reader.GetInt32(1),
                                NombreCategoria = reader.GetString(2)
                            },
                            NombreActividad = reader.GetString(3),
                            Descripcion = reader.IsDBNull(4) ? null : reader.GetString(4),
                            Estado = reader.IsDBNull(5) ? "Activo" : reader.GetString(5),
                            CarnetInstructor = instructorCarnet,
                            Instructor = instructorCarnet != null ? new Usuario { Carnet = instructorCarnet, NombreCompleto = instructorNombre ?? "" } : null
                        });
                    }
                }

                ViewBag.Categorias = await ObtenerCategoriasAsync(connection);
                ViewBag.Instructores = await ObtenerInstructoresAsync(connection);
            }
            catch (SqlException ex)
            {
                TempData["ErrorMessage"] = "Error al consultar actividades DAC: " + ex.Message;
                ViewBag.Categorias = new List<CategoriaDAC>();
                ViewBag.Instructores = new List<Usuario>();
            }

            return View(actividades);
        }

        // POST: /ActividadesDAC/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int idCategoria, string nombreActividad, string? descripcion, string estado, string? carnetInstructor)
        {
            if (idCategoria <= 0 || string.IsNullOrWhiteSpace(nombreActividad))
            {
                TempData["ErrorMessage"] = "Categoría y Nombre de Actividad son obligatorios.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                const string query = @"
                    INSERT INTO dac.ActividadesDAC (IdCategoria, NombreActividad, Descripcion, Estado, CarnetInstructor)
                    VALUES (@IdCategoria, @NombreActividad, @Descripcion, @Estado, @CarnetInstructor)";

                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@IdCategoria", idCategoria);
                command.Parameters.AddWithValue("@NombreActividad", nombreActividad.Trim());
                command.Parameters.AddWithValue("@Descripcion", (object?)descripcion?.Trim() ?? DBNull.Value);
                command.Parameters.AddWithValue("@Estado", string.IsNullOrWhiteSpace(estado) ? "Activo" : estado.Trim());
                command.Parameters.AddWithValue("@CarnetInstructor", string.IsNullOrWhiteSpace(carnetInstructor) ? DBNull.Value : carnetInstructor.Trim().ToUpper());

                await command.ExecuteNonQueryAsync();
                TempData["SuccessMessage"] = "Actividad DAC registrada correctamente.";
            }
            catch (SqlException ex)
            {
                TempData["ErrorMessage"] = "Error al registrar actividad DAC: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: /ActividadesDAC/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int idActividad, int idCategoria, string nombreActividad, string? descripcion, string estado, string? carnetInstructor)
        {
            if (idActividad <= 0 || idCategoria <= 0 || string.IsNullOrWhiteSpace(nombreActividad))
            {
                TempData["ErrorMessage"] = "Datos incompletos para actualizar la actividad DAC.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                const string query = @"
                    UPDATE dac.ActividadesDAC
                    SET IdCategoria = @IdCategoria,
                        NombreActividad = @NombreActividad,
                        Descripcion = @Descripcion,
                        Estado = @Estado,
                        CarnetInstructor = @CarnetInstructor
                    WHERE IdActividad = @IdActividad";

                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@IdActividad", idActividad);
                command.Parameters.AddWithValue("@IdCategoria", idCategoria);
                command.Parameters.AddWithValue("@NombreActividad", nombreActividad.Trim());
                command.Parameters.AddWithValue("@Descripcion", (object?)descripcion?.Trim() ?? DBNull.Value);
                command.Parameters.AddWithValue("@Estado", string.IsNullOrWhiteSpace(estado) ? "Activo" : estado.Trim());
                command.Parameters.AddWithValue("@CarnetInstructor", string.IsNullOrWhiteSpace(carnetInstructor) ? DBNull.Value : carnetInstructor.Trim().ToUpper());

                var rows = await command.ExecuteNonQueryAsync();
                if (rows > 0)
                {
                    TempData["SuccessMessage"] = "Actividad DAC actualizada correctamente.";
                }
                else
                {
                    TempData["ErrorMessage"] = "No se encontró la actividad a actualizar.";
                }
            }
            catch (SqlException ex)
            {
                TempData["ErrorMessage"] = "Error al actualizar actividad DAC: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: /ActividadesDAC/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int idActividad)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                const string query = "DELETE FROM dac.ActividadesDAC WHERE IdActividad = @IdActividad";
                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@IdActividad", idActividad);

                var rows = await command.ExecuteNonQueryAsync();
                if (rows > 0)
                {
                    TempData["SuccessMessage"] = "Actividad DAC eliminada correctamente.";
                }
                else
                {
                    TempData["ErrorMessage"] = "No se encontró la actividad a eliminar.";
                }
            }
            catch (SqlException ex)
            {
                TempData["ErrorMessage"] = "Error al eliminar actividad (puede tener inscripciones vinculadas): " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        private static async Task<List<CategoriaDAC>> ObtenerCategoriasAsync(SqlConnection connection)
        {
            var list = new List<CategoriaDAC>();
            const string query = "SELECT IdCategoria, NombreCategoria FROM dac.CategoriasDAC ORDER BY NombreCategoria ASC";
            using var command = new SqlCommand(query, connection);
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(new CategoriaDAC { IdCategoria = reader.GetInt32(0), NombreCategoria = reader.GetString(1) });
            }
            return list;
        }

        private static async Task<List<Usuario>> ObtenerInstructoresAsync(SqlConnection connection)
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
    }
}
