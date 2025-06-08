using System.ComponentModel.DataAnnotations;

namespace SagePlataform.DTOs
{
    public class SolicitacaoCreateDTO
    {
        [Required]
        public int ItemID { get; set; }
        [Required]
        public required string  TipoItem { get; set; }
        [Required]
        public required string  NomeItem { get; set; }
        [Required]
        public int QuantidadeSolicitada { get; set; }
        [Required]
        public decimal ValorUnitario { get; set; }
        [Required]
        public required string  Justificativa { get; set; }
        [Required]
        public required string  Solicitante { get; set; }
    }
}