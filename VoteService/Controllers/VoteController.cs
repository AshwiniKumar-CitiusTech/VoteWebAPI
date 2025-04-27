using Microsoft.AspNetCore.Mvc;
using VoteService.Data;
using VoteService.Models;
using VoteService.Services;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;




namespace VoteService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VoteController : ControllerBase
    {
        private readonly VoteDbContext _context;
        private readonly VoteProducer _producer;

        public VoteController(VoteDbContext context, VoteProducer producer)
        {
            _context = context;
            _producer = producer;
        }

        [HttpPost]
        public async Task<IActionResult> PostVote([FromBody] Vote vote)
        {
            _context.Votes.Add(vote);
            await _context.SaveChangesAsync();

            await _producer.SendVoteAsync(vote);

            return Ok(new { message = "Vote cast successfully", vote });
        }
    }
}
