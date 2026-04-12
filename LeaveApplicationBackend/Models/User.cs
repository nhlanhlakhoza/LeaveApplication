using System.ComponentModel.DataAnnotations;

namespace LeaveApplicationBackend.Models
{
    public class User
    {
        public int Id { get; set; }
        [Required]
        public string FullName { get; set; }
        [Required]
        public string LastName { get; set; }
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]

        public string Role { get; set; }

        [Required]
        public string Status { get; set; }
        [Required]
        public string Position {  get; set; }

        [Required]
        public string Department { get; set; }

    }
}
