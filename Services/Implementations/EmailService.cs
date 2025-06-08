using SagePlataform.Services.Interfaces;
using System;
using System.Threading.Tasks;

namespace SagePlataform.Services.Implementations
{
    /// <summary>
    /// Esta é uma implementação de simulação (Mock) para o serviço de email.
    /// Em um projeto real, você usaria uma biblioteca como MailKit ou SendGrid para enviar emails de verdade.
    /// </summary>
    public class EmailService : IEmailService
    {
        public Task SendCredentialsEmailAsync(string toEmail, string username, string password)
        {
            // Simula o envio do email imprimindo no console do servidor.
            Console.WriteLine("--- SIMULAÇÃO DE ENVIO DE EMAIL ---");
            Console.WriteLine($"Para: {toEmail}");
            Console.WriteLine("Assunto: Suas credenciais de acesso ao SAGE");
            Console.WriteLine("Corpo do Email:");
            Console.WriteLine($"Olá, seu acesso à plataforma SAGE foi aprovado!");
            Console.WriteLine($"Usuário: {username}");
            Console.WriteLine($"Senha Temporária: {password}");
            Console.WriteLine("Recomendamos que você altere sua senha no primeiro login.");
            Console.WriteLine("------------------------------------");

            // Em uma implementação real, o código de envio via SMTP estaria aqui.
            // Como esta é uma simulação, apenas retornamos uma tarefa completada.
            return Task.CompletedTask;
        }
    }
}
