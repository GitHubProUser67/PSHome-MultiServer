namespace MultiSocks.Aries.Messages
{
    public class Uatr : AbstractMessage
    {
        public override string _Name { get => "uatr"; }

        public override void Process(AbstractAriesServer context, AriesClient client)
        {
            client.SendMessage(this);

            client.User?.SendPlusWho(client.User, context.Project);
        }
    }
}
