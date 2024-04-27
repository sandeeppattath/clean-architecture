using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;

namespace Common.Logging
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
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
        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {

            var status = HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";
            var response = context.Response;
            var message = exception.Message;

            switch (exception)
            {
                case ApplicationException ex:
                    if (ex.Message.Contains("Invalid token"))
                    {
                        response.StatusCode = (int)HttpStatusCode.Forbidden;
                        message = ex.Message;
                        break;
                    }
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    message = ex.Message;
                    break;
                case KeyNotFoundException ex:
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    message = ex.Message;
                    break;
                default:
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    message = "error";
                    break;
            }

            var exceptionResult = JsonSerializer.Serialize(new
            {
                IsSuccess = false,
                Message = message,
            });


            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)status;
            if (exception.InnerException == null)
            {
                _logger.LogError($"Message : {exception.Message}{Environment.NewLine}" +
                                        $"Time : {DateTime.Now}{Environment.NewLine}Method :" + $" {context.Request.Method}{Environment.NewLine}" +
                                        $"Path : {context.Request.Path}{Environment.NewLine}" +
                                        $"Error :" + $"{exception.StackTrace}");
            }
            else
            {
                _logger.LogError($"Message : {exception.Message}{Environment.NewLine}" +
                                        $"Time : {DateTime.Now}{Environment.NewLine}" +
                                        $"Method : " + $"{context.Request.Method}{Environment.NewLine}" +
                                        $"Path : {context.Request.Path}{Environment.NewLine}" +
                                        $"Error :" + $"{exception.StackTrace}{Environment.NewLine}" +
                                        $"InnerMessage :" + $"{exception.InnerException.Message}{Environment.NewLine}" +
                                        $"InnerException :{exception.InnerException.StackTrace}");
            }
            await context.Response.WriteAsync(exceptionResult);
        }
    }
}
