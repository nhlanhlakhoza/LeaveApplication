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
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //  ONE-TO-ONE (User ↔ LeaveBalance)
            modelBuilder.Entity<User>()
                .HasOne(u => u.LeaveBalance)
                .WithOne(lb => lb.User)
                .HasForeignKey<LeaveBalance>(lb => lb.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // ONE-TO-MANY (User → LeaveApplications)
            modelBuilder.Entity<User>()
                .HasMany(u => u.LeaveApplications)
                .WithOne(l => l.User)
                .HasForeignKey(l => l.UserId);
        }
    }
}

