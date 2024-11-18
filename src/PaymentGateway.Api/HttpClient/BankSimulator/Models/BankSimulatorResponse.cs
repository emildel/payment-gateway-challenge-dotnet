namespace PaymentGateway.Api.HttpClient.BankSimulator.Models;

public class BankSimulatorResponse
{
    public bool IsSuccess { get; set; }
    
    public BankSimulatorSuccessResponse? SuccessResponse { get; set; }
    
    public BankSimulatorErrorResponse? ErrorResponse { get; set; }

    public BankSimulatorResponse(bool isSuccess, BankSimulatorSuccessResponse? successResponse,
        BankSimulatorErrorResponse? errorResponse)
    {
        this.IsSuccess = isSuccess;
        this.SuccessResponse = successResponse;
        this.ErrorResponse = errorResponse;
    }

    public static BankSimulatorResponse Success(BankSimulatorSuccessResponse? successResponse)
    {
        return new BankSimulatorResponse(true, successResponse, null);
    }
    
    public static BankSimulatorResponse Error(BankSimulatorErrorResponse errorResponse)
    {
        return new BankSimulatorResponse(false, null, errorResponse);
    }

    public static BankSimulatorResponse Error()
    {
        return new BankSimulatorResponse(false, null, null);
    }
}