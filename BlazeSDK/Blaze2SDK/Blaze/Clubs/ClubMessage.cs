using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.Clubs
{
    [TdfStruct]
    public struct ClubMessage
    {
        
        [TdfMember("CLID")]
        public uint mClubId;
        
        /// <summary>
        /// Max String Length: 30
        /// </summary>
        [TdfMember("CLNM")]
        [StringLength(30)]
        public string mClubName;
        
        [TdfMember("INID")]
        public uint mMessageId;
        
        [TdfMember("INVT")]
        public MessageType mMessageType;
        
        [TdfMember("RECV")]
        public uint mReceiverId;
        
        /// <summary>
        /// Max String Length: 32
        /// </summary>
        [TdfMember("RPRS")]
        [StringLength(32)]
        public string mReceiverPersona;
        
        [TdfMember("SEND")]
        public uint mSenderId;
        
        /// <summary>
        /// Max String Length: 32
        /// </summary>
        [TdfMember("SPRS")]
        [StringLength(32)]
        public string mSenderPersona;
        
        [TdfMember("TIME")]
        public uint mTimeSent;
        
    }
}
