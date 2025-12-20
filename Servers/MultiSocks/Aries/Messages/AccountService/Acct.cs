using MultiSocks.Aries.DataStore;
using MultiSocks.Aries.Messages.AccountService.ErrorCodes;
using MultiSocks.Utils;

namespace MultiSocks.Aries.Messages.AccountService
{
    public class Acct : AbstractMessage
    {
        public override string _Name { get => "acct"; }

        public override void Process(AbstractAriesServer context, AriesClient client)
        {
            if (context is not MatchmakerServer) return;

            string? VERS = GetInputCacheValue("VERS");
            string? SKU = GetInputCacheValue("SKU");
            string? MADDR = GetInputCacheValue("MADDR");
            string? NAME = GetInputCacheValue("NAME");
            string? PASS = GetInputCacheValue("PASS");
            string? TOS = GetInputCacheValue("TOS");
            string? SHARE = GetInputCacheValue("SHARE");
            string? MAIL = GetInputCacheValue("MAIL");

            if (SKU == "PS3")
            {
                string[]? maddrparams = MADDR?.Split('$');

                if (maddrparams != null)
                    NAME = maddrparams.FirstOrDefault();
            }

            string? DecryptedPass = new PasswordUtils().ssc2Decode(PASS, client.SKEY);

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

                bool? created = Program.DirtySocksDatabase?.CreateNew(info);
                if (created != null && created.Value)
                {
                    CustomLogger.LoggerAccessor.LogInfo("Created new account: " + info.Username);

                    OutputCache.Add("NAME", NAME);
                    OutputCache.Add("PERSONAS", NAME); // Little hack, persona is effectively username as that's the first persona ever added.
                    OutputCache.Add("AGE", "24");

                    client.SendMessage(this);
                }
                else
                {
                    string? alts = GetInputCacheValue("ALTS");

                    if (string.IsNullOrEmpty(alts) && int.TryParse(alts, out int integeralts))
                        client.SendMessage(new AcctDupl() { OPTS = SuggestNames(integeralts, NAME) });
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

            for (int i = 1; i <= alts; i++)
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
