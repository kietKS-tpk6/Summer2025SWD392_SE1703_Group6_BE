using Application.Usecases.CommandHandler;
using Application.Validators;
using Infrastructure;
using Application;
using MediatR;
using Microsoft.AspNetCore.Diagnostics;
using FluentValidation;
using System.Diagnostics;
var builder = WebApplication.CreateBuilder(args);
//JWT

//Insfratructure
builder.Services.AddInfrastructure(builder.Configuration);
//Application
builder.Services.AddApplication();
builder.Services.AddControllers();
// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;
        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
        var environment = context.RequestServices.GetRequiredService<IWebHostEnvironment>();

        // Log tất cả exception
        logger.LogError(exception, "Exception occurred: {ExceptionType}", exception?.GetType().Name);

        // Set default values
        context.Response.ContentType = "application/json";

        switch (exception)
        {
            case ValidationException validationException:
                context.Response.StatusCode = 400;
                var errors = validationException.Errors.Select(e => new
                {
                    Field = e.PropertyName,
                    Message = e.ErrorMessage,
                    Code = e.ErrorCode
                });

                await context.Response.WriteAsJsonAsync(new
                {
                    Message = "Validation failed",
                    Errors = errors
                });
                break;

            case UnauthorizedAccessException:
                context.Response.StatusCode = 401;
                await context.Response.WriteAsJsonAsync(new
                {
                    Message = "Email hoặc mật khẩu không đúng."
                });
                break;

            case ArgumentException argEx:
                context.Response.StatusCode = 400;
                await context.Response.WriteAsJsonAsync(new
                {
                    Message = "Invalid argument",
                    Detail = environment.IsDevelopment() ? argEx.Message : "Bad request"
                });
                break;

            case KeyNotFoundException:
                context.Response.StatusCode = 404;
                await context.Response.WriteAsJsonAsync(new
                {
                    Message = "Resource not found"
                });
                break;

            // Thêm các custom exception khác nếu cần
            // case YourCustomException customEx:
            //     context.Response.StatusCode = 422;
            //     await context.Response.WriteAsJsonAsync(new { ... });
            //     break;

            default:
                // Xử lý tất cả exception khác
                context.Response.StatusCode = 500;
                await context.Response.WriteAsJsonAsync(new
                {
                    Message = "An internal server error occurred",
                    Detail = environment.IsDevelopment()
                        ? $"{exception?.GetType().Name}: {exception?.Message}"
                        : "Please contact support if the problem persists",
                    // Thêm tracking ID để debug
                    TrackingId = Activity.Current?.Id ?? context.TraceIdentifier
                });
                break;
        }
    });
});


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
