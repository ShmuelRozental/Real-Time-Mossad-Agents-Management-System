using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Real_Time_Mossad_Agents_Management_System.Attributes;
using Real_Time_Mossad_Agents_Management_System.Data;
using Real_Time_Mossad_Agents_Management_System.Enums;
using Real_Time_Mossad_Agents_Management_System.Models;
using Real_Time_Mossad_Agents_Management_System.Services;

namespace Real_Time_Mossad_Agents_Management_System.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class MissionsController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly MissionsServices<Mission> _missionsServices;


        public MissionsController(AppDbContext dbContext, MissionsServices<Mission> missionsServices)
        {
            _dbContext = dbContext;
            _missionsServices = missionsServices;

        }

        // GET: api/Missions
        [HttpGet]
        //[AuthorizeRoles("Admin", "Manager")]
        public async Task<ActionResult<IEnumerable<Mission>>> GetMissions()
        {
            try
            {
                var missions = await _missionsServices.GetAllAsync();
                return Ok(_missionsServices);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/Missions/5
        [HttpGet("{id}")]
        //[AuthorizeRoles("SimulationServer")]

        public async Task<ActionResult<Mission>> GetMission(int id)
        {
            var mission = await _missionsServices.GetAsync(id);

            if (mission == null)
            {
                return NotFound();
            }

            return Ok(mission);
        }

        // PUT: api/Missions/5
        [HttpPut("{id}")]
        public async Task<ActionResult> MissionAssignedment(int id, [FromBody] Dictionary<string, string> status)
        {
            
            if (status.TryGetValue("status", out var statusValue))
            {
               
                if (Enum.TryParse<MissionStatus>(statusValue, true, out var missionStatus))
                {
                    await _missionsServices.UpdateStatus(id, missionStatus);
                    return Ok();
                }
                else
                {
                    return BadRequest("Invalid MissionStatus value.");
                }
            }
            else
            {
                return BadRequest("Status not provided.");
            }
        }
        //POST: api/Missions/Update
        [HttpPost("Update")]
        public async Task<ActionResult> UpdateMissions()
        {
            await _missionsServices.StartMissionsAsync();
            return Ok();
        }
    }
}