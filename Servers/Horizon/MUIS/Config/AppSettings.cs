namespace Horizon.MUIS.Config
{
    public class AppSettings
    {
        /// <summary>
        /// This settings respective app id.
        /// </summary>
        public int AppId { get; }

        /// <summary>
        /// When true, logging into a non-existent account will create the account and log the user in.
        /// Required for PAL support.
        /// </summary>
        public bool CreateAccountOnNotFound { get; private set; } = true;

        /// <summary>
        /// When true, attempts to create new accounts will be rejected.
        /// Useful for maintenance or beta testing mode.
        /// </summary>
        public bool DisableAccountCreation { get; private set; } = false;

        /// <summary>
        /// When true, only account ids in AccountIdWhitelist will be allowed to login.
        /// </summary>
        public bool EnableAccountWhitelist { get; private set; } = false;

        /// <summary>
        /// List of account ids that are allowed to login. When EnableAccountWhitelist is set to true.
        /// </summary>
        public int[] AccountIdWhitelist { get; private set; } = Array.Empty<int>();

        /// <summary>
        /// Regular expression whitelist filter for most text inputs. If you only want to accept numbers you'd enter (\d)+ for example.
        /// </summary>
        public string TextFilterDefault { get; private set; } = string.Empty;

        /// <summary>
        /// Regular expression whitelist filter for account names. If you only want to accept numbers you'd enter (\d)+ for example.
        /// </summary>
        public string TextFilterAccountName { get; private set; } = string.Empty;

        /// <summary>
        /// Regular expression whitelist filter for clan names. If you only want to accept numbers you'd enter (\d)+ for example.
        /// </summary>
        public string TextFilterClanName { get; private set; } = string.Empty;

        /// <summary>
        /// Regular expression whitelist filter for the clan message. If you only want to accept numbers you'd enter (\d)+ for example.
        /// </summary>
        public string TextFilterClanMessage { get; private set; } = string.Empty;

        /// <summary>
        /// Regular expression whitelist filter for chat messages. If you only want to accept numbers you'd enter (\d)+ for example.
        /// </summary>
        public string TextFilterChat { get; private set; } = string.Empty;

        /// <summary>
        /// Regular expression whitelist filter for game names. If you only want to accept numbers you'd enter (\d)+ for example.
        /// </summary>
        public string TextFilterGameName { get; private set; } = string.Empty;

        /// <summary>
        /// When true, server will encrypt all messages.
        /// </summary>
        public bool EnableEncryption { get; private set; } = true;

        /// <summary>
        /// When true, will enable medius file services and will allow messages like MediusCreateFile, MediusUploadFile, MediusDownloadFile, MediusFileListFiles
        /// </summary>
        public bool EnableMediusFileServices { get; private set; } = true;

        /// <summary>
        /// 'Severity' of the system message sent to notify the user has been banned.
        ///  This is game specific.
        /// </summary>
        public int BanSystemMessageSeverity { get; private set; } = 0;

        /// <summary>
        /// Number of seconds before the server should send an echo to the client.
        /// </summary>
        public int ServerEchoIntervalSeconds { get; private set; } = 5;

        /// <summary>
        /// Period of time when a client is moving between medius server components where the client object will be kept alive.
        /// </summary>
        public int KeepAliveGracePeriodSeconds { get; private set; } = 8;

        /// <summary>
        /// Time since last echo before timing the client out.
        /// </summary>
        public int ClientTimeoutSeconds { get; private set; } = 25;

        /// <summary>
        /// Time since last echo before timing the client out.
        /// </summary>
        public int ClientLongTimeoutSeconds { get; private set; } = 60 * 5;

        /// <summary>
        /// Time since game created and host never connected to close the game world.
        /// </summary>
        public int GameTimeoutSeconds { get; private set; } = 30;

        /// <summary>
        /// Time, in seconds, before timing out a Dme server.
        /// </summary>
        public int DmeTimeoutSeconds { get; private set; } = 30;

        public AppSettings(int appId)
        {
            AppId = appId;
        }

        public void SetSettings(Dictionary<string, string> settings)
        {
            string? value = null;

            // CreateAccountOnNotFound
            if (settings.TryGetValue("CreateAccountOnNotFound", out value) && bool.TryParse(value, out var createAccountOnNotFound))
                CreateAccountOnNotFound = createAccountOnNotFound;
            // DisableAccountCreation
            if (settings.TryGetValue("DisableAccountCreation", out value) && bool.TryParse(value, out var disableAccountCreation))
                DisableAccountCreation = disableAccountCreation;
            // EnableAccountWhitelist
            if (settings.TryGetValue("EnableAccountWhitelist", out value) && bool.TryParse(value, out var enableAccountWhitelist))
                EnableAccountWhitelist = enableAccountWhitelist;
            // AccountIdWhitelist
            if (settings.TryGetValue("AccountIdWhitelist", out value))
                AccountIdWhitelist = value.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(x => int.Parse(x)).ToArray();

            // TextFilterDefault
            if (settings.TryGetValue("TextFilterDefault", out value))
                TextFilterDefault = value;
            // TextFilterAccountName
            if (settings.TryGetValue("TextFilterAccountName", out value))
                TextFilterAccountName = value;
            // TextFilterClanName
            if (settings.TryGetValue("TextFilterClanName", out value))
                TextFilterClanName = value;
            // TextFilterClanMessage
            if (settings.TryGetValue("TextFilterClanMessage", out value))
                TextFilterClanMessage = value;
            // TextFilterChat
            if (settings.TryGetValue("TextFilterChat", out value))
                TextFilterChat = value;
            // TextFilterGameName
            if (settings.TryGetValue("TextFilterGameName", out value))
                TextFilterGameName = value;

            // EnableEncryption
            if (settings.TryGetValue("EnableEncryption", out value) && bool.TryParse(value, out var enableEncryption))
                EnableEncryption = enableEncryption;
            // EnableMediusFileServices
            if (settings.TryGetValue("EnableMediusFileServices", out value) && bool.TryParse(value, out var enableMediusFileServices))
                EnableMediusFileServices = enableMediusFileServices;
            // BanSystemMessageSeverity
            if (settings.TryGetValue("BanSystemMessageSeverity", out value) && int.TryParse(value, out var banSystemMessageSeverity))
                BanSystemMessageSeverity = banSystemMessageSeverity;
            // ServerEchoIntervalSeconds
            if (settings.TryGetValue("ServerEchoIntervalSeconds", out value) && int.TryParse(value, out var serverEchoIntervalSeconds))
                ServerEchoIntervalSeconds = serverEchoIntervalSeconds;
            // KeepAliveGracePeriodSeconds
            if (settings.TryGetValue("KeepAliveGracePeriodSeconds", out value) && int.TryParse(value, out var keepAliveGracePeriodSeconds))
                KeepAliveGracePeriodSeconds = keepAliveGracePeriodSeconds;
            // ClientTimeoutSeconds
            if (settings.TryGetValue("ClientTimeoutSeconds", out value) && int.TryParse(value, out var clientTimeoutSeconds))
                ClientTimeoutSeconds = clientTimeoutSeconds;
            // GameTimeoutSeconds
            if (settings.TryGetValue("GameTimeoutSeconds", out value) && int.TryParse(value, out var gameTimeoutSeconds))
                GameTimeoutSeconds = gameTimeoutSeconds;
            // DmeTimeoutSeconds
            if (settings.TryGetValue("DmeTimeoutSeconds", out value) && int.TryParse(value, out var dmeTimeoutSeconds))
                DmeTimeoutSeconds = dmeTimeoutSeconds;
        }

        public Dictionary<string, string> GetSettings()
        {
            return new Dictionary<string, string>()
            {
                { "CreateAccountOnNotFound", CreateAccountOnNotFound.ToString() },
                { "DisableAccountCreation", DisableAccountCreation.ToString() },
                { "EnableAccountWhitelist", EnableAccountWhitelist.ToString() },
                { "AccountIdWhitelist", string.Join(",", AccountIdWhitelist) },

                { "TextFilterDefault", TextFilterDefault },
                { "TextFilterAccountName", TextFilterAccountName },
                { "TextFilterClanName", TextFilterClanName },
                { "TextFilterClanMessage", TextFilterClanMessage },
                { "TextFilterChat", TextFilterChat },
                { "TextFilterGameName", TextFilterGameName },

                { "EnableEncryption", EnableEncryption.ToString() },
                { "EnableMediusFileServices", EnableMediusFileServices.ToString() },
                { "BanSystemMessageSeverity", BanSystemMessageSeverity.ToString() },
                { "ServerEchoIntervalSeconds", ServerEchoIntervalSeconds.ToString() },
                { "KeepAliveGracePeriodSeconds", KeepAliveGracePeriodSeconds.ToString() },
                { "ClientTimeoutSeconds", ClientTimeoutSeconds.ToString() },
                { "GameTimeoutSeconds", GameTimeoutSeconds.ToString() },
                { "DmeTimeoutSeconds", DmeTimeoutSeconds.ToString() },
            };
        }
    }
}