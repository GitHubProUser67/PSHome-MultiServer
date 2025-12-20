using System;

namespace Horizon.RT.Common
{
    #region RT_MSG
    public enum RT_MSG_TYPE : byte
    {
        RT_MSG_CLIENT_CONNECT_TCP,
        RT_MSG_CLIENT_DISCONNECT,
        RT_MSG_CLIENT_APP_BROADCAST,
        RT_MSG_CLIENT_APP_SINGLE,
        RT_MSG_CLIENT_APP_LIST,
        RT_MSG_CLIENT_ECHO,
        RT_MSG_SERVER_CONNECT_REJECT,
        RT_MSG_SERVER_CONNECT_ACCEPT_TCP,
        RT_MSG_SERVER_CONNECT_NOTIFY,
        RT_MSG_SERVER_DISCONNECT_NOTIFY,
        RT_MSG_SERVER_APP,
        RT_MSG_CLIENT_APP_TOSERVER,
        RT_MSG_UDP_APP,
        RT_MSG_CLIENT_SET_RECV_FLAG,
        RT_MSG_CLIENT_SET_AGG_TIME,
        RT_MSG_CLIENT_FLUSH_ALL,
        RT_MSG_CLIENT_FLUSH_SINGLE,
        RT_MSG_SERVER_FORCED_DISCONNECT,
        RT_MSG_CLIENT_CRYPTKEY_PUBLIC,
        RT_MSG_SERVER_CRYPTKEY_PEER,
        RT_MSG_SERVER_CRYPTKEY_GAME,
        RT_MSG_CLIENT_CONNECT_TCP_AUX_UDP,
        RT_MSG_CLIENT_CONNECT_AUX_UDP,
        RT_MSG_CLIENT_CONNECT_READY_AUX_UDP,
        RT_MSG_SERVER_INFO_AUX_UDP,
        RT_MSG_SERVER_CONNECT_ACCEPT_AUX_UDP,
        RT_MSG_SERVER_CONNECT_COMPLETE,
        RT_MSG_CLIENT_CRYPTKEY_PEER,
        RT_MSG_SERVER_SYSTEM_MESSAGE,
        RT_MSG_SERVER_CHEAT_QUERY,
        RT_MSG_SERVER_MEMORY_POKE,
        RT_MSG_SERVER_ECHO,
        RT_MSG_CLIENT_DISCONNECT_WITH_REASON,
        RT_MSG_CLIENT_CONNECT_READY_TCP,
        RT_MSG_SERVER_CONNECT_REQUIRE,
        RT_MSG_CLIENT_CONNECT_READY_REQUIRE,
        RT_MSG_CLIENT_HELLO,
        RT_MSG_SERVER_HELLO,
        RT_MSG_SERVER_STARTUP_INFO_NOTIFY,
        RT_MSG_CLIENT_PEER_QUERY,
        RT_MSG_SERVER_PEER_QUERY_NOTIFY,
        RT_MSG_CLIENT_PEER_QUERY_LIST,
        RT_MSG_SERVER_PEER_QUERY_LIST_NOTIFY,
        RT_MSG_CLIENT_WALLCLOCK_QUERY,
        RT_MSG_SERVER_WALLCLOCK_QUERY_NOTIFY,
        RT_MSG_CLIENT_TIMEBASE_QUERY,
        RT_MSG_SERVER_TIMEBASE_QUERY_NOTIFY,
        RT_MSG_CLIENT_TOKEN_MESSAGE,
        RT_MSG_SERVER_TOKEN_MESSAGE,
        RT_MSG_CLIENT_SYSTEM_MESSAGE,
        RT_MSG_CLIENT_APP_BROADCAST_QOS,
        RT_MSG_CLIENT_APP_SINGLE_QOS,
        RT_MSG_CLIENT_APP_LIST_QOS,
        RT_MSG_CLIENT_APP_GROUP_LIST,
        RT_MSG_CLIENT_APP_GROUP_LIST_QOS,
        RT_MSG_CLIENT_JOIN_GROUP,
        RT_MSG_CLIENT_LEAVE_GROUP,
        RT_MSG_CLIENT_JOIN_GROUP_LIST,
        RT_MSG_CLIENT_LEAVE_GROUP_LIST,
        //RT_MSG_CLIENT_APP_FILTER,
        //RT_MSG_CLIENT_APP_FILTER_QOS,
        RT_MSG_CLIENT_MULTI_APP_TOSERVER = 59,
        RT_MSG_SERVER_MULTI_APP_TOCLIENT,
        RT_MSG_CLIENT_APP_TO_PLUGIN,
        RT_MSG_SERVER_PLUGIN_TO_APP,
        RT_MSG_CLIENT_AGGREGATE_MESSAGE,
        RT_MSG_SERVER_AGGREGATE_MESSAGE,
        RT_MSG_CLIENT_MAX_MSGLEN,
        RT_MSG_SERVER_MAX_MSGLEN

    }

    public enum RT_MSG_CONNECT_REJECT_REASON : byte
    {
        RT_MSG_REJECT_VERSION,
        RT_MSG_REJECT_WORLD_FULL,
        RT_MSG_REJECT_WORLD_ID,
        RT_MSG_REJECT_APP_SIGNATURE,
        RT_MSG_REJECT_ENCRYPTION,
        RT_MSG_REJECT_ACCESS_KEY,
        RT_MSG_REJECT_AUX_UDP_FAILURE
    }

    public enum RT_MSG_CLIENT_DISCONNECT_REASON : byte
    {
        RT_MSG_CLIENT_DISCONNECT_NONE,
        RT_MSG_CLIENT_DISCONNECT_NORMAL,
        RT_MSG_CLIENT_DISCONNECT_CONNECT_FAIL,
        RT_MSG_CLIENT_DISCONNECT_STREAMMEDIA_FAIL,
        RT_MSG_CLIENT_DISCONNECT_UPDATE_FAIL,
        RT_MSG_CLIENT_DISCONNECT_INACTIVITY,
        RT_MSG_CLIENT_DISCONNECT_SHUTDOWN,
        RT_MSG_CLIENT_DISCONNECT_LENGTH_MISMATCH,
        MAX_RT_MSG_CLIENT_DISCONNECT_REASON
    }

    public enum RT_MSG_FORCED_DISCONNECT_REASON : int
    {
        RT_MSG_FORCED_DISCONNECT_NONE,
        RT_MSG_FORCED_DISCONNECT_ERROR,
        RT_MSG_FORCED_DISCONNECT_SHUTDOWN,
        RT_MSG_FORCED_DISCONNECT_END_SESSION,
        RT_MSG_FORCED_DISCONNECT_END_GAME,
        RT_MSG_FORCED_DISCONNECT_INACTIVITY,
        RT_MSG_FORCED_DISCONNECT_BAD_PERF,
        RT_MSG_FORCED_DISCONNECT_BANNED
    }

    public enum RT_MSG_SERVER_CONNECTION_EVENT : int
    {
        RT_MSG_SERVER_CLIENT_DISCONNECT,
        RT_MSG_SERVER_CLIENT_CONNECT,
    }

    public enum RT_RESULT : int
    {
        RT_RESULT_OK,
        RT_RESULT_ERROR = 1,
        RT_INVALID_PARAM = 2,
        RT_NULL_PTR_ERROR = 3,
        RT_NOT_IMPLEMENTED = 4,
        RT_NOT_INITIALIZED = 5,
        RT_ERROR_UNKNOWN = 6,
        RT_NOT_SUPPORTED = 7,
        RT_NO_SPC = 8,
        RT_INVALID_CSTRING = 9,
        RT_MEMORY_ALLOC_ERROR = 64,
    }

    public enum RT_TOKEN_MESSAGE_TYPE : byte
    {
        RT_TOKEN_UNKNOWN_MESSAGE = 0,
        RT_TOKEN_CLIENT_QUERY = 1,
        RT_TOKEN_CLIENT_REQUEST = 2,
        RT_TOKEN_CLIENT_RELEASE = 3,
        RT_TOKEN_CLIENT_LIST_QUERY = 4,
        RT_TOKEN_CLIENT_LIST_REQUEST = 5,
        RT_TOKEN_CLIENT_LIST_RELEASE = 6,
        RT_TOKEN_SERVER_OWNED = 7,
        RT_TOKEN_SERVER_GRANTED = 8,
        RT_TOKEN_SERVER_FREED = 9,
        RT_TOKEN_SERVER_RELEASED = 0xA,
        RT_TOKEN_SERVER_LIST_OWNED = 0xB,
        RT_TOKEN_SERVER_LIST_GRANTED = 0xC,
        RT_TOKEN_SERVER_LIST_FREED = 0xD,
        RT_TOKEN_SERVER_LIST_RELEASED = 0xE,
        RT_TOKEN_SERVER_OWNER_REMOVED = 0xF
    }

    #endregion

    public enum SECURITY_MODE : int
    {
        MODE_UNKNOWN,
        MODE_SECURED,
        MODE_UNSECURED
    }

    public enum MediusAssignedGameType : int
    {
        AssignedGameTypeMMS = 0,
        AssignedGameTypeTournament = 1,
    }

    #region MediusCallbackStatus
    public enum MediusCallbackStatus : int
    {
        /// <summary>
        /// Session begin failed
        /// </summary>
        MediusBeginSessionFailed = -1000,
        /// <summary>
        /// Account already exists, can not register with the same account name.
        /// </summary>
        MediusAccountAlreadyExists = -999,
        /// <summary>
        /// Account name was not found.
        /// </summary>
        MediusAccountNotFound = -998,
        /// <summary>
        /// The account is marked as already being logged in to the system. 
        /// </summary>
        MediusAccountLoggedIn = -997,
        /// <summary>
        /// Unable to properly end the session.
        /// </summary>
        MediusEndSessionFailed = -996,
        /// <summary>
        /// Login failed.
        /// </summary>
        MediusLoginFailed = -995,
        /// <summary>
        /// Registration failed.
        /// </summary>
        MediusRegistrationFailed = -994,
        /// <summary>
        /// The login step was incorrect.  For example, login without having a session.
        /// </summary>
        MediusIncorrectLoginStep = -993,
        /// <summary>
        /// The user is already the leader of a clan, and can not be the leader of multiple clans.
        /// </summary>
        MediusAlreadyLeaderOfClan = -992,
        /// <summary>
        /// World Manager error.
        /// </summary>
        MediusWMError = -991,
        /// <summary>
        /// The player attempted some request that requires being the leader of the clan.
        /// </summary>
        MediusNotClanLeader = -990,
        /// <summary>
        /// The player is not privileged to make the request. <Br></Br> 
        /// Typically, the user’s session has been destroyed, but is still connected to the server.
        /// </summary>
        MediusPlayerNotPrivileged = -989,
        /// <summary>
        /// An internal database error occurred.
        /// </summary>
        MediusDBError = -988,
        /// <summary>
        /// A DME layer error.
        /// </summary>
        MediusDMEError = -987,
        /// <summary>
        /// The maximum number of worlds has been exceeded.
        /// </summary>
        MediusExceedsMaxWorlds = -986,
        /// <summary>
        /// The request has been denied.
        /// </summary>
        MediusRequestDenied = -985,
        /// <summary>
        /// Setting the game list filter failed.
        /// </summary>
        MediusSetGameListFilterFailed = -984,
        /// <summary>
        /// Clearing the game list filter failed.
        /// </summary>
        MediusClearGameListFilterFailed = -983,
        /// <summary>
        /// Getting the game list filter failed.
        /// </summary>
        MediusGetGameListFilterFailed = -982,
        /// <summary>
        /// The number of filters is at the maximum.
        /// </summary>
        MediusNumFiltersAtMax = -981,
        /// <summary>
        /// The filter being referenced does not exist.
        /// </summary>
        MediusFilterNotFound = -980,
        /// <summary>
        /// The request message was invalid.
        /// </summary>
        MediusInvalidRequestMsg = -979,
        /// <summary>
        /// The specified password was invalid.
        /// </summary>
        MediusInvalidPassword = -978,
        /// <summary>
        /// The game was not found.
        /// </summary>
        MediusGameNotFound = -977,
        /// <summary>
        /// The channel was not found.
        /// </summary>
        MediusChannelNotFound = -976,
        /// <summary>
        /// The game name already exists.
        /// </summary>
        MediusGameNameExists = -975,
        /// <summary>
        /// The channel name already exists.
        /// </summary>
        MediusChannelNameExists = -974,
        /// <summary>
        /// The game name was not found.
        /// </summary>
        MediusGameNameNotFound = -973,
        /// <summary>
        /// The player has been banned.
        /// </summary>
        MediusPlayerBanned = -972,
        /// <summary>
        /// The clan was not found.
        /// </summary>
        MediusClanNotFound = -971,
        /// <summary>
        /// The clan name already exists.
        /// </summary>
        MediusClanNameInUse = -970,
        /// <summary>
        /// Session key is invalid.
        /// </summary>
        MediusSessionKeyInvalid = -969,
        /// <summary>
        /// The text string is invalid.
        /// </summary>
        MediusTextStringInvalid = -968,
        /// <summary>
        /// The filtering failed.
        /// </summary>
        MediusFilterFailed = -967,
        /// <summary>
        /// General fail message.
        /// </summary>
        MediusFail = -966,
        /// <summary>
        /// Medius File Services (MFS) Internal error.
        /// </summary>
        MediusFileInternalAccessError = -965,
        /// <summary>
        /// Insufficient permissions for the MFS request.
        /// </summary>
        MediusFileNoPermissions = -964,
        /// <summary>
        /// The file requested in MFS does not exist.
        /// </summary>
        MediusFileDoesNotExist = -963,
        /// <summary>
        /// The file requested in MFS already exists.
        /// </summary>
        MediusFileAlreadyExists = -962,
        /// <summary>
        /// The filename is not valid in MFS.
        /// </summary>
        MediusFileInvalidFilename = -961,
        /// <summary>
        /// The user’s quota has been exceeded.
        /// </summary>
        MediusFileQuotaExceeded = -960,
        /// <summary>
        /// The cache system had an internal failure.
        /// </summary>
        MediusCacheFailure = -959,
        /// <summary>
        /// The data already exists.
        /// </summary>
        MediusDataAlreadyExists = -958,
        /// <summary>
        /// The data does not exist.
        /// </summary>
        MediusDataDoesNotExist = -957,
        /// <summary>
        /// A maximum count has been exceeded.
        /// </summary>
        MediusMaxExceeded = -956,
        /// <summary>
        /// The key used is incorrect.
        /// </summary>
        MediusKeyError = -955,
        /// <summary>
        /// The application ID is not compatible.
        /// </summary>
        MediusIncompatibleAppID = -954,
        /// <summary>
        /// The account has been banned.
        /// </summary>
        MediusAccountBanned = -953,
        /// <summary>
        /// The machine has been banned.
        /// </summary>
        MediusMachineBanned = -952,
        /// <summary>
        /// The leader of the clan can not leave. <br></br> Must disband instead.
        /// </summary>
        MediusLeaderCannotLeaveClan = -951,
        /// <summary>
        /// The feature requested is not enabled.
        /// </summary>
        MediusFeatureNotEnabled = -950,
        /// <summary>
        /// The same DNAS signature is already logged in.
        /// </summary>
        MediusDNASSignatureLoggedIn = -949,
        /// <summary>
        /// The world is full.  Unable to join.
        /// </summary>
        MediusWorldIsFull = -948,
        /// <summary>
        /// The user is not a member of the clan.
        /// </summary>
        MediusNotClanMember = -947,
        /// <summary>
        /// The server is busy.  Try again later.
        /// </summary>
        MediusServerBusy = -946,
        /// <summary>
        /// The maximum number of game worlds per lobby world has been exceeded.
        /// </summary>
        MediusNumGameWorldsPerLobbyWorldExceeded = -945,
        /// <summary>
        /// The account name is not UC compliant.
        /// </summary>
        MediusAccountNotUCCompliant = -944,
        /// <summary>
        /// The password is not UC compliant.
        /// </summary>
        MediusPasswordNotUCCompliant = -943,
        /// <summary>
        /// There is an internal gateway error.
        /// </summary>
        MediusGatewayError = -942,
        /// <summary>
        /// The transaction has been cancelled.
        /// </summary>
        MediusTransactionCanceled = -941,
        /// <summary>
        /// The session has failed.
        /// </summary>
        MediusSessionFail = -940,
        /// <summary>
        /// The token is already in use.
        /// </summary>
        MediusTokenAlreadyTaken = -939,
        /// <summary>
        /// The token being referenced does not exist.
        /// </summary>
        MediusTokenDoesNotExist = -938,
        /// <summary>
        /// The subscription has been aborted.
        /// </summary>
        MediusSubscriptionAborted = -937,
        /// <summary>
        /// The subscription is invalid.
        /// </summary>
        MediusSubscriptionInvalid = -936,
        /// <summary>
        /// The user is not a member of an list.
        /// </summary>
        MediusNotAMember = -935,
        MediusBillingVerificationRequired = -934,
        /// <summary>
        /// The user's access level is insufficient
        /// </summary>
        MediusAccessLevelInsufficient = -933,
        /// <summary>
        /// The game world is closed
        /// </summary>
        MediusWorldClosed = -932,
        MediusTransactionTimedOut = -931,
        // MediusCASError = -931 //Zipper Interactive
        MediusStepSendFailed = -930,
        MediusMatchTypeNoMatch_DEPRECATED = -929,
        /// <summary>
        /// Medius Matchmaking Server not found
        /// </summary>
        MediusMatchServerNotFound = -928,
        /// <summary>
        /// Matchmake Create Game Failed
        /// </summary>
        MediusMatchGameCreationFailed = -927,
        MediusGameListSortOperationFailed = -926,
        MediusNumSortCriteriaAtMax = -925,
        MediusSortCriteriaNotFound = -924,
        MediusEntitlementCheckFailed = -923,
        ExtraMediusCallbackStatus = -1,
        /// <summary>
        /// Success
        /// </summary>
        MediusSuccess = 0,
        /// <summary>
        /// No results.  This is a valid state.
        /// </summary>
        MediusNoResult = 1,
        /// <summary>
        /// The request has been accepted.
        /// </summary>
        MediusRequestAccepted = 2,
        /// <summary>
        /// The world has been created with reduced size.
        /// </summary>
        MediusWorldCreatedSizeReduced = 3,
        /// <summary>
        /// The criteria has been met.
        /// </summary>
        MediusPass = 4,
        /// <summary>
        /// Join Queue for PS3 TicketLogin
        /// </summary>
        MediusInQueue = 5,
        /// <summary>
        /// Join Assigned Game pre-determined by server
        /// </summary>
        MediusJoinAssignedGame = 6,
        /// <summary>
        /// If no games exist for matchmaking host one pre-determined by database/server
        /// </summary>
        MediusMatchTypeHostGame = 7,
        /// <summary>
        /// Referral to Medius Matchmaking Service <br></br> 
        /// 3rd possible response from MediusMatchPartyResponse 
        /// </summary>
        MediusMatchTypeReferral = 8,
        /// <summary>
        /// MediusJoinLeastPopulatedChannel return MediusAlreadyInLeastPopulatedChannel
        /// </summary>
        MediusAlreadyInLeastPopulatedChannel = 9,
        /// <summary>
        /// 
        /// </summary>
        MediusVulgarityFound = 10,
        /// <summary>
        /// Matchmaking in progress
        /// </summary>
        MediusMatchingInProgress = 11,
    }
    #endregion

