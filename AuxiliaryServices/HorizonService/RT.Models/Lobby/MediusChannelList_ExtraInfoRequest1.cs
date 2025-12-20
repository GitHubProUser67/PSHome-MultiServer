using Horizon.RT.Common;

namespace Horizon.RT.Models
{
    [MediusMessage(NetMessageClass.MessageClassLobbyExt, MediusLobbyExtMessageIds.ChannelList_ExtraInfo1)]
    public class MediusChannelList_ExtraInfoRequest1 : MediusChannelList_ExtraInfoRequest, IMediusRequest
    {
        public override byte PacketType => (byte)MediusLobbyExtMessageIds.ChannelList_ExtraInfo1;
    }
}