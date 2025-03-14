using Microsoft.AspNetCore.Http;

namespace BashindaAPI.Services
{
    public interface IFileService
    {
        Task<string?> SaveFileAsync(IFormFile file);
        void DeleteFile(string filePath);
    }
} 