using System.Collections.Concurrent;
using System.Net;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using CastleLibrary.NetHasher;
using CastleLibrary.Utils;
using CustomLogger;
using EndianTools;
using MultiServerLibrary.Extension;

namespace MultiServerLibrary.SSL
{
    public static class CertificateHelper
    {
        // Change to your liking.
        public static string certificate_email = "MultiServer@gmail.com";

        // PEM file headers.
        public const string certBegin = "-----BEGIN CERTIFICATE-----";
        public const string certEnd = "-----END CERTIFICATE-----";
        public const string keyBegin = "-----BEGIN RSA PRIVATE KEY-----";
        public const string keyEnd = "-----END RSA PRIVATE KEY-----";
        public const string CRT_HEADER = $"{certBegin}\n";
        public const string CRT_FOOTER = $"\n{certEnd}\n";
        public const string PRIVATE_RSA_KEY_HEADER = $"{keyBegin}\n";
        public const string PRIVATE_RSA_KEY_FOOTER = $"\n{keyEnd}";
        public const string PUBLIC_RSA_KEY_HEADER = "-----BEGIN RSA PUBLIC KEY-----\n";
        public const string PUBLIC_RSA_KEY_FOOTER = "\n-----END RSA PUBLIC KEY-----";

        public const string ENTRUST_NET_CA =
            "-----BEGIN CERTIFICATE-----\r\n"
            + "MIIEKjCCAxKgAwIBAgIEOGPe+DANBgkqhkiG9w0BAQUFADCBtDEUMBIGA1UEChML\r\n"
            + "RW50cnVzdC5uZXQxQDA+BgNVBAsUN3d3dy5lbnRydXN0Lm5ldC9DUFNfMjA0OCBp\r\n"
            + "bmNvcnAuIGJ5IHJlZi4gKGxpbWl0cyBsaWFiLikxJTAjBgNVBAsTHChjKSAxOTk5\r\n"
            + "IEVudHJ1c3QubmV0IExpbWl0ZWQxMzAxBgNVBAMTKkVudHJ1c3QubmV0IENlcnRp\r\n"
            + "ZmljYXRpb24gQXV0aG9yaXR5ICgyMDQ4KTAeFw05OTEyMjQxNzUwNTFaFw0yOTA3\r\n"
            + "MjQxNDE1MTJaMIG0MRQwEgYDVQQKEwtFbnRydXN0Lm5ldDFAMD4GA1UECxQ3d3d3\r\n"
            + "LmVudHJ1c3QubmV0L0NQU18yMDQ4IGluY29ycC4gYnkgcmVmLiAobGltaXRzIGxp\r\n"
            + "YWIuKTElMCMGA1UECxMcKGMpIDE5OTkgRW50cnVzdC5uZXQgTGltaXRlZDEzMDEG\r\n"
            + "A1UEAxMqRW50cnVzdC5uZXQgQ2VydGlmaWNhdGlvbiBBdXRob3JpdHkgKDIwNDgp\r\n"
            + "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEArU1LqRKGsuqjIAcVFmQq\r\n"
            + "K0vRvwtKTY7tgHalZ7d4QMBzQshowNtTK91euHaYNZOLGp18EzoOH1u3Hs/lJBQe\r\n"
            + "sYGpjX24zGtLA/ECDNyrpUAkAH90lKGdCCmziAv1h3edVc3kw37XamSrhRSGlVuX\r\n"
            + "MlBvPci6Zgzj/L24ScF2iUkZ/cCovYmjZy/Gn7xxGWC4LeksyZB2ZnuU4q941mVT\r\n"
            + "XTzWnLLPKQP5L6RQstRIzgUyVYr9smRMDuSYB3Xbf9+5CFVghTAp+XtIpGmG4zU/\r\n"
            + "HoZdenoVve8AjhUiVBcAkCaTvA5JaJG/+EfTnZVCwQ5N328mz8MYIWJmQ3DW1cAH\r\n"
            + "4QIDAQABo0IwQDAOBgNVHQ8BAf8EBAMCAQYwDwYDVR0TAQH/BAUwAwEB/zAdBgNV\r\n"
            + "HQ4EFgQUVeSB0RGAvtiJuQijMfmhJAkWuXAwDQYJKoZIhvcNAQEFBQADggEBADub\r\n"
            + "j1abMOdTmXx6eadNl9cZlZD7Bh/KM3xGY4+WZiT6QBshJ8rmcnPyT/4xmf3IDExo\r\n"
            + "U8aAghOY+rat2l098c5u9hURlIIM7j+VrxGrD9cv3h8Dj1csHsm7mhpElesYT6Yf\r\n"
            + "zX1XEC+bBAlahLVu2B064dae0Wx5XnkcFMXj0EyTO2U87d89vqbllRrDtRnDvV5b\r\n"
            + "u/8j72gZyxKTJ1wDLW8w0B62GqzeWvfRqqgnpv55gcR5mTNXuhKwqeBCbJPKVt7+\r\n"
            + "bYQLCIt+jerXmCHG8+c8eS9enNFMFY3h7CI3zJpDC5fcgJCNs2ebb0gIFVbPv/Er\r\n"
            + "fF6adulZkMV8gzURZVE=\r\n"
            + "-----END CERTIFICATE-----\n";
        public const string CLOUDFLARE_NET_CA =
            "-----BEGIN CERTIFICATE-----\n"
            + "MIIEADCCAuigAwIBAgIID+rOSdTGfGcwDQYJKoZIhvcNAQELBQAwgYsxCzAJBgNV\n"
            + "BAYTAlVTMRkwFwYDVQQKExBDbG91ZEZsYXJlLCBJbmMuMTQwMgYDVQQLEytDbG91\n"
            + "ZEZsYXJlIE9yaWdpbiBTU0wgQ2VydGlmaWNhdGUgQXV0aG9yaXR5MRYwFAYDVQQH\n"
            + "Ew1TYW4gRnJhbmNpc2NvMRMwEQYDVQQIEwpDYWxpZm9ybmlhMB4XDTE5MDgyMzIx\n"
            + "MDgwMFoXDTI5MDgxNTE3MDAwMFowgYsxCzAJBgNVBAYTAlVTMRkwFwYDVQQKExBD\n"
            + "bG91ZEZsYXJlLCBJbmMuMTQwMgYDVQQLEytDbG91ZEZsYXJlIE9yaWdpbiBTU0wg\n"
            + "Q2VydGlmaWNhdGUgQXV0aG9yaXR5MRYwFAYDVQQHEw1TYW4gRnJhbmNpc2NvMRMw\n"
            + "EQYDVQQIEwpDYWxpZm9ybmlhMIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKC\n"
            + "AQEAwEiVZ/UoQpHmFsHvk5isBxRehukP8DG9JhFev3WZtG76WoTthvLJFRKFCHXm\n"
            + "V6Z5/66Z4S09mgsUuFwvJzMnE6Ej6yIsYNCb9r9QORa8BdhrkNn6kdTly3mdnykb\n"
            + "OomnwbUfLlExVgNdlP0XoRoeMwbQ4598foiHblO2B/LKuNfJzAMfS7oZe34b+vLB\n"
            + "yrP/1bgCSLdc1AxQc1AC0EsQQhgcyTJNgnG4va1c7ogPlwKyhbDyZ4e59N5lbYPJ\n"
            + "SmXI/cAe3jXj1FBLJZkwnoDKe0v13xeF+nF32smSH0qB7aJX2tBMW4TWtFPmzs5I\n"
            + "lwrFSySWAdwYdgxw180yKU0dvwIDAQABo2YwZDAOBgNVHQ8BAf8EBAMCAQYwEgYD\n"
            + "VR0TAQH/BAgwBgEB/wIBAjAdBgNVHQ4EFgQUJOhTV118NECHqeuU27rhFnj8KaQw\n"
            + "HwYDVR0jBBgwFoAUJOhTV118NECHqeuU27rhFnj8KaQwDQYJKoZIhvcNAQELBQAD\n"
            + "ggEBAHwOf9Ur1l0Ar5vFE6PNrZWrDfQIMyEfdgSKofCdTckbqXNTiXdgbHs+TWoQ\n"
            + "wAB0pfJDAHJDXOTCWRyTeXOseeOi5Btj5CnEuw3P0oXqdqevM1/+uWp0CM35zgZ8\n"
            + "VD4aITxity0djzE6Qnx3Syzz+ZkoBgTnNum7d9A66/V636x4vTeqbZFBr9erJzgz\n"
            + "hhurjcoacvRNhnjtDRM0dPeiCJ50CP3wEYuvUzDHUaowOsnLCjQIkWbR7Ni6KEIk\n"
            + "MOz2U0OBSif3FTkhCgZWQKOOLo1P42jHC3ssUZAtVNXrCk3fw9/E15k8NPkBazZ6\n"
            + "0iykLhH1trywrKRMVw67F44IE8Y=\n"
            + "-----END CERTIFICATE-----\n";
        public const string LETSENCRYPT_ISRG1_NET_CA =
            "-----BEGIN CERTIFICATE-----\n"
            + "MIIFazCCA1OgAwIBAgIRAIIQz7DSQONZRGPgu2OCiwAwDQYJKoZIhvcNAQELBQAw\n"
            + "TzELMAkGA1UEBhMCVVMxKTAnBgNVBAoTIEludGVybmV0IFNlY3VyaXR5IFJlc2Vh\n"
            + "cmNoIEdyb3VwMRUwEwYDVQQDEwxJU1JHIFJvb3QgWDEwHhcNMTUwNjA0MTEwNDM4\n"
            + "WhcNMzUwNjA0MTEwNDM4WjBPMQswCQYDVQQGEwJVUzEpMCcGA1UEChMgSW50ZXJu\n"
            + "ZXQgU2VjdXJpdHkgUmVzZWFyY2ggR3JvdXAxFTATBgNVBAMTDElTUkcgUm9vdCBY\n"
            + "MTCCAiIwDQYJKoZIhvcNAQEBBQADggIPADCCAgoCggIBAK3oJHP0FDfzm54rVygc\n"
            + "h77ct984kIxuPOZXoHj3dcKi/vVqbvYATyjb3miGbESTtrFj/RQSa78f0uoxmyF+\n"
            + "0TM8ukj13Xnfs7j/EvEhmkvBioZxaUpmZmyPfjxwv60pIgbz5MDmgK7iS4+3mX6U\n"
            + "A5/TR5d8mUgjU+g4rk8Kb4Mu0UlXjIB0ttov0DiNewNwIRt18jA8+o+u3dpjq+sW\n"
            + "T8KOEUt+zwvo/7V3LvSye0rgTBIlDHCNAymg4VMk7BPZ7hm/ELNKjD+Jo2FR3qyH\n"
            + "B5T0Y3HsLuJvW5iB4YlcNHlsdu87kGJ55tukmi8mxdAQ4Q7e2RCOFvu396j3x+UC\n"
            + "B5iPNgiV5+I3lg02dZ77DnKxHZu8A/lJBdiB3QW0KtZB6awBdpUKD9jf1b0SHzUv\n"
            + "KBds0pjBqAlkd25HN7rOrFleaJ1/ctaJxQZBKT5ZPt0m9STJEadao0xAH0ahmbWn\n"
            + "OlFuhjuefXKnEgV4We0+UXgVCwOPjdAvBbI+e0ocS3MFEvzG6uBQE3xDk3SzynTn\n"
            + "jh8BCNAw1FtxNrQHusEwMFxIt4I7mKZ9YIqioymCzLq9gwQbooMDQaHWBfEbwrbw\n"
            + "qHyGO0aoSCqI3Haadr8faqU9GY/rOPNk3sgrDQoo//fb4hVC1CLQJ13hef4Y53CI\n"
            + "rU7m2Ys6xt0nUW7/vGT1M0NPAgMBAAGjQjBAMA4GA1UdDwEB/wQEAwIBBjAPBgNV\n"
            + "HRMBAf8EBTADAQH/MB0GA1UdDgQWBBR5tFnme7bl5AFzgAiIyBpY9umbbjANBgkq\n"
            + "hkiG9w0BAQsFAAOCAgEAVR9YqbyyqFDQDLHYGmkgJykIrGF1XIpu+ILlaS/V9lZL\n"
            + "ubhzEFnTIZd+50xx+7LSYK05qAvqFyFWhfFQDlnrzuBZ6brJFe+GnY+EgPbk6ZGQ\n"
            + "3BebYhtF8GaV0nxvwuo77x/Py9auJ/GpsMiu/X1+mvoiBOv/2X/qkSsisRcOj/KK\n"
            + "NFtY2PwByVS5uCbMiogziUwthDyC3+6WVwW6LLv3xLfHTjuCvjHIInNzktHCgKQ5\n"
            + "ORAzI4JMPJ+GslWYHb4phowim57iaztXOoJwTdwJx4nLCgdNbOhdjsnvzqvHu7Ur\n"
            + "TkXWStAmzOVyyghqpZXjFaH3pO3JLF+l+/+sKAIuvtd7u+Nxe5AW0wdeRlN8NwdC\n"
            + "jNPElpzVmbUq4JUagEiuTDkHzsxHpFKVK7q4+63SM1N95R1NbdWhscdCb+ZAJzVc\n"
            + "oyi3B43njTOQ5yOf+1CceWxG1bQVs5ZufpsMljq4Ui0/1lvh+wjChP4kqKOJ2qxq\n"
            + "4RgqsahDYVvTH9w7jXbyLeiNdd8XM2w9U/t7y0Ff/9yi0GE44Za4rF2LN9d11TPA\n"
            + "mRGunUHBcnWEvgJBQl9nJEiU0Zsnvgc/ubhPgXRR4Xq37Z0j4r7g1SgEEzwxA57d\n"
            + "emyPxgcYxn/eR44/KJ4EBs+lVDR3veyJm+kXQ99b21/+jh5Xos1AnX5iItreGCc=\n"
            + "-----END CERTIFICATE-----\n";
        public const string LETSENCRYPT_ISRG2_NET_CA =
            "-----BEGIN CERTIFICATE-----\n"
            + "MIICGzCCAaGgAwIBAgIQQdKd0XLq7qeAwSxs6S+HUjAKBggqhkjOPQQDAzBPMQsw\n"
            + "CQYDVQQGEwJVUzEpMCcGA1UEChMgSW50ZXJuZXQgU2VjdXJpdHkgUmVzZWFyY2gg\n"
            + "R3JvdXAxFTATBgNVBAMTDElTUkcgUm9vdCBYMjAeFw0yMDA5MDQwMDAwMDBaFw00\n"
            + "MDA5MTcxNjAwMDBaME8xCzAJBgNVBAYTAlVTMSkwJwYDVQQKEyBJbnRlcm5ldCBT\n"
            + "ZWN1cml0eSBSZXNlYXJjaCBHcm91cDEVMBMGA1UEAxMMSVNSRyBSb290IFgyMHYw\n"
            + "EAYHKoZIzj0CAQYFK4EEACIDYgAEzZvVn4CDCuwJSvMWSj5cz3es3mcFDR0HttwW\n"
            + "+1qLFNvicWDEukWVEYmO6gbf9yoWHKS5xcUy4APgHoIYOIvXRdgKam7mAHf7AlF9\n"
            + "ItgKbppbd9/w+kHsOdx1ymgHDB/qo0IwQDAOBgNVHQ8BAf8EBAMCAQYwDwYDVR0T\n"
            + "AQH/BAUwAwEB/zAdBgNVHQ4EFgQUfEKWrt5LSDv6kviejM9ti6lyN5UwCgYIKoZI\n"
            + "zj0EAwMDaAAwZQIwe3lORlCEwkSHRhtFcP9Ymd70/aTSVaYgLXTWNLxBo1BfASdW\n"
            + "tL4ndQavEi51mI38AjEAi/V3bNTIZargCyzuFJ0nN6T5U6VR5CmD1/iQMVtCnwr1\n"
            + "/q4AaOeMSQ+2b1tbFfLn\n"
            + "-----END CERTIFICATE-----\n";
        private static readonly RSAParameters ROOT_CA_PARAMETERS = new()
        {
            Modulus = Convert.FromBase64String(
                "2ZTXqhDKcw0ncDFYMh4MVTwV/2f8e"
                    + "GMjFom88ZB/a25TT95iziXfz6O+AB57wGvpUGnnRpkYtJ1GnSvUNWzUtGK3G"
                    + "XaYIbPywn6FoUssw9W7kOh2VR8vSulKJsF7xzZRb7X/c5UpWlrU3pMPweAu3"
                    + "svz+v8C9ZXBPZkbdkWjAOzIvzeItoMt+2XX91MJSji78NaGw9y3tGvzl3QaS"
                    + "t2rZqRg3VMWSMl+02CRuYK14ATrgzj6i7fzXKP3HE1Ri9eBhxUhHv2hcV/M5"
                    + "innIOePVvvVGoro8KWI13g+dm7kIlovo1DngmxthaI6mbhHa9/HkqmvIgKnq"
                    + "AbgDKzWhmStqQ=="
            ),
            Exponent = Convert.FromBase64String("AQAB"),
            D = Convert.FromBase64String(
                "grvuQZ9JJYwX0E+14Jcxbd1mkkoW5vcaVCZ"
                    + "6wuLBzPlDUdAbqiYTrp2CQmwOi3XLgKfBcSf4Mj31+eYl4dv8ik5uGfyqOEX"
                    + "5bWe8P0f+I8U+qDklMMxGDErUZSkIiJBYqji+vuI3MLU3Bm1yoFllkDUX6g5"
                    + "j5tAOhkaCu7Pn11tS8HAy2hdhWdq+30FBG4XShDjLqyust2sFRxoICeoh/2n"
                    + "PAnjJeykrFia3awH1s2zEQ9ET4TtXYcA+jrzKB4zaowqmyFMsu3kIJ0qs7Ut"
                    + "qEfZLwi9CFCDX7PFTVVcbTu4IEO18VkdUoKgMDaoT8B/038quxgwFIh2h3wu"
                    + "arIVe6Q=="
            ),
            P = Convert.FromBase64String(
                "9x8uZXEtImprq9b8cmeiPUZNE8B5RzdCvMr"
                    + "M0+NKmXQ2JV17wn/aXjuY+b4BEKRJZznvPl5nU3tnCCUHYXIfWG2GIONYvD+"
                    + "JpQKpN5pIc9oas8BQnH0e7EIzl/i6EAMM1dJozisxityCfRINEHkWiJx37G7"
                    + "uPDm7OkNyYBE+E/8="
            ),
            Q = Convert.FromBase64String(
                "4WX5Oku7T1ALMAA/fsLRRwQ6/qt/eQzc4v/"
                    + "i8pfFwND6LBO0CidkB0GM6umD9ImjdvffZGWyjZDPFskEAweJXU3lorcFqea"
                    + "HiLa+Z9T/F/fgsKV6ToJ1l2jbifW9WpPe+1lUFj9s2M4ZCiQE61bgq1zPfIJ"
                    + "NrHDdbZy5rYwMHlc="
            ),
            DP = Convert.FromBase64String(
                "fl5Ckns6clPrNVddhn86No1Bku0k12cJyJ"
                    + "MIBP5AwpHrslXImKBaoT9mracc0k7Afnngvor12XnMKR0OViVOpCB1q1G2qa"
                    + "TwFSJ0N8u8awnIB807K5rL+lKsIXV+Z/u3T4wmLe9miTTTwXM+nQLepAMnTA"
                    + "854jA/br7YuQl4Li8="
            ),
            DQ = Convert.FromBase64String(
                "lOxQWDETaFrlmWiAi1ti9L4Z0Iw1ZCCYjS"
                    + "8unsSitzwcHyVBjnfqQlUQK2HwepC6PW+W3PnImHp2KYLVML85BjnioLi2eE"
                    + "RFhpHfijET/p0bivs6rUbLNSfl7eg8nO0Ypg+mXDC51SGPL8EOswOq2+4tdQ"
                    + "GPGoFT/AlSMRVYKG8="
            ),
            InverseQ = Convert.FromBase64String(
                "KSuQxwk41mqOaEBazuMd7xDvsbV7"
                    + "yrJMlxg14nhMusVJRSUciJdJ34RrJgHCA0zxgxvRSX3T7l0j7VcCtcrgBwcX"
                    + "wwZ+eoQTa+wMnM2WF4H7HkgWGdJFBmE/nxyVPoix9Zgl9yft/3a5i9MJQQYv"
                    + "UgfijpR+3SzmknNWj8sMcZM="
            ),
        };

