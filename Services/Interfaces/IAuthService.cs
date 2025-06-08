using SagePlataform.DTOs;
using SagePlataform.Models; // Adicionado para reconhecer 'SolicitacaoAcesso'
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SagePlataform.Services.Interfaces
{
    public interface IAuthService
    {
        // Adicionado '?' para indicar que a resposta pode ser nula, corrigindo o aviso CS8603
        Task<LoginResponseDTO?> LoginAsync(LoginRequestDTO loginRequest);

        Task<bool> RegisterUserAsync(RegisterUserDTO registerRequest);

        // Assinaturas para o novo fluxo de solicitação de acesso
        Task<(bool Success, string Message)> CreateAccessRequestAsync(AccessRequestDTO requestDto);
        Task<IEnumerable<SolicitacaoAcesso>> GetPendingAccessRequestsAsync();
        Task<(bool Success, string Message)> ApproveAccessRequestAsync(ApproveRequestDTO approveDto);
        Task<bool> RejectAccessRequestAsync(int solicitacaoId);
    }
}
