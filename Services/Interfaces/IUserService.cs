using SagePlataform.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SagePlataform.Services.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAllUsersAsync(string currentAdminCpf);
        Task<bool> UpdateUserRoleAsync(string cpf, string newRole);
        Task<bool> ToggleUserBlockAsync(string cpf);
        Task<bool> DeleteUserAsync(string cpf);
    }
}