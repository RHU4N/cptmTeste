using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace cptmApiTeste.Domain.Model.InspecaoAggregate
{
    [Table("INSPECAO")]
    public class Inspecao
    {
        [Key]
        [Column("ID")]
        public int id { get;private set; }

        [Column("TITULO")]
        public string titulo { get; private set; }

        [Column("DESCRICAO")]
        public string descricao { get; private set; }

        [Column("DATA")]
        public DateTime data { get; private set; }

        [Column("PHOTO")]
        public byte[] photo { get; private set; }

        [Column("LOCALIZACAO")]
        public string? localizacao { get; private set; }

        [Column("LATITUDE")]
        public double? latitude { get; private set; }

        [Column("LONGITUDE")]
        public double? longitude { get; private set; }

        [Column("USUARIO_ID")]
        public int? usuarioId { get; private set; }

        public Inspecao(string titulo, string descricao, DateTime data, byte[] photo, string? localizacao, double? latitude, double? longitude, int usuarioId)
        {
            this.titulo = titulo ?? throw new ArgumentNullException(nameof(titulo));
            this.descricao = descricao ?? throw new ArgumentNullException(nameof(descricao));
            this.data = data;
            this.photo = photo ?? throw new ArgumentNullException(nameof(photo));
            this.localizacao = localizacao;
            this.latitude = latitude;
            this.longitude = longitude;
            this.usuarioId = usuarioId;
        }

        public Inspecao(int id, string titulo, string descricao, DateTime data, byte[] photo, string? localizacao, double? latitude, double? longitude, int usuarioId)
        {
            this.id = id;
            this.titulo = titulo ?? throw new ArgumentNullException(nameof(titulo));
            this.descricao = descricao ?? throw new ArgumentNullException(nameof(descricao));
            this.data = data;
            this.photo = photo ?? throw new ArgumentNullException(nameof(photo));
            this.localizacao = localizacao;
            this.latitude = latitude;
            this.longitude = longitude;
            this.usuarioId = usuarioId;
        }

        public Inspecao()
        {
            titulo = string.Empty;
            descricao = string.Empty;
            photo = Array.Empty<byte>();
            localizacao = null;
            usuarioId = null;
        }
    }
}

