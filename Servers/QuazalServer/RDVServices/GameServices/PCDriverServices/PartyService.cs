using QuazalServer.QNetZ.Attributes;
using QuazalServer.RDVServices.DDL.Models;
using QuazalServer.QNetZ.Interfaces;
using QuazalServer.QNetZ;

namespace QuazalServer.RDVServices.GameServices.PCDriverServices
{
    /// <summary>
    /// Hermes party service
    ///		Additional layer to the Match making service AND a game session
    /// </summary>
    [RMCService((ushort)RMCProtocolId.PartyService)]
    class PartyService : RMCServiceBase
    {
        [RMCMethod(1)]
        public RMCResult SendGameIdToParty(uint id, uint toJoinId, byte gameType, string msgRequest)
        {
            var gathering = PartySessions.GatheringList.FirstOrDefault(x => x.Session.m_idMyself == id);

            if (gathering != null)
            {
                foreach (var pid in gathering.Participants)
                {
                    var qclient = Context.Handler.GetQClientByClientPID(pid);

                    if (qclient != null)
                    {
                        var notification = new NotificationEvent(NotificationEventsType.HermesPartySession, 0)
                        {
                            m_pidSource = Context.Client.PlayerInfo.PID,
                            m_uiParam1 = toJoinId,
                            m_uiParam2 = gameType,
                            m_strParam = $"NetZHost:{msgRequest}",
                            m_uiParam3 = 0
                        };

                        NotificationQueue.SendNotification(Context.Handler, qclient, notification);
                    }
                }
            }
            else
            {
                CustomLogger.LoggerAccessor.LogError($"PartyService.SendGameIdToParty - no gathering with gid={id}");
            }

            return Error(0);
        }

        [RMCMethod(2)]
        public void SendGameIdToPlayerByName(string playerName, uint toJoinId, byte gameType, string msgRequest)
        {
            UNIMPLEMENTED();
        }

        [RMCMethod(3)]
        public RMCResult SendGameIdToPlayerByID(uint pid, uint toJoinId, byte gameType, string msgRequest)
        {
            // send to single client with PID only
            var qclient = Context.Handler.GetQClientByClientPID(pid);

            if (qclient != null)
            {
                var notification = new NotificationEvent(NotificationEventsType.HermesPartySession, 1)
                {
                    m_pidSource = Context.Client.PlayerInfo.PID,
                    m_uiParam1 = toJoinId,
                    m_uiParam2 = gameType,
                    m_strParam = $"NetZHost:{msgRequest}",
                    m_uiParam3 = 0
                };

                NotificationQueue.SendNotification(Context.Handler, qclient, notification);
            }

            return Error(0);
        }

        [RMCMethod(4)]
        public RMCResult NotifyPartyToLeaveGame(uint id)
        {
            // in party id send to all clients
            var gathering = PartySessions.GatheringList.FirstOrDefault(x => x.Session.m_idMyself == id);

            if (gathering != null)
            {
                foreach (var pid in gathering.Participants)
                {
                    var qclient = Context.Handler.GetQClientByClientPID(pid);

                    if (qclient != null)
                    {
                        var notification = new NotificationEvent(NotificationEventsType.HermesPartySession, 2)
                        {
                            m_pidSource = Context.Client.PlayerInfo.PID,
                            m_uiParam1 = 0,
                            m_uiParam2 = 0,
                            m_strParam = "NotifyPartyToLeaveGame",
                            m_uiParam3 = 0
                        };

                        NotificationQueue.SendNotification(Context.Handler, qclient, notification);
                    }
                }
            }
            else
            {
                CustomLogger.LoggerAccessor.LogError($"PartyService.NotifyPartyToLeaveGame - no gathering with gid={id}");
            }

            return Error(0);
        }

        [RMCMethod(5)]
        public void NotifyPlayerToLeavePartyByName(string playerName)
        {
            UNIMPLEMENTED();
        }

        [RMCMethod(6)]
        public void NotifyPlayerToLeavePartyByID(uint id)
        {
            UNIMPLEMENTED();
        }

        [RMCMethod(7)]
        public void SendNewPartyIdToParty(uint oldParty, uint newParty)
        {
            UNIMPLEMENTED();
        }

        [RMCMethod(8)]
        public RMCResult ShouldBecomeNewPartyHost(uint pid, uint partyId)
        {
            UNIMPLEMENTED();

            // newHostId = pid

            return Result(new { newHostId = 1u });
        }

