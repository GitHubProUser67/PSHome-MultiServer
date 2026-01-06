using System.Security.Cryptography;

namespace CastleLibrary.S0ny.PS3_Creator
{
    internal abstract class Decryptor
    {

        public virtual void DoInit(byte[] key, byte[] iv) { }

        public virtual void DoUpdate(byte[] i, int inOffset, byte[] o, int outOffset, int len) { }
    }

    internal class NoCrypt : Decryptor
    {

        public override void DoInit(byte[] key, byte[] iv)
        {
            // Do nothing
        }

        public override void DoUpdate(byte[] i, int inOffset, byte[] o, int outOffset, int len)
        {
            ConversionUtils.Arraycopy(i, inOffset, o, outOffset, len);
        }
    }

    internal class AESCBC128Decrypt : Decryptor
    {
        Aes c;
        ICryptoTransform ct;
        public override void DoInit(byte[] key, byte[] iv)
        {
            try
            {
                c = Aes.Create();
                c.Padding = PaddingMode.None;
                c.Mode = CipherMode.CBC;
                c.Key = key;
                c.IV = iv;
                ct = c.CreateDecryptor();
            }
            catch
            {
            }
        }

        public override void DoUpdate(byte[] i, int inOffset, byte[] o, int outOffset, int len)
        {
            try
            {
                ct.TransformBlock(i, inOffset, len, o, outOffset);
            }
            catch
            {
            }
        }
    }

    internal class AESCBC128Encrypt : Decryptor
    {
        Aes c;
        ICryptoTransform ct;
        public override void DoInit(byte[] key, byte[] iv)
        {
            try
            {
                c = Aes.Create();
                c.Padding = PaddingMode.None;
                c.Mode = CipherMode.CBC;
                c.Key = key;
                c.IV = iv;
                ct = c.CreateEncryptor();
            }
            catch
            {
            }
        }

        public override void DoUpdate(byte[] i, int inOffset, byte[] o, int outOffset, int len)
        {
            ct.TransformBlock(i, inOffset, len, o, outOffset);
        }
    }
}
