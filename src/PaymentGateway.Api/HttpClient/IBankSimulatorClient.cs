using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.HttpClient;

public interface IBankSimulatorClient
{
    Task<PostPaymentResponse?> PostPayment(PostPaymentRequest postPaymentRequest);
}