using System.Net;
using System.Text.Json;
using FluentValidation;

namespace ECommerce.Product.API.Infrastructure.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var code = HttpStatusCode.InternalServerError; // Varsayılan 500
            object result;

            if (exception is ValidationException validationException)
            {
                code = HttpStatusCode.BadRequest; // Doğrulama hatası ise 400
                result = new
                {
                    Errors = validationException.Errors.Select(e => e.ErrorMessage)
                };
            }
            else
            {
                // Diğer beklenmedik hatalar
                result = new { Error = exception.Message };
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;

            return context.Response.WriteAsync(JsonSerializer.Serialize(result));
        }
    }
}