        private static readonly string[] tlds =
        {
            // Most common generic TLDs
            ".com",
            ".org",
            ".net",
            ".online",
            // Major country-code TLDs
            ".us",
            ".uk",
            ".ca",
            ".au",
            ".de",
            ".fr",
            ".es",
            ".it",
            ".nl",
            ".se",
            ".no",
            ".fi",
            ".dk",
            ".eu",
            // Large internet markets
            ".jp",
            ".cn",
            ".in",
            ".br",
            ".ru",
            // Reserved / testing
            ".example",
        };

        private static readonly string[] certExtensions = { string.Empty, ".cer", ".pem", ".pfx" };
        private static readonly string[] keyExtensions = { string.Empty, ".pem", ".pfx", ".pvk" };

        private static readonly ConcurrentDictionary<string, X509Certificate2> FakeCertificates =
            new();

        /// <summary>
        /// Creates a Root CA Cert for chain signed usage.
        /// <para>Creation d'un certificat Root pour usage sur une chaine de certificats.</para>
        /// </summary>
        /// <param name="directoryPath">The output RootCA filename.</param>
        /// <param name="Hashing">The Hashing algorithm to use.</param>
        /// <returns>A X509Certificate2.</returns>
        public static X509Certificate2 CreateRootCertificateAuthority(
            string OutputCertificatePath,
            HashAlgorithmName Hashing,
            string CN = "MultiServer Certificate Authority",
            string OU = "Scientists Department",
            string O = "MultiServer Corp",
            string L = "New York",
            string S = "Northeastern United",
            string C = "US"
        )
        {
            var certDirectoryPath = Path.GetDirectoryName(OutputCertificatePath);

            var certSerialNumber = new byte[16];

            using (var rsa = RSA.Create())
            {
                rsa.ImportParameters(ROOT_CA_PARAMETERS);

                var request = new CertificateRequest(
                    $"CN={CN}, OU={OU}, O=\"{O}\", L={L}, S={S}, C={C}",
                    rsa,
                    Hashing,
                    RSASignaturePadding.Pkcs1
                );

                // Configure the certificate as CA.
                request.CertificateExtensions.Add(
                    new X509BasicConstraintsExtension(true, true, 12, true)
                );

                // Configure the certificate for Digital Signature and Key Encipherment.
                request.CertificateExtensions.Add(
                    new X509KeyUsageExtension(X509KeyUsageFlags.KeyCertSign, true)
                );

                var RootCACertificate = request
                    .Create(
                        request.SubjectName,
                        CreateSignatureGenerator(rsa),
                        new DateTimeOffset(new DateTime(2011, 1, 1)),
                        new DateTimeOffset(new DateTime(2130, 1, 1)),
                        certSerialNumber
                    )
                    .CopyWithPrivateKey(rsa);

                var PemRootCACertificate =
                    CRT_HEADER
                    + Convert.ToBase64String(
                        RootCACertificate.RawData,
                        Base64FormattingOptions.InsertLineBreaks
                    )
                    + CRT_FOOTER;

                File.WriteAllText(
                    certDirectoryPath
                        + $"/{Path.GetFileNameWithoutExtension(OutputCertificatePath)}_privkey.pem",
                    PRIVATE_RSA_KEY_HEADER
                        + Convert.ToBase64String(
                            rsa.ExportRSAPrivateKey(),
                            Base64FormattingOptions.InsertLineBreaks
                        )
                        + PRIVATE_RSA_KEY_FOOTER
                );

                rsa.Clear();

                File.WriteAllText(
                    certDirectoryPath
                        + $"/{Path.GetFileNameWithoutExtension(OutputCertificatePath)}.pem",
                    PemRootCACertificate
                );

                var outputExtension =
                    certExtensions.FirstOrDefault(ext =>
                        Path.GetExtension(OutputCertificatePath)
                            .Equals(ext, StringComparison.OrdinalIgnoreCase)
                    ) ?? ".pfx";

                if (".cer".Equals(outputExtension, StringComparison.OrdinalIgnoreCase))
                    File.WriteAllBytes(
                        OutputCertificatePath,
                        RootCACertificate.Export(X509ContentType.Cert, string.Empty)
                    );
                else if (".pem".Equals(outputExtension, StringComparison.OrdinalIgnoreCase))
                {
                    // Do nothing, we saved it as a pem previously.
                }
                else
                    File.WriteAllBytes(
                        OutputCertificatePath,
                        RootCACertificate.Export(X509ContentType.Pfx, string.Empty)
                    );

                return RootCACertificate;
            }
        }

