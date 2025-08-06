namespace MultiSocks.Aries.Messages.AuthService
{
    public class SKey : AbstractMessage
    {
        public override string _Name { get => "skey"; }

        public override void Process(AbstractAriesServer context, AriesClient client)
        {
            //TODO: get actual session key

            client.SKEY = "51ba8aee64ddfacae5baefa6bf61e009";

            OutputCache.Add("SKEY", "$" + client.SKEY);

            if (!string.IsNullOrEmpty(context.Project) && context.Project.Contains("BURNOUT5"))
            {
                if (context.SKU == "PS3")
                    OutputCache.Add("DP", "PS3/Burnout-Dec2007/mod");
                else if (context.SKU == "PC")
                {
                    OutputCache.Add("DP", "PC/Burnout-2008/na1");
                    OutputCache.Add("GFID", "\"ODS:19038.110.Base Product;BURNOUT PARADISE ULTIMATE EDITION_PC_ONLINE_ACCESS\"");
                    OutputCache.Add("PLATFORM", "pc");
                    OutputCache.Add("PSID", "PS-REG-BURNOUT2008");
                }
                else
                    OutputCache.Add("DP", "XBOX360/Burnout-Dec2007/mod");
            }

            client.SendMessage(this);
        }
    }
}
