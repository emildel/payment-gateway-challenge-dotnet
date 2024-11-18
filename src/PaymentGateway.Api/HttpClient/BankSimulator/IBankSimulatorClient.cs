using PaymentGateway.Api.HttpClient.BankSimulator.Models;
using PaymentGateway.Api.Models.Requests;

namespace PaymentGateway.Api.HttpClient.BankSimulator;

public interface IBankSimulatorClient
{
    Task<BankSimulatorResponse> PostPayment(PostPaymentRequest postPaymentRequest);
}