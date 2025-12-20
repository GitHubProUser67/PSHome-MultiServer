using QuazalServer.RDVServices.DDL.Models;
using QuazalServer.QNetZ;
using QuazalServer.QNetZ.Attributes;
using QuazalServer.QNetZ.DDL;
using QuazalServer.QNetZ.Interfaces;
using QuazalServer.QNetZ.Connection;

namespace QuazalServer.RDVServices.GameServices.PCDriverServices
{
    /// <summary>
    /// Hermes match making service
    ///		Implements pre-game match making lobbies/gatherings (also known as Party buses)
    /// </summary>
    [RMCService((ushort)RMCProtocolId.MatchMakingService)]
    class MatchMakingService : RMCServiceBase
    {
        static uint GatheringIdCounter = 39000;
        static List<SentInvitation> InvitationList = new();

        [RMCMethod(1)]
        public RMCResult RegisterGathering(AnyData<HermesPartySession> anyGathering)
        {
            if (Context != null && Context.Client.PlayerInfo != null)
            {
                PlayerInfo? plInfo = Context.Client.PlayerInfo;
                uint playerPid = plInfo.PID;

                if (anyGathering.data != null)
                {
                    HermesPartySession gathering = anyGathering.data;
                    gathering.m_idMyself = ++GatheringIdCounter;
                    gathering.m_pidOwner = playerPid;

                    PartySessions.GatheringList.Add(new PartySessionGathering(gathering));

                    return Result(new { gatheringId = gathering.m_idMyself });
                }
            }

            return Error((uint)ErrorCode.RendezVous_DDLMismatch);
        }

        [RMCMethod(2)]
        public RMCResult UnregisterGathering(uint idGathering)
        {
            bool result = false;
            var gathering = PartySessions.GatheringList.FirstOrDefault(x => x.Session.m_idMyself == idGathering);

            if (gathering != null)
            {
                PartySessions.GatheringList.Remove(gathering);
                result = true;
            }
            else
                CustomLogger.LoggerAccessor.LogError($"MatchMakingService.UnregisterGathering - no gathering with gid={idGathering}");

            return Result(new { retVal = result });
        }

        [RMCMethod(3)]
        public void UnregisterGatherings(ICollection<uint> lstGatherings)
        {
            UNIMPLEMENTED();
        }

        [RMCMethod(4)]
        public RMCResult UpdateGathering(AnyData<HermesPartySession> anyGathering)
        {
            bool result = false;
            if (anyGathering.data != null)
            {
                var srcGathering = anyGathering.data;
                var gathering = PartySessions.GatheringList.FirstOrDefault(x => x.Session.m_idMyself == srcGathering.m_idMyself);

                if (gathering != null)
                {
                    gathering.Session.m_uiMinParticipants = srcGathering.m_uiMinParticipants;
                    gathering.Session.m_uiMaxParticipants = srcGathering.m_uiMaxParticipants;
                    gathering.Session.m_uiParticipationPolicy = srcGathering.m_uiParticipationPolicy;
                    gathering.Session.m_uiPolicyArgument = srcGathering.m_uiPolicyArgument;
                    gathering.Session.m_uiFlags = srcGathering.m_uiFlags;
                    gathering.Session.m_uiState = srcGathering.m_uiState;

                    gathering.Session.m_strDescription = srcGathering.m_strDescription;
                    gathering.Session.m_freePublicSlots = srcGathering.m_freePublicSlots;
                    gathering.Session.m_freePrivateSlots = srcGathering.m_freePrivateSlots;
                    gathering.Session.m_maxPrivateSlots = srcGathering.m_maxPrivateSlots;
                    gathering.Session.m_privacySettings = srcGathering.m_privacySettings;
                    gathering.Session.m_name = srcGathering.m_name;
                    gathering.Session.m_buffurizedOwnerId = srcGathering.m_buffurizedOwnerId;

                    // are notifications sent?

                    result = true;
                }
                else
                    CustomLogger.LoggerAccessor.LogError($"MatchMakingService.UpdateGathering - no gathering with gid={srcGathering.m_idMyself}");
            }

            return Result(new { retVal = result });
        }

