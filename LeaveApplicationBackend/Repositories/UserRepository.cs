using backendMpact.Data;
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

            return true;
        }
    }
}
