namespace Horizon.RT.Common
{
    public static class Constants
    {
        public const int MESSAGEID_MAXLEN = 21;
        public const int SESSIONKEY_MAXLEN = 17;

        #region Account 
        public const int ACCOUNTNAME_MAXLEN = 32;
        /// <summary>
        /// The account stats field contains up to this many bytes of binary data.
        /// </summary>
        public const int ACCOUNTSTATS_MAXLEN = 256;
        #endregion

        /// <summary>
        /// PS3
        /// </summary>
        public const int ALIASNAME_MAXLEN = 32;

        #region Clans
        /// <summary>
        /// Maximum number of bytes in a clan name, including null termination.
        /// </summary>
        public const int CLANNAME_MAXLEN = 32;
        /// <summary>
        /// Maximum number of bytes in a clan stats field., This is a fixed length field, and is binary.
        /// There are no default values for this field. Please set the clan stats when creating the clan.
        /// </summary>
        public const int CLANSTATS_MAXLEN = 256;
        public const int CLANMSG_MAXLEN = 200;
        public const int CLANMSG_MAXLEN_113 = 2048;
        public const int CLANMSG_MAXLEN_113_2 = 1200;
        #endregion

        /// <summary>
        /// Used for MediusPostDebugInfo.
        /// Maximum number of bytes in a MediusPostDebugInfoRequest, including null termination.
        /// </summary>
        public const int DEBUGMESSAGE_MAXLEN = 200;

        /// <summary>
        /// Maximum number of bytes in a MediusErrorMessage from the server to client, including null termination.
        /// </summary>
        public const int ERRORMSG_MAXLEN = 256;
        public const int PASSWORD_MAXLEN = 32;
        public const int WORLDNAME_MAXLEN = 64;
        public const int WORLDPASSWORD_MAXLEN = 32;
        public const int LOBBYNAME_MAXLEN = 64;
        public const int LOBBYPASSWORD_MAXLEN = WORLDPASSWORD_MAXLEN;
        public const int LOCATIONNAME_MAXLEN = 64;

        #region Game
        /// <summary>
        /// Maximum number of bytes in a game name, including null termination.
        /// </summary>
        public const int GAMENAME_MAXLEN = 64;
        /// <summary>
        /// Maximum number of bytes in a game password, including null termination.
        /// </summary>
        public const int GAMEPASSWORD_MAXLEN = 32;
        /// <summary>
        /// Maximum number of bytes for the game stats, The is a binary field of fixed length, and no default value.
        /// </summary>
        public const int GAMESTATS_MAXLEN = 256;
        /// <summary>
        /// Maximum number of bytes in a spectator password, including null termination.
        /// </summary>
        public const int SPECTATORPASSWORD_MAXLEN = 32;
        #endregion

        public const int WINNINGTEAM_MAXLEN = 64;

        /// <summary>
        /// Maximum number of bytes in a MACHINE signature post. All binary data. (DEPRECATED)
        /// </summary>
        public const int MACHINESIGNATURE_MAXLEN = 128;
        /// <summary>
        /// Maximum number of bytes in a DNAS signature post. All binary data.
        /// </summary>
        public const int DNASSIGNATURE_MAXLEN = 32;
        /// <summary>
        /// 
        /// </summary>
        public const int VERSIONSERVER_MAXLEN = 56;
        /// <summary>
        /// The maximum number of bytes in a single announcement text chunk, as returned by the server, including
        /// null termination.
        /// </summary>
        public const int ANNOUNCEMENT_MAXLEN = 1000;
        /// <summary>
        /// The maximum number of bytes in a single announcement text chunk, as returned by the server, including
        /// null termination. PS3 Clients only
        /// </summary>
        public const int ANNOUNCEMENT1_MAXLEN = 4000;
        public const int MEDIUS_GENERIC_CHAT_FILTER_BYTES_LEN = 16;
        public const int MEDIUS_MESSAGE_MAXLEN = 512;
        public const int MEDIUS_UDP_MESSAGE_MAXLEN = 584;
        public const int NEWS_MAXLEN = 256;
        public const int POLICY_MAXLEN = 256;
        public const int PLAYERNAME_MAXLEN = 32;
        /// <summary>
        /// The maximum number of bytes that an application can use to describe itself, including null termination.
        /// </summary>
        public const int APPNAME_MAXLEN = 32;
        /// <summary>
        /// Maximum number of bytes in a chat message, including null termination.
        /// </summary>
        public const int CHATMESSAGE_MAXLEN = 64;
        public const int BINARYMESSAGE_MAXLEN = 400;
        public const int IP_MAXLEN = 20;
        public const int MEDIUS_TOKEN_MAXSIZE = 8;

