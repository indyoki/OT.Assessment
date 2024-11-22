using System.Text;
using System.Text.Json;
using Common.Models;
using OT.Assessment.Consumer.Repository;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Serilog;

namespace OT.Assessment.Consumer
{
    public class RabbitMQService
    {
        private readonly SqlRepository _sqlRepository;

        public RabbitMQService(string connectionString)
        {
            _sqlRepository = new SqlRepository(connectionString);
        }
        public async Task ConsumerPlayerWagers(IChannel channel)
        {
            try
            {
                var consumer = new AsyncEventingBasicConsumer(channel);
                consumer.ReceivedAsync += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var messageString = Encoding.UTF8.GetString(body);
                    SaveToRepo(messageString);
                    Log.Information($"Received message with body: {messageString}");

                    return Task.CompletedTask;
                };

                await channel.BasicConsumeAsync("casino.wagers", autoAck: true, consumer: consumer);
            }
            catch (Exception ex)
            {
                Log.Error("An error occured consuming message from the queue, exception: {ex}", ex);
                throw;
            }
        }
        private void SaveToRepo(string messageString)
        {
            var message = JsonSerializer.Deserialize<CasinoWagerRequest>(messageString);
            _sqlRepository.SaveWagerToDatabase(message).ConfigureAwait(false).GetAwaiter().GetResult();
        }
    }
}
