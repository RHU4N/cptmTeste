using cptmApiTeste.Domain.Model.UsuarioAggregate;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace cptmApiTeste.Services
{
    public class JwtTokenService
    {
        private readonly IConfiguration _configuration;

        public JwtTokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public (string token, DateTime expiresAtUtc) GenerateToken(Usuario usuario)
        {
            var issuer = _configuration["Jwt:Issuer"] ?? "cptm-api";
            var audience = _configuration["Jwt:Audience"] ?? "cptm-web";
            var secret = _configuration["Jwt:Secret"] ?? "MinhaChaveSecretaMuitoForteESegura12345";
            var expiresMinutes = int.TryParse(_configuration["Jwt:ExpiresMinutes"], out var parsedMinutes)
                ? parsedMinutes
                : 120;

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiresAtUtc = DateTime.UtcNow.AddMinutes(expiresMinutes);

            var normalizedRole = string.Equals(usuario.role?.Trim(), "admin", StringComparison.OrdinalIgnoreCase)
                ? "admin"
                : "user";

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, usuario.username),
                new Claim(ClaimTypes.Name, usuario.username),
                new Claim(ClaimTypes.NameIdentifier, usuario.id.ToString()),
                new Claim(ClaimTypes.Role, normalizedRole),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: expiresAtUtc,
                signingCredentials: credentials);

            return (new JwtSecurityTokenHandler().WriteToken(token), expiresAtUtc);
        }
    }
}