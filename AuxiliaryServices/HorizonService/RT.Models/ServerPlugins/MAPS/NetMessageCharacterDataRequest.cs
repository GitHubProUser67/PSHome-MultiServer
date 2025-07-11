using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace HorizonService.RT.Models.ServerPlugins.MAPS
{
    public class NetMessaageCharacterDataRequest : IStreamSerializer
    {

        public byte s_nonUIRequestID;
        public QueryType m_queryType;
        public string m_characterName;
        public int m_accountID;
        public int m_requestID;

        public void Deserialize(BinaryReader reader)
        {
            s_nonUIRequestID = reader.ReadByte();
            m_queryType = reader.Read<QueryType>();
            m_characterName = reader.ReadString();
            m_accountID = reader.ReadInt32();
            m_requestID = reader.ReadInt32();
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.Write(s_nonUIRequestID);
            writer.Write(m_queryType);
            if (m_characterName == null)
                writer.Write(string.Empty);
            else
                writer.Write(m_characterName);
            writer.Write(m_accountID);
            writer.Write(m_requestID);
        }

        public override string ToString()
        {
            return base.ToString() + " " +
                $"s_nonUIRequestID: {s_nonUIRequestID} " +
                $"m_queryType: {m_queryType} " +
                $"m_characterName: {m_characterName} " +
                $"m_accountID: {m_accountID} " +
                $"m_requestID: {m_requestID}";
        }
    }
}
