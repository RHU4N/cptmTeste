namespace cptmApiTeste.Domain.Model.InspecaoAggregate
{
    public interface IInspecaoRepository
    {
        void Add(Inspecao inspecao);
        Inspecao? Get(int id);
        IEnumerable<Inspecao> GetAll();
        void Update(Inspecao inspecao);
        void Delete(int id);


    }
}
