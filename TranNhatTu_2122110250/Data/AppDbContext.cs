using Microsoft.EntityFrameworkCore;
using TranNhatTu_2122110250.Model;

namespace TranNhatTu_2122110250.Data
{
	public class AppDbContext : DbContext
	{
		public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
		public DbSet<Product> Products { get; set; }
		public DbSet<Category> Category { get; set; }
		public DbSet<Order> Order { get; set; }
		public DbSet<OrderDetail> OrderDetail { get; set; }
		public DbSet<User> User { get; set; }
		public DbSet<Brands> Brands { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Cart>()
                .HasMany(c => c.Items)
                .WithOne(ci => ci.Cart)
                .HasForeignKey(ci => ci.CartId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
