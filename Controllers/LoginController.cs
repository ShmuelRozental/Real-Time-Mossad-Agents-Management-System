using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Real_Time_Mossad_Agents_Management_System.Data;
using Real_Time_Mossad_Agents_Management_System.Interfaces;
using Real_Time_Mossad_Agents_Management_System.Models;

namespace Real_Time_Mossad_Agents_Management_System.Controllers
{

    [Route("[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly AppDbContext _appDbContext;
        private readonly IJwtService _jwtService;
        private const string ValidId = "SimulationServer";

        public LoginController(AppDbContext appDbContext, IJwtService jwtService)
        {
            _appDbContext = appDbContext;
            _jwtService = jwtService;
        }

        [HttpPost]
        public async Task<ActionResult> Login([FromBody] User user)
        {
            //var validUser = await _appDbContext.Users
            //    .FirstOrDefaultAsync(u => u.Name == user.Name && u.Password == user.Password);

            if (user == null || string.IsNullOrEmpty(user.Id))
            {
                Console.WriteLine("Received invalid JSON.");
                return BadRequest("Invalid or missing JSON payload.");
            }

            if (user.Id != ValidId)
            {
                return Unauthorized("Invalid ID.");
            }

            var token = _jwtService.GenerateJwtToken(user);

            Response.Headers.Add("Authorization", $"Bearer {token}");

            return Ok( new { token = token });
        }
    }
}