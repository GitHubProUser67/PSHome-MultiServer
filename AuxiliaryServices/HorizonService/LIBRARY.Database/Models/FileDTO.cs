using System;

namespace Horizon.LIBRARY.Database.Models
{
    public class FileDTO
    {
        public int AppId { get; set; }
        public string FileName { get; set; }
        public string ServerChecksum { get; set; }
        public int FileID { get; set; }
        public int FileSize { get; set; }
        public int CreationTimeStamp { get; set; }
        public int OwnerID { get; set; }
        public int GroupID { get; set; }
        public uint OwnerPermissionRWX { get; set; }
        public uint GroupPermissionRWX { get; set; }
        public uint GlobalPermissionRWX { get; set; }
        public uint ServerOperationID { get; set; }
        public FileAttributesDTO fileAttributesDTO { get; set; }
        public FileMetaDataDTO fileMetaDataDTO { get; set; }
        public DateTime CreateDt { get; set; }
    }

    public partial class FileAttributesDTO
    {
        public int AppId { get; set; }
        public int FileID { get; set; }
        public string FileName { get; set; }
        public string Description { get; set; }
        public uint LastChangedTimeStamp { get; set; }
        public uint LastChangedByUserID { get; set; }
        public uint NumberAccesses { get; set; }
        public uint StreamableFlag { get; set; }
        public uint StreamingDataRate { get; set; }
        public DateTime CreateDt { get; set; }
    }

    public partial class FileMetaDataDTO
    {
        public int AppId { get; set; }
        public int FileID { get; set; }
        public string FileName { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
        public DateTime CreateDt { get; set; }
    }
}