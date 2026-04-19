
using LeaveApplicationBackend.Data;
using LeaveApplicationBackend.DTO;
using LeaveApplicationBackend.Models;
using LeaveApplicationBackend.Repositories;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.EntityFrameworkCore;
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
        private readonly ILeaveRepository _leaveRepo;
        public UserService(IUserRepository repo, ILeaveRepository leaveRepo)
        {
            _repo = repo;
            _leaveRepo = leaveRepo;
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
                Department = request.Department,
                Team =request.Team,
                HireDate=request.HireDate
            };
            //  SAVE USER TO THE DATABASE
            _repo.Add(user);
            await _repo.SaveChangesAsync(); 

            //  CREATE LEAVE BALANCE AUTOMATICALLY
            var balance = new LeaveBalance
            {   
                UserId = user.Id,   
                AnnualLeave = 0,
                SickLeave = 10,
                FamilyLeave = 5,
                LastUpdated = DateTime.Now
            };

            //  SAVE LEAVE BALANCE TO THE DATABASE
           
            await _leaveRepo.AddBalance(balance);
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


        public async Task<List<AllUsers>> GetAllUsers()
        {
            var users = await _repo.GetAllUsers();

            return users.Select(u => new AllUsers
            {
                Id = u.Id,
                FullName = u.FullName,
                Lastname = u.LastName,
                Email = u.Email,
                Role = u.Role,
                Department = u.Department,
                Position = u.Position
            }).ToList();
        }

        public async Task EditUser(DTO.RegisterRequest request)
        {
            var user = await _repo.GetByEmailAsync(request.Email); 

            if (user == null)
            {
                throw new Exception("User not found");
            }

            // Update existing fields
            user.FullName = request.FullName;
            user.LastName = request.LastName;
            user.Role = request.Role;
            user.Position = request.Position;
            user.Department = request.Department;
            user.Team = request.Team;

            // Save changes
            await _repo.Update(user);

           
        }

        public async Task<AllUsers?> GetUserByEmail(string email)
        {
            var user = await _repo.GetByEmailAsync(email);

            if (user == null)
                return null;

            return new AllUsers
            {
                Id = user.Id,
                FullName = user.FullName,
                Lastname = user.LastName,
                Email = user.Email,
                Role = user.Role,
                Department = user.Department,
                Position = user.Position,
                Team =user.Team
            };
        }

        public async Task<bool> EditUserByEmail(string email, DTO.RegisterRequest request)
        {
            var user = await _repo.GetByEmailAsync(email);

            if (user == null)
                return false;

           
            user.FullName = request.FullName;
            user.LastName = request.LastName;
            user.Role = request.Role;
            user.Position = request.Position;
            user.Department = request.Department;
            user.Email = request.Email;
            user.Team = request.Team;
            await _repo.Update(user);

            return true;
        }
        private readonly AppDbContext _context;

       
        public async Task<bool> DeleteUserByEmail(string email)
        {
            var user = await _repo.GetByEmailAsync(email);

            if (user == null)
                return false;

            await _repo.DeleteUser(user);

            return true;
        }
    }
}
