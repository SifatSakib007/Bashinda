namespace Bashinda.ViewModels
{
    public class ServiceResponse<T>
    {
        public bool Success { get; set; }
        public string[] Errors { get; set; } = Array.Empty<string>();
        public T? Data { get; set; }
    }
} 