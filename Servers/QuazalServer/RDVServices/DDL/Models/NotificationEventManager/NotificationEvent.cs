using System.Text;

namespace QuazalServer.RDVServices.DDL.Models
{
	public enum NotificationEventsType
	{
		FriendEvent				= 1,
		SessionLaunched			= 2,
		ParticipationEvent		= 3,
		OwnershipChangeEvent	= 4,
		FriendStatusChangeEvent	= 5,
		ForceDisconnectEvent	= 6,
		GameSessionEvent		= 7,
		FirstUserNotification	= 1000,
		 // Hermes::CustomNotificationEvents start
		SwissRoundTournament	= 1001,
		MetaSession				= 1002,
		Clans					= 1003,
		HermesPartySession		= 1004,
		PartyProbeMatchmaking	= 1005,
		PartyJoinMatchmaking	= 1006,
		Statistics				= 1008,
	};

	// some events:
	// FriendStatusChangeEvent param0 = PID, param1 = status
	// GameSessionEvent(7) subtype 3 - matchmaking game found?
	// GameSessionEvent(7) subtype 13 - when other player disconnects

	// see PlatformListenerServiceRDV::ProcessNotificationEvent in game
	public class NotificationEvent
	{
		public NotificationEvent()
		{

		}
		public NotificationEvent(NotificationEventsType type, uint subType)
		{
			m_uiType = (uint)type * 1000 + subType;
		}

		public uint m_pidSource { get; set; }

		// NotificationType
		public uint m_uiType { get; set; }
		public uint m_uiParam1 { get; set; }
		public uint m_uiParam2 { get; set; }
		public string? m_strParam { get; set; }
		public uint m_uiParam3 { get; set; }

		public override string ToString()
		{
			var stringBuilder = new StringBuilder();

			var type = (m_uiType - (m_uiType % 1000)) / 1000;
			var subType = m_uiType % 1000;

			stringBuilder.AppendLine("NotificationEvent {");
			stringBuilder.AppendLine($"    m_pidSource = { m_pidSource }");
			stringBuilder.AppendLine($"    m_uiType = ({ (NotificationEventsType)type }, { subType })");
			stringBuilder.AppendLine($"    m_uiParam1 = { m_uiParam1 }");
			stringBuilder.AppendLine($"    m_uiParam2 = { m_uiParam2 }");
			stringBuilder.AppendLine($"    m_strParam = { m_strParam }");
			stringBuilder.AppendLine($"    m_uiParam3 = { m_uiParam3 }");
			stringBuilder.AppendLine("}");

			return stringBuilder.ToString();
		}
}
}
