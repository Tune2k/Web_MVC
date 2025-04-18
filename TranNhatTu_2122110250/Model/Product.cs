using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TranNhatTu_2122110250.Model
{
    public class Product
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Image { get; set; }

        public string? Model3D { get; set; }

        public decimal Price { get; set; }

        public string Description { get; set; }

        // Số lượng sản phẩm trong giỏ hàng
        public int CartCount { get; set; }

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
        public string? CreatedBy { get; set; }

        public DateTime? CreatedDate { get; set; }

        public string? UpdatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public string? DeletedBy { get; set; }

        public DateTime? DeletedDate { get; set; }
    }
}
