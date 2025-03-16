namespace BashindaAPI.DTOs
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; } = true;
        public T? Data { get; set; }
        public string[] Errors { get; set; } = Array.Empty<string>();
        public string? Message { get; set; }

        public static ApiResponse<T> SuccessResponse(T data, string? message = null)
        {
            return new ApiResponse<T>
            {
                Success = true,
                Data = data,
                Errors = Array.Empty<string>(),
                Message = message
            };
        }

        public static ApiResponse<T> ErrorResponse(string? message = null, params string[] errors)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Data = default,
                Errors = errors,
                Message = message
            };
        }
    }
} 