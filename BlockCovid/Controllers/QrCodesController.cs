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
using System.Data.Common;

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


        /// <summary>
        /// Permet de recupérer la liste des QrCodes de l'utilisateur ayant le role "Establishment"
        /// </summary>
        /// <response code="200">La liste à bien été envoyé même si elle est vide</response>
        /// <response code="401">Si l'utilisateur n'est pas autorisé à appeler la méthode</response>  
        // GET: api/QrCodes
        [Authorize(Roles = "Establishment")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<QrCodeDto>>> GetQrCode()
        {
            //login dans le claims du token
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            string login = identity.FindFirst("login").Value;
           
            return await _qrCodesRepository.GetQrCodesByLoginAsync(login);
        }


        /// <summary>
        /// Permet d'enregistrer un QrCode
        /// </summary> 
        /// <response code="201">Qrcode enregistré</response>
        /// <response code="400">Si le qrCode recu ne respecte pas le model de QrCodeDto ou le particpant dans le token ne se trouve pas en db</response> 
        /// <response code="401">Si l'utilisateur n'est pas autorisé à appeler la méthode</response>  
        // POST: api/QrCodes
        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<QrCodeDto>> PostQrCode(QrCodeDto qrCodeDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            // Get le login dans le token
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
            } catch(DbUpdateException)
            {
                return BadRequest(new {message = "The id already exist" });
            }

            return CreatedAtAction("GetQrCode", new { id = qrCode.QrCodeID }, qrCodeDto);
        }

        /// <summary>
        /// Scan qrCode et envoie une notif au personne ayant eu un contact avec la personne +
        /// </summary>
        /// <response code="200">Notif envoyée</response>
        /// <response code="400"> le QR code n'existe pas ou le citizen n'existe pas</response> 
        [HttpPost("scanQrCode")]
        public async Task<IActionResult> scanQrCode(ScanQrCodeDto scanQrCodeDto)
        {
          
           
            if (_qrCodesRepository.QrCodeExists(scanQrCodeDto.QrCode) == false )
            {
                return BadRequest(new { message = "qrCode doesn't exist" });
            }
            if (_qrCodesRepository.CitizenExists(scanQrCodeDto.citizen) == false)
            {
                return BadRequest(new { message = "Citizen doesn't exist" });
            }

            try {

                 await _qrCodesRepository.ScanQrCode(scanQrCodeDto);
                }
            catch (DbException Exception)
            {
                return BadRequest(new { message = "erreur interne" });
            }
            return  Ok(scanQrCodeDto); // TODO : à changer
        }
    }
}
