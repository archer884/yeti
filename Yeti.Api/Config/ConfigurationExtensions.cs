using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

using Lamar;

using Yeti.Core;

namespace Yeti.Api.Config;

public static class ConfigurationExtensions
{
    public static void ConfigureAuth(this ServiceRegistry services, IConfiguration configuration)
    {
        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                var key = configuration["Jwt:Key"] ?? throw new ConfigurationException("Jwt:Key");

                options.TokenValidationParameters = new()
                {
                    ValidateAudience = false,
                    // ValidAudience = configuration["Jwt:Audience"],
                    ValidateIssuer = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                    ValidateLifetime = true,
                };
            });
    }

    public static void ConfigureSwagger(this ServiceRegistry services)
    {
        services.AddSwaggerGen(swagger =>
        {
            var definition = new OpenApiSecurityScheme()
            {
                Name = "Bearer",
                BearerFormat = "JWT",
                Scheme = "bearer",
                Description = "Auth token",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
            };

            OpenApiSecurityRequirement requirements = new() { { new("jwt_auth"), [] } };

            swagger.AddSecurityDefinition("jwt_auth", definition);
            swagger.AddSecurityRequirement(_ => requirements);
        });
    }
}
