using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using BlockCovid.Models;

namespace BlockCovid.Dal
{
    public static class SeedData
    {
        public static void EnsurePopulated(IApplicationBuilder app)
        {
            BlockCovidContext context = app.ApplicationServices
            .CreateScope().ServiceProvider.GetRequiredService<BlockCovidContext>();
            /*Applique une migration si la base de données n'est pas à jour*/
            
            if (context.Database.GetPendingMigrations().Any())
            {
                context.Database.Migrate();
            }

             if (!context.Citizens.Any())
             {
                 context.Citizens.AddRange(
                     new Citizen
                     {
                         Is_Positive = false
                     },
                     new Citizen
                     {
                         Is_Positive = false
                     });
                 context.SaveChanges();
             }
             

            if (!context.Participants.Any())
            {
                context.Participants.AddRange(
                    new Participant
                    {
                        Login = "user1@example.com",
                        Password = "$2a$11$4IFsV0menn9l.bTiRISW7ulQU/yeGCtAW1t7FPpcxceQrLHCkxYzO",
                        Participant_Type = ParticipantType.Doctor
                    },
                    new Participant
                    {
                        Login = "user2@example.com",
                        Password = "$$2a$11$nPtjqRikfgHe26J/kbLgPeFNoUfzcAhwhUqvM90xZRMDJ06kyyShi",
                        Participant_Type = ParticipantType.Establishment
                    });
                context.SaveChanges();  
            }


            if (!context.Citizens.Any())
            {
                context.Citizens.AddRange(
                    new Citizen
                    {
                        Is_Positive = false,
                        TokenFireBase= "fuoa6jCf0DbvfWXZUMxOwr:APA91bG3xTYltMW5bc3z2qZAPsk3qMsH64rajb4nG9JL0tyZScr5buLx0Uu7zaIpp3txWiZS2lq_SB6KUg6zkma5-j425AfijlYhbNsSEcCFaZr3_u7UuEauRY2XSr23LzpBu5D1LzKr"
                    }
                   
                   ) ;
                context.SaveChanges();
            }
            
        }
    }
}
