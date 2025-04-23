using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PhamTranXuanTan_2122110248.Models
{
    public partial class ProductMasterData
    {
        
        public int id { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập tên sản phẩm")]
         [Display(Name = "Tên sản phẩm")]
        public string name { get; set; }
        [Display(Name = "Chi tiết sản phẩm")]
        public string description { get; set; }
        [Display(Name = "Gía sản phẩm")]
        public decimal price { get; set; }
        [Display(Name = "Danh mục sản phẩm")]
        public int category_id { get; set; }
        [Display(Name = "Thương hiệu sản phẩm")]
        public Nullable<int> brand_id { get; set; }
        [Display(Name = "Hình ảnh sản phẩm")]
        public string image { get; set; }
        
        public Nullable<System.DateTime> created_at { get; set; }
    }
}