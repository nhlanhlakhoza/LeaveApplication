using LeaveApplicationBackend.DTO;
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


        [HttpPost("apply")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> ApplyLeave([FromForm] ApplyLeaveDTO dto, [FromQuery] string email)
        {
            var result = await _service.ApplyLeave(email, dto);

            if (result.Contains("not found") || result.Contains("No approver"))
                return BadRequest(result);

            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _service.GetAll());
        }

        [HttpPut("process/{id}")]
        public async Task<IActionResult> Process(int id, string approverEmail, int action, string rejectionReason)
        {
            var result = await _service.ProcessLeave(id, approverEmail, action, rejectionReason);

            if (result.Contains("Not") || result.Contains("Invalid"))
                return BadRequest(result);

            return Ok(result);
        }

       
        [HttpGet("by-user/{email}")]
        public async Task<ActionResult<List<LeaveApplication>>> GetLeaveByEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return BadRequest("Email is required.");

            var leaves = await _service.GetLeaveByEmail(email);

            if (leaves == null || !leaves.Any())
                return Ok(new List<LeaveApplication>());

            return Ok(leaves);
        }

        
        [HttpGet("approver/{email}")]
        public async Task<ActionResult<List<LeaveApplication>>> GetApproveByEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return BadRequest("Email is required.");

            var leaves = await _service.GetLeaveByEmailApprove(email);

            if (leaves == null || !leaves.Any())
                return Ok(new List<LeaveApplication>());

            return Ok(leaves);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetLeaveById(int id)
        {
            var leave = await _service.GetLeaveByIdAsync(id);

            if (leave == null)
                return NotFound();

            return Ok(leave);
        }

        [HttpGet("totals/{email}")]
        public IActionResult GetLeaveTotals(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return BadRequest("Email is required");
            }

            var result = _service.GetLeaveTotalsByEmail(email);

            return Ok(result);
        }
    }
}
