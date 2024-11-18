using System.Reflection;

using Moq;

using PaymentGateway.Api.Enums;
using PaymentGateway.Api.HttpClient.BankSimulator;
using PaymentGateway.Api.HttpClient.BankSimulator.Models;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Api.Services;

namespace PaymentGateway.Api.Tests;

public class PaymentsRepositoryTests
{
    private readonly Mock<IBankSimulatorClient> _mockBankSimulatorClient;
    private readonly PaymentsRepository _paymentsRepository;
    private readonly Dictionary<Guid, PaymentResponse> _paymentsDictionary = new();

    public PaymentsRepositoryTests()
    {
        _mockBankSimulatorClient = new Mock<IBankSimulatorClient>();
        _paymentsRepository = new PaymentsRepository(_mockBankSimulatorClient.Object, _paymentsDictionary);
    }
    
    [Fact]
    public async Task AddReturnsPaymentWhenAcquiringBankAcceptsPayment()
    {
        // Arrange
        var paymentRequest = new PostPaymentRequest
        {
            CardNumber = "2222405343248112",
            ExpiryMonth = 5,
            ExpiryYear = 2026,
            Currency = "USD",
            Amount = 500,
            Cvv = "1234"
        };

        var bankSimSuccessResponse = new BankSimulatorSuccessResponse() { Authorized = true, AuthorizationCode = "1234"};
        
        var bankSimulatorResponse = new BankSimulatorResponse(true, bankSimSuccessResponse, null);

        _mockBankSimulatorClient.Setup(s => s.PostPayment(It.IsAny<PostPaymentRequest>()))
            .ReturnsAsync(bankSimulatorResponse);

        // Act
        var result = await _paymentsRepository.Add(paymentRequest);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(paymentRequest.Amount, result?.Amount);
        Assert.Equal(PaymentStatus.Authorized, result?.Status);
        Assert.Equal("USD", result?.Currency);
        Assert.Equal(paymentRequest.ExpiryMonth, result?.ExpiryMonth);
        Assert.Equal(paymentRequest.ExpiryYear, result?.ExpiryYear);
        Assert.Equal(8112, result?.CardNumberLastFour);
    }
    
    [Fact]
    public async Task AddReturnsNullWhenAcquiringBankRejectsPayment()
    {
        // Arrange
        var paymentRequest = new PostPaymentRequest
        {
            CardNumber = "2222405343248112",
            ExpiryMonth = 5,
            ExpiryYear = 2026,
            Currency = "USD",
            Amount = 500,
            Cvv = "1234"
        };
        
        var bankSimulatorResponse = new BankSimulatorResponse(false, null, 
            new BankSimulatorErrorResponse("banking_error"));
        
        _mockBankSimulatorClient.Setup(client => client.PostPayment(It.IsAny<PostPaymentRequest>()))
            .ReturnsAsync(bankSimulatorResponse);

        // Act
        var result = await _paymentsRepository.Add(paymentRequest);

        // Assert
        Assert.Null(result); // Payment should be null if the bank simulator failed
    }
    
    [Fact]
    public void GetReturnsPaymentWhenPaymentExists()
    {
        // Arrange
        var paymentId = Guid.NewGuid();
        var payment = new PaymentResponse
        {
            Id = paymentId,
            Amount = 100,
            Status = PaymentStatus.Authorized,
            Currency = "USD"
        };

        // Add the payment to the repository (in-memory dictionary)
        _paymentsDictionary.Add(paymentId, payment);

        // Act
        var result = _paymentsRepository.Get(paymentId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(paymentId, result?.Id);
        Assert.Equal(100, result?.Amount);
        Assert.Equal(PaymentStatus.Authorized, result?.Status);
    }
    
    [Fact]
    public void GetReturnsNullWhenPaymentDoesNotExist()
    {
        // Arrange
        var paymentId = Guid.NewGuid();

        // Act
        var result = _paymentsRepository.Get(paymentId);

        // Assert
        Assert.Null(result);
    }
}