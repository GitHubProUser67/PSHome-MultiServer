namespace MultiSocks.Aries.Components
{
    public class Fupd : AbstractMessage
    {
        public override string _Name
        {
            get => "fupd";
        }

        public override void Process(AbstractAriesServer context, AriesClient client)
        {
            var TAG = GetInputCacheValue("TAG");
            var ADD = GetInputCacheValue("ADD");
            var DELETE = GetInputCacheValue("DELETE");

            var user = client.User;

            if (!string.IsNullOrEmpty(TAG) && user != null)
            {
                int? index;

                switch (TAG)
                {
                    case "F":
                        if (!string.IsNullOrEmpty(ADD))
                        {
                            var SplitedList = ADD.Split(",");

                            foreach (var SplitedUser in SplitedList)
                            {
                                index = Program.DirtySocksDatabase?.AddFriend(user.ID, SplitedUser);

                                if (index.HasValue && index.Value >= 0)
                                    user.Friends.Add(SplitedUser);
                            }
                        }
                        if (!string.IsNullOrEmpty(DELETE))
                        {
                            var SplitedList = DELETE.Split(",");

                            foreach (var SplitedUser in SplitedList)
                            {
                                index = Program.DirtySocksDatabase?.DeleteFriend(
                                    user.ID,
                                    SplitedUser
                                );

                                if (index.HasValue && index.Value != -1)
                                    user.Friends.Remove(SplitedUser);
                            }
                        }
                        break;
                    case "R":
                        if (!string.IsNullOrEmpty(ADD))
                        {
                            var SplitedList = ADD.Split(",");

                            foreach (var SplitedUser in SplitedList)
                            {
                                index = Program.DirtySocksDatabase?.AddRival(user.ID, SplitedUser);

                                if (index.HasValue && index.Value >= 0)
                                    user.Rivals.Add(SplitedUser);
                            }
                        }
                        if (!string.IsNullOrEmpty(DELETE))
                        {
                            var SplitedList = DELETE.Split(",");

                            foreach (var SplitedUser in SplitedList)
                            {
                                index = Program.DirtySocksDatabase?.DeleteRival(
                                    user.ID,
                                    SplitedUser
                                );

                                if (index.HasValue && index.Value != -1)
                                    user.Rivals.Remove(SplitedUser);
                            }
                        }
                        break;
                }
            }

            client.SendMessage(this);
        }
    }
}
