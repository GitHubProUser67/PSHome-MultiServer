using Tdf;

namespace Blaze2SDK.Blaze.Authentication
{
    [TdfStruct]
    public struct ConsoleCreateAccountResponse
    {
        
        [TdfMember("RSLT")]
        public CreateResult mCreateResult;
        
        [TdfMember("SESS")]
        public SessionInfo mSessionInfo;
        
        public enum CreateResult : int
        {
            CREATED = 0x0,
            ASSOCIATED = 0x1,
            ACTIVATED = 0x2,
        }
        
    }
}
