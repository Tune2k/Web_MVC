using System.ComponentModel.DataAnnotations;

namespace TranNhatTu_2122110250.Model
{
	public class Category
	{
		public int Id { get; set; }
		public string Name { get; set; }

		// Danh sách sản phẩm thuộc danh mục này
		public List<Product> Products { get; set; } = new List<Product>();
	}
}
