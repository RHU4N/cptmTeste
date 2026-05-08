using cptmApiTeste.Aplication.ViewModel;
using cptmApiTeste.Domain.DTOs;
using cptmApiTeste.Domain.Model.InspecaoAggregate;
using cptmApiTeste.Infraestrutura;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using System.Security.Claims;
using System.Globalization;

namespace cptmApiTeste.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class InspecaoController : ControllerBase
    {
        private readonly IInspecaoRepository _inspecaopository;
        private readonly ConectContext _context;

        public InspecaoController(IInspecaoRepository inspecaopository, ConectContext context)
        {
            _inspecaopository = inspecaopository ?? throw new ArgumentNullException(nameof(inspecaopository));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            if (!TryGetAuthenticatedUserId(out var usuarioId))
            {
                return Unauthorized();
            }

            var isAdmin = IsAdmin();
            var usersById = _context.Usuarios
                .AsNoTracking()
                .ToDictionary(x => x.id, x => x.username);

            var inspecoes = isAdmin
                ? _inspecaopository.GetAll().Select(x => ToDto(x, usersById))
                : _inspecaopository.GetAllByUsuario(usuarioId).Select(x => ToDto(x, usersById));

            return Ok(inspecoes);
        }

        [HttpGet("{id:int}")]
        public IActionResult GetById(int id)
        {
            if (!TryGetAuthenticatedUserId(out var usuarioId))
            {
                return Unauthorized();
            }

            var isAdmin = IsAdmin();
            var inspecao = isAdmin
                ? _inspecaopository.Get(id)
                : _inspecaopository.Get(id, usuarioId);

            if (inspecao is null)
            {
                return NotFound();
            }

            var usersById = _context.Usuarios
                .AsNoTracking()
                .ToDictionary(x => x.id, x => x.username);

            return Ok(ToDto(inspecao, usersById));
        }

        [HttpGet("{id}/foto")]
        public IActionResult GetFoto(int id)
        {
            if (!TryGetAuthenticatedUserId(out var usuarioId))
            {
                return Unauthorized();
            }

            var inspecao = IsAdmin()
                ? _inspecaopository.Get(id)
                : _inspecaopository.Get(id, usuarioId);

            if (inspecao is null || inspecao.photo is null || inspecao.photo.Length == 0)
            {
                return NotFound();
            }

            return File(inspecao.photo, "image/*");
        }

            [HttpPost]
        [Consumes("multipart/form-data")]
        public IActionResult Add([FromForm] inspecaoViewModel inspecaoViewModel)
        {
            if (!TryGetAuthenticatedUserId(out var usuarioId))
            {
                return Unauthorized();
            }

            if (inspecaoViewModel.Photo is null || inspecaoViewModel.Photo.Length == 0)
            {
                return BadRequest("Foto obrigatória.");
            }

            double latParsed = 0, lonParsed = 0;
            var latProvided = !string.IsNullOrWhiteSpace(inspecaoViewModel.latitude);
            var lonProvided = !string.IsNullOrWhiteSpace(inspecaoViewModel.longitude);

            if (latProvided && !TryParseCoordinate(inspecaoViewModel.latitude, out latParsed))
            {
                return BadRequest("Localização inválida.");
            }

            if (lonProvided && !TryParseCoordinate(inspecaoViewModel.longitude, out lonParsed))
            {
                return BadRequest("Localização inválida.");
            }

            double? latitude = latProvided ? latParsed : null;
            double? longitude = lonProvided ? lonParsed : null;

            if (!IsValidLocation(latitude, longitude, inspecaoViewModel.localizacao))
            {
                return BadRequest("Localização inválida.");
            }

            var dto = ToDto(inspecaoViewModel, latitude, longitude);
            var inspecao = ToEntity(dto, usuarioId);

            _inspecaopository.Add(inspecao);

            return CreatedAtAction(nameof(GetById), new { id = inspecao.id }, ToDto(inspecao));
        }

        [HttpPut("{id:int}")]
        [Consumes("multipart/form-data")]
        public IActionResult Update(int id, [FromForm] inspecaoViewModel inspecaoViewModel)
        {
            if (!TryGetAuthenticatedUserId(out var usuarioId))
            {
                return Unauthorized();
            }

            var existingInspecao = _inspecaopository.Get(id, usuarioId);
            if (existingInspecao is null)
            {
                return NotFound();
            }

            double latParsed = 0, lonParsed = 0;
            var latProvided = !string.IsNullOrWhiteSpace(inspecaoViewModel.latitude);
            var lonProvided = !string.IsNullOrWhiteSpace(inspecaoViewModel.longitude);

            if (latProvided && !TryParseCoordinate(inspecaoViewModel.latitude, out latParsed))
            {
                return BadRequest("Localização inválida.");
            }

            if (lonProvided && !TryParseCoordinate(inspecaoViewModel.longitude, out lonParsed))
            {
                return BadRequest("Localização inválida.");
            }

            double? latitude = latProvided ? latParsed : null;
            double? longitude = lonProvided ? lonParsed : null;

            if (!IsValidLocation(latitude, longitude, inspecaoViewModel.localizacao))
            {
                return BadRequest("Localização inválida.");
            }

            var hasNewPhoto = inspecaoViewModel.Photo is not null && inspecaoViewModel.Photo.Length > 0;
            var dto = ToDto(inspecaoViewModel, latitude, longitude, hasNewPhoto ? inspecaoViewModel.Photo : null);
            var photo = hasNewPhoto ? null : existingInspecao.photo;
            if (!hasNewPhoto && photo is not null)
            {
                dto.photo = photo;
            }

            dto.id = id;
            var inspecao = ToEntity(dto, usuarioId);

            var updated = _inspecaopository.Update(inspecao, usuarioId);
            if (!updated)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            if (!TryGetAuthenticatedUserId(out var usuarioId))
            {
                return Unauthorized();
            }

            var deleted = _inspecaopository.Delete(id, usuarioId);
            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }

        private static InspecaoDTO ToDto(Inspecao inspecao)
        {
            return ToDto(inspecao, null);
        }

        private static InspecaoDTO ToDto(Inspecao inspecao, IReadOnlyDictionary<int, string>? usersById)
        {
            string? username = null;
            if (inspecao.usuarioId.HasValue && usersById is not null)
            {
                usersById.TryGetValue(inspecao.usuarioId.Value, out username);
            }

            return new InspecaoDTO
            {
                id = inspecao.id,
                usuarioId = inspecao.usuarioId,
                usuarioUsername = username,
                titulo = inspecao.titulo,
                descricao = inspecao.descricao,
                data = inspecao.data,
                localizacao = inspecao.localizacao,
                latitude = inspecao.latitude,
                longitude = inspecao.longitude,
                photo = inspecao.photo
            };
        }

        private static InspecaoDTO ToDto(inspecaoViewModel inspecaoViewModel, double? latitude, double? longitude)
        {
            using var memoryStream = new MemoryStream();
            inspecaoViewModel.Photo.CopyTo(memoryStream);

            return new InspecaoDTO
            {
                titulo = inspecaoViewModel.titulo,
                descricao = inspecaoViewModel.descricao,
                data = inspecaoViewModel.data,
                localizacao = inspecaoViewModel.localizacao,
                latitude = latitude,
                longitude = longitude,
                photo = memoryStream.ToArray()
            };
        }

        private static InspecaoDTO ToDto(inspecaoViewModel inspecaoViewModel, double? latitude, double? longitude, IFormFile photoToUse)
        {
            byte[] photoBytes = null;

            if (photoToUse is not null && photoToUse.Length > 0)
            {
                using var memoryStream = new MemoryStream();
                photoToUse.CopyTo(memoryStream);
                photoBytes = memoryStream.ToArray();
            }

            return new InspecaoDTO
            {
                titulo = inspecaoViewModel.titulo,
                descricao = inspecaoViewModel.descricao,
                data = inspecaoViewModel.data,
                localizacao = inspecaoViewModel.localizacao,
                latitude = latitude,
                longitude = longitude,
                photo = photoBytes
            };
        }

        private static Inspecao ToEntity(InspecaoDTO dto, int usuarioId)
        {
            return dto.id == 0
                ? new Inspecao(dto.titulo, dto.descricao, dto.data, dto.photo, dto.localizacao, dto.latitude, dto.longitude, usuarioId)
                : new Inspecao(dto.id, dto.titulo, dto.descricao, dto.data, dto.photo, dto.localizacao, dto.latitude, dto.longitude, usuarioId);
        }

        private bool TryGetAuthenticatedUserId(out int userId)
        {
            userId = 0;
            var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? User.FindFirstValue("nameid")
                ?? User.FindFirstValue(JwtRegisteredClaimNames.NameId)
                ?? User.FindFirstValue("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");

            if (int.TryParse(userIdValue, out userId))
            {
                return true;
            }

            var username = User.FindFirstValue(ClaimTypes.Name)
                ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub)
                ?? User.FindFirstValue("unique_name")
                ?? User.FindFirstValue("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name");

            if (string.IsNullOrWhiteSpace(username))
            {
                return false;
            }

            var usuario = _context.Usuarios
                .AsNoTracking()
                .FirstOrDefault(x => x.username == username);

            if (usuario is null)
            {
                return false;
            }

            userId = usuario.id;
            return true;
        }

        private bool IsAdmin()
        {
            if (User.IsInRole("admin"))
            {
                return true;
            }

            var role = User.FindFirstValue(ClaimTypes.Role)
                ?? User.FindFirstValue("role")
                ?? User.FindFirstValue("http://schemas.microsoft.com/ws/2008/06/identity/claims/role");

            return string.Equals(role, "admin", StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsValidLocation(double? latitude, double? longitude, string? localizacao)
        {
            if (latitude.HasValue && longitude.HasValue)
            {
                return latitude.Value is >= -90 and <= 90
                    && longitude.Value is >= -180 and <= 180;
            }

            return !string.IsNullOrWhiteSpace(localizacao);
        }

        private static bool TryParseCoordinate(string? value, out double coordinate)
        {
            coordinate = 0;
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }

            var normalized = value.Trim().Replace(',', '.');
            return double.TryParse(normalized, NumberStyles.Float, CultureInfo.InvariantCulture, out coordinate);
        }
    }
}
