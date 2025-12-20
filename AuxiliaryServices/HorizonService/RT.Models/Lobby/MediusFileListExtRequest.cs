using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    [MediusMessage(NetMessageClass.MessageClassLobbyExt, MediusLobbyExtMessageIds.FileListExtRequest)]
    public class MediusFileListExtRequest : BaseLobbyExtMessage, IMediusRequest
    {

        public override byte PacketType => (byte)MediusLobbyExtMessageIds.FileListExtRequest;

        public string FileNameBeginsWith;
        public uint FilesizeGreaterThan;
        public uint FilesizeLessThan;
        public int OwnerByID;
        public uint NewerThanTimestamp;
        public uint StartingEntryNumber;
        public uint PageSize;
        public MessageId MessageID { get; set; }

        public MediusFileMetaData metaData;
        public MediusComparisonOperator metaOperator;
        public MediusFileSortBy sortBy;
        public MediusSortOrder sortOrder;

        public override void Deserialize(MessageReader reader)
        {
            // 
            base.Deserialize(reader);

            // MediusFileListRequest Wrapped
            FileNameBeginsWith = reader.ReadString(Constants.MEDIUS_FILE_MAX_FILENAME_LENGTH);
            FilesizeGreaterThan = reader.ReadUInt32();
            FilesizeLessThan = reader.ReadUInt32();
            OwnerByID = reader.ReadInt32();
            NewerThanTimestamp = reader.ReadUInt32();
            StartingEntryNumber = reader.ReadUInt32();
            PageSize = reader.ReadUInt32();
            //
            MessageID = reader.Read<MessageId>();
            reader.ReadBytes(3);

            //MediusFileListExtRequest
            metaData = reader.Read<MediusFileMetaData>();
            metaOperator = reader.Read<MediusComparisonOperator>();
            sortBy = reader.Read<MediusFileSortBy>();
            sortOrder = reader.Read<MediusSortOrder>();
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

            writer.Write(metaData);
            writer.Write(metaOperator);
            writer.Write(sortBy);
            writer.Write(sortOrder);

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
                $"MessageID:{MessageID} " +
                $"MetaData: {metaData} " +
                $"MetaOperator: {metaOperator} " + 
                $"SortBy: {sortBy} " +
                $"SortOrder: {sortOrder}";
        }
    }
}
