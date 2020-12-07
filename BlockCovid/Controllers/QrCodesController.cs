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

namespace BlockCovid.Controllers
{
    [EnableCors("_myAllowSpecificOrigins")]
    [Route("api/[controller]")]
    [ApiController]
    public class QrCodesController : ControllerBase
    {
        private readonly BlockCovidContext _context;

        public QrCodesController(BlockCovidContext context)
        {
            _context = context;
        }



        // GET: api/QrCodes
        //[Authorize(Roles = "Doctor")]  // valide token
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<QrCode>>> GetQrCode()
        {
            //login dans le claims du token
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var login = identity.FindFirst("login").Value;

            return await _context.QrCode.Where(qr => qr.Participant.Login == login).ToListAsync();
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
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<QrCode>> PostQrCode(QrCode qrCode)
        {
            _context.QrCode.Add(qrCode);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetQrCode", new { id = qrCode.QrCodeID }, qrCode);
        }

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

        private bool QrCodeExists(string id)
        {
            return _context.QrCode.Any(e => e.QrCodeID == id);
        }
    }
}
