using System;

namespace Horizon.LIBRARY.Database.Entities
{
    public partial class Files
    {
        public int Id { get; set; }
        public int AppId { get; set; }
        public string FileName { get; set; }
        public string ServerChecksum { get; set; }
        public int FileID { get; set; }
        public int FileSize { get; set; }
        public int CreationTimeStamp { get; set; }
        public int OwnerID { get; set; }
        public int GroupID { get; set; }
        public int OwnerPermissionRWX { get; set; } // Should be ushort but database isn't compatible with this type, so pad to int...
        public int GroupPermissionRWX { get; set; } // Should be ushort but database isn't compatible with this type, so pad to int...
        public int GlobalPermissionRWX { get; set; } // Should be ushort but database isn't compatible with this type, so pad to int...
        public int ServerOperationID { get; set; } // Should be ushort but database isn't compatible with this type, so pad to int...
        public DateTime CreateDt { get; set; } = DateTime.UtcNow; // Set default value in constructor
        public DateTime ModifiedDt { get; set; }
    }

    public partial class FileAttributes
    {
        public int AppId { get; set; }
        public int ID { get; set; }
        public int FileID { get; set; }
        public string FileName { get; set; }
        public string Description { get; set; }
        public int LastChangedTimeStamp { get; set; }
        public int LastChangedByUserID { get; set; }
        public int NumberAccesses { get; set; }
        public int StreamableFlag { get; set; }
        public int StreamingDataRate { get; set; }
        public DateTime CreateDt { get; set; } = DateTime.UtcNow; // Set default value in constructor
        public DateTime ModifiedDt { get; set; }
    }

    public partial class FileMetaData
    {
        public int AppId { get; set; }
        public int Id { get; set; }
        public int FileID { get; set; }
        public string FileName { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
        public DateTime CreateDt { get; set; } = DateTime.UtcNow; // Set default value in constructor
        public DateTime ModifiedDt { get; set; }
    }

    #region MediusComparisonOperator
    /// <summary>
    /// Specifies the operator used in filtering operations
    /// </summary>
    public enum MediusComparisonOperator : int
    {
        /// <summary>
        /// Less than comparison operator
        /// </summary>
        LESS_THAN,
        /// <summary>
        /// Less than or equal to comparison operator
        /// </summary>
        LESS_THAN_OR_EQUAL_TO,
        /// <summary>
        /// Equal to comparison operator
        /// </summary>
        EQUAL_TO,
        /// <summary>
        /// Greater than or equal to comparison operator
        /// </summary>
        GREATER_THAN_OR_EQUAL_TO,
        /// <summary>
        /// Great than comparison operator
        /// </summary>
        GREATER_THAN,
        /// <summary>
        /// Not equals comparison operator
        /// </summary>
        NOT_EQUALS,
    }
    #endregion

    public enum MediusFileSortBy : int
    {
        MFSortByNothing,
        MFSortByName,
        MFSortByFileSize,
        MFSortByTimeStamp,
        MFSortByGroupID,
        MFSortByPopularity,
        MFSortByMetaValue,
        MFSortByMetaString
    }

    public enum MediusSortOrder : int
    {
        MEDIUS_ASCENDING,
        MEDIUS_DESCENDING,
    }
}