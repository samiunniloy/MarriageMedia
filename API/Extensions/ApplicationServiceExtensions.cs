﻿using API.Data;
using API.Helpers;
using API.Interfaces;
using API.services;
using API.Services;
using Microsoft.EntityFrameworkCore;

namespace API.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
        {  
            services.AddControllers();
            services.AddScoped<ITokenService, TokenServices>();
            services.AddDbContext<DataContext>(options =>
            {
                options.UseSqlite(config.GetConnectionString("DefaultConnection"));
            });
            services.AddCors();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ILikesRepository, Likerepository>();
            services.AddScoped<IPhotoService, PhotoService>();
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
          services.Configure<CloudinarySettings>(config.GetSection("CloudinarySettings"));
            return services;
        }
    }
}