        [RMCMethod(9)]
        public RMCResult PartyLeaderNetZIsValid(uint partyId, int param1, int param2)
        {
            var gathering = PartySessions.GatheringList.FirstOrDefault(x => x.Session.m_idMyself == partyId);

            if (gathering != null)
            {
                foreach (var pid in gathering.Participants)
                {
                    var qclient = Context.Handler.GetQClientByClientPID(pid);

                    if (qclient != null)
                    {
                        var notification = new NotificationEvent(NotificationEventsType.HermesPartySession, 4)
                        {
                            m_pidSource = Context.Client.PlayerInfo.PID,
                            m_uiParam1 = (uint)param1,
                            m_uiParam2 = (uint)param2,
                            m_strParam = "PartyLeaderNetZIsValid",
                            m_uiParam3 = 0
                        };

                        NotificationQueue.SendNotification(Context.Handler, qclient, notification);
                    }
                }
            }
            else
            {
                CustomLogger.LoggerAccessor.LogError($"PartyService.PartyLeaderNetZIsValid - no gathering with gid={partyId}");
            }

            return Error(0);
        }

        [RMCMethod(10)]
        public RMCResult QueryMatchmaking(uint toMatchmaking, uint fromParty, int nbPlayers, int applyMask)
        {
            // toMatchmaking - can be query ID?

            /*
			"notification": {
				"m_pidSource": 376135,		// pid
				"m_uiType": 1004007,
				"m_uiParam1": 39874,		// fromParty
				"m_uiParam2": 1,			// nbPlayers
				"m_strParam": "MM:22143|",  // toMatchmaking
				"m_uiParam3": 0				// applyMask ???
			}
			*/

            var session = GameSessions.SessionList.FirstOrDefault(x => x.Id == toMatchmaking);

            if (session != null)
            {
                // send to all players?
                // OR only to host?
                foreach (var pid in session.AllParticipants)
                {
                    var qclient = Context.Handler.GetQClientByClientPID(pid);

                    if (qclient != null)
                    {
                        var notification = new NotificationEvent(NotificationEventsType.HermesPartySession, 7)
                        {
                            m_pidSource = Context.Client.PlayerInfo.PID,
                            m_uiParam1 = fromParty,
                            m_uiParam2 = (uint)nbPlayers,
                            m_strParam = $"MM:{toMatchmaking}|",
                            m_uiParam3 = (uint)applyMask
                        };

                        NotificationQueue.SendNotification(Context.Handler, qclient, notification);
                    }
                }
            }
            else
            {
                CustomLogger.LoggerAccessor.LogError($"PartyService.QueryMatchmaking - no session with id={toMatchmaking}");
            }

            return Error(0);
        }

        [RMCMethod(11)]
        public RMCResult ResponseMatchmaking(uint toParty, uint fromMatchmaking, int approved)
        {
            // what this method should do?
            /*
			   "toParty": 39874,
			  "fromMatchmaking": 22143,
			  "approved": 1
			 */

            /*
			"notification": {
				"m_pidSource": 376135,
				"m_uiType": 1004008,
				"m_uiParam1": 22148,
				"m_uiParam2": 1,
				"m_strParam": "ResponseMatchmaking",
				"m_uiParam3": 0
			  }
			*/

            var gathering = PartySessions.GatheringList.FirstOrDefault(x => x.Session.m_idMyself == toParty);

            if (gathering != null)
            {
                foreach (var pid in gathering.Participants)
                {
                    var qclient = Context.Handler.GetQClientByClientPID(pid);

                    if (qclient != null)
                    {
                        NotificationEvent notification;
                        notification = new NotificationEvent(NotificationEventsType.HermesPartySession, 8)
                        {
                            m_pidSource = Context.Client.PlayerInfo.PID,
                            m_uiParam1 = fromMatchmaking,
                            m_uiParam2 = (uint)approved,
                            m_strParam = "ResponseMatchmaking",
                            m_uiParam3 = 0
                        };

                        NotificationQueue.SendNotification(Context.Handler, qclient, notification);
                    }
                }
            }
            else
            {
                CustomLogger.LoggerAccessor.LogError($"PartyService.ResponseMatchmaking - no gathering with gid={toParty}");
            }

            return Error(0);
        }

