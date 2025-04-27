using System;

namespace VoteService.Models
{
    public class Vote
    {
        public int Id { get; set; }
        public int ContestantId { get; set; }
        public DateTime VotedAt { get; set; } = DateTime.UtcNow;
    }
}
