namespace cptmApiTeste.Domain.DTOs
{
    public class LoginResponseDTO
    {
        public string token { get; set; } = string.Empty;
        public string username { get; set; } = string.Empty;
        public string role { get; set; } = string.Empty;
        public DateTime expiresAtUtc { get; set; }
    }
}