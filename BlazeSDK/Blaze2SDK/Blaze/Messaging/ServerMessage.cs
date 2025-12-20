using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.Messaging
{
    [TdfStruct]
    public struct ServerMessage
    {
        
        [TdfMember("FLAG")]
        public ServerFlags mFlags;
        
        [TdfMember("MGID")]
        public uint mMessageId;
        
        /// <summary>
        /// Max String Length: 256
        /// </summary>
        [TdfMember("NAME")]
        [StringLength(256)]
        public string mSourceName;
        
        [TdfMember("PYLD")]
        public ClientMessage mPayload;
        
        [TdfMember("SRCE")]
        public ulong mSource;
        
        [TdfMember("TIME")]
        public uint mTimestamp;
        
    }
}
