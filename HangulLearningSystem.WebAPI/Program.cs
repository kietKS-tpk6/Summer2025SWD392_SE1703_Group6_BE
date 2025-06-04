using Application.Usecases.CommandHandler;
using Application.Validators;
using Infrastructure;
using Application;
using MediatR;
using Microsoft.AspNetCore.Diagnostics;
using FluentValidation;
using System.Diagnostics;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Kiểm tra JWT Settings
var jwtSettings = configuration.GetSection("Jwt");
var jwtKey = jwtSettings["Key"];
var jwtIssuer = jwtSettings["Issuer"];
var jwtAudience = jwtSettings["Audience"];

// Validate JWT configuration
if (string.IsNullOrEmpty(jwtKey) || string.IsNullOrEmpty(jwtIssuer) || string.IsNullOrEmpty(jwtAudience))
{
    throw new InvalidOperationException("JWT settings are not properly configured in appsettings.json");
}

// Infrastructure & Application
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddControllers();

// Swagger - Chỉ cần một lần cấu hình
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy => policy
            .WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
        );
});

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "HangulLearningSystem.WebAPI", Version = "v1" });

    // JWT Security configuration
    var jwtSecurityScheme = new OpenApiSecurityScheme
    {
        BearerFormat = "JWT",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        Description = "Nhập token dạng: Bearer {token}",
        Reference = new OpenApiReference
        {
            Id = JwtBearerDefaults.AuthenticationScheme,
            Type = ReferenceType.SecurityScheme
        }
    };

    c.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { jwtSecurityScheme, Array.Empty<string>() }
    });
});

// JWT Authentication với Events để handle lỗi JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ClockSkew = TimeSpan.Zero,

        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        RoleClaimType = "Role"
    };

    // Events để handle JWT errors
    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
            logger.LogError("JWT Authentication failed: {Error}", context.Exception.Message);
            return Task.CompletedTask;
        },

        OnChallenge = context =>
        {
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
            logger.LogWarning("JWT Challenge: {Error} - {ErrorDescription}", context.Error, context.ErrorDescription);

            // Custom response cho 401
            context.HandleResponse();
            context.Response.StatusCode = 401;
            context.Response.ContentType = "application/json";

            return context.Response.WriteAsJsonAsync(new
            {
                Message = "Token không hợp lệ hoặc đã hết hạn",
                Error = context.Error ?? "invalid_token",
                ErrorDescription = context.ErrorDescription ?? "Token validation failed",
                Timestamp = DateTime.UtcNow
            });
        },

        OnForbidden = context =>
        {
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
            logger.LogWarning("Access forbidden for: {User}", context.Principal?.Identity?.Name ?? "Anonymous");

            // Custom response cho 403
            context.Response.StatusCode = 403;
            context.Response.ContentType = "application/json";

            return context.Response.WriteAsJsonAsync(new
            {
                Message = "Bạn không có quyền truy cập tài nguyên này",
                Timestamp = DateTime.UtcNow
            });
        }
    };
});

var app = builder.Build();

// Middleware để log tất cả requests
app.Use(async (context, next) =>
{
    var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
    var sw = Stopwatch.StartNew();

    try
    {
        await next();
        sw.Stop();

        // Log successful requests
        logger.LogInformation("Request: {Method} {Path} -> {StatusCode} ({ElapsedMs}ms)",
            context.Request.Method, context.Request.Path, context.Response.StatusCode, sw.ElapsedMilliseconds);
    }
    catch (Exception ex)
    {
        sw.Stop();
        logger.LogError(ex, "Request failed: {Method} {Path} -> Exception after {ElapsedMs}ms",
            context.Request.Method, context.Request.Path, sw.ElapsedMilliseconds);
        throw; // Re-throw để ExceptionHandler xử lý
    }
});

