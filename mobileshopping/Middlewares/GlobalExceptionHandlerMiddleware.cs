using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using mobileshopping.Exceptions; // Thêm dòng này để gọi được các Custom Exceptions
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace mobileshopping.Middlewares
{
    public class GlobalExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;
        private readonly IWebHostEnvironment _env;

        public GlobalExceptionHandlerMiddleware(
            RequestDelegate next,
            ILogger<GlobalExceptionHandlerMiddleware> logger,
            IWebHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
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

        private async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            var traceId = Guid.NewGuid();

            // Mặc định là lỗi hệ thống (500)
            var statusCode = StatusCodes.Status500InternalServerError;
            var title = "Internal Server Error";
            var message = "Một lỗi hệ thống đã xảy ra. Vui lòng thử lại sau.";

            // Kiểm tra xem lỗi có phải do mình chủ động throw ra không
            if (ex is BaseException customException)
            {
                statusCode = customException.StatusCode;
                title = customException.GetType().Name.Replace("Exception", " Error");
                message = customException.Message;
            }
            else if (ex is UnauthorizedAccessException)
            {
                statusCode = StatusCodes.Status401Unauthorized;
                title = "Unauthorized Error";
                message = "Bạn không có quyền truy cập tính năng này.";
            }

            // Ghi log chi tiết lỗi để backend dev (như bạn) dễ tra cứu
            if (statusCode == StatusCodes.Status500InternalServerError)
            {
                _logger.LogError(ex, "TraceId: {TraceId} | Lỗi hệ thống nghiêm trọng: {Message}", traceId, ex.Message);
            }
            else
            {
                _logger.LogWarning("TraceId: {TraceId} | Lỗi logic/nghiệp vụ: {Message}", traceId, ex.Message);
            }

            // Cấu hình Response
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";

            var problemDetails = new ProblemDetails
            {
                Type = $"https://httpstatuses.io/{statusCode}",
                Title = title,
                Status = statusCode,
                Instance = context.Request.Path,
                Detail = message
            };

            // Thêm mã traceId để client có thể gửi cho admin kiểm tra
            problemDetails.Extensions.Add("traceId", traceId);

            // Môi trường Dev thì in ra cả đống stack trace để dễ debug
            if (_env.IsDevelopment())
            {
                problemDetails.Extensions.Add("exceptionDetails", ex.ToString());
            }

            // Trả về chuỗi JSON
            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            await context.Response.WriteAsJsonAsync(problemDetails, options);
        }
    }
}