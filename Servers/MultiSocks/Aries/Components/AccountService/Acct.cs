using MultiSocks.Aries.DataStore;
using MultiSocks.Aries.Components.AccountService.ErrorCodes;
using MultiSocks.Utils;

namespace MultiSocks.Aries.Components.AccountService
{
    public class Acct : AbstractMessage
    {
        public override string _Name
        {
            get => "acct";
        }

        public override void Process(AbstractAriesServer context, AriesClient client)
        {
            if (context is not MatchmakerServer)
                return;

            var VERS = GetInputCacheValue("VERS");
            var SKU = GetInputCacheValue("SKU");
            var MADDR = GetInputCacheValue("MADDR");
            var NAME = GetInputCacheValue("NAME");
            var PASS = GetInputCacheValue("PASS");
            var TOS = GetInputCacheValue("TOS");
            var SHARE = GetInputCacheValue("SHARE");
            var MAIL = GetInputCacheValue("MAIL");

            if (SKU == "PS3")
            {
                var maddrparams = MADDR?.Split('$');

                if (maddrparams != null)
                    NAME = maddrparams.FirstOrDefault();
            }

            var DecryptedPass = PasswordUtils.Ssc2Decode(PASS, client.SKEY);

            if (!string.IsNullOrEmpty(NAME))
            {
                DbAccount info = new()
                {
                    Username = NAME,
                    TOS = TOS,
                    SHARE = SHARE,
                    MAIL = MAIL,
                    Password = DecryptedPass,
                };

                var created = Program.DirtySocksDatabase?.CreateNew(info);
                if (created != null && created.Value)
                {
                    CustomLogger.LoggerAccessor.LogInfo(
                        $"[Acct] - Created new account: {info.Username}"
                    );

                    OutputCache.Add("NAME", NAME);
                    OutputCache.Add("PERSONAS", info.Personas[0]);
                    OutputCache.Add("AGE", "24");

                    client.SendMessage(this);
                }
                else
                {
                    var alts = GetInputCacheValue("ALTS");

                    if (string.IsNullOrEmpty(alts) && int.TryParse(alts, out var integeralts))
                        client.SendMessage(
                            new AcctDupl() { OPTS = SuggestNames(integeralts, NAME) }
                        );
                    else
                        client.SendMessage(new AcctDupl());
                }
            }
            else
                client.SendMessage(new AcctImst());
        }

        public static string SuggestNames(int alts, string name)
        {
            HashSet<string> opts = new();

            if (name.Length > 8)
                name = name[..7];

            Random random = new();

            for (var i = 1; i <= alts; i++)
            {
                if (i == 1)
                    opts.Add(name + "Kid");
                else if (i == 2)
                    opts.Add(name + "Rule");
                else
                    opts.Add(name + random.Next(1000, 10000));
            }

            return string.Join(",", opts.Select(s => s.Length > 12 ? s.Substring(0, 11) : s));
        }
    }
}
