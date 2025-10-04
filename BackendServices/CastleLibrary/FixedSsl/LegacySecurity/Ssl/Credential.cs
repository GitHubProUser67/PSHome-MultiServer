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
using Org.Mentalis.LegacySecurity.Certificates;

namespace Org.Mentalis.LegacySecurity.Ssl {
	/// <summary>
	/// Defines a credential that can be used in a secure socket connection to authenticate, encrypt and decrypt.
	/// </summary>
	internal class Credential {
		/// <summary>
		/// Initializes a new Credential instance.
		/// </summary>
		/// <param name="options">A <see cref="SecurityOptions"/> instance.</param>
		public Credential(SecurityOptions options) {
			m_Handle = Marshal.AllocHGlobal(8); //sizeof(CredHandle) == 8
			if (options.certificate == null && options.use == CredentialUse.Server)
				AcquireDefaultServerCertificate(options);
			else
				AcquireHandle(options);
		}
		/// <summary>
		/// Destroys the credential and releases all associated unmanaged resources.
		/// </summary>
		public void Close() {
			if (!m_Handle.Equals(IntPtr.Zero)) {
				SspiProvider.FreeCredentialsHandle(m_Handle);
				Marshal.FreeHGlobal(m_Handle);
				m_Handle = IntPtr.Zero;
			}
			try {
				GC.SuppressFinalize(this);
			} catch {}
		}
		/// <summary>
		/// Destroys the credential and releases all associated unmanaged resources.
		/// </summary>
		~Credential() {
			Close();
		}
		/// <summary>
		/// Acquires a new security credential, according to the specified <see cref="SecurityOptions"/>.
		/// </summary>
		/// <param name="options">The SecurityOptions to use when creating the credential.</param>
		/// <exception cref="SecurityException">An error occurs while acquiring the credentials.</exception>
		protected void AcquireHandle(SecurityOptions options) {
			SCHANNEL_CRED credential = new SCHANNEL_CRED();
			credential.dwVersion = SecurityConstants.SCHANNEL_CRED_VERSION;
			IntPtr cp;
			if (options.certificate != null) {
				cp = Marshal.AllocHGlobal(IntPtr.Size);
				Marshal.WriteIntPtr(cp, options.certificate.Handle);
				credential.cCreds = 1;
				credential.paCred = cp; // *certContext
			} else { // use default client authentication
				cp = IntPtr.Zero;
				credential.cCreds = 0;
				credential.paCred = IntPtr.Zero;
			}
			credential.cSupportedAlgs = 0;
			credential.palgSupportedAlgs = IntPtr.Zero;
			credential.grbitEnabledProtocols = (int)options.secureProtocol;
			if (options.use == CredentialUse.Client) {
				if (options.certificate == null)
					credential.dwFlags |= SecurityConstants.SCH_CRED_USE_DEFAULT_CREDS;
				else
					credential.dwFlags |= SecurityConstants.SCH_CRED_NO_DEFAULT_CREDS;
			}
			if (options.verifyType == CredentialVerification.Auto || options.verifyType == CredentialVerification.AutoWithoutCName)
				credential.dwFlags |= SecurityConstants.SCH_CRED_AUTO_CRED_VALIDATION;
			else
				credential.dwFlags |= SecurityConstants.SCH_CRED_MANUAL_CRED_VALIDATION;
			if (options.verifyType == CredentialVerification.AutoWithoutCName)
				credential.dwFlags |= SecurityConstants.SCH_CRED_NO_SERVERNAME_CHECK;
			// Try to create an SSPI credential
			int ret = SspiProvider.AcquireCredentialsHandle(
				null,								// Name of principal	
				SecurityConstants.UNISP_NAME,		// Name of package
				(int)options.use,					// Flags indicating use
				IntPtr.Zero,						// Pointer to logon ID
				ref credential,						// Package specific data
				IntPtr.Zero,						// Pointer to GetKey() func
				IntPtr.Zero,						// Value to pass to GetKey()
				m_Handle,							// (out) Cred Handle
				IntPtr.Zero);						// (out) Lifetime (optional)
			if (cp != IntPtr.Zero)
				Marshal.FreeHGlobal(cp);
			if(ret != SecurityConstants.SEC_E_OK)
				throw new SecurityException("AcquireCredentialsHandle failed");
		}
		/// <summary>
		/// Acquires a new secure server credential.
		/// </summary>
		/// <param name="options">The SecurityOptions to use when creating the credential.</param>
		/// <exception cref="SecurityException">Could not find a valid server certificate in the stores.</exception>
		protected void AcquireDefaultServerCertificate(SecurityOptions options) {
			bool ok = false;
			string[] stores = { "MY", "ROOT", "CA", "TRUST", "SPC" };
			for(int i = 0; !ok && i < stores.Length; i++) {
				try {
					CertificateStore cs = new CertificateStore(stores[i]);
					Certificate[] certs = cs.EnumCertificates(new string[]{"1.3.6.1.5.5.7.3.1"}); // search for server authentication certificates
					for(int c = 0; c < certs.Length; c++) {
						try {
							options.certificate = certs[c];
							AcquireHandle(options);
							ok = true;
						} catch {}
					}
				} catch {}
			}
			if (!ok)
				throw new CertificateException("Could not acquire default server certificate.");
		}
		/// <summary>
		/// Gets the credential handle.
		/// </summary>
		/// <value>An <see cref="IntPtr"/> instance.</value>
		public IntPtr Handle {
			get {
				return m_Handle;
			}
		}
		/// <summary>
		/// Holds the value of the Handle property.
		/// </summary>
		private IntPtr m_Handle;
	}
}