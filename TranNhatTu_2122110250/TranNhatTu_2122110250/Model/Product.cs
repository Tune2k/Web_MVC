using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TranNhatTu_2122110250.Model
{
    public class Product
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên sản phẩm là bắt buộc.")]
        public string Name { get; set; }

        public string Image { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Giá không hợp lệ.")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Mô tả là bắt buộc.")]
        public string Description { get; set; }

        // Tồn kho
        [Range(0, int.MaxValue, ErrorMessage = "Tồn kho không hợp lệ.")]
        public int Stock { get; set; }

        // ✅ Đây là cột FK chính xác - giữ lại
        [Required(ErrorMessage = "Danh mục là bắt buộc.")]
        public int CategoryId { get; set; }

        // ✅ Navigation property
        public Category Category { get; set; }

        // ✅ Chỉ dùng hiển thị, không lưu vào DB
        [NotMapped]
        public string? Category_name { get; set; }

        // Tracking fields
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }




    }
}
