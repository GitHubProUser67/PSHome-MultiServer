using Tdf;

namespace Blaze2SDK.Blaze
{
    [TdfStruct]
    public struct LookupUsersRequest
    {
        
        [TdfMember("LTYP")]
        public LookupType mLookupType;
        
        [TdfMember("ULST")]
        public List<UserIdentification> mUserIdentificationList;
        
        public enum LookupType : int
        {
            BLAZE_ID = 0x0,
            PERSONA_NAME = 0x1,
            EXTERNAL_ID = 0x2,
            ACCOUNT_ID = 0x3,
        }
        
    }
}
