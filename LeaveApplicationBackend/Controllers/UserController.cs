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

        private string GenerateRandomPassword(int length = 10)
        {
            const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*";
            var random = new Random();
            return new string(Enumerable.Repeat(validChars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }


    }
}
