using LeaveApplicationBackend.DTO;
using LeaveApplicationBackend.Services;
using Microsoft.AspNetCore.Mvc;

namespace LeaveApplicationBackend.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController:ControllerBase
    {

        private readonly IUserService _service;
        private readonly IEmailService _emailService;
        public UserController(IUserService service, IEmailService emailService)
        {
            _service = service;
            _emailService = emailService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateUser(RegisterRequest request)
        {
            // Check if user already exists
            if (await _service.UserExists(request.Email))
                return BadRequest("User already exists");

           
            request.Password = GenerateRandomPassword(10);

            try
            {

                await _service.CreateUser(request);


                var emailSent = await _emailService.SendEmailAsync(
      request.Email,
      "Your New Password",
      $"Hello {request.FullName},\n\nYour new password is: {request.Password}\n\nPlease change it after login.",
      null // no attachments
         );



                if (!emailSent)
                {
                    // Log failure but return success for user creation
                    Console.WriteLine($"Failed to send email to {request.Email}");
                    return Ok("User created successfully, but failed to send email.");
                }

                return Ok("User created successfully and password sent via email.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error creating user or sending email: " + ex.Message);
                return StatusCode(500, "An error occurred while creating the user.");
            }

        }

        //Login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                var loginResponse = await _service.Login(request);
                return Ok(loginResponse); // returns token + message
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        private string GenerateRandomPassword(int length = 10)
        {
            const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*";
            var random = new Random();
            return new string(Enumerable.Repeat(validChars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _service.GetAllUsers();
            return Ok(users);
        }

        [HttpPut("edit-by-email/{email}")]
        public async Task<IActionResult> EditUser(string email, [FromBody] DTO.RegisterRequest request)
        {
            if (string.IsNullOrWhiteSpace(email))
                return BadRequest("Email is required");

            try
            {
                var result = await _service.EditUserByEmail(email, request);

                if (!result)
                    return NotFound("User not found");

                return Ok(new { message = "User updated successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }



        [HttpGet("by-email/{email}")]
        public async Task<IActionResult> GetUserByEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return BadRequest(new { message = "Email is required" });
            }

            try
            {
                // Decode email from URL
                var decodedEmail = Uri.UnescapeDataString(email);

                var user = await _service.GetUserByEmail(decodedEmail);

                if (user == null)
                {
                    return NotFound(new { message = "User not found" });
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Something went wrong", error = ex.Message });
            }
        }


    }
}
