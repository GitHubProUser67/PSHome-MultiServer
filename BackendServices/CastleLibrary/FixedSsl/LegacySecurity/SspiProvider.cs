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
	/// Defines the external methods of the CryptoAPI and SCHANNEL API.
	/// </summary>
	internal sealed class SspiProvider {
		/// <summary>
		/// Defeat instantiation of this class.
		/// </summary>
		private SspiProvider() {}
		/// <summary>
		/// The CertCloseStore function closes a certificate store handle and reduces the reference count on the store. There needs to be a corresponding call to CertCloseStore for each successful call to the CertOpenStore or CertDuplicateStore functions.
		/// </summary>
		/// <param name="hCertStore">Handle of the certificate store to be closed.</param>
		/// <param name="dwFlags">Typically, this parameter uses the default value zero. The default is to close the store with memory remaining allocated for contexts that have not been freed. In this case, no check is made to determine whether memory for contexts remains allocated. </param>
		/// <returns>If the function succeeds, the return value is TRUE.<br>If the function fails, the return value is FALSE.</br></returns>
		[DllImport(@"crypt32.dll")]	
		internal static extern int CertCloseStore(IntPtr hCertStore, int dwFlags);
		/// <summary>
		/// The CertFindCertificateInStore function finds the first or next certificate context in a certificate store that matches a search criteria established by the dwFindType and its associated pvFindPara. This function can be used in a loop to find all of the certificates in a certificate store that match the specified find criteria.
		/// </summary>
		/// <param name="hCertStore">Handle of the certificate store to be searched.</param>
		/// <param name="dwCertEncodingType">Specifies the type of encoding used. Both the certificate and message encoding types must be specified by combining them with a bitwise-OR operation.</param>
		/// <param name="dwFindFlags">Used with some dwFindType values to modify the search criteria. For most dwFindType values, dwFindFlags is not used and should be set to zero. For detailed information, see the Remarks section later in this topic.</param>
		/// <param name="dwFindType">Specifies the type of search being made. The search type determines the data type, contents, and the use of pvFindPara.</param>
		/// <param name="pvFindPara">Points to a data item or structure used with dwFindType.</param>
		/// <param name="pPrevCertContext">Pointer to the last CERT_CONTEXT structure returned by this function. This parameter must be NULL on the first call of the function. A pPrevCertContext parameter that is not NULL is always freed by this function, even if the function causes an error.</param>
		/// <returns>If the function succeeds, the return value is a pointer to a read-only CERT_CONTEXT structure.<br>If the function fails and a certificate that matches the search criteria is not found, the return value is NULL.</br><br>A non-NULL CERT_CONTEXT that CertFindCertificateInStore returns must be freed by CertFreeCertificateContext or by being passed as the pPrevCertContext parameter on a subsequent call to CertFindCertificateInStore.</br></returns>
		[DllImport(@"crypt32.dll")]	
		internal static extern IntPtr CertFindCertificateInStore(IntPtr hCertStore, int dwCertEncodingType, int dwFindFlags, int dwFindType, IntPtr pvFindPara, IntPtr pPrevCertContext);
		/// <summary>
		/// The CertFindCertificateInStore function finds the first or next certificate context in a certificate store that matches a search criteria established by the dwFindType and its associated pvFindPara. This function can be used in a loop to find all of the certificates in a certificate store that match the specified find criteria.
		/// </summary>
		/// <param name="hCertStore">Handle of the certificate store to be searched.</param>
		/// <param name="dwCertEncodingType">Specifies the type of encoding used. Both the certificate and message encoding types must be specified by combining them with a bitwise-OR operation.</param>
		/// <param name="dwFindFlags">Used with some dwFindType values to modify the search criteria. For most dwFindType values, dwFindFlags is not used and should be set to zero. For detailed information, see the Remarks section later in this topic.</param>
		/// <param name="dwFindType">Specifies the type of search being made. The search type determines the data type, contents, and the use of pvFindPara.</param>
		/// <param name="pvFindPara">Points to a data item or structure used with dwFindType.</param>
		/// <param name="pPrevCertContext">Pointer to the last CERT_CONTEXT structure returned by this function. This parameter must be NULL on the first call of the function. A pPrevCertContext parameter that is not NULL is always freed by this function, even if the function causes an error.</param>
		/// <returns>If the function succeeds, the return value is a pointer to a read-only CERT_CONTEXT structure.<br>If the function fails and a certificate that matches the search criteria is not found, the return value is NULL.</br><br>A non-NULL CERT_CONTEXT that CertFindCertificateInStore returns must be freed by CertFreeCertificateContext or by being passed as the pPrevCertContext parameter on a subsequent call to CertFindCertificateInStore.</br></returns>
		[DllImport(@"crypt32.dll", EntryPoint="CertFindCertificateInStore")]	
		internal static extern IntPtr CertFindHashCertificateInStore(IntPtr hCertStore, int dwCertEncodingType, int dwFindFlags, int dwFindType, ref DataBlob pvFindPara, IntPtr pPrevCertContext);
		/// <summary>
		/// The CertFreeCertificateContext function frees a certificate context by decrementing its reference count. When the reference count goes to zero, CertFreeCertificateContext frees the memory occupied by a certificate context. Each context obtained by a get, find, enumerate, duplicate, or create operation must be freed by the appropriate free operation.
		/// </summary>
		/// <param name="pCertContext">Pointer to the CERT_CONTEXT to be freed.</param>
		/// <returns>The function always returns TRUE.</returns>
		[DllImport(@"crypt32.dll")]	
		internal static extern int CertFreeCertificateContext(IntPtr pCertContext);
		/// <summary>
		/// The CertOpenSystemStore function is a simplified function used to open the most common system certificate store. To open certificate stores with more complex requirements, such as file-based or memory-based stores, use CertOpenStore.
		/// </summary>
		/// <param name="hProv">HCRYPTPROV handle of a cryptographic service provider (CSP). Set hProv to NULL to use the default CSP. If hProv is not NULL, it must be a CSP handle created using CryptAcquireContext.</param>
		/// <param name="szSubsystemProtocol">String naming a system store.</param>
		/// <returns>If the function succeeds, the return value is a handle to the certificate store.<br>If the function fails, the return value is NULL.</br></returns>
		[DllImport(@"crypt32.dll", EntryPoint="CertOpenSystemStoreA", CharSet=CharSet.Ansi)]	
		internal static extern IntPtr CertOpenSystemStore(IntPtr hProv, string szSubsystemProtocol);
		/// <summary>
		/// The CertDuplicateCertificateContext function duplicates a certificate context by incrementing its reference count.
		/// </summary>
		/// <param name="pCertContext">Pointer to the CERT_CONTEXT structure for which the reference count is incremented.</param>
		/// <returns>Currently, a copy is not made of the context, and the returned pointer to a context has the same value as the pointer to a context that was input. If the pointer passed into this function is NULL, NULL is returned.</returns>
		[DllImport(@"crypt32.dll")]	
		internal static extern IntPtr CertDuplicateCertificateContext(IntPtr pCertContext);
		/// <summary>
		/// The CertDuplicateStore function duplicates a store handle by incrementing the store's reference count.
		/// </summary>
		/// <param name="hCertStore">Handle of the certificate store for which the reference count is being incremented.</param>
		/// <returns>Currently, a copy is not made of the handle, and the returned handle is the same as the handle that was input. If NULL is passed in, the called function will raise an access violation (AV) exception.</returns>
		[DllImport(@"crypt32.dll")]	
		internal static extern IntPtr CertDuplicateStore(IntPtr hCertStore);
		/// <summary>
		/// The PFXImportCertStore function imports a PFX BLOB and returns the handle of a store containing certificates and any associated private keys</summary>
		/// <param name="pPFX">Pointer to a crypt_data_blob strucutre containing a PFX packet with the exported and encrypted certificates and keys</param>
		/// <param name="szPassword">String password used to decrypt and verify the PFX packet</param>
		/// <param name="dwFlags">Flag values. Not used here</param>
		/// <returns>If the function succeeds, the return value is a handle to the certificate store</returns>
		[DllImport(@"crypt32.dll", CharSet=CharSet.Unicode)]
		internal static extern IntPtr PFXImportCertStore(ref DataBlob pPFX, string szPassword, int dwFlags);
		/// <summary>
		/// The PFXImportCertStore function imports a PFX BLOB and returns the handle of a store containing certificates and any associated private keys</summary>
		/// <param name="pPFX">Pointer to a crypt_data_blob strucutre containing a PFX packet with the exported and encrypted certificates and keys</param>
		/// <param name="szPassword">String password used to decrypt and verify the PFX packet</param>
		/// <param name="dwFlags">Flag values. Not used here</param>
		/// <returns>If the function succeeds, the return value is a handle to the certificate store</returns>
		[DllImport(@"crypt32.dll", CharSet=CharSet.Unicode)]
		internal static extern int PFXVerifyPassword(ref DataBlob pPFX, string szPassword, int dwFlags);
		/// <summary>
		/// The PFXIsPFXBlob function attempts to decode the outer layer of a BLOB as a PFX packet.
		/// </summary>
		/// <param name="pPFX">Pointer to a CRYPT_DATA_BLOB structure that the function will attempt to decode as a PFX packet. </param>
		/// <returns>The function returns TRUE if the BLOB can be decoded as a PFX packet. If outer layer of the BLOB cannot be decoded as a PFX packet, the function returns FALSE.</returns>
		[DllImport(@"crypt32.dll")]
		internal static extern int PFXIsPFXBlob(ref DataBlob pPFX);
		/// <summary>
		/// The CertOpenStore function opens a certificate store using a specified store provider type. While this function can open a certificate store for most purposes, CertOpenSystemStore is recommended to open the most common certificate stores. CertOpenStore is required for more complex options and special cases.
		/// </summary>
		/// <param name="lpszStoreProvider">Specifies the store provider type.</param>
		/// <param name="dwMsgAndCertEncodingType">Applicable only to the CERT_STORE_PROV_MSG, CERT_STORE_PROV_PKCS7, or CERT_STORE_PROV_FILENAME provider types. For all other provider types, this parameter is unused and should be set to 0.</param>
		/// <param name="hCryptProv">HCRYPTPROV handle to a cryptographic provider. Passing NULL in this parameter causes an appropriate, default provider to be used. Using the default provider is recommended. The default or specified cryptographic provider is used for all store functions that verify the signature of a subject certificate or CRL.</param>
		/// <param name="dwFlags">These values consist of high-word and low-word values combined using a bitwise-OR operation.</param>
		/// <param name="pvPara">Pointer to a VOID that can point to data of different data types depending on the provider being used. Detailed information about the type and content to be passed in pvPara is specified in the descriptions of the available providers.</param>
		/// <returns>If the function succeeds, the return value is a handle to the certificate store.<br>If the function fails, the return value is NULL. For extended error information, call GetLastError.</br></returns>
		[DllImport(@"crypt32.dll", CharSet=CharSet.Ansi)]	
		internal static extern IntPtr CertOpenStore(IntPtr lpszStoreProvider, int dwMsgAndCertEncodingType, IntPtr hCryptProv, int dwFlags, string pvPara);
		/// <summary>
		/// The CertOpenStore function opens a certificate store using a specified store provider type. While this function can open a certificate store for most purposes, CertOpenSystemStore is recommended to open the most common certificate stores. CertOpenStore is required for more complex options and special cases.
		/// </summary>
		/// <param name="lpszStoreProvider">Specifies the store provider type.</param>
		/// <param name="dwMsgAndCertEncodingType">Applicable only to the CERT_STORE_PROV_MSG, CERT_STORE_PROV_PKCS7, or CERT_STORE_PROV_FILENAME provider types. For all other provider types, this parameter is unused and should be set to 0.</param>
		/// <param name="hCryptProv">HCRYPTPROV handle to a cryptographic provider. Passing NULL in this parameter causes an appropriate, default provider to be used. Using the default provider is recommended. The default or specified cryptographic provider is used for all store functions that verify the signature of a subject certificate or CRL.</param>
		/// <param name="dwFlags">These values consist of high-word and low-word values combined using a bitwise-OR operation.</param>
		/// <param name="pvPara">Pointer to a VOID that can point to data of different data types depending on the provider being used. Detailed information about the type and content to be passed in pvPara is specified in the descriptions of the available providers.</param>
		/// <returns>If the function succeeds, the return value is a handle to the certificate store.<br>If the function fails, the return value is NULL. For extended error information, call GetLastError.</br></returns>
		[DllImport(@"crypt32.dll", EntryPoint="CertOpenStore")]	
		internal static extern IntPtr CertOpenStoreData(IntPtr lpszStoreProvider, int dwMsgAndCertEncodingType, IntPtr hCryptProv, int dwFlags, ref DataBlob pvPara);
		/// <summary>
		/// The CertGetCertificateContextProperty function retrieves the information contained in an extended property of a certificate context.
		/// </summary>
		/// <param name="pCertContext">Pointer to the CERT_CONTEXT of the certificate containing the property to be retrieved. </param>
		/// <param name="dwPropId">Identifies the property to be retrieved.</param>
		/// <param name="pvData">Pointer to a buffer to receive the data as determined by dwPropId. Structures pointed to by members of a structure returned are also returned following the base structure. Therefore, the size contained in pcbData often exceed the size of the base structure.</param>
		/// <param name="pcbData">Pointer to a DWORD value specifying the size, in bytes, of the buffer pointed to by the pvData parameter. When the function returns, the DWORD value contains the number of bytes to be stored in the buffer.</param>
		/// <returns>If the function succeeds, the return value is TRUE.<br/>If the function fails, the return value is FALSE. For extended error information, call GetLastError.</returns>
		[DllImport(@"crypt32.dll")]	
		internal static extern int CertGetCertificateContextProperty(IntPtr pCertContext, int dwPropId, IntPtr pvData, ref int pcbData);
		/// <summary>
		/// The CertGetNameString function obtains the subject or issuer name from a certificate CERT_CONTEXT structure and converts it to a NULL-terminated character string.
		/// </summary>
		/// <param name="pCertContext">Pointer to a CERT_CONTEXT certificate context that includes a subject and issuer name to be converted.</param>
		/// <param name="dwType">DWORD indicating how the name is to be found and how the output is to be formatted.</param>
		/// <param name="dwFlags">Indicates the type of processing needed.</param>
		/// <param name="pvTypePara">Pointer to either a DWORD containing the dwStrType or an object identifier (OID) specifying the name attribute. The type pointed to is determined by the value of dwType.</param>
		/// <param name="pszNameString">Pointer to an allocated buffer to receive the returned string. If pszNameString is not NULL and cchNameString is not zero, pszNameString is a NULL-terminated string.</param>
		/// <param name="cchNameString">Size, in characters, allocated for the returned string. The size must include the terminating NULL character.</param>
		/// <returns>Returns the number of characters converted, including the terminating zero character. If pszNameString is NULL or cchNameString is zero, returns the required size of the destination string (including the terminating NULL character). If the specified name type is not found, returns a NULL-terminated empty string with a returned character count of 1.</returns>
		[DllImport(@"crypt32.dll", EntryPoint="CertGetNameStringA")]	
		internal static extern int CertGetNameString(IntPtr pCertContext, int dwType, int dwFlags, IntPtr pvTypePara, IntPtr pszNameString, int cchNameString);
		/// <summary>
		/// The CryptImportPublicKeyInfo function converts and imports the public key information into the provider and returns a handle of the public key. CryptImportPublicKeyInfoEx provides a revised version of this function.
		/// </summary>
		/// <param name="hCryptProv">Handle of the CSP to use when importing the public key. This handle must have already been created using CryptAcquireContext.</param>
		/// <param name="dwCertEncodingType">Specifies the encoding type used. It is always acceptable to specify both the certificate and message encoding types by combining them with a bitwise-OR operation.</param>
		/// <param name="pInfo">Pointer to a CERT_PUBLIC_KEY_INFO that contains the public key to import into the provider.</param>
		/// <param name="phKey">Pointer to the handle of the imported public key.</param>
		/// <returns>If the function succeeds, the return value is non-zero (TRUE).<br/>If the function fails, the return value is zero (FALSE). For extended error information, call GetLastError.</returns>
		[DllImport(@"crypt32.dll")]	
		internal static extern int CryptImportPublicKeyInfo(IntPtr hCryptProv, int dwCertEncodingType, IntPtr pInfo, ref IntPtr phKey);
		/// <summary>
		/// The CryptExportKey function exports a cryptographic key or a key pair from a cryptographic service provider (CSP) in a secure manner.
		/// </summary>
		/// <param name="hKey">Handle to the key to be exported.</param>
		/// <param name="hExpKey">Handle to a cryptographic key of the destination user. The key data within the exported key BLOB is encrypted using this key. This ensures that only the destination user is able to make use of the key BLOB.</param>
		/// <param name="dwBlobType">Specifies the type of key BLOB to be exported in pbData.</param>
		/// <param name="dwFlags">The following flag values are defined.</param>
		/// <param name="pbData">Pointer to a buffer to receive the key BLOB data.</param>
		/// <param name="pdwDataLen">Pointer to a DWORD value specifying the size, in bytes, of the buffer pointed to by the pbData parameter. When the function returns, the DWORD value contains the number of bytes stored in the buffer.</param>
		/// <returns>If the function succeeds, the return value is nonzero (TRUE).<br/>If the function fails, the return value is zero (FALSE). For extended error information, call GetLastError.</returns>
		[DllImport(@"advapi32.dll", SetLastError=true)]	
		internal static extern int CryptExportKey(int hKey, int hExpKey, int dwBlobType, int dwFlags, IntPtr pbData, ref int pdwDataLen);
		/// <summary>
		/// The CryptAcquireContext function is used to acquire a handle to a particular key container within a particular cryptographic service provider (CSP). This returned handle is used in calls to CryptoAPI functions that use the selected CSP.
		/// </summary>
		/// <param name="phProv">Pointer to a handle of a cryptographic service provider (CSP).</param>
		/// <param name="pszContainer">Key container name.</param>
		/// <param name="pszProvider">NULL terminated string specifying the name of the CSP to be used. </param>
		/// <param name="dwProvType">Specifies the type of provider to acquire.</param>
		/// <param name="dwFlags">Flag values.</param>
		/// <returns>If the function succeeds, the return value is nonzero (TRUE).<br/>If the function fails, the return value is zero (FALSE). For extended error information, call GetLastError.</returns>
		[DllImport(@"advapi32.dll", EntryPoint="CryptAcquireContextA", CharSet=CharSet.Ansi, SetLastError=true)]
		internal static extern int CryptAcquireContext(ref int phProv, IntPtr pszContainer, string pszProvider, int dwProvType, int dwFlags);
		/// <summary>
		/// The CryptReleaseContext function releases the handle of a cryptographic service provider (CSP) and a key container. At each call to this function, the reference count on the CSP is reduced by one. When the reference count reaches zero, the context is fully released and it can no longer be used by any function in the application.
		/// </summary>
		/// <param name="hProv">Handle of a cryptographic service provider (CSP) created by a call to CryptAcquireContext.</param>
		/// <param name="dwFlags">for future use and must be zero.</param>
		/// <returns>If the function succeeds, the return value is nonzero (TRUE).<br/>If the function fails, the return value is zero (FALSE).</returns>
		[DllImport(@"advapi32.dll")]	
		internal static extern int CryptReleaseContext(int hProv, int dwFlags);
		/// <summary>
		/// The CopyMemory function copies a block of memory from one location to another.
		/// </summary>
		/// <param name="Destination">Pointer to the starting address of the copied block's destination.</param>
		/// <param name="Source">Pointer to the starting address of the block of memory to copy.</param>
		/// <param name="Length">Specifies the size, in bytes, of the block of memory to copy.</param>
		[DllImport(@"kernel32.dll", EntryPoint="RtlMoveMemory")]	
		internal static extern void CopyMemory(IntPtr Destination, ref IntPtr Source, int Length);
		/// <summary>
		/// The CertFindCertificateInStore function finds the first or next certificate context in a certificate store that matches a search criteria established by the dwFindType and its associated pvFindPara. This function can be used in a loop to find all of the certificates in a certificate store that match the specified find criteria.
		/// </summary>
		/// <param name="hCertStore">Handle of the certificate store to be searched.</param>
		/// <param name="dwCertEncodingType">Specifies the type of encoding used.</param>
		/// <param name="dwFindFlags">Used with some dwFindType values to modify the search criteria.</param>
		/// <param name="dwFindType">Specifies the type of search being made.</param>
		/// <param name="pvFindPara">Points to a data item or structure used with dwFindType.</param>
		/// <param name="pPrevCertContext">Pointer to the last CERT_CONTEXT structure returned by this function.</param>
		/// <returns>If the function succeeds, the return value is a pointer to a read-only CERT_CONTEXT structure.<br>If the function fails and a certificate that matches the search criteria is not found, the return value is NULL.</br></returns>
		[DllImport(@"crypt32.dll", EntryPoint="CertFindCertificateInStore")]	
		internal static extern IntPtr CertFindUsageCertificateInStore(IntPtr hCertStore, int dwCertEncodingType, int dwFindFlags, int dwFindType, ref TrustListUsage pvFindPara, IntPtr pPrevCertContext);
		/// <summary>
		/// The CertVerifyTimeValidity function verifies the time validity of a certificate.
		/// </summary>
		/// <param name="pTimeToVerify">Pointer to a FILETIME structure containing the comparison time. If NULL, the current time is used.</param>
		/// <param name="pCertInfo">Pointer to the CERT_INFO structure of the certificate for which the time is being verified.</param>
		/// <returns>Returns a minus one if the comparison time is before the NotBefore member of the CERT_INFO structure. Returns a plus one if the comparison time is after the NotAfter member. Returns zero for valid time for the certificate.</returns>
		[DllImport(@"crypt32.dll")]	
		internal static extern int CertVerifyTimeValidity(ref long pTimeToVerify, IntPtr pCertInfo);
		/// <summary>
		/// The CertFindExtension function finds the first extension in the CERT_EXTENSION array, as identified by its object identifier (OID). This function can be used in the processing of a decoded certificate. A CERT_INFO structure is derived from a decoded certificate. The rgExtension array is retrieved from that structure and passed to this function in the rgExtension parameter. This function determines whether a particular extension is in the array, and if so, returns a pointer to it.
		/// </summary>
		/// <param name="pszObjId">Pointer to the object identifier (OID) to use in the search.</param>
		/// <param name="cExtensions">Number of extensions in the rgExtensions array.</param>
		/// <param name="rgExtensions">Array of CERT_EXTENSION structures.</param>
		/// <returns>Returns a pointer to the extension, if one is found. Otherwise, NULL is returned.</returns>
		[DllImport(@"crypt32.dll", EntryPoint="CertFindExtension", CharSet=CharSet.Ansi)]
		internal static extern IntPtr CertFindExtension(string pszObjId, int cExtensions, IntPtr rgExtensions);
		/// <summary>
		/// The CryptDecodeObject function decodes a structure of the type indicated by the lpszStructType parameter. The use of CryptDecodeObjectEx is recommended as an API that performs the same function with significant performance improvements.
		/// </summary>
		/// <param name="dwCertEncodingType">Type of encoding used.</param>
		/// <param name="lpszStructType">Pointer to an OID defining the structure type. If the high-order word of the lpszStructType parameter is zero, the low-order word specifies the integer identifier for the type of the specified structure. Otherwise, this parameter is a long pointer to a NULL-terminated string.</param>
		/// <param name="pbEncoded">Pointer to the encoded structure to be decoded.</param>
		/// <param name="cbEncoded">Number of bytes pointed to by pbEncoded.</param>
		/// <param name="dwFlags">The following flags are defined.</param>
		/// <param name="pvStructInfo">Pointer to a buffer to receive the decoded structure. When the buffer that is specified is not large enough to receive the decoded structure, the function sets the ERROR_MORE_DATA code and stores the required buffer size, in bytes, in the variable pointed to by pcbStructInfo.</param>
		/// <param name="pcbStructInfo">Pointer to a DWORD value specifying the size, in bytes, of the buffer pointed to by the pvStructInfo parameter. When the function returns, this DWORD value contains the size of the decoded data copied to *pvStructInfo. The size contained in the variable pointed to by pcbStructInfo can indicate a size larger than the decoded structure, as the decoded structure can include pointers to other structures. This size is the sum of the size needed by the decoded structure and other structures pointed to.</param>
		/// <returns>If the function succeeds, the return value is nonzero (TRUE).<br/>If the function fails, the return value is zero (FALSE). For extended error information, call GetLastError. Some possible error codes are listed in the following table.</returns>
		[DllImport(@"crypt32.dll")]
		internal static extern int CryptDecodeObject(int dwCertEncodingType, IntPtr lpszStructType, IntPtr pbEncoded, int cbEncoded, int dwFlags, IntPtr pvStructInfo, ref int pcbStructInfo);
		/// <summary>
		/// The CertGetPublicKeyLength function acquires the bit length of public/private keys from a public-key BLOB.
		/// </summary>
		/// <param name="dwCertEncodingType">Specifies the encoding type used.</param>
		/// <param name="pPublicKey">Pointer to the public key BLOB containing the keys for which the length is being retrieved.</param>
		/// <returns>Returns the length of the public/private keys in bits. If unable to determine the key's length, returns zero.</returns>
		[DllImport(@"crypt32.dll")]
		internal static extern int CertGetPublicKeyLength(int dwCertEncodingType, IntPtr pPublicKey);
		/// <summary>
		/// The CertFindRDNAttr function finds the first RDN attribute identified by its object identifier (OID) in a list of the Relative Distinguished Names (RDN).
		/// </summary>
		/// <param name="pszObjId">Pointer to the object identifier (OID) to use In the search.</param>
		/// <param name="pName">Pointer to a CERT_NAME_INFO structure containing the list of the Relative Distinguished Names to be searched.</param>
		/// <returns>Returns a pointer to the attribute, if one is found. Otherwise, NULL is returned.</returns>
		[DllImport(@"crypt32.dll", CharSet=CharSet.Ansi)]
		internal static extern IntPtr CertFindRDNAttr(string pszObjId, IntPtr pName);
		/// <summary>
		/// The CertGetIntendedKeyUsage function acquires the intended key usage bytes from a certificate. The intended key usage can be in either the szOID_KEY_USAGE ("2.5.29.15") or szOID_KEY_ATTRIBUTES ("2.5.29.2") extension.
		/// </summary>
		/// <param name="dwCertEncodingType">Specifies the encoding type used.</param>
		/// <param name="pCertInfo">Pointer to CERT_INFO structure of the specified certificate.</param>
		/// <param name="pbKeyUsage">Pointer to a buffer containing the intended key usage.</param>
		/// <param name="cbKeyUsage">Size, in bytes, of the intended key usage.</param>
		/// <returns>If the certificate does not have any intended key usage bytes, FALSE is returned and pbKeyUsage is zeroed. Otherwise, TRUE is returned and up to cbKeyUsage number of bytes are copied into pbKeyUsage. Any remaining bytes not copied are zeroed.</returns>
		[DllImport(@"crypt32.dll")]
		internal static extern int CertGetIntendedKeyUsage(int dwCertEncodingType, IntPtr pCertInfo, IntPtr pbKeyUsage, int cbKeyUsage);
		/// <summary>
		/// The CertGetEnhancedKeyUsage function returns information from the EKU extension or the EKU extended property of a certificate. EKUs indicate valid uses of the certificate.
		/// </summary>
		/// <param name="pCertContext">Pointer to a CERT_CONTEXT certificate context.</param>
		/// <param name="dwFlags">Indicates whether the function will report on a certificate's extensions, its extended properties, or both. If set to zero (0), the function returns the valid uses of a certificate based on both the certificate's EKU extension and its EKU extended property value.</param>
		/// <param name="pUsage">Pointer to receive a CERT_ENHKEY_USAGE structure (equivalent to a CTL_USAGE structure) indicating the valid uses of the certificate.</param>
		/// <param name="pcbUsage">Pointer to a DWORD that specifies the size, in bytes, of the structure pointed to by pUsage. When the function returns, the DWORD contains the size in bytes of the structure.</param>
		/// <returns>Returns non-zero (TRUE) if the function succeeds and zero (FALSE) if the function fails.</returns>
		[DllImport(@"crypt32.dll")]
		internal static extern int CertGetEnhancedKeyUsage(IntPtr pCertContext, int dwFlags, IntPtr pUsage, ref int pcbUsage);
		/// <summary>
		/// The CertGetCertificateChain function builds a certificate chain context starting from an end certificate and going back, if possible, to a trusted root certificate.
		/// </summary>
		/// <param name="hChainEngine">Handle of the chain engine (name space and cache) to be used. If hChainEngine is NULL, the default chain engine, HCCE_CURRENT_USER, is used. Can be set to HCCE_LOCAL_MACHINE.</param>
		/// <param name="pCertContext">Pointer to the CERT_CONTEXT of the end certificate, the certificate for which a chain is being built. This certificate context will be the zero-index element in the first simple chain.</param>
		/// <param name="pTime">Pointer to a FILETIME variable indicating the time for which the chain is to be validated. Note that the time does not affect trust list, revocation, or root store checking. The current system time is used if NULL is passed to this parameter. Trust in a particular certificate being a trusted root is based on the current state of the root store and not the state of the root store at a time passed in by this parameter. For revocation, a CRL, itself, must be valid at the current time. The process for determining whether a particular certificate listed in a valid CRL is revoked is not the same for Service Pack 4 (SP4) and Windows 2000or later. In Windows 2000 or later, and not in SP4, the value of this parameter is used to determine if a certificate listed in a CRL has been revoked.</param>
		/// <param name="hAdditionalStore">Handle of any additional store to searched for supporting certificates and CTLs. This parameter can be NULL if no additional store is to be searched.</param>
		/// <param name="pChainPara">Pointer to a CERT_CHAIN_PARA structure that includes chain-building parameters.</param>
		/// <param name="dwFlags">Flag values indicating special processing.</param>
		/// <param name="pvReserved">Reserved parameter, must be NULL.</param>
		/// <param name="ppChainContext">Pointer to a pointer to the chain context created.</param>
		/// <returns>Returns TRUE (non-zero) if the function succeeds or FALSE (zero) if the function fails. For extended error information, call GetLastError.</returns>
		[DllImport(@"crypt32.dll")]
		internal static extern int CertGetCertificateChain(IntPtr hChainEngine, IntPtr pCertContext, IntPtr pTime, IntPtr hAdditionalStore, ref ChainParameters pChainPara, int dwFlags, IntPtr pvReserved, ref IntPtr ppChainContext);
		/// <summary>
		/// The CertFreeCertificateChain function frees a certificate chain by reducing its reference count. If the reference count becomes zero, memory allocated for the chain is released.
		/// </summary>
		/// <param name="pChainContext">Pointer to a CERT_CHAIN_CONTEXT certificate chain context to be freed. If the reference count on the context reaches zero, the storage allocated for the context is freed.</param>
		[DllImport(@"crypt32.dll")]
		internal static extern void CertFreeCertificateChain(IntPtr pChainContext);
		/// <summary>
		/// The CertVerifyCertificateChainPolicy function checks a certificate chain to verify its validity, including its compliance with any specified validity policy criteria.
		/// </summary>
		/// <param name="pszPolicyOID">Current predefined verify chain policy structures are listed in the following table.</param>
		/// <param name="pChainContext">Pointer to a CERT_CHAIN_CONTEXT structure containing a chain to be verified.</param>
		/// <param name="pPolicyPara">Pointer to a CERT_CHAIN_POLICY_PARA structure that provides the policy verification criteria for the chain. The dwFlags member of that structure can be set to change the default policy checking behavior.</param>
		/// <param name="pPolicyStatus">Pointer to a CERT_CHAIN_POLICY_STATUS structure where status information on the chain is returned. OID-specific extra status can be returned in the pvExtraPolicyStatus member of this structure.</param>
		/// <returns>If the chain can be verified for the specified policy, TRUE is returned and the dwError member of the pPolicyStatus is updated. A dwError of 0 (ERROR_SUCCESS or S_OK) indicates the chain satisfies the specified policy. If the chain cannot be validated, FALSE is returned.</returns>
		[DllImport(@"crypt32.dll")]
		internal static extern int CertVerifyCertificateChainPolicy(IntPtr pszPolicyOID, IntPtr pChainContext, ref ChainPolicyParameters pPolicyPara, ref ChainPolicyStatus pPolicyStatus);
		/// <summary>
		/// The CertGetIssuerCertificateFromStore function retrieves the certificate context from the certificate store for the first or next issuer of the specified subject certificate. The new Certificate Chain Verification Functions are recommended instead of the use of this function.
		/// </summary>
		/// <param name="hCertStore">Handle of a certificate store.</param>
		/// <param name="pSubjectContext">Pointer to a CERT_CONTEXT structure containing the subject information. This parameter can be obtained from any certificate store or can be created by the calling application using the CertCreateCertificateContext function.</param>
		/// <param name="pPrevIssuerContext">Pointer to a CERT_CONTEXT structure containing the issuer information. An issuer can have multiple certificates, especially when a validity period is about to change. This parameter must be NULL on the call to get the first issuer certificate. To get the next certificate for the issuer, set pPrevIssuerContext to the CERT_CONTEXT structure returned by the previous call.</param>
		/// <param name="pdwFlags">The following flags enable verification checks on the returned certificate. They can be combined using a bitwise-OR operation to enable multiple verifications.</param>
		/// <returns>If the function succeeds, the return value is a pointer to a read-only issuer CERT_CONTEXT.<br>If the function fails and the first or next issuer certificate is not found, the return value is NULL.</br></returns>
		[DllImport(@"crypt32.dll")]
		internal static extern IntPtr CertGetIssuerCertificateFromStore(IntPtr hCertStore, IntPtr pSubjectContext, IntPtr pPrevIssuerContext, ref int pdwFlags);
		/// <summary>
		/// The CryptAcquireCertificatePrivateKey function acquires a HCRYPTPROV cryptographic service provider (CSP) handle including access to its related key container and the dwKeySpec for a user's specified certificate context. This function is used to get access to a user's private key when the user's certificate is available and the handle of a CSP with the user's key container is not available. This function can be used by the owner of a private key and not by any other user.
		/// </summary>
		/// <param name="pCert">Pointer to a CERT_CONTEXT structure containing the specified certificate context. This is the certificate context for which a private key will be acquired.</param>
		/// <param name="dwFlags">The flags.</param>
		/// <param name="pvReserved">Reserved for future use and must be NULL.</param>
		/// <param name="phCryptProv">Pointer to the returned HCRYPTPROV value. Depending on the dwFlags value, the HCRYPTPROV value must be freed by the calling application or explicitly freed on the last free action of the certificate context.</param>
		/// <param name="pdwKeySpec">Pointer to a DWORD value identifying the private key to use from the acquired provider's key container. It can be AT_KEYEXCHANGE or AT_SIGNATURE.</param>
		/// <param name="pfCallerFreeProv">Pointer to a BOOL flag.</param>
		/// <returns>If the function succeeds, the return value is nonzero (TRUE).<br>If the function fails, the return value is zero (FALSE).</br></returns>
		[DllImport(@"crypt32.dll")]
		internal static extern int CryptAcquireCertificatePrivateKey(IntPtr pCert, int dwFlags, IntPtr pvReserved, ref int phCryptProv, ref int pdwKeySpec, ref int pfCallerFreeProv);
		/// <summary>
		/// The CertGetValidUsages function returns an array of usages consisting of the intersection of the valid usages for all certificates in an array of certificates.
		/// </summary>
		/// <param name="cCerts">Number of certificates in the array to be checked.</param>
		/// <param name="rghCerts">Array of certificates to be checked for valid usage.</param>
		/// <param name="cNumOIDs">Number of valid usages found as the intersection of the valid usages of all certificates in the array. If all of the certificates are valid for all usages, cNumOIDS is set to negative one ( -1).</param>
		/// <param name="rghOIDs">Array of the OIDs of the valid usages that are shared by all of the certificates in the rghCerts array. This parameter can be NULL to set the size of this structure for memory allocation purposes. For more information, see Retrieving Data of Unknown Length.</param>
		/// <param name="pcbOIDs">Pointer to a DWORD value that specifies the size, in bytes, of the rghOIDs array and the strings pointed to. When the function returns, the DWORD value contains the number of bytes needed for the array.</param>
		/// <returns>If the function succeeds, the return value is nonzero (TRUE) .If the function fails, the return value is zero (FALSE). For extended error information, call GetLastError.</returns>
		[DllImport(@"crypt32.dll")]
		internal static extern int CertGetValidUsages(int cCerts, IntPtr rghCerts, ref int cNumOIDs, IntPtr rghOIDs, ref int pcbOIDs);
		/// <summary>
		/// The CertAddCertificateContextToStore function adds a certificate context to the certificate store.
		/// </summary>
		/// <param name="hCertStore">Handle of a certificate store.</param>
		/// <param name="pCertContext">Pointer to the CERT_CONTEXT structure to be added to the store.</param>
		/// <param name="dwAddDisposition">Specifies the action to take if a matching certificate or a link to a matching certificate already exists in the store.</param>
		/// <param name="ppStoreContext">Pointer to a pointer to the copy to be made of the certificate that was added to the store.</param>
		/// <returns>If the function succeeds, the return value is TRUE.<br>If the function fails, the return value is FALSE.</br></returns>
		[DllImport(@"crypt32.dll")]
		internal static extern int CertAddCertificateContextToStore(IntPtr hCertStore, IntPtr pCertContext, int dwAddDisposition, IntPtr ppStoreContext);
		/// <summary>
		/// The CertDeleteCertificateFromStore function deletes the specified certificate context from the certificate store.
		/// </summary>
		/// <param name="pCertContext">Pointer to the CERT_CONTEXT structure to be deleted.</param>
		/// <returns>If the function succeeds, the return value is TRUE.<br>If the function fails, the return value is FALSE.</br></returns>
		[DllImport(@"crypt32.dll")]
		internal static extern int CertDeleteCertificateFromStore(IntPtr pCertContext);
		/// <summary>
		/// The PFXExportCertStoreEx function exports the certificates and, if available, their associated private keys from the referenced certificate store.
		/// </summary>
		/// <param name="hStore">Handle of the certificate store containing the certificates to be exported.</param>
		/// <param name="pPFX">Pointer to a CRYPT_DATA_BLOB structure to contain the PFX packet with the exported certificates and keys. If pPFX->pbData is NULL, the function calculates the number of bytes needed for the encoded BLOB and returns this in pPFX->cbData. When the function is called with pPFX-pdData pointing an allocated buffer of the needed size, the function copies the encoded bytes into the buffer and updates pPFX->cbData with the encode byte length.</param>
		/// <param name="szPassword">String password used to encrypt and verify the PFX packet.</param>
		/// <param name="pvReserved"> Reserved for future use. Currently, this parameter must be NULL.</param>
		/// <param name="dwFlags">Flag values.</param>
		/// <returns>Returns TRUE (non-zero) if the function succeeds, and FALSE (zero) if the function fails. For extended error information, call GetLastError.</returns>
		[DllImport(@"crypt32.dll", CharSet=CharSet.Unicode)]
		internal static extern int PFXExportCertStoreEx(IntPtr hStore, ref DataBlob pPFX, string szPassword, IntPtr pvReserved, int dwFlags);
		/// <summary>
		/// The CertCompareCertificate function compares two certificates to determine whether or not they are identical.
		/// </summary>
		/// <param name="dwCertEncodingType">Specifies the encoding type used.</param>
		/// <param name="pCertId1">Pointer to the CERT_INFO for the first certificate in the comparison.</param>
		/// <param name="pCertId2">Pointer to the CERT_INFO for the second certificate in the comparison.</param>
		/// <returns></returns>
		[DllImport(@"crypt32.dll")]
		internal static extern int CertCompareCertificate(int dwCertEncodingType, IntPtr pCertId1, IntPtr pCertId2);
		/// <summary>
		/// The CertSaveStore function saves the certificate store to a file or to a memory BLOB.
		/// </summary>
		/// <param name="hCertStore">Handle of the certificate store to be saved.</param>
		/// <param name="dwMsgAndCertEncodingType">Specifies the certificate and message encoding types.</param>
		/// <param name="dwSaveAs">Specifies whether the store can be saved as a serialized store containing properties in addition to encoded certificates, CRL, and CTLs (CERT_STORE_SAVE_AS_STORE), or as a PKCS #7 signed message that does not include additional properties (CERT_STORE_SAVE_AS_PKCS7).</param>
		/// <param name="dwSaveTo">Along with the pvSaveToPara parameter, specifies where to save the store.</param>
		/// <param name="pvSaveToPara">Pointer used to save the store. May be a file handle or a pointer to a MEMORY_BLOB structure.</param>
		/// <param name="dwFlags">Reserved for future use and must be zero.</param>
		/// <returns>If the function succeeds, the return value is TRUE.<br>If the function fails, the return value is FALSE. For extended error information, call GetLastError.</br></returns>
		[DllImport(@"crypt32.dll")]
		internal static extern int CertSaveStore(IntPtr hCertStore, int dwMsgAndCertEncodingType, int dwSaveAs, int dwSaveTo, ref DataBlob pvSaveToPara, int dwFlags);
		/// <summary>
		/// The CertCreateCertificateContext function creates a certificate context from an encoded certificate. The created context is not persisted to a certificate store. The function makes a copy of the encoded certificate within the created context.
		/// </summary>
		/// <param name="dwCertEncodingType">Specifies the type of encoding used.</param>
		/// <param name="pbCertEncoded">Pointer to a buffer containing the encoded certificate from which the context is to be created.</param>
		/// <param name="cbCertEncoded">Size, in bytes, of the pbCertEncoded buffer.</param>
		/// <returns>If the function succeeds, the return value is a pointer to a read-only CERT_CONTEXT.<br>If the function is unable to decode and create the certificate context, the return value is NULL. For extended error information, call GetLastError. Some possible error codes follow.</br></returns>
		[DllImport(@"crypt32.dll")]
		internal static extern IntPtr CertCreateCertificateContext(int dwCertEncodingType, IntPtr pbCertEncoded, int cbCertEncoded);
		/// <summary>
		/// The CryptGenKey function generates a random cryptographic session key or a public/private key pair. A handle to the key or key pair is returned in phKey. This handle can then be used as needed with any CryptoAPI function requiring a key handle.
		/// </summary>
		/// <param name="hProv">HCRYPTPROV handle of a cryptographic service provider (CSP) created by a call to CryptAcquireContext.</param>
		/// <param name="Algid">ALG_ID structure identifying the algorithm for which the key is to be generated. Values for this parameter vary depending on the CSP used.</param>
		/// <param name="dwFlags">Specifies the type of key generated.</param>
		/// <param name="phKey">Address to which the function copies the handle of the newly generated key.</param>
		/// <returns>If the function succeeds, the return value is nonzero (TRUE).<br>If the function fails, the return value is zero (FALSE). For extended error information, call GetLastError.</br></returns>
		[DllImport(@"advapi32.dll", SetLastError=true)]
		internal static extern int CryptGenKey(int hProv, IntPtr Algid, int dwFlags, ref int phKey);
		/// <summary>
		/// The CryptDestroyKey function releases the handle referenced by the hKey parameter. After a key handle has been released, it becomes invalid and cannot be used again.
		/// </summary>
		/// <param name="hKey">Handle of the key to be destroyed.</param>
		/// <returns>If the function succeeds, the return value is nonzero (TRUE).<br>If the function fails, the return value is zero (FALSE).</br></returns>
		[DllImport(@"advapi32.dll")]
		internal static extern int CryptDestroyKey(int hKey);
		/// <summary>
		/// The CryptImportKey function transfers a cryptographic key from a key BLOB into a cryptographic service provider (CSP).This function can be used to import an Schannel session key, regular session key, public key, or public/private key pair. For all but the public key, the key or key pair is encrypted.
		/// </summary>
		/// <param name="hProv">HCRYPTPROV handle of a cryptographic service provider (CSP) created by a call to CryptAcquireContext.</param>
		/// <param name="pbData">BYTE sequence containing the key CRYPTOAPI_BLOB. This key BLOB was generated by the CryptExportKey function, either in this application or by another application possibly running on a different computer.</param>
		/// <param name="dwDataLen">Length, in bytes, of the key BLOB.</param>
		/// <param name="hPubKey">The meaning of this parameter differs depending on the CSP type and the type of key BLOB being imported.</param>
		/// <param name="dwFlags">Currently used only when a public/private key pair in the form of a PRIVATEKEYBLOB is imported into the CSP.</param>
		/// <param name="phKey">Pointer to the handle of the imported key.</param>
		/// <returns>If the function succeeds, the return value is nonzero (TRUE).<br>If the function fails, the return value is zero (FALSE).</br></returns>
		[DllImport(@"advapi32.dll", SetLastError=true)]
		internal static extern int CryptImportKey(int hProv, IntPtr pbData, int dwDataLen, int hPubKey, int dwFlags, ref int phKey);
		/// <summary>
		/// The CryptImportKey function transfers a cryptographic key from a key BLOB into a cryptographic service provider (CSP).This function can be used to import an Schannel session key, regular session key, public key, or public/private key pair. For all but the public key, the key or key pair is encrypted.
		/// </summary>
		/// <param name="hProv">HCRYPTPROV handle of a cryptographic service provider (CSP) created by a call to CryptAcquireContext.</param>
		/// <param name="pbData">BYTE sequence containing the key CRYPTOAPI_BLOB. This key BLOB was generated by the CryptExportKey function, either in this application or by another application possibly running on a different computer.</param>
		/// <param name="dwDataLen">Length, in bytes, of the key BLOB.</param>
		/// <param name="hPubKey">The meaning of this parameter differs depending on the CSP type and the type of key BLOB being imported.</param>
		/// <param name="dwFlags">Currently used only when a public/private key pair in the form of a PRIVATEKEYBLOB is imported into the CSP.</param>
		/// <param name="phKey">Pointer to the handle of the imported key.</param>
		/// <returns>If the function succeeds, the return value is nonzero (TRUE).<br>If the function fails, the return value is zero (FALSE).</br></returns>
		[DllImport(@"advapi32.dll", SetLastError=true)]
		internal static extern int CryptImportKey(int hProv, byte[] pbData, int dwDataLen, int hPubKey, int dwFlags, ref int phKey);
		/// <summary>
		/// The CryptGetKeyParam function retrieves data that governs the operations of a key. If the Microsoft Cryptographic Service Provider is used, the base symmetric keying material is not obtainable by this function or any other function.
		/// </summary>
		/// <param name="hKey">Handle to the key being queried.</param>
		/// <param name="dwParam">Specifies the query being made.</param>
		/// <param name="pbData">Pointer to a sequence of BYTES to receive the data. The function returns the specified data in this buffer. The form of this data depends on the value of dwParam.</param>
		/// <param name="pdwDataLen">Pointer to a DWORD value specifying the size, in bytes, of the buffer pointed to by the pbData parameter. When the function returns, the DWORD value contains the number of bytes stored in the buffer.</param>
		/// <param name="dwFlags">Reserved for future use and must be zero.</param>
		/// <returns>If the function succeeds, the return value is nonzero (TRUE).<br>If the function fails, the return value is zero (FALSE).</br></returns>
		[DllImport(@"advapi32.dll")]
		internal static extern int CryptGetKeyParam(int hKey, int dwParam, ref int pbData, ref int pdwDataLen, int dwFlags);
		/// <summary>
		/// The CryptGetKeyParam function retrieves data that governs the operations of a key. If the Microsoft Cryptographic Service Provider is used, the base symmetric keying material is not obtainable by this function or any other function.
		/// </summary>
		/// <param name="hKey">Handle to the key being queried.</param>
		/// <param name="dwParam">Specifies the query being made.</param>
		/// <param name="pbData">Pointer to a sequence of BYTES to receive the data. The function returns the specified data in this buffer. The form of this data depends on the value of dwParam.</param>
		/// <param name="pdwDataLen">Pointer to a DWORD value specifying the size, in bytes, of the buffer pointed to by the pbData parameter. When the function returns, the DWORD value contains the number of bytes stored in the buffer.</param>
		/// <param name="dwFlags">Reserved for future use and must be zero.</param>
		/// <returns>If the function succeeds, the return value is nonzero (TRUE).<br>If the function fails, the return value is zero (FALSE).</br></returns>
		[DllImport(@"advapi32.dll")]
		internal static extern int CryptGetKeyParam(int hKey, int dwParam, byte[] pbData, ref int pdwDataLen, int dwFlags);
		/// <summary>
		/// The CryptGetKeyParam function retrieves data that governs the operations of a key. If the Microsoft Cryptographic Service Provider is used, the base symmetric keying material is not obtainable by this function or any other function.
		/// </summary>
		/// <param name="hKey">Handle to the key being queried.</param>
		/// <param name="dwParam">Specifies the query being made.</param>
		/// <param name="pbData">Pointer to a sequence of BYTES to receive the data. The function returns the specified data in this buffer. The form of this data depends on the value of dwParam.</param>
		/// <param name="pdwDataLen">Pointer to a DWORD value specifying the size, in bytes, of the buffer pointed to by the pbData parameter. When the function returns, the DWORD value contains the number of bytes stored in the buffer.</param>
		/// <param name="dwFlags">Reserved for future use and must be zero.</param>
		/// <returns>If the function succeeds, the return value is nonzero (TRUE).<br>If the function fails, the return value is zero (FALSE).</br></returns>
		[DllImport(@"advapi32.dll")]
		internal static extern int CryptGetKeyParam(int hKey, int dwParam, ref IntPtr pbData, ref int pdwDataLen, int dwFlags);
		/// <summary>
		/// The CryptGetProvParam function retrieves parameters that govern the operations of a cryptographic service provider (CSP).
		/// </summary>
		/// <param name="hProv">Handle of the cryptographic service provider (CSP) target of the query. This handle must have been created using CryptAcquireContext.</param>
		/// <param name="dwParam">Specifies the nature of the query.</param>
		/// <param name="pbData">Pointer to a buffer to receive the data. The form of this data varies depending on the value of dwParam.</param>
		/// <param name="pdwDataLen">Pointer to a DWORD value specifying the size, in bytes, of the buffer pointed to by the pbData parameter. When the function returns, the DWORD value contains the number of bytes stored or to be stored in the buffer.</param>
		/// <param name="dwFlags">If dwParam is one of the enumeration values (PP_ENUMALGS, PP_ENUMALGS_EX, or PP_ENUMCONTAINERS), the CRYPT_FIRST flag can be specified. If this flag is set, the first item in the enumeration list is returned. If this flag is not set, the next item in the list is returned.</param>
		/// <returns>If the function succeeds, the return value is nonzero (TRUE).<br>If the function fails, the return value is zero (FALSE).</br></returns>
		[DllImport(@"advapi32.dll")]
		internal static extern int CryptGetProvParam(int hProv, int dwParam, IntPtr pbData, ref int pdwDataLen, int dwFlags);
		/// <summary>
		/// The CryptGenRandom function fills a buffer with cryptographically random bytes.
		/// </summary>
		/// <param name="hProv">HCRYPTPROV handle of a cryptographic service provider (CSP) created by a call to CryptAcquireContext.</param>
		/// <param name="dwLen">Number of bytes of random data to be generated.</param>
		/// <param name="pbBuffer">Buffer to receive the returned data. This buffer must be at least dwLen bytes in length.</param>
		/// <returns>If the function succeeds, the return value is nonzero (TRUE).<br>If the function fails, the return value is zero (FALSE).</br></returns>
		[DllImport(@"advapi32.dll")]
		internal static extern int CryptGenRandom(int hProv, int dwLen, IntPtr pbBuffer);
		/// <summary>
		/// The CryptSetKeyParam function customizes various aspects of a session key's operations. The values set by this function are not persisted to memory and can only be used with in a single session.
		/// </summary>
		/// <param name="hKey">Handle to the key for which values are to be set.</param>
		/// <param name="dwParam">The following tables contain predefined values that can be used.</param>
		/// <param name="pbData">Pointer to a buffer initialized with the value to be set before calling CryptSetKeyParam. The form of this data varies depending on the value of dwParam.</param>
		/// <param name="dwFlags">Used only when dwParam is KP_ALGID. The dwFlags parameter is used to pass in flag values for the enabled key. The dwFlags parameter can hold values such as the key size and the other flag values allowed when generating the same type of key with CryptGenKey. For information on allowable flag values, see CryptGenKey.</param>
		/// <returns>If the function succeeds, the return value is nonzero (TRUE).<br>If the function fails, the return value is zero (FALSE).</br></returns>
		[DllImport(@"advapi32.dll", SetLastError=true)]
		internal static extern int CryptSetKeyParam(int hKey, int dwParam, byte[] pbData, int dwFlags);
		/// <summary>
		/// The CryptSetKeyParam function customizes various aspects of a session key's operations. The values set by this function are not persisted to memory and can only be used with in a single session.
		/// </summary>
		/// <param name="hKey">Handle to the key for which values are to be set.</param>
		/// <param name="dwParam">The following tables contain predefined values that can be used.</param>
		/// <param name="pbData">Pointer to a buffer initialized with the value to be set before calling CryptSetKeyParam. The form of this data varies depending on the value of dwParam.</param>
		/// <param name="dwFlags">Used only when dwParam is KP_ALGID. The dwFlags parameter is used to pass in flag values for the enabled key. The dwFlags parameter can hold values such as the key size and the other flag values allowed when generating the same type of key with CryptGenKey. For information on allowable flag values, see CryptGenKey.</param>
		/// <returns>If the function succeeds, the return value is nonzero (TRUE).<br>If the function fails, the return value is zero (FALSE).</br></returns>
		[DllImport(@"advapi32.dll", SetLastError=true)]
		internal static extern int CryptSetKeyParam(int hKey, int dwParam, ref int pbData, int dwFlags);
		/// <summary>
		/// The CryptEncrypt function encrypts data. The algorithm used to encrypt the data is designated by the key held by the CSP module and is referenced by the hKey parameter.
		/// </summary>
		/// <param name="hKey">Handle to the encryption key. An application obtains this handle by using either the CryptGenKey or the CryptImportKey function. The key specifies the encryption algorithm used.</param>
		/// <param name="hHash">Handle to a hash object. If data is to be hashed and encrypted simultaneously, a handle to a hash object can be passed in the hHash parameter. The hash value is updated with the plaintext passed in. This option is useful when generating signed and encrypted text.</param>
		/// <param name="Final">Specifying whether this is the last section in a series being encrypted. Final is set TRUE for the last or only block and FALSE if there are more blocks to be encrypted. For more information, see the Remarks section later in this topic.</param>
		/// <param name="dwFlags">The following dwFlags value is defined but reserved for a future release: CRYPT_OAEP</param>
		/// <param name="pbData">Pointer to a buffer holding the data to be encrypted. The encrypted data overwrites the data to be encrypted in this buffer.</param>
		/// <param name="pdwDataLen">Pointer to a DWORD value holding the length of the data buffer. Before calling this function, the DWORD value is set to the number of bytes to be encrypted. Upon return, the DWORD value contains the number of bytes needed to hold the encrypted data.</param>
		/// <param name="dwBufLen">The length in bytes of the input pbData buffer.</param>
		/// <returns>If the function succeeds, the return value is TRUE. <br>If the function fails, the return value is FALSE.</br></returns>
		[DllImport(@"advapi32.dll", SetLastError=true)]
		internal static extern int CryptEncrypt(int hKey, int hHash, int Final, int dwFlags, IntPtr pbData, ref int pdwDataLen, int dwBufLen);
		/// <summary>
		/// The CryptEncrypt function encrypts data. The algorithm used to encrypt the data is designated by the key held by the CSP module and is referenced by the hKey parameter.
		/// </summary>
		/// <param name="hKey">Handle to the encryption key. An application obtains this handle by using either the CryptGenKey or the CryptImportKey function. The key specifies the encryption algorithm used.</param>
		/// <param name="hHash">Handle to a hash object. If data is to be hashed and encrypted simultaneously, a handle to a hash object can be passed in the hHash parameter. The hash value is updated with the plaintext passed in. This option is useful when generating signed and encrypted text.</param>
		/// <param name="Final">Specifying whether this is the last section in a series being encrypted. Final is set TRUE for the last or only block and FALSE if there are more blocks to be encrypted. For more information, see the Remarks section later in this topic.</param>
		/// <param name="dwFlags">The following dwFlags value is defined but reserved for a future release: CRYPT_OAEP</param>
		/// <param name="pbData">Pointer to a buffer holding the data to be encrypted. The encrypted data overwrites the data to be encrypted in this buffer.</param>
		/// <param name="pdwDataLen">Pointer to a DWORD value holding the length of the data buffer. Before calling this function, the DWORD value is set to the number of bytes to be encrypted. Upon return, the DWORD value contains the number of bytes needed to hold the encrypted data.</param>
		/// <param name="dwBufLen">The length in bytes of the input pbData buffer.</param>
		/// <returns>If the function succeeds, the return value is TRUE. <br>If the function fails, the return value is FALSE.</br></returns>
		[DllImport(@"advapi32.dll")]
		internal static extern int CryptEncrypt(int hKey, int hHash, int Final, int dwFlags, byte[] pbData, ref int pdwDataLen, int dwBufLen);
		/// <summary>
		/// The CryptDecrypt function decrypts data previously encrypted using CryptEncrypt function.
		/// </summary>
		/// <param name="hKey">Handle to the key to use for the decryption. An application obtains this handle by using either the CryptGenKey or CryptImportKey function. This key specifies the decryption algorithm to be used.</param>
		/// <param name="hHash">Handle to a hash object. If data is to be decrypted and hashed simultaneously, a handle to a hash object is passed in this parameter. The hash value is updated with the decrypted plaintext. This option is useful when simultaneously decrypting and verifying a signature.</param>
		/// <param name="Final">Specifies whether this is the last section in a series being decrypted. This value is TRUE if this is the last or only block. If it is not the last block, it is FALSE. For more information, see the Remarks section later in this topic.</param>
		/// <param name="dwFlags">The following dwFlags value is defined but reserved for a future release: CRYPT_OAEP</param>
		/// <param name="pbData">Buffer holding the data to be decrypted. After the decryption has been performed, the plaintext is placed back in this same buffer. The number of encrypted bytes in this buffer is specified by pdwDataLen.</param>
		/// <param name="pdwDataLen">Pointer to a DWORD value indicating the length of the pbData buffer. Before calling this function, the calling application sets the DWORD value to the number of bytes to be decrypted. Upon return, the DWORD value contains the number of bytes of the decrypted plaintext.</param>
		/// <returns>If the function succeeds, the return value is nonzero (TRUE).<br>If the function fails, the return value is zero (FALSE).</br></returns>
		[DllImport(@"advapi32.dll")]
		internal static extern int CryptDecrypt(int hKey, int hHash, int Final, int dwFlags, byte[] pbData, ref int pdwDataLen);




		/// <summary>
		/// The DeleteSecurityContext function deletes the local data structures associated with the specified security context.
		/// </summary>
		/// <param name="phContext">Handle of the security context to delete.</param>
		/// <returns>If the function succeeds, the return value is SEC_E_OK.<br>If the function fails, the return value can be the following error code: SEC_E_INVALID_HANDLE</br></returns>
		internal static int DeleteSecurityContext(IntPtr phContext) {
			if (IsNT4)
				return SspiNt4Provider.DeleteSecurityContext(phContext);
			else
				return SspiNormalProvider.DeleteSecurityContext(phContext);
		}
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
		internal static int InitializeSecurityContext(IntPtr phCredential, IntPtr phContext, string pszTargetName, int fContextReq, int Reserved1, int TargetDataRep, IntPtr pInput, int Reserved2, IntPtr phNewContext, IntPtr pOutput, ref int pfContextAttr, IntPtr ptsExpiry) {
			if (IsNT4)
				return SspiNt4Provider.InitializeSecurityContext(phCredential, phContext, pszTargetName, fContextReq, Reserved1, TargetDataRep, pInput, Reserved2, phNewContext, pOutput, ref pfContextAttr, ptsExpiry);
			else
				return SspiNormalProvider.InitializeSecurityContext(phCredential, phContext, pszTargetName, fContextReq, Reserved1, TargetDataRep, pInput, Reserved2, phNewContext, pOutput, ref pfContextAttr, ptsExpiry);
		}
		/// <summary>
		/// The FreeContextBuffer function enables callers of security package functions to free a memory buffer allocated by the security package. For example, the InitializeSecurityContext function may allocate a buffer for returning the outbound context token.
		/// </summary>
		/// <param name="pvContextBuffer">Pointer to memory allocated by the security package.</param>
		/// <returns>If the function succeeds, the return value is SEC_E_OK.<br>If the function fails, the return value is a nonzero error code.</br></returns>
		internal static int FreeContextBuffer(IntPtr pvContextBuffer) {
			if (IsNT4)
				return SspiNt4Provider.FreeContextBuffer(pvContextBuffer);
			else
				return SspiNormalProvider.FreeContextBuffer(pvContextBuffer);
		}
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
		internal static int AcceptSecurityContext(IntPtr phCredential, IntPtr phContext, IntPtr pInput, int fContextReq, int TargetDataRep, IntPtr phNewContext, IntPtr pOutput, ref int pfContextAttr, IntPtr ptsExpiry) {
			if (IsNT4)
				return SspiNt4Provider.AcceptSecurityContext(phCredential, phContext, pInput, fContextReq, TargetDataRep, phNewContext, pOutput, ref pfContextAttr, ptsExpiry);
			else
				return SspiNormalProvider.AcceptSecurityContext(phCredential, phContext, pInput, fContextReq, TargetDataRep, phNewContext, pOutput, ref pfContextAttr, ptsExpiry);
		}
		/// <summary>
		/// The FreeCredentialsHandle function notifies the security system that the credentials are no longer needed. An application calls this function to free the credential handle acquired in the call to the AcquireCredentialsHandle function. When all references to this credential set have been removed, the credentials themselves can be removed.
		/// </summary>
		/// <param name="phCredential">Pointer to the credential handle obtained by using the AcquireCredentialsHandle function.</param>
		/// <returns>If the function succeeds, the return value is SEC_E_OK.<br>If the function fails, the return value may be the following error code: SEC_E_INVALID_HANDLE</br></returns>
		internal static int FreeCredentialsHandle(IntPtr phCredential) {
			if (IsNT4)
				return SspiNt4Provider.FreeCredentialsHandle(phCredential);
			else
				return SspiNormalProvider.FreeCredentialsHandle(phCredential);
		}
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
		internal static int AcquireCredentialsHandle(string pszPrincipal, string pszPackage, int fCredentialUse, IntPtr pvLogonID, ref SCHANNEL_CRED pAuthData, IntPtr pGetKeyFn, IntPtr pvGetKeyArgument, IntPtr phCredential, IntPtr ptsExpiry) {
			if (IsNT4)
				return SspiNt4Provider.AcquireCredentialsHandle(pszPrincipal, pszPackage, fCredentialUse, pvLogonID, ref pAuthData, pGetKeyFn, pvGetKeyArgument, phCredential, ptsExpiry);
			else
				return SspiNormalProvider.AcquireCredentialsHandle(pszPrincipal, pszPackage, fCredentialUse, pvLogonID, ref pAuthData, pGetKeyFn, pvGetKeyArgument, phCredential, ptsExpiry);
		}
		/// <summary>
		/// The QueryContextAttributes function retrieves information about attributes or features supported by a specified security context.
		/// </summary>
		/// <param name="phContext">Handle to the security context to be queried.</param>
		/// <param name="ulAttribute">Specifies which context attribute is returned.</param>
		/// <param name="pBuffer">Pointer to a structure containing the requested attribute. The ulAttribute parameter description indicates the structure returned for each attribute.</param>
		/// <returns>If the function succeeds, the return value is SEC_E_OK.<br>If the function fails, the return value is a nonzero error code.</br></returns>
		internal static int QueryContextAttributesSize(IntPtr phContext, int ulAttribute, ref SecPkgContext_StreamSizes pBuffer) {
			if (IsNT4)
				return SspiNt4Provider.QueryContextAttributesSize(phContext, ulAttribute, ref pBuffer);
			else
				return SspiNormalProvider.QueryContextAttributesSize(phContext, ulAttribute, ref pBuffer);
		}
		/// <summary>
		/// The QueryContextAttributes function retrieves information about attributes or features supported by a specified security context.
		/// </summary>
		/// <param name="phContext">Handle to the security context to be queried.</param>
		/// <param name="ulAttribute">Specifies which context attribute is returned.</param>
		/// <param name="pBuffer">Pointer to a structure containing the requested attribute. The ulAttribute parameter description indicates the structure returned for each attribute.</param>
		/// <returns>If the function succeeds, the return value is SEC_E_OK.<br>If the function fails, the return value is a nonzero error code.</br></returns>
		internal static int QueryContextAttributesCertificate(IntPtr phContext, int ulAttribute, ref IntPtr pBuffer) {
			if (IsNT4)
				return SspiNt4Provider.QueryContextAttributesCertificate(phContext, ulAttribute, ref pBuffer);
			else
				return SspiNormalProvider.QueryContextAttributesCertificate(phContext, ulAttribute, ref pBuffer);
		}
		/// <summary>
		/// The EncryptMessage function encrypts a message using the session key negotiated with the remote party who will receive the message. The encryption algorithm is determined by the cipher suite in use.
		/// <br><b>Note</b>  In "streaming" mode, that is, when the ISC_REQ_STREAM or ASC_REQ_STREAM flag was specified during the handshake, EncryptMessage or DecryptMessage cannot be called at the same time from multiple threads unless each thread has its own SSPI context. Each encryption or decryption operation changes the internal state of the encryption key. If the encryption key states are not synchronized on the client and server, the decryption operation fails.</br>
		/// </summary>
		/// <param name="phContext">Handle to the security context previously established with the message recipient.</param>
		/// <param name="fQOP">Not used with Schannel. Specify zero.</param>
		/// <param name="pMessage">Pointer to a SecBufferDesc structure. On input, this structure contains one or more SecBuffer structures, exactly one of which must be of type SECBUFFER_DATA. This buffer contains the message that is encrypted in place. For more information, see Remarks.</param>
		/// <param name="MessageSeqNo">Not used with Schannel. Specify zero.</param>
		/// <returns>If the function succeeds, the return value is SEC_E_OK.<br>If the function fails, the return value is a nonzero error code.</br></returns>
		internal static int EncryptMessage(IntPtr phContext, int fQOP, IntPtr pMessage, int MessageSeqNo) {
			if (IsNT4)
				return SspiNt4Provider.EncryptMessage(phContext, fQOP, pMessage, MessageSeqNo);
			else
				return SspiNormalProvider.EncryptMessage(phContext, fQOP, pMessage, MessageSeqNo);
		}
		/// <summary>
		/// The DecryptMessage function decrypts secured messages. It also signals when the message sender is requesting a renegotiation (redo) of the connection attributes or has shutdown the connection.
		/// <br><b>Note</b>  In "streaming" mode, that is, when the ISC_REQ_STREAM or ASC_REQ_STREAM flag was specified during the handshake, EncryptMessage or DecryptMessage cannot be called at the same time from multiple threads unless each thread has its own SSPI context. Each encryption or decryption operation changes the internal state of the encryption key. If the encryption key states are not synchronized on the client and server, the decryption operation fails.</br>
		/// </summary>
		/// <param name="phContext">Handle to the security context previously established with the message sender.</param>
		/// <param name="pMessage">Pointer to a SecBufferDesc structure. For contexts that are not connection-oriented, on input, the structure must contain four SecBuffer structures. Exactly one buffer must be of type SECBUFFER_DATA and contain an encrypted message, which is decrypted in place. The remaining buffers are used for output and must be of type SECBUFFER_EMPTY.<br>For connection-oriented contexts a SECBUFFER_DATA type buffer must be supplied, as noted for non-connection-oriented contexts. Additionally, a second SECBUFFER_TOKEN type buffer containing a security token must also be supplied.</br></param>
		/// <param name="MessageSeqNo">Not used with Schannel. Specify zero.</param>
		/// <param name="pfQOP">Not used with Schannel. Specify NULL.</param>
		/// <returns>If the function completes successfully and no connection state changes, such as renegotiation or shutdown, were initiated by the remote party, the return value is SEC_E_OK.<br>If the function fails or the connection state has changed, the function returns a nonzero value indicating the error or state.</br></returns>
		internal static int DecryptMessage(IntPtr phContext, IntPtr pMessage, int MessageSeqNo, IntPtr pfQOP) {
			if (IsNT4)
				return SspiNt4Provider.DecryptMessage(phContext, pMessage, MessageSeqNo, pfQOP);
			else
				return SspiNormalProvider.DecryptMessage(phContext, pMessage, MessageSeqNo, pfQOP);
		}
		/// <summary>
		/// The ApplyControlToken function is used to shut down the security context underlying an existing Schannel connection.
		/// </summary>
		/// <param name="phContext">Handle to a security context. Schannel will shut down the context and notify the remote party with whom this context was established. See Remarks.</param>
		/// <param name="pInput">Pointer to a SecBufferDesc structure that describes a single SecBuffer structure.</param>
		/// <returns>If the function succeeds, the return value is SEC_E_OK.<br>If the function fails, the return value is a nonzero error code.</br></returns>
		internal static int ApplyControlToken(IntPtr phContext, IntPtr pInput) {
			if (IsNT4)
				return SspiNt4Provider.ApplyControlToken(phContext, pInput);
			else
				return SspiNormalProvider.ApplyControlToken(phContext, pInput);
		}




		/// <summary>
		/// Indicates whether the program is running on NT4 or not.
		/// </summary>
		/// <value><b>true</b> if the program is running on Windows NT4, <b>false</b> otherwise.</value>
		private static readonly bool IsNT4 = (Environment.OSVersion.Platform == PlatformID.Win32NT && Environment.OSVersion.Version.Major == 4);
	}
}