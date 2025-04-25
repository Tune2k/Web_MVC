using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TranNhatTu_2122110250.Data;
using TranNhatTu_2122110250.Model;
using System.Linq;
using TranNhatTu_2122110250.Areas.Admin.ViewModels;

namespace TranNhatTu_2122110250.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UserController : Controller
    {
        private readonly AppDbContext _context;

        // Constructor để inject DbContext
        public UserController(AppDbContext context)
        {
            _context = context;
        }

        // Hiển thị danh sách người dùng
        public IActionResult Index(string searchTerm, int page = 1, int pageSize = 5)
        {
            var query = _context.User.AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(u => u.Username.Contains(searchTerm));
            }

            int totalUsers = query.Count();
            var users = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var model = new UserIndexViewModel
            {
                Users = users,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(totalUsers / (double)pageSize)
            };

            return View(model);
        }


        // Hiển thị trang tạo người dùng
        public IActionResult Create()
        {
            return View(); // Trả về View tạo người dùng
        }

        // Xử lý tạo người dùng
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(User user)
        {
            if (ModelState.IsValid)
            {
                _context.User.Add(user); // Thêm người dùng vào DbContext
                _context.SaveChanges(); // Lưu vào cơ sở dữ liệu
                TempData["SuccessMessage"] = "Người dùng đã được tạo thành công!"; // Thêm thông báo thành công
                return RedirectToAction(nameof(Index)); // Sau khi tạo người dùng, chuyển đến trang danh sách
            }
            TempData["ErrorMessage"] = "Thông tin người dùng không hợp lệ."; // Thêm thông báo lỗi nếu không hợp lệ
            return View(user); // Nếu không hợp lệ, trả lại trang tạo người dùng
        }

        // Hiển thị trang chỉnh sửa người dùng
        // GET: /Admin/User/Edit/1
        // GET: /Admin/User/Edit/1
        // GET: Admin/User/Edit/5
        public IActionResult Edit(int id)
        {
            var user = _context.User.Find(id);
            if (user == null)
            {
                return NotFound();
            }

            // CHUYỂN SANG VIEWMODEL
            var viewModel = new UserEditViewModel
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role

                // Không đưa Password vào nếu không cần sửa
            };

            return View(viewModel); // ✅ View đang dùng UserEditViewModel
        }


        // Xử lý chỉnh sửa người dùng
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(UserEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _context.User.Find(model.Id);
                if (user == null)
                {
                    return NotFound();
                }
                user.Username = model.Username;
                user.Email = model.Email;
                user.Role = model.Role;
                // Không thay đổi Password nếu không nhập

                _context.Update(user);
                _context.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(model); // ✅ Trả lại đúng ViewModel nếu có lỗi
        }












        // Xác nhận xóa người dùng
        public IActionResult Delete(int id)
        {
            var user = _context.User.Find(id); // Tìm người dùng theo ID
            if (user == null)
            {
                TempData["ErrorMessage"] = "Người dùng không tồn tại."; // Thêm thông báo lỗi
                return RedirectToAction(nameof(Index)); // Trở lại trang danh sách người dùng nếu không tìm thấy
            }
            return View(user); // Trả về View xác nhận xóa
        }

        // Xử lý xóa người dùng
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var user = _context.User.Find(id); // Tìm người dùng theo ID
            if (user != null)
            {
                // Xóa tất cả cart liên quan tới user
                var carts = _context.Carts.Where(c => c.UserId == id).ToList();
                _context.Carts.RemoveRange(carts);

                _context.User.Remove(user); // Xóa người dùng
                _context.SaveChanges(); // Lưu thay đổi vào cơ sở dữ liệu

                TempData["SuccessMessage"] = "Người dùng và giỏ hàng đã được xóa thành công!";
            }
            else
            {
                TempData["ErrorMessage"] = "Không tìm thấy người dùng để xóa.";
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateRole(int id, string role)
        {
            var user = await _context.User.FindAsync(id); // Đảm bảo là DbSet tên "Users"

            if (user == null)
            {
                return NotFound();
            }

            user.Role = role;
            _context.Update(user);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}
