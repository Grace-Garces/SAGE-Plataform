// Em FluxoFinanceiroController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SagePlataform.Data;
using SagePlataform.Models;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace SagePlataform.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FluxoFinanceiroController : ControllerBase
    {
        private readonly AppDbContext _context;

        public FluxoFinanceiroController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("total-entradas")]
        public async Task<IActionResult> GetTotalEntradas()
        {
            try
            {
                decimal totalEntradas = await _context.FluxoFinanceiroEntradas.SumAsync(e => e.Valor);
                return Ok(new { totalEntradas = totalEntradas });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERRO no GetTotalEntradas: {ex.Message}");
                return StatusCode(500, "Erro interno do servidor ao calcular total de entradas.");
            }
        }

        [HttpGet("entradas-periodo")]
        public async Task<IActionResult> GetEntradasPorPeriodo([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            Console.WriteLine($"DEBUG: Buscando entradas para o período de {startDate:yyyy-MM-dd} a {endDate:yyyy-MM-dd}");
            try
            {
                var entradas = await _context.FluxoFinanceiroEntradas
                                             .Where(e => e.DataEntrada >= startDate && e.DataEntrada <= endDate)
                                             .OrderByDescending(e => e.DataEntrada)
                                             .ToListAsync();
                Console.WriteLine($"DEBUG: Encontradas {entradas.Count} entradas.");
                return Ok(entradas);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERRO no GetEntradasPorPeriodo: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
                return StatusCode(500, "Erro interno do servidor ao buscar entradas.");
            }
        }
    }
}