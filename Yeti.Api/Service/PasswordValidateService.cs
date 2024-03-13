using Isopoh.Cryptography.Argon2;

namespace Yeti.Api.Service;

public class PasswordValidateService
{
    public bool Verify(string candidate, string encoded) => Argon2.Verify(encoded, candidate);
}
