namespace MultiSocks.Aries.Messages
{
    public class Sdta : AbstractMessage
    {
        public override string _Name { get => "sdta"; }

        public string SLOT { get; set; } = "0";
        public string STATS { get; set; } = "1,2,3,4,5,6,7,8,9,10,11,12,13";

        public override void Process(AbstractAriesServer context, AriesClient client)
        {
            client.SendMessage(this);
        }
    }
}
