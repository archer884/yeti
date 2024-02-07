using Lamar;
using Lamar.Microsoft.DependencyInjection;
using Microsoft.EntityFrameworkCore;

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
        application.UseSwaggerUI();
    }

    application.MapControllers();
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

    services.AddControllers();
    services.AddEndpointsApiExplorer();
    services.AddSwaggerGen();
}
