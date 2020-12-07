using BlockCovid.Interfaces;
using BlockCovid.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BlockCovid.Models.Dto;
using System.Text;
using System.Net;
using System.IO;
using System.Text.Json;
using BlockCovid.ConfigurationSettings;
using Microsoft.Extensions.Options;

namespace BlockCovid.Dal.Repositories
{
    public class EFCitizensRepository : ICitizensRepository
    {
        private readonly BlockCovidContext _context;
        private readonly IMapper _mapper;
        private IOptions<FireBaseSettings> _fireBaseSettings;
        public EFCitizensRepository(BlockCovidContext context, IMapper mapper, IOptions<FireBaseSettings> fireBaseSettings)
        {
            _context = context;
            _fireBaseSettings = fireBaseSettings;
            _mapper = mapper;
        }

        public async Task<Citizen> GetCitizenByIdAsync(long id)
        {
            return await _context.Citizens.FindAsync(id);
        }

        public async Task<Citizen> CreateCitizensAsync(Citizen citizen)
        {
           
            _context.Citizens.Add(citizen);
            await _context.SaveChangesAsync();
            return citizen;
        }


        public bool CitizenExists(long id)
        {
            return _context.Citizens.Any(e => e.CitizenID == id);
        }


        public async Task ToNotify(long id)
        {


             IQueryable<CitizenQrCodeDto> listCustomer = await Task.Run(() =>from CitizenQrCode c in _context.CitizenQrCode
                                                          where c.CitizenId==id                                                      
                                                          select _mapper.Map<CitizenQrCodeDto>(c)) ;
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

        public Task<CitizenDto> IfCitizenInDbAsync(CitizenDto citizenDto)
        {
            CitizenDto cDto = (from Citizen c in _context.Citizens
                                                        where c.TokenFireBase == citizenDto.TokenFireBase
                                     select _mapper.Map<CitizenDto>(c)).FirstOrDefault();
            if( cDto== null)
            {
                
                return null;
            }
           
            return Task.FromResult(cDto);
        }
    }
}
