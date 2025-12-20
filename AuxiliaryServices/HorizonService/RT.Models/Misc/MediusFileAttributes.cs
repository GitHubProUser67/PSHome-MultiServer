using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    /// <summary>
    /// File Attributes.
    /// </summary>
    public class MediusFileAttributes : IStreamSerializer
    {
        /// <summary>
        /// Client provided text description of file
        /// </summary>
        public string Description; //MEDIUS_FILE_MAX_DESCRIPTION_LENGTH
        /// <summary>
        /// Read only date/time when file was last changed
        /// </summary>
        public uint LastChangedTimeStamp;
        /// <summary>
        /// Read only ID of user that last changed the file
        /// </summary>
        public uint LastChangedByUserID;
        /// <summary>
        /// Read only number of accesses requested for the file
        /// </summary>
        public uint NumberAccesses;
        /// <summary>
        /// Flag specifying if the file is streamable
        /// </summary>
        public uint StreamableFlag;
        /// <summary>
        /// The desired data rate to stream the file data to/from the client
        /// </summary>
        public uint StreamingDataRate;

        public virtual void Deserialize(BinaryReader reader)
        {
            Description = reader.ReadString(Constants.MEDIUS_FILE_MAX_DESCRIPTION_LENGTH);
            LastChangedTimeStamp = reader.ReadUInt32();
            LastChangedByUserID = reader.ReadUInt32();
            NumberAccesses = reader.ReadUInt32();
            StreamableFlag = reader.ReadUInt32();
            StreamingDataRate = reader.ReadUInt32();
        }

        public virtual void Serialize(BinaryWriter writer)
        {
            writer.Write(Description, Constants.MEDIUS_FILE_MAX_DESCRIPTION_LENGTH);
            writer.Write(LastChangedTimeStamp);
            writer.Write(LastChangedByUserID);
            writer.Write(NumberAccesses);
            writer.Write(StreamableFlag);
            writer.Write(StreamingDataRate);
        }

        public override string ToString()
        {
            return base.ToString() + " " +
                $"Description: {Description} " +
                $"LastChangedTimeStamp: {LastChangedTimeStamp} " +
                $"LastChangedByUserID: {LastChangedByUserID} " +
                $"NumberAccesses: {NumberAccesses} " +
                $"StreamableFlag: {StreamableFlag} " +
                $"StreamingDataRate: {StreamingDataRate}";
        }
    }
}
