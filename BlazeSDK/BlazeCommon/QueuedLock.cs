namespace BlazeCommon
{
    //https://stackoverflow.com/questions/4228864/does-lock-guarantee-acquired-in-order-requested
    //https://stackoverflow.com/questions/961869/is-there-a-synchronization-class-that-guarantee-fifo-order-in-c/961904#961904
    public sealed class QueuedLock
    {
        private SemaphoreSlim slim;
        private volatile int ticketsCount = 0;
        private volatile int ticketToRide = 1;

        public QueuedLock()
        {
            slim = new SemaphoreSlim(1, 1);
        }

        public void Enter()
        {
            int myTicket = Interlocked.Increment(ref ticketsCount);
            slim.Wait();
            while (true)
            {

                if (myTicket == ticketToRide)
                {
                    return;
                }
                else
                {
                    slim.Release();
                    slim.Wait();
                }
            }
        }

        public async Task EnterAsync()
        {
            int myTicket = Interlocked.Increment(ref ticketsCount);
            await slim.WaitAsync().ConfigureAwait(false);
            while (true)
            {

                if (myTicket == ticketToRide)
                {
                    return;
                }
                else
                {
                    slim.Release();
                    await slim.WaitAsync().ConfigureAwait(false);
                }
            }
        }


        public void Exit()
        {
            Interlocked.Increment(ref ticketToRide);
            slim.Release();
        }
    }
}
