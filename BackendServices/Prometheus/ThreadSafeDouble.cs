using System.Globalization;

namespace Prometheus;

internal struct ThreadSafeDouble(double value)
{
    private long _value = BitConverter.DoubleToInt64Bits(value);

    public double Value
    {
        get { return BitConverter.Int64BitsToDouble(Interlocked.Read(ref _value)); }
        set { Interlocked.Exchange(ref _value, BitConverter.DoubleToInt64Bits(value)); }
    }

    public void Add(double increment)
    {
        while (true)
        {
            var initialValue = Volatile.Read(ref _value);
            var computedValue = BitConverter.Int64BitsToDouble(initialValue) + increment;

            if (
                initialValue
                == Interlocked.CompareExchange(
                    ref _value,
                    BitConverter.DoubleToInt64Bits(computedValue),
                    initialValue
                )
            )
                return;
        }
    }

    /// <summary>
    /// Sets the value to this, unless the existing value is already greater.
    /// </summary>
    public void IncrementTo(double to)
    {
        while (true)
        {
            var initialRaw = Volatile.Read(ref _value);
            var initialValue = BitConverter.Int64BitsToDouble(initialRaw);

            if (initialValue >= to)
                return; // Already greater.

            if (
                initialRaw
                == Interlocked.CompareExchange(
                    ref _value,
                    BitConverter.DoubleToInt64Bits(to),
                    initialRaw
                )
            )
                return;
        }
    }

    /// <summary>
    /// Sets the value to this, unless the existing value is already smaller.
    /// </summary>
    public void DecrementTo(double to)
    {
        while (true)
        {
            var initialRaw = Volatile.Read(ref _value);
            var initialValue = BitConverter.Int64BitsToDouble(initialRaw);

            if (initialValue <= to)
                return; // Already smaller.

            if (
                initialRaw
                == Interlocked.CompareExchange(
                    ref _value,
                    BitConverter.DoubleToInt64Bits(to),
                    initialRaw
                )
            )
                return;
        }
    }

    public override string ToString()
    {
        return Value.ToString(CultureInfo.InvariantCulture);
    }

    public override bool Equals(object? obj)
    {
        return obj is ThreadSafeDouble other ? Value.Equals(other.Value) : Value.Equals(obj);
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }
}
