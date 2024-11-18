using System.Text.Json.Serialization;

namespace PaymentGateway.Api.HttpClient.BankSimulator.Models;

public class BankSimulatorSuccessResponse
{
    [JsonPropertyName("authorized")]
    public bool Authorized { get; set; }
    
    [JsonPropertyName("authorization_code")]
    public string AuthorizationCode { get; set; }
}