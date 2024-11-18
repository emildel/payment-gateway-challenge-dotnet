using System.Text.Json;

using PaymentGateway.Api.HttpClient.BankSimulator.Models;
using PaymentGateway.Api.Models.Requests;

namespace PaymentGateway.Api.HttpClient.BankSimulator;

public class BankSimulatorClient : IBankSimulatorClient
{
    private readonly IHttpClientFactory _httpClientFactory;

    public BankSimulatorClient(IHttpClientFactory httpClientFactory)
    {
        this._httpClientFactory = httpClientFactory;
    }

    public async Task<BankSimulatorResponse> PostPayment(PostPaymentRequest postPaymentRequest)
    {
        ArgumentNullException.ThrowIfNull(postPaymentRequest);

        using var client = _httpClientFactory.CreateClient(nameof(BankSimulatorClient));

        try
        {
            // This should always succeed since we have validated the cvv is parsable to an integer in our validator
            _ = int.TryParse(postPaymentRequest.Cvv, out int cvv); 
            
            var paymentRequest = new PostPaymentRequestToSimulator
            {
                CardNumber = postPaymentRequest.CardNumber,
                ExpiryDate = $"{postPaymentRequest.ExpiryMonth:00}/{postPaymentRequest.ExpiryYear}",
                Currency = postPaymentRequest.Currency,
                Amount = postPaymentRequest.Amount,
                Cvv = cvv,
            };
            
            var response = await client.PostAsJsonAsync("/payments", paymentRequest);

            return await ResponseHandler(response);
        }
        catch (Exception ex)
        {
            // log the inner exception here
            return BankSimulatorResponse.Error();
        }
    }

    private static async Task<BankSimulatorResponse> ResponseHandler(HttpResponseMessage response)
    {
        
        var content = await response.Content.ReadAsStringAsync();
        if (response.IsSuccessStatusCode)
        {
            var successResponse = JsonSerializer.Deserialize<BankSimulatorSuccessResponse>(content);

            if (successResponse == null)
            {
                throw new JsonException("Failed to deserialize the success response");
            }
                
            return BankSimulatorResponse.Success(successResponse);
        }

        var errorResponse = JsonSerializer.Deserialize<BankSimulatorErrorResponse>(content);

        if (errorResponse == null)
        {
            throw new JsonException("Failed to deserialize the error response");
        }
        
        // log the error response from the acquiring bank => errorResponse.ErrorMessage
        return BankSimulatorResponse.Error();
    }
}