        [RMCMethod(5)]
        public RMCResult Invite(uint idGathering, ICollection<uint> lstPrincipals, string strMessage)
        {
            bool result = false;
            if (Context != null && Context.Client.PlayerInfo != null)
            {
                PlayerInfo? plInfo = Context.Client.PlayerInfo;
                PartySessionGathering? gathering = PartySessions.GatheringList.FirstOrDefault(x => x.Session.m_idMyself == idGathering);

                if (gathering != null)
                {
                    var invitations = lstPrincipals.Select(x => new SentInvitation
                    {
                        SentById = plInfo.PID,
                        GatheringId = idGathering,
                        GuestId = x,
                        Message = strMessage
                    }).ToList();

                    // send notifications to invited user
                    foreach (var inv in invitations)
                    {
                        var qclient = Context.Handler.GetQClientByClientPID(inv.GuestId);

                        if (qclient != null)
                        {
                            var senderNotification = new NotificationEvent(NotificationEventsType.ParticipationEvent, 3)
                            {
                                m_pidSource = plInfo.PID,
                                m_uiParam1 = idGathering,
                                m_uiParam2 = plInfo.PID,
                                m_strParam = strMessage,
                                m_uiParam3 = 0
                            };

                            NotificationQueue.SendNotification(Context.Handler, qclient, senderNotification);
                        }
                    }

                    // remove old
                    InvitationList.RemoveAll(x => invitations.Any(y => x.GatheringId == y.GatheringId && x.GuestId == y.GuestId));

                    // add new
                    InvitationList.AddRange(invitations);

                    result = true;
                }
                else
                    CustomLogger.LoggerAccessor.LogError($"MatchMakingService.Invite - no gathering with gid={idGathering}");
            }

            return Result(new { retVal = result });
        }

        [RMCMethod(6)]
        public RMCResult AcceptInvitation(uint idGathering, string strMessage)
        {
            bool result = false;
            if (Context != null && Context.Client.PlayerInfo != null)
            {
                var plInfo = Context.Client.PlayerInfo;
                var gathering = PartySessions.GatheringList.FirstOrDefault(x => x.Session.m_idMyself == idGathering);
                var invitation = InvitationList.FirstOrDefault(x => x.GatheringId == idGathering && x.GuestId == plInfo.PID);

                if (invitation == null)
                {
                    CustomLogger.LoggerAccessor.LogError($"MatchMakingService.AcceptInvitation - no invitation found to gathering gid={idGathering} for PID={plInfo.PID}");
                    return Result(new { retVal = result });
                }

                if (gathering != null && gathering.Participants != null)
                {
                    // send notification to invitation sender
                    var qsender = Context.Handler.GetQClientByClientPID(invitation.SentById);

                    if (qsender != null)
                    {
                        // accepted invitation event
                        NotificationEvent senderNotification = new(NotificationEventsType.ParticipationEvent, 4)
                        {
                            m_pidSource = plInfo.PID,
                            m_uiParam1 = idGathering,
                            m_uiParam2 = plInfo.PID,
                            m_strParam = strMessage,
                            m_uiParam3 = 0
                        };

                        NotificationQueue.SendNotification(Context.Handler, qsender, senderNotification);
                    }

                    // should he be added?
                    gathering.Participants.Add(plInfo.PID);

                    // send to all party members
                    foreach (var pid in gathering.Participants)
                    {
                        QClient? qclient = Context.Handler.GetQClientByClientPID(pid);

                        if (qclient != null)
                        {
                            NotificationEvent notification = new(NotificationEventsType.ParticipationEvent, 1)
                            {
                                m_pidSource = plInfo.PID,
                                m_uiParam1 = idGathering,
                                m_uiParam2 = plInfo.PID,
                                m_strParam = strMessage,
                                m_uiParam3 = 0
                            };

                            NotificationQueue.SendNotification(Context.Handler, qclient, notification);
                        }
                    }

                    // done with it
                    InvitationList.Remove(invitation);

                    result = true;
                }
                else
                    CustomLogger.LoggerAccessor.LogError($"MatchMakingService.AcceptInvitation - no gathering with gid={idGathering}");
            }

            return Result(new { retVal = result });
        }

        [RMCMethod(7)]
        public RMCResult DeclineInvitation(uint idGathering, string strMessage)
        {
            bool result = false;
            if (Context != null && Context.Client.PlayerInfo != null)
            {
                var plInfo = Context.Client.PlayerInfo;
                var gathering = PartySessions.GatheringList.FirstOrDefault(x => x.Session.m_idMyself == idGathering);
                var invitation = InvitationList.FirstOrDefault(x => x.GatheringId == idGathering && x.GuestId == plInfo.PID);

                if (invitation == null)
                {
                    CustomLogger.LoggerAccessor.LogError($"MatchMakingService.DeclineInvitation - no invitation found to gathering gid={idGathering} for PID={plInfo.PID}");
                    return Result(new { retVal = result });
                }

                if (gathering == null)
                    CustomLogger.LoggerAccessor.LogWarn($"MatchMakingService.DeclineInvitation - no gathering with gid={idGathering}, removing invitation anyway");

                // send notification to invitation sender
                QClient? qsender = Context.Handler.GetQClientByClientPID(invitation.SentById);

                if (qsender != null)
                {
                    // decline invitation event
                    // is that correct?
                    NotificationEvent senderNotification = new(NotificationEventsType.ParticipationEvent, 2)
                    {
                        m_pidSource = plInfo.PID,
                        m_uiParam1 = idGathering,
                        m_uiParam2 = plInfo.PID,
                        m_strParam = strMessage,
                        m_uiParam3 = 0
                    };

                    NotificationQueue.SendNotification(Context.Handler, qsender, senderNotification);
                }

                // done with it
                InvitationList.Remove(invitation);
            }

            return Result(new { retVal = result });
        }

