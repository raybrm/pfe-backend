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
using BlockCovid.Models.Dto;
using AutoMapper;

namespace BlockCovid.Controllers
{
    [EnableCors("_myAllowSpecificOrigins")]
    [Produces("application/json")] // ce que le controler va renvoyer
    [Route("api/[controller]")]
    [ApiController]
    public class CitizensController : ControllerBase
    {
        private readonly ICitizensRepository _citizen;
        private readonly IMapper _mapper;
        public CitizensController(ICitizensRepository citizen, IMapper mapper)
        {
            
            _citizen = citizen;
            _mapper = mapper;
        }
       
        

        // GET: api/Citizens/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CitizenDto>> GetCitizen(long id)
        {
            _citizen.ToNotify(id);
           var citizen = await _citizen.GetCitizenByIdAsync(id);

            if (citizen == null)
            {
                return NotFound();
            }

            return _mapper.Map<CitizenDto>(citizen);
        }

        // POST: api/Citizens
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<CitizenDto>> PostCitizen(CitizenDto citizenDto)
        {
            CitizenDto cDto=_citizen.IfCitizenInDbAsync(citizenDto);
            if (cDto == null)
            {
                var citizen = _mapper.Map<Citizen>(citizenDto);


                System.Diagnostics.Debug.WriteLine("AAAAAAAAAAAAAAAAAA");
                var citizenToReturn = await _citizen.CreateCitizensAsync(citizen);
                return CreatedAtAction("GetCitizen", new { id = citizen.CitizenID }, _mapper.Map<CitizenDto>(citizenToReturn));
            }

            return cDto;
           

          
           /* System.Diagnostics.Debug.WriteLine("AAAAAAAAAAAAAAAAAA");
            var citizenToReturn = await _citizen.CreateCitizensAsync(citizen);
          
            return CreatedAtAction("GetCitizen", new { id = citizen.CitizenID }, _mapper.Map<CitizenDto>(citizenToReturn));*/
        }

     
    }
}
