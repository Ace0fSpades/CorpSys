using Pr3_Server.Models;
using Pr3_Server.Services;

var builder = WebApplication.CreateBuilder(args);

// Добавляем сервисы в контейнер
builder.Services.AddControllers();
builder.Services.AddScoped<IFileAnalyzerService, FileAnalyzerService>();
builder.Services.AddLogging();

// Регистрируем конфигурацию
builder.Services.Configure<FileUploadOptions>(options =>
{
    options.UploadsDirectory = "uploads";
    options.AnalysisResultsFile = "analysis_result.txt";
});

// Настраиваем Kestrel для прослушивания порта 5000
builder.WebHost.UseKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(5000);
});

var app = builder.Build();

// Настраиваем конвейер HTTP-запросов
app.UseRouting();
app.MapControllers();

await app.RunAsync();