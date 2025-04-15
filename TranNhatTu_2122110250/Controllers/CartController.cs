using Microsoft.AspNetCore.Mvc;
using TranNhatTu_2122110250.Data;
using TranNhatTu_2122110250.Model;
using Microsoft.EntityFrameworkCore;


public class CartController : Controller
{
    private readonly AppDbContext _context;

    public CartController(AppDbContext context)
    {
        _context = context;
    }

    private int? GetUserId()
    {
        // Lấy userId từ session hoặc cookie
        return HttpContext.Session.GetInt32("UserId");
    }

    public IActionResult Index()
    {
        var userId = GetUserId();

        if (userId == null)
        {
            return RedirectToAction("Login", "Account"); // Redirect nếu chưa đăng nhập
        }

        // Lấy giỏ hàng từ cơ sở dữ liệu
        var cart = _context.Carts
            .Include(c => c.Items)
            .ThenInclude(ci => ci.Product)
            .FirstOrDefault(c => c.UserId == userId);

        if (cart == null || !cart.Items.Any())
        {
            TempData["Message"] = "Giỏ hàng của bạn đang trống.";
            return View(new List<CartItem>());
        }

        return View(cart.Items);
    }

    [HttpPost]
    public IActionResult AddToCart(int id)
    {
        var userId = GetUserId();
        if (userId == null)
            return RedirectToAction("Login", "Account");

        var product = _context.Products.FirstOrDefault(p => p.Id == id);
        if (product == null || product.Stock <= 0)
        {
            TempData["Message"] = "Sản phẩm không khả dụng.";
            return RedirectToAction("Index", "Home");
        }

        var cart = _context.Carts
            .Include(c => c.Items)
            .FirstOrDefault(c => c.UserId == userId);

        if (cart == null)
        {
            cart = new Cart { UserId = userId.Value };
            _context.Carts.Add(cart);
            _context.SaveChanges(); // Lưu để tạo Cart.Id
        }

        var cartItem = _context.CartItems
            .FirstOrDefault(ci => ci.CartId == cart.Id && ci.ProductId == id);

        if (cartItem != null)
        {
            if (cartItem.Quantity < product.Stock)
            {
                cartItem.Quantity++;
            }
            else
            {
                TempData["Message"] = "Đã đạt số lượng tối đa trong kho.";
                return RedirectToAction("Index", "Cart");
            }
        }
        else
        {
            cartItem = new CartItem
            {
                CartId = cart.Id,
                ProductId = product.Id,
                Name = product.Name,
                Price = product.Price,
                Image = product.Image,
                Quantity = 1,
                Stock = product.Stock
            };
            _context.CartItems.Add(cartItem); // Quan trọng: thêm trực tiếp vào DbSet
        }

        cart.UpdatedAt = DateTime.Now;
        _context.SaveChanges();

        TempData["Message"] = $"{product.Name} đã được thêm vào giỏ hàng.";
        return RedirectToAction("Index", "Cart");
    }




    public IActionResult Remove(int id)
    {
        var userId = GetUserId();
        if (userId == null)
        {
            return RedirectToAction("Login", "Account"); // Redirect nếu chưa đăng nhập
        }

        var cart = _context.Carts
            .Include(c => c.Items)
            .FirstOrDefault(c => c.UserId == userId);

        var item = cart?.Items.FirstOrDefault(i => i.ProductId == id);
        if (item != null)
        {
            cart.Items.Remove(item);
            _context.SaveChanges();
        }

        return RedirectToAction("Index");
    }

    [HttpPost]
    public IActionResult Update(List<CartItem> cartItems)
    {
        var userId = GetUserId();
        if (userId == null)
        {
            return RedirectToAction("Login", "Account"); // Redirect nếu chưa đăng nhập
        }

        var cart = _context.Carts
            .Include(c => c.Items)
            .FirstOrDefault(c => c.UserId == userId);

        foreach (var item in cartItems)
        {
            var existingItem = cart.Items.FirstOrDefault(ci => ci.ProductId == item.ProductId);
            var product = _context.Products.FirstOrDefault(p => p.Id == item.ProductId);

            if (existingItem != null && product != null)
            {
                if (item.Quantity <= 0)
                {
                    cart.Items.Remove(existingItem);
                }
                else if (product.Stock >= item.Quantity)
                {
                    existingItem.Quantity = item.Quantity;
                }
                else
                {
                    TempData["Message"] = $"Không đủ hàng cho {product.Name}";
                }
            }
        }

        _context.SaveChanges();
        return RedirectToAction("Index");
    }
}
