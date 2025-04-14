    using System.ComponentModel.DataAnnotations;

    namespace TranNhatTu_2122110250.Model
    {
        public class User
        {
            public int Id { get; set; }

            [Display(Name = "Tên người dùng")]
            public string FirstName { get; set; }

            [Display(Name = "Họ người dùng")]
            public string LastName { get; set; }

            [Required(ErrorMessage = "Vui lòng nhập email")]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
            [Display(Name = "Mật khẩu")]
            public string Password { get; set; }
            public Cart Cart { get; set; }  // Liên kết với giỏ hàng

    }
    }
