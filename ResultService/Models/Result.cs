using System.ComponentModel.DataAnnotations;

namespace ResultService.Models
{
    public class Result
    {
        [Key]
        public int Id { get; set; } // ✅ Primary key

        public int ContestantId { get; set; }
        public int Votes { get; set; }
    }
}

