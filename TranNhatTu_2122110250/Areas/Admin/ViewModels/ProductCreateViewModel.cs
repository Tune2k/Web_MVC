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
        public List<SelectListItem> Categories { get; set; } = new();
    }
}

