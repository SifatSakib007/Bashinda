using System.Text.Json.Serialization;

namespace Bashinda.Models
{
    public class ApiResponse<T>
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("data")]
        public T Data { get; set; }

        [JsonPropertyName("errors")]
        public string[] Errors { get; set; } = Array.Empty<string>();

        [JsonPropertyName("message")]  // Add this to match WebAPI
        public string? Message { get; set; }


    }
}
