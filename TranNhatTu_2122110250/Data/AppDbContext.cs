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
		//public DbSet<Cart> Carts { get; set; }
		//public DbSet<CartItem> CartItems { get; set; }
	}
}
