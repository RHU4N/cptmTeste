using cptmApiTeste.Aplication.ViewModel;
using cptmApiTeste.Domain.DTOs;
using cptmApiTeste.Domain.Model.InspecaoAggregate;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace cptmApiTeste.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InspecaoController : ControllerBase
    {
        private readonly IInspecaoRepository _inspecaopository;

        public InspecaoController(IInspecaoRepository inspecaopository)
        {
            _inspecaopository = inspecaopository ?? throw new ArgumentNullException(nameof(inspecaopository));
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var inspecoes = _inspecaopository.GetAll().Select(ToDto);
            return Ok(inspecoes);
        }

        [HttpGet("{id:int}")]
        public IActionResult GetById(int id)
        {
            var inspecao = _inspecaopository.Get(id);
            return inspecao is null ? NotFound() : Ok(ToDto(inspecao));
        }

        [HttpGet("{id}/foto")]
        public IActionResult GetFoto(int id)
        {
            var inspecao = _inspecaopository.Get(id);
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
            if (inspecaoViewModel.Photo is null || inspecaoViewModel.Photo.Length == 0)
            {
                return BadRequest("Foto obrigatória.");
            }

            if (!TryParseCoordinate(inspecaoViewModel.latitude, out var latitude)
                || !TryParseCoordinate(inspecaoViewModel.longitude, out var longitude)
                || !IsValidLocation(latitude, longitude, inspecaoViewModel.localizacao))
            {
                return BadRequest("Localização inválida.");
            }

            var dto = ToDto(inspecaoViewModel, latitude, longitude);
            var inspecao = ToEntity(dto);

            _inspecaopository.Add(inspecao);

            return CreatedAtAction(nameof(GetById), new { id = inspecao.id }, ToDto(inspecao));
        }

        [HttpPut("{id:int}")]
        [Consumes("multipart/form-data")]
        public IActionResult Update(int id, [FromForm] inspecaoViewModel inspecaoViewModel)
        {
            var existingInspecao = _inspecaopository.Get(id);
            if (existingInspecao is null)
            {
                return NotFound();
            }

            if (inspecaoViewModel.Photo is null || inspecaoViewModel.Photo.Length == 0)
            {
                return BadRequest("Foto obrigatória.");
            }

            if (!TryParseCoordinate(inspecaoViewModel.latitude, out var latitude)
                || !TryParseCoordinate(inspecaoViewModel.longitude, out var longitude)
                || !IsValidLocation(latitude, longitude, inspecaoViewModel.localizacao))
            {
                return BadRequest("Localização inválida.");
            }

            var dto = ToDto(inspecaoViewModel, latitude, longitude);
            dto.id = id;
            var inspecao = ToEntity(dto);

            _inspecaopository.Update(inspecao);

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            var inspecao = _inspecaopository.Get(id);
            if (inspecao is null)
            {
                return NotFound();
            }

            _inspecaopository.Delete(id);
            return NoContent();
        }

        private static InspecaoDTO ToDto(Inspecao inspecao)
        {
            return new InspecaoDTO
            {
                id = inspecao.id,
                titulo = inspecao.titulo,
                descricao = inspecao.descricao,
                data = inspecao.data,
                localizacao = inspecao.localizacao,
                latitude = inspecao.latitude,
                longitude = inspecao.longitude,
                photo = inspecao.photo
            };
        }

        private static InspecaoDTO ToDto(inspecaoViewModel inspecaoViewModel, double latitude, double longitude)
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

        private static Inspecao ToEntity(InspecaoDTO dto)
        {
            return dto.id == 0
                ? new Inspecao(dto.titulo, dto.descricao, dto.data, dto.photo, dto.localizacao, dto.latitude, dto.longitude)
                : new Inspecao(dto.id, dto.titulo, dto.descricao, dto.data, dto.photo, dto.localizacao, dto.latitude, dto.longitude);
        }

        private static bool IsValidLocation(double latitude, double longitude, string? localizacao)
        {
            return latitude is >= -90 and <= 90
                && longitude is >= -180 and <= 180
                && !string.IsNullOrWhiteSpace(localizacao);
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
