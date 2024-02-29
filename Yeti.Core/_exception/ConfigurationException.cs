namespace Yeti.Core;

public class ConfigurationException(string property) : Exception
{
    public override string ToString() => $"bad or missing parameter: {property}";
}
