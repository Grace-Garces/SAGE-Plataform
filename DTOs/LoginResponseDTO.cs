namespace SagePlataform.DTOs
{
    // Esta classe representa o objeto 'user' dentro da resposta JSON
    public class UserDataDTO
    {
        public required string  Cpf { get; set; }
        public required string Email { get; set; }
        public required string  Username { get; set; }
        public required string  FullName { get; set; }
        public required string  Role { get; set; }
    }

    // Esta é a resposta principal enviada ao frontend
    public class LoginResponseDTO
    {
  public required UserDataDTO User { get; set; }

        public bool ForcePasswordReset { get; set; }
    }
}