    #region MediusAccessLevelType
    public enum MediusAccessLevelType : uint
    {
        MEDIUS_ACCESSLEVEL_DEFAULT,
        MEDIUS_ACCESSLEVEL_PRIVILEGED,
        MEDIUS_ACCESSLEVEL_BILLING_VERIFIED,
        MEDIUS_ACCESSLEVEL_MODERATOR = 4,
        MEDIUS_ACCESSLEVEL_RESERVED_3 = 8,
        MEDIUS_ACCESSLEVEL_RESERVED_4 = 0x10,
        MEDIUS_ACCESSLEVEL_RESERVED_5 = 0x20,
        MEDIUS_ACCESSLEVEL_RESERVED_6 = 0x40,
        MEDIUS_ACCESSLEVEL_RESERVED_7 = 0x80,
        MEDIUS_ACCESSLEVEL_RESERVED_8 = 0x100,
        MEDIUS_ACCESSLEVEL_RESERVED_9 = 0x200,
        MEDIUS_ACCESSLEVEL_RESERVED_10 = 0x400,
        MEDIUS_ACCESSLEVEL_RESERVED_11 = 0x800,
        MEDIUS_ACCESSLEVEL_RESERVED_12 = 0x1000,
        MEDIUS_ACCESSLEVEL_RESERVED_13 = 0x2000,
        MEDIUS_ACCESSLEVEL_RESERVED_14 = 0x4000,
        MEDIUS_ACCESSLEVEL_RESERVED_15 = 0x8000,
        MEDIUS_ACCESSLEVEL_RESERVED_16 = 0x10000,
        MEDIUS_ACCESSLEVEL_RESERVED_17 = 0x20000,
        MEDIUS_ACCESSLEVEL_RESERVED_18 = 0x40000,
        MEDIUS_ACCESSLEVEL_RESERVED_19 = 0x80000,
        MEDIUS_ACCESSLEVEL_RESERVED_20 = 0x100000,
        MEDIUS_ACCESSLEVEL_RESERVED_21 = 0x200000,
        MEDIUS_ACCESSLEVEL_RESERVED_22 = 0x400000,
        MEDIUS_ACCESSLEVEL_RESERVED_23 = 0x800000,
        MEDIUS_ACCESSLEVEL_EXTRA = 0xFFFFFF,
        MEDIUS_ACCESSLEVEL_RESERVED_24 = 0x1000000,
        MEDIUS_ACCESSLEVEL_RESERVED_25 = 0x2000000,
        MEDIUS_ACCESSLEVEL_RESERVED_26 = 0x4000000,
        MEDIUS_ACCESSLEVEL_RESERVED_27 = 0x8000000,
        MEDIUS_ACCESSLEVEL_RESERVED_28 = 0x10000000,
        MEDIUS_ACCESSLEVEL_RESERVED_29 = 0x20000000,
        MEDIUS_ACCESSLEVEL_RESERVED_30 = 0x40000000,
        MEDIUS_ACCESSLEVEL_RESERVED_31 = 0x80000000,
    }
    #endregion

    public enum MediusPasswordType : int
    {
        MediusPasswordNotSet,
        MediusPasswordSet,
    }

    public enum MediusAccountType : int
    {
        MediusChildAccount,
        MediusMasterAccount,
    }

    #region MediusBuddyAddType
    /// <summary>
    /// Introduced in Medius Library version (v1.50)
    /// There is a new enumeration type called MediusBuddyAddType.  When set to 
    /// AddSymmetric, then when a player accepts your buddy invitation, you will
    /// automatically be updated in their buddy list as well.Default behaviour of
    /// Medius is to require both users to invite each other, AddSymmetric requires
    /// only one user to invite.
    /// </summary>
    public enum MediusBuddyAddType : int
    {
        /// <summary>
        /// Add User to your Buddy List,
        /// but without the requirement that the buddy see you on their list
        /// </summary>
        AddSingle,
        /// <summary>
        /// Request that each person appears on the other's buddy list.
        /// </summary>
        AddSymmetric
    }
    #endregion

    public enum MediusLadderType : int
    {
        MediusLadderTypePlayer = 0,
        MediusLadderTypeClan = 1,
    }

    #region MediusPlayerStatus
    /// <summary>
    /// Defines current activity status of player.
    /// </summary>
    public enum MediusPlayerStatus : int
    {
        /// <summary>
        /// Player is currently not connected.
        /// </summary>
        MediusPlayerDisconnected,
        /// <summary>
        /// Player is currently on an authentication world.
        /// </summary>
        MediusPlayerInAuthWorld,
        /// <summary>
        /// Player is currently in a chat channel.
        /// </summary>
        MediusPlayerInChatWorld,
        /// <summary>
        /// Player is currently in a game world.
        /// </summary>
        MediusPlayerInGameWorld,
        /// <summary>
        /// Player is online in some other universe
        /// </summary>
        MediusPlayerInOtherUniverse,
        /// <summary>
        /// Reserved for internal use
        /// </summary>
        LastMediusPLayerStatus,
    }
    #endregion

    #region MediusCharacterEncodingType
    public enum MediusCharacterEncodingType : int
    {
        /// <summary>
        /// No change to current encoding.
        /// </summary>
        MediusCharacterEncoding_NoUpdate,
        /// <summary>
        /// ISO-8859-1 single byte encoding 0x00 - 0xFF.
        /// </summary>
        MediusCharacterEncoding_ISO8859_1,
        /// <summary>
        /// UTF-8 Multibyte Encoding.
        /// </summary>
        MediusCharacterEncoding_UTF8,
        /// <summary>
        /// Placeholder to normalize the field size on different compilers
        /// </summary>
        ExtraMediusCharacterEncodingType = 0xffffff
    }
    #endregion

    public enum MediusLanguageType : int
    {
        MediusLanguage_NoUpdate,
        MediusLanguage_USEnglish,
        MediusLanguage_UKEnglish,
        MediusLanguage_Japanese,
        MediusLanguage_Korean,
        MediusLanguage_Italian,
        MediusLanguage_Spanish,
        MediusLanguage_German,
        MediusLanguage_French,
        MediusLanguage_Dutch,
        MediusLanguage_Portuguese,
        MediusLanguage_Chinese,
        MediusLanguage_Taiwanese,
        MediusLanguage_Finnish,
        MediusLanguage_Norwegian,
    }

    public enum MediusChatChallengeRequest : int
    {
        MediusChatChallengeRequestJoin,
        MediusChatChallengeRequestAceept,
    }

    public enum MediusChatChallengeResponse : int
    {
        MediusChatChallengeNoResponse = 0,
        MediusChatChallengeResponseJoin = 1,
        MediusChatChallengeResponseAccept = 2,
        MediusChatChallengeResponseDenied = 3,
        MediusChatChallengeResponseBusy = 4
    }
    public enum MediusChatStatus : int
    {
        MediusChatStatusNoResponse = 0,
        MediusChatStatusAvailable = 1,
        MediusChatStatusPrivate = 2,
        MediusChatStatusAway = 3,
        MediusChatStatusIdle = 4,
        MediusChatStatusStaging = 5,
        MediusChatStatusLoading = 6,
        MediusChatStatusInGame = 7,
        MediusChatStatusChatHost = 8,
        MediusChatStatusChatClient = 9
    }

    #region MediusClanStatus
    /// <summary>
    /// Whether or not a clan is active
    /// </summary>
    public enum MediusClanStatus : int
    {
        /// <summary>
        /// The clan is active
        /// </summary>
        ClanActive,
        /// <summary>
        /// The chan has been disbanded
        /// </summary>
        ClanDisbanded = -1,
    }
    #endregion

    #region MediusChatToggle
    public enum MediusChatToggle : int
    {
        MEDIUS_CHAT_ENABLE,
        MEDIUS_CHAT_DISABLE,
    }
    #endregion

    #region MediusConnectionType
    /// <summary>
    /// Specify which type of network connection is being used<br></br>
    /// Note, the connection type is set during the initial session begin request.
    /// </summary>
    public enum MediusConnectionType : int
    {
        /// <summary>
        /// The connection is on a modem.
        /// </summary>
        Modem = 0,
        /// <summary>
        /// The connection is on Ethernet.
        /// </summary>
        Ethernet = 1,
        /// <summary>
        /// The connection is wireless
        /// </summary>
        Wireless = 2,
    }
    #endregion

    #region MediusDnasCategory
    /// <summary>
    /// Post the dnas signature for this application<br></br>
    /// The DNAS category must correspond with the type of auth.dat file requested from SCEI.
    /// </summary>
    public enum MediusDnasCategory : int
    {
        /// <summary>
        /// DNAS Console ID
        /// </summary>
        DnasConsoleID,
        /// <summary>
        /// DNAS title ID
        /// </summary>
        DnasTitleID,
        /// <summary>
        /// DNAS disk ID
        /// </summary>
        DnasDiskID,
    }
    #endregion

    #region Medius Device Type
    /// <summary>
    /// Specifies a device type for Account I/O operations
    /// </summary>
    public enum MediusDeviceType : int
    {
        /// <summary>
        /// Use a Memory Card as the target
        /// </summary>
        MEDIUS_MEMCARD,
        /// <summary>
        /// Use the HDD as the target
        /// </summary>
        MEDIUS_HDD,
        /// <summary>
        /// Use Host0 as the target
        /// </summary>
        MEDIUS_HOST0,
        /// <summary>
        /// Placeholder to normalize the field size on different compilers
        /// </summary>
        ExtraMediusDeviceType = 0xffffff
    }
    #endregion

    #region MediusSCETerritory
    /// <summary>
    /// Introduced in Medius 1.40.0023 <br></br>
    /// Changed in Medius 1.40.0036 to now inlucde all possible memory card territories<br></br>
    /// Identifies the appropriate TRC territory for this title, for memory card, and HDD-related operations
    /// </summary>
    public enum MediusSCETerritory : int
    {
        /// <summary>
        /// Sony Computer Entertainment, America
        /// </summary>
        SCEA,
        /// <summary>
        /// Sony Computer Entertainment, Europe
        /// </summary>
        SCEE,
        /// <summary>
        /// Sony Computer Entertainment, Japan
        /// </summary>
        SCEI,
        /// <summary>
        /// Third Party SCEA
        /// </summary>
        SCEA_THIRDPARTY,
        /// <summary>
        /// Third Party SCEE
        /// </summary>
        SCEE_THIRDPARTY,
        /// <summary>
        /// Third Party SCEI
        /// </summary>
        SCEI_THIRDPARTY,
        /// <summary>
        /// Placeholder to normalize the field size on different compilers
        /// </summary>
        ExtraSCETerritoryType = 0xffffff
    }
    #endregion

    #region MediusStoredConfirmationType
    /// <summary>
    /// Error Codes related to storage functions
    /// </summary>
    public enum MediusStoredConfirmationType : int
    {
        /// <summary>
        /// Stored Successfully
        /// </summary>
        MediusStoredSuccess,
        /// <summary>
        /// File not found
        /// </summary>
        MediusStoredFileNotFound = -1,
        /// <summary>
        /// Device not found
        /// </summary>
        MediusStoredDeviceNotFound = -2,
        /// <summary>
        /// Directory Not Found
        /// </summary>
        MediusStoredDirectoryNotFound = -3,
        /// <summary>
        /// File already exists
        /// </summary>
        MediusStoredItemAlreadyExists = -4,
        /// <summary>
        /// Placeholder to normalize the field size on different compilers
        /// </summary>
        ExtraMediusStoredConfirmationType = 0xffffff
    }
    #endregion

    public enum MediusPlayerSearchType : int
    {
        PlayerAccountID,
        PlayerAccountName,
    }

    public enum MediusCrossChatMessageType : int
    {
        StatusRequest,
        StatusResponse,
        StatusUnavailable,
        ChatRequest,
        ChatAccept,
        ChatRequestJoin,
        ChatAcceptJoin,
        ChatUnavailable,
        BinarySignal,
        MaxMsgType
    }

    public enum MediusBinaryMessageType : int
    {
        BroadcastBinaryMsg,
        TargetBinaryMsg,
        BroadcastBinaryMsgAcrossEntireUniverse,
        BroadcastBinaryMsgDeprecated0,
        TargetBinaryMsgDeprecated0,
        BroadcastBinaryMsgAcrossEntireUniverseDeprecated0
    }

    public enum MediusMessageType : int
    {
        /// <summary>
        /// Applies to request to the annoncements message.
        /// </summary>
        AnnouncementMessage,
    }

    [Flags]
    public enum MediusWorldGenericFieldLevelType : int
    {
        MediusWorldGenericFieldLevel0 = 0,
        MediusWorldGenericFieldLevel1 = (1 << 0),
        MediusWorldGenericFieldLevel2 = (1 << 1),
        MediusWorldGenericFieldLevel3 = (1 << 2),
        MediusWorldGenericFieldLevel4 = (1 << 3),
        MediusWorldGenericFieldLevel12 = (1 << 4),
        MediusWorldGenericFieldLevel123 = (1 << 5),
        MediusWorldGenericFieldLevel1234 = (1 << 6),
        MediusWorldGenericFieldLevel23 = (1 << 7),
        MediusWorldGenericFieldLevel234 = (1 << 8),
        MediusWorldGenericFieldLevel34 = (1 << 9),
    }

    public enum MediusUtilTypeWorldPersistence : int
    {
        MediusUtilWorldPersistent,
        MediusUtilWorldNotPersistent
    }

    #region MediusApplicationType
    /// <summary>
    /// Enumeration used to determine the application types within the Medius SDK
    /// </summary>
    public enum MediusApplicationType : int
    {
        /// <summary>
        /// Game type.
        /// </summary>
        MediusAppTypeGame,
        /// <summary>
        /// Lobby chat channel type.
        /// </summary>
        LobbyChatChannel,
    }
    #endregion

    #region MediusTextFilterType
    /// <summary>
    /// Whether a text string submitted to a MediusTextFilter() call should be pass/fail or search-and-replace
    /// </summary>
    public enum MediusTextFilterType : int
    {
        /// <summary>
        /// Type of Filtering: Pass or fail.
        /// </summary>
        MediusTextFilterPassFail = 0,
        /// <summary>
        /// Type of filtering: replace text with strike-out characters.
        /// </summary>
        MediusTextFilterReplace = 1,
        /// <summary>
        /// Placeholder to normalize the field size on different compilers
        /// </summary>
        ExtraMediusTextFilter = 0xffffff
    }
    #endregion

