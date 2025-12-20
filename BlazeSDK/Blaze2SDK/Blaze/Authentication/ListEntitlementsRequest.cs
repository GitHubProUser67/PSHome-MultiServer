using Tdf;

namespace Blaze2SDK.Blaze.Authentication
{
    [TdfStruct]
    public struct ListEntitlementsRequest
    {
        
        [TdfMember("BUID")]
        public uint mUserId;
        
        [TdfMember("EPSN")]
        public ushort mPageNo;
        
        [TdfMember("EPSZ")]
        public ushort mPageSize;
        
        /// <summary>
        /// Max String Length: 64
        /// </summary>
        [TdfMember("GNLS")]
        public List<string> mGroupNameList;
        
        [TdfMember("OLD")]
        public bool mIsLegacy;
        
        [TdfMember("ONLY")]
        public bool mRestrictive;
        
        [TdfMember("PERS")]
        public bool mWithPersona;
        
    }
}
