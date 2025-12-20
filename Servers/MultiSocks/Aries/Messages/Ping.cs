namespace MultiSocks.Aries.Messages
{
    public class Ping : AbstractMessage
    {
        public override string _Name { get => "~png"; }

        public string? TIME { get; set; }

        public override void Process(AbstractAriesServer context, AriesClient client)
        {
            client.Ping = (int)new TimeSpan(DateTime.Now.Ticks - client.PingSendTick).TotalMilliseconds;
        }
    }
}
