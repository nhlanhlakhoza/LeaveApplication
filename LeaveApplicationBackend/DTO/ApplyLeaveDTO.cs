namespace LeaveApplicationBackend.DTO
{
    public class ApplyLeaveDTO
    {
        public string LeaveType { get; set; } = "";

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int NumberOfDays { get; set; }
        public string Reason { get; set; } = "";
        public string EmployeeName { get; set; } = "";
        public IFormFile? Document { get; set; }
    }
}
