using System.ComponentModel.DataAnnotations;

namespace SagePlataform.DTOs
{
    public class LoginRequestDTO
    {
        [Required]
        public required string Username { get; set; }

        [Required]
        public required string Password { get; set; }
    }
}