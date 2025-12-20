using Tdf;

namespace Blaze2SDK.Blaze.League
{
    [TdfStruct]
    public struct PositionPref
    {
        
        [TdfMember("RDPF")]
        public List<sbyte> mPreferences;
        
    }
}
