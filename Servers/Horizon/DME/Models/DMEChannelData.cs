using System.Collections.Concurrent;
using Horizon.RT.Models;
using MultiServerLibrary.Extension;

namespace Horizon.DME.Models
{
    public class DMEChannelData
    {
        public int ApplicationId { get; set; } = 0;
        public DMEObject? DMEObject { get; set; } = null;
        public ConcurrentQueue<BaseScertMessage> RecvQueue { get; } = new();
        public ConcurrentQueue<BaseScertMessage> SendQueue { get; } = new();

        /// <summary>
        /// When true, all messages from this client will be ignored.
        /// </summary>
        public bool Ignore { get; set; } = false;
        public DateTime TimeConnected { get; set; } = DateTimeUtils.GetHighPrecisionUtcTime();

        /// <summary>
        /// Timesout client if they authenticated after a given number of seconds.
        /// </summary>
        public bool ShouldDestroy =>
            DMEObject == null
            && (DateTimeUtils.GetHighPrecisionUtcTime() - TimeConnected).TotalSeconds
                > DATABASE
                    .DatabaseManager.GetAppSettingsOrDefault(ApplicationId)
                    .ClientTimeoutSeconds;
    }
}
