using System.Xml.Serialization;

namespace SVO.Games.PS3
{
    [XmlRoot("Leaderboard")]
    public class Leaderboard2048
    {
        [XmlElement("GetList")]
        public Get2048List? GetList { get; set; }
    }

    [XmlRoot("GetList")]
    public class Get2048List
    {
        [XmlElement("lb")]
        public List<Leaderboard2048Entry>? Entries { get; set; }
    }

    [XmlRoot("lb")]
    public class Leaderboard2048Entry
    {
        [XmlAttribute("id")]
        public int Id { get; set; }

        [XmlAttribute("tr")]
        public int Tr { get; set; }

        [XmlAttribute("gm")]
        public int Gm { get; set; }

        [XmlAttribute("rt")]
        public int Rt { get; set; }

        [XmlAttribute("sc")]
        public int Sc { get; set; }

        [XmlAttribute("sId")]
        public string? SId { get; set; }

        [XmlAttribute("url")]
        public string? Url { get; set; }
    }
}
