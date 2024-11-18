using System.Text.Json.Serialization;

namespace PaymentGateway.Api.Models.Requests;

public class PostPaymentRequest
{
    [JsonPropertyName("card_number")]
    public required string CardNumber { get; set; }
    
    [JsonPropertyName("expiry_month")]
    public required int ExpiryMonth { get; set; }
    
    [JsonPropertyName("expiry_year")]
    public int ExpiryYear { get; set; }
    
    [JsonPropertyName("currency")]
    public required string Currency { get; set; }
    
    [JsonPropertyName("amount")]
    public int Amount { get; set; }
    
    [JsonPropertyName("cvv")]
    public string Cvv { get; set; }
}