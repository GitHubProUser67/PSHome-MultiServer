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
        // signExtF80UI64
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SignExtF80UI64(uint a64) => (a64 >> 15) != 0;

        // expExtF80UI64
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ExpExtF80UI64(uint a64) => (int)(a64 & 0x7FFF);

        // packToExtF80UI64
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort PackToExtF80UI64(bool sign, int exp) =>
            (ushort)((sign ? (1U << 15) : 0U) | (uint)exp);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ExtFloat80 PackToExtF80(bool sign, int exp, ulong sig) =>
            ExtFloat80.FromBitsUI80(PackToExtF80UI64(sign, exp), sig);

        // isNaNExtF80UI
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNaNExtF80UI(int a64, ulong a0) => ((a64 & 0x7FFF) == 0x7FFF) && (a0 & 0x7FFFFFFFFFFFFFFF) != 0;

        // softfloat_normSubnormalExtF80Sig
        public static (int exp, ulong sig) NormSubnormalExtF80Sig(ulong sig)
        {
            var shiftDist = CountLeadingZeroes64(sig);
            return (
                exp: -shiftDist,
                sig: sig << shiftDist
            );
        }

        // softfloat_roundPackToExtF80
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ExtFloat80 RoundPackToExtF80(SoftFloatContext context, bool sign, int exp, ulong sig, ulong sigExtra, ExtFloat80RoundingPrecision roundingPrecision)
        {
            Debug.Assert(roundingPrecision is ExtFloat80RoundingPrecision._32 or ExtFloat80RoundingPrecision._64 or ExtFloat80RoundingPrecision._80, "Unexpected rounding precision.");
            return roundingPrecision switch
            {
                ExtFloat80RoundingPrecision._32 => RoundPackToExtF80Impl32Or64(context, sign, exp, sig, sigExtra, 0x0000008000000000, 0x000000FFFFFFFFFF),
                ExtFloat80RoundingPrecision._64 => RoundPackToExtF80Impl32Or64(context, sign, exp, sig, sigExtra, 0x0000000000000400, 0x00000000000007FF),
                _ => RoundPackToExtF80Impl80(context, sign, exp, sig, sigExtra),
            };
        }

        // Called when rounding precision is 32 or 64.
        private static ExtFloat80 RoundPackToExtF80Impl32Or64(SoftFloatContext context, bool sign, int exp, ulong sig, ulong sigExtra, ulong roundIncrement, ulong roundMask)
        {
            ulong roundBits;

            var roundingMode = context.Rounding;
            var roundNearEven = (roundingMode == RoundingMode.NearEven);

            sig |= sigExtra != 0 ? 1UL : 0;
            roundBits = sig & roundMask;
            if (!roundNearEven && roundingMode != RoundingMode.NearMaxMag)
                roundIncrement = (roundingMode == (sign ? RoundingMode.Min : RoundingMode.Max)) ? roundMask : 0;

            if (0x7FFD <= (uint)(exp - 1))
            {
                if (exp <= 0)
                {
                    var isTiny = context.DetectTininess == TininessMode.BeforeRounding || exp < 0 || sig <= sig + roundIncrement;
                    sig = ShiftRightJam64(sig, 1 - exp);
                    roundBits = sig & roundMask;
                    if (roundBits != 0)
                    {
                        if (isTiny)
                            context.RaiseFlags(ExceptionFlags.Underflow);

                        context.ExceptionFlags |= ExceptionFlags.Inexact;
                        if (roundingMode == RoundingMode.Odd)
                            sig |= roundMask + 1;
                    }

                    sig += roundIncrement;
                    exp = (sig & 0x8000000000000000) != 0 ? 1 : 0;
                    roundIncrement = roundMask + 1;
                    if (roundNearEven && (roundBits << 1) == roundIncrement)
                        roundMask |= roundIncrement;

                    sig &= ~roundMask;
                    return PackToExtF80(sign, exp, sig);
                }

                if (0x7FFE < exp || (exp == 0x7FFE && sig + roundIncrement < sig))
                {
                    context.RaiseFlags(ExceptionFlags.Overflow | ExceptionFlags.Inexact);
                    if (roundNearEven || roundingMode == RoundingMode.NearMaxMag ||
                        roundingMode == (sign ? RoundingMode.Min : RoundingMode.Max))
                    {
                        exp = 0x7FFF;
                        sig = 0x8000000000000000;
                    }
                    else
                    {
                        exp = 0x7FFE;
                        sig = ~roundMask;
                    }

                    return PackToExtF80(sign, exp, sig);
                }
            }

            if (roundBits != 0)
            {
                context.ExceptionFlags |= ExceptionFlags.Inexact;
                if (roundingMode == RoundingMode.Odd)
                {
                    sig = (sig & ~roundMask) | (roundMask + 1);
                    return PackToExtF80(sign, exp, sig);
                }
            }

            sig += roundIncrement;
            if (sig < roundIncrement)
            {
                ++exp;
                sig = 0x8000000000000000;
            }

            roundIncrement = roundMask + 1;
            if (roundNearEven && (roundBits << 1) == roundIncrement)
                roundMask |= roundIncrement;

            sig &= ~roundMask;
            return PackToExtF80(sign, exp, sig);
        }

        // Called when rounding precision is 80 (or anything except 32 or 64).
        private static ExtFloat80 RoundPackToExtF80Impl80(SoftFloatContext context, bool sign, int exp, ulong sig, ulong sigExtra)
        {
            var roundingMode = context.Rounding;
            var roundNearEven = (roundingMode == RoundingMode.NearEven);
            var roundIncrement = (!roundNearEven && roundingMode != RoundingMode.NearMaxMag)
                ? (roundingMode == (sign ? RoundingMode.Min : RoundingMode.Max) && sigExtra != 0)
                : (0x8000000000000000 <= sigExtra);

            if (0x7FFD <= (uint)(exp - 1))
            {
                if (exp <= 0)
                {
                    var isTiny = context.DetectTininess == TininessMode.BeforeRounding || exp < 0 || !roundIncrement || sig < 0xFFFFFFFFFFFFFFFF;
                    (sigExtra, sig) = ShiftRightJam64Extra(sig, sigExtra, 1 - exp);
                    exp = 0;
                    if (sigExtra != 0)
                    {
                        if (isTiny)
                            context.RaiseFlags(ExceptionFlags.Underflow);

                        context.ExceptionFlags |= ExceptionFlags.Inexact;
                        if (roundingMode == RoundingMode.Odd)
                        {
                            sig |= 1;
                            return PackToExtF80(sign, exp, sig);
                        }
                    }

                    roundIncrement = (!roundNearEven && roundingMode != RoundingMode.NearMaxMag)
                        ? (roundingMode == (sign ? RoundingMode.Min : RoundingMode.Max) && sigExtra != 0)
                        : (0x8000000000000000 <= sigExtra);
                    if (roundIncrement)
                    {
                        ++sig;
                        sig &= ~((sigExtra & 0x7FFFFFFFFFFFFFFF) == 0 & roundNearEven ? 1UL : 0);
                        exp = (sig & 0x8000000000000000) != 0 ? 1 : 0;
                    }

                    return PackToExtF80(sign, exp, sig);
                }

                if (0x7FFE < exp || (exp == 0x7FFE && sig == 0xFFFFFFFFFFFFFFFF && roundIncrement))
                {
                    context.RaiseFlags(ExceptionFlags.Overflow | ExceptionFlags.Inexact);
                    if (roundNearEven || roundingMode == RoundingMode.NearMaxMag ||
                        roundingMode == (sign ? RoundingMode.Min : RoundingMode.Max))
                    {
                        exp = 0x7FFF;
                        sig = 0x8000000000000000;
                    }
                    else
                    {
                        exp = 0x7FFE;
                        sig = ~0UL;
                    }

                    return PackToExtF80(sign, exp, sig);
                }
            }

            if (sigExtra != 0)
            {
                context.ExceptionFlags |= ExceptionFlags.Inexact;
                if (roundingMode == RoundingMode.Odd)
                    return PackToExtF80(sign, exp, sig | 1);
            }

            if (roundIncrement)
            {
                ++sig;
                if (sig == 0)
                {
                    ++exp;
                    sig = 0x8000000000000000;
                }
                else
                {
                    sig &= ~((sigExtra & 0x7FFFFFFFFFFFFFFF) == 0 & roundNearEven ? 1UL : 0);
                }
            }

            return PackToExtF80(sign, exp, sig);
        }

        // softfloat_normRoundPackToExtF80
        public static ExtFloat80 NormRoundPackToExtF80(SoftFloatContext context, bool sign, int exp, ulong sig, ulong sigExtra, ExtFloat80RoundingPrecision roundingPrecision)
        {
            if (sig == 0)
            {
                exp -= 64;
                sig = sigExtra;
                sigExtra = 0;
            }

            var shiftDist = CountLeadingZeroes64(sig);
            exp -= shiftDist;
            if (shiftDist != 0)
                (sig, sigExtra) = ShortShiftLeft128(sig, sigExtra, shiftDist);

            return RoundPackToExtF80(context, sign, exp, sig, sigExtra, roundingPrecision);
        }

        // softfloat_addMagsExtF80
        public static ExtFloat80 AddMagsExtF80(SoftFloatContext context, uint uiA64, ulong uiA0, uint uiB64, ulong uiB0, bool signZ)
        {
            int expA, expB, expDiff, expZ;
            ulong sigA, sigB, sigZ, sigZExtra;

            expA = ExpExtF80UI64(uiA64);
            sigA = uiA0;
            expB = ExpExtF80UI64(uiB64);
            sigB = uiB0;

            expDiff = expA - expB;
            if (expDiff == 0)
            {
                if (expA == 0x7FFF)
                {
                    return (((sigA | sigB) & 0x7FFFFFFFFFFFFFFF) != 0)
                        ? context.PropagateNaNExtFloat80Bits(uiA64, uiA0, uiB64, uiB0)
                        : ExtFloat80.FromBitsUI80((ushort)uiA64, uiA0);
                }

                sigZ = sigA + sigB;
                sigZExtra = 0;
                if (expA == 0)
                {
                    (expZ, sigZ) = NormSubnormalExtF80Sig(sigZ);
                    expZ++;
                    return RoundPackToExtF80(context, signZ, expZ, sigZ, sigZExtra, context.RoundingPrecisionExtFloat80);
                }

                expZ = expA;
            }
            else
            {
                if (expDiff < 0)
                {
                    if (expB == 0x7FFF)
                    {
                        return ((sigB & 0x7FFFFFFFFFFFFFFF) != 0)
                            ? context.PropagateNaNExtFloat80Bits(uiA64, uiA0, uiB64, uiB0)
                            : PackToExtF80(signZ, 0x7FFF, uiB0);
                    }

                    expZ = expB;
                    if (expA == 0)
                    {
                        ++expDiff;
                        sigZExtra = 0;
                        if (expDiff == 0)
                            goto newlyAligned;
                    }

                    (sigZExtra, sigA) = ShiftRightJam64Extra(sigA, 0, -expDiff);
                }
                else
                {
                    if (expA == 0x7FFF)
                    {
                        return ((sigA & 0x7FFFFFFFFFFFFFFF) != 0)
                            ? context.PropagateNaNExtFloat80Bits(uiA64, uiA0, uiB64, uiB0)
                            : ExtFloat80.FromBitsUI80((ushort)uiA64, uiA0);
                    }

                    expZ = expA;
                    if (expB == 0)
                    {
                        --expDiff;
                        sigZExtra = 0;
                        if (expDiff == 0)
                            goto newlyAligned;
                    }

                    (sigZExtra, sigB) = ShiftRightJam64Extra(sigB, 0, expDiff);
                }

            newlyAligned:
                sigZ = sigA + sigB;
                if ((sigZ & 0x8000000000000000) != 0)
                    return RoundPackToExtF80(context, signZ, expZ, sigZ, sigZExtra, context.RoundingPrecisionExtFloat80);
            }

            (sigZExtra, sigZ) = ShortShiftRightJam64Extra(sigZ, sigZExtra, 1);
            sigZ |= 0x8000000000000000;
            ++expZ;
            return RoundPackToExtF80(context, signZ, expZ, sigZ, sigZExtra, context.RoundingPrecisionExtFloat80);
        }

        // softfloat_subMagsExtF80
        public static ExtFloat80 SubMagsExtF80(SoftFloatContext context, uint uiA64, ulong uiA0, uint uiB64, ulong uiB0, bool signZ)
        {
            int expA, expB, expDiff, expZ;
            ulong sigA, sigB, sigExtra;
            SFUInt128 sig128;

            expA = ExpExtF80UI64(uiA64);
            sigA = uiA0;
            expB = ExpExtF80UI64(uiB64);
            sigB = uiB0;

            expDiff = expA - expB;
            if (expDiff == 0)
            {
                if (expA == 0x7FFF)
                {
                    if (((sigA | sigB) & 0x7FFFFFFFFFFFFFFF) != 0)
                        return context.PropagateNaNExtFloat80Bits(uiA64, uiA0, uiB64, uiB0);

                    context.RaiseFlags(ExceptionFlags.Invalid);
                    return context.DefaultNaNExtFloat80;
                }

                expZ = expA;
                if (expZ == 0)
                    expZ = 1;

                sigExtra = 0;

                if (sigB < sigA)
                {
                    sig128 = Sub128(sigA, 0, sigB, sigExtra);
                }
                else if (sigA < sigB)
                {
                    signZ = !signZ;
                    sig128 = Sub128(sigB, 0, sigA, sigExtra);
                }
                else
                {
                    return PackToExtF80(context.Rounding == RoundingMode.Min, 0, 0);
                }
            }
            else if (0 < expDiff)
            {
                if (expA == 0x7FFF)
                {
                    return ((sigA & 0x7FFFFFFFFFFFFFFF) != 0)
                        ? context.PropagateNaNExtFloat80Bits(uiA64, uiA0, uiB64, uiB0)
                        : ExtFloat80.FromBitsUI80((ushort)uiA64, uiA0);
                }

                if (expB == 0)
                {
                    --expDiff;
                    sigExtra = 0;
                    if (expDiff != 0)
                        (sigB, sigExtra) = ShiftRightJam128(sigB, 0, expDiff);
                }
                else
                {
                    (sigB, sigExtra) = ShiftRightJam128(sigB, 0, expDiff);
                }

                expZ = expA;
                sig128 = Sub128(sigA, 0, sigB, sigExtra);
            }
            else //if (expDiff < 0)
            {
                if (expB == 0x7FFF)
                {
                    return ((sigB & 0x7FFFFFFFFFFFFFFF) != 0)
                        ? context.PropagateNaNExtFloat80Bits(uiA64, uiA0, uiB64, uiB0)
                        : PackToExtF80(!signZ, 0x7FFF, 0x8000000000000000);
                }

                if (expA == 0)
                {
                    ++expDiff;
                    sigExtra = 0;
                    if (expDiff != 0)
                        (sigA, sigExtra) = ShiftRightJam128(sigA, 0, -expDiff);
                }
                else
                {
                    (sigA, sigExtra) = ShiftRightJam128(sigA, 0, -expDiff);
                }

                signZ = !signZ;
                expZ = expB;
                sig128 = Sub128(sigB, 0, sigA, sigExtra);
            }

            return NormRoundPackToExtF80(context, signZ, expZ, sig128.V64, sig128.V00, context.RoundingPrecisionExtFloat80);
        }
    }

}