using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.Clubs
{
    [TdfStruct]
    public struct ClubTickerMessage
    {
        
        /// <summary>
        /// Max String Length: 256
        /// </summary>
        [TdfMember("TIMD")]
        [StringLength(256)]
        public string mMetadata;
        
        /// <summary>
        /// Max String Length: 2304
        /// </summary>
        [TdfMember("TITX")]
        [StringLength(2304)]
        public string mText;
        
        [TdfMember("TSTM")]
        public uint mTimestamp;
        
    }
}
