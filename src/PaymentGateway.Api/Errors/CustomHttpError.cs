using System.Text.Json.Serialization;

namespace PaymentGateway.Api.Errors;

public class CustomHttpError
{
    [JsonPropertyName("custom_code")]
    public string CustomCode { get; set; }
    
    [JsonPropertyName("message")]
    public string Message { get; set; }

    public CustomHttpError(string customCode, string message)
    {
        this.CustomCode = customCode;
        this.Message = message;
    }
}