using Lamar;
using Lamar.Microsoft.DependencyInjection;
using Microsoft.EntityFrameworkCore;

using Yeti.Api.Config;
using Yeti.Api.Service;
using Yeti.Core;
using Yeti.Core.Config;
using Yeti.Core.Service;
using Yeti.Db;

var isDevelopment = false;
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
    isDevelopment = environmentName == "Development";
}

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddConfiguration(configuration);
builder.Host.UseLamar().ConfigureContainer<ServiceRegistry>(ConfigureServices);

var application = ConfigureApplication(builder.Build());
application.Logger.LogInformation("startup complete");

if (application.Environment.IsDevelopment())
{
    var auth = application.Services.GetRequiredService<TokenService>();
    Console.WriteLine(auth.GenerateToken(1));
}

await application.RunAsync();

WebApplication ConfigureApplication(WebApplication application)
{
    if (application.Environment.IsDevelopment())
    {
        application.UseSwagger();
        application.UseSwaggerUI();
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

    services.ConfigureAuth(configuration);

    if (isDevelopment)
    {
        services.ConfigureSwagger();
    }

    services.AddAuthorization();
    services.AddControllers();
    services.AddEndpointsApiExplorer();
    services.AddHttpContextAccessor();
    services.AddSwaggerGen();

    services.AddDbContextPool<WriterContext>(
        config => config.UseNpgsql(configuration.GetConnectionString("WriterContext")));

    services.ConfigureOptions<ConfigureIndexOptions>();
    services.ConfigureOptions<ConfigureTokenOptions>();

    services.AddHttpClient<IndexClient>(x =>
    {
        x.DefaultRequestHeaders.Add("Accept", "application/json");
        x.BaseAddress = new(configuration["Search:Url"] ?? throw new ConfigurationException("Search:Url"));
    });

    // I'm pretty sure using this as a singleton is fine, but I don't really KNOW, I guess...
    services.AddSingleton<IIndexingService, IndexingService>();
}
