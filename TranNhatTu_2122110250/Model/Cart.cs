using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TranNhatTu_2122110250.Model
{
    public class Cart
    {
        [Key]
        public int Id { get; set; }

        public int? UserId { get; set; } // Nếu có user đăng nhập

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public virtual List<CartItem> Items { get; set; } = new List<CartItem>();
    }
}
