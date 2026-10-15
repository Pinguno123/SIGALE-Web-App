using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SIGALE_Web_App.Data;

namespace SIGALE_Web_App.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UsuariosController(
            ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var usuarios = await _context.Usuarios
                .Include(u => u.Rol)
                .AsNoTracking()
                .OrderBy(u => u.NombreCompleto)
                .ToListAsync();

            return View(usuarios);
        }
    }
}
