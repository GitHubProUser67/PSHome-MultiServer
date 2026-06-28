namespace Org.BouncyCastle.Runtime.Intrinsics.X86
{
    internal static class Bmi2
    {
        internal static bool IsEnabled => System.Runtime.Intrinsics.X86.Bmi2.IsSupported;

        internal static class X64
        {
            internal static bool IsEnabled => System.Runtime.Intrinsics.X86.Bmi2.X64.IsSupported;
        }
    }
}
