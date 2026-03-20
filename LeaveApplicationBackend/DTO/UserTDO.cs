namespace LeaveApplicationBackend.DTO
{
    public class UserTDO
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
