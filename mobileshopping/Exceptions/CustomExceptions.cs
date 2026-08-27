using Microsoft.AspNetCore.Http;
using System;

namespace mobileshopping.Exceptions
{
    // Lớp base cho tất cả các custom exception trong dự án
    public class BaseException : Exception
    {
        public int StatusCode { get; }

        public BaseException(string message, int statusCode) : base(message)
        {
            StatusCode = statusCode;
        }
    }

    // Dùng khi không tìm thấy dữ liệu (VD: Không tìm thấy sản phẩm, User, Đơn hàng...)
    public class NotFoundException : BaseException
    {
        public NotFoundException(string message)
            : base(message, StatusCodes.Status404NotFound)
        {
        }
    }

    // Dùng khi Client gửi dữ liệu sai hoặc vi phạm logic nghiệp vụ (VD: Số lượng < 0, Hết hàng...)
    public class BadRequestException : BaseException
    {
        public BadRequestException(string message)
            : base(message, StatusCodes.Status400BadRequest)
        {
        }
    }

    // Dùng khi chưa đăng nhập hoặc token hết hạn
    public class UnauthorizedException : BaseException
    {
        public UnauthorizedException(string message)
            : base(message, StatusCodes.Status401Unauthorized)
        {
        }
    }

    // Dùng khi user đã đăng nhập nhưng không có quyền thực hiện thao tác (VD: User thường đòi xóa User khác)
    public class ForbiddenException : BaseException
    {
        public ForbiddenException(string message)
            : base(message, StatusCodes.Status403Forbidden)
        {
        }
    }
}