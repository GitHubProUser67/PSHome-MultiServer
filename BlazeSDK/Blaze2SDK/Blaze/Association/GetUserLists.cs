using Tdf;

namespace Blaze2SDK.Blaze.Association
{
    [TdfStruct]
    public struct GetUserLists
    {
        
        [TdfMember("ALST")]
        public List<ListSetting> mListSettingVector;
        
    }
}
