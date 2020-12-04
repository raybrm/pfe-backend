using BlockCovid.Models;
using Microsoft.EntityFrameworkCore;

namespace BlockCovid.Dal
{
    public class BlockCovidContext : DbContext
    {
        public BlockCovidContext(DbContextOptions<BlockCovidContext> options) : base (options) { }

        public DbSet<Citizen> Citizens { get; set; }
        public DbSet<Participant> Participants { get; set; }
        public DbSet<QrCode> QrCode { get; set; }
        public DbSet<CitizenQrCode> CitizenQrCode { get; set; }
    }
}
