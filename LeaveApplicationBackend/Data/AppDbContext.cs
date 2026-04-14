using LeaveApplicationBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace LeaveApplicationBackend.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<User> Users { get; set; }   // Creates Users table on the database
        public DbSet<LeaveApplication> LeaveApplications { get; set; }

        public DbSet<LeaveBalance> LeaveBalances { get; set; }
    }
}

