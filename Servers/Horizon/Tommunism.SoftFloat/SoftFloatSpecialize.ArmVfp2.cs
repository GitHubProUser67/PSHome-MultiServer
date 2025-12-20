using System;

namespace Tommunism.SoftFloat
{
    using static Internals;

    partial class SoftFloatSpecialize
    {
        public sealed class ArmVfp2 : SoftFloatSpecialize
        {
            #region Default Instance & Constructor

            /// <summary>
            /// Gets the instance to use for the ARM-VFPv2 specialized implementation details.
            /// </summary>
            public static ArmVfp2 Instance { get; } = new();

            // This is a sealed class with constant default NaN bits, so it should be safe to cache them.
            public ArmVfp2() => InitializeDefaultNaNs();

            #endregion

            public override TininessMode InitialDetectTininess => TininessMode.BeforeRounding;

            #region Integer Conversion Constants

            public override uint UInt32FromPositiveOverflow => 0xFFFFFFFF;

            public override uint UInt32FromNegativeOverflow => 0;

            public override uint UInt32FromNaN => 0;

            public override int Int32FromPositiveOverflow => 0x7FFFFFFF;

            public override int Int32FromNegativeOverflow => -0x7FFFFFFF - 1;

            public override int Int32FromNaN => 0;

            public override ulong UInt64FromPositiveOverflow => 0xFFFFFFFFFFFFFFFF;

            public override ulong UInt64FromNegativeOverflow => 0;

            public override ulong UInt64FromNaN => 0;

            public override long Int64FromPositiveOverflow => 0x7FFFFFFFFFFFFFFF;

            public override long Int64FromNegativeOverflow => -0x7FFFFFFFFFFFFFFF - 1;

            public override long Int64FromNaN => 0;

            internal override SpecializeNaNIntegerKind UInt32NaNKind => SpecializeNaNIntegerKind.NaNIsNegOverflow;

            internal override SpecializeNaNIntegerKind Int32NaNKind => SpecializeNaNIntegerKind.NaNIsUnique;

            #endregion

            #region Float16

            public override ushort DefaultNaNFloat16Bits => 0x7E00;

            public override ushort PropagateNaNFloat16Bits(SoftFloatContext context, uint bitsA, uint bitsB)
            {
                var isSigNaNA = IsSignalingNaNFloat16Bits(bitsA);
                if (isSigNaNA || IsSignalingNaNFloat16Bits(bitsB))
                {
                    context.RaiseFlags(ExceptionFlags.Invalid);
                    return (ushort)((isSigNaNA ? bitsA : bitsB) | 0x0200);
                }

                return (ushort)(IsNaNF16UI(bitsA) ? bitsA : bitsB);
            }

            #endregion

            #region Float32

            public override uint DefaultNaNFloat32Bits => 0x7FC00000;

            public override uint PropagateNaNFloat32Bits(SoftFloatContext context, uint bitsA, uint bitsB)
            {
                var isSigNaNA = IsSignalingNaNFloat32Bits(bitsA);
                if (isSigNaNA || IsSignalingNaNFloat32Bits(bitsB))
                {
                    context.RaiseFlags(ExceptionFlags.Invalid);
                    return (isSigNaNA ? bitsA : bitsB) | 0x00400000;
                }

                return IsNaNF32UI(bitsA) ? bitsA : bitsB;
            }

            #endregion

            #region Float64

            public override ulong DefaultNaNFloat64Bits => 0x7FF8000000000000;

            public override ulong PropagateNaNFloat64Bits(SoftFloatContext context, ulong bitsA, ulong bitsB)
            {
                var isSigNaNA = IsSignalingNaNFloat64Bits(bitsA);
                if (isSigNaNA || IsSignalingNaNFloat64Bits(bitsB))
                {
                    context.RaiseFlags(ExceptionFlags.Invalid);
                    return (isSigNaNA ? bitsA : bitsB) | 0x0008000000000000;
                }

                return IsNaNF64UI(bitsA) ? bitsA : bitsB;
            }

            #endregion

            #region ExtFloat80

            public override SFUInt128 DefaultNaNExtFloat80Bits => new(upper: 0x7FFF, lower: 0xC000000000000000);

            public override SFUInt128 PropagateNaNExtFloat80Bits(SoftFloatContext context, uint bitsA64, ulong bitsA0, uint bitsB64, ulong bitsB0)
            {
                var isSigNaNA = IsSignalingNaNExtFloat80Bits(bitsA64, bitsA0);
                if (isSigNaNA || IsSignalingNaNExtFloat80Bits(bitsB64, bitsB0))
                {
                    context.RaiseFlags(ExceptionFlags.Invalid);
                    return isSigNaNA
                        ? new SFUInt128(upper: bitsA64, lower: bitsA0 | 0xC000000000000000)
                        : new SFUInt128(upper: bitsB64, lower: bitsB0 | 0xC000000000000000);
                }

                return IsNaNF128UI(bitsA64, bitsA0)
                    ? new SFUInt128(upper: bitsA64, lower: bitsA0 | 0xC000000000000000)
                    : new SFUInt128(upper: bitsB64, lower: bitsB0 | 0xC000000000000000);
            }

            #endregion

            #region Float128

            public override SFUInt128 DefaultNaNFloat128Bits => new(upper: 0x7FFF800000000000, lower: 0x0000000000000000);

            public override SFUInt128 PropagateNaNFloat128Bits(SoftFloatContext context, ulong bitsA64, ulong bitsA0, ulong bitsB64, ulong bitsB0)
            {
                var isSigNaNA = IsSignalingNaNFloat128Bits(bitsA64, bitsA0);
                if (isSigNaNA || IsSignalingNaNFloat128Bits(bitsB64, bitsB0))
                {
                    context.RaiseFlags(ExceptionFlags.Invalid);
                    return isSigNaNA
                        ? new SFUInt128(upper: bitsA64 | 0x0000800000000000, lower: bitsA0)
                        : new SFUInt128(upper: bitsB64 | 0x0000800000000000, lower: bitsB0);
                }

                return IsNaNF128UI(bitsA64, bitsA0)
                    ? new SFUInt128(upper: bitsA64 | 0x0000800000000000, lower: bitsA0)
                    : new SFUInt128(upper: bitsB64 | 0x0000800000000000, lower: bitsB0);
            }

            #endregion
        }
    }

}