    #region MediusGameListSortDirection

    public enum MediusGameListSortDirection : int
    {
        MEDIUS_SORT_ASCENDING,
        MEDIUS_SORT_DESCENDING,
    }

    #endregion

    public enum MediusSortOrder : int
    {
        MEDIUS_ASCENDING,
        MEDIUS_DESCENDING,
    }

    #region Policy Type
    public enum MediusPolicyType : int
    {
        Usage,
        Privacy,
    }
    #endregion

    #region MediusUserAction
    /// <summary>
    /// User actions to indicate activity within and across worlds.
    /// </summary>
    public enum MediusUserAction : int
    {
        /// <summary>
        /// DEPRECATED NORMALLY<br></br>
        /// Used to denote that the player is still online
        /// </summary>
        KeepAlive,
        /// <summary>
        /// Sent when a player joins a chat world.
        /// </summary>
        JoinedChatWorld,
        /// <summary>
        /// Sent when a player leaves a game world.
        /// </summary>
        LeftGameWorld,
        /// <summary>
        /// Sent when a player leaves a party world.
        /// </summary>
        LeftPartyWorld
    }
    #endregion

    #region MediusJoinType
    /// <summary>
    /// Specifies how a player is attempting to join a game world
    /// </summary>
    public enum MediusJoinType : int
    {
        /// <summary>
        /// Join a game as a normal player.
        /// </summary>
        MediusJoinAsPlayer = 0,
        /// <summary>
        /// Join a game as a spectator.
        /// </summary>
        MediusJoinAsSpectator = 1,
        /// <summary>
        /// Join a game as a large scale spectator.
        /// </summary>
        MediusJoinAsMassSpectator = 2,
    }
    #endregion

    #region MediusMatchGameState
    /// <summary>
    /// Medius Match Options 
    /// Introduced in Medius Client/Server library 3.03
    /// </summary>
    public enum MediusMatchGameState : int
    {
        /// <summary>
        /// Suspend MatchGame
        /// </summary>
        MatchGameStateSuspend,
        /// <summary>
        /// Resume MatchGame
        /// </summary>
        MatchGameStateResume,
        /// <summary>
        /// Stop MatchGame
        /// </summary>
        MatchGameStateStop,
    }
    #endregion

    #region MediusMatchOptions
    /// <summary>
    /// Medius Match Options 
    /// Introduced in Medius Client/Server library 3.03
    /// </summary>
    public enum MediusMatchOptions : int
    {
        /// <summary>
        /// No Match Options
        /// </summary>
        MatchOptionsNone,
        /// <summary>
        /// Join match late
        /// </summary>
        MatchLateJoin,
        /// <summary>
        /// Custom Match Game
        /// </summary>
        MatchCustomGame
    }
    #endregion

    /// <summary>
    /// Medius Time Zones
    /// </summary>
    public enum MediusTimeZone : int
    {
        /// <summary>
        /// [GMT-12] IDLW International Date Line - West
        /// </summary>
        MediusTimeZone_IDLW = -1200,
        /// <summary>
        /// [GMT-10] Hawaiian Standard Time
        /// </summary>
        MediusTimeZone_HST = -1000,
        MediusTimeZone_AKST = -900,
        MediusTimeZone_AKDT = -800,
        /// <summary>
        /// [GMT-8] Pacific Standard Time
        /// </summary>
        MediusTimeZone_PST = -801,
        /// <summary>
        /// [GMT-7] Pacific Daylight Time
        /// </summary>
        MediusTimeZone_PDT = -700,
        /// <summary>
        /// [GMT-7] Mountain Standard Time
        /// </summary>
        MediusTimeZone_MST = -701,
        /// <summary>
        /// [GMT-6] Mountain Daylight Time
        /// </summary>
        MediusTimeZone_MDT = -600,
        /// <summary>
        /// [GMT-6] Central Standard Time
        /// </summary>
        MediusTimeZone_CST = -601,

        /// <summary>
        /// [GMT-5] Central Daylight Time
        /// </summary>
        MediusTimeZone_CDT = -500,
        MediusTimeZone_EST = -501,
        MediusTimeZone_EDT = -400,
        MediusTimeZone_AST = -401,
        MediusTimeZone_NST = -350,
        MediusTimeZone_ADT = -300,
        MediusTimeZone_NDT = -250,
        MediusTimeZone_WAT = -100,
        MediusTimeZone_GMT = 0,
        MediusTimeZone_UTC = 1,
        MediusTimeZone_WET = 2,
        MediusTimeZone_BST = 100,
        MediusTimeZone_IRISHST = 101,
        MediusTimeZone_WEST = 102,
        MediusTimeZone_CET = 103,
        MediusTimeZone_CEST = 200,
        MediusTimeZone_SWEDISHST = 201,
        MediusTimeZone_FST = 202,
        MediusTimeZone_CAT = 203,
        MediusTimeZone_SAST = 204,
        MediusTimeZone_EET = 205,
        MediusTimeZone_ISRAELST = 206,
        MediusTimeZone_EEST = 300,
        MediusTimeZone_BT = 301,
        MediusTimeZone_MSK = 302,
        MediusTimeZone_IRANST = 350,
        MediusTimeZone_MSD = 400,
        MediusTimeZone_INDIANST = 550,
        MediusTimeZone_JT = 750,
        MediusTimeZone_HKT = 800,
        MediusTimeZone_CCT = 801,
        MediusTimeZone_AWST = 802,
        MediusTimeZone_MT = 850,
        MediusTimeZone_KST = 900,
        MediusTimeZone_JST = 901,
        MediusTimeZone_ACST = 950,
        MediusTimeZone_AEST = 1000,
        MediusTimeZone_GST = 1001,
        MediusTimeZone_ACDT = 1050,
        MediusTimeZone_AEDT = 1100,
        MediusTimeZone_SST = 1101,
        MediusTimeZone_NZST = 1200,
        MediusTimeZone_IDLE = 1201,
        MediusTimeZone_NZDT = 1300,
    }

    #region MediusFindWorldType
    /// <summary>
    /// Search types for finding chat channels and/or game worlds.
    /// </summary>
    public enum MediusFindWorldType : int
    {
        /// <summary>
        /// Search for a game world with these parameters
        /// </summary>
        FindGameWorld,
        /// <summary>
        /// Search for a lobby chat world with these parameters
        /// </summary>
        FindLobbyWorld,
        /// <summary>
        /// Search for either a game or lobby world with these parameters
        /// </summary>
        FIndAllWorlds,
    }
    #endregion

    #region MediusBanReasonType
    /// <summary>
    /// 
    /// </summary>
    public enum MediusBanReasonType : int
    {
        /// <summary>
        /// Reserved.
        /// </summary>
        MediusInvalidBanReason,

        /// <summary>
        /// Ban for Chatting.
        /// </summary>
        MediusBanForChatting,

        /// <summary>
        /// Ban for Vulgarity.
        /// </summary>
        MediusBanForVulgarity,

        /// <summary>
        /// Ban for other title-definted reason.
        /// </summary>
        MediusBanForOtherReason,

        /// <summary>
        /// Placeholder to normalize the field size on different compilers
        /// </summary>
        ExtraMediusBanReasonType = 0xffffff
    }
    #endregion

    #region MediusVoteActionType
    /// <summary>
    /// Enumeration used to identify action of MediusVoteToBanPlayer Request (add/remove)
    /// </summary>
    public enum MediusVoteActionType : int
    {
        /// <summary>
        /// Invalid Vote Action
        /// </summary>
        MediusInvalidVoteAction,
        /// <summary>
        /// Add a vote to ban a player
        /// </summary>
        MediusAddVote,
        /// <summary>
        /// Remove a vote to ban a player
        /// </summary>
        MediusRemoveVote,
    }
    #endregion

    #region MediusWorldAttributesType
    [Flags]
    public enum MediusWorldAttributesType : int
    {
        /// <summary>
        /// Default game world attributes. Nothing special.
        /// </summary>
        GAME_WORLD_NONE = 0,
        /// <summary>
        /// Supports connected spectator worlds
        /// </summary>
        GAME_WORLD_ALLOW_REBROADCAST = 1,
        /// <summary>
        /// Indicates that this world is a spectator world
        /// </summary>
        GAME_WORLD_ALLOW_SPECTATOR = 2,
        /// <summary>
        /// Indicates this world was generated internally <Br></Br>
        /// Not by a client request
        /// </summary>
        GAME_WORLD_INTERNAL = 4,
        /// <summary>
        /// Unknown
        /// </summary>
        GAME_WORLD_RECORD = 8
    }
    #endregion

    #region MediusChatMessageType
    /// <summary>
    /// As of 2.10, MediusBuddyChatType is not supported yet. BroadcastAcrossEntireUniverse is usable, but highly discouraged.
    /// Special server side flags needed to enable this type of chat message due to the high load it represents
    /// </summary>
    public enum MediusChatMessageType : int
    {
        /// <summary>
        /// Sends to all in given chat channel
        /// </summary>
        Broadcast,
        /// <summary>
        /// Sends directly to another player
        /// </summary>
        Whisper,
        /// <summary>
        /// Sends to all given chat channels
        /// </summary>
        BroadcastAcrossEntireUniverse,
        /// <summary>
        /// Sends to all members of a clan
        /// </summary>
        Clan,
        /// <summary>
        /// Sends chat to all members in your buddy list
        /// </summary>
        Buddy
    }
    #endregion

    #region MediusWorldStatus
    /// <summary>
    /// Used to set a game world to a certain state. <br></br> 
    /// This affects whether the game can be joined or not and is displayed in GameInfo and GameList information.
    /// </summary>
    public enum MediusWorldStatus : int
    {
        /// <summary>
        /// Game world is not active
        /// </summary>
        WorldInactive,
        /// <summary>
        /// Players are staging in the game, but not yet playing.
        /// </summary>
        WorldStaging,
        /// <summary>
        /// Players are to join and play in the game world.
        /// </summary>
        WorldActive,
        /// <summary>
        /// Players are not allowed to join this game world.
        /// </summary>
        WorldClosed,
        /// <summary>
        /// Set by server while creation is in progress.
        /// </summary>
        WorldPendingCreation,
        /// <summary>
        /// Set by server for spectator worlds only after creation while <br></br> 
        /// connection to the game world is in progress.
        /// </summary>
        WorldPendingConnectToGame,
    }
    #endregion

    #region MediusGameListFilterField
    public enum MediusGameListFilterField : int
    {
        MEDIUS_FILTER_PLAYER_COUNT = 1,
        MEDIUS_FILTER_MIN_PLAYERS = 2,
        MEDIUS_FILTER_MAX_PLAYERS = 3,
        MEDIUS_FILTER_GAME_LEVEL = 4,
        MEDIUS_FILTER_PLAYER_SKILL_LEVEL = 5,
        MEDIUS_FILTER_RULES_SET = 6,
        MEDIUS_FILTER_GENERIC_FIELD_1 = 7,
        MEDIUS_FILTER_GENERIC_FIELD_2 = 8,
        MEDIUS_FILTER_GENERIC_FIELD_3 = 9,
        MEDIUS_FILTER_LOBBY_WORLDID = 0x0A,
        MEDIUS_FILTER_GENERIC_FIELD_4 = 0x0B,
        MEDIUS_FILTER_GENERIC_FIELD_5 = 0x0C,
        MEDIUS_FILTER_GENERIC_FIELD_6 = 0x0D,
        MEDIUS_FILTER_GENERIC_FIELD_7 = 0x0E,
        MEDIUS_FILTER_GENERIC_FIELD_8 = 0x0F,
        MEDIUS_FILTER_MATCH_MAKING_GAMES = 0x10,
        MEDIUS_FILTER_LOCATION_ID = 0x11,
        MEDIUS_FILTER_BUDDY_COUNT = 0x12,
        MEDIUS_FILTER_CLAN_MEMBER_COUNT = 0x13,
        MEDIUS_FILTER_IGNORED_PLAYER_COUNT = 0x14,
    }
    #endregion

    #region MediusGenerateRandomSelection
    /// <summary>
    /// Generate / Do not generate flags for random name generation during acccount logins.
    /// NOTES: Deprecated as of 2.10, will be removed in a future release of the API.
    /// </summary>
    public enum MediusGenerateRandomSelection : int
    {
        /// <summary>
        /// Do Not generate a random name on login.
        /// </summary>
        NotGenerate = 0,
        /// <summary>
        /// Generate a random name for login.
        /// </summary>
        GenerateRandom = 100,
        /// <summary>
        /// Placeholder to normalize the field size on different compilers
        /// </summary>
        ExtraMediusGenerateRandomSelection
    }
    #endregion

    #region MediusWorldSecurityLevelType
    /// <summary>
    /// Security level for a world. Determines if passwords are needed.
    /// </summary>
    [Flags]
    public enum MediusWorldSecurityLevelType : int
    {
        /// <summary>
        /// No security on world
        /// </summary>
        WORLD_SECURITY_NONE = 0,
        /// <summary>
        /// Password required to join as a player
        /// </summary>
        WORLD_SECURITY_PLAYER_PASSWORD = (1 << 0),
        /// <summary>
        /// World is closed to new players
        /// </summary>
        WORLD_SECURITY_CLOSED = (1 << 1),
        /// <summary>
        /// Password is required to join as a spectator
        /// </summary>
        WORLD_SECURITY_SPECTATOR_PASSWORD = (1 << 2),
        /// <summary>
        /// Placeholder to normalize the field size on different compilers
        /// </summary>
        WORLD_SECURITY_EXTRA = 0xFFFFFF
    }
    #endregion

    #region MediusLobbyFilterType
    /// <summary>
    /// Whether the filter mask(s) submitted to a MediusSetLobbyWorldFilter() <br></br>
    /// call will be Lobby&Filter=Filter or Lobby&Filter=Lobby.
    /// </summary>
    public enum MediusLobbyFilterType : int
    {
        /// <summary>
        /// Lobby filtering rules. Lobby&Filter = Lobby.
        /// </summary>
        MediusLobbyFilterEqualsLobby,
        /// <summary>
        /// Lobby filtering rules. Lobby&Filter = Filter.
        /// </summary>
        MediusLobbyFilterEqualsFilter,
        /// <summary>
        ///  Lobby filtering rules. Lobby&Filter = FILTER_EQUALS.
        /// </summary>
        FILTER_EQUALS
    }
    #endregion

    #region MediusLobbyFilterMaskLevelType
    /// <summary>
    /// Allows the user to set the number of filtermasks to use for the lobby world filter (GF1,2,3,4).
    /// </summary>
    [Flags]
    public enum MediusLobbyFilterMaskLevelType : int
    {
        /// <summary>
        /// not using filter mask
        /// </summary>
        MediusLobbyFilterMaskLevel0 = 0,
        /// <summary>
        /// use only filter mask 1
        /// </summary>
        MediusLobbyFilterMaskLevel1 = 1 << 0,
        /// <summary>
        /// use only filter mask 2
        /// </summary>
        MediusLobbyFilterMaskLevel2 = 1 << 1,
        /// <summary>
        /// use only filter mask 3
        /// </summary>
        MediusLobbyFilterMaskLevel3 = 1 << 2,
        /// <summary>
        /// use only filter mask 4
        /// </summary>
        MediusLobbyFilterMaskLevel4 = (1 << 3),
        /// <summary>
        /// use 1 and 2
        /// </summary>
        MediusLobbyFilterMaskLevel12 = (1 << 4),
        /// <summary>
        /// use 1, 2, and 3
        /// </summary>
        MediusLobbyFilterMaskLevel123 = (1 << 5),
        /// <summary>
        /// use 1, 2, 3, and 4
        /// </summary>
        MediusLobbyFilterMaskLevel1234 = (1 << 6),
        /// <summary>
        ///use 2 and 3
        /// </summary>
        MediusLobbyFilterMaskLevel23 = (1 << 7),
        /// <summary>
        ///  use 2, 3, and 4
        /// </summary>
        MediusLobbyFilterMaskLevel234 = (1 << 8),
        /// <summary>
        /// use 3 and 4
        /// </summary>
        MediusLobbyFilterMaskLevel34 = (1 << 9),
    }
    #endregion

    #region MediusComparisonOperator
    /// <summary>
    /// Specifies the operator used in filtering operations
    /// </summary>
    public enum MediusComparisonOperator : int
    {
        /// <summary>
        /// Less than comparison operator
        /// </summary>
        LESS_THAN,
        /// <summary>
        /// Less than or equal to comparison operator
        /// </summary>
        LESS_THAN_OR_EQUAL_TO,
        /// <summary>
        /// Equal to comparison operator
        /// </summary>
        EQUAL_TO,
        /// <summary>
        /// Greater than or equal to comparison operator
        /// </summary>
        GREATER_THAN_OR_EQUAL_TO,
        /// <summary>
        /// Great than comparison operator
        /// </summary>
        GREATER_THAN,
        /// <summary>
        /// Not equals comparison operator
        /// </summary>
        NOT_EQUALS,
    }
    #endregion

