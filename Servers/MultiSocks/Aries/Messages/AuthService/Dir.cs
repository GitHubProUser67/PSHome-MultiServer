namespace MultiSocks.Aries.Messages.AuthService
{
    public class Dir : AbstractMessage
    {
        public override string _Name { get => "@dir"; }

        public override void Process(AbstractAriesServer context, AriesClient client)
        {
            if (context is not RedirectorServer rc) return;

            Random rand = new();

            OutputCache.Add("SESS", (rand.Next(1000, 10000) + rand.Next(1000, 10000) + rand.Next(10, 100)).ToString());
            OutputCache.Add("MASK", $"{rand.Next(1000, 10000)}f3f70ecb1757cd7001b9a7a{rand.Next(1000, 10000)}");
            OutputCache.Add("ADDR", rc.RedirIP);
            OutputCache.Add("PORT", rc.RedirPort);

            client.SendMessage(this);
        }
    }
}
