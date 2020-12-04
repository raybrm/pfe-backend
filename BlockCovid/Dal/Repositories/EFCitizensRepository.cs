﻿using BlockCovid.Interfaces;
using BlockCovid.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BlockCovid.Models.Dto;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Text;
using System.Net;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Runtime.Serialization.Json;
using BlockCovid.Services;

namespace BlockCovid.Dal.Repositories
{
    public class EFCitizensRepository : ICitizensRepository
    {
        private readonly BlockCovidContext _context;
        private readonly IMapper _mapper;
        public EFCitizensRepository(BlockCovidContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }


        public async Task<List<Citizen>> GetCitizensAsync()
        {
            return await _context.Citizens.ToListAsync();
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



        public void ToNotify(long id)
        {


            /*  IQueryable<CitizenQrCodeDto> listCustomer = from CitizenQrCode c in _context.CitizenQrCode
                                                          where c.CitizenId==1
                                                          select _mapper.Map<CitizenQrCodeDto>(c) ;

              foreach(CitizenQrCodeDto citizenQrCode in listCustomer)
              {

                  System.Diagnostics.Debug.WriteLine(citizenQrCode.Timestamp+" "+citizenQrCode.CitizenQrCodeId);

                  DateTime datePlusUneHeure = citizenQrCode.Timestamp.AddHours(1);
                  DateTime dateMoinsUneHeure = citizenQrCode.Timestamp.AddHours(-1);

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

                //new System.Web.Script.Serialization.JavaScriptSerializer();
                var json = JsonSerializer.Serialize(data);

                Byte[] byteArray = Encoding.UTF8.GetBytes(json);

                string SERVER_API_KEY = "AAAAu8EAEYM:APA91bHQEPeJrX7C-CsPj3dtc-oHWeFB2iTJSqix3CPVh1AiI_Iu_WcxLC9Gl4nkV1M2Im1qTRPaHA8NTiSoKokimJwP_apg0Kp6JU7MXwDXAp_5ENXu6yN_M14t5cR1JeL2tGDscawP";//Configuration["Firebase:SERVER_FIREBASE"];


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

                String sResponseFromServer = tReader.ReadToEnd();

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

    
}
