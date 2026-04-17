using LeaveApplicationBackend.Data;
using LeaveApplicationBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace LeaveApplicationBackend.Repositories
{
    public class LeaveRepository : ILeaveRepository
    {
        private readonly AppDbContext _context;

        public LeaveRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddLeave(LeaveApplication leave)
        {
            _context.LeaveApplications.Add(leave);
            await _context.SaveChangesAsync();
        }

        public async Task<LeaveApplication?> GetById(int id)
        {
            return await _context.LeaveApplications.FindAsync(id);
        }

        public async Task<List<LeaveApplication>> GetAll()
        {
            return await _context.LeaveApplications.ToListAsync();
        }

        public async Task UpdateLeave(LeaveApplication leave)
        {
            _context.LeaveApplications.Update(leave);
            await _context.SaveChangesAsync();
        }

        public async Task<LeaveBalance?> GetBalance(string email)
        {
            var balance = await _context.LeaveBalances
                .FirstOrDefaultAsync(x => x.EmployeeEmail == email);

            //  AUTO FIX IF MISSING
            if (balance == null)
            {
                balance = new LeaveBalance
                {
                    EmployeeEmail = email,
                    AnnualLeave = 0,
                    SickLeave = 10,
                    FamilyLeave = 5
                };

                _context.LeaveBalances.Add(balance);
                await _context.SaveChangesAsync();
            }

            return balance;
        }

        public async Task UpdateBalance(LeaveBalance balance)
        {
            _context.LeaveBalances.Update(balance);
            await _context.SaveChangesAsync();
        }

        public async Task AddBalance(LeaveBalance balance)
        {
            _context.LeaveBalances.Add(balance);
            await _context.SaveChangesAsync();
        }
        public async Task<List<LeaveApplication>> GetLeavesForWeek(DateTime start, DateTime end)
        {
            return await _context.LeaveApplications
                .Where(l => l.Status == "Approved" &&
                            l.StartDate <= end &&
                            l.EndDate >= start)
                .ToListAsync();
        }

        public async Task<List<LeaveApplication>> GetByEmailAsync(string email)
        {
            return await _context.LeaveApplications
                .Where(u => u.EmployeeEmail == email)
                .ToListAsync();
        }

        public async Task<List<LeaveApplication>> GetByEmailApproval(string email)
        {
            return await _context.LeaveApplications
                .Where(u => u.ApproverEmail == email && u.Status == "Pending")
                .ToListAsync();
        }
    }

 }
