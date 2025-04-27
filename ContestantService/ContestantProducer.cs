using Confluent.Kafka;
using System.Text.Json;
using ContestantService.Models;

namespace ContestantService.Services
{
    public class ContestantProducer
    {
        private readonly string _bootstrapServers = "localhost:9092";  // Adjust as needed

        public async Task ProduceContestantAsync(Contestant contestant)
        {
            var config = new ProducerConfig { BootstrapServers = _bootstrapServers };

            using var producer = new ProducerBuilder<Null, string>(config).Build();
            var message = new Message<Null, string>
            {
                Value = JsonSerializer.Serialize(contestant)
            };

            await producer.ProduceAsync("contestant-topic", message);
        }
    }
}
