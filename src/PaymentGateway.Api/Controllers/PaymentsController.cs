using FluentValidation;

using Microsoft.AspNetCore.Mvc;

using PaymentGateway.Api.Interfaces;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PaymentsController : Controller
{
    private readonly IPaymentsRepository _paymentsRepository;
    private readonly IValidator<PostPaymentRequest> _postPaymentRequestValidator;

    public PaymentsController(IPaymentsRepository paymentsRepository, IValidator<PostPaymentRequest> postPaymentRequestValidator)
    {
        this._paymentsRepository = paymentsRepository;
        this._postPaymentRequestValidator = postPaymentRequestValidator;
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(GetPaymentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GetPaymentResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(GetPaymentResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(GetPaymentResponse), StatusCodes.Status404NotFound)]
    public ActionResult<GetPaymentResponse?> GetPaymentAsync(Guid id)
    {
        var payment = _paymentsRepository.Get(id);

        if (payment is null)
        {
            return NotFound();
        }
        
        return new OkObjectResult(payment);
    }
    
    [HttpPost]
    public async Task<ActionResult<PostPaymentResponse?>> PostPaymentsAsync([FromBody] PostPaymentRequest paymentRequest)
    {
        try
        {
            var payment = await _paymentsRepository.Add(paymentRequest);
            return new OkObjectResult(payment);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
        

        
    }
}