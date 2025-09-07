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
	/// Defines the different connection actions.
	/// </summary>
	internal enum ConnectionActions : int {
		/// <summary>Shuts down a connection.</summary>
		Shutdown = SecurityConstants.SCHANNEL_SHUTDOWN,
		/// <summary>Renegotiates a connection.</summary>
		Renegotiate = SecurityConstants.SCHANNEL_RENEGOTIATE
	}
	/// <summary>
	/// Defines the different handshake types.
	/// </summary>
	internal enum HandshakeType {
		/// <summary>Client initializes a connection.</summary>
		ClientHello,
		/// <summary>Server initializes a connection.</summary>
		ServerHello,
		/// <summary>Client renegotiates a connection.</summary>
		ClientRenegotiate,
		/// <summary>Server renegotiates a connection.</summary>
		ServerRenegotiate
	}
	/// <summary>
	/// Specifies the type of security protocol that an instance of the SecureSocket class can use. If you require PCT support, you can enable it inside the source code. Open the SecureSocket.cs file, scroll to the bottom and follow the instructions.
	/// </summary>
	//[CLSCompliant(true)]
	public enum SecureProtocol : int {
		/// <summary>No security protocol will be used. The SecureSocket will act as a normal Socket.</summary>
		None = 0,
		/*
		 *  To the developers using this class library: 
		 *  -------------------------------------------
		 *    If you require PCT support in your application, simply uncomment the enumeration field below.
		 *    However, do note that PCT should not be used in new applications; in fact, PCT is disabled by
		 *    default on Windows Server 2003. Using the PCT protocol on this platform is therefore not
		 *    supported and will result in an exception. The PCT protocol can be turned on again in Windows
		 *    Server 2003 by changing some registry entries, but this is not recommended.
		 */
		// <summary>PCT will be used to authenticate the client and encrypt the data.</summary>
		// Pct1 = 0x1 | 0x2,  // uncomment this line to add PCT support
		/// <summary>SSLv2 will be used to authenticate the client and encrypt the data. SSL2 should not be used for new applications; use SSL3 and TLS1 whenever possible.</summary>
		Ssl2 = 0x4 | 0x8,
		/// <summary>SSLv3 will be used to authenticate the client and encrypt the data.</summary>
		Ssl3 = 0x10 | 0x20,
		/// <summary>TLS will be used to authenticate the client and encrypt the data.</summary>
		Tls1 = 0x40 | 0x80
	}
	/// <summary>
	/// The SecBuffer structure describes a buffer allocated by a transport application to pass to a security package.
	/// </summary>
	//[StructLayout(LayoutKind.Sequential, CharSet=CharSet.Ansi)]
	//[CLSCompliant(true)]
	internal class SecBuffer {
		public SecBuffer() : this(SecurityConstants.SECBUFFER_EMPTY) {}
		public SecBuffer(int type) {
			this.cbBuffer = 0;
			this.BufferType = type;
			this.ptrBuffer = IntPtr.Zero;
			this.pvBuffer = null;
		}
		public SecBuffer(int type, byte[] buffer, int len) {
			this.cbBuffer = len;
			this.BufferType = type;
			this.ptrBuffer = IntPtr.Zero;
			this.pvBuffer = buffer;
		}
		/// <summary>Specifies the size, in bytes, of the buffer pointed to by the pvBuffer member.</summary>
		public int cbBuffer;
		/// <summary>Bit flags that indicate the type of buffer.</summary>
		public int BufferType;
		/// <summary>Pointer to a buffer.</summary>
		public IntPtr ptrBuffer;
		/// <summary>Byte array to a buffer.</summary>
		public byte[] pvBuffer; // this last entry is not part of the Win32 equivalent !!!
	}
	/// <summary>
	/// The SecBufferDesc structure describes an array of SecBuffer structures to pass from a transport application to a security package.
	/// </summary>
	//[StructLayout(LayoutKind.Sequential, CharSet=CharSet.Ansi)]
	//[CLSCompliant(true)]
	internal class SecBufferDesc {
		public SecBufferDesc(int count) {
			ulVersion = SecurityConstants.SECBUFFER_VERSION;
			cBuffers = count;
			pBuffers = new SecBuffer[count];
		}
		public SecBufferDesc() {}
		/// <summary>Specifies the version number of this structure. This member must be SECBUFFER_VERSION.</summary>
		public int ulVersion;
		/// <summary>Indicates the number of SecBuffer structures in the pBuffers array.</summary>
		public int cBuffers;
		/// <summary>Pointer to an array of SecBuffer structures.</summary>
		public SecBuffer[] pBuffers;
	}
	/// <summary>
	/// The SCHANNEL_CRED structure contains the data for an Schannel credential.
	/// </summary>
	[StructLayout(LayoutKind.Sequential, CharSet=CharSet.Ansi)]
	//[CLSCompliant(true)]
	internal struct SCHANNEL_CRED {
		/// <summary>Set to SCHANNEL_CRED_VERSION.</summary>
		public int dwVersion;
		/// <summary>Number of structures in the paCred array.</summary>
		public int cCreds;
		/// <summary>Array of pointers to CERT_CONTEXT structures. Each pointer specifies a certificate containing a private key to be used in authenticating the application. Typically, this array contains one structure for each key exchange method supported by the application.<br>Client applications often pass in an empty list and either depend on Schannel to find an appropriate certificate or create a certificate later if needed.</br></summary>
		public IntPtr paCred; 
		/// <summary>Optional. Valid for server applications only. Handle to a certificate store containing self-signed root certificates for certification authorities (CAs) trusted by the application. This member is used only by server-side applications requiring client authentication.</summary>
		public IntPtr hRootStore;
		/// <summary>Reserved.</summary>
		public int cMappers;
		/// <summary>Reserved.</summary>
		public IntPtr aphMappers;
		/// <summary>Number of algorithms in the palgSupportedAlgs array.</summary>
		public int cSupportedAlgs;
		/// <summary>Optional. Pointer to an array of ALG_IDs representing the algorithms supported by connections made with credentials acquired using this structure. If cSupportedAlgs is zero or palgSupportedAlgs is NULL, Schannel uses the system defaults.</summary>
		public IntPtr palgSupportedAlgs;
		/// <summary>Optional. DWORD containing a bit string representing the protocols supported by connections made with credentials acquired using this structure. If this member is zero, Schannel selects the protocol. Transport Layer Security 1.0 should be chosen for new development.</summary>
		public int grbitEnabledProtocols;
		/// <summary>Minimum bulk encryption cipher strength allowed for connections, in bits.<br>If this member is zero, Schannel uses the system default. If this member is -1 the SSL3/TLS MAC-only cipher suites (also known as NULL cipher) are enabled.</br></summary>
		public int dwMinimumCipherStrength;
		/// <summary>Maximum bulk encryption cipher strength allowed for connections, in bits.<br>If this member is zero, Schannel uses the system default.</br></summary>
		public int dwMaximumCipherStrength;
		/// <summary>The maximum life span of credentials acquired using this structure.</summary>
		public int dwSessionLifespan;
		/// <summary>Contains bit flags that control the behavior of Schannel in Windows 2000 and Windows XP.</summary>
		public int dwFlags;
		/// <summary>Reserved. Must be zero.</summary>
		public int reserved;
	}
	/// <summary>
	/// Specifies the use of the credential.
	/// </summary>
	//[CLSCompliant(true)]
	public enum CredentialUse : int {
		/// <summary>The credential can be used for server sockets.</summary>
		Server = SecurityConstants.SECPKG_CRED_INBOUND,
		/// <summary>The credential can be used for client sockets.</summary>
		Client = SecurityConstants.SECPKG_CRED_OUTBOUND
	}
	/// <summary>
	/// The SecPkgContext_StreamSizes structure indicates the sizes of the various stream components for use with the message support functions. The QueryContextAttributes function uses this structure.
	/// </summary>
	[StructLayout(LayoutKind.Sequential, CharSet=CharSet.Ansi)]
	//[CLSCompliant(true)]
	internal struct SecPkgContext_StreamSizes {
		/// <summary>Specifies the size, in bytes, of the header portion. If zero, no header is used.</summary>
		public int cbHeader;
		/// <summary>Specifies the size, in bytes, of the trailer portion. If zero, no trailer is used.</summary>
		public int cbTrailer;
		/// <summary>Specifies the size, in bytes, of the largest message that can be encapsulated.</summary>
		public int cbMaximumMessage;
		/// <summary>Specifies the number of buffers to pass.</summary>
		public int cBuffers;
		/// <summary>Specifies the preferred integral size of the messages. F or example, 8 indicates that messages should be of size 0 mod 8 for optimal performance. Messages other than this block size can be padded.</summary>
		public int cbBlockSize;
	}
	/// <summary>
	/// Specifies the method used to verify the remote credential.
	/// </summary>
	//[CLSCompliant(true)]
	public enum CredentialVerification : int {
		/// <summary>The remote certificate will be manually verified.</summary>
		/// <remarks>When an incoming connection is accepted, the SecureSocket will raise a CertVerification event.</remarks>
		Manual,
		/// <summary>The remote certificate will be automatically verified by the SSPI.</summary>
		Auto,
		/// <summary>The remote certificate will be automatically verified by the SSPI, but the common name of the server will not be checked.</summary>
		AutoWithoutCName,
		/// <summary>The remote certificate will not be verified.</summary>
		None
	}
	/// <summary>
	/// Specifies the possible security flags.
	/// </summary>
	/// <remarks>These flags can be combined with a bitwise OR.</remarks>
	//[CLSCompliant(true)]
	public enum SecurityFlags {
		/// <summary></summary>
		Default = SecurityConstants.ISC_REQ_SEQUENCE_DETECT | SecurityConstants.ISC_REQ_REPLAY_DETECT | SecurityConstants.ISC_REQ_CONFIDENTIALITY,
		/// <summary>Detect messages that are sent out of sequence.</summary>
		SequenceDetection = SecurityConstants.ISC_REQ_SEQUENCE_DETECT,
		/// <summary>Detect replayed messages.</summary>
		ReplayDetection = SecurityConstants.ISC_REQ_REPLAY_DETECT,
		/// <summary>Encrypt data before sending it over the network.</summary>
		Confidentiality = SecurityConstants.ISC_REQ_CONFIDENTIALITY,
		/// <summary>Require client authentication.</summary>
		MutualAuthentication = SecurityConstants.ISC_REQ_MUTUAL_AUTH
	}
	/// <summary>
	/// References the callback method to be called when the remote certificate should be verified.
	/// </summary>
	/// <param name="socket">The <see cref="SecureSocket"/> that received the certificate to verify.</param>
	/// <param name="remote">The certificate of the remote party to verify.</param>
	/// <param name="e">A <see cref="VerifyEventArgs"/> instance used to (in)validate the certificate.</param>
	//[CLSCompliant(true)]
	public delegate void CertVerifyEventHandler(SecureSocket socket, Certificate remote, VerifyEventArgs e);
}