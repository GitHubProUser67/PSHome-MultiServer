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

namespace Org.Mentalis.LegacySecurity.Ssl {
	/// <summary>
	/// Implements various methods to do SSL and TLS related tasks.
	/// </summary>
	internal class SslProvider {
		/// <summary>
		/// Initializes a new SslProvider instance.
		/// </summary>
		/// <param name="protocol">The protocol to use.</param>
		/// <param name="credential">The credential to use when authenticating.</param>
		public SslProvider(SecureProtocol protocol, Credential credential) {
			m_Context = Marshal.AllocHGlobal(8); //sizeof(CtxtHandle) == 8
			m_Protocol = protocol;
			m_Credential = credential;
		}
		/// <summary>
		/// Destroys the SslProvider instance and releases all unmanaged resources.
		/// </summary>
		public void Dispose() {
			if (!m_Context.Equals(IntPtr.Zero)) {
				SspiProvider.DeleteSecurityContext(m_Context);
				Marshal.FreeHGlobal(m_Context);
				m_Context = IntPtr.Zero;
			}
			try {
				GC.SuppressFinalize(this);
			} catch {}
		}
		/// <summary>
		/// Destroys the SslProvider instance and releases all unmanaged resources.
		/// </summary>
		~SslProvider() {
			Dispose();
		}
		/// <summary>
		/// Returns an array of bytes that the client has to send to the server to initiate an SSL/TLS handshake.
		/// </summary>
		/// <param name="serverName">A name that uniquely identifies the server.</param>
		/// <param name="flags">A bitwise combination of the <see cref="SecurityFlags"/> values.</param>
		/// <param name="verifyType">One of the <see cref="CredentialVerification"/> values.</param>
		/// <returns>An array of bytes if successful, null otherwise.</returns>
		public byte[] GetClientHello(string serverName, SecurityFlags flags, CredentialVerification verifyType) {
			int dwSSPIOutFlags = 0;
			int scRet;
			int dwSSPIFlags = (int)flags | SecurityConstants.ISC_REQ_ALLOCATE_MEMORY | SecurityConstants.ISC_RET_EXTENDED_ERROR | SecurityConstants.ISC_REQ_STREAM;
			if (verifyType == CredentialVerification.Manual || verifyType == CredentialVerification.None)
				dwSSPIFlags |= SecurityConstants.ISC_REQ_MANUAL_CRED_VALIDATION;
			//  Initiate a ClientHello message and generate a token.
			SecBufferDesc OutBuffer = new SecBufferDesc(1);
			OutBuffer.pBuffers[0] = new SecBuffer(SecurityConstants.SECBUFFER_TOKEN);
			// call the SSPI function
			IntPtr output = this.SecBuffDescToUnmanaged(OutBuffer);
			scRet = SspiProvider.InitializeSecurityContext(Credential.Handle, IntPtr.Zero, serverName, dwSSPIFlags, 0, SecurityConstants.SECURITY_NATIVE_DREP,
				IntPtr.Zero, 0, Context, output, ref dwSSPIOutFlags, IntPtr.Zero);
			OutBuffer = this.UnmanagedToSecBuffDesc(output);
			Marshal.FreeHGlobal(output);
			// interpret output
			if(scRet != SecurityConstants.SEC_I_CONTINUE_NEEDED)
				return null; // some error occured
			if (OutBuffer.pBuffers[0].cbBuffer > 0 && OutBuffer.pBuffers[0].pvBuffer != null) {
				SspiProvider.FreeContextBuffer(OutBuffer.pBuffers[0].ptrBuffer);
				return OutBuffer.pBuffers[0].pvBuffer;
			} else {
				return null;
			}
		}
		/// <summary>
		/// Returns the proper bytes that have to be sent to the remote server in order to continue the handshake.
		/// </summary>
		/// <param name="inbuf">An array of bytes that contains the bytes that were received from the remote host after sending the HELLO bytes.</param>
		/// <param name="inSize">The number of bytes that were received from the remote host after sending the HELLO bytes.</param>
		/// <param name="outbuf">A pointer to an array of bytes that receives the bytes that have to be sent to the server, if any.</param>
		/// <param name="secStatus">A pointer to an integer that will receive the status of the negotiation.</param>
		/// <param name="flags">A bitwise combination of the <see cref="SecurityFlags"/> values.</param>
		/// <param name="verifyType">One of the <see cref="CredentialVerification"/> values.</param>
		/// <returns>Returns zero if successful, a nonzero value otherwise.</returns>
		/// <remarks>
		/// The secStatus variable can hold one of the following values after calling the GetClientHandshakeBytes method:
		/// <br>SEC_E_OK: Negotiation is complete. If outbuf is not null, then those bytes should be sent to the server. If extra is not null, then it contains bytes that are part of the first encrypted message sent from the server to the client.</br>
		/// <br>SEC_I_CONTINUE_NEEDED: Negotiation is not yet complete, but the bytes in outbuf should be sent to the server and the client must wait for a server reply in order to continue the handshake.</br>
		/// <br>SEC_E_INCOMPLETE_MESSAGE: The received bytes are incomplete. The client should read more bytes and try again.</br>
		/// </remarks>
		/// <remarks>
		/// If <c>inSize</c> is not zero after the GetClientHandshakeBytes has been called, it specifies the number of bytes in the input buffer that should not be overwritten by consecutive receives from the remote host. Instead, consecutive receives should start appending new bytes from position <c>inSize</c> in the buffer.
		/// </remarks>
		public int GetClientHandshakeBytes(ref byte[] inbuf, ref int inSize, ref byte[] outbuf, ref int secStatus, SecurityFlags flags, CredentialVerification verifyType) {
			int dwSSPIOutFlags = 0, dwSSPIFlags = (int)flags | SecurityConstants.ISC_RET_EXTENDED_ERROR | SecurityConstants.ISC_REQ_ALLOCATE_MEMORY | SecurityConstants.ISC_REQ_STREAM;
			if (verifyType == CredentialVerification.Manual || verifyType == CredentialVerification.None)
				dwSSPIFlags |= SecurityConstants.ISC_REQ_MANUAL_CRED_VALIDATION;
			if (inbuf == null || inSize <= 0)
				return -1;
			// Set up the input buffers. Buffer 0 is used to pass in data
			// received from the server. Schannel will consume some or all
			// of this. Leftover data (if any) will be placed in buffer 1 and
			// given a buffer type of SECBUFFER_EXTRA.
			SecBufferDesc InBuffer = new SecBufferDesc(2);
			InBuffer.pBuffers[0] = new SecBuffer(SecurityConstants.SECBUFFER_TOKEN, inbuf, inSize);
			InBuffer.pBuffers[1] = new SecBuffer();
			// Set up the output buffers.
			SecBufferDesc OutBuffer = new SecBufferDesc(1);
			OutBuffer.pBuffers[0] = new SecBuffer(SecurityConstants.SECBUFFER_TOKEN);
			// Call InitializeSecurityContext.
			IntPtr input = this.SecBuffDescToUnmanaged(InBuffer), output = this.SecBuffDescToUnmanaged(OutBuffer);
			secStatus = SspiProvider.InitializeSecurityContext(Credential.Handle, Context, null, dwSSPIFlags, 0, SecurityConstants.SECURITY_NATIVE_DREP,
				input, 0, IntPtr.Zero, output, ref dwSSPIOutFlags, IntPtr.Zero);
			byte[] bytes= new Byte[2956];
			Marshal.Copy(input, bytes, 0, bytes.Length);
			InBuffer = this.UnmanagedToSecBuffDesc(input);
			OutBuffer = this.UnmanagedToSecBuffDesc(output);
			Marshal.FreeHGlobal(input);
			Marshal.FreeHGlobal(output);
			// If InitializeSecurityContext was successful (or if the error was 
			// one of the special extended ones), send the contents of the output
			// buffer to the server.
			if(secStatus == SecurityConstants.SEC_E_OK || secStatus == SecurityConstants.SEC_I_CONTINUE_NEEDED || (secStatus < 0 && (dwSSPIOutFlags & SecurityConstants.ISC_RET_EXTENDED_ERROR) != 0)) {
				if(OutBuffer.pBuffers[0].cbBuffer != 0 && OutBuffer.pBuffers[0].pvBuffer != null) {
					outbuf = OutBuffer.pBuffers[0].pvBuffer;
					SspiProvider.FreeContextBuffer(OutBuffer.pBuffers[0].ptrBuffer);
				} else {
					outbuf = null;
				}
			}
			if(InBuffer.pBuffers[1].BufferType == SecurityConstants.SECBUFFER_EXTRA && InBuffer.pBuffers[1].pvBuffer != null) {
				inSize = InBuffer.pBuffers[1].cbBuffer;
				Array.Copy(InBuffer.pBuffers[1].pvBuffer, 0, inbuf, 0, inSize);
				SspiProvider.FreeContextBuffer(OutBuffer.pBuffers[0].ptrBuffer);
			} else if(secStatus == SecurityConstants.SEC_E_INCOMPLETE_MESSAGE) {
				inSize = InBuffer.pBuffers[0].cbBuffer;
				Array.Copy(InBuffer.pBuffers[0].pvBuffer, 0, inbuf, 0, inSize);
			} else {
				inSize = 0;
			}
			// Check for fatal error.
			if(secStatus < 0 && secStatus != SecurityConstants.SEC_E_INCOMPLETE_MESSAGE)
				return -4;
			// If InitializeSecurityContext returned SEC_I_INCOMPLETE_CREDENTIALS,
			// then the server just requested client authentication. 
			if(secStatus == SecurityConstants.SEC_I_INCOMPLETE_CREDENTIALS)
				return -5;
			return 0;
		}
		/// <summary>
		/// Returns the proper bytes that have to be sent to the remote client in order to continue the handshake.
		/// </summary>
		/// <param name="inbuf">An array of bytes that contains the bytes that were received from the remote client.</param>
		/// <param name="inSize">The number of bytes that were received from the remote client.</param>
		/// <param name="outbuf">A pointer to an array of bytes that receives the bytes that have to be sent to the client, if any.</param>
		/// <param name="secStatus">A pointer to an integer that will receive the status of the negotiation.</param>
		/// <param name="initContext"><b>true</b> if the security context has to be initialized, <b>false</b> otherwise.</param>
		/// <param name="flags">A bitwise combination of the <see cref="SecurityFlags"/> values.</param>
		/// <returns>Returns zero if successful, a nonzero value otherwise.</returns>
		/// <remarks>
		/// The secStatus variable can hold one of the following values after calling the GetClientHandshakeBytes method:
		/// <br>SEC_E_OK: Negotiation is complete. If outbuf is not null, then those bytes should be sent to the server. If extra is not null, then it contains bytes that are part of the first encrypted message sent from the server to the client.</br>
		/// <br>SEC_I_CONTINUE_NEEDED: Negotiation is not yet complete, but the bytes in outbuf should be sent to the server and the client must wait for a server reply in order to continue the handshake.</br>
		/// <br>SEC_E_INCOMPLETE_MESSAGE: The received bytes are incomplete. The client should read more bytes and try again.</br>
		/// </remarks>
		/// <remarks>
		/// If <c>inSize</c> is not zero after the GetClientHandshakeBytes has been called, it specifies the number of bytes in the input buffer that should not be overwritten by consecutive receives from the remote client. Instead, consecutive receives should start appending new bytes from position <c>inSize</c> in the buffer.
		/// </remarks>
		public int GetServerHandshakeBytes(ref byte[] inbuf, ref int inSize, ref byte[] outbuf, ref int secStatus, bool initContext, SecurityFlags flags) {
			int dwSSPIOutFlags = 0, dwSSPIFlags = (int)flags | SecurityConstants.ASC_REQ_EXTENDED_ERROR | SecurityConstants.ASC_REQ_ALLOCATE_MEMORY | SecurityConstants.ASC_REQ_STREAM;
			if (inbuf == null || inSize <= 0)
				return -1;
			secStatus = SecurityConstants.SEC_I_CONTINUE_NEEDED;
			// Set up the input buffers. Buffer 0 is used to pass in data
			// received from the server. Schannel will consume some or all
			// of this. Leftover data (if any) will be placed in buffer 1 and
			// given a buffer type of SECBUFFER_EXTRA.
			SecBufferDesc InBuffer = new SecBufferDesc(2);
			InBuffer.pBuffers[0] = new SecBuffer(SecurityConstants.SECBUFFER_TOKEN, inbuf, inSize);
			InBuffer.pBuffers[1] = new SecBuffer();
			// Set up the output buffers
			SecBufferDesc OutBuffer = new SecBufferDesc(1);
			OutBuffer.pBuffers[0] = new SecBuffer(SecurityConstants.SECBUFFER_TOKEN);
			// Call AcceptSecurityContext.
			IntPtr input = this.SecBuffDescToUnmanaged(InBuffer), output = this.SecBuffDescToUnmanaged(OutBuffer);
			secStatus = SspiProvider.AcceptSecurityContext(Credential.Handle, initContext ? IntPtr.Zero : Context, input, dwSSPIFlags, SecurityConstants.SECURITY_NATIVE_DREP, initContext ? Context : IntPtr.Zero, output, ref dwSSPIOutFlags, IntPtr.Zero);
			InBuffer = this.UnmanagedToSecBuffDesc(input);
			OutBuffer = this.UnmanagedToSecBuffDesc(output);
			Marshal.FreeHGlobal(input);
			Marshal.FreeHGlobal(output);
			// If AcceptSecurityContext was successful (or if the error was 
			// one of the special extended ones), send the contends of the output
			// buffer to the server.
			if(secStatus == SecurityConstants.SEC_E_OK || secStatus == SecurityConstants.SEC_I_CONTINUE_NEEDED || (secStatus < 0 && (dwSSPIOutFlags & SecurityConstants.ISC_RET_EXTENDED_ERROR) != 0)) {
				if(OutBuffer.pBuffers[0].cbBuffer != 0 && OutBuffer.pBuffers[0].pvBuffer != null) {
					outbuf = OutBuffer.pBuffers[0].pvBuffer;
					SspiProvider.FreeContextBuffer(OutBuffer.pBuffers[0].ptrBuffer);
				} else {
					outbuf = null;
				}
			}
			// Copy any leftover data from the "extra" buffer
			if (InBuffer.pBuffers[1].BufferType == SecurityConstants.SECBUFFER_EXTRA) {
				inSize = InBuffer.pBuffers[1].cbBuffer;
				Array.Copy(InBuffer.pBuffers[1].pvBuffer, 0, inbuf, 0, inSize);
				SspiProvider.FreeContextBuffer(InBuffer.pBuffers[1].ptrBuffer);
			} else if(secStatus == SecurityConstants.SEC_E_INCOMPLETE_MESSAGE) {
				inSize = InBuffer.pBuffers[0].cbBuffer;
				Array.Copy(InBuffer.pBuffers[0].pvBuffer, 0, inbuf, 0, inSize);
			} else {
				inSize = 0;
			}
			if(secStatus == SecurityConstants.SEC_E_OK) {
				return 0;
			} else if(secStatus < 0 && secStatus != SecurityConstants.SEC_E_INCOMPLETE_MESSAGE) {
				return -2;
			}
			return 0;
		}
		/// <summary>
		/// Returns the buffer sizes of the protocol in use.
		/// </summary>
		/// <param name="header">A pointer to an integer that will receive the size of the header.</param>
		/// <param name="message">A pointer to an integer that will receive the maximum size of a message.</param>
		/// <param name="trailer">A pointer to an integer that will receive the size of the trailer.</param>
		/// <returns>Returns zero if successful, a nonzero value otherwise.</returns>
		public int GetBufferSizes(ref int header, ref int message, ref int trailer) {
			SecPkgContext_StreamSizes sizes = new SecPkgContext_StreamSizes();
			int ret = SspiProvider.QueryContextAttributesSize(Context, SecurityConstants.SECPKG_ATTR_STREAM_SIZES, ref sizes);
			if (ret != SecurityConstants.SEC_E_OK) { // an error can occur if the context hasn't been initialized yet
				return -1;
			}
			header = sizes.cbHeader;
			message = sizes.cbMaximumMessage;
			trailer = sizes.cbTrailer;
			if (Protocol == SecureProtocol.Ssl2 && message > 32000)
				message = 32000; // QueryContextAttributesSize reports wrong buffer size for SSL2
			return 0;
		}
		/// <summary>
		/// Encrypts a message according to the current security context.
		/// </summary>
		/// <param name="buffer">The buffer to encrypt.</param>
		/// <returns>An array of bytes if successful, null otherwise.</returns>
		public byte[] EncryptMessage(byte[] buffer) {
			int header = 0, message = 0, trailer = 0;
			GetBufferSizes(ref header, ref message, ref trailer);
			if (buffer != null && buffer.Length > message)
				return null;
			SecBufferDesc Message = new SecBufferDesc(4);
			Message.pBuffers[0] = new SecBuffer(SecurityConstants.SECBUFFER_STREAM_HEADER, new byte[header], header);
			Message.pBuffers[1] = new SecBuffer(SecurityConstants.SECBUFFER_DATA, buffer, buffer == null ? 0 : buffer.Length);
			Message.pBuffers[2] = new SecBuffer(SecurityConstants.SECBUFFER_STREAM_TRAILER, new byte[trailer], trailer);
			Message.pBuffers[3] = new SecBuffer();
			IntPtr msgunm = this.SecBuffDescToUnmanaged(Message);
			int retval = SspiProvider.EncryptMessage(Context, 0, msgunm, 0);
			Message = this.UnmanagedToSecBuffDesc(msgunm);
			Marshal.FreeHGlobal(msgunm);
			if(retval == SecurityConstants.SEC_E_OK) {
				byte[] ret = new byte[Message.pBuffers[0].cbBuffer + Message.pBuffers[1].cbBuffer + Message.pBuffers[2].cbBuffer];
				if (Message.pBuffers[0].cbBuffer > 0)
					Array.Copy(Message.pBuffers[0].pvBuffer, 0, ret, 0, Message.pBuffers[0].cbBuffer);
				if (Message.pBuffers[1].cbBuffer > 0)
					Array.Copy(Message.pBuffers[1].pvBuffer, 0, ret, Message.pBuffers[0].cbBuffer, Message.pBuffers[1].cbBuffer);
				if (Message.pBuffers[2].cbBuffer > 0)
					Array.Copy(Message.pBuffers[2].pvBuffer, 0, ret, Message.pBuffers[0].cbBuffer + Message.pBuffers[1].cbBuffer, Message.pBuffers[2].cbBuffer);
				return ret;
			} else {
				return null;
			}
		}
		/// <summary>
		/// Decrypts a message according to the current security context.
		/// </summary>
		/// <param name="inbuf">The buffer to decrypt.</param>
		/// <param name="inSize">The number of bytes to decrypt.</param>
		/// <param name="status"> a pointer to an integer that receives the status of the decryption operation.</param>
		/// <returns>An array of bytes if successful, null otherwise.</returns>
		/// <remarks>
		/// The status variable can hold one of the following values after calling the DecryptMessage method:
		/// <br>SEC_E_OK: Decryption was completed successfully.</br>
		/// <br>SEC_I_RENEGOTIATE: Decryption was completed successfully but the remote host has requested a renegotiation of the security context.</br>
		/// <br>SEC_E_INCOMPLETE_MESSAGE: The data in the input buffers is incomplete. The application must obtain more data and call DecryptMessage again.</br>
		/// <br>SEC_I_CONTEXT_EXPIRED: The remote party shut down the connection.</br>
		/// <br>SEC_E_INVALID_HANDLE: The security context is invalid.</br>
		/// </remarks>
		/// <remarks>
		/// After decryption, all the unprocessed data is copied to the beginning of the input buffer and <c>inSize</c> receives the number of unprocessed data.
		/// <br>Thanks to Len Holgate for fixing a bug in this method</br>
		/// </remarks>
		public byte[] DecryptMessage(byte[] inbuf, ref int inSize, ref int status) {
			// Attempt to decrypt the received data.
			SecBufferDesc Message = new SecBufferDesc(4);
			Message.pBuffers[0] = new SecBuffer(SecurityConstants.SECBUFFER_DATA, inbuf, inSize);
			Message.pBuffers[1] = new SecBuffer();
			Message.pBuffers[2] = new SecBuffer();
			Message.pBuffers[3] = new SecBuffer();
			IntPtr msgunm = this.SecBuffDescToUnmanaged(Message);
			status = SspiProvider.DecryptMessage(Context, msgunm, 0, IntPtr.Zero);
			Message = this.UnmanagedToSecBuffDesc(msgunm);
			Marshal.FreeHGlobal(msgunm);
			// return if message is incomplete or if security context has expired
			if(status != SecurityConstants.SEC_E_OK && status != SecurityConstants.SEC_I_RENEGOTIATE)
				return null;
			// Locate data and (optional) extra buffers.
			SecBuffer pDataBuffer = null, pExtraBuffer = null;
			for(int i = 1; i < Message.cBuffers; i++) {
				if(Message.pBuffers[i].BufferType == SecurityConstants.SECBUFFER_DATA) {
					pDataBuffer = Message.pBuffers[i];
				}
				if(Message.pBuffers[i].BufferType == SecurityConstants.SECBUFFER_EXTRA) {
					pExtraBuffer = Message.pBuffers[i];
				}
			}
			byte[] ret = null;
			if (pDataBuffer != null && pDataBuffer.cbBuffer > 0) {
				ret = pDataBuffer.pvBuffer;
				SspiProvider.FreeContextBuffer(pDataBuffer.ptrBuffer);
			}
			if (pExtraBuffer != null) {
				Array.Copy(pExtraBuffer.pvBuffer, 0, inbuf, 0, pExtraBuffer.cbBuffer);
				inSize = pExtraBuffer.cbBuffer;
				SspiProvider.FreeContextBuffer(pExtraBuffer.ptrBuffer);
			} else if(status == SecurityConstants.SEC_E_INCOMPLETE_MESSAGE) {
				// do nothing
			} else {
				inSize = 0;
			}
			return ret;
		}
		/// <summary>
		/// Returns an array of bytes that should be sent to the remote host in order to force a renegotiate of the connection or to shut a connection down.
		/// </summary>
		/// <param name="flags">A bitwise combination of the <see cref="SecurityFlags"/> values.</param>
		/// <param name="action">One of the <see cref="ConnectionActions"/> values.</param>
		/// <param name="client"><b>true</b> if the caller is a client socket, <b>false</b> if the caller is a server socket.</param>
		/// <param name="serverName">The common name of the remote server.</param>
		/// <returns>An array of bytes that should be sent to the remote host in order to force a renegotiation.</returns>
		public byte[] GetActionBytes(SecurityFlags flags, ConnectionActions action, bool client, string serverName) {
			SecBufferDesc OutBuffer = new SecBufferDesc(1);
			int ret;
			if (action == ConnectionActions.Shutdown) {
				OutBuffer.pBuffers[0] = new SecBuffer(SecurityConstants.SECBUFFER_TOKEN, BitConverter.GetBytes((int)action), 4);
				// Notify schannel that we are about to close the connection.
				int dwType = (int)action;
				IntPtr input = this.SecBuffDescToUnmanaged(OutBuffer);
				ret = SspiProvider.ApplyControlToken(Context, input);
				Marshal.FreeHGlobal(input);
				if(ret < 0)
					return null;
			}
			// Build an SSL shutdown or negotiate message.
			int dwSSPIOutFlags = 0, dwSSPIFlags = (int)flags | SecurityConstants.ISC_RET_EXTENDED_ERROR | SecurityConstants.ISC_REQ_ALLOCATE_MEMORY | SecurityConstants.ISC_REQ_STREAM;
			OutBuffer.pBuffers[0] = new SecBuffer(SecurityConstants.SECBUFFER_TOKEN);
			IntPtr output = this.SecBuffDescToUnmanaged(OutBuffer);
			if (client) {
				ret = SspiProvider.InitializeSecurityContext(Credential.Handle, Context, serverName, dwSSPIFlags, 0, SecurityConstants.SECURITY_NATIVE_DREP, IntPtr.Zero, 0, IntPtr.Zero, output, ref dwSSPIOutFlags, IntPtr.Zero);
			} else {
				ret = SspiProvider.AcceptSecurityContext(Credential.Handle, Context, IntPtr.Zero, dwSSPIFlags, SecurityConstants.SECURITY_NATIVE_DREP, IntPtr.Zero, output, ref dwSSPIOutFlags, IntPtr.Zero);
			}
			OutBuffer = this.UnmanagedToSecBuffDesc(output);
			Marshal.FreeHGlobal(output);
			if(ret < 0 || OutBuffer.pBuffers[0].cbBuffer <= 0)
				return null;
			SspiProvider.FreeContextBuffer(OutBuffer.pBuffers[0].ptrBuffer);
			return OutBuffer.pBuffers[0].pvBuffer;
		}
		/// <summary>
		/// Converts a managed <see cref="SecBufferDesc"/> to a block of unmanaged memory.
		/// </summary>
		/// <param name="desc">The SecBufferDesc to convert.</param>
		/// <returns>A block of unmanaged memory that represents <paramref name="desc"/>.</returns>
		/// <remarks>
		/// This method does not check whether <paramref name="desc"/> is valid.
		/// </remarks>
		public IntPtr SecBuffDescToUnmanaged(SecBufferDesc desc) {
			int ds = IntPtr.Size + 8;
			int[] offsets = new int[desc.cBuffers];
			int length = (desc.cBuffers + 1) * ds;
			for(int i = 0; i < desc.cBuffers; i++) {
				offsets[i] = length;
				length += desc.pBuffers[i].cbBuffer;
			}
			IntPtr ret = Marshal.AllocHGlobal(length);
			// write descriptor
			Marshal.WriteInt32(ret, 0, desc.ulVersion);
			Marshal.WriteInt32(ret, 4, desc.cBuffers);
			Marshal.WriteIntPtr(ret, 8, new IntPtr(ret.ToInt64() + ds));
			// write buffers
			int offset;
			for(int i = 0; i < desc.cBuffers; i++) {
				offset = (i + 1) * ds;
				Marshal.WriteInt32(ret, offset, desc.pBuffers[i].cbBuffer);
				Marshal.WriteInt32(ret, offset + 4, desc.pBuffers[i].BufferType);
				if (desc.pBuffers[i].pvBuffer == null) {
					Marshal.WriteIntPtr(ret, offset + 8, IntPtr.Zero);
				} else {
					Marshal.WriteIntPtr(ret, offset + 8, new IntPtr(ret.ToInt64() + offsets[i]));
					Marshal.Copy(desc.pBuffers[i].pvBuffer, 0, new IntPtr(ret.ToInt64() + offsets[i]), desc.pBuffers[i].cbBuffer);
				}
			}
			return ret;
		}
		/// <summary>
		/// Converts a block of unmanaged memory to a managed <see cref="SecBufferDesc"/>.
		/// </summary>
		/// <param name="unman">The block of unmanaged memory to convert.</param>
		/// <returns>A SecBufferDesc that's represented by <paramref name="unman"/>.</returns>
		/// <remarks>
		/// This method does not check whether <paramref name="unman"/> is valid.
		/// </remarks>
		public SecBufferDesc UnmanagedToSecBuffDesc(IntPtr unman) {
			SecBufferDesc ret = new SecBufferDesc();
			int ds = IntPtr.Size + 8;
			ret.ulVersion = Marshal.ReadInt32(unman, 0);
			ret.cBuffers = Marshal.ReadInt32(unman, 4);
			ret.pBuffers = new SecBuffer[ret.cBuffers];
			IntPtr buffers = Marshal.ReadIntPtr(unman, 8);
			int offset = 0;
			for(int i = 0; i < ret.cBuffers; i++) {
				ret.pBuffers[i] = new SecBuffer();
				ret.pBuffers[i].cbBuffer = Marshal.ReadInt32(buffers, offset);
				ret.pBuffers[i].BufferType = Marshal.ReadInt32(buffers, offset + 4);
				ret.pBuffers[i].ptrBuffer = Marshal.ReadIntPtr(buffers, offset + 8);
				if (ret.pBuffers[i].ptrBuffer != IntPtr.Zero && ret.pBuffers[i].cbBuffer > 0) {
					int bt = ret.pBuffers[i].BufferType;
					if (bt == SecurityConstants.SECBUFFER_DATA || bt == SecurityConstants.SECBUFFER_EXTRA || bt == SecurityConstants.SECBUFFER_STREAM_HEADER || bt == SecurityConstants.SECBUFFER_STREAM_TRAILER || bt == SecurityConstants.SECBUFFER_TOKEN) {
						ret.pBuffers[i].pvBuffer = new byte[ret.pBuffers[i].cbBuffer];
						Marshal.Copy(ret.pBuffers[i].ptrBuffer, ret.pBuffers[i].pvBuffer, 0, ret.pBuffers[i].cbBuffer);
					}
				} else {
					ret.pBuffers[i].pvBuffer = null;
				}
				offset += ds;
			}
			return ret;
		}
		/// <summary>
		/// Returns a renegotiation BLOB.
		/// </summary>
		/// <param name="flags">A bitwise combination of one of more <see cref="SecurityFlags"/> values.</param>
		/// <param name="buffer">The input buffer received from the remote host.</param>
		/// <param name="len">The number of bytes in the input buffer.</param>
		/// <param name="ret">A reference to an integer that receives the status of the renegotiation.</param>
		/// <returns>An array of bytes that should be sent to the remote host -or- a null reference (<b>Nothing</b> in Visual Basic) if there are no bytes that should be sent to the remote host.</returns>
		public byte[] GetServerRenegotiationBytes(SecurityFlags flags, byte[] buffer, ref int len, ref int ret) {
			int dwSSPIOutFlags = 0, dwSSPIFlags = (int)flags | SecurityConstants.ISC_RET_EXTENDED_ERROR | SecurityConstants.ISC_REQ_ALLOCATE_MEMORY | SecurityConstants.ISC_REQ_STREAM;
			SecBufferDesc InBuffer = new SecBufferDesc(2);
			SecBufferDesc OutBuffer = new SecBufferDesc(1);
			// initialize the input buffers
			InBuffer.pBuffers[0] = new SecBuffer(SecurityConstants.SECBUFFER_TOKEN, buffer, len);
			InBuffer.pBuffers[1] = new SecBuffer();
			// initialize the output buffers
			OutBuffer.pBuffers[0] = new SecBuffer(SecurityConstants.SECBUFFER_TOKEN);
			// call the SSPI function
			IntPtr input = SecBuffDescToUnmanaged(InBuffer), output = SecBuffDescToUnmanaged(OutBuffer);
			ret = SspiProvider.AcceptSecurityContext(Credential.Handle, Context, input, dwSSPIFlags, SecurityConstants.SECURITY_NATIVE_DREP, IntPtr.Zero, output, ref dwSSPIOutFlags, IntPtr.Zero);
			OutBuffer = UnmanagedToSecBuffDesc(output);
			Marshal.FreeHGlobal(input);
			Marshal.FreeHGlobal(output);
			// interpret the output data
			byte[] retval = null;
			if (OutBuffer.pBuffers[0].BufferType == SecurityConstants.SECBUFFER_TOKEN && OutBuffer.pBuffers[0].cbBuffer > 0) {
				retval = OutBuffer.pBuffers[0].pvBuffer;
				SspiProvider.FreeContextBuffer(OutBuffer.pBuffers[0].ptrBuffer);
			}
			if (OutBuffer.pBuffers[0].BufferType == SecurityConstants.SECBUFFER_EXTRA && OutBuffer.pBuffers[0].cbBuffer > 0) {
				Array.Copy(OutBuffer.pBuffers[0].pvBuffer, 0, buffer, 0, OutBuffer.pBuffers[0].cbBuffer);
				SspiProvider.FreeContextBuffer(OutBuffer.pBuffers[0].ptrBuffer);
				len = OutBuffer.pBuffers[0].cbBuffer;
			} else if(ret == SecurityConstants.SEC_E_INCOMPLETE_MESSAGE) {
				len = InBuffer.pBuffers[0].cbBuffer;
				Array.Copy(InBuffer.pBuffers[0].pvBuffer, 0, buffer, 0, len);
			} else {
				len = 0;
			}
			return retval;
		}
		/// <summary>
		/// Returns a renegotiation BLOB.
		/// </summary>
		/// <param name="flags">A bitwise combination of one of more <see cref="SecurityFlags"/> values.</param>
		/// <param name="serverName">The common name of the remote server.</param>
		/// <param name="buffer">The input buffer received from the remote host.</param>
		/// <param name="len">The number of bytes in the input buffer.</param>
		/// <param name="ret">A reference to an integer that receives the status of the renegotiation.</param>
		/// <returns>An array of bytes that should be sent to the remote host -or- a null reference (<b>Nothing</b> in Visual Basic) if there are no bytes that should be sent to the remote host.</returns>
		public byte[] GetClientRenegotiationBytes(SecurityFlags flags, string serverName, byte[] buffer, ref int len, ref int ret) {
			int dwSSPIOutFlags = 0, dwSSPIFlags = (int)flags | SecurityConstants.ISC_RET_EXTENDED_ERROR | SecurityConstants.ISC_REQ_ALLOCATE_MEMORY | SecurityConstants.ISC_REQ_STREAM;
			SecBufferDesc InBuffer = new SecBufferDesc(2);
			SecBufferDesc OutBuffer = new SecBufferDesc(1);
			// initialize the input buffers
			InBuffer.pBuffers[0] = new SecBuffer(SecurityConstants.SECBUFFER_TOKEN, buffer, len);
			InBuffer.pBuffers[1] = new SecBuffer();
			// initialize the output buffers
			OutBuffer.pBuffers[0] = new SecBuffer(SecurityConstants.SECBUFFER_TOKEN);
			// call the SSPI function
			IntPtr input = SecBuffDescToUnmanaged(InBuffer), output = SecBuffDescToUnmanaged(OutBuffer);
			ret = SspiProvider.InitializeSecurityContext(Credential.Handle, Context, serverName, dwSSPIFlags, 0, SecurityConstants.SECURITY_NATIVE_DREP, input, 0, IntPtr.Zero, output, ref dwSSPIOutFlags, IntPtr.Zero);
			InBuffer = UnmanagedToSecBuffDesc(input);
			OutBuffer = UnmanagedToSecBuffDesc(output);
			Marshal.FreeHGlobal(input);
			Marshal.FreeHGlobal(output);
			// interpret the output data
			byte[] retval = null;
			if (!OutBuffer.pBuffers[0].ptrBuffer.Equals(IntPtr.Zero) && OutBuffer.pBuffers[0].cbBuffer > 0) {
				retval = OutBuffer.pBuffers[0].pvBuffer;
				SspiProvider.FreeContextBuffer(OutBuffer.pBuffers[0].ptrBuffer);
			}
			if (InBuffer.pBuffers[1].BufferType == SecurityConstants.SECBUFFER_EXTRA && InBuffer.pBuffers[1].cbBuffer > 0) {
				Array.Copy(InBuffer.pBuffers[1].pvBuffer, 0, buffer, 0, InBuffer.pBuffers[1].cbBuffer);
				SspiProvider.FreeContextBuffer(InBuffer.pBuffers[1].ptrBuffer);
				len = InBuffer.pBuffers[1].cbBuffer;
			} else if(ret == SecurityConstants.SEC_E_INCOMPLETE_MESSAGE) {
				len = InBuffer.pBuffers[0].cbBuffer;
				Array.Copy(InBuffer.pBuffers[0].pvBuffer, 0, buffer, 0, len);
			} else {
				len = 0;
			}
			return retval;
		}
		/// <summary>
		/// Gets the security context handle.
		/// </summary>
		/// <value>An <see cref="IntPtr"/> instance.</value>
		public IntPtr Context {
			get {
				return m_Context;
			}
		}
		/// <summary>
		/// Gets the security protocol in use.
		/// </summary>
		/// <value>One of the <see cref="SecureProtocol"/> values.</value>
		public SecureProtocol Protocol {
			get {
				return m_Protocol;
			}
		}
		/// <summary>
		/// Gets the credential in use.
		/// </summary>
		/// <value>A <see cref="Credential"/> instance.</value>
		public Credential Credential {
			get {
				return m_Credential;
			}
		}
		/// <summary>Holds the value of the Context property.</summary>
		private IntPtr m_Context;
		/// <summary>Holds the value of the Protocol property.</summary>
		private SecureProtocol m_Protocol;
		/// <summary>Holds the value of the Credential property.</summary>
		private Credential m_Credential;
	}
}