        public const int UNIVERSENAME_MAXLEN = 128;
        public const int UNIVERSEDNS_MAXLEN = 128;
        public const int UNIVERSEDESCRIPTION_MAXLEN = 256;
        public const int UNIVERSE_BSP_MAXLEN = 8;
        public const int UNIVERSE_BSP_NAME_MAXLEN = 128;
        public const int UNIVERSE_EXTENDED_INFO_MAXLEN = 128;
        public const int UNIVERSE_SVO_URL_MAXLEN = 128;

        public const int LADDERSTATSWIDE_MAXLEN = 100;

        public const int NET_SESSION_KEY_LEN = 17;
        /// <summary>
        /// Maximum number of bytes in the access key field
        /// </summary>
        public const int NET_ACCESS_KEY_LEN = 17;
        public const int NET_MAX_NETADDRESS_LENGTH = 16;
        public const int NET_ADDRESS_LIST_COUNT = 2;

        public const int RSA_SIZE_DWORD = 16;

        public const int MGCL_MESSAGEID_MAXLEN = 21;
        public const int MGCL_SERVERVERSION_MAXLEN = 16;
        public const int MGCL_SERVERVERSION_MAXLEN1 = 8;
        public const int MGCL_GAMENAME_MAXLEN = 64;
        public const int MGCL_GAMESTATS_MAXLEN = 256;
        public const int MGCL_GAMEPASSWORD_MAXLEN = 32;
        public const int MGCL_SERVERIP_MAXLEN = 20;
        public const int MGCL_ACCESSKEY_MAXLEN = 17;
        public const int MGCL_SESSIONKEY_MAXLEN = 17;
        //PS3
        public const int MGCL_GAMENAME_MAXLEN1 = 15;
        public const int MGCL_GAMEPASSWORD_MAXLEN1 = 15;

        public const int SUPERSETNAME_MAXLEN = 50;
        public const int SUPERSETDESCRIPTION_MAXLEN = 256;
        public const int SUPERSETEXTRAINFO_MAXLEN = 256;

        public const int REQUESTDATA_MAXLEN = 16;
        public const int APPLICATIONDATA_MAXLEN = 15;

        public const int PARTYNAME_MAXLEN = 64;
        public const int PARTYPASSWORD_MAXLEN = 32;

        // Resistance 2
        public const int R2GAMENAME_MAXLEN = 25;

        //Medius File Services
        public const int MEDIUS_FILE_MAX_DOWNLOAD_DATA_SIZE = 464;
        public const int MEDIUS_FILE_MAX_FILENAME_LENGTH = 128;
        public const int MEDIUS_FILE_CHECKSUM_NUMBYTES = 16;
        //public const int MEDIUS_FILE_CHECKSUM_NUMBYTES = 48;
        public const int MEDIUS_FILE_MAX_DESCRIPTION_LENGTH = 256;
        public const int MEDIUS_FILE_MAX_KEY_LENGTH = 56;
        public const int MEDIUS_FILE_MAX_VALUE_LENGTH = 256;

        public const int DME_FRAGMENT_MAX_PAYLOAD_SIZE = 484;
        public const int DME_NAME_LENGTH = 24;
        public const int DME_VERSION_LENGTH = 20;

        public const int BUFFER_SIZE = 1500;

        #region Deprecated

        /// <summary>
        /// Length of array storing stats to be used for calculating stats.
        /// </summary>
        public const int LADDERSTATS_MAXLEN = 15;

        /// <summary>
        /// Maximum number of bytes used to denote the path to the medius.ico file location on the memory card.
        /// Deprecated.
        /// </summary>
        public const int ICONLOCATION_MAXLEN = 64;
        #endregion

        #region OTG

        public const int SYSTEMID_MAXLEN = 20;

        #endregion
    }
}