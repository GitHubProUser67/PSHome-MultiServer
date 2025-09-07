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
using System.IO;
using System.Security;
using System.Security.Cryptography.X509Certificates;
using System.Runtime.InteropServices;
using System.Collections.Specialized;

namespace Org.Mentalis.LegacySecurity.Certificates {
	/// <summary>
	/// Defines a X509 v3 encoded certificate.
	/// </summary>
	[CLSCompliant(true)]
	public class Certificate : ICloneable {
		/// <summary>
		/// Creates a new instance of the <see cref="Certificate"/> class by opening a PFX file and retrieving the first certificate from it.
		/// </summary>
		/// <param name="file">The full path to the PFX file.</param>
		/// <param name="password">The password used to encrypt the private key.</param>
		/// <returns>One of the certificates in the PFX file.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="file"/> or <paramref name="password"/> is a null reference (<b>Nothing</b> in Visual Basic).</exception>
		/// <exception cref="CertificateException">An error occurs while loading certificates from the specified file.</exception>
		public static Certificate CreateFromPfxFile(string file, string password) {
			return CertificateStore.CreateFromPfxFile(file, password).FindCertificate();
		}
		/// <summary>
		/// Creates a new instance of the <see cref="Certificate"/> class by opening a PFX file and retrieving the first certificate from it.
		/// </summary>
		/// <param name="file">The contents of a PFX file.</param>
		/// <param name="password">The password used to encrypt the private key.</param>
		/// <returns>One of the certificates in the PFX file.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="file"/> or <paramref name="password"/> is a null reference (<b>Nothing</b> in Visual Basic).</exception>
		/// <exception cref="CertificateException">An error occurs while loading certificates from the specified bytes.</exception>
		public static Certificate CreateFromPfxFile(byte[] file, string password) {
			return CertificateStore.CreateFromPfxFile(file, password).FindCertificate();
		}
		/// <summary>
		/// Creates a new instance of the <see cref="Certificate"/> class by opening a certificate file and retrieving the first certificate from it.
		/// </summary>
		/// <param name="file">The full path to the certificate file to open.</param>
		/// <returns>One of the certificates in the certificate file.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="file"/> is a null reference (<b>Nothing</b> in Visual Basic).</exception>
		/// <exception cref="CertificateException">An error occurs while loading certificates from the specified file.</exception>
		public static Certificate CreateFromCerFile(string file) {
			return CertificateStore.CreateFromCerFile(file).FindCertificate();
		}
		/// <summary>
		/// Creates a new instance of the <see cref="Certificate"/> class by reading a certificate from a certificate blob.
		/// </summary>
		/// <param name="file">The contents of the certificate file.</param>
		/// <returns>A Certificate instance.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="file"/> if a null reference (<b>Nothing</b> in Visual Basic).</exception>
		/// <exception cref="CertificateException">An error occurs while loading the specified certificate.</exception>
		public static Certificate CreateFromCerFile(byte[] file) {
			if (file == null)
				throw new ArgumentNullException();
			IntPtr data = Marshal.AllocHGlobal(file.Length);
			Marshal.Copy(file, 0, data, file.Length);
			IntPtr handle = SspiProvider.CertCreateCertificateContext(SecurityConstants.X509_ASN_ENCODING | SecurityConstants.PKCS_7_ASN_ENCODING, data, file.Length);
			Marshal.FreeHGlobal(data);
			if (handle == IntPtr.Zero)
				throw new CertificateException("Unable to load the specified certificate.");
			else
				return new Certificate(handle);
		}
		/// <summary>
		/// Duplicates a given certificate.
		/// </summary>
		/// <param name="certificate">The certificate to duplicate.</param>
		/// <exception cref="ArgumentNullException"><paramref name="certificate"/> is a null reference (<b>Nothing</b> in Visual Basic).</exception>
		public Certificate(Certificate certificate) {
			if (certificate == null)
				throw new ArgumentNullException();
			InitCertificate(certificate.Handle, true, null);
		}
		/// <summary>
		/// Initializes a new <see cref="Certificate"/> instance from a handle.
		/// </summary>
		/// <param name="handle">The handle from which to initialize the state of the new instance.</param>
		/// <exception cref="ArgumentException"><paramref name="handle"/> is invalid.</exception>
		public Certificate(IntPtr handle) : this(handle, false) {}
		/// <summary>
		/// Initializes a new <see cref="Certificate"/> instance from a handle.
		/// </summary>
		/// <param name="handle">The handle from which to initialize the state of the new instance.</param>
		/// <param name="duplicate"><b>true</b> if the handle should be duplicated, <b>false</b> otherwise.</param>
		/// <exception cref="ArgumentException"><paramref name="handle"/> is invalid.</exception>
		public Certificate(IntPtr handle, bool duplicate) {
			InitCertificate(handle, duplicate, null);
		}
		/// <summary>
		/// Initializes this <see cref="Certificate"/> instance from a handle.
		/// </summary>
		/// <param name="handle">The handle from which to initialize the state of the new instance.</param>
		/// <param name="duplicate"><b>true</b> if the handle should be duplicated, <b>false</b> otherwise.</param>
		/// <param name="store">The store that owns the certificate.</param>
		/// <exception cref="ArgumentException"><paramref name="handle"/> is invalid.</exception>
		private void InitCertificate(IntPtr handle, bool duplicate, CertificateStore store) {
			if (handle == IntPtr.Zero)
				throw new ArgumentException("Invalid certificate handle!");
			if (duplicate)
				m_Handle = SspiProvider.CertDuplicateCertificateContext(handle);
			else
				m_Handle = handle;
			CertificateContext cc = (CertificateContext)Marshal.PtrToStructure(handle, typeof(CertificateContext));
			m_CertInfo = (CertificateInfo)Marshal.PtrToStructure(cc.pCertInfo, typeof(CertificateInfo));
			if (store == null) {
				m_Store = null;
			} else {
				m_Store = store;
			}
		}
		/// <summary>
		/// Initializes this <see cref="Certificate"/> instance from a handle.
		/// </summary>
		/// <param name="handle">The handle from which to initialize the state of the new instance.</param>
		/// <param name="store">The <see cref="CertificateStore"/> that contains the certificate.</param>
		internal Certificate(IntPtr handle, CertificateStore store) {
			InitCertificate(handle, false, store);
		}
		/// <summary>
		/// Creates a copy of this <see cref="Certificate"/>.
		/// </summary>
		/// <returns>The Certificate this method creates, cast as an object.</returns>
		public object Clone() {
			return new Certificate(SspiProvider.CertDuplicateCertificateContext(Handle));
		}
		/// <summary>
		/// Disposes of the certificate and frees unmanaged resources.
		/// </summary>
		~Certificate() {
			if (Handle != IntPtr.Zero)
				SspiProvider.CertFreeCertificateContext(Handle);
		}
		/// <summary>
		/// Returns a string representation of the current <see cref="Certificate"/> object.
		/// </summary>
		/// <returns>A string representation of the current Certificate object.</returns>
		public override string ToString() {
			return this.GetType().FullName;
		}
		/// <summary>
		/// Returns a string representation of the current X509Certificate object, with extra information, if specified.
		/// </summary>
		/// <param name="verbose"><b>true</b> to produce the verbose form of the string representation; otherwise, <b>false</b>.</param>
		/// <returns>A string representation of the current X509Certificate object.</returns>
		public string ToString(bool verbose) {
			if (verbose) {
				return "CERTIFICATE:\r\n" +
					"        Format:  X509\r\n" +
					"        Name:  " + GetName() + "\r\n" + 
					"        Issuing CA:  " + GetIssuerName() + "\r\n" +
					"        Key Algorithm:  " + GetKeyAlgorithm() + "\r\n" + 
					"        Serial Number:  " + GetSerialNumberString() + "\r\n" +
					"        Key Alogrithm Parameters:  " + GetKeyAlgorithmParametersString() + "\r\n" + 
					"        Public Key:  " + GetPublicKeyString();
			} else {
				return ToString();
			}
		}
		/// <summary>
		/// Returns the hash value for the X.509v3 certificate as an array of bytes.
		/// </summary>
		/// <returns>The hash value for the X.509 certificate.</returns>
		/// <exception cref="CertificateException">An error occurs while retrieving the hash of the certificate.</exception>
		public byte[] GetCertHash() {
			return GetCertHash(HashType.Default);
		}
		/// <summary>
		/// Returns the hash value for the X.509v3 certificate as an array of bytes.
		/// </summary>
		/// <param name="type">One of the <see cref="HashType"/> values.</param>
		/// <returns>The hash value for the X.509 certificate.</returns>
		/// <exception cref="CertificateException">An error occurs while retrieving the hash of the certificate.</exception>
		public byte[] GetCertHash(HashType type) {
			byte[] ret;
			IntPtr hash = Marshal.AllocHGlobal(256);
			try {
				int size = 256;
				if (SspiProvider.CertGetCertificateContextProperty(Handle, (int)type, hash, ref size) == 0 || size <= 0 || size > 256)
					throw new CertificateException("An error occurs while retrieving the hash of the certificate.");
				ret = new byte[size];
				Marshal.Copy(hash, ret, 0, size);
			} catch (Exception e) {
				throw e;
			} finally {
				Marshal.FreeHGlobal(hash);
			}
			return ret;
		}
		/// <summary>
		/// Returns the hash value for the X.509v3 certificate as a hexadecimal string.
		/// </summary>
		/// <returns>The hexadecimal string representation of the X.509 certificate hash value.</returns>
		/// <exception cref="CertificateException">An error occurs while retrieving the hash of the certificate.</exception>
		public string GetCertHashString() {
			return GetCertHashString(HashType.Default);
		}
		/// <summary>
		/// Returns the hash value for the X.509v3 certificate as a hexadecimal string.
		/// </summary>
		/// <param name="type">One of the <see cref="HashType"/> values.</param>
		/// <returns>The hexadecimal string representation of the X.509 certificate hash value.</returns>
		/// <exception cref="CertificateException">An error occurs while retrieving the hash of the certificate.</exception>
		public string GetCertHashString(HashType type) {
			return BytesToString(GetCertHash(type));
		}
		/// <summary>
		/// Converts an array of bytes to its hexadecimal string representation.
		/// </summary>
		/// <param name="buffer">The bytes to convert.</param>
		/// <returns>The hexadecimal representation of the byte array.</returns>
		private string BytesToString(byte[] buffer) {
			string ret = "";
			for(int i = 0; i < buffer.Length; i++) {
				ret += buffer[i].ToString("X2");
			}
			return ret;
		}
		/// <summary>
		/// Returns the effective date of this X.509v3 certificate.
		/// </summary>
		/// <returns>The effective date for this X.509 certificate.</returns>
		/// <remarks>The effective date is the date after which the X.509 certificate is considered valid.</remarks>
		public DateTime GetEffectiveDate() {
			return DateTime.FromFileTime(m_CertInfo.NotBefore);
		}
		/// <summary>
		/// Returns the expiration date of this X.509v3 certificate.
		/// </summary>
		/// <returns>The expiration date for this X.509 certificate.</returns>
		/// <remarks>The expiration date is the date after which the X.509 certificate is no longer considered valid.</remarks>
		public DateTime GetExpirationDate() {
			return DateTime.FromFileTime(m_CertInfo.NotAfter);
		}
		/// <summary>
		/// Returns the name of the certification authority that issued the X.509v3 certificate.
		/// </summary>
		/// <returns>The name of the certification authority that issued the X.509 certificate.</returns>
		public string GetIssuerName() {
			int length = SspiProvider.CertGetNameString(Handle, SecurityConstants.CERT_NAME_FRIENDLY_DISPLAY_TYPE, SecurityConstants.CERT_NAME_DISABLE_IE4_UTF8_FLAG | SecurityConstants.CERT_NAME_FRIENDLY_DISPLAY_TYPE, IntPtr.Zero, IntPtr.Zero, 0);
			if (length <= 0)
				throw new CertificateException("An error occurs while requesting the issuer name.");
			IntPtr name = Marshal.AllocHGlobal(length);
			SspiProvider.CertGetNameString(Handle, SecurityConstants.CERT_NAME_FRIENDLY_DISPLAY_TYPE, SecurityConstants.CERT_NAME_DISABLE_IE4_UTF8_FLAG | SecurityConstants.CERT_NAME_FRIENDLY_DISPLAY_TYPE, IntPtr.Zero, name, length);
			string ret = Marshal.PtrToStringAnsi(name);
			Marshal.FreeHGlobal(name);
			return ret;
		}
		/// <summary>
		/// Returns the key algorithm information for this X.509v3 certificate.
		/// </summary>
		/// <returns>The key algorithm information for this X.509 certificate as a string.</returns>
		public string GetKeyAlgorithm() {
			return Marshal.PtrToStringAnsi(m_CertInfo.SignatureAlgorithmpszObjId);
		}
		/// <summary>
		/// Returns the key algorithm parameters for the X.509v3 certificate.
		/// </summary>
		/// <returns>The key algorithm parameters for the X.509 certificate as an array of bytes.</returns>
		public byte[] GetKeyAlgorithmParameters() {
			byte[] ret = new byte[m_CertInfo.SignatureAlgorithmParameterscbData];
			Marshal.Copy(m_CertInfo.SignatureAlgorithmParameterspbData, ret, 0, ret.Length);
			return ret;
		}
		/// <summary>
		/// Returns the key algorithm parameters for the X.509v3 certificate.
		/// </summary>
		/// <returns>The key algorithm parameters for the X.509 certificate as a hexadecimal string.</returns>
		public string GetKeyAlgorithmParametersString() {
			return BytesToString(GetKeyAlgorithmParameters());
		}
		/// <summary>
		/// Returns the public key for the X.509v3 certificate.
		/// </summary>
		/// <returns>The public key for the X.509 certificate as an array of bytes.</returns>
		public byte[] GetPublicKey() {
			byte[] key = new byte[m_CertInfo.SubjectPublicKeyInfoPublicKeycbData];
			Marshal.Copy(m_CertInfo.SubjectPublicKeyInfoPublicKeypbData, key, 0, key.Length);
			return key;
		}
		/// <summary>
		/// Returns the public key for the X.509v3 certificate.
		/// </summary>
		/// <returns>The public key for the X.509 certificate as a hexadecimal string.</returns>
		public string GetPublicKeyString() {
			return BytesToString(GetPublicKey());
		}
		/// <summary>
		/// Returns the raw data for the entire X.509v3 certificate.
		/// </summary>
		/// <returns>A byte array containing the X.509 certificate data.</returns>
		public byte[] GetRawCertData() {
			CertificateContext cc = (CertificateContext)Marshal.PtrToStructure(Handle, typeof(CertificateContext));
			byte[] ret = new byte[cc.cbCertEncoded];
			Marshal.Copy(cc.pbCertEncoded, ret, 0, ret.Length);
			return ret;
		}
		/// <summary>
		/// Returns the raw data for the entire X.509v3 certificate.
		/// </summary>
		/// <returns>The X.509 certificate data as a hexadecimal string.</returns>
		public string GetRawCertDataString() {
			return BytesToString(GetRawCertData());
		}
		/// <summary>
		/// Returns the serial number of the X.509v3 certificate.
		/// </summary>
		/// <returns>The serial number of the X.509 certificate as an array of bytes.</returns>
		public byte[] GetSerialNumber() {
			byte[] ret = new byte[m_CertInfo.SerialNumbercbData];
			Marshal.Copy(m_CertInfo.SerialNumberpbData, ret, 0, ret.Length);
			return ret;
		}
		/// <summary>
		/// Returns the serial number of the X.509v3 certificate.
		/// </summary>
		/// <returns>The serial number of the X.509 certificate as a hexadecimal string.</returns>
		public string GetSerialNumberString() {
			return BytesToString(GetSerialNumber());
		}
		/// <summary>
		/// Returns the length of the public key of the X.509v3 certificate.
		/// </summary>
		/// <returns>Returns the length of the public key in bits. If unable to determine the key's length, returns zero.</returns>
		public int GetPublicKeyLength() {
			CertificateContext cc = (CertificateContext)Marshal.PtrToStructure(Handle, typeof(CertificateContext));
			return SspiProvider.CertGetPublicKeyLength(SecurityConstants.PKCS_7_ASN_ENCODING | SecurityConstants.X509_ASN_ENCODING, new IntPtr(cc.pCertInfo.ToInt64() + 56));
		}
		/// <summary>
		/// Returns a list of attributes of the X.509v3 certificate.
		/// </summary>
		/// <returns>A StringDictionary that contains the attributes.</returns>
		/// <exception cref="CertificateException">An error occurs while retrieving the attributes.</exception>
		public StringDictionary GetAttributes() {
			StringDictionary ret = new StringDictionary();
			int size = 0;
			SspiProvider.CryptDecodeObject(SecurityConstants.PKCS_7_ASN_ENCODING | SecurityConstants.X509_ASN_ENCODING, new IntPtr(SecurityConstants.X509_NAME), m_CertInfo.SubjectpbData, m_CertInfo.SubjectcbData, 0, IntPtr.Zero, ref size);
			if (size <= 0)
				throw new CertificateException("Unable to decode the name of the certificate.");
			IntPtr buffer = IntPtr.Zero;
			try {
				buffer = Marshal.AllocHGlobal(size);
				if (SspiProvider.CryptDecodeObject(SecurityConstants.PKCS_7_ASN_ENCODING | SecurityConstants.X509_ASN_ENCODING, new IntPtr(SecurityConstants.X509_NAME), m_CertInfo.SubjectpbData, m_CertInfo.SubjectcbData, 0, buffer, ref size) == 0)
					throw new CertificateException("Unable to decode the name of the certificate.");
				CertificateNameInfo cni = (CertificateNameInfo)Marshal.PtrToStructure(buffer, typeof(CertificateNameInfo));
				if (cni.cRDN <= 0)
					throw new CertificateException("Certificate does not have a subject relative distinguished name.");
				RelativeDistinguishedName cr;
				RdnAttribute cra;
				for(int i = 0; i < cni.cRDN; i++) {
					cr = (RelativeDistinguishedName)Marshal.PtrToStructure(new IntPtr(cni.rgRDN.ToInt64() + i * Marshal.SizeOf(typeof(RelativeDistinguishedName))), typeof(RelativeDistinguishedName));
					for(int j = 0; j < cr.cRDNAttr; j++) {
						cra = (RdnAttribute)Marshal.PtrToStructure(new IntPtr(cr.rgRDNAttr.ToInt64() + j * Marshal.SizeOf(typeof(RdnAttribute))), typeof(RdnAttribute));
						try {
							ret.Add(Marshal.PtrToStringAnsi(cra.pszObjId), Marshal.PtrToStringAnsi(cra.pbData));
						} catch {}
					}
				}
			} catch (CertificateException ce) {
				throw ce;
			} catch (Exception e) {
				throw new CertificateException("Could not get certificate attributes.", e);
			} finally {
				if (buffer != IntPtr.Zero)
					Marshal.FreeHGlobal(buffer);
			}
			return ret;
		}
		/// <summary>
		/// Returns the name of the current principal.
		/// </summary>
		/// <returns>The name of the current principal.</returns>
		/// <exception cref="CertificateException">The certificate does not have a name attribute.</exception>
		public string GetName() {
			int size = 0;
			SspiProvider.CryptDecodeObject(SecurityConstants.PKCS_7_ASN_ENCODING | SecurityConstants.X509_ASN_ENCODING, new IntPtr(SecurityConstants.X509_NAME), m_CertInfo.SubjectpbData, m_CertInfo.SubjectcbData, 0, IntPtr.Zero, ref size);
			if (size <= 0)
				throw new CertificateException("Unable to decode the name of the certificate.");
			IntPtr buffer = IntPtr.Zero;
			string ret = null;
			try {
				buffer = Marshal.AllocHGlobal(size);
				if (SspiProvider.CryptDecodeObject(SecurityConstants.PKCS_7_ASN_ENCODING | SecurityConstants.X509_ASN_ENCODING, new IntPtr(SecurityConstants.X509_NAME), m_CertInfo.SubjectpbData, m_CertInfo.SubjectcbData, 0, buffer, ref size) == 0)
					throw new CertificateException("Unable to decode the name of the certificate.");
				IntPtr attPointer = SspiProvider.CertFindRDNAttr(SecurityConstants.szOID_COMMON_NAME, buffer);
				if (attPointer == IntPtr.Zero)
					attPointer = SspiProvider.CertFindRDNAttr(SecurityConstants.szOID_RSA_unstructName, buffer);
				if (attPointer != IntPtr.Zero) {
					RdnAttribute att = (RdnAttribute)Marshal.PtrToStructure(attPointer, typeof(RdnAttribute));
					ret = Marshal.PtrToStringAnsi(att.pbData);
				}
			} catch (CertificateException ce) {
				throw ce;
			} catch (Exception e) {
				throw new CertificateException("Could not get certificate attributes.", e);
			} finally {
				if (buffer != IntPtr.Zero)
					Marshal.FreeHGlobal(buffer);
			}
			if (ret == null)
				throw new CertificateException("Certificate does not have a name attribute.");
			else
				return ret;
		}
		/// <summary>
		/// Returns a list of intended key usages of the X.509v3 certificate.
		/// </summary>
		/// <returns>An integer that contains a list of intended key usages.</returns>
		/// <remarks>Use the bitwise And operator to check whether a specific key usage is set.</remarks>
		public int GetIntendedKeyUsage() { // returns one or more of the KeyUsage values
			IntPtr buffer = Marshal.AllocHGlobal(4);
			CertificateContext cc = (CertificateContext)Marshal.PtrToStructure(Handle, typeof(CertificateContext));
			SspiProvider.CertGetIntendedKeyUsage(SecurityConstants.PKCS_7_ASN_ENCODING | SecurityConstants.X509_ASN_ENCODING, cc.pCertInfo, buffer, 4);
			byte[] mb = new byte[4];
			Marshal.Copy(buffer, mb, 0, 4);
			Marshal.FreeHGlobal(buffer);
			return BitConverter.ToInt32(mb, 0);
		}
		/// <summary>
		/// Returns a list of enhanced key usages of the X.509v3 certificate.
		/// </summary>
		/// <returns>A StringCollection that contains a list of the enhanced key usages.</returns>
		/// <exception cref="CertificateException">An error occurs while retrieving the enhanced key usages.</exception>
		public StringCollection GetEnhancedKeyUsage() {
			StringCollection ret = new StringCollection();
			int size = 0;
			SspiProvider.CertGetEnhancedKeyUsage(Handle, 0, IntPtr.Zero, ref size); 
			if (size <= 0)
				return ret;
			IntPtr buffer = Marshal.AllocHGlobal(size);
			try {
				if (SspiProvider.CertGetEnhancedKeyUsage(Handle, 0, buffer, ref size) == 0)
					throw new CertificateException("Could not obtain the enhanced key usage.");
				TrustListUsage cu = (TrustListUsage)Marshal.PtrToStructure(buffer, typeof(TrustListUsage));
				for(int i = 0; i < cu.cUsageIdentifier; i++) {
					IntPtr ip = Marshal.ReadIntPtr(cu.rgpszUsageIdentifier, i * IntPtr.Size);
					try {
						ret.Add(Marshal.PtrToStringAnsi(ip));
					} catch {}
				}
				return ret;
			} finally {
				Marshal.FreeHGlobal(buffer);
			}
		}
		/// <summary>
		/// Returns a <see cref="CertificateChain"/> where the leaf certificate corresponds to this <see cref="Certificate"/>.
		/// </summary>
		/// <returns>The CertificateChain corresponding to this Certificate.</returns>
		/// <exception cref="CertificateException">An error occurs while building the certificate chain.</exception>
		public CertificateChain GetCertificateChain() {
			if (m_Chain == null)
				m_Chain = new CertificateChain(this, Store);
			return m_Chain;
		}
		/// <summary>
		/// Checks whether the <see cref="Certificate"/> has a private key associated with it.
		/// </summary>
		/// <returns><b>true</b> if the certificate has a private key associated with it, <b>false</b> otherwise.</returns>
		public bool HasPrivateKey() {
			int handle = 0;
			int keyspec = 0;
			int free = 0;
			bool ret = false;
			if (SspiProvider.CryptAcquireCertificatePrivateKey(Handle, SecurityConstants.CRYPT_ACQUIRE_COMPARE_KEY_FLAG | SecurityConstants.CRYPT_ACQUIRE_SILENT_FLAG, IntPtr.Zero, ref handle, ref keyspec, ref free) != 0)
				ret = true;
			if (free != 0)
				SspiProvider.CryptReleaseContext(handle, 0);
			return ret;
		}
		/// <summary>
		/// Returns the name of the format of this X.509v3 certificate.
		/// </summary>
		/// <returns>The format of this X.509 certificate.</returns>
		/// <remarks>The format X.509 is always returned in this implementation.</remarks>
		public string GetFormat() {
			return "X509";
		}
		/// <summary>
		/// Returns the hash code for the X.509v3 certificate as an integer.
		/// </summary>
		/// <returns>The hash code for the X.509 certificate as an integer.</returns>
		/// <remarks>If the X.509 certificate hash is an array of more than 4 bytes, any byte after the fourth byte is not seen in this integer representation.</remarks>
		public override int GetHashCode() {
			byte[] hash = GetCertHash();
			byte[] buffer = new byte[4];
			if (hash.Length < buffer.Length)
				Array.Copy(hash, 0, buffer, 0, hash.Length);
			else
				Array.Copy(hash, 0, buffer, 0, buffer.Length);
			return BitConverter.ToInt32(buffer, 0);
		}
		/// <summary>
		/// Compares two <see cref="Certificate"/> objects for equality.
		/// </summary>
		/// <param name="other">A Certificate object to compare to the current object.</param>
		/// <returns><b>true</b> if the current Certificate object is equal to the object specified by <paramref name="other"/>; otherwise, <b>false</b>.</returns>
		public virtual bool Equals(Certificate other) {
			if (other == null)
				return false;
			CertificateContext cc1 = (CertificateContext)Marshal.PtrToStructure(this.Handle, typeof(CertificateContext));
			CertificateContext cc2 = (CertificateContext)Marshal.PtrToStructure(other.Handle, typeof(CertificateContext));
			return SspiProvider.CertCompareCertificate(SecurityConstants.X509_ASN_ENCODING | SecurityConstants.PKCS_7_ASN_ENCODING, cc1.pCertInfo, cc2.pCertInfo) != 0;
		}
		/// <summary>
		/// Compares a <see cref="Certificate"/> object and an <see cref="X509Certificate"/> object for equality.
		/// </summary>
		/// <param name="other">An X509Certificate object to compare to the current object.</param>
		/// <returns><b>true</b> if the current Certificate object is equal to the object specified by <paramref name="other"/>; otherwise, <b>false</b>.</returns>
		public virtual bool Equals(X509Certificate other) {
			if (other == null)
				return false;
			return other.GetCertHashString() == this.GetCertHashString();
		}
		/// <summary>
		/// Compares two <see cref="Certificate"/> objects for equality.
		/// </summary>
		/// <param name="other">A Certificate object to compare to the current object.</param>
		/// <returns><b>true</b> if the current Certificate object is equal to the object specified by <paramref name="other"/>; otherwise, <b>false</b>.</returns>
		public override bool Equals(object other) {
			try {
				return Equals((Certificate)other);
			} catch {
				try {
					return Equals((X509Certificate)other);
				} catch {
					return false;
				}
			}
		}
		/// <summary>
		/// Returns an array of usages consisting of the intersection of the valid usages for all certificates in an array of certificates.
		/// </summary>
		/// <param name="certificates">Array of certificates to be checked for valid usage.</param>
		/// <returns>An array of valid usages -or- a null reference (<b>Nothing</b> in Visual Basic) if all certificates support all usages.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="certificates"/> is a null reference (<b>Nothing</b> in Visual Basic).</exception>
		/// <exception cref="ArgumentException">The array of certificates contains at least one invalid entry.</exception>
		/// <exception cref="CertificateException">An error occurs while determining the intersection of valid usages.</exception>
		public static string[] GetValidUsages(Certificate[] certificates) {
			if (certificates == null)
				throw new ArgumentNullException();
			IntPtr buffer = IntPtr.Zero;
			IntPtr certs = Marshal.AllocHGlobal(certificates.Length * IntPtr.Size);
			try {
				for(int i = 0; i < certificates.Length; i++) {
					if (certificates[i] == null)
						throw new ArgumentException();
					Marshal.WriteIntPtr(certs, i * IntPtr.Size, certificates[i].Handle);
				}
				int count = 0, bytes = 0;
				if (SspiProvider.CertGetValidUsages(certificates.Length, certs, ref count, buffer, ref bytes) == 0)
					throw new CertificateException("Unable to get the valid usages.");
				if (count == -1)
					return null; // every usage is valid
				buffer = Marshal.AllocHGlobal(bytes);
				if (SspiProvider.CertGetValidUsages(certificates.Length, certs, ref count, buffer, ref bytes) == 0)
					throw new CertificateException("Unable to get the valid usages.");
				string[] ret = new string[count];
				for(int i = 0; i < count; i++) {
					ret[i] = Marshal.PtrToStringAnsi(Marshal.ReadIntPtr(buffer, i * IntPtr.Size));
				}
				return ret;
			} finally {
				Marshal.FreeHGlobal(certs);
				if (buffer != IntPtr.Zero)
					Marshal.FreeHGlobal(buffer);
			}
		}
		/// <summary>
		/// Saves the <see cref="Certificate"/> as a PFX encoded file.
		/// </summary>
		/// <param name="filename">The filename of the new PFX file.</param>
		/// <param name="password">The password to use when encrypting the private keys.</param>
		/// <param name="withPrivateKeys"><b>true</b> if the private keys should be exported [if possible], <b>false</b> otherwise.</param>
		/// <param name="withParents"><b>true</b> if the parent certificates should be exported too [if possible], <b>false</b> otherwise.</param>
		/// <remarks>If the specified file already exists, the method will throw an exception.</remarks>
		/// <exception cref="ArgumentNullException"><paramref name="filename"/> or <paramref name="password"/> is a null reference (<b>Nothing</b> in Visual Basic).</exception>
		/// <exception cref="IOException">An error occurs while writing the data to the file.</exception>
		/// <exception cref="CertificateException">An error occurs while exporting the certificate store<br>-or-</br><br>an error occurs while building the certificate chain</br><br>-or-</br><br>an error occurs while creating the store</br><br>-or-</br><br>an error occurs while adding the certificate to the store.</br></exception>
		public void ToPfxFile(string filename, string password, bool withPrivateKeys, bool withParents) {
			CreateCertStore(withParents).ToPfxFile(filename, password, withPrivateKeys);
		}
		/// <summary>
		/// Saves the <see cref="Certificate"/> as a PFX encoded buffer.
		/// </summary>
		/// <param name="password">The password to use when encrypting the private keys.</param>
		/// <param name="withPrivateKeys"><b>true</b> if the private keys should be exported [if possible], <b>false</b> otherwise.</param>
		/// <param name="withParents"><b>true</b> if the parent certificates should be exported too [if possible], <b>false</b> otherwise.</param>
		/// <returns>An array of bytes that represents the PFX encoded certificate.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="password"/> is a null reference (<b>Nothing</b> in Visual Basic).</exception>
		/// <exception cref="CertificateException">An error occurs while exporting the certificate store<br>-or-</br><br>an error occurs while building the certificate chain</br><br>-or-</br><br>an error occurs while creating the store</br><br>-or-</br><br>an error occurs while adding the certificate to the store.</br></exception>
		public byte[] ToPfxBuffer(string password, bool withPrivateKeys, bool withParents) {
			return CreateCertStore(withParents).ToPfxBuffer(password, withPrivateKeys);
		}
		/// <summary>
		/// Creates an in memory <see cref="CertificateStore"/> with this <see cref="Certificate"/> in it.
		/// </summary>
		/// <param name="withParents"><b>true</b> if the parent certificates should be included [if possible], <b>false</b> otherwise.</param>
		/// <returns>A CertificateStore instance.</returns>
		private CertificateStore CreateCertStore(bool withParents) {
			CertificateStore store = new CertificateStore();
			if (withParents) {
				Certificate[] c = this.GetCertificateChain().GetCertificates();
				for(int i = 0; i < c.Length; i++) {
					store.AddCertificate(c[i]);
				}
			} else {
				store.AddCertificate(this);
			}
			return store;
		}
		/// <summary>
		/// Saves the <see cref="Certificate"/> as an encoded file.
		/// </summary>
		/// <param name="filename">The file where to store the certificate.</param>
		/// <exception cref="ArgumentNullException"><paramref name="filename"/> is a null reference (<b>Nothing</b> in Visual Basic).</exception>
		/// <exception cref="IOException">An error occurs while writing the data.</exception>
		/// <remarks>If the specified file already exists, this method will throw an exception.</remarks>
		public void ToCerFile(string filename) {
			SaveToFile(GetCertificateBuffer(), filename);
		}
		/// <summary>
		/// Saves the <see cref="Certificate"/> as an encoded buffer.
		/// </summary>
		/// <returns>An array of bytes that represents the encoded certificate.</returns>
		public byte[] ToCerBuffer() {
			return GetCertificateBuffer();
		}
		/// <summary>
		/// Returns a buffer with the encoded certificate.
		/// </summary>
		/// <returns>An array of bytes.</returns>
		private byte[] GetCertificateBuffer() {
			CertificateContext cc = (CertificateContext)Marshal.PtrToStructure(this.Handle, typeof(CertificateContext));
			byte[] buffer = new byte[cc.cbCertEncoded];
			Marshal.Copy(cc.pbCertEncoded, buffer, 0, cc.cbCertEncoded);
			return buffer;
		}
		/// <summary>
		/// Writes a buffer with data to a file.
		/// </summary>
		/// <param name="buffer">The buffer to write.</param>
		/// <param name="filename">The filename to write the data to.</param>
		/// <exception cref="ArgumentNullException"><paramref name="filename"/> is a null reference (<b>Nothing</b> in Visual Basic).</exception>
		/// <exception cref="IOException">An error occurs while writing the data.</exception>
		private void SaveToFile(byte[] buffer, string filename) {
			if (filename == null)
				throw new ArgumentNullException();
			try {
				FileStream fs = File.Open(filename, FileMode.CreateNew, FileAccess.Write, FileShare.None);
				fs.Write(buffer, 0, buffer.Length);
				fs.Close();
			} catch (Exception e) {
				throw new IOException("Could not write data to file.", e);
			}
		}
		/// <summary>
		/// Returns an X509Certificate object that corresponds to this <see cref="Certificate"/>.
		/// </summary>
		/// <returns>An X509Certificate instance.</returns>
		public X509Certificate ToX509() {
			return new X509Certificate(SspiProvider.CertDuplicateCertificateContext(Handle));
		}
		/// <summary>
		/// Gets the handle of the Certificate.
		/// </summary>
		/// <value>An IntPtr that represents the handle of the certificate.</value>
		/// <remarks>The handle returned by this property should not be closed. If the handle is closed by an external actor, the methods of the Certificate object may fail in undocumented ways [for instance, an Access Violation may occur].</remarks>
		public IntPtr Handle {
			get {
				return m_Handle;
			}
		}
		/// <summary>
		/// Gets the handle of the associated <see cref="CertificateStore"/>, if any.
		/// </summary>
		/// <value>A CertificateStore instance -or- a null reference (<b>Nothing</b> in Visual Basic) is no store is associated with this certificate.</value>
		internal CertificateStore Store {
			get {
				return m_Store;
			}
		}
		/// <summary>
		/// The handle of the <see cref="Certificate"/> object.
		/// </summary>
		private IntPtr m_Handle;
		/// <summary>
		/// The handle of the <see cref="CertificateStore"/> object.
		/// </summary>
		private CertificateStore m_Store;
		/// <summary>
		/// A <see cref="CertificateInfo"/> instance associated with this certificate.
		/// </summary>
		private CertificateInfo m_CertInfo;
		/// <summary>
		/// A reference to the associated <see cref="CertificateChain"/>.
		/// </summary>
		private CertificateChain m_Chain = null;
	}
}