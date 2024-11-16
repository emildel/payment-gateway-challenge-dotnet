using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.Interfaces;

public interface IPaymentsRepository
{
    Task<PostPaymentResponse> Add(PostPaymentRequest payment);
    PostPaymentResponse? Get(Guid id);
}