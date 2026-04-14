namespace LeaveApplicationBackend.Models
{
    public class LeaveBalance
    {
        public int Id { get; set; }

        public string EmployeeEmail { get; set; } = "";

        public double AnnualLeave { get; set; } = 0;    
        public double SickLeave { get; set; } = 10;      
        public double FamilyLeave { get; set; } = 5;     

        public DateTime LastUpdated { get; set; } = DateTime.Now;
    }
}
