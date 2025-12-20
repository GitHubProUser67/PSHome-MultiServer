using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.Redirector
{
    [TdfStruct]
    public struct ServerInstance
    {
        
        /// <summary>
        /// Max String Length: 256
        /// </summary>
        [TdfMember("CWD")]
        [StringLength(256)]
        public string mCurrentWorkingDirectory;
        
        [TdfMember("ENDP")]
        public List<ServerEndpointInfo> mEndpoints;
        
        [TdfMember("ID")]
        public int mInstanceId;
        
        [TdfMember("LOAD")]
        public int mLoad;
        
        /// <summary>
        /// Max String Length: 256
        /// </summary>
        [TdfMember("NAME")]
        [StringLength(256)]
        public string mInstanceName;
        
    }
}
