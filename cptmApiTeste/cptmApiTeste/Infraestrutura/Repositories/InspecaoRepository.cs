using cptmApiTeste.Domain.Model.InspecaoAggregate;
using Microsoft.EntityFrameworkCore;

namespace cptmApiTeste.Infraestrutura.Repositories
{
    public class InspecaoRepository : IInspecaoRepository
    {
        private readonly ConectContext _context;

        public InspecaoRepository(ConectContext context)
        {
            _context = context;
        }

        public void Add(Inspecao inspecao)
        {
            _context.InspecaoDTO.Add(inspecao);
            _context.SaveChanges();
        }

        public Inspecao? Get(int id)
        {
            return _context.InspecaoDTO.AsNoTracking().FirstOrDefault(x => x.id == id);
        }

        public IEnumerable<Inspecao> GetAll()
        {
            return _context.InspecaoDTO.ToList();
        }

        public void Update(Inspecao inspecao)
        {
            var local = _context.InspecaoDTO.Local.FirstOrDefault(x => x.id == inspecao.id);
            if (local is not null)
            {
                _context.Entry(local).State = EntityState.Detached;
            }

            _context.Entry(inspecao).State = EntityState.Modified;
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
