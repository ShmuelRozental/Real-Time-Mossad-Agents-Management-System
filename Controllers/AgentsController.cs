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
    [Route("agents")]
    [ApiController]
    public class AgentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AgentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Agents
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Agent>>> GetAgents()
        {
            return await _context.Agents.ToListAsync();
        }

        // GET: api/Agents/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Agent>> GetAgent(int id)
        {
            var agent = await _context.Agents.FindAsync(id);

            if (agent == null)
            {
                return NotFound();
            }

            return agent;
        }

        // PUT: api/Agents/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAgent(int id, Agent agent)
        {
            if (id != agent.Id)
            {
                return BadRequest();
            }

            _context.Entry(agent).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AgentExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Agents
        [HttpPost]
        public async Task<ActionResult<int>> PostAgent(Agent agent)
        {
            _context.Agents.Add(agent);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAgent", new { id = agent.Id }, agent.Id);
        }

        // DELETE: api/Agents/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAgent(int id)
        {
            var agent = await _context.Agents.FindAsync(id);
            if (agent == null)
            {
                return NotFound();
            }

            _context.Agents.Remove(agent);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        //PUT: api/5/pin
        [HttpPut("{id}/pin")]
        public IActionResult SetInitialPosition(int id, [FromBody] PinLocation location)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var agent = _context.Agents.Find(id);
            if (agent == null)
            {
                return NotFound();
            }

            if (agent.Location == null)
            {
                agent.Location = new PinLocation();
            }
            agent.Location.X = location.X;
            agent.Location.Y = location.Y;

            _context.SaveChanges();

            return Ok(agent);
        }

        //PUT: api/5/move
        [HttpPut("{id}/move")]
        public IActionResult MoveAgent(int id, [FromBody] string directionString)
        {
            var agent = _context.Targets.Find(id);
            if (agent == null)
            {
                return NotFound();
            }

            if (!Enum.TryParse<Direction>(directionString, true, out var direction))
            {
                return BadRequest("Invalid direction value.");
            }

            if (agent.Location == null)
            {
                agent.Location = new PinLocation();
            }

            switch (direction)
            {
                case Direction.NW:
                    agent.Location.X -= 1;
                    agent.Location.Y -= 1;
                    break;
                case Direction.N:
                    agent.Location.Y -= 1;
                    break;
                case Direction.NE:
                    agent.Location.X -= 1;
                    agent.Location.Y += 1;
                    break;
                case Direction.W:
                    agent.Location.Y -= 1;
                    break;
                case Direction.E:
                    agent.Location.Y += 1;
                    break;
                case Direction.SW:
                    agent.Location.X += 1;
                    agent.Location.Y -= 1;
                    break;
                case Direction.S:
                    agent.Location.Y += 1;
                    break;
                case Direction.SE:
                    agent.Location.X += 1;
                    agent.Location.Y += 1;
                    break;
            }

            _context.SaveChanges();
            return Ok(agent);
        }


    private bool AgentExists(int id)
        {
            return _context.Agents.Any(e => e.Id == id);
        }
    }
}
