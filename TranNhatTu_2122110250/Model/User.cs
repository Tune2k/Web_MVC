using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace TranNhatTu_2122110250.Model
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        public string Role { get; set; }

        public Cart Cart { get; set; }

        // ✅ Quan hệ 1-n với Order
        public virtual List<Order> Orders { get; set; } = new List<Order>();

    }

}
