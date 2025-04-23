using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TranNhatTu_2122110250.Model
{
    public class Product
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Image { get; set; }

        public decimal Price { get; set; }

        public string Description { get; set; }

        // Tồn kho
        public int Stock { get; set; }

        // ✅ Đây là cột FK chính xác - giữ lại
        public int CategoryId { get; set; }

        // ✅ Navigation property
        public Category Category { get; set; }

        // ✅ Chỉ dùng hiển thị, không lưu vào DB
        [NotMapped]
        public string? Category_name { get; set; }

        // Tracking fields

        public DateTime? CreatedDate { get; set; }




    }
}
