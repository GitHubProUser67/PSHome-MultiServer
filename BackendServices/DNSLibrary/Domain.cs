using System.Net;
using System.Text;
using DNSLibrary.Utils;
using EndianTools;

namespace DNSLibrary
{
    public class Domain : IComparable<Domain>
    {
        private const byte ASCII_UPPERCASE_FIRST = 65;
        private const byte ASCII_UPPERCASE_LAST = 90;
        private const byte ASCII_LOWERCASE_FIRST = 97;
        private const byte ASCII_LOWERCASE_LAST = 122;
        private const byte ASCII_UPPERCASE_MASK = 223;

        private readonly byte[][] labels;

        public static Domain FromString(string domain)
        {
            return new Domain(domain);
        }

        public static Domain FromArray(byte[] message, int offset)
        {
            return FromArray(message, offset, out _);
        }

        public static Domain FromArray(byte[] message, int offset, out int endOffset)
        {
            var endOffsetAssigned = false;
            endOffset = 0;
            byte lengthOrPointer;
            IList<byte[]> labels = [];
            var visitedOffsetPointers = new HashSet<int>();

            while ((lengthOrPointer = message[offset++]) > 0)
            {
                // Two highest bits are set (pointer)
                if (lengthOrPointer.GetBitValueAt(6, 2) == 3)
                {
                    if (!endOffsetAssigned)
                    {
                        endOffsetAssigned = true;
                        endOffset = offset + 1;
                    }

                    ushort pointer = lengthOrPointer.GetBitValueAt(0, 6);
                    offset = (pointer << 8) | message[offset];

                    if (visitedOffsetPointers.Contains(offset))
                        throw new ArgumentException("[Domain] - Compression pointer loop detected");
                    visitedOffsetPointers.Add(offset);

                    continue;
                }

                if (lengthOrPointer.GetBitValueAt(6, 2) != 0)
                    throw new ArgumentException("[Domain] - Unexpected bit pattern in label length");

                var length = lengthOrPointer;
                var label = new byte[length];
                try
                {
                    Array.Copy(message, offset, label, 0, length);
                }
                catch
                {
                    // Out of bounds or invalid offset.
                    break;
                }

                labels.Add(label);

                offset += length;
            }

            if (!endOffsetAssigned)
                endOffset = offset;

            return new Domain(labels.ToArray());
        }

        public static Domain PointerName(IPAddress ip)
        {
            return new Domain(FormatReverseIP(ip));
        }

        private static string FormatReverseIP(IPAddress ip)
        {
            var address = ip.GetAddressBytes();

            if (address.Length == 4)
            {
                return string.Join(".", address.ReverseArray().Select(b => b.ToString()))
                    + ".in-addr.arpa";
            }

            var nibbles = new byte[address.Length * 2];

            for (int i = 0, j = 0; i < address.Length; i++, j = 2 * i)
            {
                var b = address[i];

                nibbles[j] = b.GetBitValueAt(4, 4);
                nibbles[j + 1] = b.GetBitValueAt(0, 4);
            }

            return string.Join(".", nibbles.ReverseArray().Select(b => b.ToString("x")))
                + ".ip6.arpa";
        }

        private static bool IsASCIIAlphabet(byte b)
        {
            return (ASCII_UPPERCASE_FIRST <= b && b <= ASCII_UPPERCASE_LAST)
                || (ASCII_LOWERCASE_FIRST <= b && b <= ASCII_LOWERCASE_LAST);
        }

        private static int CompareTo(byte a, byte b)
        {
            if (IsASCIIAlphabet(a) && IsASCIIAlphabet(b))
            {
                a &= ASCII_UPPERCASE_MASK;
                b &= ASCII_UPPERCASE_MASK;
            }

            return a - b;
        }

        private static int CompareTo(byte[] a, byte[] b)
        {
            var length = Math.Min(a.Length, b.Length);

            for (var i = 0; i < length; i++)
            {
                var v = CompareTo(a[i], b[i]);
                if (v != 0)
                    return v;
            }

            return a.Length - b.Length;
        }

        public Domain(byte[][] labels)
        {
            this.labels = labels;
        }

        public Domain(string[] labels, Encoding encoding)
        {
            this.labels = [.. labels.Select(label => encoding.GetBytes(label))];
        }

        public Domain(string domain)
            : this(domain.Split('.')) { }

        public Domain(string[] labels)
            : this(labels, Encoding.ASCII) { }

        public int Size
        {
            get { return labels.Sum(l => l.Length) + labels.Length + 1; }
        }

        public byte[] ToArray()
        {
            var result = new byte[Size];
            var offset = 0;

            foreach (var label in labels)
            {
                result[offset++] = (byte)label.Length;
                label.CopyTo(result, offset);
                offset += label.Length;
            }

            result[offset] = 0;
            return result;
        }

        public string ToString(Encoding encoding)
        {
            return string.Join(".", labels.Select(label => encoding.GetString(label)));
        }

        public override string ToString()
        {
            return ToString(Encoding.ASCII);
        }

        public int CompareTo(Domain other)
        {
            var length = Math.Min(labels.Length, other.labels.Length);

            for (var i = 0; i < length; i++)
            {
                var v = CompareTo(labels[i], other.labels[i]);
                if (v != 0)
                    return v;
            }

            return labels.Length - other.labels.Length;
        }

        public override bool Equals(object obj)
        {
            return obj != null && obj is Domain && CompareTo(obj as Domain) == 0;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = 17;

                foreach (var label in labels)
                {
                    foreach (var b in label)
                        hash = (hash * 31) + (IsASCIIAlphabet(b) ? b & ASCII_UPPERCASE_MASK : b);
                }

                return hash;
            }
        }
    }
}
