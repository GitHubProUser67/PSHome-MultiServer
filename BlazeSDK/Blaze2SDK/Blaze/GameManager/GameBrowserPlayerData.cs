using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.GameManager
{
    [TdfStruct]
    public struct GameBrowserPlayerData
    {
        
        [TdfMember("EXID")]
        public ulong mExternalId;
        
        [TdfMember("LOC")]
        public uint mAccountLocale;
        
        /// <summary>
        /// Max String Length: 256
        /// </summary>
        [TdfMember("NAME")]
        [StringLength(256)]
        public string mPlayerName;
        
        /// <summary>
        /// Max Key String Length: 32
        /// Max Value String Length: 256
        /// </summary>
        [TdfMember("PATT")]
        public SortedDictionary<string, string> mPlayerAttribs;
        
        [TdfMember("PID")]
        public uint mPlayerId;
        
        [TdfMember("TID")]
        public ushort mTeamId;
        
        [TdfMember("TIDX")]
        public ushort mTeamIndex;
        
    }
}