        /// <summary>
        /// Issue a chain-signed SSL certificate with private key.
        /// </summary>
        /// <param name="certSubject">Certificate subject (domain name).</param>
        /// <param name="issuerCertificate">Authority's certificate used to sign this certificate.</param>
        /// <param name="serverIp">IP Address of the remote server.</param>
        /// <param name="certHashAlgorithm">Certificate hash algorithm.</param>
        /// <param name="certVaildBeforeNow">Minimum Certificate validity Date.</param>
        /// <param name="certVaildAfterNow">Maximum Certificate validity Date.</param>
        /// <param name="wildcard">(optional) Enables wildcard SAN attributes.</param>
        /// <returns>Signed chain of SSL Certificates.</returns>
        public static X509Certificate MakeChainSignedCert(
            string certSubject,
            X509Certificate2 issuerCertificate,
            HashAlgorithmName certHashAlgorithm,
            IPAddress serverIp,
            DateTimeOffset certVaildBeforeNow,
            DateTimeOffset certVaildAfterNow,
            bool wildcard = false,
            string OU = "Wizards Department",
            string O = "MultiServer Corp",
            string L = "New York",
            string S = "Northeastern United",
            string C = "US"
        )
        {
            // Look if it is already issued.
            // Why: https://support.mozilla.org/en-US/kb/Certificate-contains-the-same-serial-number-as-another-certificate
            if (FakeCertificates.TryGetValue(certSubject, out var CachedCertificate))
            {
                //check that it hasn't expired
                if (
                    CachedCertificate.NotAfter > DateTime.Now
                    && CachedCertificate.NotBefore < DateTime.Now
                )
                {
                    return CachedCertificate;
                }
                else
                {
                    FakeCertificates.Remove(certSubject, out _);
                }
            }

            using (
                var RootCAPrivateKey =
                    CastleLibrary.FixedSsl.BCSSLCertificate.GetPrivateKey(issuerCertificate)
                    ?? throw new Exception(
                        "[CertificateHelper] - Issuer Certificate doesn't have a private key, Chain Signed Certificate will not be generated."
                    )
            )
            {
                using (var rsa = RSA.Create())
                {
                    var certSerialNumber = new byte[16];
                    RandomNumberGenerator.Fill(certSerialNumber);

                    var certRequestAny = new CertificateRequest(
                        $"CN={certSubject} [{GetRandomInt64(100, 999)}], OU={OU},"
                            + $" O=\"{O}\", L={L}, S={S}, C={C}",
                        rsa,
                        certHashAlgorithm,
                        RSASignaturePadding.Pkcs1
                    );

                    var sanBuilder = new SubjectAlternativeNameBuilder();

                    sanBuilder.AddDnsName(certSubject); // Some legacy clients will not recognize the cert serial-number.
                    sanBuilder.AddEmailAddress(certificate_email);
                    sanBuilder.AddIpAddress(serverIp);

                    if (wildcard)
                    {
                        tlds.Select(tld => "*" + tld).ToList().ForEach(sanBuilder.AddDnsName);
                    }

                    certRequestAny.CertificateExtensions.Add(sanBuilder.Build());

                    var certificateWithKey = new X509Certificate2(
                        certRequestAny
                            .Create(
                                issuerCertificate.IssuerName,
                                CreateSignatureGenerator(RootCAPrivateKey),
                                certVaildBeforeNow,
                                certVaildAfterNow,
                                certSerialNumber
                            )
                            .CopyWithPrivateKey(rsa)
                            .Export(X509ContentType.Pfx),
                        (string)null,
                        X509KeyStorageFlags.Exportable
                    );

                    // Save the certificate and return it.
                    FakeCertificates.TryAdd(certSubject, certificateWithKey);
                    return certificateWithKey;
                }
            }
        }

