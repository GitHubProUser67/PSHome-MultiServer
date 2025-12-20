using Tdf;

namespace Blaze2SDK.Blaze.Clubs
{
    [TdfStruct]
    public struct CustClubSettings
    {
        
        [TdfMember("COP1")]
        public uint mCustOpt1;
        
        [TdfMember("COP2")]
        public uint mCustOpt2;
        
        [TdfMember("COP3")]
        public uint mCustOpt3;
        
        [TdfMember("COP4")]
        public uint mCustOpt4;
        
        [TdfMember("COP5")]
        public uint mCustOpt5;
        
    }
}
