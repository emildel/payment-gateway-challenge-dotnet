using PaymentGateway.Api.Enums;
using PaymentGateway.Api.HttpClient.BankSimulator;
using PaymentGateway.Api.Interfaces;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.Services;

public class PaymentsRepository : IPaymentsRepository
{
    private readonly Dictionary<Guid, PaymentResponse> _payments;
    
    private readonly IBankSimulatorClient _bankSimulatorClient;

    public PaymentsRepository(IBankSimulatorClient bankSimulatorClient, Dictionary<Guid, PaymentResponse> paymentsDict)
    {
        this._bankSimulatorClient = bankSimulatorClient;
        this._payments = paymentsDict;
    }
    
    public async Task<PaymentResponse?> Add(PostPaymentRequest payment)
    {
        var bankSimulatorResponse = await _bankSimulatorClient.PostPayment(payment);

        // If the call to the acquiring bank was not successful, eg. malformed request
        if (!bankSimulatorResponse.IsSuccess)
        {
            return null;
        }

        PaymentResponse processedPayment = new()
        {
            Id = Guid.NewGuid(),
            CardNumberLastFour = int.Parse(payment.CardNumber[^4..]),
            ExpiryMonth = payment.ExpiryMonth,
            ExpiryYear = payment.ExpiryYear,
            Currency = payment.Currency,
            Amount = payment.Amount
        };
        
        // If the acquiring bank returned a 200, we know the request was valid.
        // Therefore, status can be either authorized or unauthorized
        if (bankSimulatorResponse.SuccessResponse != null)
        {
            processedPayment.Status = bankSimulatorResponse.SuccessResponse.Authorized
                ? PaymentStatus.Authorized
                : PaymentStatus.Declined;
        }
        
        _payments.Add(processedPayment.Id, processedPayment);

        return processedPayment;
    }

    public PaymentResponse? Get(Guid id)
    {
        // Returns null if no payment found with matching ID
        return _payments.GetValueOrDefault(id);
    }
}