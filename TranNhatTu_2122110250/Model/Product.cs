namespace TranNhatTu_2122110250.Model
{
	public class Product
	{
		public int Id { get; set; }

		public string Name { get; set; }

		public string Image { get; set; }

        public string Model3D { get; set; }

        public double Price { get; set; }

		public string Description { get; set; }

        // Trường mới: số lượng sản phẩm trong giỏ hàng
        public int CartCount { get; set; }

        // Trường mới: số lượng tồn kho
        public int Stock { get; set; }

        public int Category_id { get; set; }

        public string Category_name { get; set; }

        // Các trường tracking
        public string CreatedBy { get; set; }

		public DateTime CreatedDate { get; set; }

		public string UpdatedBy { get; set; }

		public DateTime UpdatedDate { get; set; }

		public string DeletedBy { get; set; }

		public DateTime DeletedDate { get; set; }

	}
}
