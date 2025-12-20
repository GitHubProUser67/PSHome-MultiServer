using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    [MediusMessage(NetMessageClass.MessageClassLobby, MediusLobbyMessageIds.LadderList_ExtraInfo0)]
    public class MediusLadderList_ExtraInfoRequest0 : MediusLadderList_ExtraInfoRequest, IMediusRequest
    {
        public override byte PacketType => (byte)MediusLobbyMessageIds.LadderList_ExtraInfo0;

    }
}