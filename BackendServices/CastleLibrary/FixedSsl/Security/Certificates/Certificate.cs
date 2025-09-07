/*
 *   Mentalis.org Security Library
 * 
 *     Copyright � 2002-2005, The Mentalis.org Team
 *     All rights reserved.
 *     http://www.mentalis.org/
 *
 *
 *   Redistribution and use in source and binary forms, with or without
 *   modification, are permitted provided that the following conditions
 *   are met:
 *
 *     - Redistributions of source code must retain the above copyright
 *        notice, this list of conditions and the following disclaimer. 
 *
 *     - Neither the name of the Mentalis.org Team, nor the names of its contributors
 *        may be used to endorse or promote products derived from this
 *        software without specific prior written permission. 
 *
 *   THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
 *   "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
 *   LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS
 *   FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL
 *   THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT,
 *   INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
 *   (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
 *   SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION)
 *   HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT,
 *   STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
 *   ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED
 *   OF THE POSSIBILITY OF SUCH DAMAGE.
*/

using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace Org.Mentalis.Security.Certificates
{
    /// <summary>
    /// Defines a X509 v3 encoded certificate.
    /// </summary>
    public class Certificate
    {
        public Certificate(X509Certificate certificate)
        {
            UnderlyingCert = new X509Certificate2(certificate);
        }
        /// <summary>
        /// Returns the length of the public key of the X.509v3 certificate.
        /// </summary>
        /// <returns>Returns the length of the public key in bits. If unable to determine the key's length, returns zero.</returns>
        public int GetPublicKeyLength()
        {
            return UnderlyingCert.GetRSAPublicKey().KeySize;
        }
        /// <summary>
        /// Checks whether the <see cref="Certificate"/> has a private key associated with it.
        /// </summary>
        /// <returns><b>true</b> if the certificate has a private key associated with it, <b>false</b> otherwise.</returns>
        public bool HasPrivateKey()
        {
            return UnderlyingCert.HasPrivateKey;
        }
        /// <summary>
        /// Saves the <see cref="Certificate"/> as an encoded buffer.
        /// </summary>
        /// <returns>An array of bytes that represents the encoded certificate.</returns>
        public byte[] ToCerBuffer()
        {
            return UnderlyingCert.Export(X509ContentType.Cert);
            //return GetCertificateBuffer();
        }
        /// <summary>
        /// Gets the handle of the associated <see cref="CertificateStore"/>, if any.
        /// </summary>
        /// <value>A CertificateStore instance -or- a null reference (<b>Nothing</b> in Visual Basic) is no store is associated with this certificate.</value>
        internal CertificateStore Store
        {
            get
            {
                return m_Store;
            }
            set
            {
                m_Store = value;
            }
        }
        /// <summary>
        /// Gets the private key for the certificate.
        /// </summary>
        /// <value>A System.Security.Cryptography.RSA containing the private key for the certificate.</value>
        /// <exception cref="CertificateException">An error occurs while retrieving the RSA instance associated with the certificate.</exception>
        // Thanks go out to Hernan de Lahitte for fixing a bug in this method
        public RSA PrivateKey
        {
            get
            {
                var underlyingRsa = UnderlyingCert.GetRSAPrivateKey();

                try
                {
                    RSACryptoServiceProvider provider = new RSACryptoServiceProvider();
                    provider.ImportParameters(underlyingRsa.ExportParameters(true));
                    return provider;
                }
                catch (CryptographicException)
                {
                    
                }

                // Fallback: return the original RSA object directly if not exportable
                return underlyingRsa;
            }
        }

        /// <summary>
        /// Gets the public key derived from the certificate's data. This key cannot be used to sign or decrypt data.
        /// </summary>
        /// <value>A System.Security.Cryptography.RSA that contains the public key derived from the certificate's data.</value>
        /// <exception cref="CertificateException">An error occurs while retrieving the RSA instance associated with the certificate.</exception>
        public RSA PublicKey
        {
            get
            {
                RSACryptoServiceProvider provider = new RSACryptoServiceProvider();
#if NET6_0_OR_GREATER
                provider.ImportParameters(UnderlyingCert.PublicKey.GetRSAPublicKey()!.ExportParameters(false));
#else
                provider.ImportParameters(GetRSAPublicKeyLegacyNet(UnderlyingCert).ExportParameters(false));
#endif
                return provider;
            }
        }

        public X509Certificate2 UnderlyingCert { get; set; }
        /// <summary>
        /// The handle of the <see cref="CertificateStore"/> object.
        /// </summary>
        private CertificateStore m_Store;

        public static RSA GetRSAPublicKeyLegacyNet(X509Certificate2 certificate)
        {
            if (!(certificate.PublicKey.Key is RSA rsaPublicKey))
            {
                throw new InvalidOperationException("Certificate does not contain an RSA public key.");
            }

            return rsaPublicKey;
        }
    }
}