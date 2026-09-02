# Báo cáo sử dụng AI trong Phát triển Dự án Mobile Shopping

**Người thực hiện:** Phan Bá Vượng
**Vị trí:** Backend Developer Intern
**Dự án:** Mobile Shopping (Backend API)
**Nền tảng công nghệ:** .NET 8.0, ASP.NET Core Web API, SQL Server

---

## 1. Mục tiêu Báo cáo
Tổng hợp các hạng mục công việc đã thực hiện trong dự án Mobile Shopping và đánh giá vai trò hỗ trợ của công cụ AI (Gemini) trong việc tối ưu hóa quy trình phát triển, từ khâu thiết kế kiến trúc, viết mã nguồn (coding) đến kiểm thử ứng dụng.

## 2. Các Hạng Mục Công Việc Đã Triển Khai

### 2.1. Thiết kế Kiến trúc & Cấu trúc Dữ liệu
*   **Thiết lập Design Patterns:** Triển khai kiến trúc dự án theo mô hình phân tầng (Layered Architecture), áp dụng **Repository Pattern** và **Unit of Work** để chuẩn hóa và quản lý luồng truy xuất dữ liệu đồng bộ.
*   **Database Schema:** Thiết kế và tối ưu hóa các bảng dữ liệu lõi trong **SQL Server** (bao gồm User Profile, Product, Cart, và Order).
*   **Tích hợp ORM:** Cấu hình **Entity Framework Core** để mapping dữ liệu, thiết lập các relationship chặt chẽ giữa các thực thể nghiệp vụ.

### 2.2. Phát triển API & Xử lý Nghiệp vụ Backend
*   **API Development:** Xây dựng hệ thống Controllers xử lý logic nghiệp vụ cho quy trình mua sắm trên thiết bị di động, từ quản lý danh mục, thêm vào giỏ hàng (Cart) đến xử lý đơn hàng.
*   **Bảo mật & Phân quyền:** Tích hợp **ASP.NET Core Identity** để quản lý tài khoản người dùng. Xây dựng cơ chế xác thực an toàn bằng **JSON Web Tokens (JWT)**.
*   **Chuẩn hóa Lỗi (Exception Handling):** Xây dựng và tích hợp custom middleware **Global Exception Handler** nhằm bắt, xử lý và chuẩn hóa toàn bộ cấu trúc lỗi trả về, đảm bảo API responses luôn nhất quán.
*   **Kiểm soát luồng dữ liệu:** Định nghĩa các **DTOs** (Data Transfer Objects) để format dữ liệu giao tiếp giữa hệ thống Backend và ứng dụng client, tăng cường tính bảo mật.


### 2.3. Kiểm thử & Khắc phục Lỗi (Troubleshooting)
*   **Unit Testing:** Khởi tạo môi trường và viết các kịch bản test bằng **xUnit** kết hợp **Moq** để mock các dependencies. Quá trình test tập trung cover logic nghiệp vụ chuyên sâu tại tầng Service.



### 3. Cấu hình và Phát triển Middleware
*   **Middleware Xác thực và Phân quyền (Auth Middleware):** Tích hợp `UseAuthentication` và `UseAuthorization` vào pipeline của ứng dụng để kiểm tra, xác thực JWT token trên mỗi request từ ứng dụng Android, đảm bảo việc kiểm soát truy cập chặt chẽ vào các API nhạy cảm (như giỏ hàng, thông tin cá nhân).
*   **Xử lý Ngoại lệ Toàn cục (Global Exception Handling Middleware):** Xây dựng Custom Middleware để bắt (catch) các lỗi phát sinh (exceptions) ở cấp độ toàn cục. Chuẩn hóa dữ liệu trả về (JSON response format) giúp client dễ dàng bắt lỗi và hiển thị thông báo phù hợp cho người dùng mà không làm lộ các log hệ thống nhạy cảm.
*   **Cấu hình CORS & Ghi log (CORS & Logging Middleware):** Thiết lập chính sách Cross-Origin Resource Sharing (CORS) hợp lý để bảo vệ API và áp dụng middleware ghi log các luồng request/response hỗ trợ cho quá trình theo dõi, debug lỗi hiệu quả hơn.
