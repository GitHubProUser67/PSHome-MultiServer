using System.Globalization;
using System.Net;
using System.Threading.RateLimiting;

namespace MultiServerLibrary.HTTP
{
    public sealed class HTTPRateLimiter : IDisposable
    {
        private readonly PartitionedRateLimiter<IPEndPoint> _limiter;

        public HTTPRateLimiter()
        {
            _limiter = PartitionedRateLimiter.Create<IPEndPoint, IPAddress>(endpoint =>
            {
                if (MultiServerLibraryConfiguration.RatePermitLimit < 0)
                    return RateLimitPartition.GetNoLimiter(endpoint.Address);

                return RateLimitPartition.GetSlidingWindowLimiter(
                    endpoint.Address,
                    address => new SlidingWindowRateLimiterOptions
                    {
                        AutoReplenishment = true,
                        // No rate limit for localhost.
                        PermitLimit = IPAddress.IsLoopback(address) ? int.MaxValue : MultiServerLibraryConfiguration.RatePermitLimit,
                        Window = TimeSpan.FromSeconds(MultiServerLibraryConfiguration.RateWindowSeconds),
                        SegmentsPerWindow = MultiServerLibraryConfiguration.RateSegmentsPerWindow,
                        QueueLimit = MultiServerLibraryConfiguration.RateQueueLimit,
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst
                    });
            });
        }

        public async Task<(bool, string, string)> TryGetRateLimitSlot(IPEndPoint endpoint, CancellationToken cancellationToken = default)
        {
            using var lease = await _limiter.AcquireAsync(endpoint, 1, cancellationToken).ConfigureAwait(false);
            if (lease.IsAcquired)
                return (true, null, null);
            if (lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
                return (false, "Retry-After", ((int)retryAfter.TotalSeconds).ToString(NumberFormatInfo.InvariantInfo));
            return (false, null, null);
        }

        public void Dispose()
        {
            _limiter.Dispose();
        }
    }
}
