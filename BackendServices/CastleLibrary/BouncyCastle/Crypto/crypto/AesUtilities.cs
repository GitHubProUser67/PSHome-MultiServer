using Org.BouncyCastle.Crypto.Engines;

namespace Org.BouncyCastle.Crypto
{
    public static class AesUtilities
    {
        public static IBlockCipher CreateEngine()
        {
            if (IsHardwareAccelerated)
                return new AesEngine_X86();

            return new AesEngine();
        }

        public static bool IsHardwareAccelerated => AesEngine_X86.IsSupported;
    }
}
