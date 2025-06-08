namespace SagePlataform.DTOs
{
    // DTO para enviar a lista de usuários para o frontend
    public class UserDto
    {
        public required string  Cpf { get; set; }
        public required string  NomeCompleto { get; set; }
        public required string  Email { get; set; }
        public required string  NomeDeUsuario { get; set; }
        public required string  Role { get; set; }
        public bool IsBlocked { get; set; }
    }

    // DTO para receber a atualização de perfil
    public class UpdateUserRoleDto
    {
        public required string  NewRole { get; set; }
    }
}