using System.Text;
using Tdf.Extensions;

namespace Tdf
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class TdfMember : Attribute
    {
        public const byte TAG_LENGTH = 3;

        public string Tag { get; private set; }
        public byte[] Bytes { get; private set; }

        //public bool EncodeIfEmpty { get; set; } = false;

        public TdfMember(string tagString)
        {
            Tag = tagString;

            if (!IsASCII(tagString))
                throw new Exception($"Tag can only consist of ASCII characters from 32 to 95 ({tagString})");

            Tag = tagString.ToUpper();

            int len = Tag.Length;

            if (len > 4 || len <= 0)
                throw new Exception($"Tag length can be [1;4] ({tagString})");

            for (int i = 0; i < Tag.Length; i++)
                if (Tag[i] < ' ' || Tag[i] > '_') //' ' - 32 (0x20) //'_' - 95 (0x5F)
                    throw new Exception($"Tag can only consist of ASCII characters from 32 to 95 ({tagString})");

            if ((Tag[0] - 'A') > 25)
                throw new Exception("Tag must begin with letter [A-Z] (tag: " + tagString + ")");

            //the part where we convert string to tag bytes

            byte[] asciiBytes = Encoding.ASCII.GetBytes(Tag);
            int result = (asciiBytes[0] - 32) << 26;
            if (len > 1)
            {
                result |= ((asciiBytes[1] - 32) & 0x3F) << 20;
                if (len > 2)
                {
                    result |= ((asciiBytes[2] - 32) & 0x3F) << 14;
                    if (len > 3)
                        result |= (((asciiBytes[3] - 32) & 0x3F) << 8);
                }
            }

            Bytes = BitConverter.GetBytes(result);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(Bytes);

            //use only the first 3 bytes
            Bytes = Bytes.Take(TAG_LENGTH).ToArray();
        }

        private TdfMember(byte[] tagBytes)
        {
            if (tagBytes == null)
                throw new ArgumentNullException(nameof(tagBytes));

            if (tagBytes.Length != TAG_LENGTH)
                throw new ArgumentException("Tag must be 3 bytes long", nameof(tagBytes));

            Bytes = tagBytes;
            byte[] temp = new byte[sizeof(uint)];

            Buffer.BlockCopy(Bytes, 0, temp, 0, TAG_LENGTH);

            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(Bytes);
                Array.Reverse(temp);
            }

            uint num = BitConverter.ToUInt32(temp, 0);

            byte[] buf = new byte[4];
            int len = 4;

            uint val = num & 0x3F00;
            if (val != 0)
                buf[3] = (byte)(((num >> 8) & 0x3F) + 32);
            else
            {
                buf[3] = 0;
                len = 3;
            }

            val = (num >> 14) & 0x3F;
            if (val != 0)
                buf[2] = (byte)(val + 32);
            else
            {
                buf[2] = 0;
                len = 2;
            }

            val = (num >> 20) & 0x3F;
            if (val != 0)
                buf[1] = (byte)(val + 32);
            else
            {
                buf[1] = 0;
                len = 1;
            }

            val = num >> 26;
            if (val != 0)
                buf[0] = (byte)(val + 32);
            else
            {
                buf[0] = 0;
                len = 0;
            }

            Tag = Encoding.ASCII.GetString(buf, 0, len);
        }

        public static TdfMember FromString(string tagString)
        {
            return new TdfMember(tagString);
        }

        public static TdfMember? FromStream(Stream stream)
        {
            byte[] tag = new byte[TAG_LENGTH];
            if (!stream.ReadAll(tag, 0, TAG_LENGTH))
                return null;
            return new TdfMember(tag);
        }

        public static async Task<TdfMember?> FromStreamAsync(Stream stream)
        {
            byte[] tag = new byte[TAG_LENGTH];
            if (!await stream.ReadAllAsync(tag, 0, TAG_LENGTH).ConfigureAwait(false))
                return null;
            return new TdfMember(tag);
        }

        private static bool IsASCII(string str)
        {
            return Encoding.UTF8.GetByteCount(str) == str.Length;
        }

        public override string ToString()
        {
            return Tag;
        }

        public static implicit operator TdfMember(string tagString)
        {
            return new TdfMember(tagString);
        }

        public static implicit operator string(TdfMember tag)
        {
            return tag.ToString();
        }

        public static implicit operator byte[](TdfMember tag)
        {
            return tag.Bytes;
        }

        public override int GetHashCode()
        {
            return Tag.GetHashCode();
        }

    }
}
