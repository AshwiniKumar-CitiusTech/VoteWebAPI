using Microsoft.EntityFrameworkCore;
using ResultService.Models;

namespace ResultService.Models
{
    public class ResultDbContext : DbContext
    {
        public ResultDbContext(DbContextOptions<ResultDbContext> options) : base(options)
        {
        }

        public DbSet<Result> Results { get; set; }
    }
}
