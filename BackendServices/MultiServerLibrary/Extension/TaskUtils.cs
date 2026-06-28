namespace MultiServerLibrary.Extension
{
    public static class TaskUtils
    {
        // from https://stackoverflow.com/a/22078975
        public static async Task<TResult> TimeoutAfter<TResult>(
            this Task<TResult> task,
            TimeSpan timeout
        )
        {
            using (var timeoutCancellationTokenSource = new CancellationTokenSource())
            {
                if (
                    await Task.WhenAny(
                            task,
                            Task.Delay(timeout, timeoutCancellationTokenSource.Token)
                        )
                        .ConfigureAwait(false) == task
                )
                {
                    timeoutCancellationTokenSource.Cancel();
                    return await task.ConfigureAwait(false); // Very important in order to propagate exceptions
                }

                throw new TimeoutException(
                    "[TaskUtils] - TimeoutAfter<TResult>: The operation has timed out."
                );
            }
        }

        public static async Task<bool> TryAwait(this Task task, TimeSpan timeout)
        {
            using (var timeoutCancellationTokenSource = new CancellationTokenSource())
            {
                if (
                    await Task.WhenAny(
                            task,
                            Task.Delay(timeout, timeoutCancellationTokenSource.Token)
                        )
                        .ConfigureAwait(false) == task
                )
                {
                    timeoutCancellationTokenSource.Cancel();
                    await task.ConfigureAwait(false);
                    return true;
                }

                return false;
            }
        }

        public static async Task<(bool Success, T Result)> TryAwaitWithResult<T>(
            this Task<T> task,
            TimeSpan timeout
        )
        {
            using (var timeoutCancellationTokenSource = new CancellationTokenSource())
            {
                if (
                    await Task.WhenAny(
                            task,
                            Task.Delay(timeout, timeoutCancellationTokenSource.Token)
                        )
                        .ConfigureAwait(false) == task
                )
                {
                    timeoutCancellationTokenSource.Cancel();
                    return (true, await task.ConfigureAwait(false));
                }

                return (false, default!);
            }
        }
    }
}
