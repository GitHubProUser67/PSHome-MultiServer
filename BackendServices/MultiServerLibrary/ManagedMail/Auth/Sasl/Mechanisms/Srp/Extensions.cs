using System.IO;
using System.Text;

namespace S22.Imap.Auth.Sasl.Mechanisms.Srp {
	/// <summary>
	/// Adds extension methods to the BinaryReader class to simplify the
	/// deserialization of SRP messages.
	/// </summary>
	internal static class BinaryReaderExtensions {
		/// <summary>
		/// Reads an unsigned integer value from the underlying stream,
		/// optionally using big endian byte ordering.
		/// </summary>
		/// <param name="reader">Extension method for BinaryReader.</param>
		/// <param name="bigEndian">Set to true to interpret the integer value
		/// as big endian value.</param>
		/// <returns>The 32-byte unsigned integer value read from the underlying
		/// stream.</returns>
		public static uint ReadUInt32(this BinaryReader reader, bool bigEndian) {
			uint result = reader.ReadUInt32();
            if (bigEndian)
				return EndianTools.EndianUtils.ReverseUint(result);
			return result;
		}

		/// <summary>
		/// Reads an unsigned short value from the underlying stream, optionally
		/// using big endian byte ordering.
		/// </summary>
		/// <param name="reader">Extension method for BinaryReader.</param>
		/// <param name="bigEndian">Set to true to interpret the short value
		/// as big endian value.</param>
		/// <returns>The 16-byte unsigned short value read from the underlying
		/// stream.</returns>
		public static ushort ReadUInt16(this BinaryReader reader, bool bigEndian) {
            ushort result = reader.ReadUInt16();
            if (bigEndian)
                return EndianTools.EndianUtils.ReverseUshort(result);
            return result;
		}

		/// <summary>
		/// Reads a "multi-precision integer" from this instance.
		/// </summary>
		/// <param name="reader">Extension method for the BinaryReader class.</param>
		/// <returns>An instance of the Mpi class decoded from the bytes read
		/// from the underlying stream.</returns>
		public static Mpi ReadMpi(this BinaryReader reader) {
			return new Mpi(reader.ReadBytes(reader.ReadUInt16(true)));
		}

		/// <summary>
		/// Reads an "octet-sequence" from this instance.
		/// </summary>
		/// <param name="reader">Extension method for the BinaryReader class.</param>
		/// <returns>An instance of the OctetSequence class decoded from the bytes
		/// read from the underlying stream.</returns>
		public static OctetSequence ReadOs(this BinaryReader reader) {
			return new OctetSequence(reader.ReadBytes(reader.ReadByte()));
		}

		/// <summary>
		/// Reads an UTF-8 string from this instance.
		/// </summary>
		/// <param name="reader">Extension method for the BinaryReader class.</param>
		/// <returns>An instance of the Utf8String class decoded from the bytes
		/// read from the underlying stream.</returns>
		public static Utf8String ReadUtf8String(this BinaryReader reader) {
			return new Utf8String(Encoding.UTF8.GetString(reader.ReadBytes(reader.ReadUInt16(true))));
		}
	}
}
