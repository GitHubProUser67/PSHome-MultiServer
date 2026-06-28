namespace MultiSocks.Aries.Components
{
    public class Ottr : AbstractMessage
    {
        public override string _Name
        {
            get => "ottr";
        }

        public override void Process(AbstractAriesServer context, AriesClient client)
        {
            client.SendMessage(this);
        }
    }
}
