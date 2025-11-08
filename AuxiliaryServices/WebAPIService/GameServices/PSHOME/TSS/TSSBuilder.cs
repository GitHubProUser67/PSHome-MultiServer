using System.Collections.Generic;
using System.Xml.Serialization;

namespace WebAPIService.GameServices.PSHOME.TSS
{

    [XmlRoot("XML")]
    public class TSSBuilder
    {
        [XmlElement("VERSION")]
        public string Version { get; set; }

        [XmlElement("SHA1")]
        public List<SHA1Entry> SHA1Entries { get; set; }

        [XmlElement("objects")]
        public ObjectsSection Objects { get; set; }

        [XmlElement("SecureContentRoot")]
        public string SecureContentRoot { get; set; }

        [XmlElement("ScreenContentRoot")]
        public string ScreenContentRoot { get; set; }

        [XmlElement("secure_lua_object_resources_root")]
        public string SecureLuaObjectResourcesRoot { get; set; }

        [XmlElement("profanityfilter")]
        public ProfanityFilter ProfanityFilter { get; set; }

        [XmlElement("datacapture")]
        public DataCapture DataCapture { get; set; }

        [XmlElement("SceneRedirect")]
        public List<SceneRedirect> SceneRedirects { get; set; }

        [XmlElement("AdminObjectId")]
        public string AdminObjectId { get; set; }

        [XmlElement("DNSOverride")]
        public List<DNSOverride> DNSOverrides { get; set; }

        [XmlElement("useregionalserviceids")]
        public string UseRegionalServiceIds { get; set; }

        [XmlElement("maxserviceids")]
        public int MaxServiceIds { get; set; }

        [XmlElement("commerce")]
        public Commerce Commerce { get; set; }

        [XmlElement("connection")]
        public Connection Connection { get; set; }

        [XmlElement("disablebar")]
        public string DisableBar { get; set; }

        [XmlElement("messageQueue")]
        public MessageQueue MessageQueue { get; set; }

        [XmlElement("ssfw")]
        public SSFW Ssfw { get; set; }

        [XmlElement("global")]
        public GlobalSection Global { get; set; }

        [XmlElement("agerestrictions")]
        public AgeRestrictions AgeRestrictions { get; set; }

        [XmlElement("REGIONINFO")]
        public RegionInfo RegionInfo { get; set; }
    }

    public class SHA1Entry
    {
        [XmlAttribute("file")]
        public string File { get; set; }

        [XmlAttribute("digest")]
        public string Digest { get; set; }
    }

    public class ObjectsSection
    {
        [XmlElement("prepared_database")]
        public string PreparedDatabase { get; set; }
    }

    public class ProfanityFilter
    {
        [XmlAttribute("apiKey")]
        public string ApiKey { get; set; }

        [XmlAttribute("forceOffline")]
        public bool ForceOffline { get; set; }

        [XmlAttribute("privateKey")]
        public string PrivateKey { get; set; }

        [XmlAttribute("updaterOverrideUrl")]
        public string UpdaterOverrideUrl { get; set; }
    }


    public class DataCapture
    {
        [XmlElement("url")]
        public UrlElement Url { get; set; }
    }

    public class UrlElement
    {
        [XmlAttribute("mode")]
        public int Mode { get; set; }

        [XmlText]
        public string Value { get; set; }
    }

    public class SceneRedirect
    {
        [XmlAttribute("dest")]
        public string Dest { get; set; }

        [XmlAttribute("region")]
        public string Region { get; set; }

        [XmlAttribute("src")]
        public string Src { get; set; }
    }

    public class DNSOverride
    {
        [XmlAttribute("action")]
        public string Action { get; set; }

        [XmlAttribute("report")]
        public string Report { get; set; }

        [XmlAttribute("clearcache")]
        public string ClearCache { get; set; }

        [XmlText]
        public string Value { get; set; }
    }

    public class Commerce
    {
        [XmlElement("secure_commerce_points")]
        public string SecureCommercePoints { get; set; }
    }

    public class Connection
    {
        [XmlElement("contentserver")]
        public ContentServer ContentServer { get; set; }
    }

    public class ContentServer
    {
        [XmlAttribute("key")]
        public string Key { get; set; }

        [XmlText]
        public string Value { get; set; }
    }

    public class MessageQueue
    {
        [XmlElement("connect")]
        public Connect Connect { get; set; }

        [XmlElement("client")]
        public List<string> Clients { get; set; }

