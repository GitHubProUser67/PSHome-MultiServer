using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze
{
    [TdfStruct]
    public struct GeoLocationData
    {
        
        /// <summary>
        /// Max String Length: 3
        /// </summary>
        [TdfMember("CNTY")]
        [StringLength(3)]
        public string mCountry;
        
        /// <summary>
        /// Max String Length: 256
        /// </summary>
        [TdfMember("CTY")]
        [StringLength(256)]
        public string mCity;
        
        [TdfMember("ID")]
        public uint mBlazeId;
        
        [TdfMember("LAT")]
        public int mLatitude;
        
        [TdfMember("LON")]
        public int mLongitude;
        
        [TdfMember("OPT")]
        public bool mOptOut;
        
        /// <summary>
        /// Max String Length: 3
        /// </summary>
        [TdfMember("ST")]
        [StringLength(3)]
        public string mStateRegion;
        
    }
}