        /// <summary>
        /// Creates a master chained signed certificate.
        /// <para>Creation d'un certificat master sur une chaine de certificats issue d'un RootCA.</para>
        /// </summary>
        /// <param name="issuerCertificate">The initial RootCA.</param>
        /// <param name="Hashing">The Hashing algorithm to use.</param>
        /// <param name="OutputCertificatePath">The output chained signed certificate file path.</param>
        /// <param name="OutputCertificatePassword">The password of the output chained signed certificate.</param>
        /// <param name="DnsList">DNS to set in the SAN attributes.</param>
        public static void MakeMasterChainSignedCert(
            X509Certificate2 issuerCertificate,
            HashAlgorithmName Hashing,
            string OutputCertificatePath,
            string OutputCertificatePassword,
            string[] DnsList,
            string CN = "MultiServerCorp.online",
            string OU = "Scientists Department",
            string O = "MultiServer Corp",
            string L = "New York",
            string S = "Northeastern United",
            string C = "US",
            bool Wildcard = true
        )
        {
            if (issuerCertificate == null)
                return;

            using (
                var RootCAPrivateKey =
                    CastleLibrary.FixedSsl.BCSSLCertificate.GetPrivateKey(issuerCertificate)
                    ?? throw new Exception(
                        "[CertificateHelper] - Issuer Certificate doesn't have a private key, Chain Signed Certificate will not be generated."
                    )
            )
            {
                var CurrentDate = DateTime.Now;

                var certSerialNumber = new byte[16];
                RandomNumberGenerator.Fill(certSerialNumber);
                using (var rsa = RSA.Create())
                {
                    var Loopback = IPAddress.Loopback;
                    var LocalServerIP = InternetProtocolUtils.GetOutboundIPAddresses().First();
                    var PublicServerIP = InternetProtocolUtils
                        .TryGetServerIP(out var publicIP)
                        .Result
                        ? IPAddress.Parse(publicIP)
                        : LocalServerIP;

                    var sanBuilder = new SubjectAlternativeNameBuilder();

                    var request = new CertificateRequest(
                        $"CN={CN} [{GetRandomInt64(100, 999)}], OU={OU}, O=\"{O}\", L={L}, S={S}, C={C}",
                        rsa,
                        Hashing,
                        RSASignaturePadding.Pkcs1
                    );

                    DnsList
                        ?.Select(str => str) // Some clients do not allow wildcard domains, so we use SAN attributes as a fallback.
                        .ToList()
                        .ForEach(sanBuilder.AddDnsName);

                    if (Wildcard)
                        sanBuilder.AddDnsName("*");

                    sanBuilder.AddDnsName("localhost");
                    sanBuilder.AddDnsName(Loopback.ToString());
                    sanBuilder.AddIpAddress(Loopback);
                    sanBuilder.AddDnsName(PublicServerIP.ToString());
                    sanBuilder.AddIpAddress(PublicServerIP);

                    if (PublicServerIP != LocalServerIP)
                    {
                        sanBuilder.AddDnsName(LocalServerIP.ToString());
                        sanBuilder.AddIpAddress(LocalServerIP);
                    }

                    sanBuilder.AddEmailAddress(certificate_email);

                    request.CertificateExtensions.Add(sanBuilder.Build());

                    var ChainSignedCert = request
                        .Create(
                            issuerCertificate.IssuerName,
                            CreateSignatureGenerator(RootCAPrivateKey),
                            new DateTimeOffset(CurrentDate.AddDays(-1)),
                            new DateTimeOffset(CurrentDate.AddYears(100)),
                            certSerialNumber
                        )
                        .CopyWithPrivateKey(rsa);

                    File.WriteAllText(
                        Path.GetDirectoryName(OutputCertificatePath)
                            + $"/{Path.GetFileNameWithoutExtension(OutputCertificatePath)}_privkey.pem",
                        PRIVATE_RSA_KEY_HEADER
                            + Convert.ToBase64String(
                                rsa.ExportRSAPrivateKey(),
                                Base64FormattingOptions.InsertLineBreaks
                            )
                            + PRIVATE_RSA_KEY_FOOTER
                    );

                    File.WriteAllText(
                        Path.GetDirectoryName(OutputCertificatePath)
                            + $"/{Path.GetFileNameWithoutExtension(OutputCertificatePath)}_pubkey.pem",
                        PUBLIC_RSA_KEY_HEADER
                            + Convert.ToBase64String(
                                rsa.ExportRSAPublicKey(),
                                Base64FormattingOptions.InsertLineBreaks
                            )
                            + PUBLIC_RSA_KEY_FOOTER
                    );

                    File.WriteAllText(
                        Path.GetDirectoryName(OutputCertificatePath)
                            + $"/{Path.GetFileNameWithoutExtension(OutputCertificatePath)}.pem",
                        CRT_HEADER
                            + Convert.ToBase64String(
                                ChainSignedCert.RawData,
                                Base64FormattingOptions.InsertLineBreaks
                            )
                            + CRT_FOOTER
                    );

                    var outputExtension =
                        certExtensions.FirstOrDefault(ext =>
                            Path.GetExtension(OutputCertificatePath)
                                .Equals(ext, StringComparison.OrdinalIgnoreCase)
                        ) ?? ".pfx";

                    if (".cer".Equals(outputExtension, StringComparison.OrdinalIgnoreCase))
                        File.WriteAllBytes(
                            OutputCertificatePath,
                            ChainSignedCert.Export(X509ContentType.Cert, OutputCertificatePassword)
                        );
                    else if (".pem".Equals(outputExtension, StringComparison.OrdinalIgnoreCase))
                    {
                        // Do nothing, we saved it as a pem previously.
                    }
                    else
                        File.WriteAllBytes(
                            OutputCertificatePath,
                            ChainSignedCert.Export(X509ContentType.Pfx, OutputCertificatePassword)
                        );

                    rsa.Clear();
                }
            }
        }

