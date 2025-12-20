using System;

namespace Tommunism.SoftFloat
{
    using static Internals;

    partial class SoftFloatSpecialize
    {
        public sealed class X86Sse : SoftFloatSpecialize
        {
            #region Default Instance & Constructor

            /// <summary>
            /// Gets the instance to use for the 8086-SSE specialized implementation details.
            /// </summary>
            public static X86Sse Instance { get; } = new();

            // This is a sealed class with constant default NaN bits, so it should be safe to cache them.
            public X86Sse() => InitializeDefaultNaNs();

            #endregion

            public override TininessMode InitialDetectTininess => TininessMode.AfterRounding;

            #region Integer Conversion Constants

            public override uint UInt32FromPositiveOverflow => 0xFFFFFFFF;

            public override uint UInt32FromNegativeOverflow => 0xFFFFFFFF;

            public override uint UInt32FromNaN => 0xFFFFFFFF;

            public override int Int32FromPositiveOverflow => -0x7FFFFFFF - 1;

            public override int Int32FromNegativeOverflow => -0x7FFFFFFF - 1;

            public override int Int32FromNaN => -0x7FFFFFFF - 1;

            public override ulong UInt64FromPositiveOverflow => 0xFFFFFFFFFFFFFFFF;

            public override ulong UInt64FromNegativeOverflow => 0xFFFFFFFFFFFFFFFF;

            public override ulong UInt64FromNaN => 0xFFFFFFFFFFFFFFFF;

            public override long Int64FromPositiveOverflow => -0x7FFFFFFFFFFFFFFF - 1;

            public override long Int64FromNegativeOverflow => -0x7FFFFFFFFFFFFFFF - 1;

            public override long Int64FromNaN => -0x7FFFFFFFFFFFFFFF - 1;

            internal override SpecializeNaNIntegerKind UInt32NaNKind => SpecializeNaNIntegerKind.NaNIsPosAndNegOverflow;

            internal override SpecializeNaNIntegerKind Int32NaNKind => SpecializeNaNIntegerKind.NaNIsPosAndNegOverflow;

            #endregion

            #region Float16

            public override ushort DefaultNaNFloat16Bits => 0xFE00;

            public override ushort PropagateNaNFloat16Bits(SoftFloatContext context, uint bitsA, uint bitsB)
            {
                var isSigNaNA = IsSignalingNaNFloat16Bits(bitsA);
                if (isSigNaNA || IsSignalingNaNFloat16Bits(bitsB))
                {
                    context.RaiseFlags(ExceptionFlags.Invalid);
                    if (isSigNaNA)
                        return (ushort)(bitsA | 0x0200);
                }

                return (ushort)((IsNaNF16UI(bitsA) ? bitsA : bitsB) | 0x0200);
            }

            #endregion

            #region Float32

            public override uint DefaultNaNFloat32Bits => 0xFFC00000;

            public override uint PropagateNaNFloat32Bits(SoftFloatContext context, uint bitsA, uint bitsB)
            {
                var isSigNaNA = IsSignalingNaNFloat32Bits(bitsA);
                if (isSigNaNA || IsSignalingNaNFloat32Bits(bitsB))
                {
                    context.RaiseFlags(ExceptionFlags.Invalid);
                    if (isSigNaNA)
                        return bitsA | 0x00400000;
                }

                return (IsNaNF32UI(bitsA) ? bitsA : bitsB) | 0x00400000;
            }

            #endregion

            #region Float64

            public override ulong DefaultNaNFloat64Bits => 0xFFF8000000000000;

            public override ulong PropagateNaNFloat64Bits(SoftFloatContext context, ulong bitsA, ulong bitsB)
            {
                var isSigNaNA = IsSignalingNaNFloat64Bits(bitsA);
                if (isSigNaNA || IsSignalingNaNFloat64Bits(bitsB))
                {
                    context.RaiseFlags(ExceptionFlags.Invalid);
                    if (isSigNaNA)
                        return bitsA | 0x0008000000000000;
                }

                return (IsNaNF64UI(bitsA) ? bitsA : bitsB) | 0x0008000000000000;
            }

            #endregion

            #region ExtFloat80

            public override SFUInt128 DefaultNaNExtFloat80Bits => new(upper: 0xFFFF, lower: 0xC000000000000000);

            #endregion

            #region Float128

            public override SFUInt128 DefaultNaNFloat128Bits => new(upper: 0xFFFF800000000000, lower: 0x0000000000000000);

            public override SFUInt128 PropagateNaNFloat128Bits(SoftFloatContext context, ulong bitsA64, ulong bitsA0, ulong bitsB64, ulong bitsB0)
            {
                var isSigNaNA = IsSignalingNaNFloat128Bits(bitsA64, bitsA0);
                if (isSigNaNA || IsSignalingNaNFloat128Bits(bitsB64, bitsB0))
                {
                    context.RaiseFlags(ExceptionFlags.Invalid);
                    if (isSigNaNA)
                        return new SFUInt128(upper: bitsA64 | 0x0000800000000000, lower: bitsA0);
                }

                return IsNaNF128UI(bitsA64, bitsA0)
                    ? new SFUInt128(upper: bitsA64 | 0x0000800000000000, lower: bitsA0)
                    : new SFUInt128(upper: bitsB64 | 0x0000800000000000, lower: bitsB0);
            }

            #endregion
        }
    }

}