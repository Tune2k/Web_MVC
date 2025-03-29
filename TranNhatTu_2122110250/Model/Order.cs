using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TranNhatTu_2122110250.Model
{
	public class Order
	{
		public int Id { get; set; }  // Mã đơn hàng
		public DateTime OrderDate { get; set; } = DateTime.Now; // Ngày đặt hàng
		public string CustomerName { get; set; }  // Tên khách hàng
		public string CustomerEmail { get; set; } // Email khách hàng
		public double TotalPrice { get; set; }  // Tổng giá trị đơn hàng

		// Danh sách sản phẩm trong đơn hàng (Mối quan hệ N-N với Product)
		public List<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

	}
}
