using FluentValidation;

using PaymentGateway.Api.Models.Requests;

namespace PaymentGateway.Api.Validators;

public class PostPaymentRequestValidator : AbstractValidator<PostPaymentRequest>
{
    public PostPaymentRequestValidator()
    {
        RuleFor(r => r.CardNumber)
            .NotEmpty().WithMessage("Card number cannot be empty")
            .Must(cardNum => cardNum.All(char.IsDigit)).WithMessage("Card number can only contain digits")
            .Length(14,19).WithMessage("Card number must be between 14 and 19 digits");
        
        RuleFor(r => r.ExpiryMonth)
            .NotEmpty().WithMessage("Expiry month cannot be empty")
            .InclusiveBetween(1, 12).WithMessage("Expiry month must be between 1 and 12");

        RuleFor(r => r.ExpiryYear)
            .NotEmpty().WithMessage("Expiry year cannot be empty")
            .Must(BeValidYear).WithMessage("Expiry year cannot be in the past");

        RuleFor(r => new { r.ExpiryMonth, r.ExpiryYear })
            .Must(expiry => !IsExpired(expiry.ExpiryMonth, expiry.ExpiryYear))
            .WithMessage("Card cannot be expired")
            .WithName("Card");
        
        RuleFor(r => r.Currency)
            .NotEmpty().WithMessage("Currency cannot be empty")
            .Length(3).WithMessage("Currency must be length of 3 characters")
            .Must(BeAValidCurrency).WithMessage("Currency is not supported");

        RuleFor(r => r.Amount)
            .NotEmpty().WithMessage("Amount cannot be empty")
            .GreaterThan(0).WithMessage("Amount must be greater than 0");

        RuleFor(r => r.Cvv)
            .NotEmpty().WithMessage("CVV cannot be empty")
            .Must(cvv => int.TryParse(cvv, out _)).WithMessage("CVV can only contain digits")
            .Length(3, 4).WithMessage("CVV must be between 3 and 4 digits");
    }

    private bool BeValidYear(int year)
    {
        // Validates that card year is not in the past
        // Could add a check for a limit in the future, eg. 10 year limit in the future
        return DateTime.Now.Year <= year;
    }
    
    private bool IsExpired(int expiryMonth, int expiryYear)
    {
        if (expiryMonth < 1 || expiryMonth > 12)
        {
            return false;
        }
        var cardExpiry = new DateTime(expiryYear, expiryMonth, 1).AddMonths(1).AddDays(-1);
        return cardExpiry < DateTime.Now;
    }

    private bool BeAValidCurrency(string currency)
    {
        return ValidCurrencies.Contains(currency);
    }

    private static readonly HashSet<string> ValidCurrencies = new(StringComparer.OrdinalIgnoreCase)
    {
        "USD", "GBP", "EUR"
    };
}