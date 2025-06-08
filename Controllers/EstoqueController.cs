using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SagePlataform.Data;
using System.Linq;
using System.Threading.Tasks;

namespace SagePlataform.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EstoqueController : ControllerBase
    {
        private readonly AppDbContext _context;

        public EstoqueController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Estoque/insumos
        // Retorna a lista de insumos para o formulário
        [HttpGet("insumos")]
        public async Task<IActionResult> GetInsumos()
        {
            var insumos = await _context.InsumosCantina
                                        .OrderBy(i => i.Nome)
                                        .ToListAsync();
            return Ok(insumos);
        }

        // GET: api/Estoque/utensilios
        // Retorna a lista de utensílios para o formulário
        [HttpGet("utensilios")]
        public async Task<IActionResult> GetUtensilios()
        {
            var utensilios = await _context.UtensiliosCantina
                                           .OrderBy(u => u.Nome)
                                           .ToListAsync();
            return Ok(utensilios);
        }
    }
}