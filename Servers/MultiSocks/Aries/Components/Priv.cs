namespace MultiSocks.Aries.Components
{
    public class Priv : AbstractMessage
    {
        public override string _Name
        {
            get => "priv";
        }

        public string? PRIV { get; set; }

        public override void Process(AbstractAriesServer context, AriesClient client)
        {
            var MODE = GetInputCacheValue("GFID");

            if (!string.IsNullOrEmpty(MODE))
            {
                if (MODE == "ON")
                {
                    PRIV = "1";
                    client.SendMessage(this);
                }
                else
                {
                    PRIV = "0";
                    client.SendMessage(this);
                }
            }
            else
            {
                //We send Private Messenging to 0 in case MODE is not set!
                PRIV = "0";
                client.SendMessage(this);
            }
        }
    }
}
