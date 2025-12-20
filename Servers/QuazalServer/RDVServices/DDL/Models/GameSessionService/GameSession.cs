using QuazalServer.QNetZ.DDL;

namespace QuazalServer.RDVServices.DDL.Models
{
	public enum GameType
	{
		FreeForAll = 1,
		Race = 2,
		Takedown = 3,
		Team = 4,
		ShiftRace = 5,
		TeamRace = 6
	}

	public enum GameSessionAttributeType : uint
	{
		PublicSlots = 3,
		PrivateSlots = 4,
		FilledPublicSlots = 5,
		FilledPrivateSlots = 6,

		IsPrivate = 7,			// supposed to be, not even checked yet

		FreePublicSlots = 50,	// used internally by game
		FreePrivateSlots = 51,  // used internally by game

		UnknownAttribute102 = 102, // always 201313820
		UnknownAttribute103 = 103, // always 0
		HostRVCID = 105,
		UnknownAttribute106 = 106, // totally unknown, different values - qm=4 ffa=4 race=1 takedown=4 ffa=0 team=0 sr=0 tr=4
		UnknownAttribute107 = 107, // always 3
		UnknownAttribute108 = 108, // always 0
		GameType = 109,
		GameTypeMin = 110,
		GameTypeMax = 111,
		UnknownAttribute112 = 112, // always 1

		// TODO: other parameters

		/*
		XBOX:
			MM_SEARCH_QUERY_ID = 0,
			MM_HOST_PARAM_MAX_PUBLIC_SLOTS = 2,
			MM_HOST_PARAM_MAX_PRIVATE_SLOTS = 3,
			MM_SESSION_FILLED_PUB_SLOTS = 4,
			MM_SESSION_FREE_PUB_SLOTS = 5,
			MM_SESSION_FILLED_PRIV_SLOTS = 8,
			MM_SESSION_FREE_PRIV_SLOTS = 9,
			MM_SESSION_GAME_MODE = 10,
			MM_SESSION_GAME_TYPE = 11, // in some place it were 25
		*/
	}

	public class GameSessionProperty
	{
		public uint ID { get; set; }
		public uint Value { get; set; }
	}

	public class GameSession
	{
		public GameSession()
		{
			m_attributes = new List<GameSessionProperty>();
		}
		public uint m_typeID { get; set; }
		public ICollection<GameSessionProperty> m_attributes { get; set; }
	}

	public class GameSessionKey
	{
		public uint m_typeID { get; set; }
		public uint m_sessionID { get; set; }
	}

	public class GameSessionUpdate
	{
		public GameSessionKey? m_sessionKey { get; set; }
		public ICollection<GameSessionProperty>? m_attributes { get; set; }
	}

    public class GameSessionParticipant
    {
        public uint m_pid { get; set; }
        public string? m_name { get; set; }
        public ICollection<StationURL>? m_station_ur_ls { get; set; }
    }

    public class GameSessionInvitationSent
    {
        public GameSessionKey? m_session_key { get; set; }
        public uint m_recipient_pid { get; set; }
        public string? m_message { get; set; }
        public DateTime? m_creation_time { get; set; }
    }

    public class GameSessionInvitationReceived
    {
        public GameSessionKey? m_session_key { get; set; }
        public uint m_sender_pid { get; set; }
        public string? m_message { get; set; }
        public DateTime? m_creation_time { get; set; }
    }

    public class GameSessionQuery
    {
        public uint m_type_id { get; set; }
        public uint m_query_id { get; set; }
        public ICollection<GameSessionProperty>? m_parameters { get; set; }
    }

    public class GameSessionSocialQuery
    {
        public uint m_type_id { get; set; }
        public uint m_query_id { get; set; }
        public ICollection<GameSessionProperty>? m_parameters { get; set; }
        public ICollection<uint>? m_participant_i_ds { get; set; }
    }

    public class GameSessionMessage
    {
        public GameSessionKey? m_session_key { get; set; }
        public string? m_message { get; set; }
    }

    public class GameSessionSearchWithParticipantsResult
    {
        public GameSessionSearchResult? m_search_result { get; set; }
        public ICollection<uint>? m_participant_i_ds { get; set; }
    }

    public class GameSessionUnsuccessfulJoinSession
    {
        public GameSessionKey? m_session_key { get; set; }
        public uint m_error_category { get; set; }
        public uint m_error_code { get; set; }
    }

    public class GameSessionSearchResult
	{
		public GameSessionSearchResult()
		{
			m_sessionKey = new GameSessionKey();
			m_hostURLs = new List<StationURL>();
			m_attributes = new List<GameSessionProperty>();
		}

		public GameSessionKey m_sessionKey { get; set; }
		public uint m_hostPID { get; set; }
		public ICollection<StationURL> m_hostURLs { get; set; }
		public ICollection<GameSessionProperty> m_attributes { get; set; }
	}
}
