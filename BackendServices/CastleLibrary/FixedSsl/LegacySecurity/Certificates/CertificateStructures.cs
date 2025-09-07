/*
 *   Mentalis.org Security Library
 *     [BETA SOFTWARE]
 * 
 *     Copyright � 2002-2003, The KPD-Team
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
 *     - Neither the name of the KPD-Team, nor the names of its contributors
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
using System.Runtime.InteropServices;

namespace Org.Mentalis.LegacySecurity.Certificates {
	/// <summary>
	/// Defines the different hash type values.
	/// </summary>
	[CLSCompliant(true)]
	public enum HashType : int {
		/// <summary>The certificate will be hashed using the SHA1 algorithm.</summary>
		SHA1 = SecurityConstants.CERT_SHA1_HASH_PROP_ID,
		/// <summary>The certificate will be hashed using the MD5 algorithm.</summary>
		MD5 = SecurityConstants.CERT_MD5_HASH_PROP_ID,
		/// <summary>The certificate will be hashed using the default hashing algorithm.</summary>
		Default = SecurityConstants.CERT_HASH_PROP_ID
	}
	/// <summary>
	/// Defines the different key usage values.
	/// </summary>
	[CLSCompliant(true)]
	public enum KeyUsage : int {
		/// <summary>The key can be used for data encipherment.</summary>
		DataEncipherment = SecurityConstants.CERT_DATA_ENCIPHERMENT_KEY_USAGE,
		/// <summary>The key can be used to sign data.</summary>
		DigitalSignature = SecurityConstants.CERT_DIGITAL_SIGNATURE_KEY_USAGE,
		/// <summary>The key can be used in key agreement algorithms.</summary>
		KeyAgreement = SecurityConstants.CERT_KEY_AGREEMENT_KEY_USAGE,
		/// <summary>The key can be used to sign certificates.</summary>
		KeyCertSign = SecurityConstants.CERT_KEY_CERT_SIGN_KEY_USAGE,
		/// <summary>The key can be used for key encipherment.</summary>
		KeyEncipherment = SecurityConstants.CERT_KEY_ENCIPHERMENT_KEY_USAGE,
		/// <summary>The key can be used for electronic non-repudiation.</summary>
		NonRepudiation = SecurityConstants.CERT_NON_REPUDIATION_KEY_USAGE,
		/// <summary>The key can be used to sign certificate revocation lists.</summary>
		CrlSign = SecurityConstants.CERT_OFFLINE_CRL_SIGN_KEY_USAGE
	}
	/// <summary>
	/// Defines the different authentication type values.
	/// </summary>
	[CLSCompliant(true)]
	public enum AuthType : int {
		/// <summary>The certificate is a client certificate.</summary>
		Client = SecurityConstants.AUTHTYPE_CLIENT, // used to validate a certificate that comes from a client
		/// <summary>The certificate is a server certificate.</summary>
		Server = SecurityConstants.AUTHTYPE_SERVER // used to validate a certificate that comes from a server
	}
	/// <summary>
	/// Defines the different certificate status values.
	/// </summary>
	[CLSCompliant(true)]
	public enum CertificateStatus : int {
		/// <summary>The certificate is valid.</summary>
		ValidCertificate = 0,
		/// <summary>A required certificate is not within its validity period.</summary>
		Expired = SecurityConstants.CERT_E_EXPIRED,
		/// <summary>The certificate's basic constraints are invalid or missing.</summary>
		InvalidBasicConstraints = SecurityConstants.TRUST_E_BASIC_CONSTRAINTS,
		/// <summary>A chain of certificates was not correctly created.</summary>
		InvalidChain = SecurityConstants.CERT_E_CHAINING,
		/// <summary>The validity periods of the certification chain do not nest correctly.</summary>
		InvalidNesting = SecurityConstants.CERT_E_VALIDITYPERIODNESTING,
		/// <summary>A certificate is being used for a non permitted purpose.</summary>
		InvalidPurpose = SecurityConstants.CERT_E_PURPOSE,
		/// <summary>A certificate that can only be used as an end-entity is being used as a CA or visa versa.</summary>
		InvalidRole = SecurityConstants.CERT_E_ROLE,
		/// <summary>The signature of the certificate cannot be verified.</summary>
		InvalidSignature = SecurityConstants.TRUST_E_CERT_SIGNATURE,
		/// <summary>The certificate's CN name does not match the passed value.</summary>
		NoCNMatch = SecurityConstants.CERT_E_CN_NO_MATCH,
		/// <summary>A certificate in the chain has been explicitly revoked by its issuer.</summary>
		ParentRevoked = SecurityConstants.CERT_E_REVOKED,
		/// <summary>The revocation process could not continue. The certificates could not be checked.</summary>
		RevocationFailure = SecurityConstants.CERT_E_REVOCATION_FAILURE,
		/// <summary>Since the revocation server was offline, the called function was not able to complete the revocation check.</summary>
		RevocationServerOffline = SecurityConstants.CRYPT_E_REVOCATION_OFFLINE,
		/// <summary>The certificate or signature has been revoked.</summary>
		Revoked = SecurityConstants.CRYPT_E_REVOKED,
		/// <summary>A certification chain processed correctly but terminated in a root certificate not trusted by the trust provider.</summary>
		UntrustedRoot = SecurityConstants.CERT_E_UNTRUSTEDROOT,
		/// <summary>The root certificate is a testing certificate and policy settings disallow test certificates.</summary>
		UntrustedTestRoot = SecurityConstants.CERT_E_UNTRUSTEDTESTROOT,
		/// <summary>The certificate is not valid for the requested usage.</summary>
		WrongUsage = SecurityConstants.CERT_E_WRONG_USAGE,
		/// <summary>The certificate is invalid.</summary>
		OtherError = -1,
	}
	/// <summary>
	/// Defines the different certificate store values.
	/// </summary>
	[CLSCompliant(true)]
	public enum CertificateStoreType : int {
		/// <summary>The certificate store should be saved as a serializes store.</summary>
		SerializedStore = SecurityConstants.CERT_STORE_SAVE_AS_STORE,
		/// <summary>The certificate store should be saved as a signed PKCS7 message.</summary>
		Pkcs7Message = SecurityConstants.CERT_STORE_SAVE_AS_PKCS7
	}
	/// <summary>
	/// Defines the different verificateion flags values.
	/// </summary>
	/// <remarks>
	/// You can specify more VerificationFlags at once by combining them with the OR operator.
	/// </remarks>
	[CLSCompliant(true)]
	public enum VerificationFlags : int {
		/// <summary>No flags.</summary>
		None = 0,
		/// <summary>Ignore an invalid time.</summary>
		IgnoreTimeNotValid = SecurityConstants.CERT_CHAIN_POLICY_IGNORE_NOT_TIME_VALID_FLAG,
		/// <summary>Ignore an invalid time of the certificate trust list.</summary>
		IgnoreCtlTimeNotValid = SecurityConstants.CERT_CHAIN_POLICY_IGNORE_CTL_NOT_TIME_VALID_FLAG,
		/// <summary>Ignore an invalid time nesting.</summary>
		IgnoreTimeNotNested = SecurityConstants.CERT_CHAIN_POLICY_IGNORE_NOT_TIME_NESTED_FLAG,
		/// <summary>Ignore invalid basic contraints.</summary>
		IgnoreInvalidBasicContraints = SecurityConstants.CERT_CHAIN_POLICY_IGNORE_INVALID_BASIC_CONSTRAINTS_FLAG,
		/// <summary>Ignore all time checks.</summary>
		IgnoreAllTimeChecks = SecurityConstants.CERT_CHAIN_POLICY_IGNORE_ALL_NOT_TIME_VALID_FLAGS,
		/// <summary>Allow an unknown certificate authority.</summary>
		AllowUnknownCA = SecurityConstants.CERT_CHAIN_POLICY_ALLOW_UNKNOWN_CA_FLAG,
		/// <summary>Ignore the wrong usage of a certificate.</summary>
		IgnoreWrongUsage = SecurityConstants.CERT_CHAIN_POLICY_IGNORE_WRONG_USAGE_FLAG,
		/// <summary>Ignore an invalid name.</summary>
		IgnoreInvalidName = SecurityConstants.CERT_CHAIN_POLICY_IGNORE_INVALID_NAME_FLAG,
		/// <summary>Ignore an invalid policy.</summary>
		IgnoreInvalidPolicy = SecurityConstants.CERT_CHAIN_POLICY_IGNORE_INVALID_POLICY_FLAG,
		/// <summary>Ignore an unknown revocation status of the end certificate.</summary>
		IgnoreEndRevUnknown = SecurityConstants.CERT_CHAIN_POLICY_IGNORE_END_REV_UNKNOWN_FLAG,
		/// <summary>Ignore an unknown revocation status of the signer certificate.</summary>
		IgnoreSignerRevUnknown = SecurityConstants.CERT_CHAIN_POLICY_IGNORE_CTL_SIGNER_REV_UNKNOWN_FLAG,
		/// <summary>Ignore an unknown revocation status of the certificate authority.</summary>
		IgnoreCARevUnknown = SecurityConstants.CERT_CHAIN_POLICY_IGNORE_CA_REV_UNKNOWN_FLAG,
		/// <summary>Ignore an unknown revocation status of the root certificate.</summary>
		IgnoreRootRevUnknown = SecurityConstants.CERT_CHAIN_POLICY_IGNORE_ROOT_REV_UNKNOWN_FLAG,
		/// <summary>Ignore an unknown revocation status of any of the certificates.</summary>
		IgnoreAllRevUnknown = SecurityConstants.CERT_CHAIN_POLICY_IGNORE_ALL_REV_UNKNOWN_FLAGS,
		/// <summary>Allow a test root.</summary>
		AllowTestroot = SecurityConstants.CERT_CHAIN_POLICY_ALLOW_TESTROOT_FLAG,
		/// <summary>Trust a test root.</summary>
		TrustTestroot = SecurityConstants.CERT_CHAIN_POLICY_TRUST_TESTROOT_FLAG
	}
}