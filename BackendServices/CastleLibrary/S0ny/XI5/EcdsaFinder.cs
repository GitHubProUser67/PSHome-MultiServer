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
using CustomLogger;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Math.EC;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
#if !NET6_0_OR_GREATER
using System.Linq;
#endif
namespace CastleLibrary.S0ny.XI5
{
    public static class EcdsaFinder
    {
        private static ECDomainParameters FromX9EcParams(X9ECParameters param) =>
            new ECDomainParameters(param.Curve, param.G, param.N, param.H, param.GetSeed());

        public static ECDomainParameters CurveFromName(string name) => FromX9EcParams(ECNamedCurveTable.GetByName(name));

        public static IEnumerable<ECPoint> RecoverPublicKey(ECDomainParameters curve, XI5Ticket ticket, bool verifyResult = true)
        {
            var points = new List<ECPoint>();
            for (int i = 0; i < 4; i++)
            {
                try
                {
                    ECPoint p = RecoverPubKey(curve, ticket.R, ticket.S, ticket.HashedMessage, i);
                    if (p == null) continue;

                    if (verifyResult)
                    {
                        ECPublicKeyParameters pubKey = new ECPublicKeyParameters(p.Normalize(), curve);
                        ISigner signer = SignerUtilities.GetSigner(ticket.HashName + "withECDSA");
                        signer.Init(false, pubKey);
                        signer.BlockUpdate(ticket.Message, 0, ticket.Message.Length);
                        if (signer.VerifySignature(ticket.SignatureData))
                            points.Add(p);
                    }
                    else
                        points.Add(p);
                }
                catch
                {
                    // ignored
                }
            }

            return points;
        }

        private static ECPoint RecoverPubKey(ECDomainParameters curveParam, BigInteger r, BigInteger s, byte[] hashedMsg, int j)
        {
            if ((3 & j) != j)
                LoggerAccessor.LogWarn("[EcdsaFinder] - RecoverPubKey: The recovery param is more than 2 bits");

            BigInteger n = curveParam.N;
            bool isYOdd = (j & 1) > 0;
            bool isSecondKey = j >> 1 > 0;
            if (r.SignValue <= 0 || r.CompareTo(n) >= 0)
            {
                Console.WriteLine("Invalid r value");
                return null;
            }
            if (s.SignValue <= 0 || s.CompareTo(n) >= 0)
            {
                Console.WriteLine("Invalid s value");
                return null;
            }

            BigInteger x = isSecondKey ? r.Add(n) : r;
            ECPoint rPoint = null;
            try
            {
                rPoint = PointFromX(curveParam, x, isYOdd);
            }
            catch
            {
                // ignored
            }

            if (rPoint == null)
                return null;

            ECPoint nR = rPoint.Multiply(n);
            if (!nR.Equals(curveParam.Curve.Infinity) || !nR.IsValid())
                throw new Exception("[EcdsaFinder] - RecoverPubKey: TnR is not a valid curve point");

            BigInteger rInv = r.ModInverse(n);
#if NET6_0_OR_GREATER
            string hex = Convert.ToHexString(hashedMsg);
#else
            string hex = string.Concat(hashedMsg.Select(b => b.ToString("x2")));
#endif
            BigInteger z = new BigInteger(hex, 16);
            if (z.BitLength > n.BitLength)
                z = z.ShiftRight(z.BitLength - n.BitLength);

            return MultiplyTwo(curveParam, curveParam.G, n.Subtract(z).Multiply(rInv).Mod(n), rPoint, s.Multiply(rInv).Mod(n));
        }

        private static ECPoint PointFromX(ECDomainParameters domain, BigInteger x, bool isOdd)
        {
            BigInteger p = domain.Curve.Field.Characteristic;

            BigInteger beta = IntegerFunctions.Ressol(x.Pow(3).Add(domain.Curve.A.Multiply(domain.Curve.FromBigInteger(x)).ToBigInteger()).Add(domain.Curve.B.ToBigInteger()).Mod(p), p);

            if (beta.Mod(BigInteger.Two).Equals(BigInteger.Zero) ^ !isOdd)
                beta = p.Subtract(beta); // -y % p

            return domain.Curve.CreatePoint(x, beta);
        }

        // p * j + x * k
        private static ECPoint MultiplyTwo(ECDomainParameters curve, ECPoint p, BigInteger j, ECPoint x, BigInteger k)
        {
            int i = Math.Max(j.BitLength, k.BitLength) - 1;
            ECPoint r = curve.Curve.Infinity;
            ECPoint both = p.Add(x);

            while (i >= 0)
            {
                bool kBit = k.TestBit(i);

                r = r.Twice();

                if (j.TestBit(i))
                    r = r.Add(kBit ? both : p);
                else if (kBit)
                    r = r.Add(x);

                --i;
            }

            return r;
        }
    }
}