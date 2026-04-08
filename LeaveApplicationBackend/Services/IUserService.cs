using Microsoft.AspNetCore.Identity.Data;
using LeaveApplicationBackend.DTO;
using Azure.Core;
using LeaveApplicationBackend.Models;
namespace LeaveApplicationBackend.Services
{
    public interface IUserService
    {
        Task<bool> UserExists(string email);
        Task CreateUser(DTO.RegisterRequest request);
        Task<LoginResponse> Login(DTO.LoginRequest request);
    }
}
