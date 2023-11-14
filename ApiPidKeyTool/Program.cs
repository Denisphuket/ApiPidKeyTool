var builder = WebApplication.CreateBuilder(args);

// Добавление сервисов
builder.Services.AddScoped<KeyService>();      // Добавление KeyService
builder.Services.AddHttpClient<CidService>();  // Добавление CidService с поддержкой HttpClient

// Добавление контроллеров
builder.Services.AddControllers();

// Настройка Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
