namespace MultiSocks.Aries.Components.AuthService.News
{
    public class DefaultNewsOutCfg : AbstractMessage
    {
        public override string _Name
        {
            get => "news";
        }
        public string? PEERTIMEOUT { get; set; } = "10000";
        public string? GPS_REGIONS { get; set; } =
            $"{MultiSocksServerConfiguration.ServerBindAddress},{MultiSocksServerConfiguration.ServerBindAddress},{MultiSocksServerConfiguration.ServerBindAddress},{MultiSocksServerConfiguration.ServerBindAddress}";
        public string? WEB_OFFER_URL { get; set; } =
            "\"http://gos.ea.com/easo/editorial/common/2008/eaconnect/connect.jsp?site=easo&lkey=$LKEY$&lang=%s&country=%s\"";
        public string? WEB_OFFER_DATE { get; set; } = "2008.6.11 21:00:00";
        public string? TOSAC_URL { get; set; }
        public string? TOSA_URL { get; set; }
        public string? TOS_URL { get; set; }
        public string? TELE_DISABLE { get; set; } =
            "AD,AF,AG,AI,AL,AM,AN,AO,AQ,AR,AS,AW,AX,AZ,BA,BB,BD,BF,BH,BI,BJ,BM,BN,BO,BR,BS,BT,BV,BW,BY,BZ,CC,CD,CF,CG,CI,CK,CL,CM,CN,CO,CR,CU,CV,CX,DJ,DM,DO,DZ,EC,EG,EH,ER,ET,FJ,FK,FM,FO,GA,GD,GE,GF,GG,GH,GI,GL,GM,GN,GP,GQ,GS,GT,GU,GW,GY,HM,HN,HT,ID,IL,IM,IN,IO,IQ,IR,IS,JE,JM,JO,KE,KG,KH,KI,KM,KN,KP,KR,KW,KY,KZ,LA,LB,LC,LI,LK,LR,LS,LY,MA,MC,MD,ME,MG,MH,ML,MM,MN,MO,MP,MQ,MR,MS,MU,MV,MW,MY,MZ,NA,NC,NE,NF,NG,NI,NP,NR,NU,OM,PA,PE,PF,PG,PH,PK,PM,PN,PS,PW,PY,QA,RE,RS,RW,SA,SB,SC,SD,SG,SH,SJ,SL,SM,SN,SO,SR,ST,SV,SY,SZ,TC,TD,TF,TG,TH,TJ,TK,TL,TM,TN,TO,TT,TV,TZ,UA,UG,UM,UY,UZ,VA,VC,VE,VG,VN,VU,WF,WS,YE,YT,ZM,ZW,ZZ";
        public string? NEWS_DATE { get; set; } = "2008.6.11 21:00:00";
        public string? NEWS_URL { get; set; }
    }
}
