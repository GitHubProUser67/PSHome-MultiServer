/*
This is free and unencumbered software released into the public domain.

Anyone is free to copy, modify, publish, use, compile, sell, or
distribute this software, either in source code form or as a compiled
binary, for any purpose, commercial or non-commercial, and by any
means.

In jurisdictions that recognize copyright laws, the author or authors
of this software dedicate any and all copyright interest in the
software to the public domain. We make this dedication for the benefit
of the public at large and to the detriment of our heirs and
successors. We intend this dedication to be an overt act of
relinquishment in perpetuity of all present and future rights to this
software under copyright law.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
IN NO EVENT SHALL THE AUTHORS BE LIABLE FOR ANY CLAIM, DAMAGES OR
OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
OTHER DEALINGS IN THE SOFTWARE.

For more information, please refer to <https://unlicense.org>
*/
using Org.BouncyCastle.Math;
using System;

namespace CastleLibrary.Sony.XI5
{
    public static class IntegerFunctions
    {
        private static readonly BigInteger Zero = BigInteger.Zero;

        private static readonly BigInteger One = BigInteger.One;

        private static readonly BigInteger Two = BigInteger.Two;

        // the jacobi function uses this lookup table
        private static readonly int[] JacobiTable = { 0, 1, 0, -1, 0, -1, 0, 1 };

        // Computes the value of the Jacobi symbol (A|B). The following properties
        // hold for the Jacobi symbol which makes it a very efficient way to
        // evaluate the Legendre symbol
        // <p>
        // (A|B) = 0 IF gcd(A,B) &gt; 1<br>
        // (-1|B) = 1 IF n = 1 (mod 1)<br>
        // -1|B) = -1 IF n = 3 (mod 4)<br>
        // (A|B) (C|B) = (AC|B)<br>
        // (A|B) (A|C) = (A|CB)<br>
        // (A|B) = (C|B) IF A = C (mod B)<br>
        // (2|B) = 1 IF N = 1 OR 7 (mod 8)<br>
        // (2|B) = 1 IF N = 3 OR 5 (mod 8)
        //
        // @param A integer value
        // @param B integer value
        // @return value of the jacobi symbol (A|B)
        //
        private static int Jacobi(BigInteger A, BigInteger B)
        {
            BigInteger a, b, v;
            long k = 1;

            k = 1;

            // test trivial cases
            if (B.Equals(Zero))
            {
                a = A.Abs();
                return a.Equals(One) ? 1 : 0;
            }

            if (!A.TestBit(0) && !B.TestBit(0))
                return 0;

            a = A;
            b = B;

            if (b.SignValue == -1)
            {
                // b < 0
                b = b.Negate(); // b = -b
                if (a.SignValue == -1)
                    k = -1;
            }

            v = Zero;
            while (!b.TestBit(0))
            {
                v = v.Add(One); // v = v + 1
                b = b.Divide(Two); // b = b/2
            }

            if (v.TestBit(0))
                k = k * JacobiTable[a.IntValue & 7];

            if (a.SignValue < 0)
            {
                // a < 0
                if (b.TestBit(1))
                    k = -k; // k = -k

                a = a.Negate(); // a = -a
            }

            // main loop
            while (a.SignValue != 0)
            {
                v = Zero;
                while (!a.TestBit(0))
                {
                    // a is even
                    v = v.Add(One);
                    a = a.Divide(Two);
                }

                if (v.TestBit(0))
                    k = k * JacobiTable[b.IntValue & 7];

                if (a.CompareTo(b) < 0)
                {
                    // a < b
                    // swap and correct intermediate result
                    (a, b) = (b, a);
                    if (a.TestBit(1) && b.TestBit(1))
                        k = -k;
                }

                a = a.Subtract(b);
            }

            return b.Equals(One) ? (int)k : 0;
        }

        /**
         * Computes the square root of a BigInteger modulo a prime employing the
         * Shanks-Tonelli algorithm.
         *
         * @param a value out of which we extract the square root
         * @param p prime modulus that determines the underlying field
         * @return a number <tt>b</tt> such that b<sup>2</sup> = a (mod p) if
         *         <tt>a</tt> is a quadratic residue modulo <tt>p</tt>.
         * @throws IllegalArgumentException if <tt>a</tt> is a quadratic non-residue modulo <tt>p</tt>
         */
        public static BigInteger Ressol(BigInteger a, BigInteger p)
        {
            BigInteger v;

            if (a.CompareTo(Zero) < 0)
                a = a.Add(p);

            if (Zero.Equals(a))
                return Zero;

            if (Two.Equals(p))
                return a;

            // p = 3 mod 4
            if (p.TestBit(0) && p.TestBit(1))
            {
                if (Jacobi(a, p) != 1) throw new ArgumentException("[IntegerFunctions] - Ressol: No quadratic residue: " + a + ", " + p);

                // a quadr. residue mod p
                v = p.Add(One); // v = p+1
                v = v.ShiftRight(2); // v = v/4
                return a.ModPow(v, p); // return a^v mod p
                                       // return --> a^((p+1)/4) mod p
            }

            // initialization
            // compute k and s, where p = 2^s (2k+1) +1

            long s = 0;
            BigInteger k = p.Subtract(One); // k = p-1
            while (!k.TestBit(0))
            {
                // while k is even
                s++; // s = s+1
                k = k.ShiftRight(1); // k = k/2
            }

            k = k.Subtract(One); // k = k - 1
            k = k.ShiftRight(1); // k = k/2

            // initial values
            BigInteger r = a.ModPow(k, p); // r = a^k mod p

            BigInteger n = r.Multiply(r).Remainder(p); // n = r^2 % p
            n = n.Multiply(a).Remainder(p); // n = n * a % p
            r = r.Multiply(a).Remainder(p); // r = r * a %p

            if (One.Equals(n))
                return r;

            // non-quadratic residue
            BigInteger z = Two; // z = 2
            while (Jacobi(z, p) == 1)
            {
                // while z quadratic residue
                z = z.Add(One); // z = z + 1
            }

            v = k;
            v = v.Multiply(Two); // v = 2k
            v = v.Add(One); // v = 2k + 1
            BigInteger c = z.ModPow(v, p); // c = z^v mod p

            // iteration
            while (n.CompareTo(One) == 1)
            {
                // n > 1
                k = n; // k = n
                long t = s;
                s = 0;

                while (!k.Equals(One))
                {
                    // k != 1
                    k = k.Multiply(k).Mod(p); // k = k^2 % p
                    s++; // s = s + 1
                }

                t -= s; // t = t - s
                if (t == 0)
                    throw new ArgumentException("[IntegerFunctions] - Ressol: No quadratic residue: " + a + ", " + p);

                v = One;
                for (long i = 0; i < t - 1; i++)
                    v = v.ShiftLeft(1); // v = 1 * 2^(t - 1)

                c = c.ModPow(v, p); // c = c^v mod p
                r = r.Multiply(c).Remainder(p); // r = r * c % p
                c = c.Multiply(c).Remainder(p); // c = c^2 % p
                n = n.Multiply(c).Mod(p); // n = n * c % p
            }

            return r;
        }
    }
}