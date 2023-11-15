using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Добавление сервисов
builder.Services.AddScoped<KeyService>();      // Добавление KeyService
builder.Services.AddHttpClient<CidService>();  // Добавление CidService с поддержкой HttpClient

// Добавление контроллеров
builder.Services.AddControllers();

// Настройка Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.WebHost.UseUrls("https://localhost:7181", "http://localhost:7182");
//builder.WebHost.UseUrls("http://*:7183");

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "ApiPidKeyTool", 
        Version = "v1.0.0",
        Description = "Этот проект - API, созданный для проверки ключей продуктов с использованием внешнего инструмента `PidKey.exe`. API позволяет пользователям отправлять ключи продуктов и получать информацию о их статусе и другие детали.",
        Contact = new OpenApiContact
        {
            Name = "DenisPhuket",
            Url = new Uri("https://github.com/Denisphuket/ApiPidKeyTool"),
        },
    });
});


var app = builder.Build();

// Конфигурация HTTP request pipeline
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();