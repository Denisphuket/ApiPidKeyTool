var builder = WebApplication.CreateBuilder(args);

// Добавление сервисов
builder.Services.AddScoped<KeyService>();      // Добавление KeyService
builder.Services.AddHttpClient<CidService>();  // Добавление CidService с поддержкой HttpClient

// Добавление контроллеров
builder.Services.AddControllers();

// Настройка Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.WebHost.UseUrls("https://localhost:7182", "http://localhost:7183");

var app = builder.Build();

// Конфигурация HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