        /// <summary>
        /// Initiate the certificate generation routine.
        /// <para>Initialise la génération de certificats.</para>
        /// </summary>
        /// <param name="certPath">Output cert path.</param>
        /// <param name="certPassword">Password of the certificate file.</param>
        /// <param name="DnsList">DNS domains to include in the certificate.</param>
        /// <param name="Hashing">The Hashing algorithm to use.</param>
        public static void InitializeSSLChainSignedCertificates(
            string certPath,
            string certPassword,
            string[] DnsList,
            HashAlgorithmName Hashing
        )
        {
            if (string.IsNullOrEmpty(certPath))
            {
                LoggerAccessor.LogError(
                    "[CertificateHelper] - InitializeSSLChainSignedCertificates: Invalid certificate file path parameter."
                );
                return;
            }

            string directoryPath = null;

            try
            {
                directoryPath = Path.GetDirectoryName(certPath);
            }
            catch { }

            if (string.IsNullOrEmpty(directoryPath))
            {
                LoggerAccessor.LogError(
                    "[CertificateHelper] - InitializeSSLChainSignedCertificates: Invalid certificate file path."
                );
                return;
            }

            Directory.CreateDirectory(directoryPath);

            (string, X509Certificate2) rootCACertificate = default;

            try
            {
                using (var mutex = new Mutex(false, $"Global\\{nameof(CertificateHelper)}Lock"))
                {
                    MutexExtensions.TryWithMutex(
                        mutex,
                        null,
                        () =>
                        {
                            // Try to find a suitable RootCA.
                            foreach (
                                var certFile in Directory
                                    .GetFiles(directoryPath)
                                    .Where(f =>
                                        certExtensions.Contains(Path.GetExtension(f).ToLower())
                                    )
                            )
                            {
                                var baseName = Path.GetFileNameWithoutExtension(certFile);

                                X509Certificate2 cert = null;

                                try
                                {
                                    cert = LoadCertificate(
                                        certFile,
                                        keyExtensions
                                            .Select(ext =>
                                                Path.Combine(
                                                    directoryPath,
                                                    baseName + "_privkey" + ext
                                                )
                                            )
                                            .FirstOrDefault(File.Exists)
                                    );
                                }
                                catch
                                {
                                    // Not Important.
                                }

                                if (cert != null && IsCertificateAuthority(cert))
                                {
                                    rootCACertificate = (
                                        directoryPath
                                            + $"/{Path.GetFileNameWithoutExtension(certFile)}.pem",
                                        cert
                                    );
                                    break; // Use the first valid CA
                                }
                            }

                            // If no valid CA found, create a new one
                            if (rootCACertificate == default)
                            {
                                const string rootCaFileName = "MultiServer_rootca";

                                rootCACertificate = (
                                    directoryPath + $"/{rootCaFileName}.pem",
                                    CreateRootCertificateAuthority(
                                        Path.Combine(directoryPath, $"{rootCaFileName}.pfx"),
                                        HashAlgorithmName.SHA256
                                    )
                                );
                            }

                            if (File.Exists(rootCACertificate.Item1))
                                CreateCertificatesTextFile(
                                    rootCACertificate.Item1,
                                    directoryPath + "/CERTIFICATES.TXT"
                                );
                            else
                                LoggerAccessor.LogWarn(
                                    "[CertificateHelper] - No PEM certificate matched for the RootCA, skipping CERTIFICATES.TXT file generation..."
                                );

                            MakeMasterChainSignedCert(
                                rootCACertificate.Item2,
                                Hashing,
                                certPath,
                                certPassword,
                                DnsList
                            );
                        }
                    );
                }
            }
            catch (Exception e)
            {
                LoggerAccessor.LogError(
                    $"[CertificateHelper] - InitializeSSLChainSignedCertificates: Failed to get mutex or generating certificate (exception: {e})"
                );
            }
        }

