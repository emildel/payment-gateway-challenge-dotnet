using Microsoft.AspNetCore.Mvc;

using PaymentGateway.Api.Enums;
using PaymentGateway.Api.Errors;
using PaymentGateway.Api.Interfaces;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PaymentsController : Controller
{
    private readonly IPaymentsRepository _paymentsRepository;

    public PaymentsController(IPaymentsRepository paymentsRepository)
    {
        this._paymentsRepository = paymentsRepository;
    }

    [HttpGet("{id:guid}", Name ="GetPaymentById")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public ActionResult<PaymentResponse?> GetPaymentAsync(Guid id)
    {
        try
        {
            var payment = _paymentsRepository.Get(id);

            if (payment == null)
            {
                return this.NotFound(new CustomHttpError(
                    "NOT_FOUND",
                    "A payment with this id could not be found."));
            }

            return new OkObjectResult(payment);
        }
        catch (Exception ex)
        {
            // do some logging on what the exception was
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
    
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PaymentResponse>> PostPaymentsAsync([FromBody] PostPaymentRequest paymentRequest)
    {
        try
        {
            var payment = await _paymentsRepository.Add(paymentRequest);

            // Payment was not processed by the acquiring bank
            if (payment == null)
            {
                return this.BadRequest(new CustomHttpError(
                    "REJECTED", 
                    "The payment was rejected by the acquiring bank")
                );
            }
            
            return this.CreatedAtRoute("GetPaymentById", new { id = payment.Id }, payment);
        }
        catch (Exception ex)
        {
            // do some logging on what the exception was
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}