using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TranNhatTu_2122110250.Data;
using TranNhatTu_2122110250.Model;
using System.Linq;

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
        public IActionResult Index()
        {
            var users = _context.User.ToList(); // Lấy danh sách người dùng từ DbContext
            return View(users); // Trả về View danh sách người dùng
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
        public IActionResult Edit(int id)
        {
            var user = _context.User.Find(id);
            if (user == null)
            {
                TempData["ErrorMessage"] = "Người dùng không tồn tại.";
                return RedirectToAction(nameof(Index));
            }

            Console.WriteLine($"DEBUG: Id = {user.Id}, Name = {user.FirstName} {user.LastName}, Email = {user.Email}");
            return View(user); // Trả về view với dữ liệu người dùng
        }

        // Xử lý chỉnh sửa người dùng
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, User user)
        {
            if (id != user.Id)
            {
                TempData["ErrorMessage"] = "ID người dùng không khớp.";
                return RedirectToAction(nameof(Index));
            }

            if (ModelState.IsValid)
            {
                var existingUser = _context.User.FirstOrDefault(u => u.Id == id);
                if (existingUser == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy người dùng.";
                    return RedirectToAction(nameof(Index));
                }

                existingUser.FirstName = user.FirstName;
                existingUser.LastName = user.LastName;
                existingUser.Email = user.Email;
                existingUser.Password = user.Password;

                try
                {
                    _context.SaveChanges();
                    TempData["SuccessMessage"] = "Người dùng đã được cập nhật thành công!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    TempData["ErrorMessage"] = "Có lỗi xảy ra khi cập nhật người dùng.";
                    return RedirectToAction(nameof(Index));
                }
            }

            TempData["ErrorMessage"] = "Thông tin người dùng không hợp lệ.";
            return View(user);
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
                _context.User.Remove(user); // Xóa người dùng
                _context.SaveChanges(); // Lưu thay đổi vào cơ sở dữ liệu
                TempData["SuccessMessage"] = "Người dùng đã được xóa thành công!"; // Thêm thông báo thành công
            }
            else
            {
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi xóa người dùng."; // Thêm thông báo lỗi nếu không xóa được
            }
            return RedirectToAction(nameof(Index)); // Quay lại danh sách người dùng sau khi xóa
        }
    }
}
