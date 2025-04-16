using Microsoft.EntityFrameworkCore;
using TranNhatTu_2122110250.Data;
using TranNhatTu_2122110250.Model;

namespace TranNhatTu_2122110250.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;  // Thay vì UserManager, dùng AppDbContext

        public UserService(AppDbContext context)
        {
            _context = context;
        }

        // Xác thực người dùng
        public async Task<User> Authenticate(string email, string password)
        {
            // Tìm người dùng theo email
            var user = await _context.User.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null) return null;

            // Kiểm tra mật khẩu người dùng - bạn có thể lưu mật khẩu đã mã hóa trong database, hoặc dùng một phương pháp khác để so sánh mật khẩu
            // Ví dụ: Nếu bạn không mã hóa mật khẩu, bạn có thể so sánh trực tiếp
            if (user.Password != password)
                return null;

            return user;
        }

        // Lấy người dùng theo id
        public async Task<User> GetUserById(Guid id)
        {
            return await _context.User.FindAsync(id);
        }
    }
}
