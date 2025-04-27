using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ResultService.Models; // 🛠️ Add this for ResultDbContext
using System.Threading.Tasks;

namespace ResultService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ResultController : ControllerBase
    {
        private readonly ResultDbContext _dbContext;

        public ResultController(ResultDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetResults()
        {
            var results = await _dbContext.Results.ToListAsync();
            return Ok(results);
        }
    }
}

