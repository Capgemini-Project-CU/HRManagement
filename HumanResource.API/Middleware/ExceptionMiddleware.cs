using System.Net;
using System.Text.Json;
using HumanResource.API.Exceptions;

namespace HumanResource.API.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;

            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Exception occurred while processing request: {Path}",
                    context.Request.Path);

                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(
            HttpContext context,
            Exception exception)
        {
            context.Response.ContentType = "application/json";

            int statusCode;
            string message;

            switch (exception)
            {
                case NotFoundException:
                    statusCode = (int)HttpStatusCode.NotFound;
                    message = exception.Message;
                    break;

                case BadRequestException:
                    statusCode = (int)HttpStatusCode.BadRequest;
                    message = exception.Message;
                    break;

                case UnauthorizedException:
                    statusCode = (int)HttpStatusCode.Unauthorized;
                    message = exception.Message;
                    break;

                case ConflictException:
                    statusCode = (int)HttpStatusCode.Conflict;
                    message = exception.Message;
                    break;

                default:
                    statusCode =
                        (int)HttpStatusCode.InternalServerError;

                    message = exception.InnerException != null
                        ? exception.InnerException.Message
                        : exception.Message;

                    break;
            }

            context.Response.StatusCode = statusCode;

            var response = new
            {
                statusCode,
                message,
                details = exception.InnerException?.Message,
                exceptionType = exception.GetType().Name,
                stackTrace = exception.StackTrace
            };

            var jsonResponse = JsonSerializer.Serialize(
                response,
                new JsonSerializerOptions
                {
                    WriteIndented = true
                });

            await context.Response.WriteAsync(jsonResponse);
        }
    }
}