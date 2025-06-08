using System.ComponentModel.DataAnnotations;

namespace SagePlataform.DTOs
{
    public class ApproveRequestDTO
    {
        [Required]
        public int SolicitacaoId { get; set; }

        [Required]
        public required string Role { get; set; }

        [Required]
        public required string Username { get; set; }

    }
 }