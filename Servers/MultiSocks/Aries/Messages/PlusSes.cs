namespace MultiSocks.Aries.Messages
{
    public class PlusSes : AbstractMessage
    {
        public override string _Name { get => "+ses"; }

        public string? IDENT { get; set; }
        public string WHEN { get; set; } = "2003.12.8 15:52:54";
        public string NAME { get; set; } = "session";
        public string? PRIV { get; set; }
        public string? OPID0 { get; set; }
        public string? GAMEPORT { get; set; }
        public string? NUMPART { get; set; }
        public string? CUSTFLAGS { get; set; }
        public string? PARTPARAMS { get; set; }
        public string? PARAMS { get; set; }
        public string? GPSHOST { get; set; }
        public string? PARTSIZE0 { get; set; }
        public string? MADDR0 { get; set; }
        public string? MINSIZE { get; set; }
        public string? MAXSIZE { get; set; }
        public string? LADDR0 { get; set; }
        public string? ADDR0 { get; set; }
        public string? OPPO0 { get; set; }
        public string? VOIPPORT { get; set; }
        public string? HOST { get; set; } //host persona name
        public string? ROOM { get; set; } //room name
        public string COUNT { get; set; } = "1";
        public string USERFLAGS { get; set; } = "0";
        public string SYSFLAGS { get; set; } = "0";
        public string? KIND { get; set; }

        public string?[]? OPID { get; set; } //opponent userid
        public string?[]? OPPO { get; set; } //opponent persona
        public string?[]? ADDR { get; set; } //ip for user

        public string? SEED { get; set; }
        public string? SELF { get; set; } //my persona name
    }
}
