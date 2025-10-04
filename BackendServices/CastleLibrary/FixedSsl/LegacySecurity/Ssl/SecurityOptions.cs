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
using Org.Mentalis.LegacySecurity.Certificates;

namespace Org.Mentalis.LegacySecurity.Ssl {
	/// <summary>
	/// Represents the security options that should be used when connecting to a secure server, or when accepting secure connections.
	/// </summary>
	public struct SecurityOptions {
		/// <summary>
		/// Initializes a new instance of the SecurityOptions structure.
		/// </summary>
		/// <param name="protocol">One of the <see cref="SecureProtocol"/> values.</param>
		/// <param name="cert">A <see cref="Certificate"/> instance.</param>
		/// <param name="use">One of the <see cref="CredentialUse"/> values.</param>
		/// <param name="verifyType">One of the <see cref="CredentialVerification"/> values.</param>
		/// <param name="verifier">The <see cref="CertVerifyEventHandler"/> delegate.</param>
		/// <param name="commonName">The common name of the remote computer. This is usually a domain name.</param>
		/// <param name="flags">A bitwise combination of the <see cref="SecurityFlags"/> values.</param>
		public SecurityOptions(SecureProtocol protocol, Certificate cert, CredentialUse use, CredentialVerification verifyType, CertVerifyEventHandler verifier, string commonName, SecurityFlags flags) {
			this.secureProtocol = protocol;
			this.certificate = cert;
			this.use = use;
			this.verifyType = verifyType;
			this.verifier = verifier;
			this.commonName = commonName;
			this.flags = flags;
		}
		/// <summary>
		/// Initializes a new instance of the SecurityOptions structure.
		/// </summary>
		/// <param name="protocol">One of the <see cref="SecureProtocol"/> values.</param>
		/// <param name="cert">A <see cref="Certificate"/> instance.</param>
		/// <param name="use">One of the <see cref="CredentialUse"/> values.</param>
		/// <remarks>
		/// All other members of the structure will be instantiated with default values.
		/// </remarks>
		public SecurityOptions(SecureProtocol protocol, Certificate cert, CredentialUse use) : this(protocol, cert, use, CredentialVerification.Auto, null, null, SecurityFlags.Default) {}
		/// <summary>
		/// Initializes a new instance of the SecurityOptions structure.
		/// </summary>
		/// <param name="protocol">One of the <see cref="SecureProtocol"/> values.</param>
		/// <remarks>
		/// All other members of the structure will be instantiated with default values.
		/// </remarks>
		public SecurityOptions(SecureProtocol protocol) : this(protocol, null, CredentialUse.Client, CredentialVerification.Auto, null, null, SecurityFlags.Default) {}
		/// <summary>One of the <see cref="SecureProtocol"/> values.</summary>
		public SecureProtocol secureProtocol;
		/// <summary>A <see cref="Certificate"/> instance.</summary>
		public Certificate certificate;
		/// <summary>One of the <see cref="CredentialUse"/> values.</summary>
		public CredentialUse use;
		/// <summary>One of the <see cref="CredentialVerification"/> values.</summary>
		public CredentialVerification verifyType;
		/// <summary>The <see cref="CertVerifyEventHandler"/> delegate.</summary>
		public CertVerifyEventHandler verifier;
		/// <summary>The common name of the remote computer. This is usually a domain name.</summary>
		public string commonName;
		/// <summary>A bitwise combination of the <see cref="SecurityFlags"/> values.</summary>
		public SecurityFlags flags;
	}
}
