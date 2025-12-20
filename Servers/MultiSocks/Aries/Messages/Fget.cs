using MultiSocks.Aries.Model;

namespace MultiSocks.Aries.Messages
{
    public class Fget : AbstractMessage
    {
        public override string _Name { get => "fget"; }

        public override void Process(AbstractAriesServer context, AriesClient client)
        {
            AriesUser? user = client.User;
            if (user == null) return;

            string? TAG = GetInputCacheValue("TAG");

            user.SendPlusWho(user, context.Project);

            client.SendMessage(new PlusFup());

            if (!string.IsNullOrEmpty(TAG))
                OutputCache.Add(TAG, string.Empty);

            client.SendMessage(this);
        }
    }
}
