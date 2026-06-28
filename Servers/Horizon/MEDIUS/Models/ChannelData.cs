using System.Collections.Concurrent;
using Horizon.CustomServers.Models;
using Horizon.MUM.Models;
using Horizon.RT.Models;
using MultiServerLibrary.Extension;

namespace Horizon.MEDIUS.Models
{
    public class ChannelData
    {
        public int ApplicationId { get; set; } = 0;
        public ClientObject? ClientObject { get; set; } = null;
        public ClientObject? MeClientObject { get; set; } = null;
        public string? MachineId { get; set; } = null;
        public ConcurrentQueue<BaseScertMessage> RecvQueue { get; } =
            new ConcurrentQueue<BaseScertMessage>();
        public ConcurrentQueue<BaseScertMessage> SendQueue { get; } =
            new ConcurrentQueue<BaseScertMessage>();

        public ServerClientState State { get; set; } = ServerClientState.DISCONNECTED;

        public bool? IsBanned { get; set; } = null;

        /// <summary>
        /// When true, all messages from this client will be ignored.
        /// </summary>
        public bool Ignore { get; set; } = false;
        public DateTime TimeConnected { get; set; } = DateTimeUtils.GetHighPrecisionUtcTime();

        /// <summary>
        /// Timesout client if they haven't authenticated after a given number of seconds.
        /// </summary>
        public bool ShouldDestroy =>
            ClientObject == null
            && (DateTimeUtils.GetHighPrecisionUtcTime() - TimeConnected).TotalSeconds
                > DATABASE
                    .DatabaseManager.GetAppSettingsOrDefault(ApplicationId)
                    .ClientTimeoutSeconds;
    }
}
