using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using TranNhatTu_2122110250.Model;

namespace TranNhatTu_2122110250.ViewModels
{
    using Microsoft.AspNetCore.Mvc.Rendering;
    using System.ComponentModel.DataAnnotations;
    using TranNhatTu_2122110250.Model;

    public class ProductCreateViewModel
    {
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

        // Nếu bạn dùng Category dropdown:
        public IEnumerable<SelectListItem>? Categories { get; set; }

        //// Optionally, bạn có thể có thêm các trường riêng biệt nếu cần
        //public string Name => Product?.Name;
        //public decimal Price => Product?.Price ?? 0;
        //public int Stock => Product?.Stock ?? 0;
        //public string Description => Product?.Description;



        public string? CategoryLoadError { get; set; } // ✅ để hiển thị lỗi nếu không load được danh mục
    }
}

