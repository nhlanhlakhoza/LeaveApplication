namespace LeaveApplicationBackend.DTO
{
    public class RegisterRequest
    {
        public string FullName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
      
        public string Position {get; set; }
        public string Department { get; set; }
        public string Team{ get; set; }

        public DateTime HireDate { get; set; }
    }
}
