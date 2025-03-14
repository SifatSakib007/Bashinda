namespace BashindaAPI.DTOs
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; } = true;
        public T? Data { get; set; }
        public string[] Errors { get; set; } = Array.Empty<string>();

        public static ApiResponse<T> SuccessResponse(T data)
        {
            return new ApiResponse<T>
            {
                Success = true,
                Data = data,
                Errors = Array.Empty<string>()
            };
        }

        public static ApiResponse<T> ErrorResponse(params string[] errors)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Data = default,
                Errors = errors
            };
        }
    }
} 