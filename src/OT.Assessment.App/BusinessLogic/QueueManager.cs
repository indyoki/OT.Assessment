using Common.Models;
using OT.Assessment.App.BusinessLogic.Interfaces;
using RabbitMQ.Client;
using Serilog;
using System.Text;
using System.Text.Json;

namespace OT.Assessment.App.BusinessLogic
{
    public class QueueManager : IQueueManager
    {
        private readonly IConfiguration _config;
        public QueueManager(IConfiguration config)
        {
            _config = config;
        }

        public async Task PublishPlayerWager(CasinoWagerRequest request)
        {
            try
            {
                var queueConfig = _config.GetSection("RabbitMQ");
                var factory = new ConnectionFactory 
                { 
                    HostName = queueConfig.GetValue<string>("host"),
                    UserName = queueConfig.GetValue<string>("userName"),
                    Password = queueConfig.GetValue<string>("password")
                };
                var connection = await factory.CreateConnectionAsync();
                var channel = await connection.CreateChannelAsync() ;

                await channel.QueueDeclareAsync(queue: queueConfig.GetValue<string>("queueName"), durable: false, exclusive: false, autoDelete: false, arguments: null);

                var message = JsonSerializer.Serialize(request);
                var body = Encoding.UTF8.GetBytes(message);
                var queueName = queueConfig.GetValue<string>("queueName");

                await channel.BasicPublishAsync(exchange: string.Empty, routingKey: queueName, body: body);

                Log.Information("Message published successfully. Queue name: {0}, Message body: {1}", queueName, message );
            }
            catch (Exception ex)
            {
                Log.Error("An error occured while attempting to publish message, exception {ex} ", ex.Message);
                throw ex;
            }
        }
    }
}
