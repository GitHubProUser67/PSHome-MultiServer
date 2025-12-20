using QuazalServer.RDVServices.DDL.Models;
using QuazalServer.QNetZ;
using QuazalServer.QNetZ.Attributes;
using QuazalServer.QNetZ.Interfaces;
using QuazalServer.RDVServices.RMC;
using Alcatraz.Context.Entities;
using RDVServices;
using Microsoft.EntityFrameworkCore;

namespace QuazalServer.RDVServices.GameServices.PS3SparkServices
{
    /// <summary>
    /// User friends service
    /// </summary>
    [RMCService((ushort)RMCProtocolId.FriendsService)]
    public class FriendsService : RMCServiceBase
    {
        [RMCMethod(1)]
        public void AddFriend()
        {
            UNIMPLEMENTED();
        }

        [RMCMethod(2)]
        public RMCResult AddFriendByName(string strPlayerName, uint uiDetails, string strMessage)
        {
            bool result = false;
            var plInfo = Context.Client.PlayerInfo;
            var myUserPid = plInfo.PID;

            using (var db = DBHelper.GetDbContext(Context.Handler.Factory.Item1))
            {
                var foundUser = db.Users
                    .AsNoTracking()
                    .Where(x => x.Id != myUserPid)
                    .FirstOrDefault(x => x.PlayerNickName == strPlayerName);

                if (foundUser != null)
                {
                    var existringRequest = db.UserRelationships
                        .FirstOrDefault(x => x.User1Id == myUserPid && x.User2Id == foundUser.Id ||
                                             x.User1Id == foundUser.Id && x.User2Id == myUserPid);

                    if (existringRequest != null)
                    {
                        return Result(new { retVal = false });
                    }

                    // add new relationship with ID 3
                    db.UserRelationships.Add(new UserRelationship
                    {
                        Details = uiDetails,
                        User1Id = myUserPid,
                        User2Id = foundUser.Id,
                        ByRelationShip = 3
                    });
                    db.SaveChanges();

                    result = true;

                    // send notification
                    var notification = new NotificationEvent(NotificationEventsType.FriendEvent, 0)
                    {
                        m_pidSource = myUserPid,
                        m_uiParam1 = myUserPid,       // i'm just guessing
                        m_uiParam2 = 2,
                        m_strParam = strMessage
                    };

                    // send to proper client
                    var qClient = Context.Handler.GetQClientByClientPID(foundUser.Id);

                    if (qClient != null)
                        NotificationQueue.SendNotification(Context.Handler, qClient, notification);
                }
            }

            return Result(new { retVal = result });
        }

        [RMCMethod(3)]
        public void AddFriendWithDetails()
        {
            UNIMPLEMENTED();
        }

        [RMCMethod(4)]
        public void AddFriendByNameWithDetails()
        {
            UNIMPLEMENTED();
        }

        [RMCMethod(5)]
        public RMCResult AcceptFriendship(uint uiPlayer)
        {
            bool result = false;
            var plInfo = Context.Client.PlayerInfo;
            var myUserPid = plInfo.PID;

            using (var db = DBHelper.GetDbContext(Context.Handler.Factory.Item1))
            {
                var foundUser = db.Users
                    .AsNoTracking()
                    .FirstOrDefault(x => x.Id == uiPlayer);

                if (foundUser != null)
                {
                    var existringRequest = db.UserRelationships
                        .FirstOrDefault(x => x.User1Id == myUserPid && x.User2Id == foundUser.Id ||
                                             x.User1Id == foundUser.Id && x.User2Id == myUserPid);

                    if (existringRequest != null)
                    {
                        existringRequest.ByRelationShip = 1;
                        db.SaveChanges();
                    }
                    else
                    {
                        return Result(new { retVal = false });
                    }

                    result = true;

                    // send notification

                    var notification = new NotificationEvent(NotificationEventsType.FriendEvent, 0)
                    {
                        m_pidSource = myUserPid,
                        m_uiParam1 = foundUser.Id,      // i'm just guessing
                        m_uiParam2 = 1
                    };

                    // should be that sent to friend too?
                    NotificationQueue.SendNotification(Context.Handler, Context.Client, notification);
                }
            }

            return Result(new { retVal = result });
        }

        [RMCMethod(6)]
        public RMCResult DeclineFriendship(uint uiPlayer)
        {
            var plInfo = Context.Client.PlayerInfo;
            var myUserPid = plInfo.PID;

            // remove relationship from database
            using (var db = DBHelper.GetDbContext(Context.Handler.Factory.Item1))
            {
                var existringRequest = db.UserRelationships
                    .FirstOrDefault(x => x.User1Id == myUserPid && x.User2Id == uiPlayer ||
                                         x.User1Id == uiPlayer && x.User2Id == myUserPid);
                if (existringRequest != null)
                {
                    db.UserRelationships.Remove(existringRequest);
                    db.SaveChanges();
                }
            }

            // send notification
            var notification = new NotificationEvent(NotificationEventsType.FriendEvent, 0)
            {
                m_pidSource = myUserPid,
                m_uiParam1 = myUserPid,       // i'm just guessing
                m_uiParam2 = 3
            };

            // send to proper client
            var qClient = Context.Handler.GetQClientByClientPID(uiPlayer);

            if (qClient != null)
            {
                NotificationQueue.SendNotification(Context.Handler, qClient, notification);
            }

            return Result(new { retVal = true });
        }

