namespace MultiServerLibrary.Extension
{
    public static class CharUtils
    {
        public static int GetHexVal(this char hex, byte mode = 0)
        {
            int val = (int)hex;

            switch (mode)
            {
                case 1: // For uppercase A-F letters:
                    return val - (val < 58 ? 48 : 55);
                case 2: // For lowercase a-f letters:
                    return val - (val < 58 ? 48 : 87);
            }

            // Or the two combined, but a bit slower:
            return val - (val < 58 ? 48 : (val < 97 ? 55 : 87));
        }
    }
}
