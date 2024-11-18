using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Moq;

using PaymentGateway.Api.Controllers;
using PaymentGateway.Api.Enums;
using PaymentGateway.Api.Interfaces;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.Tests;

public class PaymentsControllerTests
{
    private readonly Random _random = new();
    private readonly Mock<IPaymentsRepository> _mockPaymentsRepository;
    private readonly PaymentsController _controller;

    public PaymentsControllerTests()
    {
        _mockPaymentsRepository = new Mock<IPaymentsRepository>();
        _controller = new PaymentsController(_mockPaymentsRepository.Object);
    }
    
    [Fact]
    public void Returns200IfPaymentIsFoundWhenCallingGetPayment()
    {
        // Arrange
        var paymentId = Guid.NewGuid();
        var mockPayment = new PaymentResponse
        {
            Id = paymentId,
            Status = PaymentStatus.Authorized,
            CardNumberLastFour = 1234,
            ExpiryMonth = 7,
            ExpiryYear = 2025,
            Currency = "USD",
            Amount = 10000
        };
        
        _mockPaymentsRepository.Setup(s => s.Get(It.IsAny<Guid>()))
            .Returns(mockPayment);
        
        // Act
        var response = _controller.GetPaymentAsync(paymentId).Result as OkObjectResult;
        var returnedPayment = response.Value as PaymentResponse;
        
        // Assert
        Assert.NotNull(response);
        Assert.Equal(StatusCodes.Status200OK, response.StatusCode);
        Assert.Equal(paymentId, returnedPayment.Id);
        Assert.Equal(PaymentStatus.Authorized, returnedPayment.Status);
        Assert.Equal(1234, returnedPayment.CardNumberLastFour);
        Assert.Equal(7, returnedPayment.ExpiryMonth);
        Assert.Equal(2025, returnedPayment.ExpiryYear);
        Assert.Equal("USD", returnedPayment.Currency);
        Assert.Equal(10000, returnedPayment.Amount);
    }

    [Fact]
    public void Returns404IfPaymentNotFoundWhenCallingGetPayment()
    {
        // Arrange
        var paymentId = Guid.NewGuid();
        
        _mockPaymentsRepository.Setup(s => s.Get(It.IsAny<Guid>()))
            .Returns<PaymentResponse>(null);
        
        // Act
        var response = _controller.GetPaymentAsync(paymentId).Result as NotFoundObjectResult;
        
        // Assert
        Assert.NotNull(response);
        Assert.Equal(StatusCodes.Status404NotFound, response.StatusCode);
    }
    
    [Fact]
    public void Returns500IfExceptionIsThrownWhenCallingGetPayment()
    {
        // Arrange
        var paymentId = Guid.NewGuid();

        _mockPaymentsRepository.Setup(s => s.Get(It.IsAny<Guid>()))
            .Throws<Exception>();
        
        // Act
        var response = _controller.GetPaymentAsync(paymentId).Result as StatusCodeResult;
        
        // Assert
        Assert.NotNull(response);
        Assert.Equal(StatusCodes.Status500InternalServerError, response.StatusCode);
    }
    
    [Fact]
    public async Task Returns201IfPaymentIsProcessedWhenCallingPostPayment()
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

        var authorizedPayment = new PaymentResponse
        {
            Id = Guid.NewGuid(), Status = PaymentStatus.Authorized, CardNumberLastFour = 8112, Amount = 500
        };

        _mockPaymentsRepository.Setup(s => s.Add(It.IsAny<PostPaymentRequest>()))
            .ReturnsAsync(authorizedPayment);
        
        // Act
        var response = await _controller.PostPaymentsAsync(paymentRequest);
        var createdResult = response.Result as CreatedAtRouteResult;
        
        // Assert
        Assert.NotNull(response);
        Assert.NotNull(createdResult);
        Assert.Equal(StatusCodes.Status201Created, createdResult.StatusCode);
    }
    
    [Fact]
    public async Task Returns400IfAcquiringBankRejectedPaymentWhenCallingPostPayment()
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

        _mockPaymentsRepository.Setup(s => s.Add(It.IsAny<PostPaymentRequest>()))
            .ReturnsAsync((PaymentResponse)null);
        
        // Act
        var response = await _controller.PostPaymentsAsync(paymentRequest);
        var badRequestResult = response.Result as BadRequestObjectResult;
        
        // Assert
        Assert.NotNull(response);
        Assert.NotNull(badRequestResult);
        Assert.Equal(badRequestResult.StatusCode, StatusCodes.Status400BadRequest);
    }
    
    [Fact]
    public async Task Returns500IfExceptionOccursDuringPaymentProcessingWhenCallingPostPayment()
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

        _mockPaymentsRepository.Setup(s => s.Add(It.IsAny<PostPaymentRequest>()))
            .Throws<Exception>();
        
        // Act
        var response = await _controller.PostPaymentsAsync(paymentRequest);
        var exceptionResult = response.Result as StatusCodeResult;
        
        // Assert
        Assert.NotNull(response);
        Assert.NotNull(exceptionResult);
        Assert.Equal(exceptionResult.StatusCode, StatusCodes.Status500InternalServerError);
    }
}