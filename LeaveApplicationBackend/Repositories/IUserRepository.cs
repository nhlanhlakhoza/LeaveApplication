using LeaveApplicationBackend.Models;

namespace LeaveApplicationBackend.Repositories
{
    public interface IUserRepository

    {
        void Add(User user);
        Task<bool> UserExists(string email);
        Task<User> GetByEmailAsync(string email);
    }
}
