using CastleLibrary.Utils;
using System;
using System.Security.Cryptography;

namespace CastleLibrary.S0ny.XI5.PSNVerification
{
    public interface ITicketPublicSigningKey
    {
        public string Curve { get; }
        public string Xhex { get; }
        public string Yhex { get; }
        public string PemStr { get
            {
                // Parse X/Y from hex
                byte[] x = Xhex.HexStrToBytes();
                byte[] y = Yhex.HexStrToBytes();

                // Construct uncompressed EC point: 0x04 + X + Y
                byte[] uncompressed = new byte[1 + x.Length + y.Length];
                uncompressed[0] = 0x04;
                Buffer.BlockCopy(x, 0, uncompressed, 1, x.Length);
                Buffer.BlockCopy(y, 0, uncompressed, 1 + x.Length, y.Length);

                // Export SPKI → PEM
                using (var ecdsa = ECDsa.Create(new ECParameters
                {
                    Curve = Curve switch
                    {
                        "secp192r1" => new ECCurve
                        {
                            CurveType = ECCurve.ECCurveType.PrimeShortWeierstrass,
                            A = "FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFEFFFFFFFFFFFFFFFC".HexStrToBytes(),
                            B = "64210519E59C80E70FA7E9AB72243049FEB8DEECC146B9B1".HexStrToBytes(),
                            Prime = "FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFEFFFFFFFFFFFFFFFF".HexStrToBytes(),
                            Order = "FFFFFFFFFFFFFFFFFFFFFFFF99DEF836146BC9B1B4D22831".HexStrToBytes(),
                            Cofactor = new byte[] { 0x01 },
                            G = new ECPoint
                            {
                                X = "188DA80EB03090F67CBF20EB43A18800F4FF0AFD82FF1012".HexStrToBytes(),
                                Y = "07192B95FFC8DA78631011ED6B24CDD573F977A11E794811".HexStrToBytes()
                            }
                        },
                        "secp256r1" => ECCurve.NamedCurves.nistP256,
                        _ => throw new NotSupportedException("Unsupported curve: " + Curve)
                    },
                    Q = new ECPoint { X = x, Y = y }
                }))
                    return new string(PemEncoding.Write("PUBLIC KEY", ecdsa.ExportSubjectPublicKeyInfo()));
            }
        }
    }
}
