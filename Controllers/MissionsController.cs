using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Real_Time_Mossad_Agents_Management_System.Data;
using Real_Time_Mossad_Agents_Management_System.Enums;
using Real_Time_Mossad_Agents_Management_System.Models;
using Real_Time_Mossad_Agents_Management_System.Services;

namespace Real_Time_Mossad_Agents_Management_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MissionsController<T> : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly MissionsServices<T> _missionsServices;


        public MissionsController(AppDbContext dbContext, MissionsServices<T> missionsServices)
        {
            _dbContext = dbContext;
            _missionsServices = missionsServices;

        }

        // GET: api/Missions
        [HttpGet]
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
        public async Task<ActionResult<Mission>> GetMission(int missionId)
        {
            var missions = await _missionsServices.GetAsync(missionId);

            if (missions == null)
            {
                return NotFound();
            }

            return Ok(missions);
        }

        // PUT: api/Missions/5
        [HttpPut("{id}")]
        public async Task<ActionResult> MissionAssignedment(int missionId, [FromBody] MissionStatus status)
        {
            await _missionsServices.UpdateStatus(missionId, status);

            return Ok();

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