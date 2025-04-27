using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ContestantService.Data;
using ContestantService.Models;
using ContestantService.Services;

namespace ContestantService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContestantsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ContestantProducer _kafkaProducer;

        public ContestantsController(ApplicationDbContext context, ContestantProducer kafkaProducer)
        {
            _context = context;
            _kafkaProducer = kafkaProducer;
        }

        // POST: api/Contestants
        [HttpPost]
        public async Task<IActionResult> PostContestant(Contestant contestant)
        {
            _context.Contestants.Add(contestant);
            await _context.SaveChangesAsync();

            // Publish contestant data to Kafka
            await _kafkaProducer.ProduceContestantAsync(contestant);

            return CreatedAtAction(nameof(GetContestant), new { id = contestant.Id }, contestant);
        }

        // GET: api/Contestants
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Contestant>>> GetContestants()
        {
            return await _context.Contestants.ToListAsync();
        }

        // GET: api/Contestants/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Contestant>> GetContestant(int id)
        {
            var contestant = await _context.Contestants.FindAsync(id);

            if (contestant == null)
            {
                return NotFound();
            }

            return contestant;
        }
    }
}
