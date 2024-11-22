using Microsoft.Extensions.Configuration;
using OT.Assessment.Consumer;
using RabbitMQ.Client;
using Serilog;

var config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration(config =>
    {
        config.SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();
    })
    .ConfigureServices((context, services) =>
    {
        //configure services
    })
    .Build();

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.File("Service_Logs.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

var factory = new ConnectionFactory { HostName = config.GetSection("RabbitMQ").GetValue<string>("host") };
var connection = await factory.CreateConnectionAsync();
var channel = await connection.CreateChannelAsync();

await channel.QueueDeclareAsync(queue: config.GetSection("RabbitMQ").GetValue<string>("queueName"), durable: false, exclusive: false, autoDelete: false, arguments: null);

var service = new RabbitMQService(config.GetConnectionString("DatabaseConnection"));

var logger = host.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("Application started {time:yyyy-MM-dd HH:mm:ss}", DateTime.Now);
await service.ConsumerPlayerWagers(channel);
await host.RunAsync();

logger.LogInformation("Application ended {time:yyyy-MM-dd HH:mm:ss}", DateTime.Now);