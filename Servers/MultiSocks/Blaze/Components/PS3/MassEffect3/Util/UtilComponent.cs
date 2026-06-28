using Blaze3SDK.Blaze;
using Blaze3SDK.Blaze.Util;
using Blaze3SDK.Components;
using BlazeCommon;
using CustomLogger;
using MultiSocks.Blaze.Model;
using MultiSocks.Utils;

namespace MultiSocks.Blaze.Components.PS3.MassEffect3.Util
{
    // FIXME.
    internal class UtilComponent : UtilComponentBase.Server
    {
        private static readonly byte[] skey =
        {
            0x5E,
            0x8A,
            0xCB,
            0xDD,
            0xF8,
            0xEC,
            0xC1,
            0x95,
            0x98,
            0x99,
            0xF9,
            0x94,
            0xC0,
            0xAD,
            0xEE,
            0xFC,
            0xCE,
            0xA4,
            0x87,
            0xDE,
            0x8A,
            0xA6,
            0xCE,
            0xDC,
            0xB0,
            0xEE,
            0xE8,
            0xE5,
            0xB3,
            0xF5,
            0xAD,
            0x9A,
            0xB2,
            0xE5,
            0xE4,
            0xB1,
            0x99,
            0x86,
            0xC7,
            0x8E,
            0x9B,
            0xB0,
            0xF4,
            0xC0,
            0x81,
            0xA3,
            0xA7,
            0x8D,
            0x9C,
            0xBA,
            0xC2,
            0x89,
            0xD3,
            0xC3,
            0xAC,
            0x98,
            0x96,
            0xA4,
            0xE0,
            0xC0,
            0x81,
            0x83,
            0x86,
            0x8C,
            0x98,
            0xB0,
            0xE0,
            0xCC,
            0x89,
            0x93,
            0xC6,
            0xCC,
            0x9A,
            0xE4,
            0xC8,
            0x99,
            0xE3,
            0x82,
            0xEE,
            0xD8,
            0x97,
            0xED,
            0xC2,
            0xCD,
            0x9B,
            0xD7,
            0xCC,
            0x99,
            0xB3,
            0xE5,
            0xC6,
            0xD1,
            0xEB,
            0xB2,
            0xA6,
            0x8B,
            0xB8,
            0xE3,
            0xD8,
            0xC4,
            0xA1,
            0x83,
            0xC6,
            0x8C,
            0x9C,
            0xB6,
            0xF0,
            0xD0,
            0xC1,
            0x93,
            0x87,
            0xCB,
            0xB2,
            0xEE,
            0x88,
            0x95,
            0xD2,
            0x80,
            0x80,
        };

        public override Task<PreAuthResponse> PreAuthAsync(
            PreAuthRequest request,
            BlazeRpcContext context
        )
        {
#if DEBUG
            LoggerAccessor.LogInfo($"[Blaze] - Util: Connection Id    : {context.Connection.ID}");
            LoggerAccessor.LogInfo($"[Blaze] - Util: Locale     : {request.mClientData.mLocale}");
            LoggerAccessor.LogInfo(
                $"[Blaze] - Util: Client Type      : {request.mClientData.mClientType}"
            );
            LoggerAccessor.LogInfo(
                $"[Blaze] - Util: Client Platform  : {request.mClientInfo.mPlatform}"
            );
            LoggerAccessor.LogInfo(
                $"[Blaze] - Util: Client mEnvironment  : {request.mClientInfo.mEnvironment}"
            );
#endif

            var fetchConfig = new SortedDictionary<string, string>
            {
                { "pingPeriod", "15s" },
                { "voipHeadsetUpdateRate", "1000" },
                { "xlspConnectionIdleTimeout", "300" },
            };

            var qosPingSiteInfoSJC = new QosPingSiteInfo()
            {
                mAddress = "gossjcprod-qos01.ea.com",
                mPort = 17502,
                mSiteName = "prod-sjc",
            };

            var qosPingSiteInfoIAD = new QosPingSiteInfo()
            {
                mAddress = "gosiadprod-qos01.ea.com",
                mPort = 17502,
                mSiteName = "bio-iad-prod-shared",
            };
            var qosPingSiteInfolhr = new QosPingSiteInfo()
            {
                mAddress = "gosgvaprod-qos01.ea.com",
                mPort = 17502,
                mSiteName = "bio-dub-prod-shared",
            };

            var pingSiteInfoByAliasNames = new SortedDictionary<string, QosPingSiteInfo>
            {
                { "ea-sjc", qosPingSiteInfoSJC },
                { "rs-iad", qosPingSiteInfoIAD },
                { "rs-lhr", qosPingSiteInfolhr },
            };

            return Task.FromResult(
                new PreAuthResponse()
                {
                    mAnonymousChildAccountsEnabled = false,
                    mAuthenticationSource = "300238",

                    //ushort list
                    mComponentIds = new List<ushort>()
                    {
                        1,
                        25,
                        4,
                        28,
                        7,
                        9,
                        63490,
                        30720,
                        15,
                        30721,
                        30722,
                        30723,
                        30725,
                        30726,
                        2000,
                    },
                    mConfig = new FetchConfigResponse() { mConfig = fetchConfig },
                    mInstanceName = "masseffect-3-ps3",
                    mLegalDocGameIdentifier = string.Empty,
                    mParentalConsentEntitlementGroupName = string.Empty,
                    mParentalConsentEntitlementTag = string.Empty,
                    mPersonaNamespace = "ps3",
                    mPlatform = "ps3",
                    mQosSettings = new()
                    {
                        mBandwidthPingSiteInfo = new QosPingSiteInfo()
                        {
                            mAddress = "gossjcprod-qos01.ea.com",
                            mPort = 17502,
                            mSiteName = "prod-sjc",
                        },
                        mNumLatencyProbes = 10,
                        mPingSiteInfoByAliasMap = pingSiteInfoByAliasNames,
                        mServiceId = 0x45410805,
                    },
                    mRegistrationSource = "300238",
                    mServerVersion = "Blaze 3.15.08.0 (CL# 1971992)",
                    mUnderageSupported = false,
                }
            );
        }