        [RMCMethod(8)]
        public void CancelInvitation()
        {
            UNIMPLEMENTED();
        }

        [RMCMethod(9)]
        public RMCResult GetInvitationsSent(uint idGathering)
        {
            if (Context != null && Context.Client.PlayerInfo != null)
            {
                var plInfo = Context.Client.PlayerInfo;
                var myUserPid = plInfo.PID;
                var list = InvitationList
                    .Where(x => x.GatheringId == idGathering && x.SentById == myUserPid)
                    .Select(x => new Invitation()
                    {
                        m_idGathering = x.GatheringId,
                        m_idGuest = x.GuestId,
                        m_strMessage = x.Message
                    });

                return Result(list);
            }

            return Error(0);
        }

        [RMCMethod(10)]
        public RMCResult GetInvitationsReceived()
        {
            if (Context != null && Context.Client.PlayerInfo != null)
            {
                PlayerInfo? plInfo = Context.Client.PlayerInfo;
                uint myUserPid = plInfo.PID;
                var list = InvitationList
                    .Where(x => x.GuestId == myUserPid)
                    .Select(x => new Invitation()
                    {
                        m_idGathering = x.GatheringId,
                        m_idGuest = x.GuestId,
                        m_strMessage = x.Message
                    });

                return Result(list);
            }

            return Error(0);
        }

        [RMCMethod(11)]
        public RMCResult Participate(uint idGathering, string strMessage)
        {
            bool result = false;
            if (Context != null && Context.Client.PlayerInfo != null)
            {
                var plInfo = Context.Client.PlayerInfo;
                var gathering = PartySessions.GatheringList.FirstOrDefault(x => x.Session.m_idMyself == idGathering);

                if (gathering != null)
                {
                    PartySessions.UpdateGatheringParticipation(plInfo, idGathering);
                    result = true;
                }
                else
                    CustomLogger.LoggerAccessor.LogError($"MatchMakingService.Participate - no gathering with gid={idGathering}");
            }

            return Result(new { retVal = result });
        }

        [RMCMethod(12)]
        public RMCResult CancelParticipation(uint idGathering, string strMessage)
        {
            bool result = false;
            if (Context != null && Context.Client.PlayerInfo != null)
            {
                var plInfo = Context.Client.PlayerInfo;
                var gathering = PartySessions.GatheringList.FirstOrDefault(x => x.Session.m_idMyself == idGathering);

                if (gathering != null && gathering.Participants != null)
                {
                    // NOTE: Disabled gathering dropping
                    // PartySessions.UpdateGatheringParticipation(plInfo, uint.MaxValue);

                    // send to all party members
                    foreach (var pid in gathering.Participants)
                    {
                        if (pid == plInfo.PID)
                            continue;

                        var qclient = Context.Handler.GetQClientByClientPID(pid);

                        if (qclient != null)
                        {
                            NotificationEvent leaveNotification = new(NotificationEventsType.ParticipationEvent, 2)
                            {
                                m_pidSource = plInfo.PID,
                                m_uiParam1 = idGathering,
                                m_uiParam2 = plInfo.PID,
                                m_strParam = strMessage,
                                m_uiParam3 = 0
                            };

                            NotificationQueue.SendNotification(Context.Handler, qclient, leaveNotification);
                        }
                    }

                    result = true;
                }
                else
                {
                    CustomLogger.LoggerAccessor.LogError($"MatchMakingService.CancelParticipation - no gathering with gid={idGathering}");
                }
            }

            return Result(new { retVal = result });
        }

        [RMCMethod(13)]
        public void GetParticipants()
        {
            UNIMPLEMENTED();
        }

        [RMCMethod(14)]
        public void AddParticipants()
        {
            UNIMPLEMENTED();
        }

        [RMCMethod(15)]
        public void GetDetailedParticipants()
        {
            UNIMPLEMENTED();
        }

        [RMCMethod(16)]
        public void GetParticipantsURLs()
        {
            UNIMPLEMENTED();
        }

        [RMCMethod(17)]
        public void FindByType()
        {
            UNIMPLEMENTED();
        }

        [RMCMethod(18)]
        public void FindByDescription()
        {
            UNIMPLEMENTED();
        }

        [RMCMethod(19)]
        public void FindByDescriptionRegex()
        {
            UNIMPLEMENTED();
        }

