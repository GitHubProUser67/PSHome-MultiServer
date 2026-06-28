namespace DNSLibrary.Utils
{
    public static class TaskExtensions
    {
        public static async Task<T> WithCancellation<T>(this Task<T> task, CancellationToken token)
        {
            var tcs = new TaskCompletionSource<bool>();
            var registration = token.Register(
                src =>
                {
                    ((TaskCompletionSource<bool>)src).TrySetResult(true);
                },
                tcs
            );

            using (registration)
            {
                if (await Task.WhenAny(task, tcs.Task).ConfigureAwait(false) != task)
                {
                    throw new OperationCanceledException(token);
                }
            }

            return await task.ConfigureAwait(false);
        }

        public static async Task<T> WithCancellationTimeout<T>(
            this Task<T> task,
            TimeSpan timeout,
            CancellationToken cancellationToken = default(CancellationToken)
        )
        {
            using (var timeoutSource = new CancellationTokenSource(timeout))
            using (
                var linkSource = CancellationTokenSource.CreateLinkedTokenSource(
                    timeoutSource.Token,
                    cancellationToken
                )
            )
            {
                return await task.WithCancellation(linkSource.Token).ConfigureAwait(false);
            }
        }
    }
}
