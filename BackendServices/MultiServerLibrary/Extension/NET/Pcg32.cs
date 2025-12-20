/* Adapted from: https://github.com/wjakob/pcg32/blob/master/pcg32.h
 * 
 * Tiny self-contained version of the PCG Random Number Generation for C++
 * put together from pieces of the much larger C/C++ codebase.
 * Wenzel Jakob, February 2015
 *
 * The PCG random number generator was developed by Melissa O'Neill
 * <oneill@pcg-random.org>
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
 * For additional information about the PCG random number generation scheme,
 * including its license and other licensing options, visit
 *
 *     http://www.pcg-random.org
 */

using System.Collections.Generic;

namespace System
{
    /// PCG32 Pseudorandom number generator
    public class Pcg32
    {
        private const ulong PCG32_DEFAULT_STATE = 0x853c49e6748fea9bUL;
        private const ulong PCG32_DEFAULT_STREAM = 0xda3e39cb94b95bdbUL;
        private const ulong PCG32_MULT = 0x5851f42d4c957f2dUL;

        private ulong state;  // RNG state.  All values are possible.
        private ulong inc;    // Controls which RNG sequence (stream) is selected. Must *always* be odd.

        private object _lock = new object();

        /// Initialize the pseudorandom number generator with default seed
        public Pcg32()
        {
            state = PCG32_DEFAULT_STATE;
            inc = PCG32_DEFAULT_STREAM;
        }

        /// Initialize the pseudorandom number generator with the \ref seed() function
        public Pcg32(ulong initstate, ulong initseq = 1)
        {
            Seed(initstate, initseq);
        }

        /**
          * \brief Seed the pseudorandom number generator
          *
          * Specified in two parts: a state initializer and a sequence selection
          * constant (a.k.a. stream id)
          */
        private void Seed(ulong initstate, ulong initseq = 1)
        {
            state = 0U;
            inc = (initseq << 1) | 1;
            NextUInt();
            state += initstate;
            NextUInt();
        }

        /// Generate a uniformly distributed unsigned 32-bit random number
        public uint NextUInt()
        {
            ulong oldstate;

            lock (_lock)
            {
                oldstate = state;
                state = oldstate * PCG32_MULT + inc;
            }

            uint xorshifted = (uint)(((oldstate >> 18) ^ oldstate) >> 27);
            uint rot = (uint)(oldstate >> 59);
            return (xorshifted >> (int)rot) | (xorshifted << ((-(int)rot) & 31));
        }

        /// Generate a uniformly distributed number, r, where 0 <= r < bound
        public uint NextUInt(uint bound)
        {
            // To avoid bias, we need to make the range of the RNG a multiple of
            // bound, which we do by dropping output less than a threshold.
            // A naive scheme to calculate the threshold would be to do
            //
            //     uint32_t threshold = 0x100000000ull % bound;
            //
            // but 64-bit div/mod is slower than 32-bit div/mod (especially on
            // 32-bit platforms).  In essence, we do
            //
            //     uint32_t threshold = (0x100000000ull-bound) % bound;
            //
            // because this version will calculate the same modulus, but the LHS
            // value is less than 2^32.

            uint threshold = (~bound + 1u) % bound;

            // Uniformity guarantees that this loop will terminate.  In practice, it
            // should usually terminate quickly; on average (assuming all bounds are
            // equally likely), 82.25% of the time, we can expect it to require just
            // one iteration.  In the worst case, someone passes a bound of 2^31 + 1
            // (i.e., 2147483649), which invalidates almost 50% of the range.  In
            // practice, bounds are typically small and only a tiny amount of the range
            // is eliminated.
            while (true)
            {
                uint r = NextUInt();
                if (r >= threshold)
                    return r % bound;
            }
        }

        /// Generate a single precision floating point value on the interval [0, 1)
        public float NextFloat()
        {
            /* Trick from MTGP: generate an uniformly distributed
             single precision number in [1,2) and subtract 1. */
            return BitConverter.ToSingle(BitConverter.GetBytes((NextUInt() >> 9) | 0x3f800000u), 0) - 1.0f;
        }

