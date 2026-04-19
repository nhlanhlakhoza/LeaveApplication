using LeaveApplicationBackend.Data;
using LeaveApplicationBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace LeaveApplicationBackend.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;
        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public void Add(User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();
        }
              public async Task<bool> UserExists(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }
        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<List<User>> GetAllUsers()
        {
            return await _context.Users.ToListAsync();
        }
       
        public async Task Update(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteUser(User user)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }

        public async Task<User?> GetSupervisorByTeam(string team)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Role == "Supervisor" && u.Team == team);
        }

        public async Task<User?> GetManager()
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Role == "Manager");
        }

        public async Task<User?> GetGM()
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Role == "GM");
        }

        public async Task<User?> GetDirector()
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Role == "Director");
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
