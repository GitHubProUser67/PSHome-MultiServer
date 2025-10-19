using Tommunism.SoftFloat;

namespace PS2FloatLibrary
{
    //****************************************************************
    // Booth Multiplier
    //****************************************************************
    public class BoothMultiplier
    {
        public static ulong MultiplyMantissa(uint a, uint b)
        {
            // Emulate PS2 Wallace tree.
            Wallace wallace = new Wallace(a, b);
            ulong manA = Float64.FromBitsUI64(wallace.fs_multiplier).RawMantissa;
            ulong manB = Float64.FromBitsUI64(wallace.ft_multiplier).RawMantissa;
            ulong full = manA + manB;
            ulong product = (ulong)manA * (ulong)manB;
			
            // Massive thanks to the PCSX2 Team to have found the imprecision masks (TellowKrinkle)

            // Emulate PS2 imprecision: mask lower 15 bits of the lower and upper halves
            ulong lo = product & ~0x7fffu;
            ulong hi = (product >> 16) & ~0x7fffu;

            // Apply the final multiply adjustment using the masked values
            return full - (((lo + hi) ^ (full)) & 0x8000);
        }
    }
}
