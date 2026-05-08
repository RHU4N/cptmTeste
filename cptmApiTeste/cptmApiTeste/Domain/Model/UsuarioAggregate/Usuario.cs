using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace cptmApiTeste.Domain.Model.UsuarioAggregate
{
    [Table("USUARIO")]
    public class Usuario
    {
        [Key]
        [Column("ID")]
        public int id { get; private set; }

        [Column("USERNAME")]
        public string username { get; private set; }

        [Column("PASSWORD")]
        public string password { get; private set; }

        [Column("ROLE")]
        public string role { get; private set; }

        public Usuario(string username, string password, string role)
        {
            this.username = username ?? throw new ArgumentNullException(nameof(username));
            this.password = password ?? throw new ArgumentNullException(nameof(password));
            this.role = role ?? throw new ArgumentNullException(nameof(role));
        }

        public Usuario(int id, string username, string password, string role)
        {
            this.id = id;
            this.username = username ?? throw new ArgumentNullException(nameof(username));
            this.password = password ?? throw new ArgumentNullException(nameof(password));
            this.role = role ?? throw new ArgumentNullException(nameof(role));
        }

        public Usuario()
        {
            username = string.Empty;
            password = string.Empty;
            role = string.Empty;
        }

        public void AtualizarSenha(string novaSenhaHash)
        {
            password = novaSenhaHash ?? throw new ArgumentNullException(nameof(novaSenhaHash));
        }

        public void AtualizarRole(string novaRole)
        {
            role = novaRole ?? throw new ArgumentNullException(nameof(novaRole));
        }
    }
}