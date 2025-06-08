using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SagePlataform.Data;
using SagePlataform.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic; // Necessário para List<object>

namespace SagePlataform.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SolicitacaoController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SolicitacaoController(AppDbContext context)
        {
            _context = context;
        }

        public class SolicitacaoCreateDTO
        {
            public required string Solicitante { get; set; }
            public required string TipoItem { get; set; }
            public int ItemID { get; set; }
            public required string NomeItem { get; set; }
            public decimal ValorUnitario { get; set; }
            public int QuantidadeSolicitada { get; set; }
            public required string Justificativa { get; set; }
        }

        [HttpGet]
        public async Task<IActionResult> GetSolicitacoes()
        {
            return Ok(await _context.Solicitacoes.ToListAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSolicitacao(int id)
        {
            var solicitacao = await _context.Solicitacoes.FindAsync(id);
            if (solicitacao == null)
            {
                return NotFound();
            }
            return Ok(solicitacao);
        }

        [HttpGet("minhas/{solicitanteUsername}")]
        public async Task<IActionResult> GetMinhasSolicitacoes(string solicitanteUsername)
        {
            var solicitacoes = await _context.Solicitacoes
                                                 .Where(s => s.Solicitante == solicitanteUsername)
                                                 .OrderByDescending(s => s.DataSolicitacao)
                                                 .ToListAsync();
            return Ok(solicitacoes);
        }

        [HttpGet("gastos-aprovados-mes")]
        public async Task<IActionResult> GetGastosAprovadosNoMes()
        {
            var currentMonth = DateTime.Now.Month;
            var currentYear = DateTime.Now.Year;

            var totalGastosMes = await _context.Solicitacoes
                .Where(s => (s.Status == "Aprovada" || s.Status == "Pendente") &&
                            s.DataSolicitacao.Month == currentMonth &&
                            s.DataSolicitacao.Year == currentYear)
                .SumAsync(s => s.ValorTotalSolicitacao);

            return Ok(new { totalGastosAprovadosMes = totalGastosMes });
        }

        [HttpGet("pendente-admin")]
        public async Task<IActionResult> GetPendingAdminSolicitations()
        {
            var solicitacoes = await _context.Solicitacoes
                                             .Where(s => s.Status == "Pendente")
                                             .OrderByDescending(s => s.DataSolicitacao)
                                             .ToListAsync();
            return Ok(solicitacoes);
        }

        [HttpGet("pendente-master")]
        public async Task<IActionResult> GetPendingMasterSolicitations() 
        {
            var solicitacoes = await _context.Solicitacoes
                                             .Where(s => s.Status == "Pendente - Aprovacao Master") 
                                             .OrderByDescending(s => s.DataSolicitacao)
                                             .ToListAsync();
            return Ok(solicitacoes);
        }

        [HttpGet("saidas-periodo")]
        public async Task<IActionResult> GetSaidasPorPeriodo([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            var saidas = await _context.Solicitacoes
                                      .Where(s => (s.Status == "Aprovada" || s.Status == "Pendente") && 
                                                  s.DataSolicitacao >= startDate && s.DataSolicitacao <= endDate)
                                      .OrderByDescending(s => s.DataSolicitacao)
                                      .ToListAsync();
            return Ok(saidas);
        }
        
        [HttpGet("gastos-previstos")]
        public async Task<IActionResult> GetGastosPrevistos()
        {
            var thirtyDaysAgo = DateTime.Now.AddDays(-30);

            var totalGastosPrevistos = await _context.Solicitacoes
                .Where(s => s.Status == "Pendente" && s.DataSolicitacao >= thirtyDaysAgo)
                .SumAsync(s => s.ValorTotalSolicitacao);

            return Ok(new { totalGastosPrevistos = totalGastosPrevistos });
        }

        [HttpGet("gastos-aprovados-periodo/{days}")]
        public async Task<IActionResult> GetGastosAprovadosPorPeriodo(int days)
        {
            var startDate = DateTime.Now.AddDays(-days);

            var gastos = await _context.Solicitacoes
                .Where(s => s.Status == "Aprovada" && s.DataAcao.HasValue && s.DataAcao.Value >= startDate) // CORREÇÃO
                .ToListAsync();
            
            return Ok(gastos);
        }

        [HttpGet("gastos-aprovados-meses/{months}")]
        public async Task<IActionResult> GetGastosAprovadosPorMeses(int months)
        {
            var currentMonth = DateTime.Now.Month;
            var currentYear = DateTime.Now.Year;

            var monthlyExpenses = new List<object>();

            for (int i = 0; i < months; i++)
            {
                var month = (currentMonth - i - 1 + 12) % 12 + 1;
                var year = currentYear;
                if (currentMonth - i - 1 < 0) year--;

                var totalExpenses = await _context.Solicitacoes
                    .Where(s => s.Status == "Aprovada" && s.DataAcao.HasValue && // CORREÇÃO
                                s.DataAcao.Value.Month == month && // CORREÇÃO
                                s.DataAcao.Value.Year == year)     // CORREÇÃO
                    .SumAsync(s => s.ValorTotalSolicitacao);
                
                monthlyExpenses.Add(new { MonthYear = $"{year}-{month:00}", TotalExpenses = totalExpenses });
            }

            return Ok(monthlyExpenses.OrderBy(m => ((dynamic)m).MonthYear));
        }

        [HttpGet("ultimas-compras-aprovadas/{count}")]
        public async Task<IActionResult> GetUltimasComprasAprovadas(int count)
        {
            var compras = await _context.Solicitacoes
                .Where(s => s.Status == "Aprovada" && s.DataAcao.HasValue) // CORREÇÃO
                .OrderByDescending(s => s.DataAcao.Value) // CORREÇÃO
                .Take(count)
                .ToListAsync();
            
            return Ok(compras);
        }


        [HttpPost("aprovar-admin/{id}")]
        public async Task<IActionResult> AprovarSolicitacaoAdmin(int id, [FromBody] string aprovadorUsername)
        {
            var solicitacao = await _context.Solicitacoes.FindAsync(id);
            if (solicitacao == null) return NotFound();

            if (solicitacao.Status != "Pendente")
            {
                return BadRequest("Esta solicitação não está no status 'Pendente' para aprovação.");
            }

            solicitacao.Status = "Aprovada";
            solicitacao.Aprovador = aprovadorUsername;
            solicitacao.DataAcao = DateTime.Now;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Solicitação aprovada com sucesso!" });
        }

        [HttpPost("rejeitar/{id}")]
        public async Task<IActionResult> RejeitarSolicitacao(int id, [FromBody] string aprovadorUsername)
        {
            var solicitacao = await _context.Solicitacoes.FindAsync(id);
            if (solicitacao == null) return NotFound();

            if (solicitacao.Status != "Pendente" && solicitacao.Status != "Aprovada")
            {
                return BadRequest("Esta solicitação não pode ser rejeitada neste status atual.");
            }

            solicitacao.Status = "Recusada";
            solicitacao.Aprovador = aprovadorUsername;
            solicitacao.DataAcao = DateTime.Now;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Solicitação rejeitada com sucesso!" });
        }


        [HttpPost("criar")]
        public async Task<IActionResult> CriarSolicitacao([FromBody] SolicitacaoCreateDTO dto)
        {
            var novaSolicitacao = new Solicitacao
            {
                Solicitante = dto.Solicitante,
                TipoItem = dto.TipoItem,
                ItemID = dto.ItemID,
                NomeItem = dto.NomeItem,
                ValorUnitario = dto.ValorUnitario,
                QuantidadeSolicitada = dto.QuantidadeSolicitada,
                Justificativa = dto.Justificativa,
                Status = "Pendente",
                DataSolicitacao = DateTime.Now
            };

            _context.Solicitacoes.Add(novaSolicitacao);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetSolicitacao), new { id = novaSolicitacao.SolicitacaoID }, new { message = "Solicitação criada com sucesso!", solicitacaoId = novaSolicitacao.SolicitacaoID });
        }

        [HttpPut("atualizarStatus/{id}")]
        public async Task<IActionResult> AtualizarStatus(int id, [FromBody] string novoStatus)
        {
            var solicitacao = await _context.Solicitacoes.FindAsync(id);
            if (solicitacao == null)
            {
                return NotFound();
            }

            if (novoStatus != "Pendente" && novoStatus != "Aprovada" && novoStatus != "Recusada")
            {
                return BadRequest("Status inválido. Valores permitidos: 'Pendente', 'Aprovada', 'Recusada'.");
            }

            solicitacao.Status = novoStatus;
            solicitacao.DataAcao = DateTime.Now;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Solicitacoes.Any(e => e.SolicitacaoID == id))
                {
                    return NotFound();
                }
                    throw;
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSolicitacao(int id)
        {
            var solicitacao = await _context.Solicitacoes.FindAsync(id);
            if (solicitacao == null)
            {
                return NotFound();
            }

            _context.Solicitacoes.Remove(solicitacao);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}