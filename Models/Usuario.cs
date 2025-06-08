using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SagePlataform.Models
{
    [Table("Usuarios")]
    public class Usuario
    {
        [Key]
        [Column("CPF")]
        public required string  Cpf { get; set; }

        [Column("EMAIL")]
        public required string  Email { get; set; }

        [Column("NOME_COMPLETO")]
        public required string  NomeCompleto { get; set; }

        [Column("USUARIO")]
        public required string  NomeDeUsuario { get; set; }

        [Column("SENHA")]
        public required string  SenhaHash { get; set; }

        [Column("Role")]
        public required string  Role { get; set; }

        // NOVA PROPRIEDADE: Adicione esta linha.
        // Você precisará adicionar uma coluna 'IsBlocked' (BIT, NOT NULL, DEFAULT 0) na sua tabela Usuarios.
        [Column("IsBlocked")]
        public bool IsBlocked { get; set; }
    }
}