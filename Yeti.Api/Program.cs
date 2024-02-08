using System.Text;
using Lamar;
using Lamar.Microsoft.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Yeti.Db;

var configuration = new ConfigurationBuilder()
    .AddJsonFile(Path.Join(Environment.CurrentDirectory, "appsettings.json"), optional: false)
    .AddEnvironmentVariables()
    .AddCommandLine(args)
    .Build();

// As far as I can tell, at no point AFTER the web application has been created does it attempt to
// read the environment variable describing its environment. Setting this before calling
// Environment.IsDevelopment() but after creating the application builder accomplishes NOTHING.
if (configuration.GetValue<string?>("Environment") is string environmentName)
{
    Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", environmentName);
}

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddConfiguration(configuration);
builder.Host.UseLamar().ConfigureContainer<ServiceRegistry>(ConfigureServices);

var application = ConfigureApplication(builder.Build());
// This is a hack to ensure that my "database" includes seed data.
await application.Services.GetRequiredService<WriterContext>().Database.EnsureCreatedAsync();
application.Logger.LogInformation("startup complete");
await application.RunAsync();

WebApplication ConfigureApplication(WebApplication application)
{
    if (application.Environment.IsDevelopment())
    {
        application.UseSwagger();
        application.UseSwaggerUI(s =>
        {
            s.EnablePersistAuthorization();
        });
    }

    application.MapControllers();
    application.UseAuthentication();
    application.UseAuthorization();
    return application;
}

void ConfigureServices(ServiceRegistry services)
{
    services.AddLogging(config =>
    {
        config.AddSimpleConsole();
        config.SetMinimumLevel(LogLevel.Information);
        config.AddConfiguration(configuration);
    });

    services.AddDbContextPool<WriterContext>(config => config.UseInMemoryDatabase("yeti"));

    services
        .AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            var key = configuration["Jwt:Key"] ?? throw new Exception("missing jwt signing key");
            options.TokenValidationParameters = new()
            {
                ValidateAudience = false,
                // ValidAudience = configuration["Jwt:Audience"],
                ValidateIssuer = true,
                ValidIssuer = configuration["Jwt:Issuer"],
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                ValidateLifetime = false,
            };
        });

    services.AddAuthorization();
    services.AddControllers();
    services.AddEndpointsApiExplorer();
    services.AddSwaggerGen();
}
