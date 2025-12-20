using Tdf;

namespace Blaze2SDK.Blaze.Registration
{
    [TdfStruct]
    public struct RegistrationWipestatsRequest
    {
        
        [TdfMember("EVID")]
        public int mEventID;
        
        [TdfMember("USID")]
        public uint mUserID;
        
    }
}
