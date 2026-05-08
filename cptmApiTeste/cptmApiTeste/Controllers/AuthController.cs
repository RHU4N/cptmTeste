using cptmApiTeste.Domain.DTOs;
using cptmApiTeste.Domain.Model.UsuarioAggregate;
using cptmApiTeste.Infraestrutura;
using cptmApiTeste.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace cptmApiTeste.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ConectContext _context;
        private readonly JwtTokenService _jwtTokenService;
        private readonly IPasswordHasher<Usuario> _passwordHasher;
        private readonly IConfiguration _configuration;

        public AuthController(
            ConectContext context,
            JwtTokenService jwtTokenService,
            IPasswordHasher<Usuario> passwordHasher,
            IConfiguration configuration)
        {
            _context = context;
            _jwtTokenService = jwtTokenService;
            _passwordHasher = passwordHasher;
            _configuration = configuration;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDTO request)
        {
            if (string.IsNullOrWhiteSpace(request.username) || string.IsNullOrWhiteSpace(request.password))
            {
                return BadRequest("Usuário e senha são obrigatórios.");
            }

            var activationCode = request.activationCode?.Trim();
            var expectedActivationCode = _configuration["Auth:ActivationCode"] ?? "1234";

            if (!string.Equals(activationCode, expectedActivationCode, StringComparison.Ordinal))
            {
                return BadRequest("Código de ativação inválido.");
            }

            var username = request.username.Trim();
            var userAlreadyExists = await _context.Usuarios
                .AsNoTracking()
                .CountAsync(x => x.username == username) > 0;

            if (userAlreadyExists)
            {
                return Conflict("Usuário já existe.");
            }

            var passwordHash = _passwordHasher.HashPassword(null!, request.password);
            var usuario = new Usuario(username, passwordHash, "user");

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            var (token, expiresAtUtc) = _jwtTokenService.GenerateToken(usuario);

            return Created(string.Empty, new LoginResponseDTO
            {
                token = token,
                username = usuario.username,
                role = usuario.role,
                expiresAtUtc = expiresAtUtc
            });
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO request)
        {
            if (string.IsNullOrWhiteSpace(request.username) || string.IsNullOrWhiteSpace(request.password))
            {
                return BadRequest("Usuário e senha são obrigatórios.");
            }

            var username = request.username.Trim();
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(x => x.username == username);

            if (usuario is null)
            {
                return Unauthorized("Usuário ou senha inválidos.");
            }

            var passwordVerification = PasswordVerificationResult.Failed;
            try
            {
                passwordVerification = _passwordHasher.VerifyHashedPassword(usuario, usuario.password, request.password);
            }
            catch (FormatException)
            {
                // Senha legada em texto puro: segue para validação fallback.
            }

            var isLegacyPlainTextPassword = string.Equals(usuario.password, request.password, StringComparison.Ordinal);

            if (passwordVerification == PasswordVerificationResult.Failed && !isLegacyPlainTextPassword)
            {
                return Unauthorized("Usuário ou senha inválidos.");
            }

            if (isLegacyPlainTextPassword || passwordVerification == PasswordVerificationResult.SuccessRehashNeeded)
            {
                var newHash = _passwordHasher.HashPassword(usuario, request.password);
                usuario.AtualizarSenha(newHash);
                await _context.SaveChangesAsync();
            }

            var (token, expiresAtUtc) = _jwtTokenService.GenerateToken(usuario);

            return Ok(new LoginResponseDTO
            {
                token = token,
                username = usuario.username,
                role = usuario.role,
                expiresAtUtc = expiresAtUtc
            });
        }
    }
}