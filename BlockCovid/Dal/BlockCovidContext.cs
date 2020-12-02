using BlockCovid.Models;
using Microsoft.EntityFrameworkCore;

namespace BlockCovid.Dal
{
    public class BlockCovidContext : DbContext
    {
        public BlockCovidContext(DbContextOptions<BlockCovidContext> options) : base (options) { }

        public DbSet<Citizen> Citizens { get; set; }
    }
}
