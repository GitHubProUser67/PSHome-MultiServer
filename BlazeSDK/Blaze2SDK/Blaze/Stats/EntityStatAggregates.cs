using Tdf;

namespace Blaze2SDK.Blaze.Stats
{
    [TdfStruct]
    public struct EntityStatAggregates
    {
        
        /// <summary>
        /// Max String Length: 256
        /// </summary>
        [TdfMember("AGGR")]
        public List<string> mAggrValues;
        
        [TdfMember("TYPE")]
        public AggregateType mType;
        
    }
}
