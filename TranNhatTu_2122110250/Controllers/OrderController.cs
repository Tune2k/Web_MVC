using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using TranNhatTu_2122110250.Model;
using TranNhatTu_2122110250.Data;
using TranNhatTu_2122110250.Helpers;

namespace TranNhatTu_2122110250.Controllers
{
    public class OrderController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILogger<OrderController> _logger;

        public OrderController(AppDbContext context, ILogger<OrderController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: /Order/
        public async Task<IActionResult> Index()
        {
            var userId = SessionUser.GetUserId(HttpContext);
            if (userId == null)
            {
                TempData["Message"] = "Bạn cần đăng nhập để đặt hàng.";
                return RedirectToAction("Login", "Account");
            }

            var user = await _context.User.FirstOrDefaultAsync(u => u.Id == userId.Value);
            if (user != null)
            {
                ViewBag.Username = user.Username;
                ViewBag.Email = user.Email;
            }

            var cart = await _context.Carts
                                      .Include(c => c.Items)
                                      .FirstOrDefaultAsync(c => c.UserId == userId.Value);
            if (cart == null || !cart.Items.Any())
            {
                TempData["Message"] = cart == null
                    ? "Không tìm thấy giỏ hàng."
                    : "Giỏ hàng của bạn đang trống!";
                return RedirectToAction("Index", "Cart");
            }

            var vm = new OrderViewModel
            {
                CustomerName = user.Username,
                CustomerEmail = user.Email,
                CartItems = cart.Items.Select(i => new CartItemViewModel
                {
                    ProductId = i.ProductId,
                    Name = i.Name,
                    Price = i.Price,
                    Quantity = i.Quantity
                }).ToList(),
                TotalPrice = cart.Items.Sum(i => i.Price * i.Quantity)
            };


            return View(vm);
        }

        // POST: /Order/PlaceOrder
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlaceOrderAjax([FromBody] OrderViewModel model)
        {
            var userId = SessionUser.GetUserId(HttpContext);
            if (userId == null)
            {
                _logger.LogWarning("User not logged in via AJAX.");
                return Json(new { success = false, message = "Vui lòng đăng nhập để đặt hàng." });
            }

            _logger.LogInformation("AJAX Order: {Name} - {Email} - {Total} - {Items}",
                model.CustomerName, model.CustomerEmail, model.TotalPrice, model.CartItems?.Count);

            if (!ModelState.IsValid || model.CartItems == null || !model.CartItems.Any())
            {
                return Json(new
                {
                    success = false,
                    message = "Dữ liệu không hợp lệ hoặc giỏ hàng trống.",
                    errors = ModelState.Values.SelectMany(x => x.Errors).Select(e => e.ErrorMessage)
                });
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var order = new Order
                {
                    CustomerName = model.CustomerName,
                    CustomerEmail = model.CustomerEmail,
                    TotalPrice = model.TotalPrice,
                    OrderDate = DateTime.Now,
                    UserId = userId.Value
                };
                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                var orderDetails = model.CartItems.Select(i => new OrderDetail
                {
                    OrderId = order.Id,
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,
                    Price = (decimal)i.Price
                }).ToList();

                _context.OrderDetails.AddRange(orderDetails);

                var cart = await _context.Carts
                                         .Include(c => c.Items)
                                         .FirstOrDefaultAsync(c => c.UserId == userId.Value);
                if (cart != null)
                {
                    _context.CartItems.RemoveRange(cart.Items);
                    _context.Carts.Remove(cart);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Json(new { success = true, message = "Đặt hàng thành công!" });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "AJAX PlaceOrder failed: {Message}", ex.Message);
                return Json(new { success = false, message = "Lỗi hệ thống." });
            }
        }


        // GET: /Order/Success
        public IActionResult Success()
        {
            return View();
        }
    }
}
