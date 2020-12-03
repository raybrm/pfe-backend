using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BlockCovid.Models;
using BlockCovid.Interfaces;
using Microsoft.AspNetCore.Cors;

namespace BlockCovid.Controllers
{
    [EnableCors("MyAllowSpecificOrigins")]
    [Produces("application/json")] // ce que le controler va renvoyer
    [Route("api/[controller]")]
    [ApiController]
    public class CitizensController : ControllerBase
    {
        private readonly ICitizensRepository _citizen;

        public CitizensController(ICitizensRepository citizen)
        {
            _citizen = citizen;
        }

        // GET: api/Citizens
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Citizen>>> GetCitizens()
        {
            return await _citizen.GetCitizensAsync();
        }

        // GET: api/Citizens/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Citizen>> GetCitizen(long id)
        {
            var citizen = await _citizen.GetCitizenByIdAsync(id);

            if (citizen == null)
            {
                return NotFound();
            }

            return citizen;
        }

        // POST: api/Citizens
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Citizen>> PostCitizen(Citizen citizen)
        {
            var citizenToReturn = await _citizen.CreateCitizensAsync(citizen);

            return CreatedAtAction("GetCitizen", new { id = citizen.CitizenID }, citizenToReturn);
        }

     
    }
}
