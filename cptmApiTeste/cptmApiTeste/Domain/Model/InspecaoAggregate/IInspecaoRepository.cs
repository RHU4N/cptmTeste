namespace cptmApiTeste.Domain.Model.InspecaoAggregate
{
    public interface IInspecaoRepository
    {
        void Add(Inspecao inspecao);
        Inspecao? Get(int id);
        Inspecao? Get(int id, int usuarioId);
        IEnumerable<Inspecao> GetAll();
        IEnumerable<Inspecao> GetAllByUsuario(int usuarioId);
        bool Update(Inspecao inspecao, int usuarioId);
        bool Delete(int id, int usuarioId);


    }
}
