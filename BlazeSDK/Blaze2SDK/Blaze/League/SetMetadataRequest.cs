using Tdf;

namespace Blaze2SDK.Blaze.League
{
    [TdfStruct]
    public struct SetMetadataRequest
    {
        
        [TdfMember("CRC")]
        public uint mRosterCrc;
        
        [TdfMember("GMID")]
        public uint mMemberId;
        
        [TdfMember("LGID")]
        public uint mLeagueId;
        
        [TdfMember("META")]
        public byte[] mMetadata;
        
        [TdfMember("SMET")]
        public byte mIsStringMetadata;
        
    }
}
