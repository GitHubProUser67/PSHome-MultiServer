using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    public class MediusFileMetaData : IStreamSerializer
    {
        /// <summary>
        /// Maximum number of bytes of file meta-data key. This length includes the null terminating byte at the end of<Br></Br>
        /// the string. This means that the effective meta-data key length is 63 bytes.
        /// </summary>
        public string Key;
        /// <summary>
        /// Maximum bytes of file meta-data value. This length includes the null terminating byte at the end of the<Br></Br>
        /// string. This means that the effective value length is 255 bytes.
        /// </summary>
        public string Value;

        public void Deserialize(BinaryReader reader)
        {
            Key = reader.ReadString(Constants.MEDIUS_FILE_MAX_KEY_LENGTH);
            Value = reader.ReadString(Constants.MEDIUS_FILE_MAX_VALUE_LENGTH);
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.Write(Key, Constants.MEDIUS_FILE_MAX_KEY_LENGTH);
            writer.Write(Value, Constants.MEDIUS_FILE_MAX_VALUE_LENGTH);
        }

        public override string ToString()
        {
            return $"MetaKey: {Key} " +
                $"MetaValue: {Value}";
        }
    }
}
