using Tdf;

namespace Blaze3SDK.Blaze.Rooms
{
	[TdfStruct]
	public struct ToggleRoomNotificationsRequest
	{

		[TdfMember("ENBL")]
		public bool mReceiveNotifications;

	}
}
