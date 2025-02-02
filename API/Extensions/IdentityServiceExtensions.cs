using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace API.Extensions
{
    public static class IdentityServiceExtensions
    {
        public static IServiceCollection AddIdentityService(this IServiceCollection services, IConfiguration config)
        {
            services.AddIdentityCore<AppUser>(opt =>
            {
                opt.Password.RequireNonAlphanumeric = false;
            })
                .AddRoles<AppRole>()
                .AddRoleManager<RoleManager<AppRole>>()
                //.AddSignInManager<SignInManager<AppUser>>()
                //.AddRoleValidator<RoleValidator<AppRole>>()
                .AddEntityFrameworkStores<DataContext>();
                //.AddDefaultTokenProviders();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
                    {
                        var tokenKey = config["TokenKey"] ?? throw new Exception("Tokenkey not found");
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuerSigningKey = true,
                            IssuerSigningKey =  new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey)),
                            ValidateIssuer = false,
                            ValidateAudience = false
                        };
                    });


            services.AddAuthorization(opt =>
            {
                opt.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
                opt.AddPolicy("ModeratePhotoRole", policy => policy.RequireRole("Admin", "Moderator"));
            });



            return services;
        }
    }
}
