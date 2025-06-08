using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SagePlataform.Models
{
    [Table("Solicitacoes")]
    public class Solicitacao
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("SolicitacaoID")]
        public int SolicitacaoID { get; set; }

        [Column("DataSolicitacao")]
        public DateTime DataSolicitacao { get; set; }

        [Column("ItemID")]
        public int ItemID { get; set; }

        [Column("TipoItem")]
        public required string  TipoItem { get; set; } // "Insumo" ou "Utensilio"

        [Column("NomeItem")]
        public required string  NomeItem { get; set; }

        [Column("QuantidadeSolicitada")]
        public int QuantidadeSolicitada { get; set; }

        [Column("ValorUnitario")]
        public decimal ValorUnitario { get; set; }

        [Column("ValorTotalSolicitacao")]
        public decimal ValorTotalSolicitacao { get; set; }

        [Column("Justificativa")]
        public required string  Justificativa { get; set; }

        [Column("Solicitante")]
        public required string  Solicitante { get; set; }

        [Column("Status")]
           public string Status { get; set; } = "Pendente"; 

        [Column("Aprovador")]
        public  string ? Aprovador { get; set; } // Pode ser nulo inicialmente

        [Column("DataAcao")]
        public DateTime? DataAcao { get; set; } // Pode ser nulo inicialmente
    }
}