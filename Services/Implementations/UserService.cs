using Microsoft.EntityFrameworkCore;
using SagePlataform.Data;
using SagePlataform.DTOs;
using SagePlataform.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SagePlataform.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;

        public UserService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync(string currentAdminCpf)
        {
            return await _context.Usuarios
                .Where(u => u.Cpf != currentAdminCpf) // Não lista o admin que está logado
                .Select(u => new UserDto
                {
                    Cpf = u.Cpf,
                    NomeCompleto = u.NomeCompleto,
                    Email = u.Email,
                    NomeDeUsuario = u.NomeDeUsuario,
                    Role = u.Role,
                    IsBlocked = u.IsBlocked
                })
                .ToListAsync();
        }

        public async Task<bool> UpdateUserRoleAsync(string cpf, string newRole)
        {
            var user = await _context.Usuarios.FirstOrDefaultAsync(u => u.Cpf == cpf);
            if (user == null) return false;

            user.Role = newRole;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ToggleUserBlockAsync(string cpf)
        {
            var user = await _context.Usuarios.FirstOrDefaultAsync(u => u.Cpf == cpf);
            if (user == null) return false;

            user.IsBlocked = !user.IsBlocked; // Inverte o status
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteUserAsync(string cpf)
        {
            var user = await _context.Usuarios.FirstOrDefaultAsync(u => u.Cpf == cpf);
            if (user == null) return false;

            _context.Usuarios.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}