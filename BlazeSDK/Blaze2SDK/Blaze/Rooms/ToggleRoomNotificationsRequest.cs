using Tdf;

namespace Blaze2SDK.Blaze.Rooms
{
    [TdfStruct]
    public struct ToggleRoomNotificationsRequest
    {
        
        [TdfMember("ENBL")]
        public bool mReceiveNotifications;
        
    }
}
