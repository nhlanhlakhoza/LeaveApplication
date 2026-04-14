using LeaveApplicationBackend.Models;
using LeaveApplicationBackend.Services;
using Microsoft.AspNetCore.Mvc;

namespace LeaveApplicationBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LeaveController : ControllerBase
    {
        private readonly ILeaveService _service;

        public LeaveController(ILeaveService service)
        {
            _service = service;
        }

        // ✅ APPLY LEAVE (AUTO ASSIGN APPROVER)
        [HttpPost("apply")]
        public async Task<IActionResult> ApplyLeave([FromBody] LeaveApplication leave, string email)
        {
            var result = await _service.ApplyLeave(email, leave);

            if (result.Contains("not found") || result.Contains("No approver"))
                return BadRequest(result);

            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _service.GetAll());
        }

        // ✅ APPROVE (FINAL)
        [HttpPut("approve/{id}")]
        public async Task<IActionResult> Approve(int id, string approverEmail)
        {
            var result = await _service.ApproveLeave(id, approverEmail);

            if (result.Contains("Not allowed") || result.Contains("Not found"))
                return BadRequest(result);

            return Ok(result);
        }

        // ❌ REJECT
        [HttpPut("reject/{id}")]
        public async Task<IActionResult> Reject(int id, string approverEmail)
        {
            var result = await _service.RejectLeave(id, approverEmail);

            if (result.Contains("Not allowed") || result.Contains("Not found"))
                return BadRequest(result);

            return Ok(result);
        }
    }
}
