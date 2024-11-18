using System.Net;
using System.Text.Json;

using Moq;
using Moq.Protected;

using PaymentGateway.Api.HttpClient.BankSimulator;
using PaymentGateway.Api.HttpClient.BankSimulator.Models;
using PaymentGateway.Api.Models.Requests;

namespace PaymentGateway.Api.Tests.HttpClient;

public class BankSimulatorClientTests
{
    private readonly Mock<IHttpClientFactory> _mockHttpClientFactory;
    private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
    private readonly BankSimulatorClient _bankSimulatorClient;

    public BankSimulatorClientTests()
    {
        _mockHttpClientFactory = new Mock<IHttpClientFactory>();
        _mockHttpMessageHandler = new Mock<HttpMessageHandler>(MockBehavior.Strict);

        var httpClient = new System.Net.Http.HttpClient(_mockHttpMessageHandler.Object)
        {
            BaseAddress = new Uri("http://localhost")
        };
        _mockHttpClientFactory.Setup(factory => factory.CreateClient(nameof(BankSimulatorClient)))
            .Returns(httpClient);
        
        _mockHttpMessageHandler
            .Protected()
            .Setup("Dispose", ItExpr.IsAny<bool>())
            .Verifiable();
        
        _bankSimulatorClient = new BankSimulatorClient(_mockHttpClientFactory.Object);
    }
    
    [Fact]
    public async Task PostPaymentReturnsSuccessWhenAcquiringBankSuccessfullyProcessesAuthorizedPayment()
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

        var successResponse = new BankSimulatorSuccessResponse { Authorized = true, AuthorizationCode = "auth_code" };
        var jsonResponse = JsonSerializer.Serialize(successResponse);
        
        var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(jsonResponse)
        };

        // Mocking HttpMessageHandler to return the mocked HttpResponseMessage
        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", 
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(httpResponseMessage);
        

        // Act
        var result = await _bankSimulatorClient.PostPayment(paymentRequest);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.True(result.SuccessResponse?.Authorized);
    }
    
    [Fact]
    public async Task PostPaymentReturnsErrorWhenAcquiringBankRejectsPayments()
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

        var errorResponse = new BankSimulatorErrorResponse("error_response");
        var jsonResponse = JsonSerializer.Serialize(errorResponse);
        
        var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest)
        {
            Content = new StringContent(jsonResponse)
        };

        // Mocking HttpMessageHandler to return the mocked HttpResponseMessage
        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", 
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(httpResponseMessage);
        

        // Act
        var result = await _bankSimulatorClient.PostPayment(paymentRequest);

        // Assert
        Assert.NotNull(result);
        Assert.True(!result.IsSuccess);
    }
    
    [Fact]
    public async Task PostPaymentReturnsErrorWhenCallToAcquiringBankFails()
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

        // Mocking HttpMessageHandler to return the mocked HttpResponseMessage
        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", 
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new HttpRequestException());
        

        // Act
        var result = await _bankSimulatorClient.PostPayment(paymentRequest);

        // Assert
        Assert.NotNull(result);
        Assert.True(!result.IsSuccess);
    }
}