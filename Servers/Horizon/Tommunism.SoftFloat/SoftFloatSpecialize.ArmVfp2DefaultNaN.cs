using System;

namespace Tommunism.SoftFloat
{
    partial class SoftFloatSpecialize
    {
        public sealed class ArmVfp2DefaultNaN : SoftFloatSpecialize
        {
            #region Default Instance & Constructor

            /// <summary>
            /// Gets the instance to use for the ARM-VFPv2-defaultNaN specialized implementation details.
            /// </summary>
            public static ArmVfp2DefaultNaN Instance { get; } = new();

            // This is a sealed class with constant default NaN bits, so it should be safe to cache them.
            public ArmVfp2DefaultNaN() => InitializeDefaultNaNs();

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

            public override void Float16BitsToCommonNaN(SoftFloatContext context, uint bits, out SoftFloatCommonNaN commonNaN)
            {
                commonNaN = default;
                if ((bits & 0x0200) == 0)
                    context.RaiseFlags(ExceptionFlags.Invalid);
            }

            public override ushort CommonNaNToFloat16Bits(in SoftFloatCommonNaN commonNaN) => DefaultNaNFloat16Bits;

            public override ushort PropagateNaNFloat16Bits(SoftFloatContext context, uint bitsA, uint bitsB)
            {
                if (IsSignalingNaNFloat16Bits(bitsA) || IsSignalingNaNFloat16Bits(bitsB))
                    context.RaiseFlags(ExceptionFlags.Invalid);

                return DefaultNaNFloat16Bits;
            }

            #endregion

            #region Float32

            public override uint DefaultNaNFloat32Bits => 0x7FC00000;

            public override void Float32BitsToCommonNaN(SoftFloatContext context, uint bits, out SoftFloatCommonNaN commonNaN)
            {
                commonNaN = default;
                if ((bits & 0x00400000) == 0)
                    context.RaiseFlags(ExceptionFlags.Invalid);
            }

            public override uint CommonNaNToFloat32Bits(in SoftFloatCommonNaN commonNaN) => DefaultNaNFloat32Bits;

            public override uint PropagateNaNFloat32Bits(SoftFloatContext context, uint bitsA, uint bitsB)
            {
                if (IsSignalingNaNFloat32Bits(bitsA) || IsSignalingNaNFloat32Bits(bitsB))
                    context.RaiseFlags(ExceptionFlags.Invalid);

                return DefaultNaNFloat32Bits;
            }

            #endregion

            #region Float64

            public override ulong DefaultNaNFloat64Bits => 0x7FF8000000000000;

            public override void Float64BitsToCommonNaN(SoftFloatContext context, ulong bits, out SoftFloatCommonNaN commonNaN)
            {
                commonNaN = default;
                if ((bits & 0x0008000000000000) == 0)
                    context.RaiseFlags(ExceptionFlags.Invalid);
            }

            public override ulong CommonNaNToFloat64Bits(in SoftFloatCommonNaN commonNaN) => DefaultNaNFloat64Bits;

            public override ulong PropagateNaNFloat64Bits(SoftFloatContext context, ulong bitsA, ulong bitsB)
            {
                if (IsSignalingNaNFloat64Bits(bitsA) || IsSignalingNaNFloat64Bits(bitsB))
                    context.RaiseFlags(ExceptionFlags.Invalid);

                return DefaultNaNFloat64Bits;
            }

            #endregion

            #region ExtFloat80

            public override SFUInt128 DefaultNaNExtFloat80Bits => new(upper: 0x7FFF, lower: 0xC000000000000000);

            public override void ExtFloat80BitsToCommonNaN(SoftFloatContext context, uint bits64, ulong bits0, out SoftFloatCommonNaN commonNaN)
            {
                commonNaN = default;
                if ((bits0 & 0x4000000000000000) == 0)
                    context.RaiseFlags(ExceptionFlags.Invalid);
            }

            public override SFUInt128 CommonNaNToExtFloat80Bits(in SoftFloatCommonNaN commonNaN) => DefaultNaNExtFloat80Bits;

            public override SFUInt128 PropagateNaNExtFloat80Bits(SoftFloatContext context, uint bitsA64, ulong bitsA0, uint bitsB64, ulong bitsB0)
            {
                if (IsSignalingNaNExtFloat80Bits(bitsA64, bitsA0) || IsSignalingNaNExtFloat80Bits(bitsB64, bitsB0))
                    context.RaiseFlags(ExceptionFlags.Invalid);

                return DefaultNaNExtFloat80Bits;
            }

            #endregion

            #region Float128

            public override SFUInt128 DefaultNaNFloat128Bits => new(upper: 0x7FFF800000000000, lower: 0x0000000000000000);

            public override void Float128BitsToCommonNaN(SoftFloatContext context, ulong bits64, ulong bits0, out SoftFloatCommonNaN commonNaN)
            {
                commonNaN = default;
                if ((bits64 & 0x0000800000000000) == 0)
                    context.RaiseFlags(ExceptionFlags.Invalid);
            }

            public override SFUInt128 CommonNaNToFloat128Bits(in SoftFloatCommonNaN commonNaN) => DefaultNaNFloat128Bits;

            public override SFUInt128 PropagateNaNFloat128Bits(SoftFloatContext context, ulong bitsA64, ulong bitsA0, ulong bitsB64, ulong bitsB0)
            {
                if (IsSignalingNaNFloat128Bits(bitsA64, bitsA0) || IsSignalingNaNFloat128Bits(bitsB64, bitsB0))
                    context.RaiseFlags(ExceptionFlags.Invalid);

                return DefaultNaNFloat128Bits;
            }

            #endregion
        }
    }

}