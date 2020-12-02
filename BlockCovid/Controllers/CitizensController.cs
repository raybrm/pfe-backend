using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BlockCovid.Models;

namespace BlockCovid.Controllers
{
    [Produces("application/json")] // ce que le controler va renvoyer
    [Route("api/[controller]")]
    [ApiController]
    public class CitizensController : ControllerBase
    {
        private readonly BlockCovidContext _context;

        public CitizensController(BlockCovidContext context)
        {
            _context = context;
        }

        // GET: api/Citizens
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Citizen>>> GetCitizens()
        {
            return await _context.Citizens.ToListAsync();
        }

        // GET: api/Citizens/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Citizen>> GetCitizen(long id)
        {
            var citizen = await _context.Citizens.FindAsync(id);

            if (citizen == null)
            {
                return NotFound();
            }

            return citizen;
        }

        // PUT: api/Citizens/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCitizen(long id, Citizen citizen)
        {
            if (id != citizen.CitizenID)
            {
                return BadRequest();
            }

            _context.Entry(citizen).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CitizenExists(id))
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

        // POST: api/Citizens
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Citizen>> PostCitizen(Citizen citizen)
        {
            _context.Citizens.Add(citizen);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCitizen", new { id = citizen.CitizenID }, citizen);
        }

        // DELETE: api/Citizens/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCitizen(long id)
        {
            var citizen = await _context.Citizens.FindAsync(id);
            if (citizen == null)
            {
                return NotFound();
            }

            _context.Citizens.Remove(citizen);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CitizenExists(long id)
        {
            return _context.Citizens.Any(e => e.CitizenID == id);
        }
    }
}
