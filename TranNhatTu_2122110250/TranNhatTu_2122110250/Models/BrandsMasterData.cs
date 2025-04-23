using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PhamTranXuanTan_2122110248.Models
{
    public class BrandsMasterData
    {
        public int id { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập tên thương hiệu")]
        [Display(Name = "Tên thương hiệu")]
        public string name { get; set; }
        [Display(Name = "Chi tiết thương hiệu")]
        public string description { get; set; }
        [Display(Name = "Hình ảnh thương hiệu")]
        public string image { get; set; }
        public Nullable<System.DateTime> created_at { get; set; }
    }
}