        /// <summary>
        /// Checks if the X509Certificate is of Certificate Authority type.
        /// </summary>
        /// <param name="certificate">The certificate to check on.</param>
        /// <returns>A bool.</returns>
        public static bool IsCertificateAuthority(X509Certificate certificate)
        {
            // Compare the Issuer and Subject properties of the certificate
            return certificate.Issuer == certificate.Subject;
        }

        /// <summary>
        /// Initiate a X509Certificate2 from a certificate and a privatekey.
        /// <para>Initialise un certificat X509Certificate2 depuis un fichier certificate et un fichier privatekey.</para>
        /// </summary>
        /// <param name="certificatePathInput">cert path.</param>
        /// <param name="privateKeyPathInput">private key path.</param>
        /// <param name="pfxCertificatePassword">pfx cert password.</param>
        /// <param name="pfxPrivateKeyPassword">pfx private key password.</param>
        /// <returns>A X509Certificate2.</returns>
        public static X509Certificate2 LoadCertificate(
            string certificatePathInput,
            string privateKeyPathInput = null,
            string pfxCertificatePassword = null,
            string pfxPrivateKeyPassword = null
        )
        {
            // Find first existing certificate file
            var certificatePath = Path.HasExtension(certificatePathInput)
                ? File.Exists(certificatePathInput)
                    ? certificatePathInput
                    : null
                : certExtensions
                    .Select(ext => certificatePathInput + ext)
                    .FirstOrDefault(File.Exists);

            if (certificatePath != null)
            {
#if DEBUG
                LoggerAccessor.LogInfo(
                    $"[CertificateHelper] - LoadCertificate: Using certificate: {certificatePath}"
                );
#endif
                var cert =
                    (
                        pfxCertificatePassword != null
                        && certificatePath.EndsWith(".pfx", StringComparison.OrdinalIgnoreCase)
                    )
                        ? new X509Certificate2(certificatePath, pfxCertificatePassword)
                        : new X509Certificate2(certificatePath);

                if (cert.HasPrivateKey)
                    return cert;

                using (cert)
                {
                    // Find first existing private key file
                    var privateKeyPath = Path.HasExtension(privateKeyPathInput)
                        ? File.Exists(privateKeyPathInput)
                            ? privateKeyPathInput
                            : null
                        : keyExtensions
                            .Select(ext => privateKeyPathInput + ext)
                            .FirstOrDefault(File.Exists);

                    if (privateKeyPath != null)
                    {
#if DEBUG
                        LoggerAccessor.LogInfo(
                            $"[CertificateHelper] - LoadCertificate: Using private key: {privateKeyPath}"
                        );
#endif
                        AsymmetricAlgorithm key = null;

                        try
                        {
                            key = privateKeyPath.EndsWith(
                                ".pfx",
                                StringComparison.OrdinalIgnoreCase
                            )
                                ? CastleLibrary.FixedSsl.BCSSLCertificate.GetPrivateKey(
                                    pfxPrivateKeyPassword != null
                                        ? new X509Certificate2(
                                            privateKeyPath,
                                            pfxPrivateKeyPassword
                                        )
                                        : new X509Certificate2(privateKeyPath)
                                )
                                : LoadPrivateKey(privateKeyPath);

                            if (key is RSA rsaKey)
                                return new X509Certificate2(
                                    cert.CopyWithPrivateKey(rsaKey).Export(X509ContentType.Pfx)
                                );
                            else if (key is DSA dsaKey)
                                return new X509Certificate2(
                                    cert.CopyWithPrivateKey(dsaKey).Export(X509ContentType.Pfx)
                                );
                            else if (key is ECDsa ecdsaKey)
                                return new X509Certificate2(
                                    cert.CopyWithPrivateKey(ecdsaKey).Export(X509ContentType.Pfx)
                                );
                            else if (key is ECDiffieHellman ecdiffKey)
                                return new X509Certificate2(
                                    cert.CopyWithPrivateKey(ecdiffKey).Export(X509ContentType.Pfx)
                                );
                            else
                                LoggerAccessor.LogError(
                                    $"[CertificateHelper] - LoadCertificate: Unsupported key type."
                                );
                        }
                        catch (Exception ex)
                        {
                            LoggerAccessor.LogError(
                                $"[CertificateHelper] - LoadCertificate: Loading thrown an assertion while loading private key:{privateKeyPath}. (Exception:{ex})"
                            );
                        }
                        finally
                        {
                            key?.Dispose();
                        }
                    }
                    else
                        LoggerAccessor.LogWarn(
                            $"[CertificateHelper] - LoadCertificate: Private key file not found for: {privateKeyPathInput} with {(Path.HasExtension(privateKeyPathInput) ? $"extension {Path.GetExtension(privateKeyPathInput)}" : $"extensions {string.Join(", ", keyExtensions)}")}"
                        );
                }
            }
            else
                LoggerAccessor.LogWarn(
                    $"[CertificateHelper] - LoadCertificate: Certificate file not found for: {certificatePathInput} with {(Path.HasExtension(certificatePathInput) ? $"extension {Path.GetExtension(certificatePathInput)}" : $"extensions {string.Join(", ", certExtensions)}")}"
                );

            return null;
        }

        public static X509Certificate2 LoadCertificateFromPemString(string certPem)
        {
            // Remove PEM header/footer and convert base64
            return new X509Certificate2(
                certPem
                    .Replace(certBegin, string.Empty)
                    .Replace(certEnd, string.Empty)
                    .IsBase64()
                    .DecodedBytes
            );
        }

