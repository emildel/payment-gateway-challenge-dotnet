using FluentValidation;

using PaymentGateway.Api.HttpClient;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Services;
using PaymentGateway.Api.Validators;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var bankSimulatorUrl = builder.Configuration.GetValue<string>("ClientConnectionStrings:BankSimulatorUrl");
if (bankSimulatorUrl is null)
{
    throw new Exception("Bank Simulator URL is missing from configuration");
}

builder.Services.AddHttpClient(nameof(BankSimulatorClient), c =>
    c.BaseAddress = new Uri(bankSimulatorUrl));

builder.Services.AddSingleton<PaymentsRepository>();
builder.Services.AddScoped<IValidator<PostPaymentRequest>, PostPaymentRequestValidator>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();