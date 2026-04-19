namespace LeaveApplicationBackend.DTO
{
    public class LeaveApprovalDTO
    {
        public int Id { get; set; }
        public string LeaveType { get; set; }
        public string Status { get; set; }
        public string FullName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public int NumberOfDays { get; set; }
        
    }
}