    public enum MediusFileSortBy : int
    {
        MFSortByNothing,
        MFSortByName,
        MFSortByFileSize,
        MFSortByTimeStamp,
        MFSortByGroupID,
        MFSortByPopularity,
        MFSortByMetaValue,
        MFSortByMetaString
    }

    public enum MediusQueueState : int
    {
        MQ_UNKNOWN,
        MQ_UPDATE,
        MQ_LOGINCOMPLETE
    }

    #region NetConnectionType
    public enum NetConnectionType : int
    {
        /// <summary>
        /// This value is used to specify that no information is present
        /// </summary>
        NetConnectionNone = 0,

        /// <summary>
        /// This specifies a connection to a Server via TCP
        /// </summary>
        NetConnectionTypeClientServerTCP = 1,

        /// <summary>
        /// This specifies a connection to another peer via UDP
        /// </summary>
        NetConnectionTypePeerToPeerUDP = 2,

        /// <summary>
        /// This specifies a connection to a Server via TCP and UDP.  The UDP connection is normal UDP: 
        /// there is no reliability or in-order guarantee.
        /// </summary>
        NetConnectionTypeClientServerTCPAuxUDP = 3,

        /// <summary>
        /// This specifies a connection to a Server via TCP.  This is reserved for SCE-RT "Spectator" functionality.
        /// </summary>
        NetConnectionTypeClientListenerTCP = 4,

        /// <summary>
        /// This specifies a connection to a Server via UDP
        /// </summary>
        NetConnectionTypeClientServerUDP = 5,

        /// <summary>
        /// This specifies a connection to a Server via TCP and UDP.  This is reserved for SCE-RT "Spectator" functionality.
        /// </summary>
        NetConnectionTypeClientListenerTCPAuxUDP = 6,

        /// <summary>
        ///  This specifies a connection to a Server via UDP.  This is reserved for SCE-RT "Spectator" functionality.
        /// </summary>
        NetConnectionTypeClientListenerUDP = 7,
    }
    #endregion

    public enum SERVER_FORCE_DISCONNECT_REASON : byte
    {
        SERVER_FORCED_DISCONNECT_NONE = 0,
        SERVER_FORCED_DISCONNECT_ERROR = 1,
        SERVER_FORCED_DISCONNECT_SHUTDOWN = 2,
        SERVER_FORCED_DISCONNECT_END_SESSION = 3,
        SERVER_FORCED_DISCONNECT_END_GAME = 4,
        SERVER_FORCED_DISCONNECT_TIME0UT = 5,
        SERVER_FORCED_DISCONNECT_BAD_PERF = 6,
        SERVER_FORCED_DISCONNECT_BANNED = 7
    }

    [Flags]
    public enum RT_RECV_FLAG : byte
    {
        NONE = 0,
        RECV_BROADCAST = 1,
        RECV_LIST = 2,
        RECV_SINGLE = 4,
        RECV_NOTIFICATION = 8
    }

    public enum NetConnectFailureReason : uint
    {

    }

    /// <summary>
    /// The values in this enumeration are used for determining the<br></br>
    /// type of connection being made.
    /// </summary>
    public enum NetAddressType : uint
    {
        /// <summary>
        /// This value is used to specify "Not in use"
        /// </summary>
        NetAddressNone,
        /// <summary>
        /// ASCII string representation of a client's public IPv4 address.
        /// </summary>
        NetAddressTypeExternal,
        /// <summary>
        /// ASCII string representation of a client's private IPv4 address.
        /// </summary>
        NetAddressTypeInternal,
        /// <summary>
        /// ASCII string representiation of a NAT resolution server's IPv4 address.
        /// </summary>
        NetAddressTypeNATService,
        /// <summary>
        ///4-byte binary representation of a client's public IPv4 address.
        /// </summary>
        NetAddressTypeBinaryExternal,
        /// <summary>
        /// 4-byte binary representation of a client's private IPv4 address.
        /// </summary>
        NetAddressTypeBinaryInternal,
        /// <summary>
        /// 4-byte binary representation of a client's public IPv4 address.<br></br>
        /// The Port parameter contains a 2-byte virtual port in 2 high bytes and<br></br>
        /// the actual network port in the 2 low bytes.
        /// </summary>
        NetAddressTypeBinaryExternalVport,
        /// <summary>
        /// 4-byte binary representation of a client's public IPv4 address.<br></br>
        /// The Port parameter contains a 2-byte virtual port in 2 high bytes and<br></br>
        /// the actual network port in the 2 low bytes.
        /// </summary>
        NetAddressTypeBinaryInternalVport,
        /// <summary>
        /// Contains two 4-byte binary representations of NAT resolution servers<br></br>
        /// IPv4 addresses stored back to back.
        /// </summary>
        NetAddressTypeBinaryNATServices,
        /// <summary>
        /// 
        /// </summary>
        NetAddressTypeSignalAddress,
        /// <summary>
        /// 
        /// </summary>
        NetAddressTypeSignalBlob
    }

    #region NetMessageClass
    /// <summary>
    /// Message classes allow for orthogonal (non-interacting) registration of messages.
    /// </summary>
    public enum NetMessageClass : byte
    {
        /// <summary>
        /// Identifies messages used internally by the DME.
        /// </summary>
        MessageClassDME,
        /// <summary>
        /// Identifies messages used by the Medius Lobby.
        /// </summary>
        MessageClassLobby,
        /// <summary>
        /// Identifies messages used by your game.
        /// </summary>
        MessageClassApplication,
        /// <summary>
        /// Identifies messages used by the Medius Game Communications Library (MGCL).
        /// </summary>
        MessageClassLobbyReport,
        /// <summary>
        /// Identifies additional external messages used by the Medius Lobby.
        /// </summary>
        MessageClassLobbyExt,
        /// <summary>
        /// Identifies messages used during authentication.
        /// (Deprecated)
        /// </summary>
        MessageClassLobbyAuthentication,
        /// <summary>
        /// Zipper Interactive MAG/Socom 4 only!
        /// </summary>
        MessageClassDMELocalPlugin,
        /// <summary>
        /// NOT OFFICIAL
        /// </summary>
        MessageClassGHS,
        /// <summary>
        /// Used as an array allocation size. Must always be the <i>last</i> valid value before ExtraNetMessageClass, not after.
        /// </summary>
        MaxMessageClasses,
        /// <summary>
        /// Ensures that all values are stored as 32-bit integers on all compilers.
        /// </summary>
        ExtraNetMessageClass = 0xff
    }
    #endregion

    public enum NetClientStatus : uint
    {
        /// <summary>
        /// No ClientStatus is available.
        /// </summary>
        ClientStatusNone,
        /// <summary>
        /// Client is not connected.
        /// </summary>
        ClientStatusNotConnected,
        /// <summary>
        /// Client is connected, but has not called NetJoin().
        /// </summary>
        ClientStatusConnected,
        /// <summary>
        /// Client is in the process of joining, and is now receiving its first batch of object and field updates.
        /// </summary>
        ClientStatusJoining,
        /// <summary>
        /// The client is now fully synchronized with the game, and has received all initial object creation callbacks, etc.
        /// </summary>
        ClientStatusJoined,
        /// <summary>
        /// The client is fully joined and is <i>also</i> the Session Master.
        /// </summary>
        ClientStatusJoinedSessionMaster,
        /// <summary>
        /// Ensures that all values are stored as 32-bit integers on all compilers.
        /// </summary>
        ExtraNetClientStatus = 0xffffff
    }

    /// <summary>
    /// Introduced in 1.24
    /// </summary>
    public enum NetStreamMediaAudioType : uint
    {
        /// <summary>
        /// RAW Unencoded audio data
        /// </summary>
        NetStreamMediaAudioTypeRAW = 0,
        /// <summary>
        /// Custom application audio data
        /// </summary>
        NetStreamMediaAudioTypeCUSTOM = 1,
        /// <summary>
        /// DME encoded GSM audio data
        /// </summary>
        NetStreamMediaAudioTypeGSM = 2,
        /// <summary>
        /// DME encoded LPC audio data
        /// </summary>
        NetStreamMediaAudioTypeLPC = 3,
        /// <summary>
        /// DME encoded LPC10 audio data
        /// </summary>
        NetStreamMediaAudioTypeLPC10 = 4,
        /// <summary>
        /// Ensures that all values are stored as 32-bit integers on all compilers.
        /// </summary>
        ExtraNetStreamMediaAudioType = 0xffffff
    }

    #region MediusDmeMessageIds
    public enum MediusDmeMessageIds : byte
    {
        ServerVersion = 0x00,
        Ping = 0x01,
        PacketFragment = 0x02,
        FieldUpdate = 0x03,
        ObjectUpdate = 0x04,
        DataStreamEnd = 0x05,
        AcceptClient = 0x9,
        DeleteNetObject = 0xA,
        RequestOwnership = 0xB,
        GrantOwnership = 0xC,
        ReleaseOwnership = 0xD,
        DataStreamUpdateP = 0xE,
        ClientUpdate = 0xF,
        ClientConnect = 0x10,
        ClientLeaves = 0x12,
        RequestServers = 0x13,
        ServerResponse = 0x14,
        ArbitrateJoin = 0x15,
        UpdateClientStatus = 0x16,
        SMMigrateOrder = 0x17,
        GameFind = 0x19,
        GameFindResults = 0x1A,
        StreamMediaBufferRequest = 0x1E,
        StreamMediaBufferGranted = 0x1F,
        StreamMediaBufferRelease = 0x20,
        LANTextMessage = 0x21,
        LANRawMessage = 0x22,
        StreamMediaAudioData = 0x25,
        StreamMediaVideoData = 0x26,
        StreamMediaVideoCustomData = 0x27,
        StreamMediaChannelJoin = 0x28,
        StreamMediaChannelQuit = 0x29,
        StreamMediaChannelJoinResp = 0x2B,
        StreamMediaClientStatus = 0x2D,
        StreamMediaClientOrdinal = 0x2E,
        GameMetaData = 0x2F,
        ICESignal = 0x30,
    }
    #endregion

    #region MediusMGCLMessageIds
    public enum MediusMGCLMessageIds : byte
    {
        ServerReport = 0x00,
        ServerAuthenticationRequest = 0x01,
        ServerAuthenticationResponse = 0x02,
        ServerSessionBeginRequest = 0x03,
        ServerSessionBeginResponse = 0x04,
        ServerSessionEndRequest = 0x05,
        ServerSessionEndResponse = 0x06,
        ServerCreateGameRequest = 0x07,
        ServerCreateGameResponse = 0x08,
        ServerJoinGameRequest = 0x09,
        ServerJoinGameResponse = 0x0A,
        ServerEndGameRequest = 0x0B,
        ServerEndGameResponse = 0x0C,
        ServerWorldStatusRequest = 0x0D,
        ServerWorldStatusResponse = 0x0E,
        ServerCreateGameOnSelfRequest0 = 0x0F,
        ServerCreateGameOnMeResponse = 0x10,
        ServerEndGameOnMeRequest = 0x11,
        ServerEndGameOnMeResponse = 0x12,
        ServerMoveGameWorldOnMeRequest = 0x14,
        ServerMoveGameWorldOnMeResponse = 0x15,
        ServerSetAttributesRequest = 0x16,
        ServerSetAttributesResponse = 0x17,
        ServerCreateGameWithAttributesRequest = 0x18,
        ServerCreateGameWithAttributesResponse = 0x19,
        ServerConnectGamesRequest = 0x1A,
        ServerConnectGamesResponse = 0x1B,
        ServerConnectNotification = 0x1C,
        ServerCreateGameOnSelfRequest = 0x1D,
        ServerDisconnectPlayerRequest = 0x1E,
        ServerCreateGameOnMeRequest = 0x1F,
        ServerWorldReportOnMe = 0x20,
        ServerSessionBeginRequest1 = 0x21,
        ServerSessionBeginResponse1 = 0x22,
        ServerCreateGameWithAttributesRequest2 = 0x23, //TEST
        ServerSessionBeginRequest2 = 0x24
    }
    #endregion

