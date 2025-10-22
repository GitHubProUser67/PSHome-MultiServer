using Org.BouncyCastle.Tls;
using Org.BouncyCastle.Tls.Crypto;
using Org.BouncyCastle.Tls.Crypto.Impl.BC;
using System;
using System.IO;
using System.Reflection;

namespace FixedSsl.Crypto;

/// <summary>
/// Original code from: https://github.com/zivillian/ism7mqtt
/// Credits: https://github.com/valters-tomsons/arcadia
/// </summary>
public class Rc4TlsCrypto : BcTlsCrypto
{
    private readonly bool _writeSslKeyLog = false;

    public Rc4TlsCrypto(bool WriteKeyLog)
    {
        _writeSslKeyLog = WriteKeyLog;
    }

    public override TlsCipher CreateCipher(TlsCryptoParameters cryptoParams, int encryptionAlgorithm, int macAlgorithm)
    {
        if (_writeSslKeyLog)
        {
            byte[] secret = ReflectMasterSecretFromBCTls(cryptoParams.SecurityParameters.MasterSecret) ?? throw new Exception("[Rc4TlsCrypto] - Failed to reflect master secret");

            using StreamWriter sw = File.AppendText("sslkeylog.log");
            sw.WriteLine("CLIENT_RANDOM " + Convert.ToHexString(cryptoParams.SecurityParameters.ClientRandom) + " " + Convert.ToHexString(secret));
        }

        return encryptionAlgorithm switch
        {
            EncryptionAlgorithm.RC4_128 => CreateCipher_RC4(cryptoParams, 16, macAlgorithm),
            EncryptionAlgorithm.RC4_40 => CreateCipher_RC4(cryptoParams, 5, macAlgorithm),
            _ => base.CreateCipher(cryptoParams, encryptionAlgorithm, macAlgorithm),
        };
    }

    public override bool HasEncryptionAlgorithm(int encryptionAlgorithm)
    {
        return encryptionAlgorithm switch
        {
            EncryptionAlgorithm.RC4_128 or EncryptionAlgorithm.RC4_40 => true,
            _ => base.HasEncryptionAlgorithm(encryptionAlgorithm),
        };
    }

    private TlsRc4Cipher CreateCipher_RC4(TlsCryptoParameters cryptoParams, int cipherKeySize, int macAlgorithm)
    {
        return new TlsRc4Cipher(cryptoParams, cipherKeySize, CreateMac(cryptoParams, macAlgorithm),
            CreateMac(cryptoParams, macAlgorithm));
    }

    /// <summary>
    /// Get the master secret from BouncyCastle.
    /// <para>Obtiens le master secret de BouncyCastle.</para>
    /// </summary>
    /// <param name="secret">The secret.</param>
    /// <returns>A byte array.</returns>
    private byte[] ReflectMasterSecretFromBCTls(TlsSecret secret)
    {
        // We need to use reflection to access the master secret from BC
        // because using Extract() destroys the key for subsequent calls
        return (byte[])typeof(BcTlsSecret).GetField("m_data", BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(secret);
    }
}