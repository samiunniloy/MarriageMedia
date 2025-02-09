
using RabbitMQ.Client;
using Service2.Worker;


var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddSingleton<IMongoService, MongoDBService>();
        services.AddSingleton<RabbitMQ.Client.IConnectionFactory>(sp =>
            new ConnectionFactory { HostName = "localhost" });
        services.AddHostedService<ImageProcessingWorker>();
    })
    .Build();

await host.RunAsync();