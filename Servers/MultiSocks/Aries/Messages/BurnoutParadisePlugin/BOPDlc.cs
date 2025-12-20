namespace MultiSocks.Aries.Messages
{
    public class BOPDlc : AbstractMessage
    {
        public override string _Name { get => "sviw"; }

        public string N { get; set; } = "9";
        public string DESCS { get; set; } = "1,1,1,1,1,1,1,1,1";
        public string NAMES { get; set; } = "0,3,4,0,3,4,0,3,4";
        public string PARAMS { get; set; } = "2,2,2,2,2,2,2,2,2";
        public string WIDTHS { get; set; } = "1,1,1,1,1,1,1,1,1";
        public string SYMS { get; set; } = "TOTCOM,a,0,TAKEDNS,RIVALS,ACHIEV,FBCHAL,RANK,WINS,SNTTEAM,SNTFFA";
        public string TYPES { get; set; } = "~num,~num,~num,~num,~rnk,~num,~pts,~pts";
        public string SS { get; set; } = "65";
    }
}
