namespace HomeTools.BARFramework
{
    public static class Utils
    {
        public static long GetFourByteAligned(long input)
        {
            long alignment = input & ~3L; // Use bitwise AND operator to clear the lowest two bits

            if (alignment < input)
                alignment = input + 4L & ~3L; // Add 4 and clear the lowest two bits

            return alignment;
        }
    }
}