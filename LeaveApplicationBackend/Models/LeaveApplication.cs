namespace LeaveApplicationBackend.Models
{
    public class LeaveApplication
    {
        public int Id { get; set; }

        public string EmployeeEmail { get; set; } = "";
        public string EmployeeName { get; set; } = "";

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public string LeaveType { get; set; } = "Annual"; // Annual, Sick, Family
        public string Reason { get; set; } = "";

        public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected, Cancelled
                                                        
        public string ApproverEmail { get; set; } = "";
        public string? ActionBy { get; set; }
        public DateTime? ActionDate { get; set; }
        public DateTime ApprovedDate { get; set; }
        public int NumberOfDays { get; set; }
        public string? DocumentPath { get; set; }
    }
}
