using BlogAPI;
using BlogAPI.Data;
using BlogAPI.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var key = Encoding.ASCII.GetBytes(Configuration.JwtKey);
//informa o esquema de autenticação
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
    //informa como desencriptar esse token e como vai lidar 
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true, //valida a chave da assinatura 
        IssuerSigningKey = new SymmetricSecurityKey(key), //atraves da chave assimetrica
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

builder.Services.AddControllers();
builder.Services.AddDbContext<Context>();

builder.Services.AddTransient<TokenService>();

//Configura o comportamento da API para desabilitar o filtro de ModelState do ASP.NET (validação automatica)
builder.Services.AddControllers().ConfigureApiBehaviorOptions(options => options.SuppressModelStateInvalidFilter = true);

var app = builder.Build();

//informa que precisa usar a autenticação e autorização
//sempre nessa ordem
app.UseAuthentication();
app.UseAuthorization();

//Carrega os valores de configuração
Configuration.JwtKey = app.Configuration.GetValue<string>("JwtKey");

var smtp = new Configuration.SmtpConfiguration();
app.Configuration.GetSection("SmtpConfiguration").Bind(smtp);
Configuration.Smtp = smtp;

app.MapControllers();

app.Run();
