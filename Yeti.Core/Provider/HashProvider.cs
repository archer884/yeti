using System.Text;
using Geralt;
using Microsoft.Extensions.Options;

using Yeti.Core.Config;

namespace Yeti.Core.Provider;

public class HashProvider(IOptions<HashProviderOptions> options)
{
    int Memory { get; init; } = options.Value.Memory;
    int Iterations { get; init; } = options.Value.Iterations;

    public string ComputeHash(string password)
    {
        var p = Encoding.UTF8.GetBytes(password);
        Span<byte> hash = new();
        Argon2id.ComputeHash(hash, p, Iterations, Memory);
        return Encoding.UTF8.GetString(hash).Trim();
    }

    // This is a service. We're not going to pretend that its members are always going to be static.
#pragma warning disable CA1822 // Mark members as static
    public bool VerifyHash(string hash, string password)
#pragma warning restore CA1822 // Mark members as static
    {
        var h = Encoding.UTF8.GetBytes(hash);
        var p = Encoding.UTF8.GetBytes(password);
        return Argon2id.VerifyHash(h, p);
    }
}
