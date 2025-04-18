using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TranNhatTu_2122110250.Data; // Sửa lại nếu CartItem nằm namespace khác

namespace TranNhatTu_2122110250.Model
{
    public class OrderViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập tên")]
        [Display(Name = "Tên khách hàng")]
        public string CustomerName { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập email")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [Display(Name = "Email")]
        public string CustomerEmail { get; set; }

        [Display(Name = "Giỏ hàng")]
        public List<CartItemViewModel> CartItems { get; set; } = new List<CartItemViewModel>();

        [Required]
        [Display(Name = "Tổng tiền")]
        [DataType(DataType.Currency)]
        [Range(0, double.MaxValue, ErrorMessage = "Tổng tiền không hợp lệ")]
        public decimal TotalPrice { get; set; }
    }
}
