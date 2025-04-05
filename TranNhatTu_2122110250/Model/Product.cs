namespace TranNhatTu_2122110250.Model
{
	public class Product
	{
		public int Id { get; set; }

		public string Name { get; set; }

		public string Image { get; set; }

		public double Price { get; set; }



        // Các trường tracking
        public string CreatedBy { get; set; }

		public DateTime CreatedDate { get; set; }

		public string UpdatedBy { get; set; }

		public DateTime UpdatedDate { get; set; }

		public string DeletedBy { get; set; }

		public DateTime DeletedDate { get; set; }
	}
}
