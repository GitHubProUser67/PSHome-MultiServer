using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    [MediusMessage(NetMessageClass.MessageClassLobby, MediusLobbyMessageIds.FileListFiles)]
    public class MediusFileListRequest : BaseLobbyMessage, IMediusRequest
    {

        public override byte PacketType => (byte)MediusLobbyMessageIds.FileListFiles;

        public string FileNameBeginsWith;
        public uint FilesizeGreaterThan;
        public uint FilesizeLessThan;
        public uint OwnerByID;
        public uint NewerThanTimestamp;
        public uint StartingEntryNumber;
        public uint PageSize;
        public MessageId MessageID { get; set; }

        public override void Deserialize(MessageReader reader)
        {
            // 
            base.Deserialize(reader);

            //
            FileNameBeginsWith = reader.ReadString(Constants.MEDIUS_FILE_MAX_FILENAME_LENGTH);
            FilesizeGreaterThan = reader.ReadUInt32();
            FilesizeLessThan = reader.ReadUInt32();
            OwnerByID = reader.ReadUInt32();
            NewerThanTimestamp = reader.ReadUInt32();
            StartingEntryNumber = reader.ReadUInt32();
            PageSize = reader.ReadUInt32();
            //
            MessageID = reader.Read<MessageId>();
            reader.ReadBytes(3);
        }

        public override void Serialize(MessageWriter writer)
        {
            // 
            base.Serialize(writer);

            //
            writer.Write(FileNameBeginsWith);
            writer.Write(FilesizeGreaterThan);
            writer.Write(FilesizeLessThan);
            writer.Write(OwnerByID);
            writer.Write(NewerThanTimestamp);
            writer.Write(StartingEntryNumber);
            writer.Write(PageSize);

            //
            writer.Write(MessageID ?? MessageId.Empty);
            writer.Write(new byte[3]);
        }


        public override string ToString()
        {
            return base.ToString() + " " +

                $"FileNameBeginsWith: {FileNameBeginsWith} " +
                $"FilesizeGreaterThan: {FilesizeGreaterThan} " +
                $"FilesizeLessThan: {FilesizeLessThan} " +
                $"OwnerByID: {OwnerByID} " +
                $"NewerThanTimestamp: {NewerThanTimestamp} " +
                $"StartingEntryNumber: {StartingEntryNumber} " +
                $"PageSize: {PageSize} " +
                $"MessageID:{MessageID} ";
        }
    }
}
