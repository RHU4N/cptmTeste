using System.Text.Json.Serialization;

namespace cptmApiTeste.Domain.DTOs
{
    public class InspecaoDTO
    {
        public int id { get; set; }
        public int? usuarioId { get; set; }
        public string? usuarioUsername { get; set; }
        public string titulo { get; set; } = string.Empty;
        public string descricao { get; set; } = string.Empty;
        public DateTime data { get; set; }
        public string? localizacao { get; set; }
        public double? latitude { get; set; }
        public double? longitude { get; set; }

        [JsonIgnore]
        public byte[] photo { get; set; } = Array.Empty<byte>();

        public string photoBase64 => Convert.ToBase64String(photo);
    }
}
