using Tdf;

namespace Blaze2SDK.Blaze.Authentication
{
    [TdfStruct]
    public struct CheckAgeReqResponse
    {
        
        [TdfMember("ANON")]
        public bool mustBeAnonymous;
        
        [TdfMember("PEND")]
        public byte pendingParentalConsent;
        
        [TdfMember("SPAM")]
        public byte isSpammable;
        
    }
}
