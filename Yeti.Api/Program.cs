using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Lamar;

using Yeti.Db;
using Yeti.Db.Model;

// This configuration object does nothing at this point. Eventually, it should be used to bring
// in things like db connection strings, passwords, whatever.
var configuration = new ConfigurationBuilder()
    .AddEnvironmentVariables()
    .AddJsonFile(Path.Join(Environment.CurrentDirectory, "appsettings.json"))
    .Build();

// This is the application container. It manages lifetimes and provides dependency injection.
// Ordinarily, you wouldn't configure a bare container like this; you would configure a WEB
// APPLICATION--however, I'm in a hurry and I don't actually need a web application for this
// demo.
var container = new Container(services => 
{
    services.AddLogging(config =>
    {
        config.SetMinimumLevel(LogLevel.Information);
        config.AddSimpleConsole();
    });

    services.AddDbContextPool<WriterContext>(config => config.UseInMemoryDatabase("yeti"));
});

// This is a hack to ensure that my "database" includes seed data.
await container.GetInstance<WriterContext>().Database.EnsureCreatedAsync();

var provider = container.GetService<FragmentProvider>()
    ?? throw new Exception("wtf?");

foreach (var fragment in await provider.ByWriterId(1))
{
    Console.WriteLine(fragment.Content);
}

// This class is for demonstration purposes only and wouldn't normally exist.
public class FragmentProvider(ILogger<FragmentProvider> logger, WriterContext context)
{
    public Task<List<Fragment>> ByWriterId(long writerId)
    {
        logger.LogInformation("get fragments for {writer_id}", writerId);
        return context.Fragments.Where(x => x.WriterId == writerId).ToListAsync();
    }
}
