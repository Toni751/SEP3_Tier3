using System.Text.Json.Serialization;

namespace SEP3_T3.Models
{
    public class LoginCredentials
    {
        [JsonPropertyName("email")]
        public string Email { get; set; }
        
        [JsonPropertyName("password")]
        public string Password { get; set; }
    }
}