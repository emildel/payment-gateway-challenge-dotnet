using System.Text.Json.Serialization;

using PaymentGateway.Api.Enums;

namespace PaymentGateway.Api.Models.Responses;

public class PaymentResponse
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }
    
    [JsonPropertyName("status")]
    public PaymentStatus Status { get; set; }
    
    [JsonPropertyName("last_four_card_digits")]
    public int CardNumberLastFour { get; set; }
    
    [JsonPropertyName("expiry_month")]
    public int ExpiryMonth { get; set; }
    
    [JsonPropertyName("expiry_year")]
    public int ExpiryYear { get; set; }
    
    [JsonPropertyName("currency")]
    public string Currency { get; set; }
    
    [JsonPropertyName("amount")]
    public int Amount { get; set; }
}
