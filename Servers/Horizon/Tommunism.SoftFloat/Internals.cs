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


namespace Tommunism.SoftFloat
{
    internal static partial class Internals
    {
        #region Rounding

        // softfloat_roundToUI32
        public static uint RoundToUI32(SoftFloatContext context, bool sign, ulong sig, RoundingMode roundingMode, bool exact)
        {
            uint roundIncrement, roundBits;
            uint z;

            roundIncrement = 0x800U;
            if (roundingMode is not RoundingMode.NearMaxMag and not RoundingMode.NearEven)
            {
                roundIncrement = 0;
                if (sign)
                {
                    if (sig == 0)
                        return 0;

                    if (roundingMode is RoundingMode.Min or RoundingMode.Odd)
                        goto invalid;
                }
                else
                {
                    if (roundingMode == RoundingMode.Max)
                        roundIncrement = 0xFFF;
                }
            }

            roundBits = (uint)sig & 0xFFF;
            sig += roundIncrement;
            if ((sig & 0xFFFFF00000000000) != 0)
                goto invalid;

            z = (uint)(sig >> 12);
            if (roundBits == 0x800 && roundingMode == RoundingMode.NearEven)
                z &= ~(uint)1;

            if (sign && z != 0)
                goto invalid;

            if (roundBits != 0)
            {
                if (roundingMode == RoundingMode.Odd)
                    z |= 1U;

                if (exact)
                    context.ExceptionFlags |= ExceptionFlags.Inexact;
            }

            return z;

        invalid:
            context.RaiseFlags(ExceptionFlags.Invalid);
            return context.UInt32FromOverflow(sign);
        }

        // softfloat_roundToUI64
        public static ulong RoundToUI64(SoftFloatContext context, bool sign, ulong sig, ulong sigExtra, RoundingMode roundingMode, bool exact)
        {
            if (roundingMode is RoundingMode.NearMaxMag or RoundingMode.NearEven)
            {
                if (0x8000000000000000 <= sigExtra)
                {
                    ++sig;
                    if (sig == 0)
                        goto invalid;

                    if (roundingMode == RoundingMode.NearEven && sigExtra == 0x8000000000000000)
                        sig &= ~(ulong)1;
                }
            }
            else
            {
                if (sign)
                {
                    if ((sig | sigExtra) == 0)
                        return 0;

                    if (roundingMode is RoundingMode.Min or RoundingMode.Odd)
                        goto invalid;
                }
                else
                {
                    if (roundingMode == RoundingMode.Max && sigExtra != 0)
                    {
                        ++sig;
                        if (sig == 0)
                            goto invalid;

                        if (roundingMode == RoundingMode.NearEven && sigExtra == 0x8000000000000000)
                            sig &= ~(ulong)1;
                    }
                }
            }

            if (sign && sig != 0)
                goto invalid;

            if (sigExtra != 0)
            {
                if (roundingMode == RoundingMode.Odd)
                    sig |= 1;

                if (exact)
                    context.ExceptionFlags |= ExceptionFlags.Inexact;
            }

            return sig;

        invalid:
            context.RaiseFlags(ExceptionFlags.Invalid);
            return context.UInt64FromOverflow(sign);
        }

        // softfloat_roundToI32
        public static int RoundToI32(SoftFloatContext context, bool sign, ulong sig, RoundingMode roundingMode, bool exact)
        {
            uint roundIncrement, roundBits;
            uint sig32;
            int z;

            roundIncrement = 0x800;
            if (roundingMode is not RoundingMode.NearMaxMag and not RoundingMode.NearEven)
            {
                roundIncrement = 0;
                if (sign ? roundingMode is RoundingMode.Min or RoundingMode.Odd : roundingMode == RoundingMode.Max)
                    roundIncrement = 0xFFF;
            }

            roundBits = (uint)sig & 0xFFF;
            sig += roundIncrement;
            if ((sig & 0xFFFFF00000000000) != 0)
                goto invalid;

            sig32 = (uint)(sig >> 12);
            if (roundBits == 0x800 && roundingMode == RoundingMode.NearEven)
                sig32 &= ~1U;

            z = sign ? -(int)sig32 : (int)sig32;
            if (z != 0 && ((z < 0) ^ sign))
                goto invalid;

            if (roundBits != 0)
            {
                if (roundingMode == RoundingMode.Odd)
                    z |= 1;

                if (exact)
                    context.ExceptionFlags |= ExceptionFlags.Inexact;
            }

            return z;

        invalid:
            context.RaiseFlags(ExceptionFlags.Invalid);
            return context.Int32FromOverflow(sign);
        }

        // softfloat_roundToI64
        public static long RoundToI64(SoftFloatContext context, bool sign, ulong sig, ulong sigExtra, RoundingMode roundingMode, bool exact)
        {
            long z;

            if (roundingMode is RoundingMode.NearMaxMag or RoundingMode.NearEven)
            {
                if (0x8000000000000000 <= sigExtra)
                {
                    ++sig;
                    if (sig == 0)
                        goto invalid;

                    if (roundingMode == RoundingMode.NearEven && sigExtra == 0x8000000000000000)
                        sig &= ~1UL;
                }
            }
            else
            {
                if (sigExtra != 0 && (sign ? roundingMode is RoundingMode.Min or RoundingMode.Odd : roundingMode == RoundingMode.Max))
                {
                    ++sig;
                    if (sig == 0)
                        goto invalid;

                    if (roundingMode == RoundingMode.NearEven && sigExtra == 0x8000000000000000)
                        sig &= ~1UL;
                }
            }

            z = sign ? -(long)sig : (long)sig;
            if (z != 0 && ((z < 0) ^ sign))
                goto invalid;

            if (sigExtra != 0)
            {
                if (roundingMode == RoundingMode.Odd)
                    z |= 1L;

                if (exact)
                    context.ExceptionFlags |= ExceptionFlags.Inexact;
            }

            return z;

        invalid:
            context.RaiseFlags(ExceptionFlags.Invalid);
            return context.Int64FromOverflow(sign);
        }

        #endregion
    }
}
