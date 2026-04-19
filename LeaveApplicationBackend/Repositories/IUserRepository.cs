using LeaveApplicationBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace LeaveApplicationBackend.Repositories
{
    public interface IUserRepository

    {
        void Add(User user);
        Task<bool> UserExists(string email);
        Task<User> GetByEmailAsync(string email);
        Task<List<User>> GetAllUsers();
        Task Update(User user);
        Task DeleteUser(User user);
        Task<User?> GetSupervisorByTeam(string team);
        Task<User?> GetManager();
        Task<User?> GetGM();
        Task<User?> GetDirector();
        Task SaveChangesAsync();
    }
}