        [RMCMethod(20)]
        public void FindByID()
        {
            UNIMPLEMENTED();
        }

        [RMCMethod(21)]
        public RMCResult FindBySingleID(uint id)
        {
            bool result = false;
            var gathering = PartySessions.GatheringList.FirstOrDefault(x => x.Session.m_idMyself == id);

            if (gathering != null)
                result = true;
            else
                gathering = new PartySessionGathering();

            return Result(new { bResult = result, pGathering = new AnyData<HermesPartySession>(gathering.Session) });
        }

        [RMCMethod(22)]
        public void FindByOwner()
        {
            UNIMPLEMENTED();
        }

        [RMCMethod(23)]
        public void FindByParticipants()
        {
            UNIMPLEMENTED();
        }

        [RMCMethod(24)]
        public void FindInvitations()
        {
            UNIMPLEMENTED();
        }

        [RMCMethod(25)]
        public void FindBySQLQuery()
        {
            UNIMPLEMENTED();
        }

        [RMCMethod(26)]
        public void LaunchSession()
        {
            UNIMPLEMENTED();
        }

        [RMCMethod(27)]
        public void UpdateSessionURL()
        {
            UNIMPLEMENTED();
        }

        [RMCMethod(28)]
        public void GetSessionURL()
        {
            UNIMPLEMENTED();
        }

        [RMCMethod(29)]
        public void GetState()
        {
            UNIMPLEMENTED();
        }

        [RMCMethod(30)]
        public void SetState()
        {
            UNIMPLEMENTED();
        }

        [RMCMethod(31)]
        public void ReportStats()
        {
            UNIMPLEMENTED();
        }

        [RMCMethod(32)]
        public void GetStats()
        {
            UNIMPLEMENTED();
        }

        [RMCMethod(33)]
        public void DeleteGathering()
        {
            UNIMPLEMENTED();
        }

        [RMCMethod(34)]
        public void GetPendingDeletions()
        {
            UNIMPLEMENTED();
        }

        [RMCMethod(35)]
        public void DeleteFromDeletions()
        {
            UNIMPLEMENTED();
        }

        [RMCMethod(36)]
        public RMCResult MigrateGatheringOwnership(uint gid, IEnumerable<uint> lstPotentialNewOwnersID)
        {
            bool result = false;
            UNIMPLEMENTED();
            return Result(new { retVal = result });
        }

        [RMCMethod(37)]
        public void FindByDescriptionLike()
        {
            UNIMPLEMENTED();
        }

        [RMCMethod(38)]
        public RMCResult RegisterLocalURL(uint gid, StationURL url)
        {
            UNIMPLEMENTED();
            return Error(0);
        }

        [RMCMethod(39)]
        public RMCResult RegisterLocalURLs(uint gid, IEnumerable<StationURL> lstUrls)
        {
            var gathering = PartySessions.GatheringList.FirstOrDefault(x => x.Session.m_idMyself == gid);

            if (gathering != null && gathering.Urls != null)
            {
                var newUrls = lstUrls.Where(x => !gathering.Urls.Any(u => u.urlString == x.urlString));

                gathering.Urls.Clear();
                gathering.Urls.AddRange(newUrls);
            }
            else
                CustomLogger.LoggerAccessor.LogError($"MatchMakingService.RegisterLocalURLs - no gathering with gid={gid}");

            return Error(0);
        }

        [RMCMethod(40)]
        public RMCResult UpdateSessionHost(uint gid)
        {
            if (Context != null && Context.Client.PlayerInfo != null)
            {
                PlayerInfo? plInfo = Context.Client.PlayerInfo;
                var gathering = PartySessions.GatheringList.FirstOrDefault(x => x.Session.m_idMyself == gid);

                if (gathering != null)
                    gathering.Session.m_pidHost = plInfo.PID;
                else
                    CustomLogger.LoggerAccessor.LogError($"MatchMakingService.UpdateSessionHost - no gathering with gid={gid}");
            }

            return Error(0);
        }

        [RMCMethod(41)]
        public RMCResult GetSessionURLs(uint gid)
        {
            var gathering = PartySessions.GatheringList.FirstOrDefault(x => x.Session.m_idMyself == gid);

            if (gathering != null)
                return Result(gathering.Urls);
            else
                CustomLogger.LoggerAccessor.LogError($"MatchMakingService.GetSessionURLs - no gathering with gid={gid}");

            return Error(0);
        }

        [RMCMethod(42)]
        public void UpdateSessionHost()
        {
            UNIMPLEMENTED();
        }

        [RMCMethod(43)]
        public void UpdateGatheringOwnership()
        {
            UNIMPLEMENTED();
        }

        [RMCMethod(44)]
        public void MigrateGatheringOwnership()
        {
            UNIMPLEMENTED();
        }
    }
}
