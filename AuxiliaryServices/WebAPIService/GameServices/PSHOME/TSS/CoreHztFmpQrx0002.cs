using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace WebAPIService.GameServices.PSHOME.TSS
{
    public static class CoreHztFmpQrx0002
    {
        public static string GenerateXML()
        {
            var serializer = new XmlSerializer(typeof(TSSBuilder));
            using StringWriter stringWriter = new StringWriter();
            serializer.Serialize(stringWriter, new TSSBuilder
            {
                Version = "12/16/2014 10:07:28 AM",

                SHA1Entries = new List<SHA1Entry>
                {
                    new() { File="Environments/SceneList.xml", Digest="E364FC839B0165292A5C08FB97104293F80FAF84" },
                    new() { File="Objects/ObjectCatalogue.bar", Digest="2FDF52572BB639D7B22E42D9C451EA2E6C86F544" },
                    new() { File="Objects/ObjectCatalogue_5_SCEAsia.hcdb", Digest="d248108d7e979784a1ead39a623d70a25b5bdd31" },
                    new() { File="Objects/ObjectCatalogue_5_SCEJ.hcdb", Digest="cc68693502af6ce9afd5f3c47349093f6ab7f28d" },
                    new() { File="Objects/ObjectCatalogue_5_SCEA.hcdb", Digest="70b4b6bf2f6f11694ed188eaae7d03e9c0973fc4" },
                    new() { File="Objects/ObjectCatalogue_5_SCEE.hcdb", Digest="4229ce0699151be662c6cdf1158107991f9e9e0a" },
                    new() { File="Config/Config_en-GB.sharc", Digest="c9092f482ed2736e1f8c6a0881210745dd63bea7" },
                    new() { File="Config/Config_de-DE.sharc", Digest="c26d13198c946b92547553102b88ec1cdeeadb5a" },
                    new() { File="Config/Config_fr-FR.sharc", Digest="989d3165f4b9906db9ffe6485166d2149b1c3fe7" },
                    new() { File="Config/Config_es-ES.sharc", Digest="7cfd7f0747b25f8dc5b03b88ff42dec1dbd0ccda" },
                    new() { File="Config/Config_it-IT.sharc", Digest="2d9526d70da1faa9b836c0b442da678922c3bd69" },
                    new() { File="Config/Config_en-SG.sharc", Digest="4eb6b7d8c3a6bfe4c8fcef7fc508d08399d7d530" },
                    new() { File="Config/Config_en-MY.sharc", Digest="274c5d681391552d844aa0f049c8005537c49fa5" },
                    new() { File="Config/Config_en-ID.sharc", Digest="782af70110091625eaa0209c64e320a3ebbe9ae8" },
                    new() { File="Config/Config_en-TH.sharc", Digest="c2c6bb9ef43fc2b6a08f8e547a9e228640923faf" },
                    new() { File="Config/Config_ko-KR.sharc", Digest="595eb545b95576518c147eaaf09e0947dbade44d" },
                    new() { File="Config/Config_zh-HK.sharc", Digest="4b420af145497bcc0f472714477bbd7452692eba" },
                    new() { File="Config/Config_zh-TW.sharc", Digest="1414b53d5a11e21bf44fee59f678842dc716873e" },
                    new() { File="Config/Config_en-US.sharc", Digest="4b441cb67486b4cfc56b4dcce3dbc3fdab7920c7" },
                    new() { File="Config/Config_ja-JP.sharc", Digest="b17ff42b3ec56914f5ebc861339e025a1de0b7f4" }
                },

                Objects = new ObjectsSection { PreparedDatabase = string.Empty },

                SecureContentRoot = "https://secure.$(env).homeps3.online.scee.com/",
                ScreenContentRoot = "https://secure.$(env).homeps3.online.scee.com/Screens/",
                SecureLuaObjectResourcesRoot = "https://secure.$(env).homeps3.online.scee.com/objects",

                ProfanityFilter = new ProfanityFilter
                {
                    ApiKey = "6b77c0b1-4636-4942-b08c-c4ee126b82ae",
                    ForceOffline = false,
                    PrivateKey = "NVluu9dWima10JIUKhCVvg==",
                    UpdaterOverrideUrl = "update-prod.pfs.online.scee.com"
                },

                SceneRedirects = new List<SceneRedirect>
                {
                    new() { Src="Home Square", Dest="Xmas_Home_Square_14_FFA7_18E9", Region="SCEE" },
                    new() { Src="Cinema", Dest="2013_Cinema_193B_7E40", Region="SCEE" },
                    new() { Src="Marketplace", Dest="2013_Marketplace_8480_98F0", Region="SCEE" },
                    new() { Src="Game Space", Dest="Game_Space_SCEE_5ACB_5A24", Region="SCEE" },
                    new() { Src="Game Space", Dest="SCEA_Game_Space_D9CA_5D52", Region="SCEA" }
                },

                AdminObjectId = "D16117C1-EA554B2C-A5CCE297-C97E617D",

                DNSOverrides = new List<DNSOverride>
                {
                    new() { Action="allow", Report="on", Value="0.0.0.0/0" },
                    new() { Action="error", Report="on", ClearCache="on", Value="199.19.224.0/22" },
                    new() { Action="error", Report="on", ClearCache="on", Value="205.185.112.0/20" },
                    new() { Action="error", Report="on", ClearCache="on", Value="209.141.32.0/19" },
                    new() { Action="error", Report="on", ClearCache="on", Value="199.195.248.0/21" },
                    new() { Action="error", Report="on", ClearCache="on", Value="198.98.48.0/20" },
                    new() { Action="error", Report="on", ClearCache="on", Value="198.251.80.0/20" },
                    new() { Action="error", Report="on", ClearCache="on", Value="192.95.0.0/18" }
                },

                UseRegionalServiceIds = string.Empty,
                MaxServiceIds = 65,
                Commerce = new Commerce { SecureCommercePoints = string.Empty },

                Connection = new Connection
                {
                    ContentServer = new ContentServer
                    {
                        Key = "8b9qT7u6XQ7Sf0GKSIivMEeG7NROLTZGgNtN8iI6n1Y=",
                        Value = "http://scee-home.playstation.net/c.home/prod2/live2/"
                    }
                },

                /*DisableBar = string.Empty,

                MessageQueue = new MessageQueue
                {
                    Connect = new Connect
                    {
                        Address = "prod.homemq.online.scee.com",
                        Port = 10086,
                        Login = "cprod",
                        Password = "ummagumma",
                        VHost = "prod",
                        IsCritical = false
                    },
                    Clients = new List<string>
                    {
                        "/exchange/exchange.client/pshome.client.$(user)",
                        "/exchange/exchange.client/pshome.client.command"
                    },
                        Subscribe = "/exchange/exchange.platform/pshome.platform.$(user)",
                        Posts = new List<string>
                    {
                        "/exchange/exchange.event/pshome.client.event.#",
                        "/exchange/exchange.score/pshome.content.score.#",
                        "/exchange/exchange.message/pshome.content.message.#"
                    },
                    Events = new EventsSection
                    {
                        Enabled = true,
                        Destination = new Destination
                        {
                            Default = "/exchange/exchange.event/pshome.client.event"
                        }
                    },
                    Content = new ContentSection
                    {
                        Message = "/exchange/exchange.message/pshome.content.message",
                        Score = "/exchange/exchange.score/pshome.content.score"
                    }
                },*/

                Ssfw = new SSFW
                {
                    Identity = new Identity
                    {
                        Ttl = 60,
                        Secret = "0123456789abcdef",
                        Url = "https://cprod.homeidentity.online.scee.com:10443/bb88aea9-6bf8-4201-a6ff-5d1f8da0dd37/"
                    },
                    Rewards = "https://cprod.homeserverservices.online.scee.com:10443/RewardsService/cprod/",
                    Clan = "https://cprod.homeserverservices.online.scee.com:10443/ClanService/cprod/clan/",
                    SaveData = "https://cprod.homeserverservices.online.scee.com:10443/SaveDataService/cprod/",
                    Avatar = "https://cprod.homeserverservices.online.scee.com:10443/SaveDataService/avatar/cprod/",
                    Layout = "https://cprod.homeserverservices.online.scee.com:10443/LayoutService/cprod/",
                    Trunks = "https://cprod.homeserverservices.online.scee.com:10443/RewardsService/trunks-cprod/trunks/",
                    AvatarLayout = "https://cprod.homeserverservices.online.scee.com:10443/AvatarLayoutService/cprod/",
                    Structured = "https://cprod.homeserverservices.online.scee.com:10443/SaveDataService/cprod/"
                },

                Global = new GlobalSection
                {
                    Modes = new List<Mode>
                    {
                        new() { SCEA=3, SCEJ=3, SCEE=3, SCEAsia=3, Value=0 },
                        new() { SCEA=1, SCEJ=1, SCEE=0, SCEAsia=1, Value=1 },
                        new() { SCEA=1, SCEJ=1, SCEE=1, SCEAsia=1, Value=7 },
                        new() { SCEA=2, SCEJ=2, SCEE=2, SCEAsia=2, Value=21 },
                        new() { SCEA=1, SCEJ=1, SCEE=1, SCEAsia=1, Value=12 },
                        new() { SCEA=1, SCEJ=1, SCEE=1, SCEAsia=1, Value=13 },
                        new() { SCEA=1, SCEJ=1, SCEE=1, SCEAsia=1, Value=14 },
                        new() { SCEA=1, SCEJ=1, SCEE=1, SCEAsia=1, Value=15 },
                        new() { SCEA=1, SCEJ=1, SCEE=1, SCEAsia=1, Value=16 },
                        new() { SCEA=1, SCEJ=1, SCEE=1, SCEAsia=1, Value=17 },
                        new() { SCEA=1, SCEJ=1, SCEE=1, SCEAsia=1, Value=18 },
                        new() { SCEA=1, SCEJ=1, SCEE=1, SCEAsia=1, Value=19 },
                        new() { SCEA=1, SCEJ=1, SCEE=1, SCEAsia=1, Value=20 }
                    }
                },

                AgeRestrictions = new AgeRestrictions
                {
                    Ages = new List<Age>
                    {
                        new() { Region="sceasia", Value=0 },
                        new() { Region="scej", Value=0 },
                        new() { Region="scee", Value=16 },
                        new() { Region="scea", Value=13 },
                        new() { Region="scek", Value=0 }
                    }
                },

                RegionInfo = RegionInfoFactory.Create()
            });
            return stringWriter.ToString();
        }
    }
}
