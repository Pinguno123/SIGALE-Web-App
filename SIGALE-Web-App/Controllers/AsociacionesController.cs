using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using SIGALE_Web_App.Models;

namespace SIGALE_Web_App.Controllers
{
    public class AsociacionesController : Controller
    {
        private readonly string _connectionString;

        public AsociacionesController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Cadena de conexión 'DefaultConnection' no encontrada.");
        }

        // GET: /Asociaciones
        public async Task<IActionResult> Index()
        {
            var asociaciones = new List<Asociacion>();

            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                const string query = @"
                    SELECT IdAsociacion, Acronimo, Nombre, AnioFundacion, Descripcion, Formulario, ImgUrl
                    FROM clubs.Asociaciones
                    ORDER BY Nombre ASC";

                using var command = new SqlCommand(query, connection);
                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    asociaciones.Add(new Asociacion
                    {
                        IdAsociacion = reader.GetInt32(0),
                        Acronimo = reader.GetString(1),
                        Nombre = reader.GetString(2),
                        AnioFundacion = reader.IsDBNull(3) ? null : reader.GetInt32(3),
                        Descripcion = reader.IsDBNull(4) ? null : reader.GetString(4),
                        Formulario = reader.IsDBNull(5) ? null : reader.GetString(5),
                        ImgUrl = reader.IsDBNull(6) ? null : reader.GetString(6)
                    });
                }
            }
            catch (SqlException ex)
            {
                TempData["ErrorMessage"] = "Error al consultar las asociaciones: " + ex.Message;
            }

            return View(asociaciones);
        }

        // POST: /Asociaciones/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string acronimo, string nombre, int? anioFundacion, string? descripcion, string? formulario, string? imgUrl)
        {
            if (string.IsNullOrWhiteSpace(acronimo) || string.IsNullOrWhiteSpace(nombre))
            {
                TempData["ErrorMessage"] = "Acrónimo y Nombre son requeridos.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                const string query = @"
                    INSERT INTO clubs.Asociaciones (Acronimo, Nombre, AnioFundacion, Descripcion, Formulario, ImgUrl)
                    VALUES (@Acronimo, @Nombre, @AnioFundacion, @Descripcion, @Formulario, @ImgUrl)";

                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Acronimo", acronimo.Trim());
                command.Parameters.AddWithValue("@Nombre", nombre.Trim());
                command.Parameters.AddWithValue("@AnioFundacion", (object?)anioFundacion ?? DBNull.Value);
                command.Parameters.AddWithValue("@Descripcion", (object?)descripcion?.Trim() ?? DBNull.Value);
                command.Parameters.AddWithValue("@Formulario", (object?)formulario?.Trim() ?? DBNull.Value);
                command.Parameters.AddWithValue("@ImgUrl", (object?)imgUrl?.Trim() ?? DBNull.Value);

                await command.ExecuteNonQueryAsync();
                TempData["SuccessMessage"] = "Asociación creada exitosamente.";
            }
            catch (SqlException ex)
            {
                TempData["ErrorMessage"] = "Error al registrar la asociación: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: /Asociaciones/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int idAsociacion, string acronimo, string nombre, int? anioFundacion, string? descripcion, string? formulario, string? imgUrl)
        {
            if (string.IsNullOrWhiteSpace(acronimo) || string.IsNullOrWhiteSpace(nombre))
            {
                TempData["ErrorMessage"] = "Acrónimo y Nombre son requeridos.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                const string query = @"
                    UPDATE clubs.Asociaciones
                    SET Acronimo = @Acronimo,
                        Nombre = @Nombre,
                        AnioFundacion = @AnioFundacion,
                        Descripcion = @Descripcion,
                        Formulario = @Formulario,
                        ImgUrl = @ImgUrl
                    WHERE IdAsociacion = @IdAsociacion";

                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@IdAsociacion", idAsociacion);
                command.Parameters.AddWithValue("@Acronimo", acronimo.Trim());
                command.Parameters.AddWithValue("@Nombre", nombre.Trim());
                command.Parameters.AddWithValue("@AnioFundacion", (object?)anioFundacion ?? DBNull.Value);
                command.Parameters.AddWithValue("@Descripcion", (object?)descripcion?.Trim() ?? DBNull.Value);
                command.Parameters.AddWithValue("@Formulario", (object?)formulario?.Trim() ?? DBNull.Value);
                command.Parameters.AddWithValue("@ImgUrl", (object?)imgUrl?.Trim() ?? DBNull.Value);

                var rows = await command.ExecuteNonQueryAsync();
                if (rows > 0)
                {
                    TempData["SuccessMessage"] = "Asociación actualizada exitosamente.";
                }
                else
                {
                    TempData["ErrorMessage"] = "No se encontró la asociación a editar.";
                }
            }
            catch (SqlException ex)
            {
                TempData["ErrorMessage"] = "Error al actualizar la asociación: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: /Asociaciones/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int idAsociacion)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                const string query = "DELETE FROM clubs.Asociaciones WHERE IdAsociacion = @IdAsociacion";
                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@IdAsociacion", idAsociacion);

                var rows = await command.ExecuteNonQueryAsync();
                if (rows > 0)
                {
                    TempData["SuccessMessage"] = "Asociación eliminada exitosamente.";
                }
                else
                {
                    TempData["ErrorMessage"] = "No se encontró la asociación a eliminar.";
                }
            }
            catch (SqlException ex)
            {
                TempData["ErrorMessage"] = "Error al eliminar asociación (puede tener miembros vinculados): " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
