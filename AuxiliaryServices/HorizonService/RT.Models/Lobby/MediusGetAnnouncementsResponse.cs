using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;
using System.Collections.Generic;

namespace Horizon.RT.Models
{
    [MediusMessage(NetMessageClass.MessageClassLobby, MediusLobbyMessageIds.GetAnnouncementsResponse)]
    public class MediusGetAnnouncementsResponse : BaseLobbyMessage, IMediusResponse
    {
        public override byte PacketType => (byte)MediusLobbyMessageIds.GetAnnouncementsResponse;

        public bool IsSuccess => StatusCode >= 0;

        public MessageId MessageID { get; set; }

        public MediusCallbackStatus StatusCode;
        public int AnnouncementID;
        public string Announcement; // ANNOUNCEMENT_MAXLEN
        public bool EndOfList;

        public List<int> old113AnnouncementMaxLenAppIds = new List<int>() { 21564, 21574, 21584, 21594, 22274, 22284, 22294, 22304, 20040, 20041, 20042, 20043, 20044 };

        public override void Deserialize(MessageReader reader)
        {
            base.Deserialize(reader);

            MessageID = reader.Read<MessageId>();

            reader.ReadBytes(3);
            StatusCode = reader.Read<MediusCallbackStatus>();
            AnnouncementID = reader.ReadInt32();

            if (reader.MediusVersion <= 112)
                Announcement = reader.ReadString(Constants.ANNOUNCEMENT_MAXLEN);
            else if (reader.MediusVersion == 113)
            {
                if (old113AnnouncementMaxLenAppIds.Contains(reader.AppId))
                    Announcement = reader.ReadString(Constants.ANNOUNCEMENT_MAXLEN);
                else
                    Announcement = reader.ReadString(Constants.ANNOUNCEMENT1_MAXLEN);
            }
            else
                Announcement = reader.ReadString(Constants.ANNOUNCEMENT_MAXLEN);

            EndOfList = reader.ReadBoolean();
            reader.ReadBytes(3);
        }

        public override void Serialize(MessageWriter writer)
        {
            base.Serialize(writer);

            writer.Write(MessageID ?? MessageId.Empty);

            writer.Write(new byte[3]);
            writer.Write(StatusCode);
            writer.Write(AnnouncementID);

            if (writer.MediusVersion <= 112)
                writer.Write(Announcement, Constants.ANNOUNCEMENT_MAXLEN);
            else if (writer.MediusVersion == 113)
            {
                if (old113AnnouncementMaxLenAppIds.Contains(writer.AppId))
                    writer.Write(Announcement, Constants.ANNOUNCEMENT_MAXLEN);
                else
                    writer.Write(Announcement, Constants.ANNOUNCEMENT1_MAXLEN);
            }
            else
                writer.Write(Announcement, Constants.ANNOUNCEMENT_MAXLEN);
            writer.Write(EndOfList);
            writer.Write(new byte[3]);
        }

        public override string ToString()
        {
            return base.ToString() + " " +
                $"MessageID: {MessageID} " +
                $"StatusCode: {StatusCode} " +
                $"AnnouncementID: {AnnouncementID} " +
                $"Announcement: {Announcement} " +
                $"EndOfList: {EndOfList}";
        }
    }
}
