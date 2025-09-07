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
using System.Net;
using System.Net.Sockets;
using System.Collections;
using Org.Mentalis.LegacySecurity.Certificates;

namespace Org.Mentalis.LegacySecurity.Ssl {
	/// <summary>
	/// Implements the Berkeley sockets interface and optionally encrypts/decrypts transmitted data.
	/// </summary>
	/// <remarks>Any public static (Shared in Visual Basic) members of this type are safe for multithreaded operations. Any instance members are not guaranteed to be thread safe.</remarks>
	//[CLSCompliant(true)]
	public class SecureSocket : VirtualSocket {
		/// <summary>
		/// Initializes a new instance of the SecureSocket class.
		/// </summary>
		/// <param name="addressFamily">One of the <see cref="AddressFamily"/> values.</param>
		/// <param name="socketType">One of the <see cref="SocketType"/> values.</param>
		/// <param name="protocolType">One of the <see cref="ProtocolType"/> values.</param>
		/// <exception cref="SocketException">The combination of addressFamily, socketType, and protocolType results in an invalid socket.</exception>
		/// <remarks>The SecureSocket will act as a normal Socket and will not use a secure transfer protocol.</remarks>
		public SecureSocket(AddressFamily addressFamily, SocketType socketType, ProtocolType protocolType) : this(addressFamily, socketType, protocolType, new SecurityOptions(SecureProtocol.None)) {}
		/// <summary>
		/// Initializes a new instance of the SecureSocket class.
		/// </summary>
		/// <param name="addressFamily">One of the <see cref="AddressFamily"/> values.</param>
		/// <param name="socketType">One of the <see cref="SocketType"/> values.</param>
		/// <param name="protocolType">One of the <see cref="ProtocolType"/> values.</param>
		/// <param name="options">The <see cref="SecurityOptions"/> to use.</param>
		/// <exception cref="SecurityException">An error occurs while changing the security protocol.</exception>
		public SecureSocket(AddressFamily addressFamily, SocketType socketType, ProtocolType protocolType, SecurityOptions options) : base(addressFamily, socketType, protocolType) {
			ChangeSecurityProtocol(options);
		}
		/// <summary>
		/// Initializes a new instance of the SecureSocket class.
		/// </summary>
		/// <param name="accepted">The accepted <see cref="Socket"/> instance.</param>
		/// <param name="acceptResult">An <see cref="AsyncSecureAcceptResult"/> instance.</param>
		/// <param name="options">The <see cref="SecurityOptions"/> to use.</param>
		/// <exception cref="SecurityException">An error occurs while changing the security protocol.</exception>
		internal SecureSocket(Socket accepted, AsyncSecureAcceptResult acceptResult, SecurityOptions options) : base(accepted) {
			AsyncAcceptResult = acceptResult;
			ChangeSecurityProtocol(options);
		}
		/// <summary>
		/// Establishes a connection to a remote device and optionally negotiates a secure transport protocol.
		/// </summary>
		/// <param name="remoteEP">An <see cref="EndPoint"/> that represents the remote device.</param>
		/// <exception cref="ArgumentNullException">The remoteEP parameter is a null reference (<b>Nothing</b> in Visual Basic).</exception>
		/// <exception cref="SocketException">An operating system error occurs while accessing the <see cref="SecureSocket"/>.</exception>
		/// <exception cref="ObjectDisposedException">The SecureSocket has been closed.</exception>
		/// <exception cref="SecurityException">The security negotiation failed.</exception>
		public override void Connect(EndPoint remoteEP) {
			if (SecureProtocol == SecureProtocol.None) {
				base.Connect(remoteEP);
			} else {
				this.EndConnect(this.BeginConnect(remoteEP, null, null));
			}
		}
		/// <summary>
		/// Creates a new <see cref="SecureSocket"/> to handle an incoming connection request.
		/// </summary>
		/// <returns>A SecureSocket to handle an incoming connection request.</returns>
		/// <remarks>The returned <see cref="VirtualSocket"/> can be cast to a SecureSocket if necessary.</remarks>
		/// <exception cref="SocketException">An operating system error occurs while accessing the SecureSocket.</exception>
		/// <exception cref="ObjectDisposedException">The SecureSocket has been closed.</exception>
		/// <exception cref="SecurityException">Unable to create the credentials.</exception>
		public override VirtualSocket Accept() {
			return EndAccept(BeginAccept(null, null));
		}
		/// <summary>
		/// Begins an asynchronous request to create a new <see cref="SecureSocket"/> to accept an incoming connection request.
		/// </summary>
		/// <param name="callback">The <see cref="AsyncCallback"/> delegate.</param>
		/// <param name="state">An object containing state information for this request.</param>
		/// <returns>An <see cref="IAsyncResult"/> that references the asynchronous SecureSocket creation.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="callback"/> is a null reference (<b>Nothing</b> in Visual Basic).</exception>
		/// <exception cref="SocketException">An operating system error occurs while creating the SecureSocket.</exception>
		/// <exception cref="ObjectDisposedException">The SecureSocket has been closed.</exception>
		public override IAsyncResult BeginAccept(AsyncCallback callback, object state) {
			if (AsyncAcceptResult != null)
				throw new SocketException();
			AsyncAcceptResult = new AsyncSecureAcceptResult(this, callback, state);
			base.BeginAccept(new AsyncCallback(OnAccept), null);
			return AsyncAcceptResult;
		}
		private void OnAccept(IAsyncResult ar) {
			try {
				AsyncAcceptResult.AcceptedSocket = new SecureSocket(base.InternalEndAccept(ar), AsyncAcceptResult, m_Options);
			} catch (Exception e) {
				AsyncAcceptResult.AsyncException = e;
			}
			AsyncAcceptResult.Notify();
		}
		/// <summary>
		/// Ends an asynchronous request to create a new <see cref="SecureSocket"/> to accept an incoming connection request.
		/// </summary>
		/// <param name="asyncResult">Stores state information for this asynchronous operation as well as any user defined data.</param>
		/// <returns>A SecureSocket to handle the incoming connection.</returns>
		/// <remarks>The returned <see cref="VirtualSocket"/> can be cast to a SecureSocket if necessary.</remarks>
		/// <exception cref="ArgumentNullException"><paramref name="asyncResult"/> is a null reference (<b>Nothing</b> in Visual Basic).</exception>
		/// <exception cref="ArgumentException"><paramref name="asyncResult"/> was not created by a call to <see cref="BeginAccept"/>.</exception>
		/// <exception cref="SocketException">An operating system error occurs while accessing the SecureSocket.</exception>
		/// <exception cref="ObjectDisposedException">The SecureSocket has been closed.</exception>
		/// <exception cref="SecurityException">Unable to create the credentials -or- client authentication error.</exception>
		public override VirtualSocket EndAccept(IAsyncResult asyncResult) {
			// Make sure everything is in order
			if (asyncResult == null)
				throw new ArgumentNullException();
			if (AsyncAcceptResult == null)
				throw new InvalidOperationException();
			if (AsyncAcceptResult != asyncResult)
				throw new ArgumentException();
			AsyncSecureAcceptResult ar = AsyncAcceptResult;
			// Process the (secure) EndAccept
			// block if the operation hasn't ended yet
			if (!ar.IsCompleted)
				ar.AsyncWaitHandle.WaitOne();
			AsyncAcceptResult = null;
			if (ar.AsyncException != null)
				throw ar.AsyncException;
			return ar.AcceptedSocket;
		}
		/// <summary>
		/// Begins an asynchronous request for a connection to a network device.
		/// </summary>
		/// <param name="remoteEP">An <see cref="EndPoint"/> that represents the remote device.</param>
		/// <param name="callback">The <see cref="AsyncCallback"/> delegate.</param>
		/// <param name="state">An object that contains state information for this request.</param>
		/// <returns>An <see cref="IAsyncResult"/> that references the asynchronous connection.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="remoteEP"/> is a null reference (<b>Nothing</b> in Visual Basic).</exception>
		/// <exception cref="SocketException">An operating system error occurs while creating the SecureSocket.</exception>
		/// <exception cref="ObjectDisposedException">The SecureSocket has been closed.</exception>
		public override IAsyncResult BeginConnect(EndPoint remoteEP, AsyncCallback callback, object state) {
			if (SecureProtocol == SecureProtocol.None)
				return base.BeginConnect(remoteEP, callback, state);
			// secure BeginConnect
			if (remoteEP == null)
				throw new ArgumentNullException();
			if (AsyncConnectResult != null)
				throw new SocketException(); // BeginConnect already called
			if (HasBeenConnected)
				throw new ObjectDisposedException(this.GetType().FullName);
			AsyncConnectResult = new AsyncSecureConnectResult(this, callback, state);
			base.BeginConnect(remoteEP, new AsyncCallback(OnSecureConnect), null);
			return AsyncConnectResult;
		}
		private void OnSecureConnect(IAsyncResult ar) {
			try {
				if (ar != null)
					base.EndConnect(ar);
				DoHandshake(HandshakeType.ClientHello);
			} catch (Exception e) {
				OnHandshakeComplete(e);
			}
		}
		private void DoHandshake(HandshakeType type) {
			IsInitializing = true;
			try {
				byte[] toSend = null;
				if (type == HandshakeType.ClientHello) {
					toSend = SslProvider.GetClientHello(CommonName, this.SecurityFlags, this.VerifyType);
					if (toSend == null)
						throw new SecurityException("Unable to get the HELLO bytes.");
				} else if (type == HandshakeType.ClientRenegotiate || type == HandshakeType.ServerRenegotiate) {
					toSend = SslProvider.GetActionBytes(this.SecurityFlags, ConnectionActions.Renegotiate, type == HandshakeType.ClientRenegotiate, CommonName);
					if (toSend == null) {
						throw new SecurityException("Unable to get the renegotiate bytes.");
					}
				}
				if (toSend == null) {
					OnHelloSent(null);
				} else {
					BeginRawSend(toSend, new AsyncCallback(OnHelloSent));
				}
			} catch (Exception e) {
				OnHandshakeComplete(e);
			}
		}
		private void OnHelloSent(IAsyncResult ar) {
			try {
				if (ar != null)
					this.EndRawSend(ar);
				this.BeginRawReceive();
			} catch (Exception e) {
				OnHandshakeComplete(e);
				return;
			}
		}
		/// <summary>
		/// Begins to asynchronously receive data from a connected SecureSocket.
		/// </summary>
		/// <param name="buffer">The storage location for the received data.</param>
		/// <param name="offset">The zero-based position in the buffer parameter at which to store the received data.</param>
		/// <param name="size">The number of bytes to receive.</param>
		/// <param name="socketFlags">A bitwise combination of the <see cref="SocketFlags"/> values.</param>
		/// <param name="callback">The <see cref="AsyncCallback"/> delegate.</param>
		/// <param name="state">An object containing state information for this request.</param>
		/// <returns>An <see cref="IAsyncResult"/> that references the asynchronous read.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="buffer"/> is a null reference (<b>Nothing</b> in Visual Basic).</exception>
		/// <exception cref="SocketException">An operating system error occurs while accessing the SecureSocket.</exception>
		/// <exception cref="ObjectDisposedException">SecureSocket has been closed.</exception>
		/// <exception cref="ArgumentOutOfRangeException">The offset parameter is outside the bounds of buffer or size is either smaller or larger than the buffer size.</exception>
		public override IAsyncResult BeginReceive(byte[] buffer, int offset, int size, SocketFlags socketFlags, AsyncCallback callback, object state) {
			if (SecureProtocol == SecureProtocol.None)
				return base.BeginReceive(buffer, offset, size, socketFlags, callback, state);
			if (!Connected && HasBeenConnected)
				throw new ObjectDisposedException(this.GetType().FullName);
			if (!Connected)
				throw new SocketException();
			if (AsyncReceiveResult != null)
				throw new SocketException(); // BeginConnect already called
			if (buffer == null)
				throw new ArgumentNullException();
			if (offset < 0 || offset >= buffer.Length || size > buffer.Length - offset)
				throw new ArgumentOutOfRangeException();
			AsyncReceiveResult = new AsyncSecureReceiveResult(this, callback, state);
			AsyncReceiveResult.Buffer = buffer;
			AsyncReceiveResult.Offset = offset;
			AsyncReceiveResult.Size = size;
			AsyncSecureReceiveResult arr = AsyncReceiveResult;
			if (DecryptedBuffer.Length == 0) {
				if (!IsInitializing) {
					if (ReceivedBytes != 0) {
						OnRawReceive(null);
					} else {
						BeginRawReceive();
					}
				}
			} else {
				if (DecryptedBuffer.Length <= size) {
					Array.Copy(DecryptedBuffer, 0, buffer, offset, DecryptedBuffer.Length);
					AsyncReceiveResult.SizeTransferred = DecryptedBuffer.Length;
					DecryptedBuffer = new byte[0];
				} else {
					byte[] ret = DecryptedBuffer;
					Array.Copy(ret, 0, buffer, offset, size);
					DecryptedBuffer = new byte[ret.Length - size];
					Array.Copy(ret, size, DecryptedBuffer, 0, DecryptedBuffer.Length);
					AsyncReceiveResult.SizeTransferred = size;
				}
				OnEndReceive(null);
			}
			return arr;  // AsyncReceiveResult may already be null by now
		}
		private void BeginRawReceive() {
			base.BeginReceive(ReceiveBuffer, ReceivedBytes, ReceiveBuffer.Length - ReceivedBytes, SocketFlags.None, new AsyncCallback(OnRawReceive), null);
		}
		private void OnRawReceive(IAsyncResult ar) {
			try {
				if (ar != null) {
					ReceivedBytes += base.EndReceive(ar);
				}
			} catch (Exception e) {
				if (IsInitializing)
					OnHandshakeComplete(e);
				else
					OnReceiveComplete(e);
				return;
			}
			byte[] outbuf = null;
			int status = 0;
			int ret;
			if (IsInitializing && (this.AsyncRenegotiateResult == null || this.AsyncRenegotiateResult.DoHandshake)) {
				if (this.CredentialUse == CredentialUse.Client) { // Client
					ret = SslProvider.GetClientHandshakeBytes(ref ReceiveBuffer, ref ReceivedBytes, ref outbuf, ref status, this.SecurityFlags, this.VerifyType);
				} else { // Server
					if (AsyncAcceptResult != null) { // new incoming connection
						ret = SslProvider.GetServerHandshakeBytes(ref ReceiveBuffer, ref ReceivedBytes, ref outbuf, ref status, AsyncAcceptResult.InitializeServerContext, this.SecurityFlags);
						AsyncAcceptResult.InitializeServerContext = false;
					} else if (AsyncConnectResult != null) { // (re)negotiation of existing connection
						ret = SslProvider.GetServerHandshakeBytes(ref ReceiveBuffer, ref ReceivedBytes, ref outbuf, ref status, AsyncConnectResult.InitializeServerContext, this.SecurityFlags);
						AsyncConnectResult.InitializeServerContext = false;
					} else {
						ret = SslProvider.GetServerHandshakeBytes(ref ReceiveBuffer, ref ReceivedBytes, ref outbuf, ref status, false, this.SecurityFlags);
					}
				}
				if (ret != 0) {
					OnHandshakeComplete(new SecurityException("Unable to process the server input."));
					return;
				}
				if (status == SecurityConstants.SEC_I_INCOMPLETE_CREDENTIALS) {
					OnHandshakeComplete(new SecurityException("The supplied credentials were rejected."));
					return;
				}
				try {
					if (outbuf != null && outbuf.Length != 0 && (status == SecurityConstants.SEC_E_OK || status == SecurityConstants.SEC_I_CONTINUE_NEEDED)) {
						if (status == SecurityConstants.SEC_E_OK) {
							BeginRawSend(outbuf, new AsyncCallback(OnHandshakeSent));
						} else {
							BeginRawSend(outbuf, new AsyncCallback(OnOutSent));
						}
					} else {
						if (status == SecurityConstants.SEC_E_OK) {
							OnHandshakeSent(null);
						} else if (status == SecurityConstants.SEC_E_INCOMPLETE_MESSAGE || status == SecurityConstants.SEC_I_CONTINUE_NEEDED){
							BeginRawReceive();
						} else {
							OnHandshakeComplete(new SecurityException("Illegal handshake message."));
						}
					}
				} catch (Exception e) {
					OnHandshakeComplete(e);
				}
			} else {
				if (ReceivedBytes == 0) { // other side closed connection gracefully
					OnEndReceive(null);
				} else {
					byte[] decrypted = SslProvider.DecryptMessage(ReceiveBuffer, ref ReceivedBytes, ref status);
					if (status == SecurityConstants.SEC_I_CONTEXT_EXPIRED) {
						DecryptedBuffer = new byte[0];
						byte[] toSend = SslProvider.EncryptMessage(null);
						if (toSend != null) {
							base.BeginSend(toSend, 0, toSend.Length, SocketFlags.None, new AsyncCallback(OnEndReceiveSent), null);
						} else {
							OnEndReceive(null);
						}
					} else if (status == SecurityConstants.SEC_I_RENEGOTIATE) {
						// process bytes as usual
						if (decrypted != null)
							AppendBuffer(ref DecryptedBuffer, decrypted, 0, decrypted.Length);
						if (this.AsyncRenegotiateResult != null)
							this.AsyncRenegotiateResult.DoHandshake = true;
						IsInitializing = true;
						int acs = 0;
						byte[] toSend;
						if (this.CredentialUse == CredentialUse.Client)
							toSend = SslProvider.GetClientRenegotiationBytes(SecurityFlags, this.CommonName, ReceiveBuffer, ref ReceivedBytes, ref acs);
						else
							toSend = SslProvider.GetServerRenegotiationBytes(SecurityFlags, ReceiveBuffer, ref ReceivedBytes, ref acs);
						if (acs == SecurityConstants.SEC_E_INCOMPLETE_MESSAGE) {
							BeginRawReceive();
						} else if (toSend != null) {
							if (acs == SecurityConstants.SEC_I_CONTINUE_NEEDED)
								BeginRawSend(toSend, new AsyncCallback(this.OnHelloSent)); // send data and wait for remote host reply
							else
								BeginRawSend(toSend, new AsyncCallback(this.OnHandshakeSent));
						}
						// The client calls InitializeSecurityContext, with no input buffer. This
						// builds the client_hello message, which is sent to the server.
						// then send BeginRawSend(OnHelloSent);
						// don't forget Available support! [should not block!!]
					} else if(status == SecurityConstants.SEC_E_OK) {
						if (decrypted == null) { // bug in Win2K; returns SEC_E_OK and zero length buffer when other side has shutdown the connection
							AsyncReceiveResult.SizeTransferred = 0;
							DecryptedBuffer = new byte[0];
						} else {
							if (AsyncReceiveResult != null) {
								if (AsyncReceiveResult.Size >= decrypted.Length) {
									Array.Copy(decrypted, 0, AsyncReceiveResult.Buffer, AsyncReceiveResult.Offset, decrypted.Length);
									AsyncReceiveResult.SizeTransferred = decrypted.Length;
								} else {
									AppendBuffer(ref DecryptedBuffer, decrypted, AsyncReceiveResult.Size, decrypted.Length - AsyncReceiveResult.Size);
									Array.Copy(decrypted, 0, AsyncReceiveResult.Buffer, AsyncReceiveResult.Offset, AsyncReceiveResult.Size);
									AsyncReceiveResult.SizeTransferred = AsyncReceiveResult.Size;
								}
							} else if (AsyncAvailableResult != null) { // result of 'Available' request
								AppendBuffer(ref DecryptedBuffer, decrypted, 0, decrypted.Length);
							} else { // no receive or available operation running; probably start of renegotiation
								AppendBuffer(ref DecryptedBuffer, decrypted, 0, decrypted.Length);
								if (ReceivedBytes > 0) { // unprocessed extra bytes; may contain renegotiation info
									OnRawReceive(null);
									return;
								}
							}
						}
						OnEndReceive(null);
					} else if (status == SecurityConstants.SEC_E_INCOMPLETE_MESSAGE) {
						if (AsyncAvailableResult == null)
							BeginRawReceive();
						else
							AsyncAvailableResult.Notify();
					} else {
						OnEndReceive(new SecurityException("Unable to decrypt message."));
					}
				}
			}
		}
		// append one buffer to another
		// pre: parameters must be valid
		private void AppendBuffer(ref byte[] resultBuffer, byte[] toAppend, int start, int length) {
			byte[] copy = new byte[resultBuffer.Length + length];
			Array.Copy(resultBuffer, 0, copy, 0, resultBuffer.Length);
			Array.Copy(toAppend, start, copy, resultBuffer.Length, length);
			resultBuffer = copy;
		}
		private void OnEndReceiveSent(IAsyncResult ar) {
			try {
				base.EndSend(ar);
				OnEndReceive(null);
			} catch (Exception e) {
				OnEndReceive(e);
			}
		}
		private void OnEndReceive(Exception e) {
			if (AsyncAvailableResult != null) {
				AsyncAvailableResult.AsyncException = e;
				AsyncAvailableResult.Notify();
			} else if (AsyncReceiveResult != null) {
				AsyncReceiveResult.AsyncException = e;
				AsyncReceiveResult.Notify();
			} else {
				BeginRawReceive(); // wait for more data
			}
		}
		private void OnOutSent(IAsyncResult ar) {
			try {
				EndRawSend(ar);
				BeginRawReceive();
			} catch (Exception e) {
				OnHandshakeComplete(e);
			}
		}
		private void OnHandshakeSent(IAsyncResult ar) {
			try {
				if (ar != null) {
					EndRawSend(ar);
				}
				OnHandshakeComplete(null);
			} catch (Exception e) {
				OnHandshakeComplete(e);
			}
		}
		private Certificate GetServerCert() {
			IntPtr cert = IntPtr.Zero;
			if (SspiProvider.QueryContextAttributesCertificate(SslProvider.Context, SecurityConstants.SECPKG_ATTR_REMOTE_CERT_CONTEXT, ref cert) != 0)
                return null;
			return new Certificate(cert, false);
		}
		private void OnHandshakeComplete(Exception e) {
			if (e == null) {
				if (SecureProtocol != SecureProtocol.None) {
					m_RemoteCertificate = GetServerCert();
					// call user to verify server certificate
					if (VerifyType == CredentialVerification.Manual && Verifier != null) {
						VerifyEventArgs vea = new VerifyEventArgs();
						Verifier(this, m_RemoteCertificate, vea);
						if (vea.Valid == false) {
							this.Close();
							e = new CertificateException("User cancelled secure connection.");
						}
					}
				}
			} else {
				this.Close();
			}
			HasBeenConnected = true;
			if (AsyncConnectResult != null) { // may be null when accepting incoming socket
				AsyncConnectResult.AsyncException = e;
				AsyncConnectResult.Notify();
			}
			// resume receive, if necessary
			if (AsyncReceiveResult != null) {
				if (e != null) 
				{
					OnEndReceive(e);
				} else if (ReceivedBytes != 0) {
					OnRawReceive(null);
				} else {
					BeginRawReceive();
				}
			}
			IsInitializing = false;
/*			if (AsyncAcceptResult != null && e != null) {
				AsyncAcceptResult.AsyncException = e;
				AsyncAcceptResult.Notify();
			}*/ // AsyncAcceptResult has already been called!
			if (AsyncRenegotiateResult != null) {
				AsyncRenegotiateResult.DoHandshake = false;
				AsyncRenegotiateResult.AsyncException = e;
				AsyncRenegotiateResult.Notify();
			}
			AsyncAcceptResult = null;
			if (e == null) {
				// start/resume encrypting
				lock(ToEncryptList) {
					if (ToEncryptList.Count > 0) {
						while(ToEncryptList.Count > 0) {
							OnEncryptData(0);
						}
					}
				}
			}
		}
		private void OnReceiveComplete(Exception e) {
			AsyncReceiveResult.AsyncException = e;
			AsyncReceiveResult.Notify();
		}
		/// <summary>
		/// Ends a pending asynchronous connection request.
		/// </summary>
		/// <param name="asyncResult">The result of the asynchronous operation.</param>
		/// <exception cref="ArgumentNullException"><paramref name="asyncResult"/> is a null reference (<b>Nothing</b> in Visual Basic).</exception>
		/// <exception cref="ArgumentException"><paramref name="asyncResult"/> was not returned by a call to the <see cref="BeginConnect"/> method.</exception>
		/// <exception cref="InvalidOperationException"><see cref="EndConnect"/> was previously called for the asynchronous connection.</exception>
		/// <exception cref="SocketException">An operating system error occurs while accessing the SecureSocket.</exception>
		/// <exception cref="ObjectDisposedException">The SecureSocket has been closed.</exception>
		/// <exception cref="SecurityException">An error occurred while negotiating the security protocol.</exception>
		public override void EndConnect(IAsyncResult asyncResult) {
			if (SecureProtocol == SecureProtocol.None) {
				base.EndConnect(asyncResult);
				return;
			}
			// Make sure everything is in order
			if (asyncResult == null)
				throw new ArgumentNullException();
			if (AsyncConnectResult == null)
				throw new InvalidOperationException();
			if (asyncResult != AsyncConnectResult)
				throw new ArgumentException();
			// Process the (secure) EndConnect
			// block if the operation hasn't ended yet
			AsyncSecureConnectResult ar = AsyncConnectResult;
			if (!ar.IsCompleted)
				ar.AsyncWaitHandle.WaitOne();
			AsyncConnectResult = null;
			if (ar.AsyncException != null)
				throw ar.AsyncException;
		}
		/// <summary>
		/// Ends a pending asynchronous read.
		/// </summary>
		/// <param name="asyncResult">Stores state information for this asynchronous operation as well as any user defined data.</param>
		/// <returns>The number of bytes received.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="asyncResult"/> is a null reference (<b>Nothing</b> in Visual Basic).</exception>
		/// <exception cref="ArgumentException"><paramref name="asyncResult"/> was not returned by a call to the <see cref="BeginReceive"/> method.</exception>
		/// <exception cref="InvalidOperationException"><see cref="EndReceive"/> was previously called for the asynchronous read.</exception>
		/// <exception cref="SocketException">An operating system error occurs while accessing the socket.</exception>
		/// <exception cref="ObjectDisposedException">The <see cref="SecureSocket"/> has been closed.</exception>
		/// <exception cref="SecurityException">An error occurred while decrypting data received from the remote host.</exception>
		public override int EndReceive(IAsyncResult asyncResult) {
			if (SecureProtocol == SecureProtocol.None)
				return base.EndReceive(asyncResult);
			// Make sure everything is in order
			if (asyncResult == null)
				throw new ArgumentNullException();
			if (AsyncReceiveResult == null)
				throw new InvalidOperationException();
			if (asyncResult != AsyncReceiveResult)
				throw new ArgumentException();
			// Process the (secure) EndReceive
			// block if the operation hasn't ended yet
			AsyncSecureReceiveResult ar = AsyncReceiveResult;
			if (!ar.IsCompleted)
				ar.AsyncWaitHandle.WaitOne();
			AsyncReceiveResult = null;
			if (ar.AsyncException != null)
				throw ar.AsyncException;
			return ar.SizeTransferred;
		}
		// precondition: buffer != null && callback != null and there should be no active raw send
		// postcondition: it will send _all_ bytes in a buffer
		private IAsyncResult BeginRawSend(byte[] buffer, AsyncCallback callback) {
			AsyncSendResult = new AsyncSecureSendResult(this, callback, null);
			AsyncSendResult.Buffer = buffer;
			AsyncSendResult.AsyncException = null;
			AsyncSendResult.InternalStateObject = (int)0;
			IsSending = true;
			try {
				base.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(OnRawSent), null);
			} catch (Exception e) {
				IsSending = false;
				throw e;
			}
			return AsyncSendResult;
		}
		// precondition: async != null and there should be no active raw send
		private IAsyncResult BeginRawSend(AsyncSecureSendResult async) {
			AsyncSendResult = async;
			IsSending = true;
			try {
				base.BeginSend(async.Buffer, 0, async.Buffer.Length, SocketFlags.None, new AsyncCallback(OnRawSent), null);
			} catch (Exception e) {
				IsSending = false;
				throw e;
			}
			return AsyncSendResult;
		}
		private void OnRawSent(IAsyncResult ar) {
			int sent = 0;
			try {
				sent = base.EndSend(ar);
			} catch (Exception e) {
				OnSendComplete(e);
				return;
			}
			AsyncSendResult.InternalStateObject = (int)AsyncSendResult.InternalStateObject + sent;
			if ((int)AsyncSendResult.InternalStateObject < AsyncSendResult.Buffer.Length) {
				try {
					base.BeginSend(AsyncSendResult.Buffer, (int)AsyncSendResult.InternalStateObject, AsyncSendResult.Buffer.Length - (int)AsyncSendResult.InternalStateObject, SocketFlags.None, new AsyncCallback(OnRawSent), null);
				} catch (Exception e) {
					OnSendComplete(e);
				}
			} else {
				OnSendComplete(null);
			}
		}
		private void OnSendComplete(Exception e) {
			lock (ToSendList) {
				if (ToSendList.Contains(AsyncSendResult)) {
					lock (SentList) {
						SentList.Add(AsyncSendResult);
					}
					ToSendList.Remove(AsyncSendResult);
				}
			}
			AsyncSendResult.AsyncException = e;
			AsyncSendResult.Notify();
			lock (ToSendList) {
				if (ToSendList.Count > 0) {
					bool sending = false;
					while(!sending && ToSendList.Count > 0) {
						AsyncSecureSendResult asr = (AsyncSecureSendResult)ToSendList[0];
						try {
							BeginRawSend(asr);
							sending = true;
						} catch (Exception ex) {
							lock (asr) {
								SentList.Add(AsyncSendResult);
							}
							ToSendList.Remove(asr);
							asr.AsyncException = ex;
							asr.Notify();
						}
					}
					if (!sending)
						IsSending = false;
				} else {
					IsSending = false;
				}
			}
		}
		private int EndRawSend(IAsyncResult ar) {
			if (ar != AsyncSendResult)
				throw new ArgumentException();
			AsyncSendResult = null;
			if (((AsyncSecureSendResult)ar).AsyncException != null)
				throw ((AsyncSecureSendResult)ar).AsyncException;
			else
				return (int)((AsyncSecureSendResult)ar).InternalStateObject;
		}
		/// <summary>
		/// Receives data from a connected <see cref="SecureSocket"/> into a specific location of the receive buffer.
		/// </summary>
		/// <param name="buffer">The storage location for the received data.</param>
		/// <returns>The number of bytes received.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="buffer"/> is a null reference (<b>Nothing</b> in Visual Basic).</exception>
		/// <exception cref="SocketException">An operating system error occurs while accessing the SecureSocket.</exception>
		/// <exception cref="ObjectDisposedException">The SecureSocket has been closed.</exception>
		/// <exception cref="SecurityException">An error occurred while decrypting the received data.</exception>
		public override int Receive(byte[] buffer) {
			if (buffer == null)
				throw new ArgumentNullException();
			return this.Receive(buffer, 0, buffer.Length, SocketFlags.None);
		}
		/// <summary>
		/// Receives data from a connected <see cref="SecureSocket"/> into a specific location of the receive buffer.
		/// </summary>
		/// <param name="buffer">The storage location for the received data.</param>
		/// <param name="socketFlags">A bitwise combination of the <see cref="SocketFlags"/> values.</param>
		/// <returns>The number of bytes received.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="buffer"/> is a null reference (<b>Nothing</b> in Visual Basic).</exception>
		/// <exception cref="SocketException">An operating system error occurs while accessing the SecureSocket.</exception>
		/// <exception cref="ObjectDisposedException">The SecureSocket has been closed.</exception>
		/// <exception cref="SecurityException">An error occurred while decrypting the received data.</exception>
		public override int Receive(byte[] buffer, SocketFlags socketFlags) {
			if (buffer == null)
				throw new ArgumentNullException();
			return this.Receive(buffer, 0, buffer.Length, socketFlags);
		}
		/// <summary>
		/// Receives data from a connected <see cref="SecureSocket"/> into a specific location of the receive buffer.
		/// </summary>
		/// <param name="buffer">The storage location for the received data.</param>
		/// <param name="size">The number of bytes to receive.</param>
		/// <param name="socketFlags">A bitwise combination of the <see cref="SecureSocket"/> values.</param>
		/// <returns>The number of bytes received.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="buffer"/> is a null reference (<b>Nothing</b> in Visual Basic).</exception>
		/// <exception cref="ArgumentOutOfRangeException">The size exceeds the size of buffer.</exception>
		/// <exception cref="SocketException">An operating system error occurs while accessing the SecureSocket.</exception>
		/// <exception cref="ObjectDisposedException">The SecureSocket has been closed.</exception>
		/// <exception cref="SecurityException">An error occurred while decrypting the received data.</exception>
		public override int Receive(byte[] buffer, int size, SocketFlags socketFlags) {
			if (buffer == null)
				throw new ArgumentNullException();
			return this.Receive(buffer, 0, size, socketFlags);
		}
		/// <summary>
		/// Receives data from a connected <see cref="SecureSocket"/> into a specific location of the receive buffer.
		/// </summary>
		/// <param name="buffer">The storage location for the received data.</param>
		/// <param name="offset">The location in buffer to store the received data.</param>
		/// <param name="size">The number of bytes to receive.</param>
		/// <param name="socketFlags">A bitwise combination of the <see cref="SocketFlags"/> values.</param>
		/// <returns>The number of bytes received.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="buffer"/> is a null reference (<b>Nothing</b> in Visual Basic).</exception>
		/// <exception cref="ArgumentOutOfRangeException">The size exceeds the size of buffer.</exception>
		/// <exception cref="SocketException">An operating system error occurs while accessing the SecureSocket.</exception>
		/// <exception cref="ObjectDisposedException">The SecureSocket has been closed.</exception>
		/// <exception cref="SecurityException">An error occurred while decrypting the received data.</exception>
		public override int Receive(byte[] buffer, int offset, int size, SocketFlags socketFlags) {
			if (SecureProtocol == SecureProtocol.None) {
				return base.Receive(buffer, offset, size, socketFlags);
			} else {
				return this.EndReceive(this.BeginReceive(buffer, offset, size, socketFlags, null, null));
			}
		}
		/// <summary>
		/// Changes the security protocol.
		/// </summary>
		/// <param name="options">The new <see cref="SecurityOptions"/> parameters.</param>
		/// <exception cref="SecurityException">An error occurs while changing the security protocol.</exception>
		/// <remarks>
		/// Programs should only call this method if there is no active <see cref="Connect"/>, <see cref="Accept"/> or <see cref="Send"/>!
		/// </remarks>
		public void ChangeSecurityProtocol(SecurityOptions options) {
			if (IsInitializing || IsSending)
				throw new SecurityException("Invalid socket state!");
			if (base.ProtocolType != ProtocolType.Tcp && options.secureProtocol != SecureProtocol.None)
				throw new SecurityException("SSL and TLS require underlying TCP connections!");
			m_Options = options;
			if (options.secureProtocol != SecureProtocol.None) {
				m_SslProvider = new SslProvider(options.secureProtocol, new Credential(options));
				if (base.Connected)
					DoHandshake(options.use == CredentialUse.Client ? HandshakeType.ClientHello : HandshakeType.ServerHello);
			}
		}
		/// <summary>
		/// Shuts down the secure connection.
		/// </summary>
		/// <exception cref="ObjectDisposedException">SecureSocket has been closed.</exception>
		/// <exception cref="SocketException">An operating system error occurs while accessing the SecureSocket.</exception>
		/// <exception cref="SecurityException">An error occurs while shutting the secure connection down.</exception>
		public void SecureShutdown() {
			this.EndSecureShutdown(this.BeginSecureShutdown(null, null));
		}
		/// <summary>
		/// Begins an asynchronous request to shut the connection down.
		/// </summary>
		/// <param name="callback">The <see cref="AsyncCallback"/> delegate.</param>
		/// <param name="state">An object containing state information for this request.</param>
		/// <returns>An <see cref="IAsyncResult"/> that references the asynchronous shutdown.</returns>
		/// <exception cref="InvalidOperationException"><see cref="BeginSecureShutdown"/> has already been called.</exception>
		public IAsyncResult BeginSecureShutdown(AsyncCallback callback, object state) {
			if (AsyncShutdownResult != null)
				throw new InvalidOperationException();
			//TODO: fix Secure Shutdown.. [go in shutdown loop]
			AsyncSecureShutdownResult ar = new AsyncSecureShutdownResult(this, callback, state);
			AsyncShutdownResult = ar;
			if (SecureProtocol == SecureProtocol.None) {
				OnSecureShutdown(null);
			} else {
				byte[] sdbytes = SslProvider.GetActionBytes(SecurityFlags, ConnectionActions.Shutdown, this.CredentialUse == CredentialUse.Client, this.CommonName);
				if (sdbytes != null) {
					try {
						base.BeginSend(sdbytes, 0, sdbytes.Length, SocketFlags.None, new AsyncCallback(OnSecureShutdown), null);
					} catch {
						OnSecureShutdown(null); // connection already shut down
					}
				} else {
					OnSecureShutdown(null);
				}
			}
			return ar;
		}
		private void OnSecureShutdown(IAsyncResult ar) {
			try {
				if (ar != null) {
					base.EndSend(ar);
				}
			} catch (Exception e) {
				AsyncShutdownResult.AsyncException = e;
			}
			AsyncShutdownResult.Notify();
		}
		/// <summary>
		/// Ends an asynchronous request to shut the connection down.
		/// </summary>
		/// <param name="asyncResult">An <see cref="IAsyncResult"/> that references the asynchronous shutdown.</param>
		/// <exception cref="ArgumentNullException"><paramref name="asyncResult"/> is a null reference (<b>Nothing</b> in Visual Basic).</exception>
		/// <exception cref="InvalidOperationException"><see cref="BeginSecureShutdown"/> has not been called first.</exception>
		/// <exception cref="ArgumentException"><paramref name="asyncResult"/> has not been returned by a call to <see cref="BeginSecureShutdown"/>.</exception>
		public void EndSecureShutdown(IAsyncResult asyncResult) {
			if (asyncResult == null)
				throw new ArgumentNullException();
			if (AsyncShutdownResult == null)
				throw new InvalidOperationException();
			if (asyncResult != AsyncShutdownResult)
				throw new ArgumentException();
			// Process the EndSecureShutdown
			// block if the operation hasn't ended yet
			AsyncSecureShutdownResult ar = AsyncShutdownResult;
			if (!ar.IsCompleted)
				ar.AsyncWaitHandle.WaitOne();
			AsyncShutdownResult = null;
//			if (ar.AsyncException != null)
//				throw ar.AsyncException; // ignore errors; not important
		}
		/// <summary>
		/// Sends data to a connected <see cref="SecureSocket"/>, starting at the indicated location in the data.
		/// </summary>
		/// <param name="buffer">The data to be sent.</param>
		/// <returns>The number of bytes sent to the SecureSocket.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="buffer"/> is a null reference (<b>Nothing</b> in Visual Basic).</exception>
		/// <exception cref="SocketException">An operating system error occurs while accessing the SecureSocket.</exception>
		/// <exception cref="ObjectDisposedException">The SecureSocket has been closed.</exception>
		/// <exception cref="SecurityException">Unable to encrypt the data.</exception>
		public override int Send(byte[] buffer) {
			if (buffer == null) 
				throw new ArgumentNullException();
			return this.Send(buffer, 0, buffer.Length, SocketFlags.None);
		}
		/// <summary>
		/// Sends data to a connected <see cref="SecureSocket"/>, starting at the indicated location in the data.
		/// </summary>
		/// <param name="buffer">The data to be sent.</param>
		/// <param name="socketFlags">A bitwise combination of the <see cref="SocketFlags"/> values.</param>
		/// <returns>The number of bytes sent to the SecureSocket.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="buffer"/> is a null reference (<b>Nothing</b> in Visual Basic).</exception>
		/// <exception cref="SocketException">An operating system error occurs while accessing the SecureSocket.</exception>
		/// <exception cref="ObjectDisposedException">The SecureSocket has been closed.</exception>
		/// <exception cref="SecurityException">Unable to encrypt the data.</exception>
		public override int Send(byte[] buffer, SocketFlags socketFlags) {
			if (buffer == null)
				throw new ArgumentNullException();
			return this.Send(buffer, 0, buffer.Length, socketFlags);
		}
		/// <summary>
		/// Sends data to a connected <see cref="SecureSocket"/>, starting at the indicated location in the data.
		/// </summary>
		/// <param name="buffer">The data to be sent.</param>
		/// <param name="size">The number of bytes to send.</param>
		/// <param name="socketFlags">A bitwise combination of the <see cref="SocketFlags"/> values.</param>
		/// <returns>The number of bytes sent to the SecureSocket.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="buffer"/> is a null reference (<b>Nothing</b> in Visual Basic).</exception>
		/// <exception cref="SocketException">An operating system error occurs while accessing the SecureSocket.</exception>
		/// <exception cref="ArgumentOutOfRangeException">The size parameter exceeds the size of buffer.</exception>
		/// <exception cref="ObjectDisposedException">The SecureSocket has been closed.</exception>
		/// <exception cref="SecurityException">Unable to encrypt the data.</exception>
		public override int Send(byte[] buffer, int size, SocketFlags socketFlags) {
			if (buffer == null)
				throw new ArgumentNullException();
			return this.Send(buffer, 0, size, socketFlags);
		}
		/// <summary>
		/// Sends data to a connected <see cref="SecureSocket"/>, starting at the indicated location in the data.
		/// </summary>
		/// <param name="buffer">The data to be sent.</param>
		/// <param name="offset">The position in the data buffer to begin sending data.</param>
		/// <param name="size">The number of bytes to send.</param>
		/// <param name="socketFlags">A bitwise combination of the <see cref="SocketFlags"/> values.</param>
		/// <returns>The number of bytes sent to the SecureSocket.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="buffer"/> is a null reference (<b>Nothing</b> in Visual Basic).</exception>
		/// <exception cref="SocketException">An operating system error occurs while accessing the SecureSocket.</exception>
		/// <exception cref="ArgumentOutOfRangeException">The offset or size parameter exceeds the size of buffer.</exception>
		/// <exception cref="ObjectDisposedException">The SecureSocket has been closed.</exception>
		/// <exception cref="SecurityException">Unable to encrypt the data.</exception>
		public override int Send(byte[] buffer, int offset, int size, SocketFlags socketFlags) {
			if (SecureProtocol == SecureProtocol.None)
				return base.Send(buffer, offset, size, socketFlags);
			else			
				return this.EndSend(this.BeginSend(buffer, offset, size, socketFlags, null, null));
		}
		/// <summary>
		/// Sends data asynchronously to a connected <see cref="SecureSocket"/>.
		/// </summary>
		/// <param name="buffer">The data to send.</param>
		/// <param name="offset">The zero-based position in the buffer parameter at which to begin sending data.</param>
		/// <param name="size">The number of bytes to send.</param>
		/// <param name="socketFlags">A bitwise combination of the <see cref="SocketFlags"/> values.</param>
		/// <param name="callback">The <see cref="AsyncCallback"/> delegate.</param>
		/// <param name="state">An object containing state information for this request.</param>
		/// <returns>An <see cref="IAsyncResult"/> that references the asynchronous send.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="buffer"/> is a null reference (<b>Nothing</b> in Visual Basic).</exception>
		/// <exception cref="SocketException">An operating system error occurs while accessing the SecureSocket.</exception>
		/// <exception cref="ArgumentOutOfRangeException">The specified offset or size exceeds the size of buffer.</exception>
		/// <exception cref="ObjectDisposedException">The SecureSocket has been closed.</exception>
		/// <exception cref="SecurityException">An error occurred while encrypting the data.</exception>
		public override IAsyncResult BeginSend(byte[] buffer, int offset, int size, SocketFlags socketFlags, AsyncCallback callback, object state) {
			if (SecureProtocol == SecureProtocol.None)
				return base.BeginSend(buffer, offset, size, socketFlags, callback, state);
			if (!Connected && HasBeenConnected)
				throw new ObjectDisposedException(this.GetType().FullName);
			if (!Connected)
				throw new SocketException();
			if (buffer == null)
				throw new ArgumentNullException();
			if (offset < 0 || offset >= buffer.Length || size > buffer.Length - offset)
				throw new ArgumentOutOfRangeException();
			// encrypt message
			AsyncSecureSendResult ssr = new AsyncSecureSendResult(this, callback, state);
			ssr.InternalStateObject = (int)0;
			ssr.Buffer = buffer;
			ssr.Offset = offset;
			ssr.Size = size;
			lock(ToEncryptList) {
				ToEncryptList.Add(ssr);
				if (!IsInitializing) { // Context may not be initialized yet
					OnEncryptData(ToEncryptList.Count - 1);
				}
			}
			return ssr;
		}
		private void OnEncryptData(int index) {
			AsyncSecureSendResult ssr = (AsyncSecureSendResult)ToEncryptList[index];
			try {
				int header = 0, message = 0, trailer = 0;
				if (SslProvider.GetBufferSizes(ref header, ref message, ref trailer) != 0 || message == 0)
					throw new SecurityException("Unable to query the buffer sizes.");
				if (ssr.Size < message)
					ssr.UnencryptedLength = ssr.Size;
				else
					ssr.UnencryptedLength = message;
				byte[] buffer = new byte[ssr.UnencryptedLength];
				Array.Copy(ssr.Buffer, ssr.Offset, buffer, 0, ssr.UnencryptedLength);
				ssr.Buffer = SslProvider.EncryptMessage(buffer);
				if (ssr.Buffer != null) {
					// send encrypted message
					lock(ToSendList) {
						ToSendList.Add(ssr);
						ToEncryptList.Remove(ssr);
					}
					if (!IsSending && !IsInitializing)
						BeginRawSend(ssr);
				} else {
					ssr.AsyncException = new SecurityException("Cannot encrypt data...");
					ssr.Notify();
				}
			} catch (Exception e) {
				ssr.AsyncException = e;
				ToEncryptList.Remove(ssr);
				ToSendList.Remove(ssr);
				SentList.Add(ssr);
				ssr.Notify();
			}
		}
		/// <summary>
		/// Ends a pending asynchronous send.
		/// </summary>
		/// <param name="asyncResult">The result of the asynchronous operation.</param>
		/// <returns>If successful, the number of bytes sent to the SecureSocket.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="asyncResult"/> is a null reference (<b>Nothing</b> in Visual Basic).</exception>
		/// <exception cref="ArgumentException"><paramref name="asyncResult"/> was not returned by a call to the <see cref="BeginSend"/> method.</exception>
		/// <exception cref="InvalidOperationException"><see cref="EndSend"/> was previously called for the asynchronous read.</exception>
		/// <exception cref="SocketException">An operating system error occurs while accessing the SecureSocket.</exception>
		/// <exception cref="ObjectDisposedException">The SecureSocket has been closed.</exception>
		public override int EndSend(IAsyncResult asyncResult) {
			if (SecureProtocol == SecureProtocol.None)
				return base.EndSend(asyncResult);
			if (asyncResult == null)
				throw new ArgumentNullException();
			if (!SentList.Contains(asyncResult) && !ToSendList.Contains(asyncResult) && !ToEncryptList.Contains(asyncResult)) {
				throw new InvalidOperationException();
			}
			if (!asyncResult.IsCompleted)
				asyncResult.AsyncWaitHandle.WaitOne();
			SentList.Remove(asyncResult);
			if (((AsyncSecureSendResult)asyncResult).AsyncException != null)
				throw ((AsyncSecureSendResult)asyncResult).AsyncException;
			return ((AsyncSecureSendResult)asyncResult).UnencryptedLength;
		}
		/// <summary>
		/// Gets the amount of data that has been received from the network and is available to be read.
		/// </summary>
		/// <value>The number of bytes of data that has been received from the network and are available to be read.</value>
		/// <exception cref="ObjectDisposedException">The SecureSocket has been closed.</exception>
		/// <exception cref="SocketException">An operating system error occurs while accessing the SecureSocket.</exception>
		/// <exception cref="SecurityException">An error occurs while interpreting the security data.</exception>
		public override int Available {
			get {
				if (SecureProtocol == SecureProtocol.None)
					return base.Available;
				if (!Connected && HasBeenConnected)
					throw new ObjectDisposedException(this.GetType().FullName);
				if (!Connected)
					throw new SocketException();
				if (IsInitializing) { // do not interfere with handshake
					return 0;
				}
				try {
					if (base.Available != 0) { // there are some encrypted bytes available
						AsyncAvailableResult = new AsyncSecureAvailableResult();
						BeginRawReceive();
					} else if(this.ReceivedBytes != 0) { // there are some encrypted bytes in the input buffer
						AsyncAvailableResult = new AsyncSecureAvailableResult();
						OnRawReceive(null);
					} else { // there are no new bytes available
						return DecryptedBuffer.Length;
					}
					AsyncAvailableResult.AsyncWaitHandle.WaitOne();
					if (AsyncAvailableResult.AsyncException != null)
						throw AsyncAvailableResult.AsyncException;
					return DecryptedBuffer.Length;
				} finally {
					AsyncAvailableResult = null;
				}
			}
		}
		/// <summary>
		/// Frees resources used by the <see cref="SecureSocket"/> class.
		/// </summary>
		/// <remarks>
		/// The SecureSocket class finalizer calls the Close method to close the SecureSocket and free resources associated with the SecureSocket.
		/// </remarks>
		~SecureSocket() {
			Close();
		}
		/// <summary>
		/// Gets the local certificate.
		/// </summary>
		/// <value>An instance of the <see cref="Certificate"/> class.</value>
		public Certificate LocalCertificate {
			get {
				return m_Options.certificate;
			}
		}
		/// <summary>
		/// Gets the remote certificate.
		/// </summary>
		/// <value>An instance of the <see cref="Certificate"/> class.</value>
		public Certificate RemoteCertificate {
			get {
				return m_RemoteCertificate;
			}
		}
		/// <summary>
		/// Gets the security protocol in use.
		/// </summary>
		/// <value>A bitwise combination of the <see cref="SecureProtocol"/> values.</value>
		public SecureProtocol SecureProtocol {
			get {
				return m_Options.secureProtocol;
			}
		}
		/// <summary>
		/// Gets the credential type.
		/// </summary>
		/// <value>One of the <see cref="CredentialUse"/> values.</value>
		public CredentialUse CredentialUse {
			get {
				return m_Options.use;
			}
		}
		/// <summary>
		/// Gets the common name of the remote host.
		/// </summary>
		/// <value>A string representing the common name of the remote host.</value>
		/// <remarks>
		/// The common name of the remote host is usually the domain name.
		/// </remarks>
		public string CommonName {
			get {
				return m_Options.commonName;
			}
		}
		/// <summary>
		/// Gets the credential verification type.
		/// </summary>
		/// <value>One of the <see cref="CredentialVerification"/> values.</value>
		public CredentialVerification VerifyType {
			get {
				return m_Options.verifyType;
			}
		}
		/// <summary>
		/// Gets the verify delegate.
		/// </summary>
		/// <value>A <see cref="CertVerifyEventHandler"/> instance.</value>
		public CertVerifyEventHandler Verifier {
			get {
				return m_Options.verifier;
			}
		}
		/// <summary>
		/// Gets the security flags of the connection.
		/// </summary>
		/// <value>A bitwise combination of the <see cref="SecurityFlags"/> values.</value>
		public SecurityFlags SecurityFlags {
			get {
				return m_Options.flags;
			}
		}
		/// <summary>
		/// Gets the <see cref="SslProvider"/> associated with this <see cref="SecureSocket"/>.
		/// </summary>
		/// <value>An <see cref="SslProvider"/> instance.</value>
		internal SslProvider SslProvider {
			get {
				return m_SslProvider;
			}
		}
		/// <summary>
		/// Gets or sets a value that indicates whether the socket has been connected before.
		/// </summary>
		/// <value><b>true</b> if the <see cref="SecureSocket"/> has been connected before, <b>false</b> otherwise.</value>
		protected bool HasBeenConnected {
			get {
				return m_HasBeenConnected;
			}
			set {
				m_HasBeenConnected = value;
			}
		}
		/// <summary>
		/// Gets or sets a value that indicates whether the <see cref="SecureSocket"/> is in the process of initializing the security protocol.
		/// </summary>
		/// <value><b>true</b> if the <see cref="SecureSocket"/> is in the process of initializing the security protocol, <b>false</b> otherwise.</value>
		protected bool IsInitializing {
			get {
				return m_IsInitializing;
			}
			set {
				m_IsInitializing = value;
			}
		}
		/// <summary>
		/// Gets or sets a value that indicates whether the <see cref="SecureSocket"/> is in the process of sending data to the remote server.
		/// </summary>
		/// <value><b>true</b> if the <see cref="SecureSocket"/> is in the process of sending data to the remote server, <b>false</b> otherwise.</value>
		protected bool IsSending {
			get {
				return m_IsSending;
			}
			set {
				m_IsSending = value;
			}
		}
		private ArrayList ToSendList {
			get {
				return m_ToSendList;
			}
		}
		private ArrayList SentList {
			get {
				return m_SentList;
			}
		}
		private ArrayList ToEncryptList {
			get {
				return m_ToEncryptList;
			}
		}
		private SecurityOptions m_Options;
		private Certificate m_RemoteCertificate = null;
		private bool m_HasBeenConnected;
		private bool m_IsInitializing;
		private bool m_IsSending;
		private SslProvider m_SslProvider;
		private AsyncSecureAcceptResult AsyncAcceptResult = null;
		private AsyncSecureConnectResult AsyncConnectResult = null;
		private AsyncSecureReceiveResult AsyncReceiveResult = null;
		private AsyncSecureSendResult AsyncSendResult = null; // used in BeginRawSend
		private AsyncSecureShutdownResult AsyncShutdownResult = null;
		private AsyncSecureAvailableResult AsyncAvailableResult = null;
		private AsyncSecureRenegotiateResult AsyncRenegotiateResult = null;
		private ArrayList m_ToSendList = new ArrayList(); // stores a list of AsyncSecureSendResult objects
		private ArrayList m_SentList = new ArrayList(); // stores a list of AsyncSecureSendResult objects
		private ArrayList m_ToEncryptList = new ArrayList(); // stores a list of AsyncSecureSendResult objects
		private byte[] ReceiveBuffer = new byte[65536];
		private int ReceivedBytes = 0;
		private byte[] DecryptedBuffer = new byte[0];
		/* The following methods are only used for testing purposes
		 * At this moment, users of the SecureSocket class cannot start a renegotiation;
		 * this may be supported in future versions

				public void Renegotiate() {
					this.EndRenegotiate(this.BeginRenegotiate(null, null));
				}
				public IAsyncResult BeginRenegotiate(AsyncCallback callback, object state) {
					if (!Connected && HasBeenConnected)
						throw new ObjectDisposedException(this.GetType().FullName);
					if (AsyncRenegotiateResult != null || !Connected)
						throw new SocketException();
					AsyncSecureRenegotiateResult ar = new AsyncSecureRenegotiateResult(this, callback, state);
					AsyncRenegotiateResult = ar;
					if (SecureProtocol == SecureProtocol.None || SecureProtocol == SecureProtocol.Ssl2) {
						AsyncRenegotiateResult.Notify();
					} else {
						DoHandshake(this.CredentialUse == CredentialUse.Client ? HandshakeType.ClientRenegotiate : HandshakeType.ServerRenegotiate);
					}
					return ar;
				}
				public void EndRenegotiate(IAsyncResult asyncResult) {
					if (asyncResult == null)
						throw new ArgumentNullException();
					if (AsyncRenegotiateResult == null)
						throw new InvalidOperationException();
					if (asyncResult != AsyncRenegotiateResult)
						throw new ArgumentException();
					// Process the EndRenegotiate
					// block if the operation hasn't ended yet
					AsyncSecureRenegotiateResult ar = AsyncRenegotiateResult;
					if (!ar.IsCompleted)
						ar.AsyncWaitHandle.WaitOne();
					AsyncRenegotiateResult = null;
					if (ar.AsyncException != null) {
						throw ar.AsyncException;
					}
				}*/
	}
}