using cptmApiTeste.Aplication.ViewModel;
using cptmApiTeste.Domain.DTOs;
using cptmApiTeste.Domain.Model.InspecaoAggregate;
using Microsoft.AspNetCore.Mvc;

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

            var dto = ToDto(inspecaoViewModel);
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

            var dto = ToDto(inspecaoViewModel);
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
                photo = inspecao.photo
            };
        }

        private static InspecaoDTO ToDto(inspecaoViewModel inspecaoViewModel)
        {
            using var memoryStream = new MemoryStream();
            inspecaoViewModel.Photo.CopyTo(memoryStream);

            return new InspecaoDTO
            {
                titulo = inspecaoViewModel.titulo,
                descricao = inspecaoViewModel.descricao,
                data = inspecaoViewModel.data,
                photo = memoryStream.ToArray()
            };
        }

        private static Inspecao ToEntity(InspecaoDTO dto)
        {
            return dto.id == 0
                ? new Inspecao(dto.titulo, dto.descricao, dto.data, dto.photo)
                : new Inspecao(dto.id, dto.titulo, dto.descricao, dto.data, dto.photo);
        }
    }
}