    #region MediusLobbyMessageIds
    public enum MediusLobbyMessageIds : byte
    {
        WorldReport0 = 0x00,
        PlayerReport = 0x01,
        EndGameReport = 0x02,
        SessionBegin = 0x03,
        SessionBeginResponse = 0x04,
        SessionEnd = 0x05,
        SessionEndResponse = 0x06,
        AccountLogin = 0x07,
        AccountLoginResponse = 0x08,
        AccountRegistration = 0x09,
        AccountRegistrationResponse = 0x0A,
        AccountGetProfile = 0x0B,
        AccountGetProfileResponse = 0x0C,
        AccountUpdateProfile = 0x0D,
        AccountUpdateProfileResponse = 0x0E,
        AccountUpdatePassword = 0x0F,
        AccountUpdatePasswordStatusResponse = 0x10,
        AccountUpdateStats = 0x11,
        AccountUpdateStatsResponse = 0x12,
        AccountDelete = 0x13,
        AccountDeleteResponse = 0x14,
        AccountLogout = 0x15,
        AccountLogoutResponse = 0x16,
        AccountGetID = 0x17,
        AccountGetIDResponse = 0x18,
        AnonymousLogin = 0x19,
        AnonymousLoginResponse = 0x1A,
        GetMyIP = 0x1B,
        GetMyIPResponse = 0x1C,
        CreateGameRequest0 = 0x1D,
        CreateGameResponse = 0x1E,
        CreateGameOnSelf = 0x1F,
        CreateGameOnSelfResponse = 0x20,
        CreateChannelRequest0 = 0x21,
        CreateChannelResponse = 0x22,
        JoinGameRequest0 = 0x23,
        JoinGameResponse = 0x24,
        JoinChannel = 0x25,
        JoinChannelResponse = 0x26,
        JoinChannelFwd = 0x27,
        JoinChannelFwdResponse = 0x28,
        GameList = 0x29,
        GameListResponse = 0x2A,
        ChannelList = 0x2B,
        ChannelListResponse = 0x2C,
        LobbyWorldPlayerList = 0x2D,
        LobbyWorldPlayerListResponse = 0x2E,
        GameWorldPlayerList = 0x2F,
        GameWorldPlayerListResponse = 0x30,
        PlayerInfo = 0x31,
        PlayerInfoResponse = 0x32,
        GameInfo0 = 0x33,
        GameInfoResponse0 = 0x34,
        ChannelInfo = 0x35,
        ChannelInfoResponse = 0x36,
        FindWorldByName = 0x37,
        FindWorldByNameResponse = 0x38,
        FindPlayer = 0x39,
        FindPlayerResponse = 0x3A,
        ChatMessage = 0x3B,
        ChatFwdMessage = 0x3C,
        GetBuddyList = 0x3D,
        GetBuddyListResponse = 0x3E,
        AddToBuddyList = 0x3F,
        AddToBuddyListResponse = 0x40,
        RemoveFromBuddyList = 0x41,
        RemoveFromBuddyListResponse = 0x42,
        AddToBuddyListConfirmationRequest0 = 0x43,
        AddToBuddyListConfirmationResponse = 0x44,
        AddToBuddyListFwdConfirmationRequest0 = 0x45,
        AddToBuddyListFwdConfirmationResponse0 = 0x46,
        Policy = 0x47,
        PolicyResponse = 0x48,
        UpdateUserState = 0x49,
        ErrorMessage = 0x4A,
        GetAnnouncements = 0x4B,
        GetAllAnnouncements = 0x4C,
        GetAnnouncementsResponse = 0x4D,
        SetGameListFilter0 = 0x4E,
        SetGameListFilterResponse0 = 0x4F,
        ClearGameListFilter0 = 0x50,
        ClearGameListFilterResponse = 0x51,
        GetGameListFilter = 0x52,
        GetGameListFilterResponse0 = 0x53,
        CreateClan = 0x54,
        CreateClanResponse = 0x55,
        DisbandClan = 0x56,
        DisbandClanResponse = 0x57,
        GetClanByID = 0x58,
        GetClanByIDResponse = 0x59,
        GetClanByName = 0x5A,
        GetClanByNameResponse = 0x5B,
        TransferClanLeadership = 0x5C,
        TransferClanLeadershipResponse = 0x5D,
        AddPlayerToClan = 0x5E,
        AddPlayerToClanResponse = 0x5F,
        RemovePlayerFromClan = 0x60,
        RemovePlayerFromClanResponse = 0x61,
        InvitePlayerToClan = 0x62,
        InvitePlayerToClanResponse = 0x63,
        CheckMyClanInvitations = 0x64,
        CheckMyClanInvitationsResponse = 0x65,
        RespondToClanInvitation = 0x66,
        RespondToClanInvitationResponse = 0x67,
        RevokeClanInvitation = 0x68,
        RevokeClanInvitationResponse = 0x69,
        RequestClanTeamChallenge = 0x6A,
        RequestClanTeamChallengeResponse = 0x6B,
        GetMyClanMessages = 0x6C,
        GetMyClanMessagesResponse = 0x6D,
        SendClanMessage = 0x6E,
        SendClanMessageResponse = 0x6F,
        ModifyClanMessage = 0x70,
        ModifyClanMessageResponse = 0x71,
        DeleteClanMessage = 0x72,
        DeleteClanMessageResponse = 0x73,
        RespondToClanTeamChallenge = 0x74,
        RespondToClanTeamChallengeResponse = 0x75,
        RevokeClanTeamChallenge = 0x76,
        RevokeClanTeamChallengeResponse = 0x77,
        GetClanTeamChallengeHistory = 0x78,
        GetClanTeamChallengeHistoryResponse = 0x79,
        GetClanInvitationsSent = 0x7A,
        GetClanInvitationsSentResponse = 0x7B,
        GetMyClans = 0x7C,
        GetMyClansResponse = 0x7D,
        GetAllClanMessages = 0x7E,
        GetAllClanMessagesResponse = 0x7F,
        ConfirmClanTeamChallenge = 0x80,
        ConfirmClanTeamChallengeResponse = 0x81,
        GetClanTeamChallenges = 0x82,
        GetClanTeamChallengesResponse = 0x83,
        UpdateClanStats = 0x84,
        UpdateClanStatsResponse = 0x85,
        VersionServer = 0x86,
        VersionServerResponse = 0x87,
        GetWorldSecurityLevel = 0x88,
        GetWorldSecurityLevelResponse = 0x89,
        BanPlayer = 0x8A,
        BanPlayerResponse = 0x8B,
        GetLocations = 0x8C,
        GetLocationsResponse = 0x8D,
        PickLocation = 0x8E,
        PickLocationResponse = 0x8F,
        GetClanMemberList = 0x90,
        GetClanMemberListResponse = 0x91,
        LadderPosition = 0x92,
        LadderPositionResponse = 0x93,
        LadderList = 0x94,
        LadderListResponse = 0x95,
        ChatToggle = 0x96,
        ChatToggleResponse = 0x97,
        TextFilter = 0x98,
        TextFilterResponse = 0x99,
        ReassignGameMediusWorldID = 0x9A,
        GetTotalGames = 0x9B,
        GetTotalGamesResponse = 0x9C,
        GetTotalChannels = 0x9D,
        GetTotalChannelsResponse = 0x9E,
        GetLobbyPlayerNames = 0x9F,
        GetLobbyPlayerNamesResponse = 0xA0,
        GetTotalUsers = 0xA1,
        GetTotalUsersResponse = 0xA2,
        SetLocalizationParams = 0xA3,
        StatusResponse0 = 0xA4,
        FileCreate = 0xA5,
        FileCreateResponse = 0xA6,
        FileUpload = 0xA7,
        FileUploadResponse = 0xA8,
        FileUploadServerReq = 0xA9,
        FileClose = 0xAA,
        FileCloseResponse = 0xAB,
        FileDownload = 0xAC,
        FileDownloadResponse = 0xAD,
        FileDownloadStream = 0xAE,
        FileDownloadStreamResponse = 0xAF,
        FileDelete = 0xB0,
        FileDeleteResponse = 0xB1,
        FileListFiles = 0xB2,
        FileListFilesResponse = 0xB3,
        FileUpdateAttributes = 0xB4,
        FileUpdateAttributesResponse = 0xB5,
        FileGetAttributes = 0xB6,
        FileGetAttributesResponse = 0xB7,
        FileUpdateMetaData = 0xB8,
        FileUpdateMetaDataResponse = 0xB9,
        FileGetMetaData = 0xBA,
        FileGetMetaDataResponse = 0xBB,
        FileSearchByMetaData = 0xBC,
        FileSearchByMetaDataResponse = 0xBD,
        FileCancelOperation = 0xBE,
        FileCancelOperationResponse = 0xBF,
        GetIgnoreList = 0xC0,
        GetIgnoreListResponse = 0xC1,
        AddToIgnoreList = 0xC2,
        AddToIgnoreListResponse = 0xC3,
        RemoveFromIgnoreList = 0xC4,
        RemoveFromIgnoreListResponse = 0xC5,
        SetMessageAsRead = 0xC6,
        SetMessageAsReadResponse = 0xC7,
        GetUniverseInformation = 0xC8,
        UniverseNewsResponse = 0xC9,
        UniverseStatusListResponse = 0xCA,
        MachineSignaturePost = 0xCB,
        LadderPositionFast = 0xCC,
        LadderPositionFastResponse = 0xCD,
        UpdateLadderStats = 0xCE,
        UpdateLadderStatsResponse = 0xCF,
        GetLadderStats = 0xD0,
        GetLadderStatsResponse = 0xD1,
        ClanLadderList = 0xD2,
        ClanLadderListResponse = 0xD3,
        ClanLadderPosition = 0xD4,
        ClanLadderPositionResponse = 0xD5,
        GetBuddyList_ExtraInfo = 0xD6,
        GetBuddyList_ExtraInfoResponse = 0xD7,
        GetTotalRankings = 0xD8,
        GetTotalRankingsResponse = 0xD9,
        GetClanMemberList_ExtraInfo = 0xDA,
        GetClanMemberList_ExtraInfoResponse = 0xDB,
        GetLobbyPlayerNames_ExtraInfo = 0xDC,
        GetLobbyPlayerNames_ExtraInfoResponse = 0xDD,
        BillingLogin = 0xDE,
        BillingLoginResponse = 0xDF,
        BillingListRequest = 0xE0,
        BillingListResponse = 0xE1,
        BillingDetailRequest = 0xE2,
        BillingDetailResponse = 0xE3,
        PurchaseProductRequest = 0xE4,
        PurchaseProductResponse = 0xE5,
        BillingInfo = 0xE6,
        BillingInfoResponse = 0xE7,
        BillingTunnelRequest = 0xE8,
        BillingTunnelResponse = 0xE9,
        GameList_ExtraInfo0 = 0xEA,
        GameList_ExtraInfoResponse0 = 0xEB,
        ChannelList_ExtraInfo0 = 0xEC,
        ChannelList_ExtraInfoResponse = 0xED,
        InvitePlayerToClan_ByName = 0xEE,
        LadderList_ExtraInfo0 = 0xEF,
        LadderList_ExtraInfoResponse = 0xF0,
        LadderPosition_ExtraInfo = 0xF1,
        LadderPosition_ExtraInfoResponse = 0xF2,
        JoinGame = 0xF3,
        CreateGame1 = 0xF4,
        UtilAddLobbyWorld = 0xF5,
        UtilAddLobbyWorldResponse = 0xF6,
        UtilAddGameWorldRequest = 0xF7,
        UtilAddGameWorldResponse = 0xF8,
        UtilUpdateLobbyWorldRequest = 0xF9,
        UtilUpdateLobbyWorldResponse = 0xFA,
        UtilUpdateGameWorldRequest = 0xFB,
        UtilUpdateGameWorldStatusResponse = 0xFC,
    }
    #endregion

    #region MediusLobbyExtMessageIds
    public enum MediusLobbyExtMessageIds : byte
    {
        CreateChannel1 = 0x00,
        UtilGetServerVersion = 0x01,
        UtilGetServerVersionResponse = 0x02,
        GetUniverse_ExtraInfo = 0x03,
        UniverseStatusList_ExtraInfoResponse = 0x04,
        AddToBuddyListConfirmation = 0x05,
        AddToBuddyListFwdConfirmation = 0x06,
        AddToBuddyListFwdConfirmationResponse = 0x07,
        GetBuddyInvitations = 0x08,
        GetBuddyInvitationsResponse = 0x09,
        DnasSignaturePost = 0x0A,
        UpdateLadderStatsWide = 0x0B,
        UpdateLadderStatsWideResponse = 0x0C,
        GetLadderStatsWide = 0x0D,
        GetLadderStatsWideResponse = 0x0E,
        LadderList_ExtraInfo = 0x0F,
        UtilEventMsgHandler = 0x10,
        UniverseVariableInformationResponse = 0x11,
        SetLobbyWorldFilter = 0x12,
        SetLobbyWorldFilterResponse = 0x13,
        CreateChannel = 0x14,
        ChannelList_ExtraInfo1 = 0x15,
        BinaryMessage = 0x16,
        BinaryFwdMessage = 0x17,
        PostDebugInfo = 0x18,
        PostDebugInfoResponse = 0x19,
        UpdateClanLadderStatsWide_Delta = 0x1A,
        UpdateClanLadderStatsWide_DeltaResponse = 0x1B,
        GetLadderStatsWide_wIDArray = 0x1C,
        GetLadderStatsWide_wIDArray_Response = 0x1D,
        UniverseSvoURLResponse = 0x1E,
        ChannelList_ExtraInfo = 0x1F,
        DListSubscription = 0x20,
        DListRequest = 0x21,
        DListResponse = 0x22,
        GenericChatMessage = 0x23,
        GenericChatFwdMessage = 0x24,
        GenericChatSetFilterRequest = 0x25,
        GenericChatSetFilterResponse = 0x26,
        ExtendedSessionBeginRequest = 0x27,
        TokenRequest = 0x28,
        GetServerTimeRequest = 0x2A,
        GetServerTimeResponse = 0x2B,
        VoteToBanPlayer = 0x2C,
        SetAutoChatHistoryRequest = 0x2D,
        SetAutoChatHistoryResponse = 0x2E,
        CreateGame = 0x2F,
        WorldReport = 0x30,
        ClearGameListFilter = 0x31,
        GetGameListFilterResponse = 0x32,
        SetGameListFilter = 0x33,
        SetGameListFilterResponse = 0x34,
        GameInfo = 0x35,
        GameInfoResponse = 0x36,
        GameList_ExtraInfo = 0x37,
        GameList_ExtraInfoResponse = 0x38,
        AccountUpdateStats_OpenAccess = 0x39,
        AccountUpdateStats_OpenAccessResponse = 0x3A,
        AddPlayerToClan_ByClanOfficer = 0x3B,
        AddPlayerToClan_ByClanOfficerResponse = 0x3C,
        CrossChatMessage = 0x3D,
        CroxxChatFwdMessage = 0x3E,
        QueueUpdateMessage = 0x3F,
        QueueCompleteMessage = 0x40,
        GetAccessLevelInfoRequest = 0x41,
        GetAccessLevelInfoResponse = 0x42,
        AccessLevelInfoUnsolicitedResponse = 0x43,
        FileListExtRequest = 0x44,
        FileListExtResponse = 0x45,
        FileUpdateAuxMetaDataRequest = 0x46,
        UtilGetTotalGamesFilteredRequest = 0x48,
        UtilGetTotalGamesFilteredResponse = 0x49,
        AccountLoginRequest1 = 0x4A,
        AccountLoginResponse1 = 0x4B,
        AccountLoginRequest2 = 0x4C,
        AccountLoginResponse2 = 0x4D,
        AddAliasRequest = 0x4E,
        AddAliasResponse = 0x4F,
        DeleteAliasRequest = 0x50,
        DeleteAliasResponse = 0x51,
        GetMyAliasesRequest = 0x52,
        GetMyAliasesResponse = 0x53,
        BuddySetListRequest = 0x54,
        BuddySetListResponse = 0x55,
        IgnoreSetListRequest = 0x56,
        IgnoreSetListResponse = 0x57,
        TicketLogin = 0x58, //Account Update Stats (Open Access) DEPRECATED past 2.10
        TicketLoginResponse = 0x59,
        SetLocalizationParamsRequest2 = 0x5A,
        BinaryMessage1 = 0x5B,
        BinaryFwdMessage1 = 0x5C,
        MatchGetSupersetListRequest = 0x5D,
        MatchGetSupersetListResponse = 0x5E,
        MatchPartyRequest = 0x5F,
        MatchPartyResponse = 0x60,
        GetBuddyInvitationsSentResponse = 0x61,
        GetBuddyInvitationsSentResponse1 = 0x62,
        AddToBuddyListConfirmationRequest = 0x63,
        AddToBuddyListConfirmationResponse = 0x64,
        PartyCreateRequest = 0x65,
        PartyCreateResponse = 0x66,
        PartyJoinRequest = 0x67,
        PartyJoinResponse = 0x68,
        GameListRequest = 0x69,
        PartyListResponse = 0x6A,
        PlayerIgnoresMeRequest = 0x6B,
        PlayerIgnoresMeResponse = 0x6C,
        NpIdPostRequest = 0x6D,
        StatusResponse_0 = 0x6E,
        MediusNpIdsGetByAccountNamesRequest = 0x6F,
        MediusNpIdsGetByAccountNamesResponse = 0x70,
        JoinLeastPopulatedChannelRequest = 0x71,
        JoinLeastPopulatedChannelResponse = 0x72,
        GenericChatMessage1 = 0x73,
        GenericChatFwdMessage1 = 0x74,
        MediusTextFilter1 = 0x75,
        MediusTextFilterResponse1 = 0x76,
        GetMyClanMessagesResponse = 0x77,
        MatchPartyRequest2 = 0x78,
        MatchSetGameStateRequest = 0x79, //MatchCloseLateJoinRequest
        MatchSetGameStateResponse = 0x7A, // MatchCloseLateJoinResponse
        SetLocalizationParamsRequest1 = 0x7B,
        ClanRenameRequest = 0x7C,
        ClanRenameStatusResponse = 0x7D,
        SetGameListSortRequest = 0x7E,
        SetGameListSortResponse = 0x7F,
        GetGameListSortRequest = 0x80,
        GetGameListSortResponse = 0x81,
        ClearGameListSortRequest = 0x82,
        ClearGameListSortStatusResponse = 0x83,
        SetGameListSortPriorityRequest = 0x84,
        SetGameListSortPriorityStatusResponse = 0x85,
        SetLobbyWorldFilter1 = 0x86,
        PartyJoinByIndexRequest = 0x87, // SetLobbyWorldFilterResponse1 = 0x87, //PartyJoinByIndexRequest
        MatchFindGameRequest = 0x88,
        MatchFindGameStatusResponse = 0x89,
        AssignedGameToJoinMessage = 0x8A,
        SessionBegin1 = 0x8B,
        UpdateChannelRequest = 0x8C,
        UpdateChannelResponse = 0x8D,
        PartyJoinByIndexResponse = 0x8E,
        PartyPlayerReport = 0x8F,
        GroupJoinChannelRequest = 0x90,
        AssignedLobbyToJoinMessage = 0x91,
        MatchCancelRequest = 0x92,
        MatchCancelStatusResponse = 0x93,
        MatchCreateGameRequest = 0x94,
        MatchCreateGameResponse = 0x95,
        PartyList_ExtraInfoRequest = 0x96,
        PartyList_ExtraInfoResponse = 0x97,
        UnkR2Request = 0xA1,
        UnkR2Response = 0xA2,
        SetLocalizationParamsStatusResponse1 = 0xA4,
    }
    #endregion

    #region DME 

    public enum DME_SERVER_WORLD_TYPE : int
    {
        DME_SERVER_GENERIC_WORLD,
        DME_SERVER_VIRTUAL_PRIVATE_WORLD,
        DME_SERVER_REBROADCASTER_WORLD,
        DME_SERVER_SPECTATOR_WORLD,
        DME_SERVER_WORLD_TYPE_MAX
    }

    public enum DME_SERVER_RESULT : uint
    {
        DME_SERVER_OK = 0,
        DME_SERVER_INVALID_PARAM = 1,
        DME_SERVER_NOT_IMPLEMENTED = 2,
        DME_SERVER_NOT_INITIALIZED = 3,
        DME_SERVER_UNKNOWN_RESULT = 4,
        DME_SERVER_MEM_ALLOC = 5,
        DME_SERVER_SOCKET_LIMIT = 6,
        DME_SERVER_UNKNOWN_CONN_ERROR = 7,
        DME_SERVER_CONN_MSG_ERROR = 8,
        DME_SERVER_WORLD_FULL = 9,
        DME_SERVER_STACK_LOAD_ERROR = 0x0A,
        DME_SERVER_SOCKET_CREATE_ERROR = 0x0B,
        DME_SERVER_SOCKET_OPT_ERROR = 0x0C,
        DME_SERVER_SOCKET_BIND_ERROR = 0x0D,
        DME_SERVER_SOCKET_POLL_ERROR = 0x0E,
        DME_SERVER_SOCKET_LISTEN_ERROR = 0x0F,
        DME_SERVER_SOCKET_READ_ERROR = 10,
        DME_SERVER_SOCKET_WRITE_ERROR = 11,
        DME_SERVER_INVALID_WORLD_INDEX = 12,
        DME_SERVER_WOULD_BLOCK = 13,
        DME_SERVER_TCP_GET_WORLD_INDEX = 14,
        DME_SERVER_READ_ERROR = 15,
        DME_SERVER_SOCKET_CLOSED = 16,
        DME_SERVER_MSG_TOO_BIG = 17,
        DME_SERVER_UNKNOWN_MSG_TYPE = 18,
        DME_SERVER_PARTIAL_WRITE = 19,
        DME_SERVER_SOCKET_RESET_ERROR = 0x1A,
        DME_SERVER_CIRC_BUF_ERROR = 0x1B,
        DME_SERVER_MUTEX_ERROR = 0x1C,
        DME_SERVER_NO_MORE_WORLDS = 0x1D,
        DME_SERVER_ERROR = 0x1E,
        DME_SERVER_CLIENT_LIMIT = 0x1F,
        DME_SERVER_ENCRYPTED_ERROR = 20,
        DME_SERVER_UNSECURED_ERROR = 21,
        DME_SERVER_BUFF_OVERFLOW_ERROR = 22,
        DME_SERVER_CONFIG_ERROR = 23,
        DME_SERVER_PARTIAL_RW_ERROR = 24,
        DME_SERVER_CLIENT_ALREADY_DISCONNECTED = 25
    }

