using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.Util
{
    [TdfStruct]
    public struct PreAuthResponse
    {
        
        [TdfMember("CIDS")]
        public List<ushort> mComponentIds;
        
        [TdfMember("CONF")]
        public FetchConfigResponse mConfig;
        
        [TdfMember("QOSS")]
        public QosConfigInfo mQosSettings;
        
        /// <summary>
        /// Max String Length: 256
        /// </summary>
        [TdfMember("SVER")]
        [StringLength(256)]
        public string mServerVersion;
        
    }
}
