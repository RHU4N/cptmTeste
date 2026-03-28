using cptmApiTeste.Domain.Model.InspecaoAggregate;
using Microsoft.EntityFrameworkCore;

namespace cptmApiTeste.Infraestrutura.Repositories
{
    public class InspecaoRepository : IInspecaoRepository
    {
        private readonly ConectContext _context = new ConectContext();
        public void Add(Inspecao inspecao)
        {
            _context.Database.EnsureCreated();
            _context.InspecaoDTO.Add(inspecao);
            _context.SaveChanges();
        }

        public Inspecao? Get(int id)
        {
            return _context.InspecaoDTO.Find(id);
        }

        public IEnumerable<Inspecao> GetAll()
        {
            return _context.InspecaoDTO.ToList();
        }

        public void Update(Inspecao inspecao)
        {
            _context.InspecaoDTO.Update(inspecao);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var inspecao = _context.InspecaoDTO.Find(id);
            if (inspecao is null)
            {
                return;
            }

            _context.InspecaoDTO.Remove(inspecao);
            _context.SaveChanges();
        }
    }
}
