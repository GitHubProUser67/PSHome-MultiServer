namespace MultiSocks.Aries.Components
{
    public class Rept : AbstractMessage
    {
        public override string _Name
        {
            get => "rept";
        }

        public override void Process(AbstractAriesServer context, AriesClient client)
        {
            client.SendMessage(this);
        }
    }
}
