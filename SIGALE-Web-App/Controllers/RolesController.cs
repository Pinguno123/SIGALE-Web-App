using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using SIGALE_Web_App.Models;

namespace SIGALE_Web_App.Controllers
{
    public class RolesController : Controller
    {
        private readonly string _connectionString;

        public RolesController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Cadena de conexión 'DefaultConnection' no encontrada.");
        }

        // GET: /Roles
        public async Task<IActionResult> Index()
        {
            var roles = new List<Rol>();

            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                const string query = "SELECT IdRol, NombreRol, Descripcion FROM auth.Roles ORDER BY IdRol ASC";
                using var command = new SqlCommand(query, connection);
                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    roles.Add(new Rol
                    {
                        IdRol = reader.GetInt32(0),
                        NombreRol = reader.GetString(1),
                        Descripcion = reader.IsDBNull(2) ? null : reader.GetString(2)
                    });
                }
            }
            catch (SqlException ex)
            {
                TempData["ErrorMessage"] = "Error al consultar los roles: " + ex.Message;
            }

            return View(roles);
        }

        // POST: /Roles/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string nombreRol, string? descripcion)
        {
            if (string.IsNullOrWhiteSpace(nombreRol))
            {
                TempData["ErrorMessage"] = "El nombre del rol es requerido.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                const string query = "INSERT INTO auth.Roles (NombreRol, Descripcion) VALUES (@NombreRol, @Descripcion)";
                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@NombreRol", nombreRol.Trim());
                command.Parameters.AddWithValue("@Descripcion", (object?)descripcion?.Trim() ?? DBNull.Value);

                await command.ExecuteNonQueryAsync();
                TempData["SuccessMessage"] = "Rol creado correctamente.";
            }
            catch (SqlException ex)
            {
                TempData["ErrorMessage"] = "Error al insertar el rol: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: /Roles/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int idRol, string nombreRol, string? descripcion)
        {
            if (string.IsNullOrWhiteSpace(nombreRol))
            {
                TempData["ErrorMessage"] = "El nombre del rol es requerido.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                const string query = "UPDATE auth.Roles SET NombreRol = @NombreRol, Descripcion = @Descripcion WHERE IdRol = @IdRol";
                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@IdRol", idRol);
                command.Parameters.AddWithValue("@NombreRol", nombreRol.Trim());
                command.Parameters.AddWithValue("@Descripcion", (object?)descripcion?.Trim() ?? DBNull.Value);

                var rowsAffected = await command.ExecuteNonQueryAsync();
                if (rowsAffected > 0)
                {
                    TempData["SuccessMessage"] = "Rol actualizado correctamente.";
                }
                else
                {
                    TempData["ErrorMessage"] = "No se encontró el rol a actualizar.";
                }
            }
            catch (SqlException ex)
            {
                TempData["ErrorMessage"] = "Error al actualizar el rol: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: /Roles/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int idRol)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                const string query = "DELETE FROM auth.Roles WHERE IdRol = @IdRol";
                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@IdRol", idRol);

                var rowsAffected = await command.ExecuteNonQueryAsync();
                if (rowsAffected > 0)
                {
                    TempData["SuccessMessage"] = "Rol eliminado correctamente.";
                }
                else
                {
                    TempData["ErrorMessage"] = "No se encontró el rol a eliminar.";
                }
            }
            catch (SqlException ex)
            {
                TempData["ErrorMessage"] = "Error al eliminar el rol (es posible que existan usuarios asociados): " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
