using Lamar;
using Lamar.Microsoft.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

using Yeti.Api.Config;
using Yeti.Api.Service;
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
    Console.WriteLine(auth.GenerateAccessToken(1, TimeSpan.FromDays(7)));
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

    services.ConfigureOptions<Configure<HashProviderOptions>>();
    services.ConfigureOptions<Configure<IndexOptions>>();
    services.ConfigureOptions<Configure<TokenOptions>>();

    services.AddHttpClient<IndexClient>((config, client) =>
    {
        var url = config.GetRequiredService<IOptions<IndexOptions>>().Value.Url;
        client.BaseAddress = new(url);
        client.DefaultRequestHeaders.Add("Accept", "application/json");
    });

    // Do NOT ask how much reading I had to do to find this...
    // Ok, it was LOTS. I can't bear to delete it yet. This is how to configure the client for
    // some kind of authorization.
    // .ConfigurePrimaryHttpMessageHandler(config =>
    // {
    //     var options = config.GetRequiredService<IOptions<HotelClientOptions>>().Value;
    //     var credential = options.HasCredential
    //         ? new NetworkCredential(options.Username, options.Password)
    //         : CredentialCache.DefaultNetworkCredentials;

    //     CredentialCache cache = [];
    //     cache.Add(new(options.BaseUrl), "NTLM", credential);
    //     return new HttpClientHandler { Credentials = cache };
    // });

    // I'm pretty sure using this as a singleton is fine, but I don't really KNOW, I guess...
    services.AddSingleton<IIndexingService, IndexingService>();
}
