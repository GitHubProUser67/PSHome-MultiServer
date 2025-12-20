namespace MultiSocks.Aries.Messages.AuthService
{
    public class Addr : AbstractMessage
    {
        public override string _Name { get => "addr"; }

        public override void Process(AbstractAriesServer context, AriesClient client)
        {
            client.Port = GetInputCacheValue("PORT");
            client.LADDR = GetInputCacheValue("ADDR") ?? "127.0.0.1";

            client.SendMessage(new Ping());
        }
    }
}
