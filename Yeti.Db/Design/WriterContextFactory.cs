using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Yeti.Db.Design;

public partial class WriterContextFactory : IDesignTimeDbContextFactory<WriterContext>
{
    public WriterContext CreateDbContext(string[] args)
    {
        if (GetConnectionString(args) is string connectionString)
        {
            Console.WriteLine(connectionString);
            var optionsBuilder = new DbContextOptionsBuilder<WriterContext>()
                .UseNpgsql(connectionString);
            return new WriterContext(optionsBuilder.Options);
        }

        throw new Exception("missing connection string");
    }

    static string? GetConnectionString(string[] args)
    {
        // An explicit environment variable wins (handy in containers); otherwise fall back to a
        // .env file passed as an argument or present in the working directory.
        if (Environment.GetEnvironmentVariable("CONNECTION_STRING") is { Length: > 0 } env)
        {
            return env.Trim('"');
        }

        var path = args.FirstOrDefault(x => File.Exists(x));
        if (path is null && File.Exists(".env"))
        {
            path = ".env";
        }

        if (path is null)
        {
            return null;
        }

        var match = ConnectionString().Match(File.ReadAllText(path));
        return match.Success ? match.Groups[1].Value.Trim().Trim('"') : null;
    }

    [GeneratedRegex("CONNECTION_STRING=(.+)")]
    private static partial Regex ConnectionString();
}
