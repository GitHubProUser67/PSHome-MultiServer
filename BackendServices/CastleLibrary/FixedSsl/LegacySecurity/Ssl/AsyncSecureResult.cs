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
using System.Threading;

namespace Org.Mentalis.LegacySecurity.Ssl {
	/// <summary>
	/// A class that implements the IAsyncResult interface.
	/// </summary>
	internal class AsyncSecureResult : IAsyncResult {
		/// <summary>Initializes the internal variables of this object</summary>
		/// <param name="parent">The parent object of this AsyncSecureResult.</param>
		/// <param name="callback">The callback function associated with this asynchronous action.</param>
		/// <param name="stateObject">An object that contains state information for this request.</param>
		internal AsyncSecureResult(object parent, AsyncCallback callback, object stateObject) {
			m_Parent = parent;
			m_StateObject = stateObject;
			m_Completed = false;
			if (callback != null)
				Callback += callback;
		}
		/// <summary>
		/// Sets the waithandle and calls the listeners, if any.
		/// </summary>
		public void Notify() {
			m_Completed = true;
			if (Callback != null)
				Callback(this);
			if (m_WaitHandle != null)
				m_WaitHandle.Set();
		}
		/// <summary>Gets the parent object of this AsyncSecureResult.</summary>
		/// <value>An object representing the parent of this AsyncSecureResult.</value>
		public object Parent {
			get {
				return m_Parent;
			}
		}
		/// <summary>Gets or sets an Exception that occurred while executing the asynchronous request.</summary>
		/// <value>An Exception object that was thrown while executing the asynchronous request.</value>
		public Exception AsyncException {
			get {
				return m_AsyncException;
			}
			set {
				m_AsyncException = value;
			}
		}
		/// <summary>Gets a value that indicates whether the server has completed processing the call. It is illegal for the server to use any client supplied resources outside of the agreed upon sharing semantics after it sets the IsCompleted property to "true". Thus, it is safe for the client to destroy the resources after IsCompleted property returns "true".</summary>
		/// <value>A boolean that indicates whether the server has completed processing the call.</value>
		public bool IsCompleted {
			get {
				return m_Completed;
			}
		}
		/// <summary>Gets a value that indicates whether the BeginXXXX call has been completed synchronously. If this is detected in the AsyncCallback delegate, it is probable that the thread that called BeginInvoke is the current thread.</summary>
		/// <value>Returns false.</value>
		public bool CompletedSynchronously {
			get {
				return false;
			}
		}
		/// <summary>Gets an object that was passed as the state parameter of the BeginXXXX method call.</summary>
		/// <value>The object that was passed as the state parameter of the BeginXXXX method call.</value>
		public object AsyncState {
			get {
				return m_StateObject;
			}
		}
		/// <summary>Gets or sets an object that can be used for internal purposes.</summary>
		/// <value>An object instance.</value>
		public object InternalStateObject {
			get {
				return m_InternalStateObject;
			}
			set {
				m_InternalStateObject = value;
			}
		}
		/// <summary>
		/// The AsyncWaitHandle property returns the WaitHandle that can use to perform a WaitHandle.WaitOne or WaitAny or WaitAll. The object which implements IAsyncResult need not derive from the System.WaitHandle classes directly. The WaitHandle wraps its underlying synchronization primitive and should be signaled after the call is completed. This enables the client to wait for the call to complete instead polling. The Runtime supplies a number of waitable objects that mirror Win32 synchronization primitives e.g. ManualResetEvent, AutoResetEvent and Mutex.
		/// WaitHandle supplies methods that support waiting for such synchronization objects to become signaled with "any" or "all" semantics i.e. WaitHandle.WaitOne, WaitAny and WaitAll. Such methods are context aware to avoid deadlocks. The AsyncWaitHandle can be allocated eagerly or on demand. It is the choice of the IAsyncResult implementer.
		///</summary>
		/// <value>The WaitHandle associated with this asynchronous result.</value>
		public WaitHandle AsyncWaitHandle {
			get {
				if (m_WaitHandle == null)
					m_WaitHandle = new ManualResetEvent(m_Completed);
				return m_WaitHandle;
			}
		}
		// private variables
		/// <summary>Used internally to represent the state of the asynchronous request</summary>
		private bool m_Completed;
		/// <summary>Holds the value of the StateObject property.</summary>
		private object m_StateObject;
		/// <summary>Holds the value of the InternalStateObject property.</summary>
		private object m_InternalStateObject;
		/// <summary>Holds the value of the WaitHandle property.</summary>
		private ManualResetEvent m_WaitHandle;
		/// <summary>Holds the value of the Parent property.</summary>
		private object m_Parent;
		/// <summary>Holds the value of the AsyncException property.</summary>
		private Exception m_AsyncException = null;
		/// <summary>Holds the Buffer associated with this AsyncResult.</summary>
		public byte[] Buffer = null;
		/// <summary>Holds the list of callback functions.</summary>
		public event AsyncCallback Callback;
	}
	/// <summary>
	/// A class that implements the IAsyncResult interface.
	/// </summary>
	internal class AsyncSecureConnectResult : AsyncSecureResult {
		/// <summary>Initializes the internal variables of this object</summary>
		/// <param name="parent">The parent object of this AsyncSecureResult.</param>
		/// <param name="stateObject">An object that contains state information for this request.</param>
		/// <param name="callback">The callback function associated with this asynchronous action.</param>
		internal AsyncSecureConnectResult(object parent, AsyncCallback callback, object stateObject) : base(parent, callback, stateObject) {
			m_InitializeServerContext = true;
		}
		/// <summary>
		/// Gets or sets a boolean that indicates whether the server context should be initialized or not.
		/// </summary>
		/// <value><b>true</b> if the server context should be initialized, otherwise <b>false</b>.</value>
		public bool InitializeServerContext {
			get {
				return m_InitializeServerContext;
			}
			set {
				m_InitializeServerContext = value;
			}
		}
		/// <summary>Holds the value of the InitializeServerContext property.</summary>
		private bool m_InitializeServerContext;
	}
	/// <summary>
	/// A class that implements the IAsyncResult interface.
	/// </summary>
	internal class AsyncSecureSendResult : AsyncSecureTransferResult {
		/// <summary>Initializes the internal variables of this object</summary>
		/// <param name="parent">The parent object of this AsyncSecureResult.</param>
		/// <param name="stateObject">An object that contains state information for this request.</param>
		/// <param name="callback">The callback function associated with this asynchronous action.</param>
		internal AsyncSecureSendResult(object parent, AsyncCallback callback, object stateObject) : base(parent, callback, stateObject) {}
		/// <summary>
		/// Gets or sets the length of the unencrypted buffer.
		/// </summary>
		/// <value>One of the Int32 values.</value>
		public int UnencryptedLength {
			get {
				return m_UnencryptedLength;
			}
			set {
				m_UnencryptedLength = value;
			}
		}
		/// <summary>Holds the value of the UnencryptedLength property.</summary>
		private int m_UnencryptedLength;
	}
	/// <summary>
	/// A class that implements the IAsyncResult interface.
	/// </summary>
	internal class AsyncSecureReceiveResult : AsyncSecureTransferResult {
		/// <summary>Initializes the internal variables of this object</summary>
		/// <param name="parent">The parent object of this AsyncSecureResult.</param>
		/// <param name="stateObject">An object that contains state information for this request.</param>
		/// <param name="callback">The callback function associated with this asynchronous action.</param>
		internal AsyncSecureReceiveResult(object parent, AsyncCallback callback, object stateObject) : base(parent, callback, stateObject) {}
	}
	/// <summary>
	/// A class that implements the IAsyncResult interface.
	/// </summary>
	internal class AsyncSecureTransferResult : AsyncSecureResult {
		/// <summary>Initializes the internal variables of this object</summary>
		/// <param name="parent">The parent object of this AsyncSecureResult.</param>
		/// <param name="stateObject">An object that contains state information for this request.</param>
		/// <param name="callback">The callback function associated with this asynchronous action.</param>
		internal AsyncSecureTransferResult(object parent, AsyncCallback callback, object stateObject) : base(parent, callback, stateObject) {}
		/// <summary>
		/// Gets or sets the number of bytes that are transferred.
		/// </summary>
		/// <value>One of the Int32 values.</value>
		public int SizeTransferred {
			get {
				return m_SizeTransferred;
			}
			set {
				m_SizeTransferred = value;
			}
		}
		/// <summary>
		/// Gets or sets the offset in the buffer.
		/// </summary>
		/// <value>One of the Int32 values.</value>
		public int Offset {
			get {
				return m_Offset;
			}
			set {
				m_Offset = value;
			}
		}
		/// <summary>
		/// Gets or sets the number of bytes in the buffer to send.
		/// </summary>
		/// <value>One of the Int32 values.</value>
		public int Size {
			get {
				return m_Size;
			}
			set {
				m_Size = value;
			}
		}
		/// <summary>Holds the value of the Offset property.</summary>
		private int m_Offset;
		/// <summary>Holds the value of the Size property.</summary>
		private int m_Size;
		/// <summary>Holds the value of the SizeTransferred property.</summary>
		private int m_SizeTransferred;
	}
	/// <summary>
	/// A class that implements the IAsyncResult interface.
	/// </summary>
	internal class AsyncSecureAcceptResult : AsyncSecureResult {
		/// <summary>Initializes the internal variables of this object</summary>
		/// <param name="parent">The parent object of this AsyncSecureResult.</param>
		/// <param name="stateObject">An object that contains state information for this request.</param>
		/// <param name="callback">The callback function associated with this asynchronous action.</param>
		internal AsyncSecureAcceptResult(object parent, AsyncCallback callback, object stateObject) : base(parent, callback, stateObject) {
			m_InitializeServerContext = true;
		}
		/// <summary>
		/// Gets or sets a boolean that indicates whether the server context should be initialized or not.
		/// </summary>
		/// <value><b>true</b> if the server context should be initialized, otherwise <b>false</b>.</value>
		public bool InitializeServerContext {
			get {
				return m_InitializeServerContext;
			}
			set {
				m_InitializeServerContext = value;
			}
		}
		/// <summary>
		/// The Socket returned by the InternalEndAccept method.
		/// </summary>
		/// <value>An instance of the Socket class.</value>
		internal SecureSocket AcceptedSocket {
			get {
				return m_AcceptedSocket;
			}
			set {
				m_AcceptedSocket = value;
			}
		}
		/// <value>Holds the value of the AcceptedSocket property.</value>
		private SecureSocket m_AcceptedSocket;
		/// <value>Holds the value of the InitializeServerContext property.</value>
		private bool m_InitializeServerContext;
	}
	/// <summary>
	/// A class that implements the IAsyncResult interface.
	/// </summary>
	internal class AsyncSecureShutdownResult : AsyncSecureResult {
		/// <summary>Initializes the internal variables of this object</summary>
		/// <param name="parent">The parent object of this AsyncSecureShutdownResult.</param>
		/// <param name="callback">The callback function associated with this asynchronous action.</param>
		/// <param name="stateObject">An object that contains state information for this request.</param>
		internal AsyncSecureShutdownResult(object parent, AsyncCallback callback, object stateObject) : base(parent, callback, stateObject) {}
	}
	/// <summary>
	/// A class that implements the IAsyncResult interface.
	/// </summary>
	internal class AsyncSecureAvailableResult : AsyncSecureResult {
		/// <summary>Initializes the internal variables of this object</summary>
		internal AsyncSecureAvailableResult() : base(null, null, null) {}
	}
	/// <summary>
	/// A class that implements the IAsyncResult interface.
	/// </summary>
	internal class AsyncSecureRenegotiateResult : AsyncSecureResult {
		/// <summary>Initializes the internal variables of this object</summary>
		/// <param name="parent">The parent object of this AsyncSecureShutdownResult.</param>
		/// <param name="callback">The callback function associated with this asynchronous action.</param>
		/// <param name="stateObject">An object that contains state information for this request.</param>
		internal AsyncSecureRenegotiateResult(object parent, AsyncCallback callback, object stateObject) : base(parent, callback, stateObject) {
			m_DoHandshake = false;
		}
		/// <summary>
		/// Gets or sets a value that indicates whether the handshake should be performed, or not.
		/// </summary>
		/// <value><b>true</b> if a handshake should be performed, <b>false</b> otherwise.</value>
		public bool DoHandshake {
			get {
				return m_DoHandshake;
			}
			set {
				m_DoHandshake = value;
			}
		}
		/// <summary>
		/// Holds the value of the DoHandshake property.
		/// </summary>
		private bool m_DoHandshake;
	}
}