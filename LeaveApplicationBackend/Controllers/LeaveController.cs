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
        public async Task<IActionResult> Process(int id, string approverEmail, int action)
        {
            var result = await _service.ProcessLeave(id, approverEmail, action);

            if (result.Contains("Not") || result.Contains("Invalid"))
                return BadRequest(result);

            return Ok(result);
        }
    }
}
