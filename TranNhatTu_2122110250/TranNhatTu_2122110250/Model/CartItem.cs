using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Helpers;

namespace TranNhatTu_2122110250.Model
{
    public class CartItem
    {
        [Key]
        public int Id { get; set; } // Thêm Id cho EF

        [ForeignKey("Cart")]
        public int CartId { get; set; }

        public int ProductId { get; set; }

        [Required]
        public string Name { get; set; }

        public string Image { get; set; }

        public decimal Price { get; set; }

        public int Quantity { get; set; }

        public int Stock { get; set; }

        public decimal Total => Price * Quantity;
        public virtual Cart Cart { get; set; }       // Trỏ về Cart chứa item này
        public virtual Product Product { get; set; } // Trỏ đến sản phẩm trong DB
    }
}
