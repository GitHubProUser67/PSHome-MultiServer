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
        // signF64UI
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SignF64UI(ulong a) => (a >> 63) != 0;

        // expF64UI
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ExpF64UI(ulong a) => (int)((uint)(a >> 52) & 0x7FF);

        // fracF64UI
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong FracF64UI(ulong a) => a & 0x000FFFFFFFFFFFFF;

        // packToF64UI
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong PackToF64UI(bool sign, int exp, ulong sig) =>
            (sign ? (1UL << 63) : 0UL) + ((ulong)exp << 52) + sig;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Float64 PackToF64(bool sign, int exp, ulong sig) =>
            Float64.FromBitsUI64(PackToF64UI(sign, exp, sig));

        // isNaNF64UI
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNaNF64UI(ulong a) => (~a & 0x7FF0000000000000) == 0 && (a & 0x000FFFFFFFFFFFFF) != 0;

        // softfloat_normSubnormalF64Sig
        public static (int exp, ulong sig) NormSubnormalF64Sig(ulong sig)
        {
            var shiftDist = CountLeadingZeroes64(sig) - 11;
            return (
                exp: 1 - shiftDist,
                sig: sig << shiftDist
            );
        }

        // softfloat_roundPackToF64
        public static Float64 RoundPackToF64(SoftFloatContext context, bool sign, int exp, ulong sig)
        {
            var roundingMode = context.Rounding;
            var roundNearEven = roundingMode == RoundingMode.NearEven;
            var roundIncrement = (!roundNearEven && roundingMode != RoundingMode.NearMaxMag)
                ? ((roundingMode == (sign ? RoundingMode.Min : RoundingMode.Max)) ? 0x3FFU : 0)
                : 0x200U;
            var roundBits = sig & 0x3FF;

            if (0x7FD <= (uint)exp)
            {
                if (exp < 0)
                {
                    var isTiny = context.DetectTininess == TininessMode.BeforeRounding || exp < -1 || sig + roundIncrement < 0x8000000000000000;
                    sig = ShiftRightJam64(sig, -exp);
                    exp = 0;
                    roundBits = sig & 0x3FF;

                    if (isTiny && roundBits != 0)
                        context.RaiseFlags(ExceptionFlags.Underflow);
                }
                else if (0x7FD < exp || 0x8000000000000000 <= sig + roundIncrement)
                {
                    context.RaiseFlags(ExceptionFlags.Overflow | ExceptionFlags.Inexact);
                    return Float64.FromBitsUI64(PackToF64UI(sign, 0x7FF, 0) - (roundIncrement == 0 ? 1UL : 0));
                }
            }

            sig = (sig + roundIncrement) >> 10;
            if (roundBits != 0)
            {
                context.ExceptionFlags |= ExceptionFlags.Inexact;
                if (roundingMode == RoundingMode.Odd)
                    return PackToF64(sign, exp, sig | 1);
            }

            sig &= ~(((roundBits ^ 0x200) == 0 & roundNearEven) ? 1UL : 0);
            if (sig == 0)
                exp = 0;

            return PackToF64(sign, exp, sig);
        }

        // softfloat_normRoundPackToF64
        public static Float64 NormRoundPackToF64(SoftFloatContext context, bool sign, int exp, ulong sig)
        {
            var shiftDist = CountLeadingZeroes64(sig) - 1;
            exp -= shiftDist;
            return (10 <= shiftDist && ((uint)exp < 0x7FD))
                ? PackToF64(sign, sig != 0 ? exp : 0, sig << (shiftDist - 10))
                : RoundPackToF64(context, sign, exp, sig << shiftDist);
        }

        // softfloat_addMagsF64
        public static Float64 AddMagsF64(SoftFloatContext context, ulong uiA, ulong uiB, bool signZ)
        {
            int expA, expB, expDiff, expZ;
            ulong sigA, sigB, sigZ;

            expA = ExpF64UI(uiA);
            sigA = FracF64UI(uiA);
            expB = ExpF64UI(uiB);
            sigB = FracF64UI(uiB);

            expDiff = expA - expB;
            if (expDiff == 0)
            {
                if (expA == 0)
                    return Float64.FromBitsUI64(uiA + sigB);

                if (expA == 0x7FF)
                {
                    return ((sigA | sigB) != 0)
                        ? context.PropagateNaNFloat64Bits(uiA, uiB)
                        : Float64.FromBitsUI64(uiA);
                }

                expZ = expA;
                sigZ = 0x0020000000000000 + sigA + sigB;
                sigZ <<= 9;
            }
            else
            {
                sigA <<= 9;
                sigB <<= 9;
                if (expDiff < 0)
                {
                    if (expB == 0x7FF)
                    {
                        return (sigB != 0)
                            ? context.PropagateNaNFloat64Bits(uiA, uiB)
                            : PackToF64(signZ, 0x7FF, 0);
                    }

                    expZ = expB;
                    sigA = ShiftRightJam64((expA != 0) ? (sigA + 0x2000000000000000) : (sigA << 1), -expDiff);
                }
                else
                {
                    if (expA == 0x7FF)
                    {
                        return (sigA != 0)
                            ? context.PropagateNaNFloat64Bits(uiA, uiB)
                            : Float64.FromBitsUI64(uiA);
                    }

                    expZ = expA;
                    sigB = ShiftRightJam64((expB != 0) ? (sigB + 0x2000000000000000) : (sigB << 1), expDiff);
                }

                sigZ = 0x2000000000000000 + sigA + sigB;
                if (sigZ < 0x4000000000000000)
                {
                    --expZ;
                    sigZ <<= 1;
                }
            }

            return RoundPackToF64(context, signZ, expZ, sigZ);
        }

        // softfloat_subMagsF64
        public static Float64 SubMagsF64(SoftFloatContext context, ulong uiA, ulong uiB, bool signZ)
        {
            int expA, expB, expDiff, expZ;
            ulong sigA, sigB, sigZ;
            long sigDiff;
            int shiftDist;

            expA = ExpF64UI(uiA);
            sigA = FracF64UI(uiA);
            expB = ExpF64UI(uiB);
            sigB = FracF64UI(uiB);

            expDiff = expA - expB;
            if (expDiff == 0)
            {
                if (expA == 0x7FF)
                {
                    if ((sigA | sigB) != 0)
                        return context.PropagateNaNFloat64Bits(uiA, uiB);

                    context.RaiseFlags(ExceptionFlags.Invalid);
                    return context.DefaultNaNFloat64;
                }

                sigDiff = (long)sigA - (long)sigB;
                if (sigDiff == 0)
                    return PackToF64(context.Rounding == RoundingMode.Min, 0, 0);

                if (expA != 0)
                    --expA;

                if (sigDiff < 0)
                {
                    signZ = !signZ;
                    sigDiff = -sigDiff;
                }

                Debug.Assert(sigDiff >= 0);
                shiftDist = CountLeadingZeroes64((ulong)sigDiff) - 11;
                expZ = expA - shiftDist;
                if (expZ < 0)
                {
                    shiftDist = expA;
                    expZ = 0;
                }

                return PackToF64(signZ, expZ, (ulong)sigDiff << shiftDist);
            }
            else
            {
                sigA <<= 10;
                sigB <<= 10;
                if (expDiff < 0)
                {
                    signZ = !signZ;
                    if (expB == 0x7FF)
                    {
                        return (sigB != 0)
                            ? context.PropagateNaNFloat64Bits(uiA, uiB)
                            : PackToF64(signZ, 0x7FF, 0);
                    }

                    sigA += expA != 0 ? 0x4000000000000000 : sigA;
                    sigA = ShiftRightJam64(sigA, -expDiff);
                    sigB |= 0x4000000000000000;
                    expZ = expB;
                    sigZ = sigB - sigA;
                }
                else
                {
                    if (expA == 0x7FF)
                    {
                        return (sigA != 0)
                            ? context.PropagateNaNFloat64Bits(uiA, uiB)
                            : Float64.FromBitsUI64(uiA);
                    }

                    sigB += expB != 0 ? 0x4000000000000000 : sigB;
                    sigB = ShiftRightJam64(sigB, expDiff);
                    sigA |= 0x4000000000000000;
                    expZ = expA;
                    sigZ = sigA - sigB;
                }

                return NormRoundPackToF64(context, signZ, expZ - 1, sigZ);
            }
        }

        // softfloat_mulAddF64
        public static Float64 MulAddF64(SoftFloatContext context, ulong uiA, ulong uiB, ulong uiC, MulAddOperation op)
        {
            Debug.Assert(op is MulAddOperation.None or MulAddOperation.SubtractC or MulAddOperation.SubtractProduct, "Invalid MulAdd operation.");

            bool signA, signB, signC, signZ;
            int expA, expB, expC, expZ, expDiff;
            ulong sigA, sigB, sigC, magBits, uiZ, sigZ;
            SFUInt128 sig128Z, sig128C;
            int shiftDist;

            signA = SignF64UI(uiA);
            expA = ExpF64UI(uiA);
            sigA = FracF64UI(uiA);

            signB = SignF64UI(uiB);
            expB = ExpF64UI(uiB);
            sigB = FracF64UI(uiB);

            signC = SignF64UI(uiC) ^ (op == MulAddOperation.SubtractC);
            expC = ExpF64UI(uiC);
            sigC = FracF64UI(uiC);

            signZ = signA ^ signB ^ (op == MulAddOperation.SubtractProduct);

            if (expA == 0x7FF)
            {
                if (sigA != 0 || (expB == 0x7FF && sigB != 0))
                    return context.PropagateNaNFloat64Bits(uiA, uiB, uiC);

                magBits = (ulong)(long)expB | sigB;
                goto infProdArg;
            }

            if (expB == 0x7FF)
            {
                if (sigB != 0)
                    return context.PropagateNaNFloat64Bits(uiA, uiB, uiC);

                magBits = (ulong)(long)expA | sigA;
                goto infProdArg;
            }

            if (expC == 0x7FF)
            {
                return (sigC != 0)
                    ? context.PropagateNaNFloat64Bits(0, uiC)
                    : Float64.FromBitsUI64(uiC);
            }

            if (expA == 0)
            {
                if (sigA == 0)
                {
                    if (((ulong)(long)expC | sigC) == 0 && signZ != signC)
                        return PackToF64(context.Rounding == RoundingMode.Min, 0, 0);

                    return Float64.FromBitsUI64(uiC);
                }

                (expA, sigA) = NormSubnormalF64Sig(sigA);
            }

            if (expB == 0)
            {
                if (sigB == 0)
                {
                    if (((ulong)(long)expC | sigC) == 0 && signZ != signC)
                        return PackToF64(context.Rounding == RoundingMode.Min, 0, 0);

                    return Float64.FromBitsUI64(uiC);
                }

                (expB, sigB) = NormSubnormalF64Sig(sigB);
            }

            expZ = expA + expB - 0x3FE;
            sigA = (sigA | 0x0010000000000000) << 10;
            sigB = (sigB | 0x0010000000000000) << 10;
            sig128Z = Mul64To128(sigA, sigB);

            if (sig128Z.V64 < 0x2000000000000000)
            {
                --expZ;
                //sig128Z += sig128Z;
                sig128Z <<= 1; // faster than above add if inlined? probably not by much, if any
            }

            if (expC == 0)
            {
                if (sigC == 0)
                {
                    --expZ;
                    sigZ = (sig128Z.V64 << 1) | (sig128Z.V00 != 0 ? 1UL : 0);
                    return RoundPackToF64(context, signZ, expZ, sigZ);
                }

                (expC, sigC) = NormSubnormalF64Sig(sigC);
            }

            sigC = (sigC | 0x0010000000000000) << 9;

            expDiff = expZ - expC;
            if (expDiff < 0)
            {
                expZ = expC;
                if (signZ == signC || expDiff < -1)
                {
                    sig128Z.V64 = ShiftRightJam64(sig128Z.V64, -expDiff);
                }
                else
                {
                    sig128Z = ShortShiftRightJam128(sig128Z.V64, sig128Z.V00, 1);
                }
            }
            else if (expDiff != 0)
            {
                // Compiler thinks that it isn't used, because of the SkipInit hacks later in the code.
#pragma warning disable IDE0059 // Unnecessary assignment of a value
                sig128C = ShiftRightJam128(sigC, 0, expDiff);
#pragma warning restore IDE0059 // Unnecessary assignment of a value
            }

            if (signZ == signC)
            {
                if (expDiff <= 0)
                {
                    sigZ = (sigC + sig128Z.V64) | (sig128Z.V00 != 0 ? 1UL : 0UL);
                }
                else
                {
                    Unsafe.SkipInit(out sig128C); // prevent compiler error -- it's always assigned
                    sig128Z += sig128C;
                    sigZ = sig128Z.V64 | (sig128Z.V00 != 0 ? 1UL : 0UL);
                }

                if (sigZ < 0x4000000000000000)
                {
                    --expZ;
                    sigZ <<= 1;
                }
            }
            else
            {
                if (expDiff < 0)
                {
                    signZ = signC;
                    sig128Z = Sub128(sigC, 0, sig128Z.V64, sig128Z.V00);
                }
                else if (expDiff == 0)
                {
                    sig128Z.V64 -= sigC;
                    if ((sig128Z.V64 | sig128Z.V00) == 0)
                        return PackToF64(context.Rounding == RoundingMode.Min, 0, 0);

                    if ((sig128Z.V64 & 0x8000000000000000) != 0)
                    {
                        signZ = !signZ;
                        sig128Z = -sig128Z;
                    }
                }
                else
                {
                    Unsafe.SkipInit(out sig128C); // prevent compiler error -- it's always assigned
                    sig128Z -= sig128C;
                }

                if (sig128Z.V64 == 0)
                {
                    expZ -= 64;
                    sig128Z.V64 = sig128Z.V00;
                    sig128Z.V00 = 0;
                }

                shiftDist = CountLeadingZeroes64(sig128Z.V64) - 1;
                expZ -= shiftDist;
                if (shiftDist < 0)
                {
                    sigZ = ShortShiftRightJam64(sig128Z.V64, -shiftDist);
                }
                else
                {
                    sig128Z = ShortShiftLeft128(sig128Z.V64, sig128Z.V00, shiftDist);
                    sigZ = sig128Z.V64;
                }

                sigZ |= (sig128Z.V00 != 0 ? 1UL : 0);
            }

            return RoundPackToF64(context, signZ, expZ, sigZ);

        infProdArg:
            if (magBits != 0)
            {
                uiZ = PackToF64UI(signZ, 0x7FF, 0);
                if (expC != 0x7FF)
                    return Float64.FromBitsUI64(uiZ);

                if (sigC != 0)
                    return context.PropagateNaNFloat64Bits(uiZ, uiC);

                if (signZ == signC)
                    return Float64.FromBitsUI64(uiZ);
            }

            context.RaiseFlags(ExceptionFlags.Invalid);
            return context.PropagateNaNFloat64Bits(context.DefaultNaNFloat64Bits, uiC);
        }
    }

}