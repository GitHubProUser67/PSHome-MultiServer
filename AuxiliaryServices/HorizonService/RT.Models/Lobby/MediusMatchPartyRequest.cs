using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    [MediusMessage(NetMessageClass.MessageClassLobbyExt, MediusLobbyExtMessageIds.MatchPartyRequest2)]
    public class MediusMatchPartyRequest : BaseLobbyExtMessage, IMediusRequest
    {

        public override byte PacketType => (byte)MediusLobbyExtMessageIds.MatchPartyRequest2;

        public MessageId MessageID { get; set; }
        public string SessionKey; // SESSIONKEY_MAXLEN
        public int SupersetID;
        public int PartySize;
        public int MinGameSize;
        public int MaxGameSize;
        public MediusMatchOptions MatchOptions;
        public int ApplicationDataSize;
        public int[] PartyAccountIDList;
        public string ApplicationData;

        public override void Deserialize(MessageReader reader)
        {
            // 
            base.Deserialize(reader);

            //
            MessageID = reader.Read<MessageId>();

            //
            SessionKey = reader.ReadString(Constants.SESSIONKEY_MAXLEN); 

            //reader.ReadBytes(2);

            //
            SupersetID = reader.ReadInt32();
            PartySize = reader.ReadInt32();

            MinGameSize = reader.ReadInt32();
            MaxGameSize = reader.ReadInt32();
            MatchOptions = reader.Read<MediusMatchOptions>();

            ApplicationDataSize = reader.ReadInt32();

            PartyAccountIDList = new int[PartySize];
            for (int i = 0; i < PartySize; i++)
            {
                PartyAccountIDList[i] = reader.ReadInt32();
            }

            ApplicationData = reader.ReadString(Constants.APPLICATIONDATA_MAXLEN);
        }

        public override void Serialize(MessageWriter writer)
        {
            // 
            base.Serialize(writer);

            //
            writer.Write(MessageID ?? MessageId.Empty);

            //
            writer.Write(SessionKey);

            //writer.Write(new byte[2]);

            //
            writer.Write(SupersetID);
            writer.Write(PartySize);


            writer.Write(MinGameSize);
            writer.Write(MaxGameSize);
            writer.Write(MatchOptions);


            writer.Write(ApplicationDataSize);
            writer.Write(PartyAccountIDList);
            writer.Write(ApplicationData);
        }


        public override string ToString()
        {
            return base.ToString() + " " +
                $"MessageID: {MessageID} " +
                $"SessionKey: {SessionKey} " +
                $"SupersetID: {SupersetID} " +
                $"PartySize: {PartySize} " +
                $"MinGameSize: {MinGameSize} " +
                $"MaxGameSize: {MaxGameSize} " +
                $"MatchOptions: {MatchOptions} " +
                $"ApplicationDataSize: {ApplicationDataSize} " +
                $"PartyAccountIDList: {string.Join(" ", PartyAccountIDList)} " +
                $"ApplicationData:  {string.Join(" ", ApplicationData)} ";
        }
    }
}