    #endregion

    #region MGCL
    /// <summary>
    /// This structure should always populate the TrustLevel field as MGCL_NOT_TRUSTED for all peer-to-peer titles. 
    /// </summary>
    public enum MGCL_TRUST_LEVEL : int
    {
        /// <summary>
        /// This server is a trusted game server.
        /// </summary>
        MGCL_TRUSTED,
        /// <summary>
        /// This server is NOT a trusted game server.
        /// </summary>
        MGCL_NOT_TRUSTED,
    }

    /// <summary>
    /// This is a enumeration used to identify the current level of service. <br></br>
    /// This prevents create/join games from occurring on this host.
    /// </summary>
    public enum MGCL_ALERT_LEVEL : int
    {
        /// <summary>
        /// Default, no alert occurs. Allow normal use.
        /// </summary>
        MGCL_ALERT_NONE,
        /// <summary>
        /// Low load. No new clients may join.
        /// </summary>
        MGCL_ALERT_LOW,
        /// <summary>
        /// Moderate load. No new clients may join.
        /// </summary>
        MGCL_ALERT_MID,
        /// <summary>
        /// High load. No new clients may join.
        /// </summary>
        MGCL_ALERT_HIGH,
    }

    public enum MGCL_GAME_HOST_TYPE : int
    {
        MGCLGameHostClientServer = 0,
        MGCLGameHostIntegratedServer = 1,
        MGCLGameHostPeerToPeer = 2,
        MGCLGameHostLANPlay = 3,
        MGCLGameHostClientServerAuxUDP = 4,
        MGCLGameHostClientServerUDP = 5,
        MGCLGameHostIndependent = 6,
        MGCLGameHostMax = 7,
    }

    [Flags]
    public enum StartupMessageFlags : byte
    {
        None = 0,
        HasStartupInfo = 1 << 1, // 0x02
        HasGlobalTimeReset = 1 << 2, // 0x04
        HasFieldSetA = 1 << 3, // 0x08
        HasFieldSetB = 1 << 4, // 0x10
        HasFieldSetC = 1 << 5, // 0x20
    }

    public enum MGCL_ERROR_CODE : sbyte
    {
        /// <summary>
        /// Successful Response
        /// </summary>
        MGCL_SUCCESS = 0,
        /// <summary>
        /// Connection Terminated
        /// </summary>
        MGCL_CONNECTION_ERROR = -1,
        /// <summary>
        /// Unable to connect to target host
        /// </summary>
        MGCL_CONNECTION_FAILED = -2,
        /// <summary>
        /// Unable to disconnect from target host
        /// </summary>
        MGCL_DISCONNECT_FAILED = -3,
        /// <summary>
        /// Attempt to use an API call that requires a connection - without a connection
        /// </summary>
        MGCL_NOT_CONNECTED = -4,
        /// <summary>
        /// Sending of data failed
        /// </summary>
        MGCL_SEND_FAILED = -5,
        /// <summary>
        /// initializtion of MGCL Library failed
        /// </summary>
        MGCL_INITIALIZATION_FAILED = -6,
        /// <summary>
        /// Shutdown of MGCL Library failed
        /// </summary>
        MGCL_SHUTDOWN_ERROR = -7,
        MGCL_NETWORK_ERROR = -8,

        MGCL_AUTHENTICATION_FAILED = -9,

        MGCL_SESSIONBEGIN_FAILED = -10,
        MGCL_SESSIONEND_FAILED = -11,
        MGCL_UNSUCCESSFUL = -12,
        MGCL_INVALID_ARG = -13,
        MGCL_NATRESOLVE_FAILED = -14,
        MGCL_GAME_NAME_EXISTS = -15,
        MGCL_WORLDID_INUSE = -16,
        MGCL_DME_ERROR = -17,
        MGCL_CALL_MGCL_CLOSE_BEFORE_REINITIALIZING = -18,
        MGCL_NUM_GAME_WORLDS_PER_LOBBY_WORLD_EXCEEDED = -19,
    }

    #region MGCL_EVENT_TYPE
    /// <summary>
    /// This enumeration specifies the type of connect event that is sent<br></br>
    /// to Medius in an event notification message.
    /// </summary>
    public enum MGCL_EVENT_TYPE : int
    {
        /// <summary>
        /// A client disconnected from this game server/
        /// </summary>
        MGCL_EVENT_CLIENT_DISCONNECT,
        /// <summary>
        /// A client connected to this game server.
        /// </summary>
        MGCL_EVENT_CLIENT_CONNECT,
    }
    #endregion

    #region MGCL_SERVER_ATTRIBUTES
    /// <summary>
    /// This enumeration determines the specific attributes of this server during server authentication.
    /// </summary>
    [Flags]
    public enum MGCL_SERVER_ATTRIBUTES : int
    {
        /// <summary>
        /// This server has no special attributes
        /// </summary>
        MGCL_SERVER_NONE,
        /// <summary>
        /// This server allows for the rebroadcasting of game data.
        /// </summary>
        MGCL_SERVER_ALLOW_REBROADCAST,
        /// <summary>
        /// This server supports specatators to receive data.
        /// </summary>
        MGCL_SERVER_ALLOW_SPECTATOR,
        /// <summary>
        /// This server can be used as a informer type of server. <br></br>
        /// The description is ambiguous on purpose.
        /// </summary>
        MGCL_SERVER_ALLOW_INFORMER,
        /// <summary>
        /// This server can be used to monitor game traffic.
        /// </summary>
        MGCL_SERVER_ALLOW_MONITOR,
    }
    #endregion

    public enum MGCL_WORLD_ATTRIBUTES : int
    {
        MGCL_WORLD_NONE,
        MGCL_WORLD_TYPE_SPECTATOR,
        MGCL_WORLD_ALLOW_SPECTATORS,
        MGCL_WORLD_INTERNAL,
    }

    #endregion

    #region Anti-Cheat
    public enum CheatQueryType : byte
    {
        DME_SERVER_CHEAT_QUERY_RAW_MEMORY = 0,
        DME_SERVER_CHEAT_QUERY_SHA1_HASH = 1,
        DME_SERVER_CHEAT_QUERY_MD5_HASH = 2,
        DME_SERVER_CHEAT_QUERY_ACTIVE_THREAD_CNT = 3,
        DME_SERVER_CHEAT_QUERY_REMENANT_THREAD_CNT = 4,
        DME_SERVER_CHEAT_QUERY_SALTY_SHA1_HASH = 5,
        DME_SERVER_CHEAT_QUERY_SALTY_MD5_HASH = 6,
        DME_SERVER_CHEAT_QUERY_4BYTE_POKE_ADDRESS = 8,
        DME_SERVER_CHEAT_QUERY_NO_OP_FUNC_ADDRESS = 9,
        DME_SERVER_CHEAT_QUERY_INTERRUPT_CNT = 10,
        DME_SERVER_CHEAT_QUERY_THREAD_INFO = 11,
        DME_SERVER_CHEAT_QUERY_STRING_EXISTS = 12,
        DME_SERVER_CHEAT_QUERY_IOP_RAW_MEMORY = 13,
        DME_SERVER_CHEAT_QUERY_IOP_SHA1_HASH = 14,
        DME_SERVER_CHEAT_QUERY_IOP_MD5_HASH = 15,
        DME_SERVER_CHEAT_QUERY_IOP_SALTY_SHA1_HASH = 16,
        DME_SERVER_CHEAT_QUERY_IOP_SALTY_MD5_HASH = 17,
        DME_SERVER_CHEAT_QUERY_IOP_MOD_RAW = 18,
        DME_SERVER_CHEAT_QUERY_IOP_MOD_SHA1 = 19,
        DME_SERVER_CHEAT_QUERY_IOP_MOD_MD5 = 20,
        DME_SERVER_CHEAT_QUERY_IOP_MOD_SALTY_SHA1 = 21,
        DME_SERVER_CHEAT_QUERY_IOP_MOD_SALTY_MD5 = 22,
        DME_SERVER_CHEAT_QUERY_IOP_MOD_CNT = 23,
        DME_SERVER_CHEAT_QUERY_IOP_MOD_TEXT_ADDR = 24,
        DME_SERVER_CHEAT_QUERY_IOP_MOD_TEXT_SIZE = 25,
        DME_SERVER_CHEAT_QUERY_IOP_THREAD_CNT = 26,
        DME_SERVER_CHEAT_QUERY_IOP_MEM_FREE = 27,
        DME_SERVER_CHEAT_QUERY_IOP_MEM_USED = 28
    }

    public enum AnticheatEventCode : int
    {
        anticheatLOBBYCONNECT = 0,
        anticheatLOBBYDISCONNECT = 1,
        anticheatJOINGAME = 2,
        anticheatPERIODIC = 3,
        anticheatLEAVEGAME = 4,
        anticheatPLAYERREPORT = 5,
        anticheatSTATSREPORT = 6,
        anticheatCHATMESSAGE = 7,
        anticheatDELAYED = 8,
        anticheatRECONFIG = 9,
        anticheatCREATELOBBYWORLD = 10,
        anticheatGETCHANNELLIST = 11,
        anticheatGETGAMELIST = 12,
        anticheatGETMYCLANS = 13,
        anticheatGETANNOUNCEMENTS = 14,
        anticheatMAXEVENTS = 15,
        anticheatLEAVEPARTY = 16,
    }

    public enum anticheatLOBBYCONNECT : int
    {
        anticheatLOBBYCONNECT = 0,
        anticheatLOBBYDISCONNECT = 1,
        anticheatJOINGAME = 2,
        anticheatPERIODIC = 3,
        anticheatLEAVEGAME = 4,
        anticheatPLAYERREPORT = 5,
        anticheatSTATSREPORT = 6,
        anticheatCHATMESSAGE = 7,
        anticheatDELAYED = 8,
        anticheatRECONFIG = 9,
        anticheatCREATELOBBYWORLD = 10,
        anticheatGETCHANNELLIST = 11,
        anticheatGETGAMELIST = 12,
        anticheatGETMYCLANS = 13,
        anticheatGETANNOUNCEMENTS = 14,
        anticheatMAXEVENTS = 15,
    }

    public enum LM_SEVERITY_LEVEL : int
    {
        LM_INFO,
        LM_ANOMALY,
        LM_ERROR,
        LM_CRITICAL,
        LM_NONE,
        LM_DEBUG
    }

    #endregion

    #region Billing

    public enum MediusBillingBSPType : int
    {
        MEDIUS_BILLING_NOTUSED,
        MEDIUS_BILLING_SCEK,
        MEDIUS_BILLING_SCEA,
        MEDIUS_BILLING_SCEJ,
        MEDIUS_BILLING_SCEE
    }

    #endregion

    #region Notification Manager 

    public enum NMMessageTypeCode : int
    {
        NM_MESSAGE_TYPE_SINGLE = 0,
        NM_MESSAGE_TYPE_BROADCAST = 1,
        NM_MESSAGE_TYPE_GROUP = 2
    }

    #endregion

    #region Medius DList

    public enum MediusDListID : ushort
    {
        MEDIUS_DLIST_BUDDY = 0,
        MEDIUS_DLIST_CLAN_MEMBER = 1,
        MEDIUS_DLIST_LOBBY_MEMBER = 2,
        MEDIUS_DLIST_LAST = 3,
    }

    public enum MediusDListServiceLevel : byte
    {
        MEDIUS_DLEVEL_UNSUB = 0,
        MEDIUS_DLEVEL_CHANGE_EVENTS = 1,
        MEDIUS_DLEVEL_ALL_EVENTS = 2,
        MEDIUS_DLEVEL_REFRESHED = 3,
        MEDIUS_DLEVEL_RESERVED1 = 4,
        MEDIUS_DLEVEL_LAST = 5
    }

    public enum MediusDListRequestID : int
    {
        MEDIUS_DREQUEST_NONE = 0,
        MEDIUS_DREQUEST_SUBSCRIBE = 1,
        MEDIUS_DREQUEST_UNSUBSCRIBE = 2,
        MEDIUS_DREQUEST_REFRESH = 3,
        MEDIUS_SET_LEVEL = 4,
        MEDIUS_DREQUEST_LAST = 5
    }

    public enum MediusDInterestID : int
    {
        MEDIUS_DINTEREST_PLAYER = 0,
        MEDIUS_DINTEREST_LAST = 1,
    }

    public enum MediusDListAction : int
    {
        MEDIUS_DACTION_NOEVENT = 0,
        MEDIUS_DACTION_ERROR = 1,
        MEDIUS_DACTION_STATUS = 2,
        MEDIUS_DACTION_UPDATE = 3,
        MEDIUS_DACTION_ADD = 4,
        MEDIUS_DACTION_DELETE = 5,
        MEDIUS_DACTION_REFRESH = 6,
        MEDIUS_DACTION_LAST = 7
    }

    public enum MEDIUS_DTYPE : int
    {
        MEDIUS_INVALID_TYPE = 0,
        MEDIUS_DTYPE_CHAR = 1,
        MEDIUS_DTYPE_UCHAR = 2,
        MEDIUS_DTYPE_INT16 = 3,
        MEDIUS_DTYPE_UINT16 = 4,
        MEDIUS_DTYPE_INT32 = 5,
        MEDIUS_DTYPE_UINT32 = 6,
        MEDIUS_DTYPE_INT64 = 7,
        MEDIUS_DTYPE_UINT64 = 8,
        MEDIUS_DTYPE_FLOAT = 9,
        MEDIUS_DTYPE_DOUBLE = 10,
        MEDIUS_DTYPE_CSTRING = 11,
        MEDIUS_DTYPE_LAST = 12
    }

    #endregion

    #region MediusUniverseVariableInformationInfoFilter
    [Flags]
    public enum MediusUniverseVariableInformationInfoFilter : uint
    {
        INFO_UNIVERSES = (1 << 0),
        INFO_NEWS = (1 << 1),
        INFO_ID = (1 << 2),
        INFO_NAME = (1 << 3),
        INFO_DNS = (1 << 4),
        INFO_DESCRIPTION = (1 << 5),
        INFO_STATUS = (1 << 6),
        INFO_BILLING = (1 << 7),
        INFO_EXTRAINFO = (1 << 8),
        INFO_SVO_URL = (1 << 9),
    }
    #endregion

    /// <summary>
    /// Status of an outstanding clan challenge
    /// </summary>
    public enum MediusClanChallengeStatus : int
    {
        /// <summary>
        /// This is a request to challenge a clan.
        /// </summary>
        ClanChallengeRequest,
        /// <summary>
        /// Accept a clan challenge.
        /// </summary>
        ClanChallengeAccepted,
        /// <summary>
        /// Revoke an outstanding challenge to a clan.
        /// </summary>
        ClanChallengeRevoked,
        /// <summary>
        /// Refuse a request to be challenged.
        /// </summary>
        ClanChallengeRefused,
        /// <summary>
        /// Accept and confirm a challenge.
        /// </summary>
        ClanChallengeConfirmed,
    }


    #region MediusClanInvitationsResponseStatus
    /// <summary>
    /// Status of an outstanding clan challenge
    /// </summary>
    public enum MediusClanInvitationsResponseStatus : int
    {
        /// <summary>
        /// Status to join a clan is undecided.
        /// </summary>
        ClanInvitationUndecided,
        /// <summary>
        /// Accept the invitation to the clan.
        /// </summary>
        ClanInvitationAccept,
        /// <summary>
        /// Decline the invitation to the clan.
        /// </summary>
        ClanInvitationDecline,
        /// <summary>
        /// Revoke an outstanding invitation to a potential candidate.
        /// </summary>
        ClanInvitationRevoked,
    }
    #endregion

