using Microsoft.AspNetCore.Http;

namespace BashindaAPI.Services
{
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<FileService> _logger;

        public FileService(IWebHostEnvironment env, ILogger<FileService> logger)
        {
            _env = env;
            _logger = logger;
        }

        public async Task<string?> SaveFileAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return null;
            }

            // Ensure uploads directory exists
            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
            if (!Directory.Exists(uploadsFolder))
            {
                try
                {
                    Directory.CreateDirectory(uploadsFolder);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating uploads directory: {ErrorMessage}", ex.Message);
                    throw new DirectoryNotFoundException($"Could not create uploads directory: {ex.Message}");
                }
            }

            try
            {
                // Generate a safe filename
                var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(file.FileName).Replace(" ", "_");
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                
                // Save the file
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }
                
                // Return the relative path for database storage
                return Path.Combine("uploads", uniqueFileName).Replace("\\", "/");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving file {FileName}: {ErrorMessage}", file.FileName, ex.Message);
                throw;
            }
        }

        public void DeleteFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                return;
            }

            try
            {
                var fullPath = Path.Combine(_env.WebRootPath, filePath.TrimStart('/'));
                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                    _logger.LogInformation("File deleted: {FilePath}", filePath);
                }
                else
                {
                    _logger.LogWarning("File not found for deletion: {FilePath}", filePath);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting file {FilePath}: {ErrorMessage}", filePath, ex.Message);
            }
        }
    }
} 