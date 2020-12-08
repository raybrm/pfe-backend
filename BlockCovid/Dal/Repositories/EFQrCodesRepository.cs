﻿using AutoMapper;
using BlockCovid.ConfigurationSettings;
using BlockCovid.Interfaces;
using BlockCovid.Models;
using BlockCovid.Models.Dto;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using System.Net;
using System.IO;
using System.Text.Json;

namespace BlockCovid.Dal.Repositories
{
    public class EFQrCodesRepository : IQrCodesRepository
    {

        private readonly BlockCovidContext _context;
        private readonly IMapper _mapper;
        private IOptions<FireBaseSettings> _fireBaseSettings;
        public EFQrCodesRepository(BlockCovidContext context, IMapper mapper, IOptions<FireBaseSettings> fireBaseSettings)
        {
            _context = context;
            _fireBaseSettings = fireBaseSettings;
            _mapper = mapper;
        }
        public async Task<QrCode> CreateQrCodeAsync(QrCode qrCode)
        {
            _context.QrCode.Add(qrCode);
            await _context.SaveChangesAsync();
            return qrCode;
        }

        public async Task<List<QrCodeDto>> GetQrCodesByLoginAsync(string login)
        {
           return await _context.QrCode.Where(qr => qr.Participant.Login == login)
                                        .Select(q => _mapper.Map<QrCodeDto>(q))
                                        .ToListAsync();
        }
        public bool QrCodeExists(String id)
        {
            return _context.QrCode.Any(e => e.QrCodeID == id);
        }

        public async Task ScanQrCode(ScanQrCodeDto scanQrCodeDto)
        {
        
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                if (QrCodeExists(scanQrCodeDto.QrCode))
                {
                    QrCode qrCode = await _context.QrCode.Include(qr => qr.Participant).FirstOrDefaultAsync(x => x.QrCodeID == (scanQrCodeDto.QrCode));

                    ParticipantType participantType = qrCode.Participant.Participant_Type;

                    switch (participantType)
                    {
                        case ParticipantType.Doctor:

                            await DeleteQrCode(scanQrCodeDto);
                            await UpdateToPositive(scanQrCodeDto);
                            await ToNotify(scanQrCodeDto.citizen);

                            break;

                        case ParticipantType.Establishment:

                            await InsertCitizenQrCode(scanQrCodeDto);

                            break;

                        default:
                            Console.WriteLine("Default case");
                            break;
                    }

                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();
                }
            }
            catch (Exception)
            {
           
                await transaction.RollbackToSavepointAsync("Erreur en db, rollback réalisé");

            }
   
        }

        private async Task UpdateToPositive(ScanQrCodeDto scanQrCodeDto)
        {
   
            Citizen  citizen=(from Citizen c in _context.Citizens
                where c.CitizenID==scanQrCodeDto.citizen
                select c).SingleOrDefault();
                  citizen.Is_Positive = true;
                  await _context.SaveChangesAsync();
        }
        private async Task InsertCitizenQrCode(ScanQrCodeDto scanQrCodeDto)
        {
            CitizenQrCodeDto citizenQrCodeDtoToInsert = new CitizenQrCodeDto
            {
                QrCodeId = scanQrCodeDto.QrCode,
                Timestamp = DateTime.Now,
                CitizenId = scanQrCodeDto.citizen,

            };
            CitizenQrCode citizenQrCode = _mapper.Map<CitizenQrCode>(citizenQrCodeDtoToInsert);

            _context.CitizenQrCode.Add(citizenQrCode);
            await _context.SaveChangesAsync();
        }
        private async Task DeleteQrCode(ScanQrCodeDto scanQrCodeDto)
        {
            QrCode qrCode = await _context.QrCode.FindAsync(scanQrCodeDto.QrCode);
          
          _context.QrCode.Remove(qrCode);
           await _context.SaveChangesAsync();

        }

        private async Task ToNotify(long id)
        {

            HashSet < CitizenDto > ensembleCitizenDto= new HashSet<CitizenDto>(new ComparateurCitizens());

            IQueryable<CitizenQrCodeDto> listCustomer = await Task.Run(() => from CitizenQrCode c in _context.CitizenQrCode
                                                                             where c.CitizenId == id
                                                                             select _mapper.Map<CitizenQrCodeDto>(c));

            await Task.Run(async () =>
               await listCustomer.ForEachAsync(action: citizenQrCode =>
                {
     
                    DateTime datePlusUneHeure = citizenQrCode.Timestamp.AddHours(1);
                    IQueryable<CitizenDto> listCitizenDtoToNotify = (from CitizenQrCode citizenQr in _context.CitizenQrCode.Include(ct => ct.Citizen)

                                                                    where citizenQr.QrCodeId == citizenQrCode.QrCodeId
                                                                    && citizenQrCode.Timestamp.Subtract(citizenQr.Timestamp).Days<=10 && citizenQrCode.Timestamp<= citizenQr.Timestamp && citizenQr.Timestamp <= datePlusUneHeure
                                                                    && citizenQr.Citizen.Is_Positive==false
                                                                    select _mapper.Map<CitizenDto>(citizenQr.Citizen)).Distinct();

                    foreach (CitizenDto c in listCitizenDtoToNotify)
                    {
                        if (!ensembleCitizenDto.Contains(c))
                        {
                            NotifyFilters(c.TokenFireBase);
                            ensembleCitizenDto.Add(c);
                        }
                    }
               
                }));
          
        }

        public void NotifyFilters(string token)
        {
           
            try
            {
                dynamic data = new
                {
                    to = token, // Uncoment this if you want to test for single device
                                // registration_ids = singlebatch, // this is for multiple user 
                    notification = new
                    {
                        title = "COVID-19",     // Notification title
                        body = "allez vous faire diagnostiquer, vous êtes peut-être contaminer. ",    // Notification body data
                        link = "https://www.youtube.com/watch?v=z6-FWJteNLI"       // When click on notification user redirect to this link
                    }
                };


                var json = JsonSerializer.Serialize(data);

                byte[] byteArray = Encoding.UTF8.GetBytes(json);

                string SERVER_API_KEY = _fireBaseSettings.Value.ServerFireBase;


                WebRequest tRequest;
                tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
                tRequest.Method = "post";
                tRequest.ContentType = "application/json";
                tRequest.Headers.Add(string.Format("Authorization: key={0}", SERVER_API_KEY));

                tRequest.ContentLength = byteArray.Length;
                Stream dataStream = tRequest.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();

                WebResponse tResponse = tRequest.GetResponse();

                dataStream = tResponse.GetResponseStream();

                StreamReader tReader = new StreamReader(dataStream);

                string sResponseFromServer = tReader.ReadToEnd();

                tReader.Close();
                dataStream.Close();
                tResponse.Close();
            }
            catch (Exception)
            {
                throw;
            }

        }

       
    }
}
