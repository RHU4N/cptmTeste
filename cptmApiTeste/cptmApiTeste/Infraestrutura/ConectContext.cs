using cptmApiTeste.Domain.Model.InspecaoAggregate;
using Microsoft.EntityFrameworkCore;

namespace cptmApiTeste.Infraestrutura
{
    public class ConectContext : DbContext
    {
        public DbSet<Inspecao> InspecaoDTO { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseOracle("Data Source=localhost:1521/XEPDB1;User ID=RHUAN; Password=root");
            //"Data Source=MEU_HOST:1521/MEU_SERVICO;User Id=MEU_USUARIO;Password=MINHA_SENHA;"
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
           modelBuilder.Entity<Inspecao>().ToTable("INSPECAO");
           modelBuilder.Entity<Inspecao>().Property(e => e.id).HasColumnName("ID").ValueGeneratedOnAdd();
           modelBuilder.Entity<Inspecao>().Property(e => e.titulo).HasColumnName("TITULO");
           modelBuilder.Entity<Inspecao>().Property(e => e.descricao).HasColumnName("DESCRICAO");
           modelBuilder.Entity<Inspecao>().Property(e => e.data).HasColumnName("DATA");
           modelBuilder.Entity<Inspecao>().Property(e => e.photo).HasColumnName("PHOTO").HasColumnType("BLOB").IsRequired(false);

        }
    }
}
