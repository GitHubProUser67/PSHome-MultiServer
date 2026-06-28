using MultiServerLibrary;
using MultiSocks.Aries.Components.AccountService.ErrorCodes;

namespace MultiSocks.Aries.Components.AccountService
{
    public class Auth : AbstractMessage
    {
        public override string _Name
        {
            get => "auth";
        }

        public override void Process(AbstractAriesServer context, AriesClient client)
        {
            if (context is not MatchmakerServer mc)
                return;

            var VERS = GetInputCacheValue("VERS") ?? string.Empty;
            var SKU = GetInputCacheValue("SKU") ?? string.Empty;
            var MADDR = GetInputCacheValue("MADDR");
            var NAME = GetInputCacheValue("NAME");
            var PASS = GetInputCacheValue("PASS");
            var MAC = GetInputCacheValue("MAC");
            var LOC = GetInputCacheValue("LOC");
            var TOKEN = GetInputCacheValue("TOKEN");

            client.VERS = VERS;
            client.SKU = SKU;

            if (
                (
                    MultiServerLibraryConfiguration.BannedIPs != null
                    && MultiServerLibraryConfiguration.BannedIPs.Contains(client.ADDR)
                )
                || (
                    MultiServerLibraryConfiguration.VpnCheck != null
                    && MultiServerLibraryConfiguration.VpnCheck.IsVpnOrProxy(client.ADDR)
                )
            )
            {
                client.SendMessage(new AuthBlak());
                return;
            }

            if (SKU == "PS3")
            {
                var maddrparams = MADDR?.Split('$');

                if (maddrparams != null)
                    NAME = maddrparams.FirstOrDefault();
            }

            if (!string.IsNullOrEmpty(NAME))
            {
                if (NAME.Contains('@'))
                    NAME = NAME.Split("@")[0] + NAME.Split("@")[1];

                var user = Program.DirtySocksDatabase?.GetByName(NAME);
                if (user != null)
                {
                    mc.TryLogin(user, client, PASS, LOC ?? "enUS", MAC, TOKEN);
                    return;
                }
            }
            else if (!string.IsNullOrEmpty(TOKEN))
            {
                mc.TryGuestLogin(client, PASS, LOC ?? "enUS", MAC, TOKEN);
                return;
            }

            client.SendMessage(new AuthImst());
        }
    }
}
