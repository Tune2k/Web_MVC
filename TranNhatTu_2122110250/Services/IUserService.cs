using TranNhatTu_2122110250.Model;

namespace TranNhatTu_2122110250.Services
{
    public interface IUserService
    {
        Task<User> Authenticate(string email, string password);  // Phương thức để xác thực người dùng
        Task<User> GetUserById(Guid id);  // Phương thức lấy thông tin người dùng theo id
    }
}
