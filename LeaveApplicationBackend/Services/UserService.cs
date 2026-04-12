
using LeaveApplicationBackend.Models;
using LeaveApplicationBackend.Repositories;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace LeaveApplicationBackend.Services
{
    public class UserService : IUserService
    {
        // Hardcoded JWT values
        private const string JwtKey = "4bb6d1dfbafb64a681139d1586b6f1160d18159afd57c8c79136d7490630407c";
        private const string JwtIssuer = "LeaveApplication";
        private const string JwtAudience = "BlazorClient";

        private readonly IUserRepository _repo;
        private readonly IConfiguration _config;
        public UserService(IUserRepository repo)
        {
            _repo = repo;

        }

        public async Task CreateUser(DTO.RegisterRequest request)
        {
            //  Hash the password here
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(request.Password);
            var hash = sha.ComputeHash(bytes);
            var hashedPassword = Convert.ToBase64String(hash);
            // Convert DTO → User model
            var user = new User
            {

                Email = request.Email,
                FullName = request.FullName,
                LastName = request.LastName,
                Password = hashedPassword,
                Role = request.Role,
                Status = "Active",
                Position = request.Position,
                Department=request.Department,
            };

            //  Save to database
            _repo.Add(user);
        }

        public Task<bool> UserExists(string email)
        {
            return _repo.UserExists(email);
        }

       

        // JWT GENERATOR
        private string GenerateJwtToken(User user)
        {
            // Secret key
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
            new Claim("Email", user.Email.ToString()),
            new Claim("UserId", user.Id.ToString()),
            new Claim("Role",user.Role.ToString()),
            new Claim("FullName", user.FullName.ToString()),
             new Claim("lastName", user.LastName.ToString()),
        };

            // Create token
            var token = new JwtSecurityToken(
                issuer: JwtIssuer,
                audience: JwtAudience,
                claims: claims,
                expires: DateTime.Now.AddHours(2), // <-- token expires in 2 hours
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<DTO.LoginResponse> Login(DTO.LoginRequest request)
        {
            var user = await _repo.GetByEmailAsync(request.Email);

            if (user == null || string.IsNullOrEmpty(user.Password))
                throw new Exception("User not found or password not set");

            // HASH THE INPUT PASSWORD
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(request.Password);
            var hash = sha.ComputeHash(bytes);
            var hashedPassword = Convert.ToBase64String(hash);

            if (user.Password != hashedPassword)
                throw new Exception("Invalid password");

            // GENERATE JWT
            var token = GenerateJwtToken(user);

            return new DTO.LoginResponse(token, "Login successful");
        }
    }
}
