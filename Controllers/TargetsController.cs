using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Real_Time_Mossad_Agents_Management_System.Data;
using Real_Time_Mossad_Agents_Management_System.Enums;
using Real_Time_Mossad_Agents_Management_System.Interfaces;
using Real_Time_Mossad_Agents_Management_System.Models;
using Real_Time_Mossad_Agents_Management_System.Services;

namespace Real_Time_Mossad_Agents_Management_System.Controllers
{

    [Route("[controller]")]
    [ApiController]
    public class TargetsController : ControllerBase
    {
        private readonly TargetsServices _targetstsServices;


        public TargetsController(TargetsServices targetsServices)
        {
            _targetstsServices = targetsServices;

        }

        // GET: Targets
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Target>>> GetTargets()
        {
            try
            {
                var target = await _targetstsServices.GetAllEntityAsync();
                return Ok(target);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }

        }


        // GET: Targets/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Target>> GetTarget(int id)
        {
            var target = await _targetstsServices.GetEntityAsync(id);

            if (target == null)
            {
                return NotFound();
            }

            return Ok(target);
        }

        // POST: Targets
        [HttpPost]
        public async Task<ActionResult<int>> PostTarget(Target target)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var createdTarget =await _targetstsServices.CreateEntity(target);
                return StatusCode(StatusCodes.Status201Created, new { Id = target.Id });

            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // PUT: 5/pin
        [HttpPut("{id}/pin")]
        public async Task<IActionResult> SetInitialPosition(int id, [FromBody] Location location)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var target = await _targetstsServices.SetEntityLocation(id, location);
                if (target == null)
                {
                    return NotFound("Target not found.");
                }

                return Ok(target);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // PUT: Targets/5/move
        [HttpPut("{id}/move")]
        public async Task<IActionResult> MoveTarget(int id, [FromBody] string directionString)
        {
            if (!Enum.TryParse<Direction>(directionString, true, out var direction))
            {
                return BadRequest("Invalid direction value.");
            }

            try
            {
                var target = await _targetstsServices.EntityMovement(id, direction);
                if (target == null)
                {
                    return NotFound();
                }

                return Ok(target);
            }
            catch (KeyNotFoundException)
            {
                return NotFound("This target does not exist.");
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
