namespace MultiSocks.Aries.Messages.AuthService.News.Burnout
{
    public class BOPNewsOut : AbstractMessage
    {
        public override string _Name { get => "news"; }
        public string? PEERTIMEOUT { get; set; } = "10000";
        public string? BUDDY_URL { get; set; } = "\"http://gos.ea.com/\"";
        public string? BUDDY_SERVER { get; set; } = MultiSocksServerConfiguration.ServerBindAddress;
        public string? BUDDY_PORT { get; set; } = "13505";
        public string? GPS_REGIONS { get; set; } = $"{MultiSocksServerConfiguration.ServerBindAddress},{MultiSocksServerConfiguration.ServerBindAddress},{MultiSocksServerConfiguration.ServerBindAddress},{MultiSocksServerConfiguration.ServerBindAddress}";
        public string? EACONNECT_WEBOFFER_URL { get; set; } = "\"http://gos.ea.com/easo/editorial/common/2008/eaconnect/connect.jsp?site=easo&lkey=$LKEY$&lang=%s&country=%s\"";
        public string? TOSAC_URL { get; set; }
        public string? TOSA_URL { get; set; }
        public string? TOS_URL { get; set; }
        public string? USE_GLOBAL_ROAD_RULE_SCORES { get; set; } = "0";
        public string? ROAD_RULES_RESET_DATE { get; set; } = "2007.10.11 18:00:00";
        public string? CAR_OLD_ROAD_RULES_TAGFIELD { get; set; } = "\"RULES,RULES1,RULES2,RULES3,RULES4,RULES5,RULES6,RULES7,RULES8,RULES9,RULES10,RULES11,RULES12,RULES13,RULES14,RULES15,RULES16\"";
        public string? CAR_ROAD_RULES_TAGFIELD { get; set; } = "\"RULES17\"";
        public string? BIKE_DAY_OLD_ROAD_RULES_TAGFIELD { get; set; } = "\"BIKEDAYRULES1,BIKEDAYRULES2\"";
        public string? BIKE_DAY_ROAD_RULES_TAGFIELD { get; set; } = "\"BIKEDAYRULES3\"";
        public string? BIKE_NIGHT_OLD_ROAD_RULES_TAGFIELD { get; set; } = "\"BIKENIGHTRULES1,BIKENIGHTRULES2\"";
        public string? BIKE_NIGHT_ROAD_RULES_TAGFIELD { get; set; } = "\"BIKENIGHTRULES3\"";
        public string? QOS_LOBBY { get; set; } = MultiSocksServerConfiguration.ServerBindAddress;
        public string? OS_PORT { get; set; } = "17582";
        public string? ROAD_RULES_SKEY { get; set; } = "frscores";
        public string? PROFANE_STRING { get; set; } = "\"@/&!\"";
        public string? CHAL_SKEY { get; set; } = "chalscores";
        public string? FEVER_CARRIERS { get; set; } = "FritzBraun,EricWimp,Matazone,NutKC,FlufflesDaBunny,Flinnster,Molen,LingBot,DDangerous,Technocrat,The PLB,Chipper1977,Bazmobile,CustardKid,The Wibbler,AlexBowser,Blanks 82,Maxreboh,Jackhamma,MajorMajorMajor,Riskjockey,ChiefAV,Charnjit,Zietto,BurntOutDave,Belj,Cupster,Krisis1969,OrangeGopher,Phaigoman,Drastic Surgeon,Tom Underdown,Discodoktor,Cargando,Gaztech,PompeyPaul,TheSoldierBoy,louben17,Colonel Gambas,EliteBeatAgent,Uaintdown,SynergisticFX,InfamousGRouse,EAPR,EAPR 02,Jga360 JP2,EAJproduct";
        public string? TELE_DISABLE { get; set; } = "AD,AF,AG,AI,AL,AM,AN,AO,AQ,AR,AS,AW,AX,AZ,BA,BB,BD,BF,BH,BI,BJ,BM,BN,BO,BR,BS,BT,BV,BW,BY,BZ,CC,CD,CF,CG,CI,CK,CL,CM,CN,CO,CR,CU,CV,CX,DJ,DM,DO,DZ,EC,EG,EH,ER,ET,FJ,FK,FM,FO,GA,GD,GE,GF,GG,GH,GI,GL,GM,GN,GP,GQ,GS,GT,GU,GW,GY,HM,HN,HT,ID,IL,IM,IN,IO,IQ,IR,IS,JE,JM,JO,KE,KG,KH,KI,KM,KN,KP,KR,KW,KY,KZ,LA,LB,LC,LI,LK,LR,LS,LY,MA,MC,MD,ME,MG,MH,ML,MM,MN,MO,MP,MQ,MR,MS,MU,MV,MW,MY,MZ,NA,NC,NE,NF,NG,NI,NP,NR,NU,OM,PA,PE,PF,PG,PH,PK,PM,PN,PS,PW,PY,QA,RE,RS,RW,SA,SB,SC,SD,SG,SH,SJ,SL,SM,SN,SO,SR,ST,SV,SY,SZ,TC,TD,TF,TG,TH,TJ,TK,TL,TM,TN,TO,TT,TV,TZ,UA,UG,UM,UY,UZ,VA,VC,VE,VG,VN,VU,WF,WS,YE,YT,ZM,ZW,ZZ";
        public string? BUNDLE_PATH { get; set; } = "\"http://gos.ea.com/easo/editorial/Burnout/2008/livedata/bundle/\"";
        public string? NEWS_DATE { get; set; } = "2008.6.11 21:00:00";
        public string? NEWS_URL { get; set; }
        public string? AVAIL_DLC_URL { get; set; } = "\"http://gos.ea.com/easo/editorial/Burnout/2008/livedata/Ents.txt\" ";
        public string? AVATAR_URL { get; set; } = "\"http://www.criteriongames.com/pcstore/avatar.php?persona=%s\"";
        public string? AVATAR_URL_ENCRYPTED { get; set; } = "0";
        public string? STORE_DLC_URL { get; set; }
        public string? STORE_URL { get; set; }
        public string? STORE_URL_ENCRYPTED { get; set; } = "0";
        public string? ETOKEN_URL { get; set; } = "\"http://gos.ea.com/easo/editorial/common/2008/nucleus/nkeyToNucleusEncryptedToken.jsp?nkey=%s&signature=%s\"";
        public string? USE_ETOKEN { get; set; } = "0";
        public string? LIVE_NEWS_URL { get; set; }
        public string? LIVE_NEWS2_URL { get; set; }
        public string? PRODUCT_DETAILS_URL { get; set; } = "\"http://pctrial.burnoutweb.ea.com/t2b/page/ofb_pricepoints.php?productID=%s&env=live\"";
        public string? PRODUCT_SEARCH_URL { get; set; } = "\"http://pctrial.burnoutweb.ea.com/t2b/page/ofb_DLCSearch.php?env=live\"";
    }
}
