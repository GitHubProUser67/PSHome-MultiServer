namespace HorizonService.PlaystationHomePlugin.Models
{
    public class HomeOffsets
    {
        public string TitleIdOffset { get; set; }
        public string TitleIdOffset2 { get; set; }
        public string AppIdOffset { get; set; }
        public string ServiceIdOffset { get; set; }
        public string MuisVersionOffset1 { get; set; }
        public string OfflineNameOffset { get; set; }
        public string PSplusOffset { get; set; }
        public string CMDConsoleOffset { get; set; }
        public string ProfFilterOffset { get; set; }
        public string MlaaOffset { get; set; }
        public string EulaOffset { get; set; }
        public string DrawDistOffset { get; set; }
        public string DrawDistOffset2 { get; set; }
        public string DrawDistOffset3 { get; set; }
        public string DrawDistOffset4 { get; set; }
        public string DrawDistOffset5 { get; set; }
        public string DrawDistOffset6 { get; set; }
        public string DrawDistOffset7 { get; set; }
        public string DrawDistOffset8 { get; set; }
        public string NpMatchingTTYSpamOffset { get; set; }
    }

    public class HomeOffsetsJsonData
    {
        public string Sha1Hash { get; set; }
        public double VersionAsDouble { get; set; }
        public string Version { get; set; }
        public string Type { get; set; }
        public HomeOffsets Offsets { get; set; }
    }
}
