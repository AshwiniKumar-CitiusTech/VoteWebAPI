using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using ResultService.Models;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Confluent.Kafka; // Required for Kafka consumer


namespace ResultService.Services
{
    public class KafkaConsumerService : BackgroundService
    {
        private readonly ILogger<KafkaConsumerService> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IConfiguration _configuration;

        public KafkaConsumerService(ILogger<KafkaConsumerService> logger, IServiceScopeFactory scopeFactory, IConfiguration configuration)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
            _configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Starting Kafka Consumer Service...");

            var config = new ConsumerConfig
            {
                BootstrapServers = _configuration["Kafka:BootstrapServers"],
                GroupId = "result-service-consumer-group",
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            try
            {
                using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();
                consumer.Subscribe(new[] { "votes-topic", "contestant-topic" });

                _logger.LogInformation("Subscribed to Kafka topics: votes-topic, contestant-topic");

                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        var consumeResult = consumer.Consume(stoppingToken);

                        _logger.LogInformation($"Message received from topic: {consumeResult.Topic}");

                        // Creating a new scope for ResultDbContext
                        using (var scope = _scopeFactory.CreateScope())
                        {
                            var dbContext = scope.ServiceProvider.GetRequiredService<ResultDbContext>();

                            if (consumeResult.Topic == "votes-topic")
                            {
                                _logger.LogInformation($"Processing Vote message from topic: {consumeResult.Topic}");
                                var vote = JsonSerializer.Deserialize<Result>(consumeResult.Message.Value);

                                if (vote != null)
                                {
                                    _logger.LogInformation($"Vote received for ContestantId={vote.ContestantId}");

                                    // Save or update the result in DB
                                    var result = await dbContext.Results
                                        .FirstOrDefaultAsync(r => r.ContestantId == vote.ContestantId, stoppingToken);

                                    if (result != null)
                                    {
                                        result.Votes++;
                                        dbContext.Results.Update(result);
                                        _logger.LogInformation($"Updated Vote Count - ContestantId: {vote.ContestantId}, Total Votes: {result.Votes}");
                                    }
                                    else
                                    {
                                        dbContext.Results.Add(new Result
                                        {
                                            ContestantId = vote.ContestantId,
                                            Votes = 1
                                        });
                                        _logger.LogInformation($"New Contestant Added - ContestantId: {vote.ContestantId}, Total Votes: 1");
                                    }

                                    await dbContext.SaveChangesAsync(stoppingToken);
                                }
                                else
                                {
                                    _logger.LogError("Failed to deserialize Vote message.");
                                }
                            }
                            else if (consumeResult.Topic == "contestant-topic")
                            {
                                _logger.LogInformation($"Processing Contestant message from topic: {consumeResult.Topic}");
                                // Handling Contestant message (if necessary)
                                _logger.LogInformation($"Contestant message received: {consumeResult.Message.Value}");
                            }
                        }
                    }
                    catch (ConsumeException ex)
                    {
                        _logger.LogError(ex, "Consume error: {Reason}", ex.Error.Reason);
                    }
                    catch (JsonException jsonEx)
                    {
                        _logger.LogError(jsonEx, "JSON deserialization error.");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Unexpected error while consuming message.");
                    }

                    await Task.Delay(100, stoppingToken); // Wait for the next message
                }

                consumer.Close();
                _logger.LogInformation("Kafka consumer closed gracefully.");
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Kafka Consumer failed to start.");
            }
        }
    }
}
