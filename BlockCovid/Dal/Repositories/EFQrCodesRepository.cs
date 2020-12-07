using AutoMapper;
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
        public Task<QrCode> CreateQrCodeAsync(QrCode citizen)
        {
            throw new NotImplementedException();
        }

        public Task<QrCode> GetQrCodeByIdAsync(long id)
        {
            throw new NotImplementedException();
        }

        public Task<List<QrCode>> GetQrCodesAsync()
        {
            throw new NotImplementedException();
        }

        public async Task ScanQrCode(ScanQrCodeDto scanQrCodeDto)
        {
        
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                ParticipantType particiapantType = (await _context.QrCode.FindAsync(scanQrCodeDto.QrCode)).Participant.Participant_Type;

                switch (particiapantType)
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
            catch (Exception)
            {
           
                await transaction.RollbackToSavepointAsync("");

           
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
            var qrCode = await _context.QrCode.FindAsync(scanQrCodeDto.QrCode);
            _context.QrCode.Remove(qrCode);
            await _context.SaveChangesAsync();

     
        }

        private async Task ToNotify(long id)
        {


            IQueryable<CitizenQrCodeDto> listCustomer = await Task.Run(() => from CitizenQrCode c in _context.CitizenQrCode
                                                                             where c.CitizenId == id
                                                                             select _mapper.Map<CitizenQrCodeDto>(c));
            await listCustomer.ForEachAsync(async citizenQrCode =>
            {
                DateTime datePlusUneHeure = citizenQrCode.Timestamp.AddHours(1);

                //   c.QrCodeId == citizenQrCode.QrCodeId
                IQueryable<CitizenDto> listCitizenDtoToNotify = await Task.Run(() => from CitizenQrCode citizenQr in _context.CitizenQrCode
                                                                                     from Citizen citizen in _context.CitizenQrCode
                                                                                     where citizenQr.QrCodeId == citizenQrCode.QrCodeId
                                                                                     && citizenQr.Timestamp <= datePlusUneHeure
                                                                                     //&& citizen.Is_Positive==false
                                                                                     select _mapper.Map<CitizenDto>(citizen));
                await listCitizenDtoToNotify.ForEachAsync(async c =>
                {

                    await Task.Run(() => NotifyFilters(c.TokenFireBase));

                });

            });
            /* foreach (CitizenQrCodeDto citizenQrCode in listCustomer)
             {

                 System.Diagnostics.Debug.WriteLine(citizenQrCode.Timestamp+" "+citizenQrCode.CitizenQrCodeId);

                 DateTime datePlusUneHeure = citizenQrCode.Timestamp.AddHours(1);

            //   c.QrCodeId == citizenQrCode.QrCodeId
                 IQueryable<CitizenDto> listCitizenDtoToNotify = from CitizenQrCode citizenQr in _context.CitizenQrCode
                                                                       from Citizen citizen in _context.CitizenQrCode
                                                                       where citizenQr.QrCodeId==citizenQrCode.QrCodeId
                                                                       && citizenQr.Timestamp<=datePlusUneHeure
                                                                       //&& citizen.Is_Positive==false
                                                                       select _mapper.Map<CitizenDto>(citizen);
               foreach(CitizenDto citizenToNotify in listCitizenDtoToNotify)
               {
                   NotifyFilters(citizenToNotify.TokenFireBase);
               }

           }*/
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
                        title = "Positiv",     // Notification title
                        body = "pas bien ca ",    // Notification body data
                        link = "--link--"       // When click on notification user redirect to this link
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
