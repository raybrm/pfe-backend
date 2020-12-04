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
            /*
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
            */
        }
    }
}
