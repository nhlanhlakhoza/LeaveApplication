namespace LeaveApplicationBackend.DTO
{
    public class LeaveDTO
    {
        public int Id { get; set; }
        public string LeaveType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; }

        public string FullName { get; set; }
        public string Email { get; set; }
        public int NumberOfDays { get; set; }
       
    }
}
