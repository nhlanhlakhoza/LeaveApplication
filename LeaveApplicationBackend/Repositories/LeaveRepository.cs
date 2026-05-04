using LeaveApplicationBackend.Data;
using LeaveApplicationBackend.DTO;
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

        public async Task<LeaveBalance?> GetBalanceByUserId(int userId)
        {
           
            var balance = await _context.LeaveBalances
                .FirstOrDefaultAsync(x => x.UserId == userId);

            
            if (balance == null)
            {
                balance = new LeaveBalance
                {
                    UserId = userId,  
                    AnnualLeave = 0,
                    SickLeave = 10,
                    FamilyLeave = 5,
                    LastUpdated = DateTime.Now
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
            var exists = await _context.LeaveBalances
                .AnyAsync(x => x.UserId == balance.UserId);

            if (exists)
                return;

            _context.LeaveBalances.Add(balance);
            await _context.SaveChangesAsync();
        }
        public async Task<List<LeaveApplication>> GetLeavesForWeek(DateTime start, DateTime end)
        {
            return await _context.LeaveApplications
        .Include(l => l.User) 
        .Where(l => l.StartDate <= end
                 && l.EndDate >= start
                 && l.Status == "Approved")
        .ToListAsync();
        }


        public async Task<List<LeaveDTO>> GetLeaveByUserId(int userId)
        {
            return await _context.LeaveApplications
                .Where(l => l.UserId == userId )
                .OrderByDescending(l => l.StartDate)
                .Select(l => new LeaveDTO
                {
                    Id = l.Id,
                    LeaveType = l.LeaveType,
                    StartDate = l.StartDate,
                    EndDate = l.EndDate,
                    Status = l.Status,
                    NumberOfDays =l.NumberOfDays,
                    FullName = l.User.FullName,
                    Email = l.User.Email
                })
                .ToListAsync();
        }
        public async Task<List<LeaveApprovalDTO>> GetByEmailApproval(string email)
        {
            return await _context.LeaveApplications
                .Where(l => l.ApproverEmail == email && l.Status == "Pending")
                .Select(l => new LeaveApprovalDTO
                {
                    Id = l.Id,
                    LeaveType = l.LeaveType,
                    Status = l.Status,
                    FullName = l.User.FullName,
                    LastName = l.User.LastName,
                    Email = l.User.Email,
                    NumberOfDays = l.NumberOfDays,
                })
                .ToListAsync();
        }
        public async Task<LeaveApplication?> GetByIdAsync(int id)
        {
            return await _context.LeaveApplications
                .FirstOrDefaultAsync(l => l.Id == id);
        }
        public List<leaveTypeDTO> GetLeavesByEmail(string email)
        {
            return _context.LeaveBalances
                .Where(l => l.User.Email == email)
                .Select(l => new leaveTypeDTO
                {
                    Id = l.Id,
                    AnnualLeave = l.AnnualLeave,
                    SickLeave = l.SickLeave,
                    FamilyLeave = l.FamilyLeave,
                    Email = l.User.Email
                })
                .ToList();
        }
        public LeaveBalance GetLeaveBalanceByEmail(string email)
        {
            return _context.LeaveBalances
                .FirstOrDefault(l => l.User.Email == email);
        }
    }

 }
