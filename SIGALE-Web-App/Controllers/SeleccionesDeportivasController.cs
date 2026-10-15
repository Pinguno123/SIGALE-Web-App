using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using SIGALE_Web_App.Models;

namespace SIGALE_Web_App.Controllers
{
    public class SeleccionesDeportivasController : Controller
    {
        private readonly string _connectionString;

        public SeleccionesDeportivasController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Cadena de conexión 'DefaultConnection' no encontrada.");
        }

        // GET: /SeleccionesDeportivas
        public async Task<IActionResult> Index()
        {
            var selecciones = new List<SeleccionDeportiva>();

            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                const string query = @"
                    SELECT IdSeleccion, Disciplina, Rama, NombreEquipo, ImgUrl
                    FROM sports.SeleccionesDeportivas
                    ORDER BY NombreEquipo ASC";

                using var command = new SqlCommand(query, connection);
                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    selecciones.Add(new SeleccionDeportiva
                    {
                        IdSeleccion = reader.GetInt32(0),
                        Disciplina = reader.GetString(1),
                        Rama = reader.GetString(2),
                        NombreEquipo = reader.GetString(3),
                        ImgUrl = reader.IsDBNull(4) ? null : reader.GetString(4)
                    });
                }
            }
            catch (SqlException ex)
            {
                TempData["ErrorMessage"] = "Error al consultar las selecciones deportivas: " + ex.Message;
            }

            return View(selecciones);
        }

        // POST: /SeleccionesDeportivas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string disciplina, string rama, string nombreEquipo, string? imgUrl)
        {
            if (string.IsNullOrWhiteSpace(disciplina) || string.IsNullOrWhiteSpace(rama) || string.IsNullOrWhiteSpace(nombreEquipo))
            {
                TempData["ErrorMessage"] = "Disciplina, Rama y Nombre de Equipo son obligatorios.";
                return RedirectToAction(nameof(Index));
            }

            if (nombreEquipo.Trim().Length < 3)
            {
                TempData["ErrorMessage"] = "El nombre de la selección debe tener al menos 3 caracteres.";
                return RedirectToAction(nameof(Index));
            }

            if (disciplina.Trim().Length < 3)
            {
                TempData["ErrorMessage"] = "La disciplina deportiva debe tener al menos 3 caracteres.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                const string query = @"
                    INSERT INTO sports.SeleccionesDeportivas (Disciplina, Rama, NombreEquipo, ImgUrl)
                    VALUES (@Disciplina, @Rama, @NombreEquipo, @ImgUrl)";

                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Disciplina", disciplina.Trim());
                command.Parameters.AddWithValue("@Rama", rama.Trim());
                command.Parameters.AddWithValue("@NombreEquipo", nombreEquipo.Trim());
                command.Parameters.AddWithValue("@ImgUrl", (object?)imgUrl?.Trim() ?? DBNull.Value);

                await command.ExecuteNonQueryAsync();
                TempData["SuccessMessage"] = "Selección deportiva registrada exitosamente.";
            }
            catch (SqlException ex)
            {
                TempData["ErrorMessage"] = "Error al registrar selección: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: /SeleccionesDeportivas/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int idSeleccion, string disciplina, string rama, string nombreEquipo, string? imgUrl)
        {
            if (idSeleccion <= 0 || string.IsNullOrWhiteSpace(disciplina) || string.IsNullOrWhiteSpace(rama) || string.IsNullOrWhiteSpace(nombreEquipo))
            {
                TempData["ErrorMessage"] = "Datos incompletos para actualizar la selección.";
                return RedirectToAction(nameof(Index));
            }

            if (nombreEquipo.Trim().Length < 3)
            {
                TempData["ErrorMessage"] = "El nombre de la selección debe tener al menos 3 caracteres.";
                return RedirectToAction(nameof(Index));
            }

            if (disciplina.Trim().Length < 3)
            {
                TempData["ErrorMessage"] = "La disciplina deportiva debe tener al menos 3 caracteres.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                const string query = @"
                    UPDATE sports.SeleccionesDeportivas
                    SET Disciplina = @Disciplina,
                        Rama = @Rama,
                        NombreEquipo = @NombreEquipo,
                        ImgUrl = @ImgUrl
                    WHERE IdSeleccion = @IdSeleccion";

                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@IdSeleccion", idSeleccion);
                command.Parameters.AddWithValue("@Disciplina", disciplina.Trim());
                command.Parameters.AddWithValue("@Rama", rama.Trim());
                command.Parameters.AddWithValue("@NombreEquipo", nombreEquipo.Trim());
                command.Parameters.AddWithValue("@ImgUrl", (object?)imgUrl?.Trim() ?? DBNull.Value);

                var rows = await command.ExecuteNonQueryAsync();
                if (rows > 0)
                {
                    TempData["SuccessMessage"] = "Selección deportiva actualizada con éxito.";
                }
                else
                {
                    TempData["ErrorMessage"] = "No se encontró la selección a actualizar.";
                }
            }
            catch (SqlException ex)
            {
                TempData["ErrorMessage"] = "Error al actualizar selección: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: /SeleccionesDeportivas/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int idSeleccion)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                const string query = "DELETE FROM sports.SeleccionesDeportivas WHERE IdSeleccion = @IdSeleccion";
                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@IdSeleccion", idSeleccion);

                var rows = await command.ExecuteNonQueryAsync();
                if (rows > 0)
                {
                    TempData["SuccessMessage"] = "Selección eliminada correctamente.";
                }
                else
                {
                    TempData["ErrorMessage"] = "No se encontró la selección a eliminar.";
                }
            }
            catch (SqlException ex)
            {
                TempData["ErrorMessage"] = "Error al eliminar selección (puede tener participaciones vinculadas): " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
