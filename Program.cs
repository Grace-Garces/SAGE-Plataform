using Microsoft.EntityFrameworkCore;
using SagePlataform.Data;
using SagePlataform.Services.Implementations;
using SagePlataform.Services.Interfaces;
using SagePlataform.Services.Implementations; // Certifique-se de que este using está presente para SmtpEmailService
using SagePlataform.Services.Interfaces; // Certifique-se de que este using está presente para IEmailService

var builder = WebApplication.CreateBuilder(args);

// --- CONFIGURAÇÃO DO CONTROLLER E DB ---
builder.Services.AddControllers();
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

// --- REGISTRO DE SERVIÇOS ---
// Adicione a injeção de dependência para IAuthService e AuthService,
// e IUserService e UserService se ainda não estiverem lá.
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>(); // Mantido

// --- CONFIGURAÇÃO DE E-MAIL ---
var emailSettings = builder.Configuration.GetSection("EmailSettings");

// Obter a porta SMTP com um valor padrão, se não for encontrada
int smtpPort = emailSettings.GetValue<int>("SmtpPort", 587);

builder.Services.AddScoped<IEmailService>(provider =>
    new SmtpEmailService(
        emailSettings["SmtpServer"]!,
        smtpPort,
        emailSettings["SmtpUsername"]!,
        emailSettings["SmtpPassword"]!,
        emailSettings["FromEmail"]!,
        emailSettings["FromName"]!
    )
);

// --- CORS & SWAGGER ---
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// --- CONSTRUIR APP ---
// Esta é a linha crucial que cria a instância 'app'
var app = builder.Build();

// --- CONFIGURAR O PIPELINE DE REQUISIÇÕES HTTP ---
// A ordem dessas chamadas é importante!
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection(); // Redireciona HTTP para HTTPS
app.UseRouting(); // Permite que o roteamento de requisições funcione (muitas vezes implícito em UseEndpoints)

app.UseCors("AllowAll"); // Aplica a política CORS
app.UseAuthentication(); // Se você tiver autenticação (JWT, etc.), deve vir antes de UseAuthorization
app.UseAuthorization(); // Habilita o uso de atributos [Authorize] nos controllers

app.MapControllers(); // Mapeia os controllers para as rotas

// --- EXECUTAR O APP ---
app.Run(); // Inicia o servidor da web