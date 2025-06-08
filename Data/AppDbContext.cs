using Microsoft.EntityFrameworkCore;
using SagePlataform.Models;
using System.ComponentModel.DataAnnotations.Schema; // Certifique-se de que este using está aqui

namespace SagePlataform.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Solicitacao> Solicitacoes { get; set; }
        public DbSet<GlobalConfig> GlobalConfigs { get; set; }
        public DbSet<InsumoCantina> InsumosCantina { get; set; }
        public DbSet<UtensilioCantina> UtensiliosCantina { get; set; }
        public DbSet<SolicitacaoAcesso> SolicitacoesAcesso { get; set; }
        public DbSet<FluxoFinanceiroEntradas> FluxoFinanceiroEntradas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // 1. SEMPRE chame o base.OnModelCreating como primeira linha
            base.OnModelCreating(modelBuilder); 

            // 2. Configuração da chave primária para GlobalConfig
            // Use 'ConfigKey' como PK se for única e você deseja isso.
            // Se 'ConfigID' for a PK numérica autoincrementável, mude para:
            // modelBuilder.Entity<GlobalConfig>().HasKey(gc => gc.ConfigID);
            // modelBuilder.Entity<GlobalConfig>().Property(gc => gc.ConfigID).ValueGeneratedOnAdd();
            modelBuilder.Entity<GlobalConfig>()
                .HasKey(gc => gc.ConfigKey);

            // 3. Configuração da coluna computada para Solicitacao (apenas UMA vez)
            modelBuilder.Entity<Solicitacao>()
                .Property(s => s.ValorTotalSolicitacao)
                .ValueGeneratedOnAddOrUpdate()
                ;

            // 4. Configuração de precisão para propriedades decimal (para eliminar avisos e truncamento)
            modelBuilder.Entity<Solicitacao>()
                .Property(s => s.ValorUnitario)
                .HasPrecision(18, 2); 
            
            // Mantenha esta configuração se a sua entidade GlobalConfig tem um campo 'ConfigValue' do tipo decimal
            modelBuilder.Entity<GlobalConfig>()
                .Property(gc => gc.ConfigValue)
                .HasPrecision(18, 2);

            modelBuilder.Entity<FluxoFinanceiroEntradas>()
                .Property(e => e.Valor)
                .HasPrecision(18, 2);

            modelBuilder.Entity<UtensilioCantina>()
                .Property(u => u.CustoUnitario) 
                .HasPrecision(18, 2);
            
            // Se você tiver uma propriedade decimal em InsumoCantina, adicione-a aqui também:
            // modelBuilder.Entity<InsumoCantina>()
            //    .Property(i => i.CustoUnitario) // ou o nome da sua propriedade decimal
            //    .HasPrecision(18, 2);


            // 5. Mapeamento da tabela Usuarios (se a classe Usuario não for chamada 'User')
            modelBuilder.Entity<Usuario>().ToTable("Usuarios", "dbo");
            
            // Adicione outros mapeamentos de tabela se os nomes das suas classes não correspondem aos nomes das tabelas no BD
            // modelBuilder.Entity<Solicitacao>().ToTable("Solicitacoes", "dbo");
            // modelBuilder.Entity<GlobalConfig>().ToTable("GlobalConfigs", "dbo");
            // modelBuilder.Entity<InsumoCantina>().ToTable("InsumosCantina", "dbo");
            // modelBuilder.Entity<UtensilioCantina>().ToTable("UtensiliosCantina", "dbo");
            // modelBuilder.Entity<SolicitacaoAcesso>().ToTable("SolicitacoesAcesso", "dbo");
            // modelBuilder.Entity<FluxoFinanceiroEntradas>().ToTable("FluxoFinanceiro_Entradas", "dbo");
        }
    }
}