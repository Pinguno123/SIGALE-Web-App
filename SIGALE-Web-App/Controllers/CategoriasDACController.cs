using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using SIGALE_Web_App.Models;

namespace SIGALE_Web_App.Controllers
{
    public class CategoriasDACController : Controller
    {
        private readonly string _connectionString;

        public CategoriasDACController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Cadena de conexión 'DefaultConnection' no encontrada.");
        }

        // GET: /CategoriasDAC
        public async Task<IActionResult> Index()
        {
            var categorias = new List<CategoriaDAC>();

            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                const string query = @"
                    SELECT IdCategoria, NombreCategoria, Descripcion
                    FROM dac.CategoriasDAC
                    ORDER BY NombreCategoria ASC";

                using var command = new SqlCommand(query, connection);
                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    categorias.Add(new CategoriaDAC
                    {
                        IdCategoria = reader.GetInt32(0),
                        NombreCategoria = reader.GetString(1),
                        Descripcion = reader.IsDBNull(2) ? null : reader.GetString(2)
                    });
                }
            }
            catch (SqlException ex)
            {
                TempData["ErrorMessage"] = "Error al consultar las categorías DAC: " + ex.Message;
            }

            return View(categorias);
        }

        // POST: /CategoriasDAC/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string nombreCategoria, string? descripcion)
        {
            if (string.IsNullOrWhiteSpace(nombreCategoria))
            {
                TempData["ErrorMessage"] = "El nombre de la categoría es obligatorio.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                const string query = @"
                    INSERT INTO dac.CategoriasDAC (NombreCategoria, Descripcion)
                    VALUES (@NombreCategoria, @Descripcion)";

                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@NombreCategoria", nombreCategoria.Trim());
                command.Parameters.AddWithValue("@Descripcion", (object?)descripcion?.Trim() ?? DBNull.Value);

                await command.ExecuteNonQueryAsync();
                TempData["SuccessMessage"] = "Categoría DAC creada exitosamente.";
            }
            catch (SqlException ex)
            {
                TempData["ErrorMessage"] = "Error al registrar categoría: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: /CategoriasDAC/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int idCategoria, string nombreCategoria, string? descripcion)
        {
            if (idCategoria <= 0 || string.IsNullOrWhiteSpace(nombreCategoria))
            {
                TempData["ErrorMessage"] = "Datos incompletos para actualizar la categoría.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                const string query = @"
                    UPDATE dac.CategoriasDAC
                    SET NombreCategoria = @NombreCategoria,
                        Descripcion = @Descripcion
                    WHERE IdCategoria = @IdCategoria";

                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@IdCategoria", idCategoria);
                command.Parameters.AddWithValue("@NombreCategoria", nombreCategoria.Trim());
                command.Parameters.AddWithValue("@Descripcion", (object?)descripcion?.Trim() ?? DBNull.Value);

                var rows = await command.ExecuteNonQueryAsync();
                if (rows > 0)
                {
                    TempData["SuccessMessage"] = "Categoría DAC actualizada correctamente.";
                }
                else
                {
                    TempData["ErrorMessage"] = "No se encontró la categoría a actualizar.";
                }
            }
            catch (SqlException ex)
            {
                TempData["ErrorMessage"] = "Error al actualizar categoría: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: /CategoriasDAC/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int idCategoria)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                const string query = "DELETE FROM dac.CategoriasDAC WHERE IdCategoria = @IdCategoria";
                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@IdCategoria", idCategoria);

                var rows = await command.ExecuteNonQueryAsync();
                if (rows > 0)
                {
                    TempData["SuccessMessage"] = "Categoría DAC eliminada exitosamente.";
                }
                else
                {
                    TempData["ErrorMessage"] = "No se encontró la categoría a eliminar.";
                }
            }
            catch (SqlException ex)
            {
                TempData["ErrorMessage"] = "Error al eliminar categoría (puede tener actividades vinculadas): " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
