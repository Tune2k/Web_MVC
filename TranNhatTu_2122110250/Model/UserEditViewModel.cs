using System.ComponentModel.DataAnnotations;

namespace TranNhatTu_2122110250.Model
{
    public class UserEditViewModel
    {
        public int Id { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Role { get; set; }
    }
}
