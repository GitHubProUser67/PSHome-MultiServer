using Tdf;

namespace Blaze2SDK.Blaze.Association
{
    [TdfStruct]
    public struct SubscriptionInfo
    {
        
        /// <summary>
        /// Max String Length: 32
        /// </summary>
        [TdfMember("SUBR")]
        public List<string> mListNameVector;
        
    }
}
