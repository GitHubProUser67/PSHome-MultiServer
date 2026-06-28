namespace Horizon.MEDIUS.Processors
{
    public interface IMediusProcessor
    {
        ushort TCPPort { get; }
        ushort UDPPort { get; }
        Task StartAsync(int maxConcurrentListeners);
        Task StopAsync();
        Task Tick();
    }
}
