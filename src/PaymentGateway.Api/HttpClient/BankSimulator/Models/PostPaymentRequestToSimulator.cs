using System.Text.Json.Serialization;

namespace PaymentGateway.Api.HttpClient.BankSimulator.Models;

public class PostPaymentRequestToSimulator
{
    [JsonPropertyName("card_number")]
    public string CardNumber { get; set; }

    [JsonPropertyName("expiry_date")]
    public string ExpiryDate { get; set; }
    
    [JsonPropertyName("currency")]
    public string Currency { get; set; }
    
    [JsonPropertyName("amount")]
    public int Amount { get; set; }
    
    [JsonPropertyName("cvv")]
    public int Cvv { get; set; }
}