        public static AsymmetricAlgorithm LoadPrivateKey(string privateKeyPath)
        {
            byte[] keyBytes;
            (bool, byte[]) isPemFormat = (false, null);
            RSA rsa;

            var pemPrivateKeyBlocks = File.ReadAllText(privateKeyPath)
                .Split("-", StringSplitOptions.RemoveEmptyEntries);
            if (pemPrivateKeyBlocks.Length >= 2)
                isPemFormat = pemPrivateKeyBlocks[1].IsBase64();

            ECDsa ecdsa;
            ECDiffieHellman ecdh;

            if (isPemFormat.Item1)
            {
                var header = pemPrivateKeyBlocks[0];
                keyBytes = isPemFormat.Item2;

                if (header == "BEGIN PRIVATE KEY")
                {
                    DSA dsa;

                    try
                    {
                        rsa = RSA.Create();
                        rsa.ImportPkcs8PrivateKey(keyBytes, out _);
                        return rsa;
                    }
                    catch
                    {
                        rsa = null;
                    }

                    try
                    {
                        ecdsa = ECDsa.Create();
                        ecdsa.ImportPkcs8PrivateKey(keyBytes, out _);
                        return ecdsa;
                    }
                    catch
                    {
                        ecdsa = null;
                    }

                    try
                    {
                        ecdh = ECDiffieHellman.Create();
                        ecdh.ImportPkcs8PrivateKey(keyBytes, out _);
                        return ecdh;
                    }
                    catch
                    {
                        ecdh = null;
                    }

                    try
                    {
                        dsa = DSA.Create();
                        dsa.ImportPkcs8PrivateKey(keyBytes, out _);
                        return dsa;
                    }
                    catch
                    {
                        dsa = null;
                    }

                    throw new CryptographicException(
                        "[CertificateHelper] - LoadPrivateKey - Unsupported PKCS8 private key format."
                    );
                }
                else if (header == "BEGIN RSA PRIVATE KEY")
                {
                    rsa = RSA.Create();
                    rsa.ImportRSAPrivateKey(keyBytes, out _);
                    return rsa;
                }
                else if (header == "BEGIN EC PRIVATE KEY")
                {
                    ecdsa = ECDsa.Create();
                    ecdsa.ImportECPrivateKey(keyBytes, out _);
                    return ecdsa;
                }

                throw new CryptographicException(
                    "[CertificateHelper] - LoadPrivateKey - Unsupported pem private key format."
                );
            }

            keyBytes = File.ReadAllBytes(privateKeyPath);

            try
            {
                rsa = RSA.Create();
                rsa.ImportRSAPrivateKey(keyBytes, out _);
                return rsa;
            }
            catch
            {
                rsa = null;
            }

            try
            {
                ecdsa = ECDsa.Create();
                ecdsa.ImportECPrivateKey(keyBytes, out _);
                return ecdsa;
            }
            catch
            {
                ecdsa = null;
            }

            try
            {
                ecdh = ECDiffieHellman.Create();
                ecdh.ImportECPrivateKey(keyBytes, out _);
                return ecdh;
            }
            catch
            {
                ecdh = null;
            }

            throw new CryptographicException(
                "[CertificateHelper] - LoadPrivateKey - Unsupported private key format."
            );
        }

        public static RSA LoadRSAPKCS1PrivateKeyFromPemString(string privateKeyPem)
        {
            // Remove PEM headers
            var keyBytes = privateKeyPem
                .Replace("-----BEGIN PRIVATE KEY-----", string.Empty)
                .Replace("-----END PRIVATE KEY-----", string.Empty)
                .Replace(keyBegin, string.Empty)
                .Replace(keyEnd, string.Empty)
                .Trim()
                .IsBase64()
                .DecodedBytes;

            var rsa = RSA.Create();
            rsa.ImportRSAPrivateKey(keyBytes, out _);
            return rsa;
        }

        public static X509Certificate2 LoadCombinedCertificateAndKeyFromString(
            string certPem,
            string privateKeyPem
        )
        {
            if (!string.IsNullOrEmpty(certPem) && !string.IsNullOrEmpty(privateKeyPem))
            {
                try
                {
                    return LoadCertificateFromPemString(certPem)
                        .CopyWithPrivateKey(LoadRSAPKCS1PrivateKeyFromPemString(privateKeyPem));
                }
                catch
                {
                    // Not Important.
                }
            }

            return null;
        }

        /// <summary>
        /// Get a random int64 number.
        /// <para>Obtiens un nombre int64 random.</para>
        /// </summary>
        /// <param name="minValue">The min value.</param>
        /// <param name="maxValue">The max value.</param>
        /// <returns>A long.</returns>
        public static long GetRandomInt64(long minValue, long maxValue)
        {
            if (minValue >= maxValue)
                throw new ArgumentOutOfRangeException(nameof(maxValue));

            ulong range = (ulong)(maxValue - minValue);

            Span<byte> bytes = stackalloc byte[8];
            ulong limit = ulong.MaxValue - (ulong.MaxValue % range);

            ulong value;
            do
            {
                RandomNumberGenerator.Fill(bytes);
                value = EndianAwareConverter.ToUInt64(bytes, Endianness.Automatic, 0);
            } while (value >= limit);

            return (long)(value % range) + minValue;
        }

        public static void ExtractCombinedPemData(
            string content,
            out string certificate,
            out string privateKey
        )
        {
            var certStart = content.IndexOf(certBegin);
            var certEndIdx = content.IndexOf(certEnd) + certEnd.Length;
            var keyStart = content.IndexOf(keyBegin);
            var keyEndIdx = content.IndexOf(keyEnd) + keyEnd.Length;

            certificate =
                certStart >= 0 && certEndIdx > certStart
                    ? content.Substring(certStart, certEndIdx - certStart).Trim()
                    : string.Empty;

            privateKey =
                keyStart >= 0 && keyEndIdx > keyStart
                    ? content.Substring(keyStart, keyEndIdx - keyStart).Trim()
                    : string.Empty;
        }

        public static bool CertificatesMatch(X509Certificate2 cert1, X509Certificate2 cert2)
        {
            return cert1.RawData.EqualsTo(cert2.RawData);
        }

        private static X509SignatureGenerator CreateSignatureGenerator(
            AsymmetricAlgorithm privateKey
        )
        {
            return privateKey switch
            {
                RSA rsa => new RsaPkcs1SignatureGenerator(rsa),
                ECDsa ecdsa => new EcdsaSignatureGenerator(ecdsa),
                _ => null, // ECDiffieHellman and DSA are not valid for signing certs
            };
        }

