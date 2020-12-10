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
        public async Task<QrCodeDto> CreateQrCodeAsync(QrCodeDto qrCode,long participantID)
        {
            QrCode qrCodeParticipant = _mapper.Map<QrCode>(qrCode);
            qrCodeParticipant.ParticipantID = participantID;
            _context.QrCode.Add(qrCodeParticipant);
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

        public bool CitizenExists(long id)
        {
            return _context.Citizens.Any(e => e.CitizenID == id);
        }

        public async Task ScanQrCode(ScanQrCodeDto scanQrCodeDto)
        {
        
            await using var transaction = await _context.Database.BeginTransactionAsync();
          
            try
            {
                    System.Diagnostics.Debug.WriteLine("Scan Qr Code");
                    QrCode qrCode = await _context.QrCode.Include(qr => qr.Participant).FirstOrDefaultAsync(x => x.QrCodeID == (scanQrCodeDto.QrCode));
                    
                    ParticipantType participantType = qrCode.Participant.Participant_Type;
                    
                    switch (participantType)
                    {
                        case ParticipantType.Doctor:
                            System.Diagnostics.Debug.WriteLine("Qr Code du doctor scanné");
                            await DeleteQrCode(scanQrCodeDto);
                            await UpdateToPositive(scanQrCodeDto);
                            await ToNotify(scanQrCodeDto.citizen);

                            break;

                        case ParticipantType.Establishment:
                            System.Diagnostics.Debug.WriteLine("Qr Code de l'etablisement scanné");
                            await InsertCitizenQrCode(scanQrCodeDto);
                            break;

                        default:
                            break;
                    }

                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();
              
            }
            catch (Exception exc)
            {
                await transaction.RollbackAsync();
                throw new Exception(exc.Message);
               
            }
        }

        private async Task UpdateToPositive(ScanQrCodeDto scanQrCodeDto)
        {
   
            Citizen  citizen = (from Citizen c in _context.Citizens
                                where c.CitizenID==scanQrCodeDto.citizen
                                select c).SingleOrDefault();

            citizen.Is_Positive = true;
            citizen.Is_Exposed = false;
            await _context.SaveChangesAsync();
        }
        private async Task InsertCitizenQrCode(ScanQrCodeDto scanQrCodeDto)
        {
            CitizenQrCode citizen = new CitizenQrCode
            {
                QrCodeId = scanQrCodeDto.QrCode,
                Timestamp = DateTime.Now,
                CitizenId = scanQrCodeDto.citizen,
            };          
            _context.CitizenQrCode.Add(citizen);
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

            HashSet <Citizen> ensembleCitizen= new HashSet<Citizen>(new ComparateurCitizens());

            IQueryable<CitizenQrCode> listCustomer = await Task.Run(() => from CitizenQrCode c in _context.CitizenQrCode
                                                                             where c.CitizenId == id
                                                                             select c);

            await Task.Run(async () =>
               await listCustomer.ForEachAsync(action: citizenQrCode =>
               {

                   int jourCompare = (DateTime.Now.Subtract(citizenQrCode.Timestamp).Days);

                   DateTime datePlusUneHeure = citizenQrCode.Timestamp.AddHours(1);
                   DateTime dateMoinsUneHeure = citizenQrCode.Timestamp.AddHours(-1);
                   IQueryable<Citizen> listCitizenDtoToNotify = (from CitizenQrCode citizenQr in _context.CitizenQrCode.Include(ct => ct.Citizen)

                                                                 where citizenQr.QrCodeId == citizenQrCode.QrCodeId
                                                                 && (jourCompare <= 10) //permet de voir si ils se sont croisés il y a plus de 10 jours
                                                                 && citizenQr.Timestamp <= datePlusUneHeure
                                                                 && citizenQr.Citizen.Is_Positive == false
                                                                 && citizenQr.Timestamp>= dateMoinsUneHeure
                                                                 select citizenQr.Citizen).Distinct();

                   System.Diagnostics.Debug.WriteLine("Parcours la liste des citizens à notifier");
                   System.Diagnostics.Debug.WriteLine(listCitizenDtoToNotify);
                   foreach (Citizen citizen in listCitizenDtoToNotify)
                   {
                       System.Diagnostics.Debug.WriteLine("Parcours le citizen :" + citizen.CitizenID);
                       if (!ensembleCitizen.Contains(citizen))
                       {
                           System.Diagnostics.Debug.WriteLine("Modification de citizen :" + citizen.CitizenID);
                           citizen.Is_Exposed = true;
                           NotifyFilters(citizen.TokenFireBase);
                           ensembleCitizen.Add(citizen);
                       }
                   }

               }));
          
        }

        private void NotifyFilters(string token)
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
                        link = "https://fr.wikipedia.org/wiki/Catastrophe_nucl%C3%A9aire_de_Tchernobyl"       // When click on notification user redirect to this link
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
                throw new Exception("notify exception");
            }

        }

       
    }
}
