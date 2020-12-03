﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BlockCovid.Dal;
using BlockCovid.Models;

namespace BlockCovid.Controllers
{
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
        [HttpGet]
        public async Task<ActionResult<IEnumerable<QrCode>>> GetQrCode()
        {
            return await _context.QrCode.ToListAsync();
        }

        // GET: api/QrCodes/login
        [HttpGet("{login}")]
        public async Task<ActionResult<IEnumerable<QrCode>>> GetQrCode(string login)
        {
            // recupère le login dans le token, ne pas avoir donc le login en paramètre
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

        // PUT: api/QrCodes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutQrCode(long id, QrCode qrCode)
        {
            if (id != qrCode.QrCodeID)
            {
                return BadRequest();
            }

            _context.Entry(qrCode).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!QrCodeExists(id))
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

        private bool QrCodeExists(long id)
        {
            return _context.QrCode.Any(e => e.QrCodeID == id);
        }
    }
}
