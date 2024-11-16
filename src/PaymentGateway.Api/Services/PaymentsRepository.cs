using System.Collections.Concurrent;

using PaymentGateway.Api.Interfaces;
using PaymentGateway.Api.Models;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.Services;

public class PaymentsRepository : IPaymentsRepository
{
    private static readonly ConcurrentDictionary<Guid, PostPaymentResponse> _payments = new();
    
    public async Task<PostPaymentResponse> Add(PostPaymentRequest payment)
    {
        /*
         _payments.Add(new PostPaymentResponse
        {
            Id = new Guid(),
            Status = ResponseExtensions.Status ? PaymentStatus.Authorized : PaymentStatus.Declined,
            CardNumberLastFour = payment.CardNumber
            
        });
        */
        
        // Do mapping from PostPaymentRequest to PostPAymentResponse
        
        //Payments.Add(payment);
    }

    public PostPaymentResponse? Get(Guid id)
    {
        // Returns null if no payment found with matching ID
        return _payments.TryGetValue(id, out var payment) ? payment : null;
    }
}