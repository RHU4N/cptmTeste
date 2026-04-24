namespace cptmApiTeste.Aplication.ViewModel
{
    public class inspecaoViewModel
    {
        public string titulo { get; set; } = string.Empty;
        public string descricao { get; set; } = string.Empty;
        public DateTime data { get; set; }
        public IFormFile Photo { get; set; } = default!;
        public string localizacao { get; set; } = string.Empty;
        public string latitude { get; set; } = string.Empty;
        public string longitude { get; set; } = string.Empty;

    }
}
