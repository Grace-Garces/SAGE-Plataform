using Microsoft.EntityFrameworkCore;
using SagePlataform.Data;
using SagePlataform.DTOs;
using SagePlataform.Models;
using SagePlataform.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SagePlataform.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly IEmailService _emailService; // Adiciona a dependência do serviço de email

        // O construtor agora recebe o IEmailService via injeção de dependência
        public AuthService(AppDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }
        
        // --- Método de Login ---
        public async Task<LoginResponseDTO?> LoginAsync(LoginRequestDTO loginRequest)
        {
            var user = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.NomeDeUsuario == loginRequest.Username);

            if (user == null || !BCrypt.Net.BCrypt.Verify(loginRequest.Password, user.SenhaHash))
            {
                return null;
            }
            
            var response = new LoginResponseDTO
            {
                ForcePasswordReset = false, 
                User = new UserDataDTO
                {
                    Cpf = user.Cpf,
                    Email = user.Email,
                    Username = user.NomeDeUsuario,
                    FullName = user.NomeCompleto,
                    Role = user.Role
                }
            };
            
            return response;
        }

        // --- Método de Registro Direto ---
        public async Task<bool> RegisterUserAsync(RegisterUserDTO registerRequest)
        {
            var sanitizedCpf = new string(registerRequest.Cpf.Where(char.IsDigit).ToArray());
            var userExists = await _context.Usuarios.AnyAsync(u => u.Cpf == sanitizedCpf || u.NomeDeUsuario == registerRequest.Usuario);

            if (userExists) return false;

            var newUser = new Usuario
            {
                Cpf = sanitizedCpf,
                NomeCompleto = registerRequest.NomeCompleto,
                Email = registerRequest.Email,
                NomeDeUsuario = registerRequest.Usuario,
                SenhaHash = BCrypt.Net.BCrypt.HashPassword(registerRequest.Senha),
                Role = registerRequest.Role
            };

            await _context.Usuarios.AddAsync(newUser);
            await _context.SaveChangesAsync();
            return true;
        }

        // --- Implementação dos Métodos de Solicitação de Acesso ---

        public async Task<(bool Success, string Message)> CreateAccessRequestAsync(AccessRequestDTO requestDto)
        {
            var sanitizedCpf = new string(requestDto.Cpf.Where(char.IsDigit).ToArray());

            var existingRequest = await _context.SolicitacoesAcesso.AnyAsync(s => s.Cpf == sanitizedCpf && s.Status == "Pendente");
            if (existingRequest) return (false, "Já existe uma solicitação pendente para este CPF.");

            var existingUser = await _context.Usuarios.AnyAsync(u => u.Cpf == sanitizedCpf || u.Email == requestDto.Email);
            if (existingUser) return (false, "Já existe um usuário cadastrado com este CPF ou email.");

            var newRequest = new SolicitacaoAcesso
            {
                NomeCompleto = requestDto.NomeCompleto,
                Cpf = sanitizedCpf,
                Email = requestDto.Email,
                Status = "Pendente",
                DataSolicitacao = DateTime.UtcNow
            };

            await _context.SolicitacoesAcesso.AddAsync(newRequest);
            await _context.SaveChangesAsync();
            return (true, "Solicitação criada com sucesso.");
        }

        public async Task<IEnumerable<SolicitacaoAcesso>> GetPendingAccessRequestsAsync()
        {
            return await _context.SolicitacoesAcesso
                .Where(s => s.Status == "Pendente")
                .OrderBy(s => s.DataSolicitacao)
                .ToListAsync();
        }

        public async Task<(bool Success, string Message)> ApproveAccessRequestAsync(ApproveRequestDTO approveDto)
        {
            var accessRequest = await _context.SolicitacoesAcesso.FindAsync(approveDto.SolicitacaoId);
            if (accessRequest == null || accessRequest.Status != "Pendente")
            {
                return (false, "Solicitação não encontrada ou já processada.");
            }

            var randomPassword = GenerateRandomPassword();
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(randomPassword);

            var newUser = new Usuario
            {
                Cpf = accessRequest.Cpf,
                NomeCompleto = accessRequest.NomeCompleto,
                Email = accessRequest.Email,
                NomeDeUsuario = approveDto.Username,
                SenhaHash = passwordHash,
                Role = approveDto.Role
            };

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    await _context.Usuarios.AddAsync(newUser);
                    accessRequest.Status = "Aprovada";
                    await _context.SaveChangesAsync();
                    
                    // CORREÇÃO: Chamada ao serviço de email antes de confirmar a transação.
                    await _emailService.SendCredentialsEmailAsync(newUser.Email, newUser.NomeDeUsuario, randomPassword);

                    await transaction.CommitAsync();
                    
                    return (true, "Usuário criado com sucesso.");
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    Console.WriteLine(ex.ToString());
                    return (false, "Ocorreu um erro no banco de dados ao criar o usuário.");
                }
            }
        }
        
        public async Task<bool> RejectAccessRequestAsync(int solicitacaoId)
        {
            var accessRequest = await _context.SolicitacoesAcesso.FindAsync(solicitacaoId);
            if (accessRequest == null || accessRequest.Status != "Pendente") return false;
            
            accessRequest.Status = "Rejeitada";
            await _context.SaveChangesAsync();
            return true;
        }

        private string GenerateRandomPassword(int length = 12)
        {
            const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*()";
            var random = new Random();
            return new string(Enumerable.Repeat(validChars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
