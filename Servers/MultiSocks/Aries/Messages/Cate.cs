namespace MultiSocks.Aries.Messages
{
    public class Cate : AbstractMessage
    {
        public override string _Name { get => "cate"; }

        public override void Process(AbstractAriesServer context, AriesClient client)
        {
            OutputCache.Add("NSS", "18");
            OutputCache.Add("SYMS", "TEST1,TEST2,TEST3");
            OutputCache.Add("CC", "1");
            OutputCache.Add("IC", "1");
            OutputCache.Add("VC", "1");
            OutputCache.Add("R", "0,1,1,1,2,0,0");
            OutputCache.Add("U", "1,2");

            client.SendMessage(this);
        }
    }
}
