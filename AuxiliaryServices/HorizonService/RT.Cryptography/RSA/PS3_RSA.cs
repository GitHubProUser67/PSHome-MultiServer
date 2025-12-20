using Org.BouncyCastle.Math;

namespace Horizon.RT.Cryptography.RSA
{
    public class PS3_RSA : PS2_RSA
    {
        public PS3_RSA(BigInteger n, BigInteger e, BigInteger d) : base(n, e, d)
        {

        }

        public override void Hash(byte[] input, out byte[] hash)
        {
            hash = RC.PS3_RCQ.Hash(input, Context);
        }

        public override string ToString()
        {
            return $"PS3_RSA({Context}, {N}, {E}, {D})";
        }
    }
}