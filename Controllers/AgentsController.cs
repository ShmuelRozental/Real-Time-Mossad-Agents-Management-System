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

namespace Real_Time_Mossad_Agents_Management_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AgentsController : ControllerBase
    {
        private readonly AgentsServices _agentsServices;

        public AgentsController(AgentsServices agentsServices)
        {
            _agentsServices = agentsServices;
        }

        // GET: api/Agents
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Agent>>> GetAgents()
        {
            try
            {
                var agents = await _agentsServices.GetAllEntityAsync();
                return Ok(agents);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/Agents/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Agent>> GetAgent(int id)
        {
            try
            {
                var agent = await _agentsServices.GetEntityAsync(id);
                return Ok(agent);
            }
            catch (KeyNotFoundException)
            {
                return NotFound("Agent not found.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // POST: api/Agents
        [HttpPost]
        public async Task<ActionResult<int>> PostAgent(Agent agent)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var createdAgent = await _agentsServices.CreateEntity(agent);
                return CreatedAtAction(nameof(GetAgent), new { id = createdAgent.Id }, createdAgent.Id);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // PUT: api/Agents/5/pin
        [HttpPut("{id}/pin")]
        public async Task<IActionResult> SetInitialPosition(int id, [FromBody] Location location)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var agent = await _agentsServices.SetEntityLocation(id, location);
                return Ok(agent);
            }
            catch (KeyNotFoundException)
            {
                return NotFound("Agent not found.");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // PUT: api/Agents/5/move
        [HttpPut("{id}/move")]
        public async Task<IActionResult> MoveAgent(int id, [FromBody] string directionString)
        {
            if (!Enum.TryParse<Direction>(directionString, true, out var direction))
            {
                return BadRequest("Invalid direction value.");
            }

            try
            {
                var agent = await _agentsServices.EntityMovement(id, direction);
                return Ok(agent);
            }
            catch (KeyNotFoundException)
            {
                return NotFound("Agent not found.");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
