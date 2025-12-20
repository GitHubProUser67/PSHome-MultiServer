using QuazalServer.QNetZ.Attributes;
using QuazalServer.QNetZ.Interfaces;
using QuazalServer.RDVServices.DDL.Models;

namespace QuazalServer.RDVServices.GameServices.PCDriverServices
{
    [RMCService((ushort)RMCProtocolId.OfflineGameNotificationsService)]

    public class OfflineGameNotificationsService : RMCServiceBase
    {
        [RMCMethod(1)]
        public RMCResult PollNotifications()
        {
            return Result(new { list_notifications = new List<NotificationEvent>(), nb_remaining_notifs = (uint)0 });
        }

        [RMCMethod(2)]
        public RMCResult PollSpecificOfflineNotifications(List<uint> majortype)
        {
            return Result(new { list_timed_notification = new List<TimedNotification>(), ret = (uint)0 });
        }

        [RMCMethod(3)]
        public RMCResult PollAnyOfflineNotifications()
        {
            return Result(new { list_timed_notification = new List<TimedNotification>(), nb_remaining_notifs = (uint)0 });
        }
    }
}
