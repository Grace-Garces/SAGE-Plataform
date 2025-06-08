using System.Threading.Tasks;

namespace SagePlataform.Services.Interfaces
{
    public interface IEmailService
    {
        Task SendCredentialsEmailAsync(string toEmail, string username, string password);
    }
}
