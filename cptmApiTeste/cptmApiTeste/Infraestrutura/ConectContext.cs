using cptmApiTeste.Domain.Model.InspecaoAggregate;
using cptmApiTeste.Domain.Model.UsuarioAggregate;
using Microsoft.EntityFrameworkCore;

namespace cptmApiTeste.Infraestrutura
{
    public class ConectContext : DbContext
    {
        public ConectContext(DbContextOptions<ConectContext> options) : base(options)
        {
        }

        public DbSet<Inspecao> InspecaoDTO { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
           modelBuilder.Entity<Inspecao>().ToTable("INSPECAO");
           modelBuilder.Entity<Inspecao>().Property(e => e.id).HasColumnName("ID").ValueGeneratedOnAdd();
           modelBuilder.Entity<Inspecao>().Property(e => e.titulo).HasColumnName("TITULO");
           modelBuilder.Entity<Inspecao>().Property(e => e.descricao).HasColumnName("DESCRICAO");
           modelBuilder.Entity<Inspecao>().Property(e => e.data).HasColumnName("DATA");
           modelBuilder.Entity<Inspecao>().Property(e => e.photo).HasColumnName("PHOTO").HasColumnType("BLOB").IsRequired(false);
           modelBuilder.Entity<Inspecao>().Property(e => e.localizacao).HasColumnName("LOCALIZACAO").IsRequired(false);
           modelBuilder.Entity<Inspecao>().Property(e => e.latitude).HasColumnName("LATITUDE").HasColumnType("BINARY_DOUBLE").IsRequired(false);
           modelBuilder.Entity<Inspecao>().Property(e => e.longitude).HasColumnName("LONGITUDE").HasColumnType("BINARY_DOUBLE").IsRequired(false);
           modelBuilder.Entity<Inspecao>().Property(e => e.usuarioId).HasColumnName("USUARIO_ID").IsRequired(false);
           modelBuilder.Entity<Inspecao>().HasIndex(e => e.usuarioId).HasDatabaseName("IX_INSPECAO_USUARIO_ID");

           modelBuilder.Entity<Usuario>().ToTable("USUARIO");
           modelBuilder.Entity<Usuario>().Property(e => e.id).HasColumnName("ID").ValueGeneratedOnAdd();
           modelBuilder.Entity<Usuario>().Property(e => e.username).HasColumnName("USERNAME").IsRequired();
           modelBuilder.Entity<Usuario>().Property(e => e.password).HasColumnName("PASSWORD").IsRequired();
           modelBuilder.Entity<Usuario>().Property(e => e.role).HasColumnName("ROLE").IsRequired();
           modelBuilder.Entity<Usuario>().HasIndex(e => e.username).IsUnique();

        }
    }
}
