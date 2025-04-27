using Confluent.Kafka;
using System.Text.Json;
using VoteService.Models;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;  // Required for IConfiguration




namespace VoteService.Services
{
    public class VoteProducer
    {
        private readonly IConfiguration _config;

        public VoteProducer(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendVoteAsync(Vote vote)
        {
            var kafkaConfig = new ProducerConfig
            {
                BootstrapServers = _config["Kafka:BootstrapServers"]
            };

            using var producer = new ProducerBuilder<Null, string>(kafkaConfig).Build();
            string voteJson = JsonSerializer.Serialize(vote);

            await producer.ProduceAsync("votes-topic", new Message<Null, string> { Value = voteJson });
        }
    }
}
