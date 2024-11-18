using System.Text.Json;
using System.Text.Json.Serialization;

using FluentValidation;
using FluentValidation.AspNetCore;
using FluentValidation.Results;

using Microsoft.AspNetCore.Mvc;

using PaymentGateway.Api.HttpClient.BankSimulator;
using PaymentGateway.Api.Interfaces;
using PaymentGateway.Api.Middleware;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Api.Services;
using PaymentGateway.Api.Validators;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(o =>
    {
        o.JsonSerializerOptions.Converters.Add((new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)));
    });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var bankSimulatorUrl = builder.Configuration.GetValue<string>("ClientConnectionStrings:BankSimulatorUrl");
if (bankSimulatorUrl is null)
{
    throw new Exception("Bank Simulator URL is missing from configuration");
}

builder.Services.AddHttpClient(nameof(BankSimulatorClient),c =>
{
    c.BaseAddress = new Uri(bankSimulatorUrl);
    c.Timeout = TimeSpan.FromSeconds(10);
});

builder.Services.AddSingleton(new Dictionary<Guid, PaymentResponse>());
builder.Services.AddSingleton<IPaymentsRepository, PaymentsRepository>();
builder.Services.AddSingleton<IBankSimulatorClient, BankSimulatorClient>();

builder.Services.AddValidatorsFromAssemblyContaining(typeof(PostPaymentRequestValidator));

builder.Services.AddFluentValidationAutoValidation(config =>
{
    config.DisableDataAnnotationsValidation = true;
});

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState
            .Where(e => e.Value.Errors.Count > 0)
            .Select(x => new ValidationFailure(
                x.Key,
                string.Join(", ", x.Value.Errors.Select(e => e.ErrorMessage))
                ))
            .ToList();

        throw new ValidationException(errors);
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseMiddleware<ValidationMiddleware>();

app.MapControllers();

app.Run();