using System;

namespace Tommunism.SoftFloat;

/// <summary>
/// "Common NaN" structure, used to transfer NaN representations from one format to another.
/// </summary>
public struct SoftFloatCommonNaN
{
    public bool Sign { get; set; }

    public UInt128 Value { get; set; }
}
