using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;



namespace mobileshopping.Middlewares
    {
        public class GlobalExceptionHandlerMiddleware
        {
            private readonly RequestDelegate _next;
            private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

            public GlobalExceptionHandlerMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlerMiddleware> logger)
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
                    var traceId = Guid.NewGuid();

                    _logger.LogError($"Error occurred while processing the request. TraceId: {traceId}, Message: {ex.Message}, StackTrace: {ex.StackTrace}");

                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    context.Response.ContentType = "application/json";

                    var problemDetails = new ProblemDetails
                    {
                        Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
                        Title = "Internal Server Error",
                        Status = (int)StatusCodes.Status500InternalServerError,
                        Instance = context.Request.Path,
                        Detail = $"Internal server error occurred, traceId : {traceId}",
                    };

                    await context.Response.WriteAsJsonAsync(problemDetails);
                }
            }
        }
    }

