using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.Interfaces;

public interface IPaymentsRepository
{
    Task<PaymentResponse?> Add(PostPaymentRequest payment);
    PaymentResponse? Get(Guid id);
}