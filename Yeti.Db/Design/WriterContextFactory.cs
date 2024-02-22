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
        // We're going to assume you gave us a path to a .env file.
        // If not, this is going to be a really fast migration.
        var path = args.FirstOrDefault(x => File.Exists(x)) ?? ".env";
        var content = File.ReadAllText(path);

        var m = ConnectionString().Match(content);

        if (!m.Success)
        {
            return null;
        }

        return ConnectionString().Match(content).Groups[1].Value;
    }

    [GeneratedRegex("CONNECTION_STRING=(.+)")]
    private static partial Regex ConnectionString();
}