        /// <summary>
        /// Creates a specific CERTIFICATES.TXT file only if its content has changed.
        /// <para>Génération d'un fichier CERTIFICATES.TXT.</para>
        /// </summary>
        /// <param name="RootCaFileName">The root CA file.</param>
        /// <param name="FileName">The output file.</param>
        private static void CreateCertificatesTextFile(string RootCaFileName, string FileName)
        {
            string rootcaContent;
            string txtContent = null;

            if (File.Exists(FileName))
            {
                var rootCaTask = File.ReadAllTextAsync(RootCaFileName);
                var txtTask = File.ReadAllTextAsync(FileName);

                Task.WhenAll(rootCaTask, txtTask).Wait();

                rootcaContent = rootCaTask.Result;
                txtContent = txtTask.Result;
            }
            else
                rootcaContent = File.ReadAllText(RootCaFileName);

            var shouldWrite = true;

            var newContent =
                rootcaContent
                + ENTRUST_NET_CA
                + CLOUDFLARE_NET_CA
                + LETSENCRYPT_ISRG1_NET_CA
                + LETSENCRYPT_ISRG2_NET_CA;

            if (txtContent != null)
            {
                try
                {
                    shouldWrite =
                        DotNetHasher.ComputeSHA256String(txtContent)
                        != DotNetHasher.ComputeSHA256String(newContent);
                }
                catch (Exception ex)
                {
                    LoggerAccessor.LogError(
                        $"[CertificateHelper] - CreateCertificatesTextFile: Failed to hash existing file '{FileName}'! (Exception:{ex})"
                    );
                    shouldWrite = true; // fallback to writing
                }
            }

            if (shouldWrite)
            {
#if DEBUG
                LoggerAccessor.LogInfo(
                    $"[CertificateHelper] - CreateCertificatesTextFile: File '{FileName}' updating..."
                );
#endif
                _ = File.WriteAllTextAsync(FileName, newContent);
            }
        }
    }

    /// <summary>
    /// RSA-MD5, RSA-SHA1, RSA-SHA256, RSA-SHA512 signature generator for X509 certificates.
    /// </summary>
    sealed class RsaPkcs1SignatureGenerator : X509SignatureGenerator
    {
        // Workaround for SHA1 and MD5 ban in .NET 4.7.2 and .NET Core.
        // Ideas used from:
        // https://stackoverflow.com/a/59989889/7600726
        // https://github.com/dotnet/corefx/pull/18344/files/c74f630f38b6f29142c8dc73623fdcb4f7905f87#r112066147
        // https://github.com/dotnet/corefx/blob/5fe5f9aae7b2987adc7082f90712b265bee5eefc/src/System.Security.Cryptography.X509Certificates/tests/CertificateCreation/PrivateKeyAssociationTests.cs#L531-L553
        // https://github.com/dotnet/runtime/blob/89f3a9ef41383bb409b69d1a0f0db910f3ed9a34/src/libraries/System.Security.Cryptography/tests/X509Certificates/CertificateCreation/X509Sha1SignatureGenerators.cs#LL31C38-L31C38

        private readonly X509SignatureGenerator _realRsaGenerator;

        internal RsaPkcs1SignatureGenerator(RSA rsa)
        {
            _realRsaGenerator = CreateForRSA(rsa, RSASignaturePadding.Pkcs1);
        }

        protected override PublicKey BuildPublicKey() => _realRsaGenerator.PublicKey;

        /// <summary>
        /// Callback for .NET signing functions.
        /// </summary>
        /// <param name="hashAlgorithm">Hashing algorithm name.</param>
        /// <returns>Hashing algorithm ID in some correct format.</returns>
        public override byte[] GetSignatureAlgorithmIdentifier(HashAlgorithmName hashAlgorithm)
        {
            /*
             * https://bugzilla.mozilla.org/show_bug.cgi?id=1064636#c28
                300d06092a864886f70d0101020500  :md2WithRSAEncryption           1
                300b06092a864886f70d01010b      :sha256WithRSAEncryption        2
                300b06092a864886f70d010105      :sha1WithRSAEncryption          1
                300d06092a864886f70d01010c0500  :sha384WithRSAEncryption        20
                300d06092a864886f70d0101040500  :md5WithRSAEncryption           6512
                300d06092a864886f70d01010d0500  :sha512WithRSAEncryption        7715
                300d06092a864886f70d01010b0500  :sha256WithRSAEncryption        483338
                300d06092a864886f70d0101050500  :sha1WithRSAEncryption          4498605
             */

            const string MD5id = "300D06092A864886F70D0101040500";
            const string SHA1id = "300D06092A864886F70D0101050500";
            const string SHA256id = "300D06092A864886F70D01010B0500";
            const string SHA384id = "300D06092A864886F70D01010C0500"; //?
            const string SHA512id = "300D06092A864886F70D01010D0500";

            var oid = hashAlgorithm.Name switch
            {
                DotNetHasher.MD5Const => MD5id.HexStrToBytes(),
                DotNetHasher.Sha1Const => SHA1id.HexStrToBytes(),
                DotNetHasher.Sha256Const => SHA256id.HexStrToBytes(),
                DotNetHasher.Sha384Const => SHA384id.HexStrToBytes(),
                DotNetHasher.Sha512Const => SHA512id.HexStrToBytes(),
                _ => null,
            };

            if (oid == null)
                LoggerAccessor.LogError(
                    "[RsaPkcs1SignatureGenerator] - " + nameof(hashAlgorithm),
                    $"'{hashAlgorithm}' is not a supported algorithm at this moment."
                );

            return oid;
        }

        /// <summary>
        /// Sign specified <paramref name="data"/> using specified <paramref name="hashAlgorithm"/>.
        /// </summary>
        /// <returns>X.509 signature for specified data.</returns>
        public override byte[] SignData(byte[] data, HashAlgorithmName hashAlgorithm) =>
            _realRsaGenerator.SignData(data, hashAlgorithm);
    }

    /// <summary>
    /// ECDSA-SHA256, ECDSA-SHA384 signature generator for X509 certificates.
    /// </summary>
    sealed class EcdsaSignatureGenerator : X509SignatureGenerator
    {
        private readonly X509SignatureGenerator _realEcGenerator;

        internal EcdsaSignatureGenerator(ECDsa ec)
        {
            _realEcGenerator = CreateForECDsa(ec);
        }

        protected override PublicKey BuildPublicKey() => _realEcGenerator.PublicKey;

        /// <summary>
        /// Callback for .NET signing functions.
        /// </summary>
        /// <param name="hashAlgorithm">Hashing algorithm name.</param>
        /// <returns>Hashing algorithm ID in some correct format.</returns>
        public override byte[] GetSignatureAlgorithmIdentifier(HashAlgorithmName hashAlgorithm)
        {
            /*
             * https://bugzilla.mozilla.org/show_bug.cgi?id=1064636#c28
                300a06082a8648ce3d040303        :ecdsa-with-SHA384              20
                300a06082a8648ce3d040302        :ecdsa-with-SHA256              97
             */

            const string SHA256id = "300A06082A8648CE3D040302";
            const string SHA384id = "300A06082A8648CE3D040303";

            var oid = hashAlgorithm.Name switch
            {
                DotNetHasher.Sha256Const => SHA256id.HexStrToBytes(),
                DotNetHasher.Sha384Const => SHA384id.HexStrToBytes(),
                _ => null,
            };

            if (oid == null)
                LoggerAccessor.LogError(
                    "[EcdsaSignatureGenerator] - " + nameof(hashAlgorithm),
                    $"'{hashAlgorithm}' is not a supported algorithm at this moment."
                );

            return oid;
        }

        /// <summary>
        /// Sign specified <paramref name="data"/> using specified <paramref name="hashAlgorithm"/>.
        /// </summary>
        /// <returns>X.509 signature for specified data.</returns>
        public override byte[] SignData(byte[] data, HashAlgorithmName hashAlgorithm) =>
            _realEcGenerator.SignData(data, hashAlgorithm);
    }
}
