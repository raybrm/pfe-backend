using Microsoft.EntityFrameworkCore;

namespace BlockCovid.Models
{
    public class BlockCovidContext : DbContext
    {
        public BlockCovidContext(DbContextOptions<BlockCovidContext> options) : base (options) { }

        public DbSet<Citizen> Citizens { get; set; }
    }
}
