namespace Horizon.SERVER.Medius
{
    public interface IMediusComponent
    {
        int TCPPort { get; }
        int UDPPort { get; }
        void Start();
        Task Stop();
        Task Tick();
    }
}