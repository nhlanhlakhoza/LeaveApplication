using backendMpact.Services;
using LeaveApplicationBackend.Models;
using LeaveApplicationBackend.Repositories;

namespace LeaveApplicationBackend.Services
{
    public class LeaveService : ILeaveService
    {
        private readonly ILeaveRepository _repo;
        private readonly IUserRepository _userRepo;
        private readonly IEmailService _emailService;
        public LeaveService(ILeaveRepository repo, IUserRepository userRepo, IEmailService emailService)
        {
            _repo = repo;
            _userRepo = userRepo;
            _emailService = emailService;
        }

        public async Task<string> ApplyLeave(string email, LeaveApplication leave)
        {
            var user = await _userRepo.GetByEmailAsync(email);
            var balance = await _repo.GetBalance(email);

            if (user == null || balance == null)
                return "User or balance not found";

            //  CALCULATE DAYS FROM DATES
            leave.NumberOfDays = CalculateLeaveDays(leave.StartDate, leave.EndDate);

            if (leave.NumberOfDays <= 0)
                return "Invalid date range";

            //  CHECK BALANCE USING CALCULATED DAYS
            bool hasEnough = leave.LeaveType switch
            {
                "Annual" => balance.AnnualLeave >= leave.NumberOfDays,
                "Sick" => balance.SickLeave >= leave.NumberOfDays,
                "Family" => balance.FamilyLeave >= leave.NumberOfDays,
                _ => false
            };

            if (!hasEnough)
                return "Not enough leave balance";

            //  FIND APPROVER
            var approver = await GetApprover(user);

            if (approver == null)
                return "No approver found";

            leave.EmployeeEmail = email;
            leave.Status = "Pending";
            leave.ApproverEmail = approver.Email;

            await _repo.AddLeave(leave);

            //  EMAIL TO APPROVER
            await _emailService.SendEmailAsync(
                approver.Email,
                "New Leave Request",
                $"{user.FullName} requested {leave.NumberOfDays} day(s) of {leave.LeaveType} leave.",null
            );

            return "Leave submitted successfully";
        }
        public async Task AccrueLeave(User user)
        {
            var balance = await _repo.GetBalance(user.Email);

            if (balance == null)
                return;

            int yearsOfService = GetYearsOfService(user);

            double accrual = yearsOfService >= 5 ? 1.6 : 1.25;

            balance.AnnualLeave += accrual;
            balance.LastUpdated = DateTime.Now;

            await _repo.UpdateBalance(balance);
        }
        public async Task<List<LeaveApplication>> GetAll()
        {
            return await _repo.GetAll();
        }

        public async Task<string> ApproveLeave(int id, string approverEmail)
        {
            var leave = await _repo.GetById(id);

            if (leave == null)
                return "Not found";

            if (leave.ApproverEmail != approverEmail)
                return "Not allowed";

            var balance = await _repo.GetBalance(leave.EmployeeEmail);

            //  DEDUCT DAYS
            switch (leave.LeaveType)
            {
                case "Annual":
                    balance.AnnualLeave -= leave.NumberOfDays;
                    break;

                case "Sick":
                    balance.SickLeave -= leave.NumberOfDays;
                    break;

                case "Family":
                    balance.FamilyLeave -= leave.NumberOfDays;
                    break;
            }

            leave.Status = "Approved";

            await _repo.UpdateBalance(balance);
            await _repo.UpdateLeave(leave);

            return "Leave approved and deducted";
        }


        public async Task<string> RejectLeave(int id, string approverEmail)
        {
            var leave = await _repo.GetById(id);

            if (leave.ApproverEmail != approverEmail)
                return "Not allowed";

            leave.Status = "Rejected";

            await _repo.UpdateLeave(leave);

            return "Leave rejected";
        }

        private async Task<User?> GetApprover(User user)
        {
            switch (user.Role)
            {
                case "Employee":
                    //  Find supervisor based on team
                    return await _userRepo.GetSupervisorByTeam(user.Team);

                case "Supervisor":
                    return await _userRepo.GetManager();

                case "Manager":
                    return await _userRepo.GetGM();

                case "GM":
                    return await _userRepo.GetDirector();

                default:
                    return null;
            }
        }

        private int GetYearsOfService(User user)
        {
            int years = DateTime.Now.Year - user.HireDate.Year;

            if (user.HireDate.Date > DateTime.Now.AddYears(-years))
            {
                years--;
            }

            return years;
        }
        private int CalculateLeaveDays(DateTime start, DateTime end)
        {
            return (end - start).Days + 1; // +1 includes same-day leave
        }
    }
}
