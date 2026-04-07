using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using ECommerce.Shared.Commons;

namespace ECommerce.Shared.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context); // İstek yoluna devam etsin
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Bir hata oluştu: {Message}", ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            // Default değerler
            var statusCode = StatusCodes.Status500InternalServerError;
            var message = exception.Message;
            List<string>? errors = null;

            // Hata tipine göre yönetimi özelleştir
            if (exception is FluentValidation.ValidationException valEx)
            {
                statusCode = StatusCodes.Status400BadRequest;
                message = "Validasyon hatası gerçekleşti.";
                errors = valEx.Errors.Select(x => $"{x.PropertyName}: {x.ErrorMessage}").ToList();
            }
            else if (exception is UnauthorizedAccessException)
            {
                statusCode = StatusCodes.Status401Unauthorized;
            }
            else if (exception is KeyNotFoundException)
            {
                statusCode = StatusCodes.Status404NotFound;
            }

            context.Response.StatusCode = statusCode;

            var response = new ErrorDetails
            {
                StatusCode = statusCode,
                Message = message,
                Errors = errors,
                Detail = _env.IsDevelopment() ? exception.StackTrace : null,
                TraceId = context.TraceIdentifier
            };

            var json = System.Text.Json.JsonSerializer.Serialize(response);
            await context.Response.WriteAsync(json);
        }
    }
}
