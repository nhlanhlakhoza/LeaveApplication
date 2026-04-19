using backendMpact.Services;
using LeaveApplicationBackend.DTO;
using LeaveApplicationBackend.Models;
using LeaveApplicationBackend.Repositories;

namespace LeaveApplicationBackend.Services
{
    public class LeaveService : ILeaveService
    {
        private readonly ILeaveRepository _repo;
        private readonly IUserRepository _userRepo;
        private readonly IEmailService _emailService;
        private readonly FileService _fileService;
        public LeaveService(ILeaveRepository repo, IUserRepository userRepo, IEmailService emailService, FileService fileService)
        {
            _repo = repo;
            _userRepo = userRepo;
            _emailService = emailService;
            _fileService = fileService;
        }

        public async Task<string> ApplyLeave(string email, ApplyLeaveDTO dto)
        {
            var user = await _userRepo.GetByEmailAsync(email);
            var balance = await _repo.GetBalanceByUserId(user.Id);


            if (user == null || balance == null)
                return "User or balance not found";

            // CALCULATE DAYS
            int numberOfDays = CalculateLeaveDays(dto.StartDate, dto.EndDate);

            if (numberOfDays <= 0)
                return "Invalid date range";

            // CHECK BALANCE
            bool hasEnough = dto.LeaveType switch
            {
                "Annual" => balance.AnnualLeave >= numberOfDays,
                "Sick" => balance.SickLeave >= numberOfDays,
                "Family" => balance.FamilyLeave >= numberOfDays,
                _ => false
            };

            if (!hasEnough)
                return "Not enough leave balance";

            // FIND APPROVER
            var approver = await GetApprover(user);

            if (approver == null)
                return "No approver found";

            // STEP 1: CREATE LEAVE (NO FILE YET)
            var leave = new LeaveApplication
            {
                UserId = user.Id,
                LeaveType = dto.LeaveType,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                NumberOfDays = numberOfDays,
                Status = "Pending",
                Reason = dto.Reason,
                ApproverEmail = approver.Email
            };

            // STEP 2: SAVE TO DB (GET ID)
            await _repo.AddLeave(leave);

            // STEP 3: SAVE FILE USING leave.Id
            string? documentPath = null;

            if (dto.LeaveType == "Sick" || dto.LeaveType == "Family")
            {
                if (dto.Document != null)
                {
                    documentPath = await _fileService.SaveFileToDesktop(
                        dto.Document,
                        leaveId: leave.Id,
                        isPdf: true
                    );
                }
            }

            // STEP 4: UPDATE LEAVE WITH FILE PATH
            leave.DocumentPath = documentPath;
            await _repo.UpdateLeave(leave);

            // EMAIL TO APPROVER
            await _emailService.SendEmailAsync(
                approver.Email,
                "New Leave Request",
                $"{user.FullName} requested {numberOfDays} day(s) of {dto.LeaveType} leave.",
                null
            );

            return "Leave submitted successfully";
        }
        public async Task AccrueLeave(User user)
        {
            var balance = await _repo.GetBalanceByUserId(user.Id);

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
        public async Task<List<LeaveApplication>> GetLeaveByEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return new List<LeaveApplication>();

            var user = await _userRepo.GetByEmailAsync(email);

            if (user == null)
                return new List<LeaveApplication>();

            var leaves = await _repo.GetByUserIdAsync(user.Id);

            return leaves ?? new List<LeaveApplication>();
        }

        public async Task<List<LeaveApprovalDTO>> GetLeaveByEmailApprove(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return new List<LeaveApprovalDTO>();

            var leaves = await _repo.GetByEmailApproval(email);

            return leaves ?? new List<LeaveApprovalDTO>();
        }
        public async Task<string> ProcessLeave(int id, string approverEmail, int action, string rejectionReason)
        {
            var leave = await _repo.GetById(id);

            if (leave == null)
                return "Not found";

            if (leave.ApproverEmail != approverEmail)
                return "Not allowed";
            if (leave == null)
                return "Not found";

            var user = await _userRepo.GetByIdAsync(leave.UserId);
            if (user == null)
                return "User not found";

            //  APPROVE
            if (action == 1)
            {
                var balance = await _repo.GetBalanceByUserId(leave.UserId);

                if (balance == null)
                    return "Balance not found";

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
                leave.ActionDate = DateTime.Now;
                leave.RejectionReason = "";
                await _repo.UpdateBalance(balance);
                await _repo.UpdateLeave(leave);

                //  EMAIL
                await _emailService.SendEmailAsync(
                   user.Email,
                    "Leave Approved",
                    $"Good news {user.FullName}, your leave has been APPROVED.",
                    null
                );

                return "Leave approved and deducted";
            }

            //  REJECT
            else if (action == 2)
            {
                leave.Status = "Rejected";
                leave.ActionDate = DateTime.Now;
                leave.RejectionReason = rejectionReason;
                await _repo.UpdateLeave(leave);

                await _emailService.SendEmailAsync(
                    user.Email,
                    "Leave Rejected",
                    $"Hello {user.FullName}, your leave request has been REJECTED. Reason: {leave.RejectionReason}",
                    null
                );

                return "Leave rejected";
            }

            return "Invalid action";
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

        private (DateTime start, DateTime end) GetCurrentWeek()
        {
            var today = DateTime.Today;

            int diff = (7 + (today.DayOfWeek - DayOfWeek.Monday)) % 7;
            var startOfWeek = today.AddDays(-diff).Date;
            var endOfWeek = startOfWeek.AddDays(6);

            return (startOfWeek, endOfWeek);
        }
        private string BuildWeeklyEmail(List<LeaveApplication> leaves, DateTime start, DateTime end)
        {
            if (leaves == null || !leaves.Any())
                return $"No employees are on leave this week ({start:dd MMM} - {end:dd MMM}).";

            var message = $"Employees on leave this week ({start:dd MMM} - {end:dd MMM}):\n\n";

            foreach (var l in leaves)
            {
                var name = l.User?.FullName ?? "Unknown User";
                var email = l.User?.Email ?? "No Email";

                message += $"- {name} ({email}) " +
                           $"({l.LeaveType}) from {l.StartDate:dd MMM} to {l.EndDate:dd MMM}\n";
            }

            return message;
        }

        public async Task SendWeeklyLeaveReport()
        {
            var (start, end) = GetCurrentWeek();

            var leaves = await _repo.GetLeavesForWeek(start, end);

            var emailBody = BuildWeeklyEmail(leaves, start, end);

            var users = await _userRepo.GetAllUsers();

            foreach (var user in users)
            {
                await _emailService.SendEmailAsync(
                    user.Email,
                    "Weekly Leave Report",
                    emailBody,
                    null
                );
            }
        }
        public async Task<LeaveApplication?> GetLeaveByIdAsync(int id)
        {
            return await _repo.GetByIdAsync(id);
        }


    }
}
