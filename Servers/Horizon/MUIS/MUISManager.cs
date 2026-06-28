using CustomLogger;

namespace Horizon.MUIS
{
    public class MUISManager
    {
        private readonly MUISProcessor _processor = new();

        public MUISManager(MUISProcessor processor)
        {
            _processor = processor;
        }

        private async Task TickAsync()
        {
            try
            {
                await _processor.Tick().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError(
                    $"[MUISManager] - An assertion was thrown while ticking the server. (Exception:{ex})"
                );
            }
        }

        public async Task StartTickPooling(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                await TickAsync().ConfigureAwait(false);
                await Task.Delay(100, token).ConfigureAwait(false);
            }
        }
    }
}