        public override Task<PostAuthResponse> PostAuthAsync(
            NullStruct request,
            BlazeRpcContext context
        )
        {
            var skeys = string.Empty;
            foreach (var b in skey)
                skeys += (char)b;

            var pssConfig = new PssConfig()
            {
                mAddress = "playersyncservices.ea.com",
                mInitialReportTypes = (PssReportTypes)0xB,
                // To find the signature, search for pathern: B9DDE13B in the eboot (not needed here).
                mNpCommSignature = Array.Empty<byte>(),
                mOfferIds = null,
                mPort = 443,
                mProjectId = "300238",
                mTitleId = 0,
            };

            var getTelemetryServerResponse = new GetTelemetryServerResponse()
            {
                mAddress = "gostelemetry.blaze3.ea.com",
                mDisable =
                    "AD,AF,AG,AI,AL,AM,AN,AO,AQ,AR,AS,AW,AX,AZ,BA,BB,BD,BF,BH,BI,BJ,BM,BN,BO,BR,BS,BT,BV,BW,BY,BZ,CC,CD,CF,CG,CI,CK,CL,CM,CN,CO,CR,CU,CV,CX,DJ,DM,DO,DZ,EC,EG,EH,ER,ET,FJ,FK,FM,FO,GA,GD,GE,GF,GG,GH,GI,GL,GM,GN,GP,GQ,GS,GT,GU,GW,GY,HM,HN,HT,ID,IL,IM,IN,IO,IQ,IR,IS,JE,JM,JO,KE,KG,KH,KI,KM,KN,KP,KR,KW,KY,KZ,LA,LB,LC,LI,LK,LR,LS,LY,MA,MC,MD,ME,MG,MH,ML,MM,MN,MO,MP,MQ,MR,MS,MU,MV,MW,MY,MZ,NA,NC,NE,NF,NG,NI,NP,NR,NU,OM,PA,PE,PF,PG,PH,PK,PM,PN,PS,PW,PY,QA,RE,RS,RW,SA,SB,SC,SD,SG,SH,SJ,SL,SM,SN,SO,SR,ST,SV,SY,SZ,TC,TD,TF,TG,TH,TJ,TK,TL,TM,TN,TO,TT,TV,TZ,UA,UG,UM,UY,UZ,VA,VC,VE,VG,VN,VU,WF,WS,YE,YT,ZM,ZW,ZZ",
                mFilter = "-UION/****",
                mIsAnonymous = false,
                mKey = skeys,
                mLocale = 0x66724652,
                mNoToggleOk = "US,CA,MX",
                mPort = 9988,
                mSendDelay = 15000,
                mSendPercentage = 75,
                mSessionID = BlazeServerUtils.GenerateTelemetrySessionId(),
                mUseServerTime = string.Empty,
            };

            var userId = BlazeUser.BlazeUserInfo.GetUser(context.BlazeConnection)?.UserID ?? -1;

            var getTickerServerResponse = new GetTickerServerResponse()
            {
                mAddress = "10.23.15.2",
                mKey = $"{userId},10.23.15.2:8999,masseffect-3-ps3,10,50,50,50,50,0,12",
                mPort = 8999,
            };

            var userOptions = new UserOptions()
            {
                mTelemetryOpt = TelemetryOpt.TELEMETRY_OPT_IN,
                mUserId = userId,
            };

            return Task.FromResult(
                new PostAuthResponse()
                {
                    mPssConfig = pssConfig,
                    mTelemetryServer = getTelemetryServerResponse,
                    mTickerServer = getTickerServerResponse,
                    mUserOptions = userOptions,
                }
            );
        }

