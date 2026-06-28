using System.Diagnostics;

namespace System.Text;

partial struct ValueStringBuilder
{
    private static ReadOnlySpan<byte> HexCharsUpper => "0123456789ABCDEF"u8;
    private static ReadOnlySpan<byte> HexCharsLower => "0123456789abcdef"u8;

    public void AppendHex(uint value, int bitCount, bool isLowerCase = false)
    {
        Debug.Assert(bitCount is > 0 and <= int.MaxValue - 3);
        var hexCharCount = (bitCount + 3) / 4;

        // Mask the value to ensure no extra bits are emitted.
        if (bitCount < 32)
            value &= (1U << bitCount) - 1;

        // Reserve characters in builder for encoded hex data.
        var buffer = AppendSpan(hexCharCount);
        var hexAlphabet = isLowerCase ? HexCharsLower : HexCharsUpper;
        for (int shift = (hexCharCount - 1) * 4, i = 0; shift >= 0; shift -= 4, i++)
        {
            // Out of range shifts are always treated as zero values.
            var c = (shift < 32) ? ((int)(value >> shift) & 0xF) : 0;
            buffer[i] = (char)hexAlphabet[c];
        }
    }

    public void AppendHex(ulong value, int bitCount, bool isLowerCase = false)
    {
        Debug.Assert(bitCount is > 0 and <= int.MaxValue - 3);
        var hexCharCount = (bitCount + 3) / 4;

        // Mask the value to ensure no extra bits are emitted.
        if (bitCount < 64)
            value &= (1UL << bitCount) - 1;

        // Reserve characters in builder for encoded hex data.
        var buffer = AppendSpan(hexCharCount);
        var hexAlphabet = isLowerCase ? HexCharsLower : HexCharsUpper;
        for (int shift = (hexCharCount - 1) * 4, i = 0; shift >= 0; shift -= 4, i++)
        {
            // Out of range shifts are always treated as zero values.
            var c = (shift < 64) ? ((int)(value >> shift) & 0xF) : 0;
            buffer[i] = (char)hexAlphabet[c];
        }
    }
}
