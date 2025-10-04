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

using System.Security.Cryptography;

// Microsoft implementations of RC4: 56-bit and 128-bit 
namespace Org.Mentalis.LegacySecurity.Cryptography {
	/// <summary>
	/// Represents the base class from which all implementations of the <i>Alternative RC Four</i> symmetric stream cipher must inherit.
	/// ARCFour is fully compatible with the RC4<sup>TM</sup> algorithm.
	/// </summary>
	/// <remarks>
	/// RC4 is a trademark of RSA Data Security Inc.
	/// For more information about ARCFour, consult the help files.
	/// </remarks>
	public abstract class ARCFour : SymmetricAlgorithm {
		/// <summary>
		/// Initializes a new instance of the ARCFourManaged class.
		/// </summary>
		/// <remarks>
		/// The default keysize is 128 bits.
		/// </remarks>
		public ARCFour() {
			KeySizeValue = 128;
		}
		/// <summary>
		/// Gets or sets the block size of the cryptographic operation in bits.
		/// </summary>
		/// <value>The block size of ARCFour is always 8 bits.</value>
		/// <exception cref="CryptographicException">The block size is invalid.</exception>
		public override int BlockSize {
			get {
				return 8;
			}
			set {
				if (value != 8)
					throw new CryptographicException("ARCFour is a stream cipher, not a block cipher.");
			}
		}
		/// <summary>
		/// Gets or sets the feedback size of the cryptographic operation in bits.
		/// </summary>
		/// <value>This property always throws a <see cref="CryptographicException"/>.</value>
		/// <exception cref="CryptographicException">This exception is always thrown.</exception>
		/// <remarks>ARCFour doesn't use the FeedbackSize property.</remarks>
		public override int FeedbackSize {
			get {
				throw new CryptographicException("ARCFour doesn't use the feedback size property.");
			}
			set {
				throw new CryptographicException("ARCFour doesn't use the feedback size property.");
			}
		}
		/// <summary>
		/// Gets or sets the initialization vector (IV) for the symmetric algorithm.
		/// </summary>
		/// <value>This property always returns a byte array of length one. The value of the byte in the array is always set to zero.</value>
		/// <exception cref="CryptographicException">An attempt is made to set the IV to an invalid instance.</exception>
		/// <remarks>ARCFour doesn't use the IV property, however the property accepts IV's of up to one byte (ARCFour's <see cref="BlockSize"/>) in order to interoperate with software that has been written with the use of block ciphers in mind.</remarks>
		public override byte[] IV {
			get {
				return new byte[1];
			}
			set {
				if (value != null && value.Length > 1)
					throw new CryptographicException("ARCFour doesn't use an Initialization Vector.");
			}
		}
		/// <summary>
		/// Gets the block sizes that are supported by the symmetric algorithm.
		/// </summary>
		/// <value>An array containing the block sizes supported by the algorithm.</value>
		/// <remarks>Only a block size of one byte is supported by the ARCFour algorithm.</remarks>
		public override KeySizes[] LegalBlockSizes {
			get {
				return new KeySizes[] { new KeySizes(8, 8, 0) };
			}
		}
		/// <summary>
		/// Gets the key sizes that are supported by the symmetric algorithm.
		/// </summary>
		/// <value>An array containing the key sizes supported by the algorithm.</value>
		/// <remarks>Only key sizes that match an entry in this array are supported by the symmetric algorithm.</remarks>
		public override KeySizes[] LegalKeySizes {
			get {
				return new KeySizes[] { new KeySizes(8, 2048, 8) };
			}
		}
		/// <summary>
		/// Gets or sets the mode for operation of the symmetric algorithm.
		/// </summary>
		/// <value>The mode for operation of the symmetric algorithm.</value>
		/// <remarks>ARCFour only supports the OFB cipher mode. See <see cref="CipherMode"/> for a description of this mode.</remarks>
		/// <exception cref="CryptographicException">The cipher mode is not OFB.</exception>
		public override CipherMode Mode {
			get {
				return CipherMode.OFB;
			}
			set {
				if (value != CipherMode.OFB)
					throw new CryptographicException("ARCFour only supports OFB.");
			}
		}
		/// <summary>
		/// Gets or sets the padding mode used in the symmetric algorithm.
		/// </summary>
		/// <value>The padding mode used in the symmetric algorithm. This property always returns PaddingMode.None.</value>
		/// <exception cref="CryptographicException">The padding mode is set to a padding mode other than PaddingMode.None.</exception>
		public override PaddingMode Padding {
			get {
				return PaddingMode.None;
			}
			set {
				if (value != PaddingMode.None)
					throw new CryptographicException("ARCFour is a stream cipher, not a block cipher.");
			}
		}
		/// <summary>
		/// This is a stub method.
		/// </summary>
		/// <remarks>Since the ARCFour cipher doesn't use an Initialization Vector, this method will not do anything.</remarks>
		public override void GenerateIV() {
			// do nothing
		}
		/// <summary>
		/// Generates a random Key to be used for the algorithm.
		/// </summary>
		/// <remarks>Use this method to generate a random key when none is specified.</remarks>
		public override void GenerateKey() {
			byte[] key = new byte[KeySize / 8];
#if NET6_0_OR_GREATER
            RandomNumberGenerator.Fill(key);
#else
			new RNGCryptoServiceProvider().GetBytes(key);
#endif
			Key = key;
		}
		/// <summary>
		/// Creates an instance of the default cryptographic object used to perform the ARCFour transformation.
		/// </summary>
		/// <returns>The instance of a cryptographic object used to perform the ARCFour transformation.</returns>
		public static new ARCFour Create() {
			return Create("ARCFOUR");
		}
		/// <summary>
		/// Creates an instance of the specified cryptographic object used to perform the ARCFour transformation.
		/// </summary>
		/// <param name="AlgName">The name of the specific implementation of <see cref="ARCFour"/> to create.</param>
		/// <returns>A cryptographic object.</returns>
		public static new ARCFour Create(string AlgName) {
			try {
				if (AlgName.ToUpper() == "ARCFOUR" || AlgName.ToLower() == "Org.Mentalis.LegacySecurity.Cryptography.ARCFourManaged")
					return new ARCFourManaged();
			} catch {}
			return null;
		}
	}
}