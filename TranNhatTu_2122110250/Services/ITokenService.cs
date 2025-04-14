using TranNhatTu_2122110250.Model;

namespace TranNhatTu_2122110250.Services
{
    public interface ITokenService
    {
        string GenerateToken(User user);  // Phương thức tạo token
    }
}
