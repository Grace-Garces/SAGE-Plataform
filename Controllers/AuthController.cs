using Microsoft.AspNetCore.Mvc;
using SagePlataform.DTOs;
using SagePlataform.Services.Interfaces;
using System.Threading.Tasks;

namespace SagePlataform.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        // --- Endpoints de Login e Registro (Existentes) ---
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO loginRequest)
        {
            var loginResponse = await _authService.LoginAsync(loginRequest);
            if (loginResponse == null)
            {
                return Unauthorized(new { message = "Usuário ou senha inválidos." });
            }
            return Ok(loginResponse);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDTO registerRequest)
        {
            var result = await _authService.RegisterUserAsync(registerRequest);
            if (result)
            {
                return Ok(new { message = "Usuário registrado com sucesso!" });
            }
            return BadRequest(new { message = "Não foi possível registrar. O CPF ou nome de usuário já está em uso." });
        }

        // --- NOVOS ENDPOINTS PARA SOLICITAÇÃO DE ACESSO ---

        // POST: api/Auth/request-access
        // Chamado pela tela pública para solicitar acesso
        [HttpPost("request-access")]
        public async Task<IActionResult> RequestAccess([FromBody] AccessRequestDTO requestDto)
        {
            var result = await _authService.CreateAccessRequestAsync(requestDto);
            if (!result.Success)
            {
                return Conflict(new { message = result.Message }); // 409 Conflict se já existe
            }
            return Ok(new { message = "Sua solicitação de acesso foi enviada e está aguardando aprovação." });
        }

        // GET: api/Auth/pending-requests
        // Chamado pela tela do admin para listar as solicitações pendentes
        [HttpGet("pending-requests")]
        public async Task<IActionResult> GetPendingRequests()
        {
            var requests = await _authService.GetPendingAccessRequestsAsync();
            return Ok(requests);
        }

        // POST: api/Auth/approve-request
        // Chamado pelo modal do admin para aprovar uma solicitação e criar o usuário
        [HttpPost("approve-request")]
        public async Task<IActionResult> ApproveRequest([FromBody] ApproveRequestDTO approveDto)
        {
            var result = await _authService.ApproveAccessRequestAsync(approveDto);
            if (!result.Success)
            {
                return BadRequest(new { message = result.Message });
            }
            return Ok(new { message = "Usuário criado com sucesso! As credenciais foram enviadas por email." });
        }
        
        // POST: api/Auth/reject-request/{id}
        // Chamado pela tela do admin para rejeitar uma solicitação
        [HttpPost("reject-request/{id}")]
        public async Task<IActionResult> RejectRequest(int id)
        {
             var result = await _authService.RejectAccessRequestAsync(id);
             if (!result)
             {
                 return NotFound(new { message = "Solicitação não encontrada." });
             }
             return Ok(new { message = "Solicitação rejeitada com sucesso." });
        }
    }
}
