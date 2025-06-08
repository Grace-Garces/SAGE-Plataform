using System.ComponentModel.DataAnnotations;

namespace SagePlataform.DTOs
{
    public class AccessRequestDTO
    {
        [Required]
        public required string  NomeCompleto { get; set; }
        [Required]
        public required string  Cpf { get; set; }
        [Required]
        [EmailAddress]
        public required string  Email { get; set; }
    }
}