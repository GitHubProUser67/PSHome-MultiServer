using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    /// <summary>
    /// Fundamental information about a file.
    /// </summary>
    public class MediusFile : IStreamSerializer
    {
        /// <summary>
        /// Filename
        /// </summary>
        public string FileName;
        /// <summary>
        /// checksum of file data
        /// </summary>
        public string ServerChecksum;
        /// <summary>
        /// Read only ID of file assigned by server
        /// </summary>
        public int FileID;
        /// <summary>
        /// Read only file size in bytes
        /// </summary>
        public int FileSize;
        /// <summary>
        /// Read only datetime of file creation
        /// </summary>
        public int CreationTimeStamp;
        /// <summary>
        /// Owner's user account id
        /// </summary>
        public int OwnerID;
        /// <summary>
        /// Group id
        /// </summary>
        public int GroupID;
        /// <summary>
        /// Owner File Permissions
        /// </summary>
        public ushort OwnerPermissionRWX;
        /// <summary>
        /// Group File Permissions
        /// </summary>
        public ushort GroupPermissionRWX;
        /// <summary>
        /// Global File Permissions
        /// </summary>
        public ushort GlobalPermissionRWX;
        /// <summary>
        /// Read only ID used to identify the current operation being performed on file.
        /// </summary>
        public ushort ServerOperationID;

        public void Deserialize(BinaryReader reader)
        {
            FileName = reader.ReadString(Constants.MEDIUS_FILE_MAX_FILENAME_LENGTH);
            ServerChecksum = reader.ReadString(Constants.MEDIUS_FILE_CHECKSUM_NUMBYTES);
            FileID = reader.ReadInt32();
            FileSize = reader.ReadInt32();
            CreationTimeStamp = reader.ReadInt32();
            OwnerID = reader.ReadInt32();
            GroupID = reader.ReadInt32();
            OwnerPermissionRWX = reader.ReadUInt16();
            GroupPermissionRWX = reader.ReadUInt16();
            GlobalPermissionRWX = reader.ReadUInt16();
            ServerOperationID = reader.ReadUInt16();
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.Write(FileName, Constants.MEDIUS_FILE_MAX_FILENAME_LENGTH);
            writer.Write(ServerChecksum, Constants.MEDIUS_FILE_CHECKSUM_NUMBYTES);
            writer.Write(FileID);
            writer.Write(FileSize);
            writer.Write(CreationTimeStamp);
            writer.Write(OwnerID);
            writer.Write(GroupID);
            writer.Write(OwnerPermissionRWX);
            writer.Write(GroupPermissionRWX);
            writer.Write(GlobalPermissionRWX);
            writer.Write(ServerOperationID);
        }

        public override string ToString()
        {
            return $"Filename: {FileName} " +
                $"ServerChecksum: {System.Convert.ToString(ServerChecksum)} " +
                $"FileID: {FileID} " +
                $"FileSize: {FileSize} " +
                $"CreationTimeStamp: {CreationTimeStamp} " +
                $"OwnerID: {OwnerID} " +
                $"GroupID: {GroupID} " +
                $"OwnerPermissionRWX: {OwnerPermissionRWX} " +
                $"GroupPermissionRWX: {GroupPermissionRWX} " +
                $"GlobalPermissionRWX: {GlobalPermissionRWX} " +
                $"ServerOperationID: {ServerOperationID}";
        }
    }
}
