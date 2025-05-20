using Pr3_Server.Models;

namespace Pr3_Server.Services
{
    public interface IFileAnalyzerService
    {
        Task<FileAnalysisResult> AnalyzeFileAsync(string filePath);
        void SaveAnalysisResult(string fileName, FileAnalysisResult result);
    }
} 