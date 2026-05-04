using LeaveApplicationBackend.DTO;
using LeaveApplicationBackend.Models;

namespace LeaveApplicationBackend.Services
{
    public interface ILeaveService
    {
        Task<string> ApplyLeave(string email, ApplyLeaveDTO dto);
        Task<List<LeaveApplication>> GetAll();
        Task<string> ProcessLeave(int id, string approverEmail, int action, string rejectionReason);
        Task SendWeeklyLeaveReport();
        Task<List<LeaveDTO>> GetLeaveByEmail(string email);

        Task<List<LeaveApprovalDTO>> GetLeaveByEmailApprove(string email);
        Task<LeaveApplication?> GetLeaveByIdAsync(int id);
        LeaveSummaryDto GetLeaveTotalsByEmail(string email);
    }
}
