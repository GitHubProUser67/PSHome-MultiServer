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

using System;

namespace Tommunism.SoftFloat
{
    // TODO: Add a static user-settable factory for creating custom instances when using the static properties?

    public class SoftFloatContext
    {
        #region Fields

        [ThreadStatic]
        private static SoftFloatContext? _currentThreadContext;

        private static SoftFloatContext? _sharedContext;

        #endregion

        #region Constructors

        public SoftFloatContext() : this(SoftFloatSpecialize.Default)
        {
        }

        public SoftFloatContext(SoftFloatSpecialize specialize)
        {
            Specialize = specialize;
            DetectTininess = specialize.InitialDetectTininess;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether <see cref="RaiseFlags(ExceptionFlags)"/> should throw a <see cref="SoftFloatException"/>
        /// exception when called.
        /// </summary>
        /// <remarks>
        /// This is not thread safe! Alternatively this class can be derived to provide more consistency. Ideally this should be set once at
        /// the start of the program.
        /// </remarks>
        public static bool ThrowOnRaiseFlags { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating which context should be returned by <see cref="Default"/>.
        /// </summary>
        /// <remarks>
        /// This is not thread safe! It should not be changed inside or between software floating-point operations. Ideally this should be set
        /// once at the start of the program.
        /// </remarks>
        public static bool UseThreadStaticContextByDefault { get; set; } = true;

        /// <summary>
        /// Gets the default software floating-point context.
        /// </summary>
        /// <remarks>
        /// The returned value is determined by the <see cref="UseThreadStaticContextByDefault"/> property. If
        /// <see cref="UseThreadStaticContextByDefault"/> is true, then <see cref="CurrentThreadContext"/> is returned; otherwise,
        /// <see cref="SharedContext"/> is returned.
        /// </remarks>
        public static SoftFloatContext Default => UseThreadStaticContextByDefault ? CurrentThreadContext : SharedContext;

        /// <summary>
        /// Gets a software floating-point context that is unique to each thread.
        /// </summary>
        public static SoftFloatContext CurrentThreadContext
        {
            get
            {
                // Get or create state for current thread.
                var state = _currentThreadContext;
                if (state == null)
                {
                    state = new SoftFloatContext();
                    _currentThreadContext = state;
                }

                return state;
            }
        }

        /// <summary>
        /// Gets a software floating-point context that is shared with all threads. This is not thread safe!
        /// </summary>
        public static SoftFloatContext SharedContext
        {
            get
            {
                // Get or create shared state.
                var state = _sharedContext;
                if (state == null)
                {
                    state = new SoftFloatContext();
                    _sharedContext = state;
                }

                return state;
            }
        }

        /// <summary>
        /// Gets or sets the specialization details to use when performing floating-point operations.
        /// </summary>
        public SoftFloatSpecialize Specialize { get; set; }

        // softfloat_detectTininess
        /// <summary>
        /// Gets or sets software floating-point underflow tininess-detection mode.
        /// </summary>
        public TininessMode DetectTininess { get; set; }

        // softfloat_roundingMode
        /// <summary>
        /// Gets or sets software floating-point rounding mode.
        /// </summary>
        public RoundingMode Rounding { get; set; } = RoundingMode.NearEven;

        // softfloat_exceptionFlags
        /// <summary>
        /// Gets or sets software floating-point exception flags.
        /// </summary>
        /// <remarks>
        /// This can be overridden to implement custom actions to perform when floating point exceptions flags change (such as logging).
        /// 
        /// The default implementation of <see cref="RaiseFlags(ExceptionFlags)"/> gets and sets this property before optionally throwing an exception.
        /// </remarks>
        public virtual ExceptionFlags ExceptionFlags { get; set; } = ExceptionFlags.None;

        // extF80_roundingPrecision
        /// <summary>
        /// Gets or sets the rounding precision for 80-bit extended double-precision floating-point.
        /// </summary>
        public ExtFloat80RoundingPrecision RoundingPrecisionExtFloat80 { get; set; } = ExtFloat80RoundingPrecision._80;

        #endregion

        #region Methods

        /// <summary>
        /// Routine to raise any or all of the software floating-point exception flags.
        /// </summary>
        /// <remarks>
        /// This can be overridden to implement custom actions to perform when floating point exceptions occur (such as logging).
        /// 
        /// By default, this implementation simply adds the set <paramref name="exceptionFlags"/> to the <see cref="ExceptionFlags"/> property
        /// and optionally throws a <see cref="SoftFloatException"/> exception if <see cref="ThrowOnRaiseFlags"/> is true.
        /// 
        /// Some of the internal code will set the <see cref="ExceptionFlags"/> property directly and bypass this call. This method is
        /// generally called when setting error flags.
        /// </remarks>
        public virtual void RaiseFlags(ExceptionFlags exceptionFlags)
        {
            ExceptionFlags |= exceptionFlags;

            // Should an exception be thrown.
            if (ThrowOnRaiseFlags)
            {
                // Only throw the flags passed in, ignore any other flags that may have already been set.
                throw new SoftFloatException(exceptionFlags);
            }
        }

        #region Float16 Shortcut Methods

        public Float16 ToFloat16(uint value) => Float16.FromUInt32(this, value);
        public Float16 ToFloat16(ulong value) => Float16.FromUInt64(this, value);
        public Float16 ToFloat16(int value) => Float16.FromInt32(this, value);
        public Float16 ToFloat16(long value) => Float16.FromInt64(this, value);

        public uint ToUInt32(Float16 value, bool exact) => value.ToUInt32(this, Rounding, exact);
        public uint ToUInt32(Float16 value, RoundingMode roundingMode, bool exact) => value.ToUInt32(this, roundingMode, exact);
        public uint ToUInt32RoundMinMag(Float16 value, bool exact) => value.ToUInt32RoundMinMag(this, exact);

        public ulong ToUInt64(Float16 value, bool exact) => value.ToUInt64(this, Rounding, exact);
        public ulong ToUInt64(Float16 value, RoundingMode roundingMode, bool exact) => value.ToUInt64(this, roundingMode, exact);
        public ulong ToUInt64RoundMinMag(Float16 value, bool exact) => value.ToUInt64RoundMinMag(this, exact);

        public int ToInt32(Float16 value, bool exact) => value.ToInt32(this, Rounding, exact);
        public int ToInt32(Float16 value, RoundingMode roundingMode, bool exact) => value.ToInt32(this, roundingMode, exact);
        public int ToInt32RoundMinMag(Float16 value, bool exact) => value.ToInt32RoundMinMag(this, exact);

        public long ToInt64(Float16 value, bool exact) => value.ToInt64(this, Rounding, exact);
        public long ToInt64(Float16 value, RoundingMode roundingMode, bool exact) => value.ToInt64(this, roundingMode, exact);
        public long ToInt64RoundMinMag(Float16 value, bool exact) => value.ToInt64RoundMinMag(this, exact);

        public Float32 ToFloat32(Float16 value) => value.ToFloat32(this);
        public Float64 ToFloat64(Float16 value) => value.ToFloat64(this);
        public ExtFloat80 ToExtFloat80(Float16 value) => value.ToExtFloat80(this);
        public Float128 ToFloat128(Float16 value) => value.ToFloat128(this);

        public Float16 RoundToInt(Float16 value, bool exact) => value.RoundToInt(this, Rounding, exact);
        public Float16 RoundToInt(Float16 value, RoundingMode roundingMode, bool exact) => value.RoundToInt(this, roundingMode, exact);

        public Float16 Add(Float16 a, Float16 b) => Float16.Add(this, a, b);
        public Float16 Subtract(Float16 a, Float16 b) => Float16.Subtract(this, a, b);
        public Float16 Multiply(Float16 a, Float16 b) => Float16.Multiply(this, a, b);
        public Float16 MultiplyAndAdd(Float16 a, Float16 b, Float16 c) => Float16.MultiplyAndAdd(this, a, b, c);
        public Float16 Divide(Float16 a, Float16 b) => Float16.Divide(this, a, b);
        public Float16 Modulus(Float16 a, Float16 b) => Float16.Modulus(this, a, b);
        public Float16 SquareRoot(Float16 value) => value.SquareRoot(this);

        public bool CompareEqual(Float16 a, Float16 b, bool signaling) => Float16.CompareEqual(this, a, b, signaling);
        public bool CompareLessThan(Float16 a, Float16 b, bool signaling) => Float16.CompareLessThan(this, a, b, signaling);
        public bool CompareLessThanOrEqual(Float16 a, Float16 b, bool signaling) => Float16.CompareLessThanOrEqual(this, a, b, signaling);

        #endregion

        #region Float32 Shortcut Methods

        public Float32 ToFloat32(uint value) => Float32.FromUInt32(this, value);
        public Float32 ToFloat32(ulong value) => Float32.FromUInt64(this, value);
        public Float32 ToFloat32(int value) => Float32.FromInt32(this, value);
        public Float32 ToFloat32(long value) => Float32.FromInt64(this, value);

        public uint ToUInt32(Float32 value, bool exact) => value.ToUInt32(this, Rounding, exact);
        public uint ToUInt32(Float32 value, RoundingMode roundingMode, bool exact) => value.ToUInt32(this, roundingMode, exact);
        public uint ToUInt32RoundMinMag(Float32 value, bool exact) => value.ToUInt32RoundMinMag(this, exact);

        public ulong ToUInt64(Float32 value, bool exact) => value.ToUInt64(this, Rounding, exact);
        public ulong ToUInt64(Float32 value, RoundingMode roundingMode, bool exact) => value.ToUInt64(this, roundingMode, exact);
        public ulong ToUInt64RoundMinMag(Float32 value, bool exact) => value.ToUInt64RoundMinMag(this, exact);

        public int ToInt32(Float32 value, bool exact) => value.ToInt32(this, Rounding, exact);
        public int ToInt32(Float32 value, RoundingMode roundingMode, bool exact) => value.ToInt32(this, roundingMode, exact);
        public int ToInt32RoundMinMag(Float32 value, bool exact) => value.ToInt32RoundMinMag(this, exact);

        public long ToInt64(Float32 value, bool exact) => value.ToInt64(this, Rounding, exact);
        public long ToInt64(Float32 value, RoundingMode roundingMode, bool exact) => value.ToInt64(this, roundingMode, exact);
        public long ToInt64RoundMinMag(Float32 value, bool exact) => value.ToInt64RoundMinMag(this, exact);

        public Float16 ToFloat16(Float32 value) => value.ToFloat16(this);
        public Float64 ToFloat64(Float32 value) => value.ToFloat64(this);
        public ExtFloat80 ToExtFloat80(Float32 value) => value.ToExtFloat80(this);
        public Float128 ToFloat128(Float32 value) => value.ToFloat128(this);

        public Float32 RoundToInt(Float32 value, bool exact) => value.RoundToInt(this, Rounding, exact);
        public Float32 RoundToInt(Float32 value, RoundingMode roundingMode, bool exact) => value.RoundToInt(this, roundingMode, exact);

        public Float32 Add(Float32 a, Float32 b) => Float32.Add(this, a, b);
        public Float32 Subtract(Float32 a, Float32 b) => Float32.Subtract(this, a, b);
        public Float32 Multiply(Float32 a, Float32 b) => Float32.Multiply(this, a, b);
        public Float32 MultiplyAndAdd(Float32 a, Float32 b, Float32 c) => Float32.MultiplyAndAdd(this, a, b, c);
        public Float32 Divide(Float32 a, Float32 b) => Float32.Divide(this, a, b);
        public Float32 Modulus(Float32 a, Float32 b) => Float32.Modulus(this, a, b);
        public Float32 SquareRoot(Float32 value) => value.SquareRoot(this);

        public bool CompareEqual(Float32 a, Float32 b, bool signaling) => Float32.CompareEqual(this, a, b, signaling);
        public bool CompareLessThan(Float32 a, Float32 b, bool signaling) => Float32.CompareLessThan(this, a, b, signaling);
        public bool CompareLessThanOrEqual(Float32 a, Float32 b, bool signaling) => Float32.CompareLessThanOrEqual(this, a, b, signaling);

        #endregion

        #region Float64 Shortcut Methods

        public Float64 ToFloat64(uint value) => Float64.FromUInt32(this, value);
        public Float64 ToFloat64(ulong value) => Float64.FromUInt64(this, value);
        public Float64 ToFloat64(int value) => Float64.FromInt32(this, value);
        public Float64 ToFloat64(long value) => Float64.FromInt64(this, value);

        public uint ToUInt32(Float64 value, bool exact) => value.ToUInt32(this, Rounding, exact);
        public uint ToUInt32(Float64 value, RoundingMode roundingMode, bool exact) => value.ToUInt32(this, roundingMode, exact);
        public uint ToUInt32RoundMinMag(Float64 value, bool exact) => value.ToUInt32RoundMinMag(this, exact);

        public ulong ToUInt64(Float64 value, bool exact) => value.ToUInt64(this, Rounding, exact);
        public ulong ToUInt64(Float64 value, RoundingMode roundingMode, bool exact) => value.ToUInt64(this, roundingMode, exact);
        public ulong ToUInt64RoundMinMag(Float64 value, bool exact) => value.ToUInt64RoundMinMag(this, exact);

        public int ToInt32(Float64 value, bool exact) => value.ToInt32(this, Rounding, exact);
        public int ToInt32(Float64 value, RoundingMode roundingMode, bool exact) => value.ToInt32(this, roundingMode, exact);
        public int ToInt32RoundMinMag(Float64 value, bool exact) => value.ToInt32RoundMinMag(this, exact);

        public long ToInt64(Float64 value, bool exact) => value.ToInt64(this, Rounding, exact);
        public long ToInt64(Float64 value, RoundingMode roundingMode, bool exact) => value.ToInt64(this, roundingMode, exact);
        public long ToInt64RoundMinMag(Float64 value, bool exact) => value.ToInt64RoundMinMag(this, exact);

        public Float16 ToFloat16(Float64 value) => value.ToFloat16(this);
        public Float32 ToFloat32(Float64 value) => value.ToFloat32(this);
        public ExtFloat80 ToExtFloat80(Float64 value) => value.ToExtFloat80(this);
        public Float128 ToFloat128(Float64 value) => value.ToFloat128(this);

        public Float64 RoundToInt(Float64 value, bool exact) => value.RoundToInt(this, Rounding, exact);
        public Float64 RoundToInt(Float64 value, RoundingMode roundingMode, bool exact) => value.RoundToInt(this, roundingMode, exact);

        public Float64 Add(Float64 a, Float64 b) => Float64.Add(this, a, b);
        public Float64 Subtract(Float64 a, Float64 b) => Float64.Subtract(this, a, b);
        public Float64 Multiply(Float64 a, Float64 b) => Float64.Multiply(this, a, b);
        public Float64 MultiplyAndAdd(Float64 a, Float64 b, Float64 c) => Float64.MultiplyAndAdd(this, a, b, c);
        public Float64 Divide(Float64 a, Float64 b) => Float64.Divide(this, a, b);
        public Float64 Modulus(Float64 a, Float64 b) => Float64.Modulus(this, a, b);
        public Float64 SquareRoot(Float64 value) => value.SquareRoot(this);

        public bool CompareEqual(Float64 a, Float64 b, bool signaling) => Float64.CompareEqual(this, a, b, signaling);
        public bool CompareLessThan(Float64 a, Float64 b, bool signaling) => Float64.CompareLessThan(this, a, b, signaling);
        public bool CompareLessThanOrEqual(Float64 a, Float64 b, bool signaling) => Float64.CompareLessThanOrEqual(this, a, b, signaling);

        #endregion

        #region ExtFloat80 Shortcut Methods

        public ExtFloat80 ToExtFloat80(uint value) => ExtFloat80.FromUInt32(this, value);
        public ExtFloat80 ToExtFloat80(ulong value) => ExtFloat80.FromUInt64(this, value);
        public ExtFloat80 ToExtFloat80(int value) => ExtFloat80.FromInt32(this, value);
        public ExtFloat80 ToExtFloat80(long value) => ExtFloat80.FromInt64(this, value);

        public uint ToUInt32(ExtFloat80 value, bool exact) => value.ToUInt32(this, Rounding, exact);
        public uint ToUInt32(ExtFloat80 value, RoundingMode roundingMode, bool exact) => value.ToUInt32(this, roundingMode, exact);
        public uint ToUInt32RoundMinMag(ExtFloat80 value, bool exact) => value.ToUInt32RoundMinMag(this, exact);

        public ulong ToUInt64(ExtFloat80 value, bool exact) => value.ToUInt64(this, Rounding, exact);
        public ulong ToUInt64(ExtFloat80 value, RoundingMode roundingMode, bool exact) => value.ToUInt64(this, roundingMode, exact);
        public ulong ToUInt64RoundMinMag(ExtFloat80 value, bool exact) => value.ToUInt64RoundMinMag(this, exact);

        public int ToInt32(ExtFloat80 value, bool exact) => value.ToInt32(this, Rounding, exact);
        public int ToInt32(ExtFloat80 value, RoundingMode roundingMode, bool exact) => value.ToInt32(this, roundingMode, exact);
        public int ToInt32RoundMinMag(ExtFloat80 value, bool exact) => value.ToInt32RoundMinMag(this, exact);

        public long ToInt64(ExtFloat80 value, bool exact) => value.ToInt64(this, Rounding, exact);
        public long ToInt64(ExtFloat80 value, RoundingMode roundingMode, bool exact) => value.ToInt64(this, roundingMode, exact);
        public long ToInt64RoundMinMag(ExtFloat80 value, bool exact) => value.ToInt64RoundMinMag(this, exact);

        public Float16 ToFloat16(ExtFloat80 value) => value.ToFloat16(this);
        public Float32 ToFloat32(ExtFloat80 value) => value.ToFloat32(this);
        public Float64 ToFloat64(ExtFloat80 value) => value.ToFloat64(this);
        public Float128 ToFloat128(ExtFloat80 value) => value.ToFloat128(this);

        public ExtFloat80 RoundToInt(ExtFloat80 value, bool exact) => value.RoundToInt(this, Rounding, exact);
        public ExtFloat80 RoundToInt(ExtFloat80 value, RoundingMode roundingMode, bool exact) => value.RoundToInt(this, roundingMode, exact);

        public ExtFloat80 Add(ExtFloat80 a, ExtFloat80 b) => ExtFloat80.Add(this, a, b);
        public ExtFloat80 Subtract(ExtFloat80 a, ExtFloat80 b) => ExtFloat80.Subtract(this, a, b);
        public ExtFloat80 Multiply(ExtFloat80 a, ExtFloat80 b) => ExtFloat80.Multiply(this, a, b);
        public ExtFloat80 Divide(ExtFloat80 a, ExtFloat80 b) => ExtFloat80.Divide(this, a, b);
        public ExtFloat80 Modulus(ExtFloat80 a, ExtFloat80 b) => ExtFloat80.Modulus(this, a, b);
        public ExtFloat80 SquareRoot(ExtFloat80 value) => value.SquareRoot(this);

        public bool CompareEqual(ExtFloat80 a, ExtFloat80 b, bool signaling) => ExtFloat80.CompareEqual(this, a, b, signaling);
        public bool CompareLessThan(ExtFloat80 a, ExtFloat80 b, bool signaling) => ExtFloat80.CompareLessThan(this, a, b, signaling);
        public bool CompareLessThanOrEqual(ExtFloat80 a, ExtFloat80 b, bool signaling) => ExtFloat80.CompareLessThanOrEqual(this, a, b, signaling);

        #endregion

        #region Float128 Shortcut Methods

        public Float128 ToFloat128(uint value) => Float128.FromUInt32(this, value);
        public Float128 ToFloat128(ulong value) => Float128.FromUInt64(this, value);
        public Float128 ToFloat128(int value) => Float128.FromInt32(this, value);
        public Float128 ToFloat128(long value) => Float128.FromInt64(this, value);

        public uint ToUInt32(Float128 value, bool exact) => value.ToUInt32(this, Rounding, exact);
        public uint ToUInt32(Float128 value, RoundingMode roundingMode, bool exact) => value.ToUInt32(this, roundingMode, exact);
        public uint ToUInt32RoundMinMag(Float128 value, bool exact) => value.ToUInt32RoundMinMag(this, exact);

        public ulong ToUInt64(Float128 value, bool exact) => value.ToUInt64(this, Rounding, exact);
        public ulong ToUInt64(Float128 value, RoundingMode roundingMode, bool exact) => value.ToUInt64(this, roundingMode, exact);
        public ulong ToUInt64RoundMinMag(Float128 value, bool exact) => value.ToUInt64RoundMinMag(this, exact);

        public int ToInt32(Float128 value, bool exact) => value.ToInt32(this, Rounding, exact);
        public int ToInt32(Float128 value, RoundingMode roundingMode, bool exact) => value.ToInt32(this, roundingMode, exact);
        public int ToInt32RoundMinMag(Float128 value, bool exact) => value.ToInt32RoundMinMag(this, exact);

        public long ToInt64(Float128 value, bool exact) => value.ToInt64(this, Rounding, exact);
        public long ToInt64(Float128 value, RoundingMode roundingMode, bool exact) => value.ToInt64(this, roundingMode, exact);
        public long ToInt64RoundMinMag(Float128 value, bool exact) => value.ToInt64RoundMinMag(this, exact);

        public Float16 ToFloat16(Float128 value) => value.ToFloat16(this);
        public Float32 ToFloat32(Float128 value) => value.ToFloat32(this);
        public Float64 ToFloat64(Float128 value) => value.ToFloat64(this);
        public ExtFloat80 ToExtFloat80(Float128 value) => value.ToExtFloat80(this);

        public Float128 RoundToInt(Float128 value, bool exact) => value.RoundToInt(this, Rounding, exact);
        public Float128 RoundToInt(Float128 value, RoundingMode roundingMode, bool exact) => value.RoundToInt(this, roundingMode, exact);

        public Float128 Add(Float128 a, Float128 b) => Float128.Add(this, a, b);
        public Float128 Subtract(Float128 a, Float128 b) => Float128.Subtract(this, a, b);
        public Float128 Multiply(Float128 a, Float128 b) => Float128.Multiply(this, a, b);
        public Float128 MultiplyAndAdd(Float128 a, Float128 b, Float128 c) => Float128.MultiplyAndAdd(this, a, b, c);
        public Float128 Divide(Float128 a, Float128 b) => Float128.Divide(this, a, b);
        public Float128 Modulus(Float128 a, Float128 b) => Float128.Modulus(this, a, b);
        public Float128 SquareRoot(Float128 value) => value.SquareRoot(this);

        public bool CompareEqual(Float128 a, Float128 b, bool signaling) => Float128.CompareEqual(this, a, b, signaling);
        public bool CompareLessThan(Float128 a, Float128 b, bool signaling) => Float128.CompareLessThan(this, a, b, signaling);
        public bool CompareLessThanOrEqual(Float128 a, Float128 b, bool signaling) => Float128.CompareLessThanOrEqual(this, a, b, signaling);

        #endregion

        #endregion

        #region Specialize Helpers

        #region Integer Conversion Constants

        public uint UInt32FromPositiveOverflow => Specialize.UInt32FromPositiveOverflow;
        public uint UInt32FromNegativeOverflow => Specialize.UInt32FromNegativeOverflow;
        public uint UInt32FromNaN => Specialize.UInt32FromNaN;

        public int Int32FromPositiveOverflow => Specialize.Int32FromPositiveOverflow;
        public int Int32FromNegativeOverflow => Specialize.Int32FromNegativeOverflow;
        public int Int32FromNaN => Specialize.Int32FromNaN;

        public ulong UInt64FromPositiveOverflow => Specialize.UInt64FromPositiveOverflow;
        public ulong UInt64FromNegativeOverflow => Specialize.UInt64FromNegativeOverflow;
        public ulong UInt64FromNaN => Specialize.UInt64FromNaN;

        public long Int64FromPositiveOverflow => Specialize.Int64FromPositiveOverflow;
        public long Int64FromNegativeOverflow => Specialize.Int64FromNegativeOverflow;
        public long Int64FromNaN => Specialize.Int64FromNaN;

        public uint UInt32FromOverflow(bool isNegative) => Specialize.UInt32FromOverflow(isNegative);
        public int Int32FromOverflow(bool isNegative) => Specialize.Int32FromOverflow(isNegative);
        public ulong UInt64FromOverflow(bool isNegative) => Specialize.UInt64FromOverflow(isNegative);
        public long Int64FromOverflow(bool isNegative) => Specialize.Int64FromOverflow(isNegative);

        #endregion

        #region Float16

        public ushort DefaultNaNFloat16Bits => Specialize.DefaultNaNFloat16Bits;
        public Float16 DefaultNaNFloat16 => Specialize.DefaultNaNFloat16;

        public bool IsSignalingNaNFloat16Bits(uint bits) => Specialize.IsSignalingNaNFloat16Bits(bits);
        public void Float16BitsToCommonNaN(uint bits, out SoftFloatCommonNaN commonNaN) => Specialize.Float16BitsToCommonNaN(this, bits, out commonNaN);
        public Float16 CommonNaNToFloat16(in SoftFloatCommonNaN commonNaN) => Float16.FromBitsUI16(Specialize.CommonNaNToFloat16Bits(commonNaN));
        public Float16 PropagateNaNFloat16(uint bitsA, uint bitsB) => Float16.FromBitsUI16(Specialize.PropagateNaNFloat16Bits(this, bitsA, bitsB));

        public Float16 PropagateNaNFloat16(uint bitsA, uint bitsB, uint bitsC)
        {
            ushort result;
            result = Specialize.PropagateNaNFloat16Bits(this, bitsA, bitsB);
            result = Specialize.PropagateNaNFloat16Bits(this, result, bitsC);
            return Float16.FromBitsUI16(result);
        }

        #endregion

        #region Float32

        public uint DefaultNaNFloat32Bits => Specialize.DefaultNaNFloat32Bits;
        public Float32 DefaultNaNFloat32 => Specialize.DefaultNaNFloat32;

        public bool IsSignalingNaNFloat32Bits(uint bits) => Specialize.IsSignalingNaNFloat32Bits(bits);
        public void Float32BitsToCommonNaN(uint bits, out SoftFloatCommonNaN commonNaN) => Specialize.Float32BitsToCommonNaN(this, bits, out commonNaN);
        public Float32 CommonNaNToFloat32(in SoftFloatCommonNaN commonNaN) => Float32.FromBitsUI32(Specialize.CommonNaNToFloat32Bits(in commonNaN));
        public Float32 PropagateNaNFloat32Bits(uint bitsA, uint bitsB) => Float32.FromBitsUI32(Specialize.PropagateNaNFloat32Bits(this, bitsA, bitsB));

        public Float32 PropagateNaNFloat32Bits(uint bitsA, uint bitsB, uint bitsC)
        {
            uint result;
            result = Specialize.PropagateNaNFloat32Bits(this, bitsA, bitsB);
            result = Specialize.PropagateNaNFloat32Bits(this, result, bitsC);
            return Float32.FromBitsUI32(result);
        }

        #endregion

        #region Float64

        public ulong DefaultNaNFloat64Bits => Specialize.DefaultNaNFloat64Bits;
        public Float64 DefaultNaNFloat64 => Specialize.DefaultNaNFloat64;

        public bool IsSignalingNaNFloat64Bits(ulong bits) => Specialize.IsSignalingNaNFloat64Bits(bits);
        public void Float64BitsToCommonNaN(ulong bits, out SoftFloatCommonNaN commonNaN) => Specialize.Float64BitsToCommonNaN(this, bits, out commonNaN);
        public Float64 CommonNaNToFloat64(in SoftFloatCommonNaN commonNaN) => Float64.FromBitsUI64(Specialize.CommonNaNToFloat64Bits(in commonNaN));
        public Float64 PropagateNaNFloat64Bits(ulong bitsA, ulong bitsB) => Float64.FromBitsUI64(Specialize.PropagateNaNFloat64Bits(this, bitsA, bitsB));

        public Float64 PropagateNaNFloat64Bits(ulong bitsA, ulong bitsB, ulong bitsC)
        {
            ulong result;
            result = Specialize.PropagateNaNFloat64Bits(this, bitsA, bitsB);
            result = Specialize.PropagateNaNFloat64Bits(this, result, bitsC);
            return Float64.FromBitsUI64(result);
        }

        #endregion

        #region ExtFloat80

        public SFUInt128 DefaultNaNExtFloat80Bits => Specialize.DefaultNaNExtFloat80Bits;
        public ExtFloat80 DefaultNaNExtFloat80 => Specialize.DefaultNaNExtFloat80;

        public bool IsSignalingNaNExtFloat80Bits(uint bits64, ulong bits0) => Specialize.IsSignalingNaNExtFloat80Bits(bits64, bits0);
        public void ExtFloat80BitsToCommonNaN(uint bits64, ulong bits0, out SoftFloatCommonNaN commonNaN) => Specialize.ExtFloat80BitsToCommonNaN(this, bits64, bits0, out commonNaN);
        public ExtFloat80 CommonNaNToExtFloat80(in SoftFloatCommonNaN commonNaN) => ExtFloat80.FromBitsUI128(Specialize.CommonNaNToExtFloat80Bits(in commonNaN));
        public ExtFloat80 PropagateNaNExtFloat80Bits(uint bitsA64, ulong bitsA0, uint bitsB64, ulong bitsB0) => ExtFloat80.FromBitsUI128(Specialize.PropagateNaNExtFloat80Bits(this, bitsA64, bitsA0, bitsB64, bitsB0));

        public ExtFloat80 PropagateNaNExtFloat80Bits(uint bitsA64, ulong bitsA0, uint bitsB64, ulong bitsB0, uint bitsC64, ulong bitsC0)
        {
            SFUInt128 result;
            result = Specialize.PropagateNaNExtFloat80Bits(this, bitsA64, bitsA0, bitsB64, bitsB0);
            result = Specialize.PropagateNaNExtFloat80Bits(this, (uint)result.GetUpperUI64(), result.GetLowerUI64(), bitsC64, bitsC0);
            return ExtFloat80.FromBitsUI128(result);
        }

        #endregion

        #region Float128

        public SFUInt128 DefaultNaNFloat128Bits => Specialize.DefaultNaNFloat128Bits;
        public Float128 DefaultNaNFloat128 => Specialize.DefaultNaNFloat128;

        public bool IsSignalingNaNFloat128Bits(ulong bits64, ulong bits0) => Specialize.IsSignalingNaNFloat128Bits(bits64, bits0);
        public void Float128BitsToCommonNaN(ulong bits64, ulong bits0, out SoftFloatCommonNaN commonNaN) => Specialize.Float128BitsToCommonNaN(this, bits64, bits0, out commonNaN);
        public Float128 CommonNaNToFloat128(in SoftFloatCommonNaN commonNaN) => Float128.FromBitsUI128(Specialize.CommonNaNToFloat128Bits(in commonNaN));
        public Float128 PropagateNaNFloat128Bits(ulong bitsA64, ulong bitsA0, ulong bitsB64, ulong bitsB0) => Float128.FromBitsUI128(Specialize.PropagateNaNFloat128Bits(this, bitsA64, bitsA0, bitsB64, bitsB0));

        public Float128 PropagateNaNFloat128Bits(ulong bitsA64, ulong bitsA0, ulong bitsB64, ulong bitsB0, ulong bitsC64, ulong bitsC0)
        {
            SFUInt128 result;
            result = Specialize.PropagateNaNFloat128Bits(this, bitsA64, bitsA0, bitsB64, bitsB0);
            result = Specialize.PropagateNaNFloat128Bits(this, result.GetUpperUI64(), result.GetLowerUI64(), bitsC64, bitsC0);
            return Float128.FromBitsUI128(result);
        }

        #endregion

        #endregion
    }

}