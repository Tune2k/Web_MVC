namespace TranNhatTu_2122110250.Model
{
	public class OrderDetail
	{
		public int Id { get; set; }

		// Khóa ngoại đến Product
		public int ProductId { get; set; }
		public Product Product { get; set; }

		// Khóa ngoại đến Order
		public int OrderId { get; set; }
		public Order Order { get; set; }

		public int Quantity { get; set; }  // Số lượng sản phẩm
		public double Price { get; set; }  // Giá tại thời điểm đặt hàng
	}
}
