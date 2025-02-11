using MongoDB.Driver;
using RabbitMQ.Client;
using Service2.Worker;
using Service2.Worker.MongoDb;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        var mongoConfig = new MongoDbConfig
        {
            ConnectionString = context.Configuration.GetValue<string>("MongoDB:ConnectionString") ?? "mongodb://localhost:27017",
            DatabaseName = context.Configuration.GetValue<string>("MongoDB:DatabaseName") ?? "MarriageMedia"
        };

        services.AddSingleton(mongoConfig);
        services.AddSingleton<IMongoClient>(sp => new MongoClient(mongoConfig.ConnectionString));
        services.AddScoped<IProcessedImageRepository, ProcessedImageRepository>(sp =>
            new ProcessedImageRepository(
                sp.GetRequiredService<IMongoClient>(),
                mongoConfig.DatabaseName
            ));

        services.AddSingleton<IConnectionFactory>(sp =>
            new ConnectionFactory { HostName = "localhost" });

        services.AddHostedService<ImageProcessingWorker>();
    })
    .Build();

await host.RunAsync();