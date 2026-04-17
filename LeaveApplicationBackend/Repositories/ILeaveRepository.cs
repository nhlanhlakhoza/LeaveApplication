using LeaveApplicationBackend.Models;

namespace LeaveApplicationBackend.Repositories
{
    public interface ILeaveRepository
    {
        Task AddLeave(LeaveApplication leave);
        Task<LeaveApplication?> GetById(int id);
        Task<List<LeaveApplication>> GetAll();
        Task UpdateLeave(LeaveApplication leave);

        Task<LeaveBalance?> GetBalance(string email);
        Task UpdateBalance(LeaveBalance balance);
        Task AddBalance(LeaveBalance balance);
        Task<List<LeaveApplication>> GetLeavesForWeek(DateTime start, DateTime end);
        Task<List<LeaveApplication>> GetByEmailAsync(string email);
        Task<List<LeaveApplication>> GetByEmailApproval(string email);
        Task<LeaveApplication?> GetByIdAsync(int id);
    }
}
