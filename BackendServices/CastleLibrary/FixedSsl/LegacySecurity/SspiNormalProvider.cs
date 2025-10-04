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
using Org.Mentalis.LegacySecurity.Ssl;

namespace Org.Mentalis.LegacySecurity {
	/// <summary>
	/// Defines the SCHANNEL API.
	/// </summary>
	/// <remarks>
	/// These functions do not work on Windows NT4.
	/// </remarks>
	internal sealed class SspiNormalProvider {
		/// <summary>
		/// Defeat instantiation.
		/// </summary>
		private SspiNormalProvider() {}
		/// <summary>
		/// The DeleteSecurityContext function deletes the local data structures associated with the specified security context.
		/// </summary>
		/// <param name="phContext">Handle of the security context to delete.</param>
		/// <returns>If the function succeeds, the return value is SEC_E_OK.<br>If the function fails, the return value can be the following error code: SEC_E_INVALID_HANDLE</br></returns>
		[DllImport(@"secur32.dll", EntryPoint="DeleteSecurityContext", CallingConvention=CallingConvention.StdCall, CharSet=CharSet.Ansi)]	
		public static extern int DeleteSecurityContext(IntPtr phContext);
		/// <summary>
		/// The InitializeSecurityContext function creates the client-side security context used to secure communications between a client and server. 
		/// <br>The Schannel implementation of InitializeSecurityContext returns the messages required by the selected protocol and a security token that the client must pass to the target server.</br>
		///</summary>
		/// <param name="phCredential">Pointer to a handle to the client's credentials. This handle is returned by the AcquireCredentialsHandle function.</param>
		/// <param name="phContext">Pointer to a CtxtHandle. This pointer receives a token representing the security context. On the first call to InitializeSecurityContext, specify NULL. On future calls, specify the token received in the phNewContext parameter after the first call to this function.</param>
		/// <param name="pszTargetName">Specify a string that uniquely identifies the target server. Schannel uses this value to locate the session in the session cache when re-establishing a connection.</param>
		/// <param name="fContextReq">Indicates that the security context should support certain features, or attributes.</param>
		/// <param name="Reserved1">Reserved value; specify zero.</param>
		/// <param name="TargetDataRep">Not used with Schannel. Specify zero.</param>
		/// <param name="pInput">Pointer to a SecBufferDesc structure that contains pointers to the buffers supplied to Schannel. On the first call to the function, specify NULL. On additional calls, there should be two buffers. The first of type SECBUFFER_TOKEN containing the token received from the server. The second buffer will received data that is not processed by Schannel, if any.</param>
		/// <param name="Reserved2">Reserved value; must be zero.</param>
		/// <param name="phNewContext">Pointer to a CtxtHandle structure. On the first call to InitializeSecurityContext, this pointer receives a new context handle. In subsequent calls, pass this handle using the phContext parameter and specify NULL for phNewContext.</param>
		/// <param name="pOutput">Pointer to a SecBufferDesc structure that describes one SecBuffer structure of type SECBUFFER_TOKEN, which receives a security token. If the ISC_REQ_ALLOCATE_MEMORY flag is specified Schannel will allocate this buffer and put the appropriate information in the SecBufferDesc.</param>
		/// <param name="pfContextAttr">Pointer to a ULONG that receives bit flags indicating the attributes of the established context. For a list of valid values, refer to the table included in the fContextReq parameter description. pfContextAttr receives a set of flags corresponding to each of the values in the table; however, the ISC_REQ prefix is replaced by ISC_RET to differentiate between requested attributes and returned attributes.<br>Warning  Do not check for security-related attributes until the final function call returns successfully. Other (non-security) attribute flags such as the ISC_RET_ALLOCATED_MEMORY flag, can be checked before the final return.</br></param>
		/// <param name="ptsExpiry">Optional. Pointer to a TimeStamp structure. In Windows XP, when the remote party has supplied a certificate to be used for authentication, this parameter receives the expiration time for that certificate. If no certificate was supplied then a maximum time value is returned. In Windows versions prior to Windows XP, this parameter is not altered by the function.</param>
		/// <returns>If the function succeeds, the function returns one of the following values: SEC_E_OK, SEC_E_INCOMPLETE_MESSAGE, SEC_I_INCOMPLETE_CREDENTIALS or SEC_I_CONTINUE_NEEDED.<br>If the function fails, the return value is one of the following error codes: SEC_E_INVALID_HANDLE, SEC_E_TARGET_UNKNOWN, SEC_E_NO_CREDENTIALS, SEC_E_UNSUPPORTED_FUNCTION or SEC_E_INSUFFICIENT_MEMORY.</br></returns>
		[DllImport(@"secur32.dll", EntryPoint="InitializeSecurityContextA", CallingConvention=CallingConvention.StdCall, CharSet=CharSet.Ansi)]	
		public static extern int InitializeSecurityContext(IntPtr phCredential, IntPtr phContext, string pszTargetName, int fContextReq, int Reserved1, int TargetDataRep, IntPtr pInput, int Reserved2, IntPtr phNewContext, IntPtr pOutput, ref int pfContextAttr, IntPtr ptsExpiry);
		/// <summary>
		/// The FreeContextBuffer function enables callers of security package functions to free a memory buffer allocated by the security package. For example, the InitializeSecurityContext function may allocate a buffer for returning the outbound context token.
		/// </summary>
		/// <param name="pvContextBuffer">Pointer to memory allocated by the security package.</param>
		/// <returns>If the function succeeds, the return value is SEC_E_OK.<br>If the function fails, the return value is a nonzero error code.</br></returns>
		[DllImport(@"secur32.dll", EntryPoint="FreeContextBuffer", CallingConvention=CallingConvention.StdCall, CharSet=CharSet.Ansi)]	
		public static extern int FreeContextBuffer(IntPtr pvContextBuffer);
		/// <summary>
		/// The AcceptSecurityContext function creates the server-side security context used to secure communications between a client and server. The Schannel implementation of AcceptSecurityContext returns the messages required by the selected protocol and a security token, which the server must pass to the client.
		/// </summary>
		/// <param name="phCredential">Handle to the server's credentials. This handle is returned by the AcquireCredentialsHandle function.</param>
		/// <param name="phContext">Pointer to a CtxtHandle. This pointer receives a token representing the security context. On the first call to AcceptSecurityContext, specify NULL. On future calls, specify the token received in the phNewContext parameter after the first call to this function.</param>
		/// <param name="pInput">Pointer to a SecBufferDesc structure containing two SecBuffer structures. The first buffer must be of type SECBUFFER_TOKEN and contain the security token received from the client. The second buffer should be of type SECBUFFER_EMPTY.</param>
		/// <param name="fContextReq">Indicates that the security context should support certain features, or attributes.</param>
		/// <param name="TargetDataRep">Not used with Schannel. Specify zero.</param>
		/// <param name="phNewContext">Pointer to a CtxtHandle structure. On the first call to AcceptSecurityContext, this pointer receives a new context handle. In subsequent calls, pass this handle using the phContext parameter and specify NULL for phNewContext.</param>
		/// <param name="pOutput">Pointer to a SecBufferDesc structure that contains a SecBuffer structure of type SECBUFFER_TOKEN. On output, this buffer will receive a token for the security context. The token must be sent to the client. The function may also return a buffer of type SECBUFFER_EXTRA.</param>
		/// <param name="pfContextAttr">Pointer to a ULONG that receives bit flags indicating the attributes of the established context. For a list of valid values, refer to the table included in the fContextReq parameter description. pfContextAttr receives a set of flags corresponding to each of the values in the table; however, the ASC_REQ prefix is replaced by ASC_RET to differentiate between requested attributes and returned attributes.</param>
		/// <param name="ptsExpiry">Optional. Pointer to a TimeStamp structure. In Windows XP, when the remote party has supplied a certificate to be used for authentication, this parameter receives the expiration time for that certificate. If no certificate was supplied, a maximum time value is returned. In Windows versions prior to Windows XP, this parameter is not altered by the function.</param>
		/// <returns>
		/// If the function succeeds, it returns one of the following values: SEC_E_OK, SEC_E_INCOMPLETE_MESSAGE or SEC_I_CONTINUE_NEEDED
		/// <br>If the function fails, it returns one of the following values: SEC_E_INVALID_TOKEN, SEC_E_INVALID_HANDLE, SEC_E_UNSUPPORTED_FUNCTION, SEC_E_NO_CREDENTIALS or SEC_E_INSUFFICIENT_MEMORY.</br>
		/// </returns>
		[DllImport(@"secur32.dll", EntryPoint="AcceptSecurityContext", CallingConvention=CallingConvention.StdCall, CharSet=CharSet.Ansi)]	
		public static extern int AcceptSecurityContext(IntPtr phCredential, IntPtr phContext, IntPtr pInput, int fContextReq, int TargetDataRep, IntPtr phNewContext, IntPtr pOutput, ref int pfContextAttr, IntPtr ptsExpiry);
		/// <summary>
		/// The FreeCredentialsHandle function notifies the security system that the credentials are no longer needed. An application calls this function to free the credential handle acquired in the call to the AcquireCredentialsHandle function. When all references to this credential set have been removed, the credentials themselves can be removed.
		/// </summary>
		/// <param name="phCredential">Pointer to the credential handle obtained by using the AcquireCredentialsHandle function.</param>
		/// <returns>If the function succeeds, the return value is SEC_E_OK.<br>If the function fails, the return value may be the following error code: SEC_E_INVALID_HANDLE</br></returns>
		[DllImport(@"secur32.dll", EntryPoint="FreeCredentialsHandle", CallingConvention=CallingConvention.StdCall, CharSet=CharSet.Ansi)]	
		public static extern int FreeCredentialsHandle(IntPtr phCredential);
		/// <summary>
		/// The AcquireCredentialsHandle function retrieves a handle to the credentials of a security principal. This handle is required by the InitializeSecurityContext and AcceptSecurityContext functions.
		/// </summary>
		/// <param name="pszPrincipal">Not used with Schannel. Specify NULL.</param>
		/// <param name="pszPackage">Specify UNISP_NAME. This constant is defined as "Microsoft Unified Security Protocol Provider".</param>
		/// <param name="fCredentialUse">Flag indicating how the credential will be used.</param>
		/// <param name="pvLogonID">Not used with Schannel. Specify NULL.</param>
		/// <param name="pAuthData">In Windows 2000 and Windows XP, specify an SCHANNEL_CRED structure indicating the protocol to use and settings for various customizable channel features.</param>
		/// <param name="pGetKeyFn">Not used with Schannel. Specify NULL.</param>
		/// <param name="pvGetKeyArgument">Not used with Schannel. Specify NULL.</param>
		/// <param name="phCredential">Pointer to a CredHandle. Receives the requested credential handle.</param>
		/// <param name="ptsExpiry">Optional. Pointer to a TimeStamp. In Windows XP, when the remote party has supplied a certificate to be used for authentication, this parameter receives the expiration time for that certificate. If no certificate was supplied then a maximum time value is returned. In Windows versions prior to Windows XP, this parameter receives zero.</param>
		/// <returns>If the function succeeds, the return value is SEC_E_OK.<br>If the function fails, the return value is an SECURITY_STATUS indicating the error.</br></returns>
		[DllImport(@"secur32.dll", EntryPoint="AcquireCredentialsHandleA", CallingConvention=CallingConvention.StdCall, CharSet=CharSet.Ansi)]	
		public static extern int AcquireCredentialsHandle(string pszPrincipal, string pszPackage, int fCredentialUse, IntPtr pvLogonID, ref SCHANNEL_CRED pAuthData, IntPtr pGetKeyFn, IntPtr pvGetKeyArgument, IntPtr phCredential, IntPtr ptsExpiry);
		/// <summary>
		/// The QueryContextAttributes function retrieves information about attributes or features supported by a specified security context.
		/// </summary>
		/// <param name="phContext">Handle to the security context to be queried.</param>
		/// <param name="ulAttribute">Specifies which context attribute is returned.</param>
		/// <param name="pBuffer">Pointer to a structure containing the requested attribute. The ulAttribute parameter description indicates the structure returned for each attribute.</param>
		/// <returns>If the function succeeds, the return value is SEC_E_OK.<br>If the function fails, the return value is a nonzero error code.</br></returns>
		[DllImport(@"secur32.dll", EntryPoint="QueryContextAttributesA", CallingConvention=CallingConvention.StdCall, CharSet=CharSet.Ansi)]	
		public static extern int QueryContextAttributesSize(IntPtr phContext, int ulAttribute, ref SecPkgContext_StreamSizes pBuffer);
		/// <summary>
		/// The QueryContextAttributes function retrieves information about attributes or features supported by a specified security context.
		/// </summary>
		/// <param name="phContext">Handle to the security context to be queried.</param>
		/// <param name="ulAttribute">Specifies which context attribute is returned.</param>
		/// <param name="pBuffer">Pointer to a structure containing the requested attribute. The ulAttribute parameter description indicates the structure returned for each attribute.</param>
		/// <returns>If the function succeeds, the return value is SEC_E_OK.<br>If the function fails, the return value is a nonzero error code.</br></returns>
		[DllImport(@"secur32.dll", EntryPoint="QueryContextAttributesA", CallingConvention=CallingConvention.StdCall, CharSet=CharSet.Ansi)]	
		public static extern int QueryContextAttributesCertificate(IntPtr phContext, int ulAttribute, ref IntPtr pBuffer);
		/// <summary>
		/// The EncryptMessage function encrypts a message using the session key negotiated with the remote party who will receive the message. The encryption algorithm is determined by the cipher suite in use.
		/// <br><b>Note</b>  In "streaming" mode, that is, when the ISC_REQ_STREAM or ASC_REQ_STREAM flag was specified during the handshake, EncryptMessage or DecryptMessage cannot be called at the same time from multiple threads unless each thread has its own SSPI context. Each encryption or decryption operation changes the internal state of the encryption key. If the encryption key states are not synchronized on the client and server, the decryption operation fails.</br>
		/// </summary>
		/// <param name="phContext">Handle to the security context previously established with the message recipient.</param>
		/// <param name="fQOP">Not used with Schannel. Specify zero.</param>
		/// <param name="pMessage">Pointer to a SecBufferDesc structure. On input, this structure contains one or more SecBuffer structures, exactly one of which must be of type SECBUFFER_DATA. This buffer contains the message which is encrypted in place. For more information, see Remarks.</param>
		/// <param name="MessageSeqNo">Not used with Schannel. Specify zero.</param>
		/// <returns>If the function succeeds, the return value is SEC_E_OK.<br>If the function fails, the return value is a nonzero error code.</br></returns>
		[DllImport(@"secur32.dll", EntryPoint="EncryptMessage", CallingConvention=CallingConvention.StdCall, CharSet=CharSet.Ansi)]	
		public static extern int EncryptMessage(IntPtr phContext, int fQOP, IntPtr pMessage, int MessageSeqNo);
		/// <summary>
		/// The DecryptMessage function decrypts secured messages. It also signals when the message sender is requesting a renegotiation (redo) of the connection attributes or has shutdown the connection.
		/// <br><b>Note</b>  In "streaming" mode, that is, when the ISC_REQ_STREAM or ASC_REQ_STREAM flag was specified during the handshake, EncryptMessage or DecryptMessage cannot be called at the same time from multiple threads unless each thread has its own SSPI context. Each encryption or decryption operation changes the internal state of the encryption key. If the encryption key states are not synchronized on the client and server, the decryption operation fails.</br>
		/// </summary>
		/// <param name="phContext">Handle to the security context previously established with the message sender.</param>
		/// <param name="pMessage">Pointer to a SecBufferDesc structure. For contexts that are not connection-oriented, on input, the structure must contain four SecBuffer structures. Exactly one buffer must be of type SECBUFFER_DATA and contain an encrypted message, which is decrypted in place. The remaining buffers are used for output and must be of type SECBUFFER_EMPTY.<br>For connection-oriented contexts a SECBUFFER_DATA type buffer must be supplied, as noted for non-connection-oriented contexts. Additionally, a second SECBUFFER_TOKEN type buffer containing a security token must also be supplied.</br></param>
		/// <param name="MessageSeqNo">Not used with Schannel. Specify zero.</param>
		/// <param name="pfQOP">Not used with Schannel. Specify NULL.</param>
		/// <returns>If the function completes successfully and no connection state changes, such as renegotiation or shutdown, were initiated by the remote party, the return value is SEC_E_OK.<br>If the function fails or the connection state has changed, the function returns a nonzero value indicating the error or state.</br></returns>
		[DllImport(@"secur32.dll", EntryPoint="DecryptMessage", CallingConvention=CallingConvention.StdCall, CharSet=CharSet.Ansi)]	
		public static extern int DecryptMessage(IntPtr phContext, IntPtr pMessage, int MessageSeqNo, IntPtr pfQOP);
		/// <summary>
		/// The ApplyControlToken function is used to shut down the security context underlying an existing Schannel connection.
		/// </summary>
		/// <param name="phContext">Handle to a security context. Schannel will shut down the context and notify the remote party with whom this context was established. See Remarks.</param>
		/// <param name="pInput">Pointer to a SecBufferDesc structure that describes a single SecBuffer structure.</param>
		/// <returns>If the function succeeds, the return value is SEC_E_OK.<br>If the function fails, the return value is a nonzero error code.</br></returns>
		[DllImport(@"secur32.dll", EntryPoint="ApplyControlToken", CallingConvention=CallingConvention.StdCall, CharSet=CharSet.Ansi)]	
		public static extern int ApplyControlToken(IntPtr phContext, IntPtr pInput);
	}
}