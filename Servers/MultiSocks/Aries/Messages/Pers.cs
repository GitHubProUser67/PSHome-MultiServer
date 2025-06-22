namespace MultiSocks.Aries.Messages
{
    public class Pers : AbstractMessage
    {
        public override string _Name { get => "pers"; }

        public string? A { get; set; }
        public string? EXtelemetry { get; set; }
        public string? EXticker { get; set; }
        public string? IDLE { get; set; }
        public string? LA { get; set; }
        public string LKEY { get; set; } = "3fcf27540c92935b0a66fd3b0000283c";
        public string? LOC { get; set; }
        public string? MA { get; set; }
        public string? NAME { get; set; }
        public string? PERS { get; set; }
        public string? STAT { get; set; }
        public string LAST { get; set; } = "2003.12.8 15:51:58";
        public string PLAST { get; set; } = "2003.12.8 16:51:40";
        public string? PSINCE { get; set; }
        public string? SINCE { get; set; }

        public override void Process(AbstractAriesServer context, AriesClient client)
        {
            if (context is not MatchmakerServer) return;

            Model.AriesUser? user = client.User;
            if (user == null || user.SelectedPersona != -1) return;
            user.SelectPersona(GetInputCacheValue("PERS"));
            if (user.SelectedPersona == -1) return; //failed?

            if (!string.IsNullOrEmpty(context.Project) && (context.Project.Contains("BURNOUT5") || context.Project.Contains("DPR-09") || (context.Project.Contains("NASCAR09") && context.SKU == "PS3")))
            {
                string dateNow = DateTime.Now.ToString("yyyy.M.d-HH:mm:ss");

                if (client.SKU == "PS3")
                    client.SendMessage(new Pers()
                    {
                        A = user.ADDR,
                        NAME = user.Username,
                        PERS = user.PersonaName,
                        LAST = dateNow,
                        PLAST = dateNow,
                        SINCE = "2008.1.1-00:00:00",
                        PSINCE = "2008.1.1-00:00:00",
                        LKEY = "000000000000000000000000000",
                        STAT = ",,,,,,,,,,,,,,,,,,,,,,,,,,,,,,",
                        LOC = user.LOC,
                        MA = GetInputCacheValue("MAC") ?? string.Empty,
                        LA = user.LADDR,
                        IDLE = "50000"
                    });
                else
                    client.SendMessage(new Pers()
                    {
                        A = user.ADDR,
                        NAME = user.Username,
                        PERS = user.PersonaName,
                        LAST = dateNow,
                        PLAST = dateNow,
                        SINCE = "2008.1.1-00:00:00",
                        PSINCE = "2008.1.1-00:00:00",
                        LKEY = "000000000000000000000000000",
                        STAT = ",,,,,,,,,,,,,,,,,,,,,,,,,,,,,,",
                        LOC = user.LOC,
                        MA = GetInputCacheValue("MAC") ?? string.Empty,
                        LA = user.LADDR,
                        IDLE = "50000",
                        OutputCache = new Dictionary<string, string?> { { "EX-telemetry", $"{context.listenIP},9983,{user.LOC},^\xf1\xfe\xf6\xd0\xcd\xc5\xcb\x9f\xb5\xa8\xf2\xa8\xa0\xe3\xa8\xa0\x98\xa0\xcb\xa3\xb7\x8c\x9a\xb2\xac\xc8\xdc\x89\xf6\xa6\x8e\x98\xb5\xea\xd0\x91\xa3\xc6\xcc\xb1\xac\xc8\xc0\x81\x83\x86\x8c\x98\xb0\xe0\xc0\x81\xa3\xec\x8c\x99\xb5\xf0\xe0\xa1\xc6\x85\xd4\xa1\xaf\x84\xd5\x93\xe7\xed\xdb\xba\xf4\xda\xc8\x81\x83\x86\xce\x97\xee\xc2\xc5\xe1\x92\xc6\xcc\x99\xb4\xe0\xe0\xb1\xc3\xa6\xce\x98\xac\xca\xb9\xab\xb5\x8a\x80" } }
                    });

                user.SendPlusWho(user, context.Project);
            }
            else
            {
                client.SendMessage(new Pers()
                {
                    NAME = user.Username,
                    PERS = user.PersonaName,
                    MA = GetInputCacheValue("MAC") ?? string.Empty,
                    A = user.ADDR
                });
            }
        }
    }
}
