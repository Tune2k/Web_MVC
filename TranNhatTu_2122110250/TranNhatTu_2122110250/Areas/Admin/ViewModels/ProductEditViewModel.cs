using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TranNhatTu_2122110250.ViewModels
{
    public class ProductEditViewModel
    {
        [Required]
        public int Id { get; set; } // ID sản phẩm cần sửa

        [Required(ErrorMessage = "Tên sản phẩm không được để trống")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Giá sản phẩm không được để trống")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá phải lớn hơn hoặc bằng 0")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Mô tả sản phẩm không được để trống")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Tồn kho không được để trống")]
        public int Stock { get; set; }

        [Required(ErrorMessage = "Danh mục sản phẩm không được để trống")]
        public int CategoryId { get; set; }

        public IFormFile? ImageFile { get; set; }

        public string? Image { get; set; } // Đường dẫn ảnh hiện tại

        public IEnumerable<SelectListItem>? Categories { get; set; }

        public string? CategoryLoadError { get; set; }
    }
}