    #region MediusClanMessageStatus
    /// <summary>
    /// Status of a clan message
    /// </summary>
    public enum MediusClanMessageStatus : int
    {
        /// <summary>
        /// The clan message is marked as unread.
        /// </summary>
        ClanMessageUnread,
        /// <summary>
        /// The clan message has been modified.
        /// </summary>
        ClanMessageModified,
        /// <summary>
        /// The clan message has been deleted.
        /// </summary>
        ClanMessageDeleted,
        /// <summary>
        /// The clan message is marked as read.
        /// </summary>
        ClanMessageRead,
    }
    #endregion

    #region MediusClanPlayerStatus
    public enum MediusClanPlayerStatus : int
    {
        /// <summary>
        /// A player has not yet accepted the invitation.
        /// </summary>
        NotYetAccepted,
        /// <summary>
        /// The player is an active member in the clan.
        /// </summary>
        PlayerActiveInClan,
        /// <summary>
        /// The player has been removed from the clan.
        /// </summary>
        PlayerRemovedFromClan,
    }
    #endregion

    public enum MediusTokenActionType : int
    {
        MediusInvalidTokenAction = 0,
        MediusAddToken = 1,
        MediusUpdateToken = 2,
        MediusRemoveToken = 3,
    }

    #region MediusTokenCategoryType
    /// <summary>
    /// Enumeration used to identify category of a MediusToken
    /// </summary>
    public enum MediusTokenCategoryType : int
    {
        /// <summary>
        /// Invalid token category
        /// </summary>
        MediusInvalidToken = 0,
        /// <summary>
        /// Generic token category 1
        /// </summary>
        MediusGenericToken1 = 1,
        /// <summary>
        /// Generic token category 2
        /// </summary>
        MediusGenericToken2 = 2,
        /// <summary>
        /// Generic token category 3
        /// </summary>
        MediusGenericToken3 = 3,
        /// <summary>
        /// Token Assosciated with the account
        /// </summary>
        MediusAccountToken = 4,
        /// <summary>
        /// Token associated with a clan
        /// </summary>
        MediusClanToken = 5,
        /// <summary>
        /// Placeholder to normalize the field size on different compilers
        /// </summary>
        ExtraMediusTokenCategoryType = 0xffffff
    }
    #endregion

    public enum ONLINE_STATUS_TYPE : int
    {
        OFFLINE,
        AVAILABLE,
        PRIVATE,
        AWAY,
        IDLE,
        STAGING,
        LOADING,
        IN_GAME,
        CHAT_HOST,
        CHAT_CLIENT,
        MAX_ONLINE_STATUS_TYPE
    }

    #region DME 

    public enum DME_SERVER_LANGUAGE_TYPE : byte
    {
        DME_SERVER_LANGUAGE_NONE = 0,
        DME_SERVER_LANGUAGE_US_ENGLISH = 1,
        DME_SERVER_LANGUAGE_UK_ENGLISH = 2,
        DME_SERVER_LANGUAGE_JAPANESE = 3,
        DME_SERVER_LANGUAGE_KOREAN = 4,
        DME_SERVER_LANGUAGE_ITALIAN = 5,
        DME_SERVER_LANGUAGE_SPANISH = 6,
        DME_SERVER_LANGUAGE_GERMAN = 7,
        DME_SERVER_LANGUAGE_FRENCH = 8,
        DME_SERVER_LANGUAGE_DUTCH = 9,
        DME_SERVER_LANGUAGE_PORTUGUESE = 10,
        DME_SERVER_LANGUAGE_CHINESE = 11,
        DME_SERVER_LANGUAGE_TAIWANESE = 12,
    }

    public enum DME_SERVER_ENCODING_TYPE : byte
    {
        DME_SERVER_ENCODING_NONE,
        DME_SERVER_ENCODING_ISO_8859_1,
        DME_SERVER_ENCODING_UTF8
    }

    #endregion

    #region Medius File Services
    public enum MediusFileXferStatus : int
    {
        Error = 0,
        Initial = 1,
        Mid = 2,
        End = 3
    }

    public enum MFS_TransferResult : int
    {
        mfsUPLOAD_SUCCESS,
        mfsDOWNLOAD_SUCCESS,
        mfsUPLOAD_FAILURE,
        mfsDOWNLOAD_FAILURE
    }
    #endregion

    #region Medius Universe Manager

    public enum MUMCacheNextGameRetrieveType : int
    {
        MC_EXHAUSTIVE_SEARCH,
        MC_USING_LOBBY_ASSOC
    }

    public enum MC_MUM_CACHE_CONFIG_OPTION_TYPE : int
    {
        MC_MUM_CACHE_CONFIG_OPTION_ID_COMPAT,
        MC_MUM_CACHE_CONFIG_OPTION_GLIST_SHIELD,
        MC_MUM_CACHE_CONFIG_OPTION_ACCOUNT_STATS_AS_STRING,
        MC_MUM_CACHE_CONFIG_OPTION_DELTA_HASH_FAILURE_ACTION,
        MC_MUM_CACHE_CONFIG_OPTION_IN_GAME_NOTIFIER,
        MC_MUM_CACHE_CONFIG_OPTION_MAX
    }

    public enum MUM_RESULT : int
    {
        MUM_RESULT_OK,
        MUM_RESULT_FAILURE
    }

    #endregion

    #region Medius Unified Community Gateway
    public enum MUCG_MessageTypes : byte
    {
        MUCG_MsgVersion = 0,
        MUCG_MsgError = 1,
        MUCG_MsgLobbyInitialize = 2,
        MUCG_MsgLobbyAcceptance = 3,
        MUCG_MsgReqConnectedAccounts = 4,
        MUCG_MsgAccountAdd = 5,
        MUCG_MsgAccountConnect = 6,
        MUCG_MsgAccountDisconnect = 7,
        MUCG_MsgAccountRemove = 8,
        MUCG_MsgReqBuddyListPresence = 9,
        MUCG_MsgReqAccountPresence = 10,
        MUCG_MsgC, at = 11,
        MUCG_MsgPresence = 12,
        MUCG_MsgBuddyAdd = 13,
        MUCG_MsgBuddyRemove = 14,
        MUCG_MsgSync = 15,
        MUCG_MsgRecoverMode = 16,
        MUCG_MsgRecoverAccount = 17,
        MUCG_MsgProcessSync = 18,
        MUCG_MsgEventOnOffline = 19,
        MUCG_MsgEventBuddyListMod = 20,
        MUCG_MsgClanAdd = 21,
        MUCG_MsgClanRemove = 22,
        MUCG_MsgClanMemberAdd = 23,
        MUCG_MsgClanMemberRemove = 24,
        MUCG_MsgClanInviteAdd = 25,
        MUCG_MsgClanInviteRemove = 26,
        MUCG_MsgClanChat = 27,
        MUCG_MsgReqClanInvites = 28,
        MUCG_MsgClanInvite = 29,
        MUCG_MsgReqClanMembers = 30,
        MUCG_MsgEventClanMemberListMod = 31,
        MUCG_MsgEventClanInviteListMod = 32,
        MUCG_MsgEventClanDisband = 33,
        MUCG_MsgIgnoreAdd = 34,
        MUCG_MsgIgnoreRemove = 35,
        MUCG_MsgReqIgnoreListIDs = 36,
        MUCG_MsgReqClanIDs = 37,
        MUCG_MsgClanID = 38,
        MUCG_Msg_End = 39,
    }

    public enum MUCG_RESULT : byte
    {
        MUCG_RESULT_OK = 0,
        MUCG_RESULT_CONNECT_INIT_FAILED = 1,
        MUCG_RESULT_CONNECT_FAILED = 2,
        MUCG_RESULT_RT_MSG_ERROR = 3,
        MUCG_RESULT_DISCONNECT_FAILURE = 4,
        MUCG_RESULT_UPDATE_FAILURE = 5,
        MUCG_RESULT_INCOMPATIBLE_VERSION = 6,
        MUCG_RESULT_PARSE_ERROR = 7,
        MUCG_RESULT_PACK_ERROR = 8,
        MUCG_RESULT_INVALID_PARAMETER = 9,
        MUCG_RESULT_DATA_PERSIST_FAILED = 10,
        MUCG_RESULT_DATA_NOT_READY = 11,
        MUCG_RESULT_GATEWAY_ERROR = 12,
        MAX_MUCG_RESULT = 13
    }

    public enum MUCG_STATE : int
    {
        MUCG_STATE_DISCONNECTED,
        MUCG_STATE_CONNECTING,
        MUCG_STATE_VERIFYING,
        MUCG_STATE_INITIALIZING,
        MUCG_STATE_INITIALIZED,
        MUCG_STATE_PENDING_DISCONNECT,
        MUCG_STATE_IN_RECOVERY,
        MAX_MUCG_STATE
    }

    #endregion

    #region Zipper Interactive

    #region MAG Types
    public enum FactionType : int
    {
        FactionTypeFirst,
        FactionTypeGreen = 0,
        FactionTypeAllies = 0,
        FactionTypeBlack,
        FactionTypeHammer = 1,
        FactionTypeRed,
        FactionTypeSIRSAT = 2,
        FactionTypeMax,
        FactionTypeNone = 0xFFFFFFF,
        FactionTypeAll = 0xFFFFFFF
    }

    public enum GameType : int
    {
        GameTypeFirst,
        GameType64 = 0,
        GameType128,
        GameType256,
        GameTypeFactionExercise,
        GameTypeMaxQueueable,
        GameTypeTraining = 4,
        NumGameTypes,
        GameTypeNone = 0xFFFFFFF
    }

    public enum NetMessageTypeIds : int
    {
        //MAG

        kNetKernelMessageTypeStart = 0,
        NetMessageTypeHello = 1,
        NetMessageTypeProtocolInfo = 2,
        NetMessageTypeClientClaimReservation = 3,
        NetMessageTypeClientGameInfo = 4,
        NetMessageTypeClientReady = 5,
        NetMessageTypeClientEnterGame = 6,
        NetMessageTypeFieldUpdate = 7,
        NetMessageTypeCorrectionAck = 8,
        NetMessageTypeDebugText = 9,
        NetMessageTypeNetTimeRequest = 0xA,
        NetMessageTypeNetTimeResponse = 0xB,
        NetMessageTypePeerPlugin = 0xC,
        NetMessageTypeSendToGroup = 0xD,
        NetMessageTypeGroupMessage = 0xE,
        NetMessageTypeVoice = 0xF,
        NetMessageTypePluginId = 0x10,
        NetMessageTypeJoinGameRequest = 0x11,
        NetMessageTypeJoinGameResponse = 0x12,
        NetMessageTypeJoinGameUpdate = 0x13,
        NetMessageTypeCancelJoinRequest = 0x14,
        NetMessageTypeCancelJoinResponse = 0x15,
        NetMessageTypeQueueControlsRequest = 0x16,
        NetMessageTypeQueueControlsResponse = 0x17,
        NetMessageTypePartyInvite = 0x18,
        NetMessageTypePartyInviteResponse = 0x19,
        NetMessageTypePartyLeave = 0x1A,
        NetMessageTypePartyQueue = 0x1B,
        NetMessageTypePartyCancelQueue = 0x1C,
        NetMessageTypePartyEventJoin = 0x1D,
        NetMessageTypePartyEventLeave = 0x1E,
        NetMessageTypePartyEventUpdate = 0x1F,
        NetMessageTypePartyEventDecline = 0x20,
        NetMessageTypePartyReservation = 0x21,
        NetMessageTypePartyStatusUpdate = 0x22,
        NetMessageTypePartyOpenInvite = 0x23,
        NetMessageTypePartyRejoinRequest = 0x24,
        NetMessageTypePartyRejoinResponse = 0x25,
        NetMessageTypeVoiceDmeRemoveParty = 0x26,
        NetMessageTypeVoiceDmeShutdown = 0x27,
        NetMessageTypeVoiceIndexAcctMap = 0x28,
        NetMessageTypeVoiceSetGroup = 0x29,
        NetMessageTypePlayerKicked = 0x2A,
        NetMessageTypeAccountLoginRequest = 0x2B,
        NetMessageTypeAccountLoginResponse = 0x2C,
        NetMessageTypeAccountLogoutRequest = 45, // 45
        NetMessageTypeAccountLogoutResponse = 46,
        NetMessageTypeCreateAccountRequest = 0x2F,
        NetMessageTypeCreateAccountResponse = 0x30,
        NetMessageTypeNPAuthenticateRequest = 0x31,
        NetMessageTypeNPAuthenticateResponse = 0x32,
        NetMessageTypeMAPSHelloMessage = 51,
        NetMessageTypeUniverseListRequest = 0x34,
        NetMessageTypeUniverseListResponse = 0x35,
        NetMessageTypeJoinGameConfirm = 0x36,
        NetMessageTypeQueueControls = 0x37,
        NetMessageTypeServerStatus = 0x38,
        kNetGameMessageTypeStart = 0x1F4,
        NetMessageTypePatchRequest = 0x1F5,
        NetMessageTypePatchResponse = 0x1F6,
        NetMessageTypeClientReportingInfo = 0x1F7,
        NetMessageTypeClientReport = 0x1F8,
        NetMessageTypeGameErrorNotification = 0x1F9,
        NetMessageTypeInvulnerability = 0x1FA,
        kNetMessageTypeClans = 0x1FB,
        kNetGameMessageTypeEnd = 0x1FC,
        kNetObjectMessageTypeStart = 0x3E8,
        kNetObjectMessageTypeEnd = 0x3E9,
        kNetGroupMessageTypeStart = 0x5DC,
        kNetGroupMessageTypeEnd = 0x5DD,
        kNetPluginMessageTypeStart = 0x7D0,
        NetMessageTypeMUMIntroMessage = 0x7D1,
        NetMessageTypePluginRegister = 0x7D2,
        NetMessageTypeAppSpecLoadRequest = 0x7D3,
        NetMessageTypeAppSpecLoadResponse = 0x7D4,
        kNetPluginMessageTypeEnd = 0x7D5,

        NetAccountLogoutRequest = 45,
        NetMessageCharacterDataRequest,
        NetMessageCharacterDataResponse,
        NetMessageCharacterListRequest = 566,
        NetMessageCharacterListResponse = 567,
        NetMessageNewsEulaRequest = 574, // 23E
        NetMessageNewsEulaResponse = 575,
        NetMessageServerStatusRequest = 587,
        NetMessageServerStatusResponse = 588, //Needs Debug Check


