using BlogAPI.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddDbContext<Context>();

var app = builder.Build();

app.MapControllers();

app.Run();
