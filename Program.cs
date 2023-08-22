using Blog.Services;
using BlogAPI;
using BlogAPI.Data;
using BlogAPI.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.IdentityModel.Tokens;
using System.IO.Compression;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

ConfigureAuthentication(builder);
ConfigureMvc(builder);
ConfigureServices(builder);

var app = builder.Build();

LoadConfiguration(app);

//informa que precisa usar a autenticação e autorização
//sempre nessa ordem
app.UseAuthentication();
app.UseAuthorization();

app.UseResponseCompression(); //ativando metodo de compressão/cache

app.UseStaticFiles(); //para renderizar imagens

app.MapControllers();

app.Run();

void LoadConfiguration(WebApplication app)
{
    //Carrega os valores de configuração
    Configuration.JwtKey = app.Configuration.GetValue<string>("JwtKey");

    var smtp = new Configuration.SmtpConfiguration();
    app.Configuration.GetSection("Smtp").Bind(smtp); //.bind pega o JSON e converte para a classe smtp
    Configuration.Smtp = smtp;
}

void ConfigureAuthentication(WebApplicationBuilder builder)
{
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
}

void ConfigureMvc(WebApplicationBuilder builder)
{
    //configurando o cache
    builder.Services.AddMemoryCache();
    builder.Services.AddResponseCompression(options =>
    {
        // options.Providers.Add<BrotliCompressionProvider>();
        options.Providers.Add<GzipCompressionProvider>();
        // options.Providers.Add<CustomCompressionProvider>();
    });
    builder.Services.Configure<GzipCompressionProviderOptions>(options =>
    {
        options.Level = CompressionLevel.Optimal;
    });

    builder.Services
        .AddControllers().ConfigureApiBehaviorOptions(options => options.SuppressModelStateInvalidFilter = true) //Configura o comportamento da API para desabilitar o filtro de ModelState do ASP.NET (validação automatica)
        .AddJsonOptions(x => //configura o json
            {
                x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles; //vai ignorar outros ciclos/nó
                x.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault; //quando tiver obj null ele não vai redenrizar, vai ignorar
            }
        );
}

void ConfigureServices(WebApplicationBuilder builder)
{
    builder.Services.AddControllers();
    builder.Services.AddDbContext<Context>();
    builder.Services.AddTransient<TokenService>();
    builder.Services.AddTransient<EmailService>();
}