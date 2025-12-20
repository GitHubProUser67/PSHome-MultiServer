#region Copyright
/*============================================================================

This is a C# port of the SoftFloat library release 3e by Thomas Kaiser (2022).
The copyright from the original source code is listed below.

This C source file is part of the SoftFloat IEEE Floating-Point Arithmetic
Package, Release 3e, by John R. Hauser.

Copyright 2011, 2012, 2013, 2014, 2015, 2016, 2017, 2018 The Regents of the
University of California.  All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met:

 1. Redistributions of source code must retain the above copyright notice,
    this list of conditions, and the following disclaimer.

 2. Redistributions in binary form must reproduce the above copyright notice,
    this list of conditions, and the following disclaimer in the documentation
    and/or other materials provided with the distribution.

 3. Neither the name of the University nor the names of its contributors may
    be used to endorse or promote products derived from this software without
    specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE REGENTS AND CONTRIBUTORS "AS IS", AND ANY
EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE, ARE
DISCLAIMED.  IN NO EVENT SHALL THE REGENTS OR CONTRIBUTORS BE LIABLE FOR ANY
DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
(INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

=============================================================================*/
#endregion

using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Tommunism.SoftFloat
{
    using static Primitives;

    partial class Internals
    {
        // signF16UI
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SignF16UI(uint a) => ((a >> 15) & 1) != 0;

        // expF16UI
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ExpF16UI(uint a) => (int)((a >> 10) & 0x1F);

        // fracF16UI
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint FracF16UI(uint a) => a & 0x03FF;

        // packToF16UI
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort PackToF16UI(bool sign, int exp, uint sig) =>
            (ushort)((sign ? (1U << 15) : 0U) + (ushort)((uint)exp << 10) + sig);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Float16 PackToF16(bool sign, int exp, uint sig) =>
            Float16.FromBitsUI16(PackToF16UI(sign, exp, sig));

        // isNaNF16UI
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNaNF16UI(uint a) => (~a & 0x7C00) == 0 && (a & 0x03FF) != 0;

        // softfloat_normSubnormalF16Sig
        public static (int exp, uint sig) NormSubnormalF16Sig(uint sig)
        {
            var shiftDist = CountLeadingZeroes16(sig) - 5;
            return (
                exp: 1 - shiftDist,
                sig: sig << shiftDist
            );
        }

        // softfloat_roundPackToF16
        public static Float16 RoundPackToF16(SoftFloatContext context, bool sign, int exp, uint sig)
        {
            var roundingMode = context.Rounding;
            var roundNearEven = roundingMode == RoundingMode.NearEven;
            var roundIncrement = (!roundNearEven && roundingMode != RoundingMode.NearMaxMag)
                ? ((roundingMode == (sign ? RoundingMode.Min : RoundingMode.Max)) ? 0xFU : 0)
                : 0x8U;
            var roundBits = sig & 0xF;

            if (0x1D <= (uint)exp)
            {
                if (exp < 0)
                {
                    var isTiny = context.DetectTininess == TininessMode.BeforeRounding || exp < -1 || sig + roundIncrement < 0x8000;
                    sig = ShiftRightJam32(sig, -exp);
                    exp = 0;
                    roundBits = sig & 0xF;

                    if (isTiny && roundBits != 0)
                        context.RaiseFlags(ExceptionFlags.Underflow);
                }
                else if (0x1D < exp || 0x8000 <= sig + roundIncrement)
                {
                    context.RaiseFlags(ExceptionFlags.Overflow | ExceptionFlags.Inexact);
                    return Float16.FromBitsUI16((ushort)(PackToF16UI(sign, 0x1F, 0) - (roundIncrement == 0 ? 1U : 0U)));
                }
            }

            sig = ((sig + roundIncrement) >> 4);
            if (roundBits != 0)
            {
                context.ExceptionFlags |= ExceptionFlags.Inexact;
                if (roundingMode == RoundingMode.Odd)
                {
                    sig |= 1;
                    return PackToF16(sign, exp, sig);
                }
            }

            sig &= ~(((roundBits ^ 8) == 0 & roundNearEven) ? 1U : 0U);
            if (sig == 0)
                exp = 0;

            return PackToF16(sign, exp, sig);
        }

        // softfloat_normRoundPackToF16
        public static Float16 NormRoundPackToF16(SoftFloatContext context, bool sign, int exp, uint sig)
        {
            var shiftDist = CountLeadingZeroes16(sig) - 1;
            exp -= shiftDist;
            if (4 <= shiftDist && (uint)exp < 0x1D)
            {
                return PackToF16(sign, sig != 0 ? exp : 0, sig << (shiftDist - 4));
            }
            else
            {
                return RoundPackToF16(context, sign, exp, sig << shiftDist);
            }
        }

        // softfloat_addMagsF16
        public static Float16 AddMagsF16(SoftFloatContext context, uint uiA, uint uiB)
        {
            int expA, expB, expDiff, expZ, shiftDist;
            uint sigA, sigB, sigZ, uiZ, sigX, sigY;
            uint sig32Z;
            bool signZ;

            expA = ExpF16UI(uiA);
            sigA = FracF16UI(uiA);
            expB = ExpF16UI(uiB);
            sigB = FracF16UI(uiB);

            expDiff = expA - expB;
            if (expDiff == 0)
            {
                if (expA == 0)
                    return Float16.FromBitsUI16((ushort)(uiA + sigB));

                if (expA == 0x1F)
                {
                    if ((sigA | sigB) != 0)
                        return context.PropagateNaNFloat16(uiA, uiB);

                    return Float16.FromBitsUI16((ushort)uiA);
                }

                signZ = SignF16UI(uiA);
                expZ = expA;
                sigZ = 0x0800 + sigA + sigB;
                if ((sigZ & 1) == 0 && expZ < 0x1E)
                    return PackToF16(signZ, expZ, sigZ >> 1);

                sigZ <<= 3;
            }
            else
            {
                signZ = SignF16UI(uiA);
                if (expDiff < 0)
                {
                    if (expB == 0x1F)
                    {
                        if (sigB != 0)
                            return context.PropagateNaNFloat16(uiA, uiB);

                        return PackToF16(signZ, 0x1F, 0);
                    }

                    if (expDiff <= -13)
                    {
                        uiZ = PackToF16UI(signZ, expB, sigB);
                        if (((uint)expA | sigA) != 0)
                            goto addEpsilon;

                        return Float16.FromBitsUI16((ushort)uiZ);
                    }

                    expZ = expB;
                    sigX = sigB | 0x0400;
                    sigY = sigA + (expA != 0 ? 0x0400 : sigA);
                    shiftDist = 19 + expDiff;
                }
                else
                {
                    uiZ = uiA;
                    if (expA == 0x1F)
                    {
                        if (sigA != 0)
                            return context.PropagateNaNFloat16(uiA, uiB);

                        return Float16.FromBitsUI16((ushort)uiZ);
                    }

                    if (13 <= expDiff)
                    {
                        if (((uint)expB | sigB) != 0)
                            goto addEpsilon;

                        return Float16.FromBitsUI16((ushort)uiZ);
                    }

                    expZ = expA;
                    sigX = sigA | 0x0400;
                    sigY = sigB + (expB != 0 ? 0x0400 : sigB);
                    shiftDist = 19 - expDiff;
                }

                sig32Z = (sigX << 19) + (sigY << shiftDist);
                if (sig32Z < 0x40000000)
                {
                    --expZ;
                    sig32Z <<= 1;
                }

                sigZ = sig32Z >> 16;
                if ((sig32Z & 0xFFFF) != 0)
                {
                    sigZ |= 1;
                }
                else
                {
                    if ((sigZ & 0xF) == 0 && expZ < 0x1E)
                    {
                        sigZ >>= 4;
                        return PackToF16(signZ, expZ, sigZ);
                    }
                }
            }

            return RoundPackToF16(context, signZ, expZ, sigZ);

        addEpsilon:
            var roundingMode = context.Rounding;
            if (roundingMode != RoundingMode.NearEven)
            {
                if (roundingMode == (SignF16UI(uiZ) ? RoundingMode.Min : RoundingMode.Max))
                {
                    ++uiZ;
                    if ((ushort)(uiZ << 1) == 0xF800U)
                        context.RaiseFlags(ExceptionFlags.Overflow | ExceptionFlags.Inexact);
                }
                else if (roundingMode == RoundingMode.Odd)
                {
                    uiZ |= 1;
                }
            }

            context.ExceptionFlags |= ExceptionFlags.Inexact;
            return Float16.FromBitsUI16((ushort)uiZ);
        }

        // softfloat_subMagsF16
        public static Float16 SubMagsF16(SoftFloatContext context, uint uiA, uint uiB)
        {
            int expA, expB, expDiff, expZ, shiftDist;
            uint sigA, sigB, uiZ, sigZ, sigX, sigY;
            int sigDiff;
            bool signZ;

            expA = ExpF16UI(uiA);
            sigA = FracF16UI(uiA);
            expB = ExpF16UI(uiB);
            sigB = FracF16UI(uiB);

            expDiff = expA - expB;
            if (expDiff == 0)
            {
                if (expA == 0x1F)
                {
                    if ((sigA | sigB) != 0)
                        return context.PropagateNaNFloat16(uiA, uiB);

                    context.RaiseFlags(ExceptionFlags.Invalid);
                    return context.DefaultNaNFloat16;
                }

                sigDiff = (int)sigA - (int)sigB;
                if (sigDiff == 0)
                    return PackToF16(context.Rounding == RoundingMode.Min, 0, 0);

                if (expA != 0)
                    --expA;

                signZ = SignF16UI(uiA);
                if (sigDiff < 0)
                {
                    signZ = !signZ;
                    sigDiff = -sigDiff;
                }

                Debug.Assert(sigDiff >= 0);
                shiftDist = CountLeadingZeroes16((uint)sigDiff) - 5;
                expZ = expA - shiftDist;
                if (expZ < 0)
                {
                    shiftDist = expA;
                    expZ = 0;
                }

                return PackToF16(signZ, expZ, (uint)sigDiff << shiftDist);
            }
            else
            {
                signZ = SignF16UI(uiA);
                if (expDiff < 0)
                {
                    signZ = !signZ;
                    if (expB == 0x1F)
                    {
                        if (sigB != 0)
                            return context.PropagateNaNFloat16(uiA, uiB);

                        return PackToF16(signZ, 0x1F, 0);
                    }

                    if (expDiff <= -13)
                    {
                        uiZ = PackToF16UI(signZ, expB, sigB);
                        if (((uint)expA | sigA) != 0)
                            goto subEpsilon;

                        return Float16.FromBitsUI16((ushort)uiZ);
                    }

                    expZ = expA + 19;
                    sigX = sigB | 0x0400;
                    sigY = sigA + (expA != 0 ? 0x0400 : sigA);
                    expDiff = -expDiff;
                }
                else
                {
                    uiZ = uiA;
                    if (expA == 0x1F)
                    {
                        if (sigA != 0)
                            return context.PropagateNaNFloat16(uiA, uiB);

                        return Float16.FromBitsUI16((ushort)uiZ);
                    }

                    if (13 <= expDiff)
                    {
                        if (((uint)expB | sigB) != 0)
                            goto subEpsilon;

                        return Float16.FromBitsUI16((ushort)uiZ);
                    }

                    expZ = expB + 19;
                    sigX = sigA | 0x0400;
                    sigY = sigB + (expB != 0 ? 0x0400 : sigB);
                }

                uint sig32Z = (sigX << expDiff) - sigY;
                shiftDist = CountLeadingZeroes32(sig32Z) - 1;
                sig32Z <<= shiftDist;
                expZ -= shiftDist;
                sigZ = sig32Z >> 16;
                if ((sig32Z & 0xFFFF) != 0)
                {
                    sigZ |= 1;
                }
                else
                {
                    if ((sigZ & 0xF) == 0 && (uint)expZ < 0x1E)
                    {
                        sigZ >>= 4;
                        return PackToF16(signZ, expZ, sigZ);
                    }
                }

                return RoundPackToF16(context, signZ, expZ, sigZ);
            }

        subEpsilon:
            var roundingMode = context.Rounding;
            if (roundingMode != RoundingMode.NearEven)
            {
                if (roundingMode == RoundingMode.MinMag || (roundingMode == (SignF16UI(uiZ) ? RoundingMode.Max : RoundingMode.Min)))
                {
                    --uiZ;
                }
                else if (roundingMode == RoundingMode.Odd)
                {
                    uiZ = (uiZ - 1) | 1;
                }
            }

            context.ExceptionFlags |= ExceptionFlags.Inexact;
            return Float16.FromBitsUI16((ushort)uiZ);
        }

        // softfloat_mulAddF16
        public static Float16 MulAddF16(SoftFloatContext context, uint uiA, uint uiB, uint uiC, MulAddOperation op)
        {
            Debug.Assert(op is MulAddOperation.None or MulAddOperation.SubtractC or MulAddOperation.SubtractProduct, "Invalid MulAdd operation.");

            bool signA, signB, signC, signProd, signZ;
            int expA, expB, expC, expProd, expZ, expDiff, shiftDist;
            uint sigA, sigB, sigC, magBits, uiZ, sigZ;
            uint sigProd, sig32Z, sig32C;

            signA = SignF16UI(uiA);
            expA = ExpF16UI(uiA);
            sigA = FracF16UI(uiA);

            signB = SignF16UI(uiB);
            expB = ExpF16UI(uiB);
            sigB = FracF16UI(uiB);

            signC = SignF16UI(uiC) ^ (op == MulAddOperation.SubtractC);
            expC = ExpF16UI(uiC);
            sigC = FracF16UI(uiC);

            signProd = signA ^ signB ^ (op == MulAddOperation.SubtractProduct);

            if (expA == 0x1F)
            {
                if (sigA != 0 || (expB == 0x1F && sigB != 0))
                    return context.PropagateNaNFloat16(uiA, uiB, uiC);

                magBits = (uint)expB | sigB;
                goto infProdArg;
            }

            if (expB == 0x1F)
            {
                if (sigB != 0)
                    return context.PropagateNaNFloat16(uiA, uiB, uiC);

                magBits = (uint)expA | sigA;
                goto infProdArg;
            }

            if (expC == 0x1F)
            {
                if (sigC != 0)
                    return context.PropagateNaNFloat16(0, uiC);

                return Float16.FromBitsUI16((ushort)uiC);
            }

            if (expA == 0)
            {
                if (sigA == 0)
                {
                    if (((uint)expC | sigC) == 0 && signProd != signC)
                        return PackToF16(context.Rounding == RoundingMode.Min, 0, 0);

                    return Float16.FromBitsUI16((ushort)uiC);
                }

                (expA, sigA) = NormSubnormalF16Sig(sigA);
            }

            if (expB == 0)
            {
                if (sigB == 0)
                {
                    if (((uint)expC | sigC) == 0 && signProd != signC)
                        return PackToF16(context.Rounding == RoundingMode.Min, 0, 0);

                    return Float16.FromBitsUI16((ushort)uiC);
                }

                (expB, sigB) = NormSubnormalF16Sig(sigB);
            }

            expProd = expA + expB - 0xE;
            sigA = (sigA | 0x0400) << 4;
            sigB = (sigB | 0x0400) << 4;
            sigProd = sigA * sigB;

            if (sigProd < 0x20000000)
            {
                --expProd;
                sigProd <<= 1;
            }

            signZ = signProd;
            if (expC == 0)
            {
                if (sigC == 0)
                {
                    expZ = expProd - 1;
                    sigZ = (sigProd >> 15) | ((sigProd & 0x7FFF) != 0 ? 1U : 0U);
                    return RoundPackToF16(context, signZ, expZ, sigZ);
                }

                (expC, sigC) = NormSubnormalF16Sig(sigC);
            }

            sigC = (sigC | 0x0400) << 3;
            expDiff = expProd - expC;

            if (signProd == signC)
            {
                if (expDiff <= 0)
                {
                    expZ = expC;
                    sigZ = sigC + ShiftRightJam32(sigProd, 16 - expDiff);
                }
                else
                {
                    expZ = expProd;
                    sig32Z = sigProd + ShiftRightJam32(sigC << 16, expDiff);
                    sigZ = (sig32Z >> 16) | ((sig32Z & 0xFFFF) != 0 ? 1U : 0U);
                }

                if (sigZ < 0x4000)
                {
                    --expZ;
                    sigZ <<= 1;
                }
            }
            else
            {
                sig32C = sigC << 16;
                if (expDiff < 0)
                {
                    signZ = signC;
                    expZ = expC;
                    sig32Z = sig32C - ShiftRightJam32(sigProd, -expDiff);
                }
                else if (expDiff == 0)
                {
                    expZ = expProd;
                    sig32Z = sigProd - sig32C;
                    if (sig32Z == 0)
                        return PackToF16(context.Rounding == RoundingMode.Min, 0, 0);

                    if ((sig32Z & 0x80000000) != 0)
                    {
                        signZ = !signZ;
                        sig32Z = (uint)(-(int)sig32Z);
                    }
                }
                else
                {
                    expZ = expProd;
                    sig32Z = sigProd - ShiftRightJam32(sig32C, expDiff);
                }

                shiftDist = CountLeadingZeroes32(sig32Z) - 1;
                expZ -= shiftDist;
                shiftDist -= 16;
                sigZ = (shiftDist < 0)
                    ? (sig32Z >> (-shiftDist)) | ((sig32Z << (shiftDist & 31)) != 0 ? 1U : 0U)
                    : sig32Z << shiftDist;
            }

            return RoundPackToF16(context, signZ, expZ, sigZ);

        infProdArg:
            if (magBits != 0)
            {
                uiZ = PackToF16UI(signProd, 0x1F, 0);
                if (expC != 0x1F)
                    return Float16.FromBitsUI16((ushort)uiZ);

                if (sigC != 0)
                    return context.PropagateNaNFloat16(uiZ, uiC);

                if (signProd == signC)
                    return Float16.FromBitsUI16((ushort)uiZ);
            }

            context.RaiseFlags(ExceptionFlags.Invalid);
            return context.PropagateNaNFloat16(context.DefaultNaNFloat16Bits, uiC);
        }
    }

}