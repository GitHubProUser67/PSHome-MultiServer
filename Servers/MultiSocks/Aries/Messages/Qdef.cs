namespace MultiSocks.Aries.Messages
{
    public class Qdef : AbstractMessage
    {
        public override string _Name { get => "qdef"; }

        public override void Process(AbstractAriesServer context, AriesClient client)
        {
            OutputCache.Add("QMSG0", " \"Wanna play?\"");
            OutputCache.Add("QMSG1", "\"I rule!\")");
            OutputCache.Add("QMSG2", "\"Doh!\"");
            OutputCache.Add("QMSG3", "\"Mmmm... doughnuts.\"");
            OutputCache.Add("QMSG04", "\"What time is it?\"");
            OutputCache.Add("QMSG05", "\"The truth is out of style.\"");
            OutputCache.Add("QMSG06", "\"Ready for some Style Action?\"");
            OutputCache.Add("QMSG07", "\"Now that's why I am talking about!\"");
            OutputCache.Add("QMSG08", "\"Can you make it to the finish line?\"");

            client.SendMessage(this);
        }
    }
}
