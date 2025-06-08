using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SagePlataform.Models
{
    [Table("InsumosCantina")]
    public class InsumoCantina
    {
        [Key]
        [Column("InsumoID")]
        public int InsumoID { get; set; }

        [Column("Nome")]
        public required string  Nome { get; set; }

        [Column("QuantidadeEstoque")]
        public int QuantidadeEstoque { get; set; }

        [Column("UnidadeMedida")]
        public required string  UnidadeMedida { get; set; }

        [Column("CustoUnitario")]
        public decimal CustoUnitario { get; set; }
    }
}