using Newtonsoft.Json;

using PaymentGateway.Api.HttpClient.ClientModels;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.HttpClient;

public class BankSimulatorClient : IBankSimulatorClient
{
    private readonly IHttpClientFactory _httpClientFactory;

    public BankSimulatorClient(IHttpClientFactory httpClientFactory)
    {
        this._httpClientFactory = httpClientFactory;
    }

    public async Task<PostPaymentResponse?> PostPayment(PostPaymentRequest postPaymentRequest)
    {
        using var client = _httpClientFactory.CreateClient(nameof(BankSimulatorClient));

        var paymentRequest = new PostPaymentRequestToSimulator
        {
            CardNumber = postPaymentRequest.CardNumber,
            ExpiryDate = $"{postPaymentRequest.ExpiryMonth}/{postPaymentRequest.ExpiryYear}",
            Currency = postPaymentRequest.Currency,
            Amount = postPaymentRequest.Amount,
            Cvv = postPaymentRequest.Cvv,
        };
        
        var response = await client.PostAsJsonAsync("/payments", paymentRequest);

        if (response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<PostPaymentResponse>(responseContent);
        }

        return null;
    }
}