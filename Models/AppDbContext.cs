using Microsoft.EntityFrameworkCore;

namespace SalesApi.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // Đại diện cho bảng dbo."Hello world"
        public DbSet<HelloWorld> HelloWorlds { get; set; }

        // Bảng Users mới
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Cấu hình tên bảng chính xác và loại trừ khỏi Migrations (vì bảng đã tồn tại)
            modelBuilder.Entity<HelloWorld>().ToTable("Hello world", "dbo", t => t.ExcludeFromMigrations());
        }
    }
}
