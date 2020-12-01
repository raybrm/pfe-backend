using Microsoft.EntityFrameworkCore;

namespace BlockCovid.Models
{
    public class BlockCovidContext : DbContext
    {
        public DbSet<Citizen> Citizens { get; set; }
        public DbSet<Participant> Participants { get; set; }
    }
}
