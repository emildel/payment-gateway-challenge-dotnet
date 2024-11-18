using System.Text.Json.Serialization;

namespace PaymentGateway.Api.HttpClient.BankSimulator.Models;

public class BankSimulatorErrorResponse
{
    [JsonPropertyName("errorMessage")]
    public string? ErrorMessage { get; set; }

    public BankSimulatorErrorResponse(string errorMessage)
    {
        this.ErrorMessage = errorMessage;
    }
}