        public override Task<FetchConfigResponse> FetchClientConfigAsync(
            FetchClientConfigRequest request,
            BlazeRpcContext context
        )
        {
#if DEBUG
            LoggerAccessor.LogInfo($"[Blaze] - Util: Connection Id    : {context.Connection.ID}");
            LoggerAccessor.LogInfo($"[Blaze] - Util: mConfigSection    : {request.mConfigSection}");
#endif
            var ME3ClientConfigPath =
                Directory.GetCurrentDirectory() + "/static/EA/PS3/ME3_CONFIG/";

            Directory.CreateDirectory(ME3ClientConfigPath);

            var fileME3PathFull = ME3ClientConfigPath + request.mConfigSection + ".txt";

            var fileClientConfigDictionary = new SortedDictionary<string, string>();

            switch (request.mConfigSection)
            {
                case "ME3_DATA":
                    if (File.Exists(fileME3PathFull))
                    {
                        var fileConfig = File.ReadAllLines(fileME3PathFull);

                        for (var i = 0; i < fileConfig.Length; i++)
                        {
                            var parts = fileConfig[i].Split(';');

                            fileClientConfigDictionary.Add(parts[0].Trim(), parts[1].Trim());
                        }
                    }
                    else
                    {
                        LoggerAccessor.LogWarn(
                            $"[Blaze] - Util: File not found! Path expected: {fileME3PathFull}"
                        );
                    }

                    break;
                case "ME3_MSG":
                    if (File.Exists(fileME3PathFull))
                    {
                        var fileConfig = File.ReadAllLines(fileME3PathFull);

                        for (var i = 0; i < fileConfig.Length; i++)
                        {
                            var parts = fileConfig[i].Split(';');

                            fileClientConfigDictionary.Add(parts[0].Trim(), parts[1].Trim());
                        }
                    }
                    else
                    {
                        LoggerAccessor.LogWarn(
                            $"[Blaze] - Util: File not found! Path expected: {fileME3PathFull}"
                        );
                    }

                    break;
                case "ME3_ENT":
                    if (File.Exists(fileME3PathFull))
                    {
                        var fileConfig = File.ReadAllLines(fileME3PathFull);

                        for (var i = 0; i < fileConfig.Length; i++)
                        {
                            var parts = !fileConfig[i].Trim().StartsWith("ENT_ENC")
                                ? fileConfig[i].Split(';')
                                : fileConfig[i].Split(':');
                            fileClientConfigDictionary.Add(parts[0].Trim(), parts[1].Trim());
                        }
                    }
                    else
                    {
                        LoggerAccessor.LogWarn(
                            $"[Blaze] - Util: File not found! Path expected: {fileME3PathFull}"
                        );
                    }

                    break;
                case "ME3_DIME":
                    if (File.Exists(fileME3PathFull))
                    {
                        var fileConfig = File.ReadAllText(fileME3PathFull);

                        fileClientConfigDictionary.Add("Config", fileConfig);
                    }
                    else
                    {
                        LoggerAccessor.LogWarn(
                            $"[Blaze] - Util: File not found! Path expected: {fileME3PathFull}"
                        );
                    }

                    break;
            }

            return Task.FromResult(
                new FetchConfigResponse() { mConfig = fileClientConfigDictionary }
            );
        }

        /// <summary>
        /// You only need to override the base method to handle new requests.
        /// If the request type or/and response type is NullStruct, you can change the request/response types in the Component Base.
        /// </summary>
        public override Task<PingResponse> PingAsync(NullStruct request, BlazeRpcContext context)
        {
#if DEBUG
            LoggerAccessor.LogInfo(
                $"[Blaze] - Util: Ping Connection Id    : {context.Connection.ID}"
            );
#endif
            return Task.FromResult(
                new PingResponse() { mServerTime = uint.Parse(DateTime.Now.ToString("yyyyMMdd")) }
            );
        }
    }
}
