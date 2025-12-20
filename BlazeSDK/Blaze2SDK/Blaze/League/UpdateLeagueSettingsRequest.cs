using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.League
{
    [TdfStruct]
    public struct UpdateLeagueSettingsRequest
    {

        /// <summary>
        /// Max String Length: 6
        /// </summary>
        [TdfMember("ABBR")]
        [StringLength(6)]
        public string mAbbrev;

        /// <summary>
        /// Max String Length: 65
        /// </summary>
        [TdfMember("DESC")]
        [StringLength(65)]
        public string mDescription;

        [TdfMember("LFLG")]
        public LeagueFlags mLeagueFlags;

        [TdfMember("LGID")]
        public uint mLeagueId;

        [TdfMember("LOGO")]
        public ushort mLogo;

        [TdfMember("OPTS")]
        public List<uint> mOptions;

        /// <summary>
        /// Max String Length: 13
        /// </summary>
        [TdfMember("PASS")]
        [StringLength(13)]
        public string mPassword;

        [TdfMember("TRPH")]
        public uint mTrophy;

    }
}
