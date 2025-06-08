using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SagePlataform.Models
{
    [Table("FluxoFinanceiro_Entradas")]
    public class FluxoFinanceiroEntradas
    {
        [Key]
        public int EntradaID { get; set; }
        public decimal Valor { get; set; }
        public DateTime DataEntrada { get; set; }
        public required string Descricao { get; set; }
        public required string Origem { get; set; }
    }
}