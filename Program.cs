using BlogAPI.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddDbContext<Context>();

//Configura o comportamento da API para desabilitar o filtro de ModelState do ASP.NET (validação automatica)
builder.Services.AddControllers().ConfigureApiBehaviorOptions(options => options.SuppressModelStateInvalidFilter = true);

var app = builder.Build();

app.MapControllers();

app.Run();
