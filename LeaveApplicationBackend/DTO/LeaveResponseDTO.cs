namespace LeaveApplicationBackend.DTO
{
    public class LeaveResponseDTO
    {
        public int Id { get; set; }

        public string EmployeeEmail { get; set; } = "";

        public string LeaveType { get; set; } = "";

        public int NumberOfDays { get; set; }

        public string Status { get; set; } = "";

        public string ApproverEmail { get; set; } = "";

        public string RejectionReason { get; set; } = "";
    }
}