        [RMCMethod(12)]
        public RMCResult PartyProbeSessions(uint gid, uint pid, string packedSessions)
        {
            var gathering = PartySessions.GatheringList.FirstOrDefault(x => x.Session.m_idMyself == gid);

            if (gathering != null)
            {
                foreach (var participantPid in gathering.Participants)
                {
                    var qclient = Context.Handler.GetQClientByClientPID(participantPid);

                    if (qclient != null)
                    {
                        var notification = new NotificationEvent(NotificationEventsType.HermesPartySession, 3)
                        {
                            m_pidSource = Context.Client.PlayerInfo.PID,
                            m_uiParam1 = gid,
                            m_uiParam2 = pid,
                            m_strParam = packedSessions,
                            m_uiParam3 = 0
                        };

                        NotificationQueue.SendNotification(Context.Handler, qclient, notification);
                    }
                }
            }
            else
            {
                CustomLogger.LoggerAccessor.LogError($"PartyService.PartyProbeSessions - no gathering with gid={gid}");
            }

            return Error(0);

        }

        [RMCMethod(13)]
        public void PartyProbeSessionResults(uint gid, uint pid, string packedSessions)
        {
            UNIMPLEMENTED();
        }

        [RMCMethod(14)]
        public RMCResult SendMatchmakingStatus(uint gid, uint pid, uint gameType)
        {
            // Does pid == 0x30000 mean Search or Lobby?
            var gathering = PartySessions.GatheringList.FirstOrDefault(x => x.Session.m_idMyself == gid);

            if (gathering != null)
            {
                foreach (var participantPid in gathering.Participants)
                {
                    var qclient = Context.Handler.GetQClientByClientPID(participantPid);

                    if (qclient != null)
                    {
                        var notification = new NotificationEvent(NotificationEventsType.HermesPartySession, 6)
                        {
                            m_pidSource = Context.Client.PlayerInfo.PID,
                            m_uiParam1 = pid,
                            m_uiParam2 = gameType,
                            m_strParam = "",
                            m_uiParam3 = 0
                        };

                        NotificationQueue.SendNotification(Context.Handler, qclient, notification);
                    }
                }
            }
            else
            {
                CustomLogger.LoggerAccessor.LogError($"PartyService.SendMatchmakingStatus - no gathering with gid={gid}");
            }

            return Result(new { result = true });
        }

        [RMCMethod(15)]
        public RMCResult JoinMatchmakingStatus(uint gid, uint pid, bool joinSuccess)
        {
            var gathering = PartySessions.GatheringList.FirstOrDefault(x => x.Session.m_idMyself == gid);

            if (gathering != null)
            {
                foreach (var participantPid in gathering.Participants)
                {
                    // don't send back the notification
                    if (participantPid == Context.Client.PlayerInfo.PID)
                        continue;

                    var qclient = Context.Handler.GetQClientByClientPID(participantPid);

                    if (qclient != null)
                    {
                        NotificationEvent notification;
                        notification = new NotificationEvent(NotificationEventsType.PartyJoinMatchmaking, 0)
                        {
                            m_pidSource = Context.Client.PlayerInfo.PID,
                            m_uiParam1 = pid,
                            m_uiParam2 = (uint)(joinSuccess ? 1 : 0),
                            m_strParam = "JoinMatchmakingStatus",
                            m_uiParam3 = 0
                        };

                        NotificationQueue.SendNotification(Context.Handler, qclient, notification);
                    }
                }
            }
            else
            {
                CustomLogger.LoggerAccessor.LogError($"PartyService.JoinMatchmakingStatus - no gathering with gid={gid}");
            }

            return Result(new { result = true });
        }

        [RMCMethod(16)]
        public RMCResult PlayerShouldLeave(uint gid, uint pid)
        {
            UNIMPLEMENTED();
            return Result(new { result = false });
        }

        [RMCMethod(17)]
        public RMCResult IsPartyMemberActive(uint gid, uint pid)
        {
            UNIMPLEMENTED();
            return Result(new { isMemberActive = true });
        }

        [RMCMethod(18)]
        public RMCResult MigrateTo(uint pid, uint oldGathering, uint newGathering)
        {
            var plInfo = NetworkPlayers.GetPlayerInfoByPID(pid);

            if (plInfo != null)
            {
                if (plInfo.GameData().CurrentGatheringId != oldGathering)
                    CustomLogger.LoggerAccessor.LogWarn($"PartyService.MigrateTo - player {pid} old gathering is {plInfo.GameData().CurrentGatheringId}, expected {oldGathering}");

                PartySessions.UpdateGatheringParticipation(plInfo, newGathering);
            }

            return Error(0);
        }
    }
}
