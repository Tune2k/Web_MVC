using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PhamTranXuanTan_2122110248.Models
{
    public class CategoriesMasterData
    {
        public int id { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập tên danh mục")]
        [Display(Name = "Tên danh mục")]
        public string name { get; set; }
        [Display(Name = "Chi tiết danh mục")]
        public string description { get; set; }
        [Display(Name = "Hình ảnh danh mục")]
        public string image { get; set; }
        public Nullable<System.DateTime> created_at { get; set; }
    }
}