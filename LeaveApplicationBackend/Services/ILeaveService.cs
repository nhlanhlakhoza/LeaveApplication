using LeaveApplicationBackend.DTO;
using LeaveApplicationBackend.Models;

namespace LeaveApplicationBackend.Services
{
    public interface ILeaveService
    {
        Task<string> ApplyLeave(string email, ApplyLeaveDTO dto);
        Task<List<LeaveApplication>> GetAll();
        Task<string> ProcessLeave(int id, string approverEmail, int action);
        Task SendWeeklyLeaveReport();
        Task<List<LeaveApplication>> GetLeaveByEmail(string email);

        Task<List<LeaveApplication>> GetLeaveByEmailApprove(string email);
    }
}
