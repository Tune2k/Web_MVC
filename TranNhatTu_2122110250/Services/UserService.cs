using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TranNhatTu_2122110250.Data;
using TranNhatTu_2122110250.Model;

namespace TranNhatTu_2122110250.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;
        private readonly IPasswordHasher<User> _passwordHasher;

        public UserService(AppDbContext context, IPasswordHasher<User> passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        // Xác thực người dùng
        public async Task<User> Authenticate(string email, string password)
        {
            var user = await _context.User.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null) return null;

            var result = _passwordHasher.VerifyHashedPassword(user, user.Password, password);
            if (result == PasswordVerificationResult.Failed)
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
