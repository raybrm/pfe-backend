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
using BlockCovid.Interfaces;
using AutoMapper;

namespace BlockCovid.Controllers
{
    [EnableCors("_myAllowSpecificOrigins")]
    [Route("api/[controller]")]
    [ApiController]
    public class QrCodesController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IQrCodesRepository _qrCodesRepository;
        private readonly IParticipantsRepository _participantRepository;

        public QrCodesController( IMapper mapper, IQrCodesRepository qrCodesRepository, IParticipantsRepository participant)
        {
            _mapper = mapper;
            _qrCodesRepository = qrCodesRepository;
            _participantRepository = participant;
        }



        // GET: api/QrCodes
        [Authorize(Roles = "Establishment")]  // valide token
        [HttpGet]
        public async Task<ActionResult<IEnumerable<QrCodeDto>>> GetQrCode()
        {
            //login dans le claims du token
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            string login = identity.FindFirst("login").Value;
           

            return await _qrCodesRepository.GetQrCodesByLoginAsync(login);
        }


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
            string login = identity.FindFirst("login").Value;

            // Requete db pour récupere l'id du login
            var participant = await _participantRepository.GetParticipantByLoginAsync(login);

            if (participant == null)
            {
                return BadRequest(new { message = "No participant" });
            }

            QrCode qrCode = _mapper.Map<QrCode>(qrCodeDto);
            qrCode.ParticipantID = participant.ParticipantID;

            try
            {
                await _qrCodesRepository.CreateQrCodeAsync(qrCode);
            } catch(DbUpdateException exception)
            {
                return BadRequest(new {message = "The id already exist" });
            }

            return CreatedAtAction("GetQrCode", new { id = qrCode.QrCodeID }, qrCodeDto);
        }

        [HttpPost("scanQrCode")]
        public async Task<IActionResult> scanQrCode(ScanQrCodeDto scanQrCodeDto)
        {
            await _qrCodesRepository.ScanQrCode(scanQrCodeDto);
            return  Ok();
        }
    }
}
