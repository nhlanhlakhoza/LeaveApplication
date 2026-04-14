using LeaveApplicationBackend.Models;

namespace LeaveApplicationBackend.Services
{
    public interface ILeaveService
    {
        Task<string> ApplyLeave(string email, LeaveApplication leave);
        Task<List<LeaveApplication>> GetAll();

        Task<string>  ApproveLeave(int id, string approverEmail);
        Task<string>  RejectLeave(int id, string approverEmail);
       
    }
}
