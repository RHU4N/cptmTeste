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
            return _context.InspecaoDTO
                .AsNoTracking()
                .FirstOrDefault(x => x.id == id);
        }

        public Inspecao? Get(int id, int usuarioId)
        {
            return _context.InspecaoDTO
                .AsNoTracking()
                .FirstOrDefault(x => x.id == id && x.usuarioId == usuarioId);
        }

        public IEnumerable<Inspecao> GetAllByUsuario(int usuarioId)
        {
            return _context.InspecaoDTO
                .AsNoTracking()
                .Where(x => x.usuarioId == usuarioId)
                .ToList();
        }

        public IEnumerable<Inspecao> GetAll()
        {
            return _context.InspecaoDTO
                .AsNoTracking()
                .ToList();
        }

        public bool Update(Inspecao inspecao, int usuarioId)
        {
            var exists = _context.InspecaoDTO
                .AsNoTracking()
                .Count(x => x.id == inspecao.id && x.usuarioId == usuarioId) > 0;

            if (!exists)
            {
                return false;
            }

            var local = _context.InspecaoDTO.Local.FirstOrDefault(x => x.id == inspecao.id);
            if (local is not null)
            {
                _context.Entry(local).State = EntityState.Detached;
            }

            _context.Entry(inspecao).State = EntityState.Modified;
            _context.SaveChanges();
            return true;
        }

        public bool Delete(int id, int usuarioId)
        {
            var inspecao = _context.InspecaoDTO.FirstOrDefault(x => x.id == id && x.usuarioId == usuarioId);
            if (inspecao is null)
            {
                return false;
            }

            _context.InspecaoDTO.Remove(inspecao);
            _context.SaveChanges();
            return true;
        }
    }
}
