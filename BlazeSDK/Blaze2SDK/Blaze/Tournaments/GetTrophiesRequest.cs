using Tdf;

namespace Blaze2SDK.Blaze.Tournaments
{
    [TdfStruct]
    public struct GetTrophiesRequest
    {
        
        [TdfMember("BZID")]
        public uint mBlazeId;
        
        [TdfMember("NUMT")]
        public uint mNumTrophies;
        
    }
}
