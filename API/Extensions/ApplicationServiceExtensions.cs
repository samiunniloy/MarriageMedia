using API.Data;
using API.Data.Repositories;
using API.Helpers;
using API.Interfaces;
using API.services;
using API.Services;
using API.SignalR;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using StackExchange.Redis;

namespace API.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddControllers();
            services.AddScoped<ITokenService, TokenServices>();

            // SQL Server configuration
            services.AddDbContext<DataContext>(options =>
            {
                options.UseSqlite(config.GetConnectionString("DefaultConnection"));
            });



            //services.AddStackExchangeRedisCache(options =>
            //{
            //    options.Configuration = "localhost:6379"; // Redis server configuration
            //    options.InstanceName = "MarriageMedia"; // Optional: Prefix for cache keys
            //});

            //// Optional: Direct Redis connection if you need to interact with Redis directly
            //services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect("localhost:6379"));




            services.AddSingleton<IMongoClient>(sp =>
                new MongoClient(config.GetValue<string>("MongoDB:ConnectionString")));

            services.AddScoped(sp =>
                sp.GetRequiredService<IMongoClient>().GetDatabase(config.GetValue<string>("MongoDB:DatabaseName")));


            services.AddCors();
           // services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ILikesRepository, LikesRepository>();
            services.AddScoped<IMessageRepository, MessageRepository>();
            services.AddScoped<IPhotoService, PhotoService>();
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.Configure<CloudinarySettings>(config.GetSection("CloudinarySettings"));
            services.AddSignalR(e =>
            {
                e.MaximumReceiveMessageSize = 102400000;
            });
            services.AddSingleton<PresenceTracker>();

            return services;
        }
    }
}
