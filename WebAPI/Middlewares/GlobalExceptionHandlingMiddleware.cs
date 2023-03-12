using Domain.CustomExceptions;
using System.Net;
using System.Text.Json;

namespace WebAPI.Middlewares
{
    public class GlobalExceptionHandlingMiddleware : IMiddleware
    {
        private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;

        public GlobalExceptionHandlingMiddleware(ILogger<GlobalExceptionHandlingMiddleware> logger)
        {
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);

                await HandleExceptionAsync(context, e);
            }
        }

        protected Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var response = GenerateResponse(exception);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)response.statusCode;

            return context.Response.WriteAsync(response.json);
        }

        private static (HttpStatusCode statusCode, string json) GenerateResponse(Exception exception)
        {
            return exception switch
            {
                BusinessException businessException =>
                (businessException.StatusCode,
                JsonSerializer.Serialize(new
                {
                    businessException.Title,
                    Status = businessException.StatusCode,
                    businessException.Detail
                })),

                _ => (
                HttpStatusCode.InternalServerError,
                JsonSerializer.Serialize(new
                {
                    Title = "Internal Server Error",
                    Status = HttpStatusCode.InternalServerError,
                    Detail = "An internal server error has occurred"
                }))
            };
        }
    }
}
