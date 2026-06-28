namespace DNSLibrary
{
    public interface IMessage
    {
        IList<Question> Questions { get; }

        int Size { get; }
        byte[] ToArray();
    }
}
