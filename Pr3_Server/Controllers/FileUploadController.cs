using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Pr3_Server.Models;
using Pr3_Server.Services;

namespace Pr3_Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FileUploadController : ControllerBase
    {
        private readonly IFileAnalyzerService _fileAnalyzerService;
        private readonly ILogger<FileUploadController> _logger;
        private readonly FileUploadOptions _options;

        public FileUploadController(
            IFileAnalyzerService fileAnalyzerService,
            IOptions<FileUploadOptions> options,
            ILogger<FileUploadController> logger)
        {
            _fileAnalyzerService = fileAnalyzerService;
            _options = options.Value;
            _logger = logger;
        }

        [HttpPost("/upload")]
        public async Task<IActionResult> Upload()
        {
            try
            {
                var request = HttpContext.Request;

                if (!request.HasFormContentType)
                {
                    return BadRequest("Некорректный тип контента. Ожидается multipart/form-data");
                }

                var form = request.Form;
                var file = form.Files.FirstOrDefault();

                if (file == null || file.Length == 0)
                {
                    return BadRequest("Файл не был загружен.");
                }

                var fileName = Path.GetFileName(file.FileName);
                var uniqueFileName = $"{Guid.NewGuid()}_{fileName}";
                var uploadsDir = Path.Combine(Directory.GetCurrentDirectory(), _options.UploadsDirectory);
                var filePath = Path.Combine(uploadsDir, uniqueFileName);

                Directory.CreateDirectory(uploadsDir);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Запускаем анализ файла
                var result = await _fileAnalyzerService.AnalyzeFileAsync(filePath);
                
                // Сохраняем результат анализа
                _fileAnalyzerService.SaveAnalysisResult(uniqueFileName, result);

                // Возвращаем результат анализа
                return Content(result.ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при обработке загрузки файла");
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }
    }
} 