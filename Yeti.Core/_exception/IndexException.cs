namespace Yeti.Core;

public class IndexException(long id) : Exception
{
    public long Id { get; init; } = id;
    public override string Message => $"index operation failed for Id:{Id}";
}
