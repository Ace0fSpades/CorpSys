public class Program
{
    public static async Task Main(string[] args)
    {
        int numberOfClients = 3; // Количество клиентов для упрощения тестирования
        string inputDirectory = Path.Combine(Directory.GetCurrentDirectory(), "input");
        int delayMilliseconds = 500;

        if (!Directory.Exists(inputDirectory))
        {
            Console.WriteLine("Папка 'input' не существует.");
            return;
        }

        string[] files = Directory.GetFiles(inputDirectory, "*.txt");

        if (files.Length == 0)
        {
            Console.WriteLine("В папке 'input' нет текстовых файлов.");
            return;
        }

        List<Task> tasks = new List<Task>();

        for (int i = 0; i < numberOfClients; i++)
        {
            int clientNumber = i + 1;

            tasks.Add(Task.Run(async () =>
            {
                foreach (string filePath in files)
                {
                    Console.WriteLine($"Клиент {clientNumber}: Отправка файла: {Path.GetFileName(filePath)}");
                    try
                    {
                        await Task.Delay(delayMilliseconds);

                        // Отправляем файл и получаем результаты анализа в том же запросе
                        var analysisResult = await UploadFile(filePath);
                        Console.WriteLine($"Клиент {clientNumber}: Результаты анализа:\n{analysisResult}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Клиент {clientNumber}: Ошибка при отправке файла {Path.GetFileName(filePath)}: {ex.Message}");
                    }
                }
            }));
        }

        await Task.WhenAll(tasks);
        Console.WriteLine("Все клиенты завершили отправку файлов.");
    }

    private static async Task<string> UploadFile(string filePath)
    {
        var client = new HttpClient();
        var form = new MultipartFormDataContent();

        var fileContent = new StreamContent(File.OpenRead(filePath));
        form.Add(fileContent, "file", Path.GetFileName(filePath));

        var response = await client.PostAsync("http://localhost:5000/upload", form);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStringAsync();
    }
}