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

using System.Globalization;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace CastleLibrary.FixedSsl.Security.Certificates
{
    /// <summary>
    /// Defines a chain of certificates.
    /// </summary>
    public class CertificateChain
    {
        /// <summary>
        /// Initializes a new <see cref="CertificateChain"/> instance from a <see cref="Certificate"/>.
        /// </summary>
        /// <param name="cert">The certificate for which a chain is being built.</param>
        /// <remarks><paramref name="cert"/> will always be the end certificate.</remarks>
        /// <exception cref="ArgumentNullException"><paramref name="cert"/> is a null reference (<b>Nothing</b> in Visual Basic).</exception>
        /// <exception cref="CertificateException">An error occurs while building the certificate chain.</exception>
        public CertificateChain(Certificate cert)
            : this(cert, CertificateChainOptions.Default) { }

        /// <summary>
        /// Initializes a new <see cref="CertificateChain"/> instance from a <see cref="Certificate"/>.
        /// </summary>
        /// <param name="cert">The certificate for which a chain is being built.</param>
        /// <param name="additional">Any additional store to be searched for supporting certificates and CTLs.</param>
        /// <param name="options">Additional certificate chain options.</param>
        /// <remarks><paramref name="cert"/> will always be the end certificate.</remarks>
        /// <exception cref="ArgumentNullException"><paramref name="cert"/> is a null reference (<b>Nothing</b> in Visual Basic).</exception>
        /// <exception cref="CertificateException">An error occurs while building the certificate chain.</exception>
        public CertificateChain(Certificate cert, CertificateChainOptions options)
        {
            ArgumentNullException.ThrowIfNull(cert);
            m_Certificate = cert;
        }

        /// <summary>
        /// Returns the certificate for which this chain was built.
        /// </summary>
        protected Certificate Certificate
        {
            get { return m_Certificate; }
        }

        /// <summary>
        /// Verifies the end <see cref="Certificate"/> according to the SSL policy rules.
        /// </summary>
        /// <param name="server">The server that returned the certificate -or- a null reference if the certificate is a client certificate.</param>
        /// <param name="type">One of the <see cref="AuthType"/> values.</param>
        /// <returns>One of the <see cref="CertificateStatus"/> values.</returns>
        /// <exception cref="CertificateException">An error occurs while verifying the certificate.</exception>
        public virtual CertificateStatus VerifyChain(string server, AuthType type)
        {
            return VerifyChain(server, type, VerificationFlags.None);
        }

        /// <summary>
        /// Verifies the end <see cref="Certificate"/> according to the SSL policy rules.
        /// </summary>
        /// <param name="server">The server that returned the certificate -or- a null reference if the certificate is a client certificate.</param>
        /// <param name="type">One of the <see cref="AuthType"/> values.</param>
        /// <param name="flags">One or more of the <see cref="VerificationFlags"/> values. VerificationFlags values can be combined with the OR operator.</param>
        /// <returns>One of the <see cref="CertificateStatus"/> values.</returns>
        /// <exception cref="CertificateException">An error occurs while verifying the certificate.</exception>
        public virtual CertificateStatus VerifyChain(
            string server,
            AuthType type,
            VerificationFlags flags
        )
        {
            try
            {
                if (m_Certificate.UnderlyingCert is not X509Certificate2 cert)
                    throw new CertificateException("Certificate must be an X509Certificate2.");

                using (var chain = new X509Chain())
                {
                    // =========================
                    // Verification flags
                    // =========================
                    if (
                        flags.HasFlag(VerificationFlags.IgnoreAllTimeChecks)
                        || flags.HasFlag(VerificationFlags.IgnoreTimeNotValid)
                    )
                    {
                        chain.ChainPolicy.VerificationFlags |=
                            X509VerificationFlags.IgnoreNotTimeValid;
                    }

                    if (flags.HasFlag(VerificationFlags.AllowUnknownCA))
                    {
                        chain.ChainPolicy.VerificationFlags |=
                            X509VerificationFlags.AllowUnknownCertificateAuthority;
                    }

                    if (flags.HasFlag(VerificationFlags.IgnoreWrongUsage))
                    {
                        chain.ChainPolicy.VerificationFlags |=
                            X509VerificationFlags.IgnoreWrongUsage;
                    }

                    if (flags.HasFlag(VerificationFlags.IgnoreInvalidBasicContraints))
                    {
                        chain.ChainPolicy.VerificationFlags |=
                            X509VerificationFlags.IgnoreInvalidBasicConstraints;
                    }

                    if (flags.HasFlag(VerificationFlags.IgnoreInvalidPolicy))
                    {
                        chain.ChainPolicy.VerificationFlags |=
                            X509VerificationFlags.IgnoreInvalidPolicy;
                    }

                    // =========================
                    // Revocation handling
                    // =========================
                    if (flags.HasFlag(VerificationFlags.IgnoreAllRevUnknown))
                        chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;
                    else
                    {
                        chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
                        chain.ChainPolicy.RevocationFlag = X509RevocationFlag.ExcludeRoot;
                    }

                    chain.ChainPolicy.VerificationTime = DateTime.UtcNow;

                    // =========================
                    // EKU (SSL semantics)
                    // =========================
                    chain.ChainPolicy.ApplicationPolicy.Add(
                        new Oid(
                            type == AuthType.Server
                                ? "1.3.6.1.5.5.7.3.1" // ServerAuth
                                : "1.3.6.1.5.5.7.3.2" // ClientAuth
                        )
                    );

                    // =========================
                    // Hostname check (server only)
                    // =========================
                    if (
                        type == AuthType.Server
                        && !flags.HasFlag(VerificationFlags.IgnoreInvalidName)
                    )
                    {
                        if (string.IsNullOrEmpty(server) || !MatchHostname(cert, server))
                            return CertificateStatus.NoCNMatch;
                    }

                    if (chain.Build(cert))
                        return CertificateStatus.ValidCertificate;

                    // =========================
                    // Status translation
                    // =========================
                    foreach (var status in chain.ChainStatus)
                    {
                        var mapped = MapChainStatus(status.Status);
                        if (mapped != CertificateStatus.ValidCertificate)
                            return mapped;
                    }

                    return CertificateStatus.OtherError;
                }
            }
            catch (Exception ex)
            {
                throw new CertificateException(
                    "An error occurred while verifying the certificate.",
                    ex
                );
            }
        }

        private static CertificateStatus MapChainStatus(X509ChainStatusFlags flags)
        {
            if (flags == X509ChainStatusFlags.NoError)
                return CertificateStatus.ValidCertificate;
            else if (flags.HasFlag(X509ChainStatusFlags.NotTimeValid))
                return CertificateStatus.Expired;
            else if (flags.HasFlag(X509ChainStatusFlags.NotTimeNested))
                return CertificateStatus.InvalidNesting;
            else if (flags.HasFlag(X509ChainStatusFlags.InvalidBasicConstraints))
                return CertificateStatus.InvalidBasicConstraints;
            else if (flags.HasFlag(X509ChainStatusFlags.NotValidForUsage))
                return CertificateStatus.WrongUsage;
            else if (
                flags.HasFlag(X509ChainStatusFlags.InvalidPolicyConstraints)
                || flags.HasFlag(X509ChainStatusFlags.NoIssuanceChainPolicy)
            )
                return CertificateStatus.InvalidPurpose;
            else if (flags.HasFlag(X509ChainStatusFlags.Revoked))
                return CertificateStatus.Revoked;
            else if (flags.HasFlag(X509ChainStatusFlags.RevocationStatusUnknown))
                return CertificateStatus.RevocationFailure;
            else if (flags.HasFlag(X509ChainStatusFlags.OfflineRevocation))
                return CertificateStatus.RevocationServerOffline;
            else if (
                flags.HasFlag(X509ChainStatusFlags.UntrustedRoot)
                || flags.HasFlag(X509ChainStatusFlags.ExplicitDistrust)
            )
                return CertificateStatus.UntrustedRoot;
            else if (
                flags.HasFlag(X509ChainStatusFlags.PartialChain)
                || flags.HasFlag(X509ChainStatusFlags.Cyclic)
            )
                return CertificateStatus.InvalidChain;
            else if (
                flags.HasFlag(X509ChainStatusFlags.NotSignatureValid)
                || flags.HasFlag(X509ChainStatusFlags.HasWeakSignature)
            )
                return CertificateStatus.InvalidSignature;
            else if (
                flags.HasFlag(X509ChainStatusFlags.InvalidNameConstraints)
                || flags.HasFlag(X509ChainStatusFlags.HasExcludedNameConstraint)
            )
                return CertificateStatus.NoCNMatch;

            return CertificateStatus.OtherError;
        }

        private static bool MatchHostname(X509Certificate2 cert, string hostname)
        {
            if (string.IsNullOrEmpty(hostname))
                return false;

            hostname = NormalizeHostname(hostname);

            var hasSan = false;

            foreach (var dnsName in GetSubjectAltDnsNames(cert))
            {
                hasSan = true;
                if (MatchDnsName(dnsName, hostname))
                    return true;
            }

            // RFC 6125 §6.4.4:
            // Only fall back to CN if *no* SANs are present
            if (!hasSan)
            {
                var cn = cert.GetNameInfo(X509NameType.DnsName, false);
                if (!string.IsNullOrEmpty(cn))
                    return MatchDnsName(cn, hostname);
            }

            return false;
        }

        private static IEnumerable<string> GetSubjectAltDnsNames(X509Certificate2 cert)
        {
            foreach (var ext in cert.Extensions)
            {
                if (ext.Oid?.Value != "2.5.29.17")
                    continue;

                // Windows formats SANs like:
                // "DNS Name=example.com"
                foreach (
                    var line in new AsnEncodedData(ext.Oid, ext.RawData)
                        .Format(true)
                        .Split(separator, StringSplitOptions.RemoveEmptyEntries)
                )
                {
                    const string prefix = "DNS Name=";
                    if (line.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                        yield return NormalizeHostname(line.Substring(prefix.Length));
                }
            }
        }

        private static bool MatchDnsName(string pattern, string hostname)
        {
            pattern = NormalizeHostname(pattern);
            hostname = NormalizeHostname(hostname);

            // Exact match
            if (string.Equals(pattern, hostname, StringComparison.OrdinalIgnoreCase))
                return true;

            // Wildcards
            if (!pattern.Contains('*'))
                return false;

            // RFC 6125 §6.4.3
            // - Wildcard must be the entire left-most label
            // - Only one wildcard
            // - Must match at least one dot
            if (!pattern.StartsWith("*."))
                return false;

            if (pattern.IndexOf('*', 1) != -1)
                return false;

            var suffix = pattern.Substring(2); // remove "*."
            if (suffix.Length == 0)
                return false;

            // Host must be longer and have exactly one additional label
            if (!hostname.EndsWith("." + suffix, StringComparison.OrdinalIgnoreCase))
                return false;

            // Prevent "*.com" matching "example.com"
            return !hostname.Substring(0, hostname.Length - suffix.Length - 1).Contains('.');
        }

        private static string NormalizeHostname(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return string.Empty;

            name = name.Trim().TrimEnd('.');

            // IDN (RFC 5890)
            try
            {
                return new IdnMapping().GetAscii(name);
            }
            catch
            {
                // Not Important.
            }

            return name;
        }

        /// <summary>
        /// Begins verification of the end <see cref="Certificate"/> according to the SSL policy rules.
        /// </summary>
        /// <param name="server">The server that returned the certificate -or- a null reference if the certificate is a client certificate.</param>
        /// <param name="type">One of the <see cref="AuthType"/> values.</param>
        /// <param name="flags">One or more of the <see cref="VerificationFlags"/> values. VerificationFlags values can be combined with the OR operator.</param>
        /// <param name="callback">The <see cref="AsyncCallback"/> delegate.</param>
        /// <param name="asyncState">An object that contains state information for this request.</param>
        /// <returns>An <see cref="IAsyncResult"/> that references the asynchronous connection.</returns>
        /// <exception cref="CertificateException">An error occurs while queuing the verification request.</exception>
        public virtual IAsyncResult BeginVerifyChain(
            string server,
            AuthType type,
            VerificationFlags flags,
            AsyncCallback callback,
            object asyncState
        )
        {
            var ret = new CertificateVerificationResult(
                this,
                server,
                type,
                flags,
                callback,
                asyncState
            );
            return !ThreadPool.QueueUserWorkItem(new WaitCallback(this.StartVerification), ret)
                ? throw new CertificateException(
                    "Could not schedule the certificate chain for verification."
                )
                : (IAsyncResult)ret;
        }

        /// <summary>
        /// Ends a pending asynchronous certificate verification request.
        /// </summary>
        /// <param name="ar">Stores state information for this asynchronous operation as well as any user-defined data.</param>
        /// <returns>One of the <see cref="CertificateStatus"/> values.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="ar"/> is a null reference (<b>Nothing</b> in Visual Basic).</exception>
        /// <exception cref="ArgumentException">The <paramref name="ar"/> parameter was not returned by a call to the <see cref="BeginVerifyChain"/> method.</exception>
        /// <exception cref="InvalidOperationException"><b>EndVerifyChain</b> was previously called for the asynchronous chain verification.</exception>
        /// <exception cref="CertificateException">An error occurs while verifying the certificate chain.</exception>
        public virtual CertificateStatus EndVerifyChain(IAsyncResult ar)
        {
            ArgumentNullException.ThrowIfNull(ar);
            CertificateVerificationResult result;
            try
            {
                result = (CertificateVerificationResult)ar;
            }
            catch
            {
                throw new ArgumentException();
            }
            if (result.Chain != this)
                throw new ArgumentException();
            if (result.HasEnded)
                throw new InvalidOperationException();
            if (result.ThrowException != null)
                throw result.ThrowException;
            result.HasEnded = true;
            return result.Status;
        }

        /// <summary>
        /// Verifies a certificate chain and calls a delegate when finished.
        /// </summary>
        /// <param name="state">Stores state information for this asynchronous operation as well as any user-defined data.</param>
        protected void StartVerification(object state)
        {
            if (state == null)
                return;
            CertificateVerificationResult result;
            try
            {
                result = (CertificateVerificationResult)state;
            }
            catch
            {
                return;
            }
            CertificateStatus ret;
            try
            {
                ret = VerifyChain(result.Server, result.Type, result.Flags);
            }
            catch (CertificateException ce)
            {
                result.VerificationCompleted(ce, CertificateStatus.OtherError);
                return;
            }
            catch (Exception e)
            {
                result.VerificationCompleted(
                    new CertificateException("Could not verify the certificate chain.", e),
                    CertificateStatus.OtherError
                );
                return;
            }
            result.VerificationCompleted(null, ret);
        }

        /// <summary>
        /// The end certificate that was used to build the chain.
        /// </summary>
        private readonly Certificate m_Certificate;
        private static readonly char[] separator = new[] { '\r', '\n' };
    }
}