// Global Exception Handler - FIXED
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;
        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
        var environment = context.RequestServices.GetRequiredService<IWebHostEnvironment>();

        // Log tất cả exception với stack trace
        logger.LogError(exception, "Exception occurred: {ExceptionType} | Message: {Message}",
            exception?.GetType().Name ?? "Unknown", exception?.Message ?? "No message");

        // Set default values
        context.Response.ContentType = "application/json";

        try
        {
            switch (exception)
            {
                case ValidationException validationException:
                    context.Response.StatusCode = 400;

                    var errors = new List<object>();
                    if (validationException.Errors != null)
                    {
                        errors = validationException.Errors.Select(e => new
                        {
                            Field = e?.PropertyName ?? "Unknown",
                            Message = e?.ErrorMessage ?? "Validation error",
                            Code = e?.ErrorCode ?? "ValidationError"
                        }).ToList<object>();
                    }

                    await context.Response.WriteAsJsonAsync(new
                    {
                        Message = "Validation failed",
                        Errors = errors,
                        Timestamp = DateTime.UtcNow
                    });
                    break;

                case UnauthorizedAccessException unauthorizedException:
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsJsonAsync(new
                    {
                        Message = "Email hoặc mật khẩu không đúng.",
                        Detail = environment.IsDevelopment() ? (unauthorizedException.Message ?? "Unauthorized access") : null,
                        Timestamp = DateTime.UtcNow
                    });
                    break;

                case ArgumentException argEx:
                    context.Response.StatusCode = 400;

                    // Log chi tiết để debug
                    logger.LogError("ArgumentException caught - Message: '{Message}', ParamName: '{ParamName}', StackTrace: {StackTrace}",
                        argEx.Message ?? "NULL", argEx.ParamName ?? "NULL", argEx.StackTrace);

                    await context.Response.WriteAsJsonAsync(new
                    {
                        Message = "Invalid argument",
                        Detail = environment.IsDevelopment() ? (argEx.Message ?? "Invalid argument provided") : "Bad request",
                        Parameter = environment.IsDevelopment() ? argEx.ParamName : null,
                        StackTrace = environment.IsDevelopment() ? argEx.StackTrace : null,
                        Timestamp = DateTime.UtcNow
                    });
                    break;

                case KeyNotFoundException keyNotFoundEx:
                    context.Response.StatusCode = 404;
                    await context.Response.WriteAsJsonAsync(new
                    {
                        Message = "Resource not found",
                        Detail = environment.IsDevelopment() ? (keyNotFoundEx.Message ?? "The requested resource was not found") : null,
                        Timestamp = DateTime.UtcNow
                    });
                    break;

                case TimeoutException timeoutEx:
                    context.Response.StatusCode = 408;
                    await context.Response.WriteAsJsonAsync(new
                    {
                        Message = "Request timeout",
                        Detail = environment.IsDevelopment() ? (timeoutEx.Message ?? "Request timed out") : "The request took too long to process",
                        Timestamp = DateTime.UtcNow
                    });
                    break;

                case NotImplementedException notImplEx:
                    context.Response.StatusCode = 501;
                    await context.Response.WriteAsJsonAsync(new
                    {
                        Message = "Feature not implemented",
                        Detail = environment.IsDevelopment() ? (notImplEx.Message ?? "Feature not implemented") : "This feature is not yet available",
                        Timestamp = DateTime.UtcNow
                    });
                    break;

                // DEFAULT CASE - IMPROVED
                default:
                    context.Response.StatusCode = 500;

                    var errorResponse = new
                    {
                        Message = "An internal server error occurred",
                        ErrorType = exception?.GetType().Name ?? "Unknown",
                        Detail = environment.IsDevelopment()
                            ? (exception?.Message ?? "An unexpected error occurred")
                            : "Please contact support if the problem persists",

                        // Thêm thông tin debug trong Development
                        Debug = environment.IsDevelopment() ? new
                        {
                            FullException = exception?.ToString() ?? "No exception details",
                            StackTrace = exception?.StackTrace ?? "No stack trace",
                            InnerException = exception?.InnerException?.Message,
                            Source = exception?.Source,
                            TargetSite = exception?.TargetSite?.Name
                        } : null,

                        TrackingId = Activity.Current?.Id ?? context.TraceIdentifier,
                        Timestamp = DateTime.UtcNow
                    };

                    await context.Response.WriteAsJsonAsync(errorResponse);
                    break;
            }
        }
        catch (Exception serializationEx)
        {
            // Fallback nếu có lỗi khi serialize JSON
            logger.LogError(serializationEx, "Error serializing exception response");

            context.Response.StatusCode = 500;
            context.Response.ContentType = "text/plain";
            await context.Response.WriteAsync("Internal server error occurred");
        }
    });
});

// Configure pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    // Thêm middleware để log chi tiết request/response trong Development
    app.Use(async (context, next) =>
    {
        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();

        // Log request headers
        logger.LogDebug("Request Headers: {Headers}",
            string.Join(", ", context.Request.Headers.Select(h => $"{h.Key}=[{string.Join(",", h.Value)}]")));

        await next();

        // Log response status
        if (context.Response.StatusCode >= 400)
        {
            logger.LogWarning("Error Response: {StatusCode} for {Method} {Path}",
                context.Response.StatusCode, context.Request.Method, context.Request.Path);
        }
    });
}

app.UseHttpsRedirection();

// CORS middleware - Đặt trước Authentication
app.UseCors("AllowFrontend");

// Middleware để handle các HTTP status codes khác (không phải exception)
app.Use(async (context, next) =>
{
    await next();

    var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();

    // Handle các status code đặc biệt mà không có body
    if (context.Response.StatusCode == 403 && !context.Response.HasStarted && context.Response.ContentLength == 0)
    {
        logger.LogWarning("403 Forbidden without body: {Method} {Path}", context.Request.Method, context.Request.Path);

        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(new
        {
            Message = "Forbidden - Bạn không có quyền truy cập",
            StatusCode = 403,
            Path = context.Request.Path.ToString(),
            Timestamp = DateTime.UtcNow
        });
    }
    else if (context.Response.StatusCode == 404 && !context.Response.HasStarted && context.Response.ContentLength == 0)
    {
        logger.LogWarning("404 Not Found: {Method} {Path}", context.Request.Method, context.Request.Path);

        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(new
        {
            Message = "Endpoint not found",
            StatusCode = 404,
            Path = context.Request.Path.ToString(),
            Timestamp = DateTime.UtcNow
        });
    }
});

// QUAN TRỌNG: Thứ tự middleware
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();