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


        /// <summary>
        /// Retourne un citizen specifique à partir de son id
        /// </summary>
        /// <response code="200">Returns the citizen</response>
        /// <response code="404">Si le citizen est pas trouvé</response> 
        // GET: api/Citizens/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CitizenDto>> GetCitizen(long id)
        {
            
           var citizen = await _citizen.GetCitizenByIdAsync(id);

            if (citizen == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<CitizenDto>(citizen));
        }
        /// <summary>
        /// Permet de mettre à jour un citizen
        /// </summary>
        /// <response code="201">Returns the updated citizen</response>
        /// <response code="404">Si le citizen est pas trouvé</response>  
        [HttpPut]
        public async Task<ActionResult<CitizenDto>> UpdateCitizen(CitizenDto citizenDto)
        {
            
            CitizenDto cDto = await _citizen.UpdateCitizen(citizenDto);

            if (cDto == null)
            {
                return NotFound();
            }
            return Ok(cDto);

        }

        /// <summary>
        /// Permet de se connecter en lui donnant un login et mdp
        /// </summary>
        /// <response code="200">Returns the newly created citizen </response>
        // POST: api/Citizens
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("login")]
        public async Task<ActionResult<CitizenDto>> LoginCitizen(CitizenDto citizenDto)
        {
            CitizenDto cDto= await _citizen.IfCitizenInDbAsync(citizenDto);
            if (cDto == null)
            {
                return NotFound();
            }
            return cDto;

        }

        /// <summary>
        /// Permet de créer un citizen
        /// </summary>
        /// <response code="201">Returns the newly created citizen </response>
        /// <response code="400">Si citizen est null</response>  
        [HttpPost("register")]
        public async Task<ActionResult<CitizenDto>> RegisterCitizen(CitizenDto citizenDto)
        {
            CitizenDto cDto = await _citizen.IfCitizenInDbAsync(citizenDto);
            if (cDto == null)
            {
                var citizen = _mapper.Map<Citizen>(citizenDto);

                try
                {
                    var citizenToReturn = await _citizen.CreateCitizensAsync(citizen);
                    return CreatedAtAction("GetCitizen", new { id = citizen.CitizenID }, _mapper.Map<CitizenDto>(citizenToReturn));
                }
                catch (DbUpdateException)
                {
                    return Conflict(new { message = "The citizen already exist" });
                }
               
               
            }

            return cDto;

        }


    }
}
