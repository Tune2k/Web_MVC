using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using TranNhatTu_2122110250.Model;

namespace TranNhatTu_2122110250.ViewModels
{
    using Microsoft.AspNetCore.Mvc.Rendering;
    using TranNhatTu_2122110250.Model;

    public class ProductCreateViewModel
    {
        public Product Product { get; set; }
        public List<SelectListItem> Categories { get; set; }
        public List<SelectListItem> Brands { get; set; } // Dùng để hiển thị thương hiệu
        public string? CategoryLoadError { get; set; } // ✅ để hiển thị lỗi nếu không load được danh mục
    }
}

