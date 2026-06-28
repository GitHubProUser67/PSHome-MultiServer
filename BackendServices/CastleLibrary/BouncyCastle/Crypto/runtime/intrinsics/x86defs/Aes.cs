namespace Org.BouncyCastle.Runtime.Intrinsics.X86
{
    internal static class Aes
    {
        internal static bool IsEnabled => System.Runtime.Intrinsics.X86.Aes.IsSupported;

        //        internal static class X64
        //        {
        //#if NETCOREAPP3_0_OR_GREATER
        //            internal static bool IsEnabled => System.Runtime.Intrinsics.X86.Aes.X64.IsSupported;
        //#else
        //            internal static bool IsEnabled => false;
        //#endif
        //        }
    }
}
