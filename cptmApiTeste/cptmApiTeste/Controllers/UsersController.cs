using cptmApiTeste.Domain.Model.UsuarioAggregate;
using cptmApiTeste.Infraestrutura;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace cptmApiTeste.Controllers
{
    [Authorize(Roles = "admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly ConectContext _context;
        private readonly IPasswordHasher<Usuario> _passwordHasher;

        public UsersController(ConectContext context, IPasswordHasher<Usuario> passwordHasher)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? role = null, [FromQuery] string? search = null)
        {
            var query = _context.Usuarios.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(role))
            {
                var normalizedRole = NormalizeRole(role);
                query = query.Where(u => u.role.ToLower() == normalizedRole);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                var normalizedSearch = search.Trim().ToLower();
                query = query.Where(u => u.username.ToLower().Contains(normalizedSearch));
            }

            var users = await query
                .Select(u => new { id = u.id, username = u.username, role = u.role })
                .ToListAsync();

            return Ok(users);
        }

        public class CreateUserDTO
        {
            public string username { get; set; } = string.Empty;
            public string password { get; set; } = string.Empty;
            public string? role { get; set; }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUserDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto?.username) || string.IsNullOrWhiteSpace(dto.password))
            {
                return BadRequest("Usuário e senha são obrigatórios.");
            }

            var username = dto.username.Trim();
            var exists = await _context.Usuarios.AsNoTracking().AnyAsync(x => x.username == username);
            if (exists)
            {
                return Conflict("Usuário já existe.");
            }

            var hash = _passwordHasher.HashPassword(null!, dto.password);
            var user = new Usuario(username, hash, NormalizeRole(dto.role));

            _context.Usuarios.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAll), new { id = user.id }, new { id = user.id, username = user.username, role = user.role });
        }

        public class UpdateRoleDTO
        {
            public string role { get; set; } = string.Empty;
        }

        [HttpPut("{id:int}/role")]
        public async Task<IActionResult> UpdateRole(int id, [FromBody] UpdateRoleDTO dto)
        {
            var user = await _context.Usuarios.FindAsync(id);
            if (user is null) return NotFound();

            user.AtualizarRole(NormalizeRole(dto.role));
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _context.Usuarios.FindAsync(id);
            if (user is null) return NotFound();

            _context.Usuarios.Remove(user);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private static string NormalizeRole(string? role)
        {
            return string.Equals(role?.Trim(), "admin", StringComparison.OrdinalIgnoreCase)
                ? "admin"
                : "user";
        }
    }
}
