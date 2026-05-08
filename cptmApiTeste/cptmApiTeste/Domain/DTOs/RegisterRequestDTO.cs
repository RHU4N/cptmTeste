namespace cptmApiTeste.Domain.DTOs
{
    public class RegisterRequestDTO
    {
        public string username { get; set; } = string.Empty;
        public string password { get; set; } = string.Empty;
        public string? activationCode { get; set; }
    }
}
