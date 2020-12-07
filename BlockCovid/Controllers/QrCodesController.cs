using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BlockCovid.Dal;
using BlockCovid.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Security.Claims;
using BlockCovid.Models.Dto;
using AutoMapper;

namespace BlockCovid.Controllers
{
    [EnableCors("_myAllowSpecificOrigins")]
    [Route("api/[controller]")]
    [ApiController]
    public class QrCodesController : ControllerBase
    {
        private readonly BlockCovidContext _context;
        private readonly IMapper _mapper;

        public QrCodesController(BlockCovidContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }



        // GET: api/QrCodes
        [Authorize(Roles = "Establishment")]  // valide token
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<QrCodeDto>>> GetQrCode()
        {
            //login dans le claims du token
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var login = identity.FindFirst("login").Value;

            return await _context.QrCode.Where(qr => qr.Participant.Login == login)
                                        .Select(q => _mapper.Map<QrCodeDto>(q))
                                        .ToListAsync();
        }


        /*
        // GET: api/QrCodes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<QrCode>> GetQrCode(long id)
        {
            var qrCode = await _context.QrCode.FindAsync(id);

            if (qrCode == null)
            {
                return NotFound();
            }

            return qrCode;
        }
        */


        // POST: api/QrCodes
        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<QrCodeDto>> PostQrCode(QrCodeDto qrCodeDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var login = identity.FindFirst("login").Value;

            // Requete db pour récupere l'id du login
            var participant = await _context.Participants.Where(qr => qr.Login == login).FirstOrDefaultAsync();

            if (participant == null)
            {
                return BadRequest(new { message = "No participant" });
            }

            QrCode qrCode = _mapper.Map<QrCode>(qrCodeDto);
            qrCode.ParticipantID = participant.ParticipantID;

            try
            {
                _context.QrCode.Add(qrCode);
                await _context.SaveChangesAsync();
            } catch(DbUpdateException exception)
            {
                return BadRequest(new {message = "The id already exist" });
            }

            return CreatedAtAction("GetQrCode", new { id = qrCode.QrCodeID }, qrCodeDto);
        }

        /*
        // DELETE: api/QrCodes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteQrCode(long id)
        {
            var qrCode = await _context.QrCode.FindAsync(id);
            if (qrCode == null)
            {
                return NotFound();
            }

            _context.QrCode.Remove(qrCode);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        */
        private bool QrCodeExists(string id)
        {
            return _context.QrCode.Any(e => e.QrCodeID == id);
        }

        [HttpPost("scanQrCode")]
        public async Task<IActionResult> scanQrCode(ScanQrCodeDto scanQrCodeDto)
        {
            return Ok();
        }
    }
}
