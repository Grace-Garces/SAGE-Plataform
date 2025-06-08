using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SagePlataform.Models
{
    [Table("SolicitacoesAcesso")]
    public class SolicitacaoAcesso
    {
        [Key]
        public int ID { get; set; }

        [Column("NOME_COMPLETO")]
        public required string  NomeCompleto { get; set; }

        [Column("CPF")]
        public required string  Cpf { get; set; }

        [Column("EMAIL")]
        public required string  Email { get; set; }

        [Column("Status")]
        public required string  Status { get; set; } // Ex: "Pendente", "Aprovada", "Rejeitada"

        [Column("DataSolicitacao")]
        public DateTime DataSolicitacao { get; set; }
    }
}