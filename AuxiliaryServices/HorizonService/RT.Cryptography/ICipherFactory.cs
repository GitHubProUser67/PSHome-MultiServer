namespace Horizon.RT.Cryptography
{
    public interface ICipherFactory
    {
        ICipher CreateNew(CipherContext context);
        ICipher CreateNew(CipherContext context, byte[] publicKey);
        ICipher CreateNew(RSA.RsaKeyPair rsaKeyPair);
    }
}