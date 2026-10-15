using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using SIGALE_Web_App.Models;

namespace SIGALE_Web_App.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly string _connectionString;

        public UsuariosController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Cadena de conexión 'DefaultConnection' no encontrada.");
        }

        // GET: /Usuarios
        public async Task<IActionResult> Index()
        {
            var usuarios = new List<Usuario>();

            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                // Cargar usuarios con su rol
                const string query = @"
                    SELECT u.Carnet, u.NombreCompleto, u.Carrera, u.Campus, u.Activo, u.CorreoElectronico, u.IdRol, r.NombreRol
                    FROM auth.Usuarios u
                    INNER JOIN auth.Roles r ON u.IdRol = r.IdRol
                    ORDER BY u.NombreCompleto ASC";

                using (var command = new SqlCommand(query, connection))
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        usuarios.Add(new Usuario
                        {
                            Carnet = reader.GetString(0),
                            NombreCompleto = reader.GetString(1),
                            Carrera = reader.GetString(2),
                            Campus = reader.GetString(3),
                            Activo = reader.GetBoolean(4),
                            CorreoElectronico = reader.GetString(5),
                            IdRol = reader.GetInt32(6),
                            Rol = new Rol
                            {
                                IdRol = reader.GetInt32(6),
                                NombreRol = reader.GetString(7)
                            }
                        });
                    }
                }

                // Cargar lista de roles para modales
                ViewBag.Roles = await ObtenerRolesAsync(connection);
            }
            catch (SqlException ex)
            {
                TempData["ErrorMessage"] = "Error al consultar los usuarios: " + ex.Message;
                ViewBag.Roles = new List<Rol>();
            }

            return View(usuarios);
        }

        // POST: /Usuarios/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string carnet, string nombreCompleto, string carrera, string campus, string correoElectronico, int idRol, bool activo = true)
        {
            if (string.IsNullOrWhiteSpace(carnet) || string.IsNullOrWhiteSpace(nombreCompleto))
            {
                TempData["ErrorMessage"] = "Carnet y Nombre Completo son requeridos.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                const string query = @"
                    INSERT INTO auth.Usuarios (Carnet, NombreCompleto, Carrera, Campus, Activo, CorreoElectronico, IdRol)
                    VALUES (@Carnet, @NombreCompleto, @Carrera, @Campus, @Activo, @CorreoElectronico, @IdRol)";

                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Carnet", carnet.Trim().ToUpper());
                command.Parameters.AddWithValue("@NombreCompleto", nombreCompleto.Trim());
                command.Parameters.AddWithValue("@Carrera", carrera.Trim());
                command.Parameters.AddWithValue("@Campus", campus.Trim());
                command.Parameters.AddWithValue("@Activo", activo);
                command.Parameters.AddWithValue("@CorreoElectronico", correoElectronico.Trim());
                command.Parameters.AddWithValue("@IdRol", idRol);

                await command.ExecuteNonQueryAsync();
                TempData["SuccessMessage"] = "Usuario creado correctamente.";
            }
            catch (SqlException ex)
            {
                TempData["ErrorMessage"] = "Error al insertar el usuario: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: /Usuarios/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string carnet, string nombreCompleto, string carrera, string campus, string correoElectronico, int idRol, bool activo)
        {
            if (string.IsNullOrWhiteSpace(carnet) || string.IsNullOrWhiteSpace(nombreCompleto))
            {
                TempData["ErrorMessage"] = "Carnet y Nombre Completo son requeridos.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                const string query = @"
                    UPDATE auth.Usuarios 
                    SET NombreCompleto = @NombreCompleto,
                        Carrera = @Carrera,
                        Campus = @Campus,
                        Activo = @Activo,
                        CorreoElectronico = @CorreoElectronico,
                        IdRol = @IdRol
                    WHERE Carnet = @Carnet";

                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Carnet", carnet.Trim().ToUpper());
                command.Parameters.AddWithValue("@NombreCompleto", nombreCompleto.Trim());
                command.Parameters.AddWithValue("@Carrera", carrera.Trim());
                command.Parameters.AddWithValue("@Campus", campus.Trim());
                command.Parameters.AddWithValue("@Activo", activo);
                command.Parameters.AddWithValue("@CorreoElectronico", correoElectronico.Trim());
                command.Parameters.AddWithValue("@IdRol", idRol);

                var rows = await command.ExecuteNonQueryAsync();
                if (rows > 0)
                {
                    TempData["SuccessMessage"] = "Usuario actualizado correctamente.";
                }
                else
                {
                    TempData["ErrorMessage"] = "No se encontró el usuario a actualizar.";
                }
            }
            catch (SqlException ex)
            {
                TempData["ErrorMessage"] = "Error al actualizar el usuario: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: /Usuarios/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string carnet)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                const string query = "DELETE FROM auth.Usuarios WHERE Carnet = @Carnet";
                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Carnet", carnet.Trim().ToUpper());

                var rows = await command.ExecuteNonQueryAsync();
                if (rows > 0)
                {
                    TempData["SuccessMessage"] = "Usuario eliminado correctamente.";
                }
                else
                {
                    TempData["ErrorMessage"] = "No se encontró el usuario a eliminar.";
                }
            }
            catch (SqlException ex)
            {
                TempData["ErrorMessage"] = "Error al eliminar el usuario (puede tener registros vinculados): " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        private static async Task<List<Rol>> ObtenerRolesAsync(SqlConnection connection)
        {
            var roles = new List<Rol>();
            const string query = "SELECT IdRol, NombreRol FROM auth.Roles ORDER BY NombreRol ASC";
            using var command = new SqlCommand(query, connection);
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                roles.Add(new Rol
                {
                    IdRol = reader.GetInt32(0),
                    NombreRol = reader.GetString(1)
                });
            }
            return roles;
        }
    }
}
