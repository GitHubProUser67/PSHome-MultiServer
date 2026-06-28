namespace Org.BouncyCastle.Runtime.Intrinsics.X86
{
    internal static class Pclmulqdq
    {
        internal static bool IsEnabled => System.Runtime.Intrinsics.X86.Pclmulqdq.IsSupported;

        //        internal static class X64
        //        {
        //#if NETCOREAPP3_0_OR_GREATER
        //            internal static bool IsEnabled => System.Runtime.Intrinsics.X86.Pclmulqdq.X64.IsSupported;
        //#else
        //            internal static bool IsEnabled => false;
        //#endif
        //        }
    }
}
