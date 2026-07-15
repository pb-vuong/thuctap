using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;



namespace mobileshopping.Middlewares
    {
        // Không cần kế thừa : IMiddleware nữa
        public class GlobalExceptionHandlerMiddleware
        {
            private readonly RequestDelegate _next;
            private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

            // Tiêm RequestDelegate (đại diện cho tác vụ tiếp theo trong pipeline) vào Constructor
            public GlobalExceptionHandlerMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlerMiddleware> logger)
            {
                _next = next;
                _logger = logger;
            }

            // Hàm InvokeAsync giờ chỉ nhận HttpContext
            public async Task InvokeAsync(HttpContext context)
            {
                try
                {
                    // Chuyển tiếp request đến thành phần tiếp theo
                    await _next(context);
                }
                catch (Exception ex)
                {
                    var traceId = Guid.NewGuid();

                    // Ghi log
                    _logger.LogError($"Error occurred while processing the request. TraceId: {traceId}, Message: {ex.Message}, StackTrace: {ex.StackTrace}");

                    // Trả về HTTP 500
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

