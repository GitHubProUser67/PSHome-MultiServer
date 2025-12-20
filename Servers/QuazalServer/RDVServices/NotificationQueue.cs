using QuazalServer.RDVServices.DDL.Models;
using QuazalServer.QNetZ;
using System.Diagnostics;

namespace QuazalServer.RDVServices
{
    public class NotificationQueueEntry
	{
		public NotificationQueueEntry(uint _timeout, QClient _client, NotificationEvent eventData)
		{
			client = _client;
			timeout = _timeout;
			data = eventData;

			timer = new Stopwatch();
			timer.Start();
		}

		public QClient client;
		public Stopwatch timer;
		public NotificationEvent data;
		public uint timeout;
	}

	public static class NotificationQueue
	{
		private static readonly object _sync = new();
		private static List<NotificationQueueEntry> queued = new();

		public static void AddNotification(NotificationEvent eventData, QClient client, uint timeout)
		{
			var qItem = new NotificationQueueEntry(timeout, client, eventData);

			lock (_sync)
			{
				queued.Add(qItem);
			}
		}

		public static void Update(QPacketHandlerPRUDP handler)
		{
			lock (_sync)
			{
				for (int i = 0; i < queued.Count; i++)
				{
					NotificationQueueEntry n = queued[i];
					if (n.timer.ElapsedMilliseconds > n.timeout)
					{
						SendNotification(handler, n.client, n.data);

						n.timer.Stop();
						queued.RemoveAt(i);
						i--;
					}
				}
			}
		}

		public static void SendNotification(QPacketHandlerPRUDP handler, QClient client, NotificationEvent eventData)
		{
			const byte NotificationEventManagerProtId = 14; 
            RMC.RMC.SendRMCCall(handler, client, NotificationEventManagerProtId, 1, new RMC.RMCPRequestDDL<NotificationEvent>(eventData));
		}
	}
}
