using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TranNhatTu_2122110250.Data;
using TranNhatTu_2122110250.Helpers;
using TranNhatTu_2122110250.Model;
using System.Linq;

namespace TranNhatTu_2122110250.Views.ViewComponent
{
    // Sử dụng đúng namespace của ViewComponent
    public class CartInfoViewComponent : Microsoft.AspNetCore.Mvc.ViewComponent
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        // Inject IHttpContextAccessor vào ViewComponent
        public CartInfoViewComponent(AppDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public IViewComponentResult Invoke()
        {
            var userId = SessionUser.GetUserId(_httpContextAccessor.HttpContext);
            int cartCount = 0;

            if (userId != null)
            {
                var cart = _context.Carts.FirstOrDefault(c => c.UserId == userId.Value);
                if (cart != null)
                {
                    // ✅ Đếm số sản phẩm khác nhau trong giỏ hàng
                    cartCount = _context.CartItems
                        .Where(c => c.CartId == cart.Id)
                        .Count(); // dùng Count() thay vì Sum()
                }
            }

            return View("Default", cartCount);
        }
    }
}
