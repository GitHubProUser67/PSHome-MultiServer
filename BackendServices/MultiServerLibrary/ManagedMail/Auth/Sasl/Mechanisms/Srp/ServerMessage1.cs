using System.Collections.Specialized;

namespace MultiServerLibrary.ManagedMail.Auth.Sasl.Mechanisms.Srp
{
    /// <summary>
    /// Represents the first message sent by the server in response to an
    /// initial client-response.
    /// </summary>
    internal class ServerMessage1
    {
        /// <summary>
        /// The safe prime modulus sent by the server.
        /// </summary>
        public Mpi SafePrimeModulus { get; set; }

        /// <summary>
        /// The generator sent by the server.
        /// </summary>
        public Mpi Generator { get; set; }

        /// <summary>
        /// The user's password salt.
        /// </summary>
        public byte[] Salt { get; set; }

        /// <summary>
        /// The server's ephemeral public key.
        /// </summary>
        public Mpi PublicKey { get; set; }

        /// <summary>
        /// The options list indicating available security services.
        /// </summary>
        public NameValueCollection Options { get; set; }

        /// <summary>
        /// The raw options as received from the server.
        /// </summary>
        public string RawOptions { get; set; }

        /// <summary>
        /// Deserializes a new instance of the ServerMessage1 class from the
        /// specified buffer of bytes.
        /// </summary>
        /// <param name="buffer">The byte buffer to deserialize the ServerMessage1
        /// instance from.</param>
        /// <returns>An instance of the ServerMessage1 class deserialized from the
        /// specified byte array.</returns>
        /// <exception cref="FormatException">Thrown if the byte buffer does not
        /// contain valid data.</exception>
        public static ServerMessage1 Deserialize(byte[] buffer)
        {
            using (var ms = new MemoryStream(buffer))
            {
                using (var r = new BinaryReader(ms))
                {
                    var bufferLength = r.ReadUInt32(true);
                    // We don't support re-using previous sessions.
                    var reuse = r.ReadByte();
                    if (reuse != 0)
                    {
                        throw new FormatException("Unexpected re-use parameter value: " + reuse);
                    }
                    var N = r.ReadMpi();
                    var g = r.ReadMpi();
                    var salt = r.ReadOs();
                    var B = r.ReadMpi();
                    var L = r.ReadUtf8String();
                    return new ServerMessage1()
                    {
                        Generator = g,
                        PublicKey = B,
                        Salt = salt.Value,
                        SafePrimeModulus = N,
                        Options = ParseOptions(L.Value),
                        RawOptions = L.Value,
                    };
                }
            }
        }

        /// <summary>
        /// Parses the options string sent by the server.
        /// </summary>
        /// <param name="s">A comma-delimited options string.</param>
        /// <returns>An initialized instance of the NameValueCollection class
        /// containing the parsed server options.</returns>
        public static NameValueCollection ParseOptions(string s)
        {
            var coll = new NameValueCollection();
            var parts = s.Split(',');
            foreach (var p in parts)
            {
                var index = p.IndexOf('=');
                if (index < 0)
                {
                    coll.Add(p, "true");
                }
                else
                {
                    string name = p.Substring(0, index),
                        value = p.Substring(index + 1);
                    coll.Add(name, value);
                }
            }
            return coll;
        }
    }
}
