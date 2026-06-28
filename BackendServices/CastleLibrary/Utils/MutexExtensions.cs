namespace CastleLibrary.Utils
{
    public static class MutexExtensions
    {
        public static bool TryWithMutex(Mutex mutex, TimeSpan? timeout, Action action)
        {
            ArgumentNullException.ThrowIfNull(action);

            bool taken = false;

            try
            {
                try
                {
                    taken = timeout != null ? mutex.WaitOne(timeout.Value) : mutex.WaitOne();
                }
                catch (AbandonedMutexException)
                {
                    taken = true; // we now own it
                }

                if (!taken)
                    return false;

                action();
                return taken;
            }
            finally
            {
                if (taken)
                    mutex.ReleaseMutex();
            }
        }
    }
}
