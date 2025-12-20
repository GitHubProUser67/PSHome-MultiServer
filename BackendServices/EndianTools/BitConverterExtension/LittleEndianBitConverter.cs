using System;

namespace EndianTools.BitConverterExtension
{
    /// <summary>
    /// Implementation of EndianBitConverter which converts to/from little-endian
    /// byte arrays.
    /// </summary>
    public sealed class LittleEndianBitConverter : EndianBitConverter
    {
        /// <summary>
        /// Indicates the byte order ("endianess") in which data is converted using this class.
        /// </summary>
        /// <remarks>
        /// Different computer architectures store data using different byte orders. "Big-endian"
        /// means the most significant byte is on the left end of a word. "Little-endian" means the 
        /// most significant byte is on the right end of a word.
        /// </remarks>
        /// <returns>true if this converter is little-endian, false otherwise.</returns>
        public sealed override bool IsLittleEndian()
        {
            return true;
        }

        /// <summary>
        /// Indicates the byte order ("endianess") in which data is converted using this class.
        /// </summary>
        public sealed override Endianness Endianness
        {
            get { return Endianness.LittleEndian; }
        }

        /// <summary>
        /// Returns a decimal value converted from sixteen bytes 
        /// at a specified position in a byte array.
        /// </summary>
        /// <param name="value">An array of bytes.</param>
        /// <param name="startIndex">The starting position within value.</param>
        /// <returns>A decimal  formed by sixteen bytes beginning at startIndex.</returns>
        public override decimal ToDecimal(byte[] value, int startIndex)
        {
            // HACK: This always assumes four parts, each in their own endianness,
            // starting with the first part at the start of the byte array.
            // On the other hand, there's no real format specified...
            int[] parts = new int[4];
            for (int i = 0; i < 4; i++)
            {
                parts[i] = EndianAwareConverter.ToInt32(value, Endianness.LittleEndian, (uint)(startIndex + i * 4));
            }
            return new decimal(parts);
        }

        /// <summary>
        /// Copies the specified number of bytes from value to buffer, starting at index.
        /// </summary>
        /// <param name="value">The value to copy</param>
        /// <param name="bytes">The number of bytes to copy</param>
        /// <param name="buffer">The buffer to copy the bytes into</param>
        /// <param name="index">The index to start at</param>
        protected override void CopyBytesImpl(long value, int bytes, byte[] buffer, int index)
        {
            for (int i = 0; i < bytes; i++)
            {
                buffer[i + index] = unchecked((byte)(value & 0xff));
                value >>= 8;
            }
        }

        /// <summary>
        /// Returns a value built from the specified number of bytes from the given buffer,
        /// starting at index.
        /// </summary>
        /// <param name="buffer">The data in byte array format</param>
        /// <param name="startIndex">The first index to use</param>
        /// <param name="bytesToConvert">The number of bytes to use</param>
        /// <returns>The value built from the given bytes</returns>
        protected override long FromBytes(byte[] buffer, int startIndex, int bytesToConvert)
        {
            long ret = 0;
            for (int i = 0; i < bytesToConvert; i++)
            {
                ret = unchecked((ret << 8) | buffer[startIndex + bytesToConvert - 1 - i]);
            }
            return ret;
        }
    }
}
