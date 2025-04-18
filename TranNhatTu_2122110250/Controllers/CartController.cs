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
            // In session ra để kiểm tra
            var allSessionKeys = HttpContext.Session.Keys;
            foreach (var key in allSessionKeys)
            {
                var val = HttpContext.Session.GetString(key);
                Console.WriteLine($"Session[{key}] = {val}");
            }

            return RedirectToAction("Login", "Account");
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
        foreach (var key in Request.Form.Keys)
        {
            Console.WriteLine($"{key} = {Request.Form[key]}");
        }

        var userId = GetUserId();
        if (userId == null)
            return RedirectToAction("Login", "Account");

        // Lấy sản phẩm từ DB
        var product = _context.Products.FirstOrDefault(p => p.Id == id);
        if (product == null || product.Stock <= 0)
        {
            TempData["Message"] = "Sản phẩm không khả dụng.";
            return RedirectToAction("Index", "Home");
        }

        // Lấy giỏ hàng của user (nếu có), hoặc tạo mới
        var cart = _context.Carts
            .Include(c => c.Items)
            .FirstOrDefault(c => c.UserId == userId);

        if (cart == null)
        {
            cart = new Cart
            {
                UserId = userId.Value,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
            _context.Carts.Add(cart);
            _context.SaveChanges(); // Lưu để có Cart.Id
        }

        // Kiểm tra xem sản phẩm đã có trong giỏ chưa
        var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == product.Id);
        if (existingItem != null)
        {
            if (existingItem.Quantity < product.Stock)
            {
                existingItem.Quantity++;
            }
            else
            {
                TempData["Message"] = "Số lượng vượt quá tồn kho.";
                return RedirectToAction("Index", "Cart");
            }
        }
        else
        {
            var cartItem = new CartItem
            {
                CartId = cart.Id,
                ProductId = product.Id,
                Name = product.Name,
                Image = product.Image,
                Price = product.Price,
                Quantity = 1,
                Stock = product.Stock
            };
            _context.CartItems.Add(cartItem);
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
        if (cartItems == null || !cartItems.Any())
        {
            TempData["Message"] = "Không nhận được dữ liệu từ form!";
            return RedirectToAction("Index");
        }

        // In cartItems ra để kiểm tra
        foreach (var item in cartItems)
        {
            Console.WriteLine($"ProductId: {item.ProductId}, Quantity: {item.Quantity}");
        }

        var userId = GetUserId();
        if (userId == null)
        {
            return RedirectToAction("Login", "Account"); // Redirect nếu chưa đăng nhập
        }

        var cart = _context.Carts
            .Include(c => c.Items)
            .FirstOrDefault(c => c.UserId == userId);

        // Kiểm tra cartItems và update
        foreach (var item in cartItems)
        {
            var existingItem = cart.Items.FirstOrDefault(ci => ci.ProductId == item.ProductId);
            var product = _context.Products.FirstOrDefault(p => p.Id == item.ProductId);

            if (existingItem != null && product != null)
            {
                if (item.Quantity <= 0)
                {
                    cart.Items.Remove(existingItem);  // Xóa sản phẩm khỏi giỏ hàng nếu số lượng <= 0
                }
                else if (product.Stock >= item.Quantity)
                {
                    existingItem.Quantity = item.Quantity;  // Cập nhật số lượng
                    _context.Entry(existingItem).State = EntityState.Modified;  // Đặt trạng thái là Modified
                }
                else
                {
                    TempData["Message"] = $"Không đủ hàng cho {product.Name}";  // Thông báo nếu không đủ hàng
                }
            }
        }

        try
        {
            _context.SaveChanges();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            TempData["Message"] = "Có lỗi xảy ra khi cập nhật giỏ hàng!";
        }

        return RedirectToAction("Index");
    }




    //[HttpPost]
    //public IActionResult Update(List<CartItem> cartItems)
    //{
    //    var userId = GetUserId(); // hoặc Session
    //    if (userId == null)
    //    {
    //        return RedirectToAction("Login", "Account");
    //    }

    //    foreach (var updatedItem in cartItems)
    //    {
    //        var existingItem = _context.CartItems
    //            .FirstOrDefault(c => c.Id == userId && c.ProductId == updatedItem.ProductId);

    //        if (existingItem != null)
    //        {
    //            // Đảm bảo không vượt quá số lượng tồn kho
    //            var product = _context.Products.Find(updatedItem.ProductId);
    //            if (product != null)
    //            {
    //                existingItem.Quantity = Math.Min(updatedItem.Quantity, product.Stock);
    //            }
    //        }
    //    }

    //    _context.SaveChanges();

    //    TempData["Message"] = "Cập nhật giỏ hàng thành công!";
    //    return RedirectToAction("Index");
    //}

}
