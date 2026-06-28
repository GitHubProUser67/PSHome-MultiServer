using System.Globalization;

namespace Prometheus;

internal struct ThreadSafeLong(long value)
{
    private long _value = value;

    public long Value
    {
        get { return Interlocked.Read(ref _value); }
        set { Interlocked.Exchange(ref _value, value); }
    }

    public void Add(long increment)
    {
        Interlocked.Add(ref _value, increment);
    }

    public override string ToString()
    {
        return Value.ToString(CultureInfo.InvariantCulture);
    }

    public override bool Equals(object? obj)
    {
        return obj is ThreadSafeLong other ? Value.Equals(other.Value) : Value.Equals(obj);
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }
}
