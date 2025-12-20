using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    [ScertMessage(RT_MSG_TYPE.RT_MSG_SERVER_CHEAT_QUERY)]
    public class RT_MSG_SERVER_CHEAT_QUERY : BaseScertMessage
    {
        public override RT_MSG_TYPE Id => RT_MSG_TYPE.RT_MSG_SERVER_CHEAT_QUERY;

        public CheatQueryType QueryType;
        public int SequenceId;
        public uint StartAddress;
        public int Length;
        public byte[] Data;

        public override void Deserialize(MessageReader reader)
        {
            QueryType = reader.Read<CheatQueryType>();
            SequenceId = reader.ReadInt32();
            StartAddress = reader.ReadUInt32();
            Length = reader.ReadInt32();
            Data = reader.ReadRest();
        }

        public override void Serialize(MessageWriter writer)
        {
            writer.Write(QueryType);
            writer.Write(SequenceId);
            writer.Write(StartAddress);
            writer.Write(Length);
            if (Data != null)
                writer.Write(Data);
        }

        public override string ToString()
        {
            return base.ToString() + " " +
                $"QueryType: {QueryType} " +
                $"SequenceId: {SequenceId} " +
                $"StartAddress: {StartAddress:X8} " +
                $"Length: {Length} " +
                $"Data :{(Data == null ? "" : System.BitConverter.ToString(Data))}";
        }
    }
}
