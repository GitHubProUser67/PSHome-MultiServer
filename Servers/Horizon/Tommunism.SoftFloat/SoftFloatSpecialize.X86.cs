using System;

namespace Tommunism.SoftFloat
{
    using static Internals;

    partial class SoftFloatSpecialize
    {
        public sealed class X86 : SoftFloatSpecialize
        {
            #region Default Instance & Constructor

            /// <summary>
            /// Gets the instance to use for the 8086 specialized implementation details.
            /// </summary>
            public static X86 Instance { get; } = new();

            // This is a sealed class with constant default NaN bits, so it should be safe to cache them.
            public X86() => InitializeDefaultNaNs();

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
                var isSigNaNB = IsSignalingNaNFloat16Bits(bitsB);

                // Make NaNs non-signaling.
                var uiNonsigA = bitsA | 0x0200;
                var uiNonsigB = bitsB | 0x0200;

                if (isSigNaNA | isSigNaNB)
                {
                    context.RaiseFlags(ExceptionFlags.Invalid);
                    if (isSigNaNA)
                    {
                        if (!isSigNaNB)
                            return (ushort)(IsNaNF16UI(bitsB) ? uiNonsigB : uiNonsigA);
                    }
                    else
                    {
                        return (ushort)(IsNaNF16UI(bitsA) ? uiNonsigA : uiNonsigB);
                    }
                }

                var uiMagA = bitsA & 0x7FFF;
                var uiMagB = bitsB & 0x7FFF;
                if (uiMagA < uiMagB) return (ushort)uiNonsigB;
                if (uiMagB < uiMagA) return (ushort)uiNonsigA;
                return (ushort)((uiNonsigA < uiNonsigB) ? uiNonsigA : uiNonsigB);
            }

            #endregion

            #region Float32

            public override uint DefaultNaNFloat32Bits => 0xFFC00000;

            public override uint PropagateNaNFloat32Bits(SoftFloatContext context, uint bitsA, uint bitsB)
            {
                var isSigNaNA = IsSignalingNaNFloat32Bits(bitsA);
                var isSigNaNB = IsSignalingNaNFloat32Bits(bitsB);

                // Make NaNs non-signaling.
                var uiNonsigA = bitsA | 0x00400000;
                var uiNonsigB = bitsB | 0x00400000;

                if (isSigNaNA | isSigNaNB)
                {
                    context.RaiseFlags(ExceptionFlags.Invalid);
                    if (isSigNaNA)
                    {
                        if (!isSigNaNB)
                            return IsNaNF32UI(bitsB) ? uiNonsigB : uiNonsigA;
                    }
                    else
                    {
                        return IsNaNF32UI(bitsA) ? uiNonsigA : uiNonsigB;
                    }
                }

                var uiMagA = bitsA & 0x7FFFFFFF;
                var uiMagB = bitsB & 0x7FFFFFFF;
                if (uiMagA < uiMagB) return uiNonsigB;
                if (uiMagB < uiMagA) return uiNonsigA;
                return (uiNonsigA < uiNonsigB) ? uiNonsigA : uiNonsigB;
            }

            #endregion

            #region Float64

            public override ulong DefaultNaNFloat64Bits => 0xFFF8000000000000;

            public override ulong PropagateNaNFloat64Bits(SoftFloatContext context, ulong bitsA, ulong bitsB)
            {
                var isSigNaNA = IsSignalingNaNFloat64Bits(bitsA);
                var isSigNaNB = IsSignalingNaNFloat64Bits(bitsB);

                // Make NaNs non-signaling.
                var uiNonsigA = bitsA | 0x0008000000000000;
                var uiNonsigB = bitsB | 0x0008000000000000;

                if (isSigNaNA | isSigNaNB)
                {
                    context.RaiseFlags(ExceptionFlags.Invalid);
                    if (isSigNaNA)
                    {
                        if (!isSigNaNB)
                            return IsNaNF64UI(bitsB) ? uiNonsigB : uiNonsigA;
                    }
                    else
                    {
                        return IsNaNF64UI(bitsA) ? uiNonsigA : uiNonsigB;
                    }
                }

                var uiMagA = bitsA & 0x7FFFFFFFFFFFFFFF;
                var uiMagB = bitsB & 0x7FFFFFFFFFFFFFFF;
                if (uiMagA < uiMagB) return uiNonsigB;
                if (uiMagB < uiMagA) return uiNonsigA;
                return (uiNonsigA < uiNonsigB) ? uiNonsigA : uiNonsigB;
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
                var isSigNaNB = IsSignalingNaNFloat128Bits(bitsB64, bitsB0);

                // Make NaNs non-signaling.
                var uiNonsigA64 = bitsA64 | 0x0000800000000000;
                var uiNonsigB64 = bitsB64 | 0x0000800000000000;

                if (isSigNaNA | isSigNaNB)
                {
                    context.RaiseFlags(ExceptionFlags.Invalid);
                    if (isSigNaNA)
                    {
                        if (!isSigNaNB)
                        {
                            return IsNaNF128UI(bitsB64, bitsB0)
                                ? new SFUInt128(upper: uiNonsigB64, lower: bitsB0)
                                : new SFUInt128(upper: uiNonsigA64, lower: bitsA0);
                        }
                    }
                    else
                    {
                        return IsNaNF128UI(bitsA64, bitsA0)
                            ? new SFUInt128(upper: bitsA64, lower: bitsB0)
                            : new SFUInt128(upper: uiNonsigA64, lower: bitsA0);
                    }
                }

                var uiMagA64 = bitsA64 & 0x7FFFFFFFFFFFFFFF;
                var uiMagB64 = bitsB64 & 0x7FFFFFFFFFFFFFFF;

                int cmp = uiMagA64.CompareTo(uiMagB64);
                if (cmp == 0)
                {
                    cmp = bitsA0.CompareTo(bitsB0);
                    if (cmp == 0)
                        cmp = uiNonsigB64.CompareTo(uiNonsigA64);
                }

                return cmp <= 0
                    ? new SFUInt128(upper: uiNonsigB64, lower: bitsB0)
                    : new SFUInt128(upper: uiNonsigA64, lower: bitsA0);
            }

            #endregion
        }
    }

}