using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.Clubs
{
    [TdfStruct]
    public struct SetMetadataRequest
    {
        
        [TdfMember("CLID")]
        public uint mClubId;
        
        [TdfMember("MDTY")]
        public MetaDataType mMetaDataType;
        
        /// <summary>
        /// Max String Length: 2048
        /// </summary>
        [TdfMember("METD")]
        [StringLength(2048)]
        public string mMetaData;
        
    }
}
