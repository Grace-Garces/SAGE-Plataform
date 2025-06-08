using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SagePlataform.Data;
using SagePlataform.Models;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace SagePlataform.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GlobalConfigController : ControllerBase
    {
        private readonly AppDbContext _context;

        public GlobalConfigController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllConfigs()
        {
            var configs = await _context.GlobalConfigs.ToListAsync();
            // AGORA ConfigValue já é decimal, então a conversão é desnecessária
            var typedConfigs = configs.Select(c => new
            {
                c.ConfigKey,
                c.ConfigValue // Usa o valor decimal diretamente
            });
            return Ok(typedConfigs);
        }

        [HttpGet("{key}")]
        public async Task<IActionResult> GetConfigValue(string key)
        {
            var config = await _context.GlobalConfigs.FindAsync(key);
            if (config == null) return NotFound(new { message = $"Configuração '{key}' não encontrada." });

            // AGORA ConfigValue já é decimal, então a conversão é desnecessária
            return Ok(new { configValue = config.ConfigValue });
        }

        [HttpPost("update/{key}")]
        public async Task<IActionResult> UpdateConfig(string key, [FromBody] UpdateConfigRequest request)
        {
            var config = await _context.GlobalConfigs.FindAsync(key);

            if (config == null)
            {
                config = new GlobalConfig
                {
                    ConfigKey = key,
                    ConfigValue = request.Value, // Usa o decimal diretamente
                    LastUpdated = DateTime.Now
                };
                await _context.GlobalConfigs.AddAsync(config);
            }
            else
            {
                config.ConfigValue = request.Value; // Usa o decimal diretamente
                config.LastUpdated = DateTime.Now;
            }

            await _context.SaveChangesAsync();

            return Ok(new { message = $"Configuração '{key}' atualizada com sucesso." });
        }

        public class UpdateConfigRequest
        {
            public decimal Value { get; set; }
        }
    }

    }