        /**
          * \brief Generate a double precision floating point value on the interval [0, 1)
          *
          * \remark Since the underlying random number generator produces 32 bit output,
          * only the first 32 mantissa bits will be filled (however, the resolution is still
          * finer than in \ref nextFloat(), which only uses 23 mantissa bits)
          */
        public double NextDouble()
        {
            /* Trick from MTGP: generate an uniformly distributed
             double precision number in [1,2) and subtract 1. */
            return BitConverter.ToDouble(BitConverter.GetBytes(((ulong)NextUInt() << 20) | 0x3ff0000000000000UL), 0) - 1.0;
        }

        /**
          * \brief Multi-step advance function (jump-ahead, jump-back)
          *
          * The method used here is based on Brown, "Random Number Generation
          * with Arbitrary Stride", Transactions of the American Nuclear
          * Society (Nov. 1994). The algorithm is very similar to fast
          * exponentiation.
          */
        public void Advance(long delta_)
        {
            ulong acc_mult = 1u;
            ulong acc_plus = 0u;
            ulong cur_mult = PCG32_MULT;

            /* Even though delta is an unsigned integer, we can pass a signed
            integer to go backwards, it just goes "the long way round". */
            ulong delta = (ulong)delta_;

            lock (_lock)
            {
                ulong cur_plus = inc;

                while (delta > 0)
                {
                    if ((delta & 1) != 0)
                    {
                        acc_mult *= cur_mult;
                        acc_plus = acc_plus * cur_mult + cur_plus;
                    }
                    cur_plus = (cur_mult + 1) * cur_plus;
                    cur_mult *= cur_mult;
                    delta >>= 1;
                }

                state = acc_mult * state + acc_plus;
            }
        }

        /**
          * \brief Draw uniformly distributed permutation and permute the
          * given STL container
          *
          * From: Knuth, TAoCP Vol. 2 (3rd 3d), Section 3.4.2
          */
        public void Shuffle<T>(IList<T> list)
        {
            for (int i = list.Count - 1; i > 0; --i)
            {
                int j = (int)NextUInt((uint)(i + 1));
                (list[i], list[j]) = (list[j], list[i]);
            }
        }

        /// Compute the distance between two PCG32 pseudorandom number generators
        public long Distance(Pcg32 other)
        {
            lock (_lock)
            {
                if (inc != other.inc)
                    throw new InvalidOperationException("Can't compute distance between PCG32 instances with different streams.");

                ulong cur_mult = PCG32_MULT;
                ulong cur_plus = inc;
                ulong cur_state = other.state;
                ulong the_bit = 1u;
                ulong distance = 0u;

                while (state != cur_state)
                {
                    if ((state & the_bit) != (cur_state & the_bit))
                    {
                        cur_state = cur_state * cur_mult + cur_plus;
                        distance |= the_bit;
                    }

                    if ((state & the_bit) != (cur_state & the_bit))
                        throw new InvalidOperationException("Inconsistent state during distance calculation.");

                    the_bit <<= 1;
                    cur_plus = (cur_mult + 1) * cur_plus;
                    cur_mult *= cur_mult;
                }

                return (long)distance;
            }
        }

        /// Equality check
        public override bool Equals(object obj)
        {
            if (obj is Pcg32 other)
            {
                lock (_lock)
                    return state == other.state && inc == other.inc;
            }
            else
                return false;
        }

        public override int GetHashCode()
        {
            lock (_lock)
#if NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER
                return HashCode.Combine(state, inc);
#else
            {
                unchecked
                {
                    int hash = 17;
                    hash = hash * 31 + state.GetHashCode();
                    hash = hash * 31 + inc.GetHashCode();
                    return hash;
                }
            }
#endif
        }

        /// Equality operator
        public static bool operator ==(Pcg32 lhs, Pcg32 rhs) => lhs.Equals(rhs);
        /// Inequality operator
        public static bool operator !=(Pcg32 lhs, Pcg32 rhs) => !lhs.Equals(rhs);
    }
}
