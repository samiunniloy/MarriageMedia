using API.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace API.Extensions
{
    public static class MongoClientExtensions
    {
        public static IServiceCollection AddMongoClient(this IServiceCollection services, IConfiguration configuration)
        {
            // Bind MongoDB settings to a class
            var mongoSettings = configuration.GetSection("MongoDB").Get<MongoDbSettings>();

            if (mongoSettings == null || string.IsNullOrEmpty(mongoSettings.ConnectionString))
            {
                throw new ArgumentNullException(nameof(mongoSettings.ConnectionString),
                    "MongoDB connection string is missing in appsettings.json");
            }

            services.Configure<MongoDbSettings>(configuration.GetSection("MongoDB"));

            // Register MongoClient
            services.AddSingleton<IMongoClient>(sp => new MongoClient(mongoSettings.ConnectionString));

            // Register IMongoDatabase
            services.AddScoped<IMongoDatabase>(sp =>
            {
                var client = sp.GetRequiredService<IMongoClient>();
                return client.GetDatabase(mongoSettings.DatabaseName);
            });

            return services;
        }
    }
}
