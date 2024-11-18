using FluentValidation.TestHelper;

using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Validators;

namespace PaymentGateway.Api.Tests.Validators;

public class PostPaymentRequestValidatorTests
{
    private readonly PostPaymentRequestValidator _validator;

    public PostPaymentRequestValidatorTests()
    {
        this._validator = new PostPaymentRequestValidator();
    }

    [Fact]
    public void ValidatorShouldReturnErrorWhenInvalidCardIsProvided()
    {
        var model = new PostPaymentRequest
        {
            CardNumber = String.Empty,
            Currency = "USD",
            Cvv = "123",
        };
        
        var result = this._validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.CardNumber);
    }
    
    [Fact]
    public void ValidatorShouldNotReturnErrorWhenValidCardIsProvided()
    {
        var model = new PostPaymentRequest
        {
            CardNumber = "2222405343248112",
            ExpiryMonth = 12,
            ExpiryYear = 2026,
            Currency = "USD",
            Amount = 900,
            Cvv = "123",
        };
        
        var result = this._validator.TestValidate(model);

        result.ShouldNotHaveValidationErrorFor(x => x.CardNumber);
    }
    
    [Fact]
    public void ValidatorShouldReturnErrorWhenExpiredCardIsProvided()
    {
        var model = new PostPaymentRequest
        {
            CardNumber = "2222405343248112",
            ExpiryMonth = 10,
            ExpiryYear = 2023,
            Currency = "USD",
            Amount = 900,
            Cvv = "123",
        };
        
        var result = this._validator.TestValidate(model);

        result.ShouldHaveAnyValidationError();
    }
    
    [Fact]
    public void ValidatorShouldReturnErrorWhenUnsupportedCurrencyIsProvided()
    {
        var model = new PostPaymentRequest
        {
            CardNumber = "2222405343248112",
            ExpiryMonth = 10,
            ExpiryYear = 2023,
            Currency = "AUD",
            Amount = 900,
            Cvv = "123",
        };
        
        var result = this._validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Currency);
    }

}