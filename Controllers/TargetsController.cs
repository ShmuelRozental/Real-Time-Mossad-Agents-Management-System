using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Real_Time_Mossad_Agents_Management_System.Data;
using Real_Time_Mossad_Agents_Management_System.Enums;
using Real_Time_Mossad_Agents_Management_System.Interface;
using Real_Time_Mossad_Agents_Management_System.Models;
using Real_Time_Mossad_Agents_Management_System.services;

namespace Real_Time_Mossad_Agents_Management_System.Controllers
{
    [Route("targets")]
    [ApiController]
    public class TargetsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IAgentTargetService _agentTargetService;

        public TargetsController(ApplicationDbContext context, IAgentTargetService agentTargetService)
        {
            _context = context;
            _agentTargetService = agentTargetService;
        }

        // GET: api/Targets
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Target>>> GetTargets()
        {
            return await _context.Targets.ToListAsync();
        }

        // GET: api/Targets/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Target>> GetTarget(int id)
        {
            var target = await _context.Targets.FindAsync(id);

            if (target == null)
            {
                return NotFound();
            }

            return target;
        }

        // PUT: api/Targets/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTarget(int id, Target target)
        {
            if (id != target.Id)
            {
                return BadRequest();
            }

            _context.Entry(target).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TargetExists(id))
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

        // POST: api/Targets
        [HttpPost]
        public async Task<ActionResult<int>> PostTarget(Target target)
        {
            _context.Targets.Add(target);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTarget", new { id = target.Id }, target.Id);
        }

        // DELETE: api/Targets/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTarget(int id)
        {
            var target = await _context.Targets.FindAsync(id);
            if (target == null)
            {
                return NotFound();
            }

            _context.Targets.Remove(target);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        // PUT: api/5/pin
        [HttpPut("{id}/pin")]
        public IActionResult SetInitialPosition(int id, [FromBody] PinLocation location)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var target = _context.Targets.Find(id);
            if (target == null)
            {
                return NotFound();
            }

            // Update location
            if (target.Location == null)
            {
                target.Location = new PinLocation();
            }
            target.Location.X = location.X;
            target.Location.Y = location.Y;

            try
            {
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                // Log exception
                return StatusCode(500, "Internal server error.");
            }

            return Ok(target);
        }

        // PUT: api/5/move
        [HttpPut("{id}/move")]
        public IActionResult MoveTarget(int id, [FromBody] string directionString)
        {
            var target = _context.Targets.Find(id);
            if (target == null)
            {
                return NotFound();
            }

            if (!Enum.TryParse<Direction>(directionString, true, out var direction))
            {
                return BadRequest("Invalid direction value.");
            }

            // Update location
            if (target.Location == null)
            {
                target.Location = new PinLocation();
            }

            switch (direction)
            {
                case Direction.NW:
                    target.Location.X -= 1;
                    target.Location.Y += 1;
                    break;
                case Direction.N:
                    target.Location.Y += 1;
                    break;
                case Direction.NE:
                    target.Location.X += 1;
                    target.Location.Y += 1;
                    break;
                case Direction.W:
                    target.Location.X -= 1;
                    break;
                case Direction.E:
                    target.Location.X += 1;
                    break;
                case Direction.SW:
                    target.Location.X -= 1;
                    target.Location.Y -= 1;
                    break;
                case Direction.S:
                    target.Location.Y -= 1;
                    break;
                case Direction.SE:
                    target.Location.X += 1;
                    target.Location.Y -= 1;
                    break;
            }

            try
            {
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                // Log exception
                return StatusCode(500, "Internal server error.");
            }

            _agentTargetService.CheckAndUpdateAssignments(target);
            return Ok(target);
        }

        private bool TargetExists(int id)
        {
            return _context.Targets.Any(e => e.Id == id);
        }
    }
}
