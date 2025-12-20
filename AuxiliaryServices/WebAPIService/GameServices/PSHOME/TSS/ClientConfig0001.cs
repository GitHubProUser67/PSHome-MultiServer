using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace WebAPIService.GameServices.PSHOME.TSS
{
    public static class ClientConfig0001
    {
        public static string GenerateXML()
        {
            var serializer = new XmlSerializer(typeof(TSSBuilder));
            using StringWriter stringWriter = new StringWriter();
            serializer.Serialize(stringWriter, new TSSBuilder
            {
                Version = "12/20/2013 15:56:40 AM",

                SHA1Entries = new List<SHA1Entry>
                {
                    new() { File="Environments/SceneList.xml", Digest="015aa374b0ff6fc836f80c70d551951a05b126e7" },
                    new() { File="Objects/ObjectCatalogue.bar", Digest="3ef4a4d747acf4824ffcfd59a3fba8d9ddaa0877" },
                    new() { File="Objects/ObjectCatalogue_5_SCEAsia.hcdb", Digest="d7d19122ac30d7eee05b4f00c5caef7663def1d0" },
                    new() { File="Objects/ObjectCatalogue_5_SCEJ.hcdb", Digest="7b5fdb3fb239f4c9599d6a6dbdb353d5105c863a" },
                    new() { File="Objects/ObjectCatalogue_5_SCEA.hcdb", Digest="101303de4e7fbaf72827269abd142ad99d4a1991" },
                    new() { File="Objects/ObjectCatalogue_5_SCEE.hcdb", Digest="104b791b4fd16bd9bb384e22146fb6f9426bf99a" },
                    new() { File="Config/Config_en-GB.sharc", Digest="71688db5e6e3aa68d1b3f892dd8860158477f917" },
                    new() { File="Config/Config_de-DE.sharc", Digest="3357eed09c1205982424a3e19954896196cdc1d7" },
                    new() { File="Config/Config_fr-FR.sharc", Digest="0cc38a2edb20dab6091b9b6a499dd884c5cf04bd" },
                    new() { File="Config/Config_es-ES.sharc", Digest="5880c1cbfef1257f3ee2c588134e00f54e79b438" },
                    new() { File="Config/Config_it-IT.sharc", Digest="e5131faba1fadccf338e7db5469aaa948641eee7" },
                    new() { File="Config/Config_en-SG.sharc", Digest="1ba389bfe7bbab93f2c766b5bd18ba7f72d67812" },
                    new() { File="Config/Config_en-MY.sharc", Digest="023f3bee7fa68f0c4572145a91b8d937d7323c29" },
                    new() { File="Config/Config_en-ID.sharc", Digest="b4e3a36cf9faea85c1ce8e47cd41e53b5267195b" },
                    new() { File="Config/Config_en-TH.sharc", Digest="0f057dc6d945378f489451aed2e0cf5789bacd6c" },
                    new() { File="Config/Config_ko-KR.sharc", Digest="2ed1c78091e28b49a0c202478c8481be3d9c2818" },
                    new() { File="Config/Config_zh-HK.sharc", Digest="476d7ea6e92a53ad089691ec114b3a11141ae593" },
                    new() { File="Config/Config_zh-TW.sharc", Digest="d11551a1a6d4eeb2a2791e68a663cdba633534f3" },
                    new() { File="Config/Config_en-US.sharc", Digest="c530fc16ac8fb564af5cd32324074d7af8b83354" },
                    new() { File="Config/Config_ja-JP.sharc", Digest="2ea23fdd52a8a7712414672cb469355460e6f195" }
                },

                Objects = new ObjectsSection { PreparedDatabase = string.Empty },

                SecureContentRoot = "https://secure.$(env).homeps3.online.scee.com/",
                ScreenContentRoot = "https://secure.$(env).homeps3.online.scee.com/Screens/",
                SecureLuaObjectResourcesRoot = "https://secure.$(env).homeps3.online.scee.com/objects",

                DataCapture = new DataCapture
                {
                    Url = new UrlElement
                    {
                        Mode = 1,
                        Value = "https://hdc.$(env).homeps3.online.scee.com:10062/dataloaderweb/queue"
                    }
                },

                SceneRedirects = new List<SceneRedirect>
                {
                    new() { Dest="New_Home_Square_3_5305_3636", Region="SCEE", Src="Home Square" },
                    new() { Dest="2013_Cinema_193B_7E40", Region="SCEE", Src="Cinema" },
                    new() { Dest="2013_Marketplace_8480_98F0", Region="SCEE", Src="Marketplace" }
                },

                AdminObjectId = "BD824A08-34854866-91573614-3D2FF37C",

                DNSOverrides = new List<DNSOverride>
                {
                    new() { Action="error", Report="on", ClearCache="on", Value="209.141.32.0/19" },
                    new() { Action="error", Report="on", ClearCache="on", Value="192.95.35.251" }
                },

                UseRegionalServiceIds = string.Empty,
                MaxServiceIds = 65,
                Commerce = new Commerce { SecureCommercePoints = string.Empty },

                Connection = new Connection
                {
                    ContentServer = new ContentServer
                    {
                        Key = "8243a3b10f1f1660a7fc934aac263c9c5161092dc25=",
                        Value = "http://scee-home.playstation.net/c.home/prod/live/"
                    }
                },

                /*MessageQueue = new MessageQueue
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