        // SOCOM 4
        /*
        kNetKernelMessageTypeStart = 0,
        NetMessageTypeHello = 1,
        NetMessageTypeProtocolInfo = 2,
        NetMessageTypeKeepAlive = 3,
        NetMessageTypeClientClaimReservation = 4,
        NetMessageTypeClientCompleteReservation = 5,
        NetMessageTypeClientGameInfo = 6,
        NetMessageTypeClientReady = 7,
        NetMessageTypeClientEnterGame = 8,
        NetMessageTypeFieldUpdate = 9,
        NetMessageTypeCorrectionAck = 0xA,
        NetMessageTypeDebugText = 0xB,
        NetMessageTypeTransportHandshake = 0xC,
        NetMessageTypeNetTimeRequest = 0xD,
        NetMessageTypeNetTimeResponse = 0xE,
        NetMessageTypePeerPlugin = 0xF,
        NetMessageTypeSendToGroup = 0x10,
        NetMessageTypeGroupMessage = 0x11,
        NetMessageTypeVoice = 0x12,
        NetMessageTypeVoiceIn = 0x13,
        NetMessageTypeVoiceOut = 0x14,
        NetMessageTypePluginId = 0x15,
        NetMessageTypeJoinGameRequest = 0x16,
        NetMessageTypeJoinGameResponse = 0x17,
        NetMessageTypeJoinGameUpdate = 0x18,
        NetMessageTypeCancelJoinRequest = 0x19,
        NetMessageTypeCancelJoinResponse = 0x1A,
        NetMessageTypeCancelReservation = 0x1B,
        NetMessageTypeQueueControlsRequest = 0x1C,
        NetMessageTypeQueueControlsResponse = 0x1D,
        NetMessageTypePartyInvite = 0x1E,
        NetMessageTypePartyInviteResponse = 0x1F,
        NetMessageTypePartyLeave = 0x20,
        NetMessageTypePartyQueue = 0x21,
        NetMessageTypePartyCancelQueue = 0x22,
        NetMessageTypePartyEventJoin = 0x23,
        NetMessageTypePartyEventLeave = 0x24,
        NetMessageTypePartyEventUpdate = 0x25,
        NetMessageTypePartyEventDecline = 0x26,
        NetMessageTypePartyStatusUpdate = 0x27,
        NetMessageTypePartyOpenInvite = 0x28,
        NetMessageTypePartyRejoinRequest = 0x29,
        NetMessageTypePartyRejoinResponse = 0x2A,
        NetMessageTypePartyPromoteToLeader = 0x2B,
        NetMessageTypePartyKickMember = 0x2C,
        NetMessageTypeVoiceDmeRemoveParty = 0x2D,
        NetMessageTypeVoiceDmeShutdown = 0x2E,
        NetMessageTypeVoiceIndexAcctMap = 0x2F,
        NetMessageTypeVoiceSetGroup = 0x30,
        NetMessageTypePlayerKicked = 0x31,
        NetMessageTypeTrainingCompleted = 0x32,
        NetMessageTypeNATTypeUpdate = 0x33,
        NetMessageTypeClanTagUpdate = 0x34,
        NetMessageTypeAccountLoginRequest = 0x35,
        NetMessageTypeAccountLoginResponse = 0x36,
        NetMessageTypeAccountLogoutRequest = 55,
        NetMessageTypeAccountLogoutResponse = 56,
        NetMessageTypeCreateAccountRequest = 0x39,
        NetMessageTypeCreateAccountResponse = 0x3A,
        NetMessageTypeNPAuthenticateRequest = 0x3B,
        NetMessageTypeNPAuthenticateResponse = 0x3C,
        NetMessageTypeMAPSHelloMessage = 0x3D,
        NetMessageTypeUniverseListRequest = 0x3E,
        NetMessageTypeUniverseListResponse = 0x3F,
        NetMessageTypeJoinGameConfirm = 0x40,
        NetMessageTypeQueueControls = 0x41,
        NetMessageTypeServerStatus = 0x42,
        kNetGameMessageTypeStart = 500,
        NetMessageTypePatchRequest = 0x1F5,
        NetMessageTypePatchSettings = 0x1F6,
        NetMessageTypePatchURL = 0x1F7,
        NetMessageTypeClientReportingInfo = 0x1F8,
        NetMessageTypeClientReport = 0x1F9,
        NetMessageTypeFeatureGuard = 0x1FA,
        NetMessageTypeGameErrorNotification = 0x1FB,
        NetMessageTypeInvulnerability = 0x1FC,
        kNetMessageTypeClans = 0x1FD,
        NetMessageTypePauseGame = 0x1FE,
        NetMessageTypePartyReservation = 0x1FF,
        kNetGameMessageTypeEnd = 0x200,
        kNetObjectMessageTypeStart = 0x3E8,
        kNetObjectMessageTypeEnd = 0x3E9,
        kNetGroupMessageTypeStart = 0x5DC,
        kNetGroupMessageTypeEnd = 0x5DD,
        kNetPluginMessageTypeStart = 0x7D0,
        NetMessageTypeMUMIntroMessage = 0x7D1,
        NetMessageTypePluginRegister = 0x7D2,
        NetMessageTypeAppSpecLoadRequest = 0x7D3,
        NetMessageTypeAppSpecLoadResponse = 0x7D4,
        NetMessageTypeQueueEntityRequest = 0x7D5,
        NetMessageTypeUnqueueEntityRequest = 0x7D6,
        NetMessageTypeForcePlayerIntoGame = 0x7D7,
        NetMessageTypeForcePlayerIntoGameResponse = 0x7D8,
        NetMessageTypeShutdownGameServer = 0x7D9,
        kNetPluginMessageTypeEnd = 0x7DA,
        */

    }
    #endregion

    #region GameNetClient

    public enum RespawnStatus : int
    {
        kRespawnNone,
        kRespawnCancel,
        kRespawnConfirm,
        kRespawnRequestSent
    }

    public enum MicrophoneState : int
    {
        kMicOff,
        kMicRequesting,
        kMicRecording
    }

    #endregion

    #region MagNetClient
    public enum CharacteRequestType : int
    {
        kCharacterRequestNone,
        kCharacterRequestCreate,
        kCharacterRequestDelete,
        kCharacterRequestFactionReSpec
    }

    #endregion


    public enum NetResult : int
    {
        kNetSuccess = 0,
        kNetError = 1,
        kNetTimeout = 2,
        kNetCancel = 3,
        kNetMediusDisconnect = 4,
        kNetGameDisconnect = 5,
        kNetNameTaken = 6,
        kNetNameInvalid = 7,
        kNetCableDisconnect = 8,
        kNetNPDisconnect = 9,
        kNetNPConnectFailed = 0xA,
        kNetNPNoAccount = 0xB,
        kNetProtocolError = 0xC,
        kNetNotImplemented = 0xD
    }

    public enum NetPluginType : int
    {
        kNetPluginNone,
        kNetPluginMAPS,
        kNetPlgionMAS,
        kNetPlgionMLS,
        kNetPlgionMPS,
        kNetPlgionMUM,
        kNetPlgionDME,
    }

    public enum QueryType : int
    {
        kQueryTypeNone,
        kQueryByName,
        kQueryByID
    }

    #region zNetwork

    public enum NetAutoTestType : int
    {
        kAutoTestNone,
        kAutoTestGrenadeThrow,
        kAutoTestFirePrimary
    }

    public enum NetMessageNewsEulaResponseContentType : byte
    {
        News,
        Eula
    }

    public enum NetAccountLoginResponse : int
    {
        eResultFail,
        eResultSuccess,
        eResultQueue
    }

    public enum NetMessageUserNote : int
    {
        kNone,
        kGeneralComplaint,
        kTerrain_0,
        kSpawnCamping,
        kEnd
    }

    public enum NetClientType : int
    {
        kNetClientPlayer,
        kNetClientObserver
    }

    public enum JoinGamePreferenceType : int
    {
        JoinGamePreferenceNone,
        JoinGamePreferenceGameType,
        JoinGamePreferenceSpecificGame,
        JoinGamePreferenceLocation,
        JoinGamePreferenceTraining,
        JoinGamePreferenceDirective
    }

    public enum PartyInviteResponse : int
    {
        kPartyInviteAccept,
        kPartyInviteDecline,
        kPartyInviteError,
        kPartyInviteInParty,
        kPartyInviteTimeout
    }

    public enum PartyStatus : int
    {
        kPartyStatusNone,
    }

    public enum VictoryType : int
    {
        VictoryTypeFirst,
        VictoryTypeMinor = 0,
        VictoryTypeMajor,
        VictoryTypeMax,
        VictoryTypeNone = 0xFFFFFFF
    }

    #endregion

    #endregion

    #region GHS

    public enum TypeRtMsgState : int
    {
        RTMSG_DISCONNECTED,
        RTMSG_KEY_EXCHANGE,
        RTMSG_TCPCONNECT,
        RTMSG_CONNECTED,
    }

    public enum GhsMgrResult : int
    {
        ghsmgrOK,
        ghsmgrERROR = -1
    }

    public enum GhsMgrNextStep : int
    {
        ghsmgrBillingLoginRegistration,
        ghsmgrScertAccountRegistration,
        ghsmgrScertAccountConfirmation,
    }

    public enum GhsClientResult : int
    {
        gcOK = 0,
        gcPARAMETER_ERROR = -7,
        gcNET_SEND_ERROR = -6,
        gcNET_POLL_ERROR = -5,
        gcNET_CONNECT_ERROR = -4,
        gcNET_ADDRESS_ERROR = -3,
        gcNOMEM = -2,
        gcERROR = -1,
    }

    public enum GhsClientState : int
    {
        GhsClient_OFFLINE,
        GhsClient_ONLINE,
        GhsClient_CONNECTING,
        GhsClient_VERSION_XCHNG,
        GhsClient_IN_ERROR,
    }
    public enum GhsOpcode : ushort
    {
        ghs_ClientProtocolChoice = 1,
        ghs_ClientRequestNameAccountID = 2,
        ghs_ClientConfirmNameAccountID = 3,
        ghs_ClientErrorGeneral = 24576,
        ghs_ClientErrorInvalidRequestID = 24577,
        ghs_ServerProtocolNegotiation = 32769, //32769
        ghs_ServerGrantNameAccountID = 32770, //32770
        ghs_ServerConfirmNameAccountID = 32771,
        ghs_ServerRejectName = 32772,
        ghs_ServerErrorGeneral = 57344,
        ghs_ServerErrorClientRequest = 57345,
        ghs_ServerErrorInvalidProtocol = 57346,
        ghs_ServerErrorNameIdMismatch = 57347,
    }

    #endregion

    #region OTG Telemetry

    public enum eSceotTimeSpecificResultCode : int
    {
        kOtgTimeResult_NotSupported = 0xCC0002,
        kOtgTimeResult_InvalidParam = 0xCC0003,
        kOtgTimeResult_BaseTimeNotInitialized = 0xCC0004,
        kOtgTimeResult_UnknownError = 0xCC0006,
        kOtgTimeResult_InvalidTime = 0xCC0016,
        kOtgTimeResult_SystemError = 0xCC0026,
        kOtgTimeResult_Overflow = 0xCC0036
    }

    public enum eSceotTelemetryProtocolMsgType : int
    {
        kSceotTelemetryProtocolMsgType_StartServiceRequest = 0,
        kSceotTelemetryProtocolMsgType_StopServiceRequest = 1,
        kSceotTelemetryProtocolMsgType_BeginFrameRequest = 2,
        kSceotTelemetryProtocolMsgType_ResumeFrameRequest = 3,
        kSceotTelemetryProtocolMsgType_EndFrameRequest = 4,
        kSceotTelemetryProtocolMsgType_SendNextBlockRequest = 5,
        kSceotTelemetryProtocolMsgType_ThrottleRequest = 6,
        kSceotTelemetryProtocolMsgType_StartServiceResponse = 7,
        kSceotTelemetryProtocolMsgType_StopServiceResponse = 8,
        kSceotTelemetryProtocolMsgType_BeginFrameResponse = 9,
        kSceotTelemetryProtocolMsgType_ResumeFrameResponse = 0xA,
        kSceotTelemetryProtocolMsgType_EndFrameResponse = 0xB,
        kSceotTelemetryProtocolMsgType_SendNextBlockResponse = 0xC,
        kSceotTelemetryProtocolMsgType_ThrottleResponse = 0xD,
        kSceotTelemetryProtocolMsgType_StartServiceRequestWithSampling = 0xF,
        kSceotTelemetryProtocolMsgType_BeginFrameRequestWithSampling = 0xE,
        kSceotTelemetryProtocolMsgType_Bounds = 0x10,
        kSceotTelemetryProtocolMsgType_ForceLong = -1
    }

    public enum eOtgTelemetryClientResultCode : int
    {
        kOtgTelemetryClientResultCode_NoError = 0,
        kOtgTelemetryClientResultCode_Generic = 1,
        kOtgTelemetryClientResultCode_InternalError = 2,
        kOtgTelemetryClientResultCode_VersionIncompatible = 3,
        kOtgTelemetryClientResultCode_NullPtr = 4,
        kOtgTelemetryClientResultCode_AlreadyInitialized = 5,
        kOtgTelemetryClientResultCode_OutOfMemory = 6,
        kOtgTelemetryClientResultCode_InvalidParam = 7,
        kOtgTelemetryClientResultCode_FailedServiceAlreadyStarted = 8,
        kOtgTelemetryClientResultCode_FailedServiceAlreadyStopping = 9,
        kOtgTelemetryClientResultCode_FailedServiceAlreadyStopped = 0xA,
        kOtgTelemetryClientResultCode_FailedServiceNotStarted = 0xB,
        kOtgTelemetryClientResultCode_FailedServiceStillStarting = 0xC,
        kOtgTelemetryClientResultCode_FailedServiceFailedStart = 0xD,
        kOtgTelemetryClientResultCode_OutOfOpenFrames = 0xE,
        kOtgTelemetryClientResultCode_OutOfOpenFrameAcks = 0xF,
        kOtgTelemetryClientResultCode_FrameNotFound = 0x10,
        kOtgTelemetryClientResultCode_InvalidRequestId = 0x11,
        kOtgTelemetryClientResultCode_InvalidHandle = 0x12,
        kOtgTelemetryClientResultCode_FrameNotActive = 0x13,
        kOtgTelemetryClientResultCode_ServerError = 0x14,
        kOtgTelemetryClientResultCode_ConnectionFailure = 0x15,
        kOtgTelemetryClientResultCode_InvalidState = 0x16,
        kOtgTelemetryClientResultCode_FatalError = 0x17,
        kOtgTelemetryClientResultCode_FrameTimeout = 0x18,
        kOtgTelemetryClientResultCode_FrameDataQueueFull = 0x19,
        kOtgTelemetryClientResultCode_AllocFailure = 0x1A,
        kOtgTelemetryClientResultCode_CommonServiceInitFailed = 0x1B,
        kOtgTelemetryClientResultCode_FrameResumeFailed = 0x1C,
        kOtgTelemetryClientResultCode_ConnectionReady = 0x1D,
        kOtgTelemetryClientResultCode_ConnectionLost = 0x1E,
        kOtgTelemetryClientResultCode_FrameBeginFailed = 0x1F,
        kOtgTelemetryClientResultCode_FrameEndFailed = 0x20,
        kOtgTelemetryClientResultCode_GetNetworkTimeFailed = 0x21,
        kOtgTelemetryClientResultCode_ClientNotSampled = 0x22,
        kOtgTelemetryClientResultCode_FrameNotSampled = 0x23,
        kOtgTelemetryClientResultCode_Bounds = 0x24,
        kOtgTelemetryClientResultCode_ForceLong = -1
    }

    public enum kOtgSpecificResultCodes : int
    {
        kOtgSpecificResultCodes_NotSupported = 0x10011,
        kOtgSpecificResultCodes_NotImplemented = 0x10012,
        kOtgSpecificResultCodes_InvalidParam = 0x10013,
        kOtgSpecificResultCodes_TimeOut = 0x10015,
        kOtgSpecificResultCodes_ResultUnknown = 0x10016,
        kOtgSpecificResultCodes_LinkServiceFailed = 0x10016,
        kOtgSpecificResultCodes_ConfigFileLoadFailed = 0x10016,
        kOtgSpecificResultCodes_ConnectFailed = 0x10017,
        kOtgSpecificResultCodes_MemoryAllocResult = 0x10019,
        kOtgSpecificResultCodes_UnexpectedNull = 0x10023,
        kOtgSpecificResultCodes_NumerialOutOfRange = 0x10033,
        kOtgSpecificResultCodes_ConnectReject = 0x10045,
        kOtgSpecificResultCodes_SpecificExtra = -1
    }

    public enum kOtgGenericResultCodes : int
    {
        kOtgResultOk = 0,
        kOtgGenericResultCodes_Unknown = 1,
        kOtgGenericResultCodes_UnexpectedNull = 2,
        kOtgGenericResultCodes_NumerialOutOfRange = 3,
        kOtgGenericResultCodes_InProgress = 4,
        kOtgGenericResultCodes_Unsupported = 5,
        kOtgGenericResultCodes_Count = 6,
        kOtgGenericResultCodes_Extra = -1,
    }

    public enum kOtgResultCategory : int
    {
        kOtgResultCategory_None = 0,
        kOtgResultCategory_NotSupported = 1,
        kOtgResultCategory_NotImplemented = 2,
        kOtgResultCategory_InvalidParam = 3,
        kOtgResultCategory_NotInitialized = 4,
        kOtgResultCategory_OperationSkipped = 5,
        kOtgResultCategory_OperationFailed = 6,
        kOtgResultCategory_ConnectionFailed = 7,
        kOtgResultCategory_NetworkFailed = 8,
        kOtgResultCategory_MemoryFailed = 9,
        kOtgResultCategory_Assertion = 0xA,
        kOtgResultCategory_Count = 0xB,
        kOtgResultCategory_ResultExtra = -1
    }

    #endregion

    #region SVO

    public enum SVOStatsState : int
    {
        kCompleted_0 = 1,
        kHostDisconnect_0 = 2,
        kLocalDisconnect_0 = 3,
        kLostConnection_0 = 4,
        kGamePlayError_0 = 5,
        kHostPlayerKicked_0 = 6,
        kLocalPlayerKicked_0 = 7
    }

    public enum SVOBinaryStatsStarhawk : int
    {
        kDeprecrated1 = 1,
        kPlayerDetailsData = 1,
        kPlayerDetailsPost = 2,
        KPlayerSummary = 2,
        kClanDetailsPost = 3,
        kTournamentAwardsSummary = 3,
        kTournamentSoloPost = 4,
        kVehiclesSummary = 4,
        kTournamentClanPost = 5,
        kBnBPartsSummary = 5,
        kWeaponDetailsPost = 6,
        kAwardsSummary = 6,
        kVehicleDetailsPost = 7,
        kTroopWeaponsSummary = 7,
        kBnBPartDetailsPost = 8,
        kStarhawkWeaponsSummary = 8,
        kAwardDetailsPost = 9,
        kBaseWeaponDetailsData = 0x64,
        kBaseVehicleDetailsData = 0x12C,
        kBaseBnBPartDetailsData = 0x1F4

    }

    #endregion
}