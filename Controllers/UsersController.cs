using Microsoft.AspNetCore.Mvc;
using SagePlataform.DTOs;
using SagePlataform.Services.Interfaces;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SagePlataform.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            // Para evitar que o admin se delete/bloqueie, pegamos o CPF do usuário logado
            // Em uma implementação com JWT real, isso viria do token. Aqui, simulamos.
            var loggedInUserCpf = "SIMULATED_CPF"; // Troque por lógica de token real se tiver

            var users = await _userService.GetAllUsersAsync(loggedInUserCpf);
            return Ok(users);
        }

        [HttpPut("{cpf}/role")]
        public async Task<IActionResult> UpdateRole(string cpf, [FromBody] UpdateUserRoleDto dto)
        {
            var success = await _userService.UpdateUserRoleAsync(cpf, dto.NewRole);
            if (!success) return NotFound(new { message = "Usuário não encontrado." });
            return Ok(new { message = "Perfil do usuário atualizado com sucesso." });
        }

        [HttpPut("{cpf}/toggle-block")]
        public async Task<IActionResult> ToggleBlock(string cpf)
        {
            var success = await _userService.ToggleUserBlockAsync(cpf);
            if (!success) return NotFound(new { message = "Usuário não encontrado." });
            return Ok(new { message = "Status do usuário alterado com sucesso." });
        }

        [HttpDelete("{cpf}")]
        public async Task<IActionResult> DeleteUser(string cpf)
        {
            var success = await _userService.DeleteUserAsync(cpf);
            if (!success) return NotFound(new { message = "Usuário não encontrado." });
            return Ok(new { message = "Usuário excluído com sucesso." });
        }
    }
}