        [XmlElement("subscribe")]
        public string Subscribe { get; set; }

        [XmlElement("post")]
        public List<string> Posts { get; set; }

        [XmlElement("events")]
        public EventsSection Events { get; set; }

        [XmlElement("content")]
        public ContentSection Content { get; set; }
    }

    public class Connect
    {
        [XmlAttribute("address")] public string Address { get; set; }
        [XmlAttribute("port")] public int Port { get; set; }
        [XmlAttribute("login")] public string Login { get; set; }
        [XmlAttribute("password")] public string Password { get; set; }
        [XmlAttribute("vhost")] public string VHost { get; set; }
        [XmlAttribute("isCritical")] public bool IsCritical { get; set; }
    }

    public class EventsSection
    {
        [XmlElement("enabled")] public bool Enabled { get; set; }

        [XmlElement("destination")]
        public Destination Destination { get; set; }
    }

    public class Destination
    {
        [XmlElement("default")]
        public string Default { get; set; }
    }

    public class ContentSection
    {
        [XmlElement("message")] public string Message { get; set; }
        [XmlElement("score")] public string Score { get; set; }
    }

    public class SSFW
    {
        [XmlElement("identity")] public Identity Identity { get; set; }
        [XmlElement("rewards")] public string Rewards { get; set; }
        [XmlElement("clan")] public string Clan { get; set; }
        [XmlElement("savedata")] public string SaveData { get; set; }
        [XmlElement("avatar")] public string Avatar { get; set; }
        [XmlElement("layout")] public string Layout { get; set; }
        [XmlElement("trunks")] public string Trunks { get; set; }
        [XmlElement("avatarlayout")] public string AvatarLayout { get; set; }
        [XmlElement("structured")] public string Structured { get; set; }
    }

    public class Identity
    {
        [XmlAttribute("ttl")] public int Ttl { get; set; }
        [XmlAttribute("secret")] public string Secret { get; set; }
        [XmlText] public string Url { get; set; }
    }

    public class GlobalSection
    {
        [XmlElement("mode")]
        public List<Mode> Modes { get; set; }
    }

    public class Mode
    {
        [XmlAttribute("SCEA")] public int SCEA { get; set; }
        [XmlAttribute("SCEJ")] public int SCEJ { get; set; }
        [XmlAttribute("SCEE")] public int SCEE { get; set; }
        [XmlAttribute("SCEAsia")] public int SCEAsia { get; set; }
        [XmlText] public int Value { get; set; }
    }

    public class AgeRestrictions
    {
        [XmlElement("age")]
        public List<Age> Ages { get; set; }
    }

    public class Age
    {
        [XmlAttribute("region")]
        public string Region { get; set; }

        [XmlText]
        public int Value { get; set; }
    }

    public class RegionInfo
    {
        [XmlElement("INSTANCE_TYPES")]
        public InstanceTypes InstanceTypes { get; set; }

        [XmlElement("REGION_TYPES")]
        public RegionTypes RegionTypes { get; set; }

        [XmlElement("REGION_MAP")]
        public RegionMap RegionMap { get; set; }

        [XmlElement("LOCALISATIONS")]
        public Localisations Localisations { get; set; }
    }

    public class InstanceTypes
    {
        [XmlElement("TYPE")]
        public List<NamedType> Types { get; set; }
    }

    public class RegionTypes
    {
        [XmlElement("TYPE")]
        public List<RegionType> Types { get; set; }
    }

    public class RegionMap
    {
        [XmlElement("MAP")]
        public List<MapEntry> MapEntries { get; set; }
    }

    public class Localisations
    {
        [XmlElement("REF")]
        public List<RefEntry> Refs { get; set; }
    }

    public class NamedType
    {
        [XmlAttribute("name")]
        public string Name { get; set; }
    }

    public class RegionType
    {
        [XmlAttribute("name")] public string Name { get; set; }
        [XmlAttribute("territory")] public string Territory { get; set; }
        [XmlAttribute("instance")] public string Instance { get; set; }
        [XmlText] public string Value { get; set; }
    }

    public class MapEntry
    {
        [XmlAttribute("code")] public string Code { get; set; }
        [XmlAttribute("loc")] public int Loc { get; set; }
        [XmlText] public string Value { get; set; }
    }

    public class RefEntry
    {
        [XmlAttribute("language")]
        public string Language { get; set; }

        [XmlText]
        public string Value { get; set; }
    }
}
