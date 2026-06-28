using System.Numerics;
using EndianTools;

namespace MultiServerLibrary.ManagedMail.Auth.Sasl.Mechanisms.Srp
{
    /// <summary>
    /// Represents a "multi-precision integer" (MPI) as is described in the
    /// SRP specification (3.2 Multi-Precision Integers, p.5).
    /// </summary>
    /// <remarks>Multi-Precision Integers, or MPIs, are positive integers used
    /// to hold large integers used in cryptographic computations.</remarks>
    internal class Mpi
    {
        /// <summary>
        /// The underlying BigInteger instance used to represent this
        /// "multi-precision integer".
        /// </summary>
        public BigInteger Value { get; set; }

        /// <summary>
        /// Creates a new "multi-precision integer" from the specified array
        /// of bytes.
        /// </summary>
        /// <param name="data">A big-endian sequence of bytes forming the
        /// integer value of the multi-precision integer.</param>
        public Mpi(byte[] data)
        {
            var b = new byte[data.Length];
            Array.Copy(data.ReverseArray(), b, data.Length);
            var builder = new ByteBuilder().Append(b);
            // We append a null byte to the buffer which ensures the most
            // significant bit will never be set and the big integer value
            // always be positive.
            if (b.Last() != 0)
                builder.Append(0);
            Value = new BigInteger(builder.ToArray());
        }

        /// <summary>
        /// Creates a new "multi-precision integer" from the specified BigInteger
        /// instance.
        /// </summary>
        /// <param name="value">The BigInteger instance to initialize the MPI
        /// with.</param>
        public Mpi(BigInteger value)
            : this(value.ToByteArray().ReverseArray()) { }

        /// <summary>
        /// Returns a sequence of bytes in big-endian order forming the integer
        /// value of this "multi-precision integer" instance.
        /// </summary>
        /// <returns>Returns a sequence of bytes in big-endian order representing
        /// this "multi-precision integer" instance.</returns>
        public byte[] ToBytes()
        {
            var b = Value.ToByteArray().ReverseArray();
            // Strip off the 0 byte.
            return b[0] == 0 ? b.Skip(1).ToArray() : b;
        }

        /// <summary>
        /// Serializes the "multi-precision integer" into a sequence of bytes
        /// according to the requirements of the SRP specification.
        /// </summary>
        /// <returns>A big-endian sequence of bytes representing the integer
        /// value of the MPI.</returns>
        public byte[] Serialize()
        {
            // MPI's expect a big-endian sequence of bytes forming the integer
            // value, whereas BigInteger uses little-endian.
            var data = ToBytes();
            var length = Convert.ToUInt16(data.Length);

            return new ByteBuilder().Append(length, true).Append(data).ToArray();
        }
    }
}
