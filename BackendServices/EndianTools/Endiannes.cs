namespace EndianTools
{
    // Massives thanks to TDUWorld for their endian helper class.
    /// <summary>
    /// Endianness of a converter
    /// </summary>
    public enum Endianness
    {
        /// <summary>
        /// Automatic - no endianness settings required
        /// </summary>
        Automatic,
        /// <summary>
        /// Little endian - least significant byte first
        /// </summary>
        LittleEndian,
        /// <summary>
        /// Big endian - most significant byte first
        /// </summary>
        BigEndian
    }
}
