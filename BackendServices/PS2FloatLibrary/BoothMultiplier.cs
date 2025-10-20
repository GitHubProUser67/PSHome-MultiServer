using Tommunism.SoftFloat;

namespace PS2FloatLibrary
{
    //****************************************************************
    // Booth Multiplier
    //****************************************************************
    public class BoothMultiplier
    {
        private static bool _wallace = true;

        public static bool WallaceTree  // Emulates the PS2 Wallace tree when set to true (full accuracy but a lot slower).
        {
            get 
            {
                return _wallace;
            }
            set
            {
                _wallace = value;
            }
        }

        public static ulong MultiplyMantissa(uint a, uint b)
        {
            // Massive thanks to the PCSX2 Team to have found the imprecision masks (TellowKrinkle)
			const uint mask = ~0x7fffu;

			ulong partialA, partialB;

			if (_wallace)
			{
				Wallace wallace = new Wallace(a, b);

				partialA = Float64.FromBitsUI64(wallace.fs_multiplier).RawMantissa;
				partialB = Float64.FromBitsUI64(wallace.ft_multiplier).RawMantissa;
			}
			else
			{
				partialA = (ulong)a * (ulong)b;
				partialB = byte.MinValue;
			}

			ulong full = partialA + partialB;
			ulong product = partialA * partialB;

			// Emulate PS2 imprecision: mask lower 15 bits of the lower and upper halves as well as applying the final multiply adjustment using the masked values
			return full - ((((product & mask) + ((product >> 16) & mask)) ^ (full)) & 0x8000);
        }
    }
}
