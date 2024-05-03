namespace Yeti.Core;

public static class Time
{
    static DateTimeOffset? forced = null;

    public static DateTimeOffset Now => forced ?? DateTimeOffset.UtcNow;

    public static void Set(DateTimeOffset time)
    {
        forced = time;
    }

    public static void Unset()
    {
        forced = null;
    }
}
