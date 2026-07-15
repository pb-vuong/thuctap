using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using mobileshopping.Models;

namespace mobileshopping.Data
{
    // Cần kế thừa IdentityDbContext với 3 tham số: User, Role, và kiểu của khóa chính (int)
    public class AppDbContext : IdentityDbContext<User, IdentityRole<int>, int>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // Các DbSet cho các bảng nghiệp vụ của bạn
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Bạn có thể cấu hình thêm fluent API tại đây nếu cần
            // Ví dụ: Đổi tên các bảng Identity mặc định
            builder.Entity<User>(entity => { entity.ToTable(name: "Users"); });
            builder.Entity<IdentityRole<int>>(entity => { entity.ToTable(name: "Roles"); });
        }
    }
}