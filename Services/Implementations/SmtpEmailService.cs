using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using SagePlataform.Services.Interfaces;
using System;
using System.Threading.Tasks;

namespace SagePlataform.Services.Implementations
{
    public class SmtpEmailService : IEmailService
    {
        private readonly string _smtpServer;
        private readonly int _smtpPort;
        private readonly string _smtpUsername;
        private readonly string _smtpPassword;
        private readonly string _fromEmail;
        private readonly string _fromName;

        public SmtpEmailService(
            string smtpServer,
            int smtpPort,
            string smtpUsername,
            string smtpPassword,
            string fromEmail,
            string fromName)
        {
            _smtpServer = smtpServer;
            _smtpPort = smtpPort;
            _smtpUsername = smtpUsername;
            _smtpPassword = smtpPassword;
            _fromEmail = fromEmail;
            _fromName = fromName;
        }

        public async Task SendCredentialsEmailAsync(string toEmail, string username, string password)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_fromName, _fromEmail));
            message.To.Add(new MailboxAddress(username, toEmail)); // Pode adicionar o nome do usuário aqui também
            message.Subject = "Suas credenciais de acesso ao SAGE";

            // NOVO TEMPLATE HTML APLICADO AQUI
            var builder = new BodyBuilder
            {
                HtmlBody = $@"
                <!DOCTYPE html>
                <html lang=""pt-br"">
                <head>
                    <meta charset=""UTF-8"">
                    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                    <link href=""https://fonts.googleapis.com/css2?family=Inter:wght@400;600;700&display=swap"" rel=""stylesheet"">
                    <style>
                        :root {{
                            --brand-main: #5a67d8;
                            --bg-primary: #f8f9fa;
                            --surface-card: #ffffff;
                            --text-dark: #343a40;
                            --text-medium: #6c757d;
                            --border-light: #e0e6ed;
                        }}
                        body {{
                            margin: 0;
                            padding: 0;
                            font-family: 'Inter', sans-serif;
                            background-color: var(--bg-primary);
                            color: var(--text-dark);
                            -webkit-font-smoothing: antialiased;
                            -moz-osx-font-smoothing: grayscale;
                        }}
                        .email-container {{
                            max-width: 600px;
                            margin: 20px auto;
                            background-color: var(--surface-card);
                            border-radius: 8px;
                            overflow: hidden;
                            border: 1px solid var(--border-light);
                            box-shadow: 0 4px 15px rgba(0,0,0,0.08);
                        }}
                        .email-header {{
                            background-color: var(--brand-main);
                            color: white;
                            padding: 25px 30px;
                            text-align: center;
                        }}
                        .email-header h1 {{
                            margin: 0;
                            font-size: 2.5rem;
                            font-weight: 800;
                            letter-spacing: 0.1em;
                        }}
                        .email-body {{
                            padding: 30px 35px;
                            line-height: 1.6;
                            font-size: 16px;
                        }}
                        .email-body h2 {{
                            color: var(--brand-main);
                            font-size: 22px;
                            margin-top: 0;
                        }}
                        .email-body p {{
                            margin: 0 0 15px;
                        }}
                        .credentials-box {{
                            background-color: var(--bg-primary);
                            border: 1px dashed var(--border-light);
                            border-radius: 8px;
                            padding: 20px;
                            margin: 25px 0;
                            font-size: 16px;
                        }}
                        .credentials-box strong {{
                            color: var(--text-dark);
                            font-weight: 600;
                        }}
                        .credentials-box span {{
                            background-color: #e0e6ed;
                            padding: 4px 8px;
                            border-radius: 4px;
                            font-family: monospace;
                            font-weight: 600;
                        }}
                        .cta-button {{
                            display: inline-block;
                            background-color: #28a745;
                            color: #ffffff;
                            padding: 12px 25px;
                            border-radius: 5px;
                            text-decoration: none;
                            font-weight: 600;
                            font-size: 16px;
                            margin-top: 15px;
                            text-align: center;
                        }}
                         .email-footer {{
                            background-color: var(--bg-primary);
                            padding: 20px 30px;
                            text-align: center;
                            font-size: 12px;
                            color: var(--text-medium);
                            border-top: 1px solid var(--border-light);
                        }}
                    </style>
                </head>
                <body>
                    <div class=""email-container"">
                        <div class=""email-header"">
                            <h1>SAGE</h1>
                        </div>
                        <div class=""email-body"">
                            <h2>Bem-vindo(a) à Plataforma!</h2>
                            <p>Olá, {username},</p>
                            <p>Seu acesso à plataforma <strong>SAGE</strong> foi aprovado com sucesso. Estamos felizes em ter você conosco.</p>
                            <p>Abaixo estão suas credenciais de acesso. Guarde-as em um local seguro.</p>
                            
                            <div class=""credentials-box"">
                                <p style=""margin-bottom: 10px;""><strong>Usuário:</strong> {username}</p>
                                <p style=""margin: 0;""><strong>Senha Temporária:</strong> <span>{password}</span></p>
                            </div>
                            
                            <p>Para garantir a segurança da sua conta, recomendamos fortemente que você <strong>altere sua senha no primeiro login</strong>.</p>
                            
                            <a href=""http://127.0.0.1:5500/FrontEnd/LoginAcesso/login.html"" class=""cta-button"">Acessar a Plataforma</a> 
                            
                            <p style=""margin-top: 30px;"">Atenciosamente,</p>
                            <p><strong>Equipe SAGE</strong></p>
                        </div>
                        <div class=""email-footer"">
                            <p>&copy; {DateTime.Now.Year} SAGE. Todos os direitos reservados.</p>
                            <p>Este é um e-mail automático. Por favor, não responda.</p>
                        </div>
                    </div>
                </body>
                </html>
                "
            };

            message.Body = builder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(_smtpServer, _smtpPort, SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(_smtpUsername, _smtpPassword);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
        }
    }
}