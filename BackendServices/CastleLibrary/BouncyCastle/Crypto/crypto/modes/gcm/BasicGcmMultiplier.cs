using System;
using System.Runtime.Intrinsics.X86;

namespace Org.BouncyCastle.Crypto.Modes.Gcm
{
    [Obsolete("Will be removed")]
    public class BasicGcmMultiplier : IGcmMultiplier
    {
        internal static bool IsHardwareAccelerated =>
            Org.BouncyCastle.Runtime.Intrinsics.X86.Pclmulqdq.IsEnabled;

        private GcmUtilities.FieldElement H;

        public void Init(byte[] H)
        {
            GcmUtilities.AsFieldElement(H, out this.H);
        }

        public void MultiplyH(byte[] x)
        {
            GcmUtilities.AsFieldElement(x, out var T);
            GcmUtilities.Multiply(ref T, ref H);
            GcmUtilities.AsBytes(ref T, x);
        }
    }
}
