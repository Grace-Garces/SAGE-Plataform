using System.ComponentModel.DataAnnotations;

namespace SagePlataform.DTOs
{
    public class RegisterUserDTO
    {
        // Esta propriedade deve se chamar "NomeCompleto" (sem underscore)
        // O C# vai mapear o "nomeCompleto" do JSON para esta propriedade automaticamente.
        [Required]
        public required string  NomeCompleto { get; set; }

        [Required]
        public required string  Cpf { get; set; }

        [Required]
        [EmailAddress]
        public required string  Email { get; set; }

        [Required]
        public required string  Usuario { get; set; }

        [Required]
        public required string  Senha { get; set; }

        [Required]
        public required string  Role { get; set; } 
    }
}
