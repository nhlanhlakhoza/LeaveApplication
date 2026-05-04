using LeaveApplicationBackend.DTO;
using LeaveApplicationBackend.Models;

namespace LeaveApplicationBackend.Repositories
{
    public interface ILeaveRepository
    {
        Task AddLeave(LeaveApplication leave);
        Task<LeaveApplication?> GetById(int id);
        Task<List<LeaveApplication>> GetAll();
        Task UpdateLeave(LeaveApplication leave);

        Task<LeaveBalance?> GetBalanceByUserId(int userId);
        Task UpdateBalance(LeaveBalance balance);
        Task AddBalance(LeaveBalance balance);
        Task<List<LeaveApplication>> GetLeavesForWeek(DateTime start, DateTime end);
        Task<List<LeaveApprovalDTO>> GetByEmailApproval(string email);
        Task<LeaveApplication?> GetByIdAsync(int id);
        Task<List<LeaveDTO>> GetLeaveByUserId(int userId);
        List<leaveTypeDTO> GetLeavesByEmail(string email);
        LeaveBalance GetLeaveBalanceByEmail(string email);
    }
}