        [RMCMethod(7)]
        public RMCResult BlackList()
        {
            UNIMPLEMENTED();
            return Error(0);
        }

        [RMCMethod(8)]
        public RMCResult BlackListByName()
        {
            UNIMPLEMENTED();
            return Error(0);
        }

        [RMCMethod(9)]
        public RMCResult ClearRelationship(uint uiPlayer)
        {
            bool result = false;
            var plInfo = Context.Client.PlayerInfo;
            var myUserPid = plInfo.PID;

            using (var db = DBHelper.GetDbContext(Context.Handler.Factory.Item1))
            {
                var existringRequest = db.UserRelationships
                    .FirstOrDefault(x => x.User1Id == myUserPid && x.User2Id == uiPlayer ||
                                         x.User1Id == uiPlayer && x.User2Id == myUserPid);

                if (existringRequest != null)
                {
                    db.UserRelationships.Remove(existringRequest);
                    db.SaveChanges();

                    result = true;
                }
            }

            return Result(new { retVal = result });
        }

        [RMCMethod(10)]
        public RMCResult UpdateDetails()
        {
            UNIMPLEMENTED();
            return Error(0);
        }

        [RMCMethod(11)]
        public RMCResult GetList()
        {
            UNIMPLEMENTED();
            return Error(0);
        }

        [RMCMethod(12)]
        public RMCResult GetDetailedList(byte byRelationship, bool bReversed)
        {
#if false
			IEnumerable<FriendData> result;

			var plInfo = Context.Client.Info;
			var myUserPid = plInfo.PID;
			
			using (var db = DBHelper.GetDbContext(Context.Handler.Factory.Item1))
			{
				var relations = db.UserRelationships
					.Include(x => x.User1)
					.Include(x => x.User2)
					.AsNoTracking()
					.Where(x => x.User1Id == myUserPid || x.User2Id == myUserPid)
					.Where(x => x.ByRelationShip == byRelationship)
					.Select(x => x.User2Id == myUserPid ?
						new UserRelationship
						{  // swap list
							User1Id = x.User2Id,
							User1 = x.User2,
							User2Id = x.User1Id,
							User2 = x.User1,
						} : x);

				if (bReversed) // hmmmm
					relations = relations.Reverse();

				// complete the list
				result = relations.Select(x =>
					new FriendData()
					{
						m_pid = x.User2Id,
						m_strName = x.User2.PlayerNickName,
						m_strStatus = "",
						m_uiDetails = x.Details,
						m_byRelationship = (byte)x.ByRelationShip
					}).ToArray();
			}
#else
            // is that always has to be empty?
            var result = new List<FriendData>();
#endif
            return Result(result);
        }

        [RMCMethod(13)]
        public RMCResult GetRelationships(int offset, int size)
        {
            var onlinePlayerIds = NetworkPlayers.Players.Select(x => x.PID);

            var result = new RelationshipsResult();

            var myUserPid = Context.Client.PlayerInfo.PID;
            using (var db = DBHelper.GetDbContext(Context.Handler.Factory.Item1))
            {
                var relations = db.UserRelationships
                    .Include(x => x.User1)
                    .Include(x => x.User2)
                    .AsNoTracking()
                    .Where(x => x.User1Id == myUserPid || x.User2Id == myUserPid);

                //result.uiTotalCount = (uint)size;
                result.uiTotalCount = (uint)relations.Count();

                // do not show sent pending relationships
                relations = relations.Where(x => !(x.ByRelationShip == 3 && x.User1Id == myUserPid));

                //var relationsPage = relations.Skip(offset).Take(size).ToList();   // DO NOT apply pagination, it doesn't even work

                // prefer players that currently online
                // TODO: make a preference in Web UI
                if (result.uiTotalCount > 16)
                {
                    relations = relations.Where(x => onlinePlayerIds.Contains(x.User1Id == myUserPid ? x.User2Id : x.User1Id));
                    if (relations.Count() == 0)
                    {
                        result.lstRelationshipsList = new List<RelationshipData>()
                        {
                            new RelationshipData()
                            {
                                m_pid = 1,
                                m_strName = $"{result.uiTotalCount} friends",
                                m_byRelationship = 1,
                                m_byStatus = 0,
                                m_uiDetails = 0
                            },
                            new RelationshipData()
                            {
                                m_pid = 0,
                                m_strName = "Everyone is",
                                m_byRelationship = 1,
                                m_byStatus = 0,
                                m_uiDetails = 0
                            }
                        };
                        return Result(result);
                    }
                }

                var relationsPage = relations.ToArray();

                result.lstRelationshipsList = relationsPage.Select(x =>
                {
                    var swap = x.User1Id == myUserPid;
                    var res = new RelationshipData()
                    {
                        m_pid = swap ? x.User2Id : x.User1Id,
                        m_strName = swap ? x.User2.PlayerNickName : x.User1.PlayerNickName,
                        m_byStatus = (byte)(onlinePlayerIds.Contains(swap ? x.User2Id : x.User1Id) ? 1 : 0),
                        m_uiDetails = x.Details,
                        m_byRelationship = (byte)x.ByRelationShip
                    };
                    return res;
                });

            }

            return Result(result);
        }
    }
}
