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
using System.Security.Cryptography;

namespace Org.Mentalis.LegacySecurity.Cryptography {
	/// <summary>
	/// Accesses the managed version of the ARCFour algorithm. This class cannot be inherited.
	/// ARCFour is fully compatible with the RC4<sup>TM</sup> algorithm.
	/// </summary>
	/// <remarks>
	/// RC4 is a trademark of RSA Data Security Inc.
	/// For more information about ARCFour, consult the help files.
	/// </remarks>
	public sealed class ARCFourManaged : ARCFour {
		/// <summary>
		/// Initializes a new instance of the ARCFourManaged class.
		/// </summary>
		/// <remarks>
		/// The default keysize is 128 bits.
		/// </remarks>
		public ARCFourManaged() {}
		/// <summary>
		/// Creates a symmetric <see cref="ARCFour"/> decryptor object with the specified Key.
		/// </summary>
		/// <param name="rgbKey">The secret key to be used for the symmetric algorithm.</param>
		/// <param name="rgbIV">This parameter is not used an should be set to a null reference, or to an array with zero or one bytes.</param>
		/// <returns>A symmetric ARCFour decryptor object.</returns>
		/// <remarks>This method decrypts an encrypted message created using the <see cref="CreateEncryptor"/> overload with the same signature.</remarks>
		public override ICryptoTransform CreateDecryptor(byte[] rgbKey, byte[] rgbIV) {
			if (rgbKey == null)
				throw new ArgumentNullException("Key is a null reference.");
			if (rgbKey.Length == 0 || rgbKey.Length > 256)
				throw new CryptographicException("Invalid Key.");
			if (rgbIV != null && rgbIV.Length > 1)
				throw new CryptographicException("Invalid Initialization Vector.");
			return new ARCFourManagedTransform(rgbKey);
		}
		/// <summary>
		/// Creates a symmetric <see cref="ARCFour"/> encryptor object with the specified Key.
		/// </summary>
		/// <param name="rgbKey">The secret key to be used for the symmetric algorithm.</param>
		/// <param name="rgbIV">This parameter is not used an should be set to a null reference, or to an array with zero or one bytes.</param>
		/// <returns>A symmetric ARCFour encryptor object.</returns>
		/// <remarks>Use the <see cref="CreateDecryptor"/> overload with the same signature to decrypt the result of this method.</remarks>
		public override ICryptoTransform CreateEncryptor(byte[] rgbKey, byte[] rgbIV) {
			return CreateDecryptor(rgbKey, rgbIV);
		}
	}
}