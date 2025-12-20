using Tdf;

namespace Blaze2SDK.Blaze.Redirector
{
    [TdfStruct]
    public struct ServerInstanceError
    {
        
        /// <summary>
        /// Max String Length: 1024
        /// </summary>
        [TdfMember("MSGS")]
        public List<string> mMessages;
        
    }
}
