using Microsoft.EntityFrameworkCore;
using ContestantService.Models;

namespace ContestantService.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        { }

        public DbSet<Contestant> Contestants { get; set; }
    }
}
