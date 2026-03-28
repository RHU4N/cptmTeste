using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography.Pkcs;

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

        public Inspecao(string titulo, string descricao, DateTime data, byte[] photo)
        {
            this.titulo = titulo ?? throw new ArgumentNullException(nameof(titulo));
            this.descricao = descricao ?? throw new ArgumentNullException(nameof(descricao));
            this.data = data;
            this.photo = photo ?? throw new ArgumentNullException(nameof(photo));
        }

        public Inspecao(int id, string titulo, string descricao, DateTime data, byte[] photo)
        {
            this.id = id;
            this.titulo = titulo ?? throw new ArgumentNullException(nameof(titulo));
            this.descricao = descricao ?? throw new ArgumentNullException(nameof(descricao));
            this.data = data;
            this.photo = photo ?? throw new ArgumentNullException(nameof(photo));
        }

        public Inspecao()
        {
            
        }
    }
}

