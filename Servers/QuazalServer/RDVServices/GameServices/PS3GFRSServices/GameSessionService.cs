using QuazalServer.RDVServices.DDL.Models;
using QuazalServer.QNetZ;
using QuazalServer.QNetZ.Attributes;
using QuazalServer.QNetZ.DDL;
using QuazalServer.QNetZ.Interfaces;
using QuazalServer.RDVServices.RMC;

namespace QuazalServer.RDVServices.GameServices.PS3GFRSServices
{
    /// <summary>
    /// Game session 
    ///		Implements the sessions responsible for the gameplay process
    /// </summary>
    [RMCService((ushort)RMCProtocolId.GameSessionService)]
    public class GameSessionService : RMCServiceBase
    {
        static uint GameSessionCounter = 22110;

        [RMCMethod(1)]
        public RMCResult CreateSession(GameSession gameSession)
        {
            if (Context != null && Context.Client.PlayerInfo != null)
            {
                PlayerInfo? plInfo = Context.Client.PlayerInfo;
                GameSessionData newSession = new();
                GameSessions.SessionList.Add(newSession);

                newSession.Id = ++GameSessionCounter;
                newSession.HostPID = plInfo.PID;
                newSession.TypeID = gameSession.m_typeID;

                foreach (var attr in gameSession.m_attributes)
                    newSession.Attributes[attr.ID] = attr.Value;

                uint temp;
                if (!newSession.Attributes.TryGetValue((uint)GameSessionAttributeType.PublicSlots, out temp))
                    newSession.Attributes[(uint)GameSessionAttributeType.PublicSlots] = 0;

                if (!newSession.Attributes.TryGetValue((uint)GameSessionAttributeType.PrivateSlots, out temp))
                    newSession.Attributes[(uint)GameSessionAttributeType.PrivateSlots] = 8;

                if (!newSession.Attributes.TryGetValue((uint)GameSessionAttributeType.GameType, out temp))
                    newSession.Attributes[(uint)GameSessionAttributeType.GameType] = (uint)GameType.FreeForAll;

                newSession.Attributes[(uint)GameSessionAttributeType.FilledPublicSlots] = (uint)newSession.PublicParticipants.Count;
                newSession.Attributes[(uint)GameSessionAttributeType.FilledPrivateSlots] = (uint)newSession.Participants.Count;

                // TODO: give names to attributes
                if (!newSession.Attributes.TryGetValue(100, out temp))
                    newSession.Attributes[100] = 0;

                if (!newSession.Attributes.TryGetValue(101, out temp))
                    newSession.Attributes[101] = 0;

                if (!newSession.Attributes.TryGetValue(104, out temp))
                    newSession.Attributes[104] = 0;

                if (!newSession.Attributes.TryGetValue(113, out temp))
                    newSession.Attributes[113] = 0;

                // return key
                var result = new GameSessionKey();
                result.m_sessionID = newSession.Id;
                result.m_typeID = newSession.TypeID;

                return Result(result);
            }

            return Error(0);
        }

        [RMCMethod(2)]
        public RMCResult UpdateSession(GameSessionUpdate gameSessionUpdate)
        {
            if (gameSessionUpdate.m_sessionKey != null)
            {
                var session = GameSessions.SessionList
                .FirstOrDefault(x => x.Id == gameSessionUpdate.m_sessionKey.m_sessionID &&
                                     x.TypeID == gameSessionUpdate.m_sessionKey.m_typeID);

                if (session != null && gameSessionUpdate.m_attributes != null)
                {
                    // update or add attributes
                    foreach (var attr in gameSessionUpdate.m_attributes)
                    {
                        session.Attributes[attr.ID] = attr.Value;
                    }
                }
                else
                    CustomLogger.LoggerAccessor.LogError($"GameSessionService.UpdateSession - no session with id={gameSessionUpdate.m_sessionKey.m_sessionID}");
            }

            return Error(0);
        }


        [RMCMethod(3)]
        public RMCResult DeleteSession(GameSessionKey gameSessionKey)
        {
            UNIMPLEMENTED();
            return Error(0);
        }


        [RMCMethod(4)]
        public RMCResult MigrateSession(GameSessionKey gameSessionKey)
        {
            var oldSession = GameSessions.SessionList
                .FirstOrDefault(x => x.Id == gameSessionKey.m_sessionID &&
                                     x.TypeID == gameSessionKey.m_typeID);
            if (oldSession == null)
            {
                CustomLogger.LoggerAccessor.LogError($"GameSessionService.MigrateSession - no session with id={gameSessionKey.m_sessionID}");
                return Result(new GameSessionKey());
            }

            // ????
            // "notification": {
            // 	"m_pidSource": 539625,
            // 	"m_uiType": 7001,
            // 	"m_uiParam1": 31,
            // 	"m_uiParam2": 30,
            // 	"m_strParam": "",
            // 	"m_uiParam3": 1
            //   }

            var newSession = new GameSessionData();
            GameSessions.SessionList.Add(newSession);

            newSession.Id = ++GameSessionCounter;
            newSession.HostPID = Context.Client.PlayerInfo.PID;
            newSession.TypeID = oldSession.TypeID;
            newSession.Participants = oldSession.Participants;
            newSession.PublicParticipants = oldSession.PublicParticipants;

            foreach (var attr in oldSession.Attributes)
                newSession.Attributes[attr.Key] = attr.Value;

            var newSessionKey = new GameSessionKey();
            newSessionKey.m_sessionID = newSession.Id;
            newSessionKey.m_typeID = newSession.TypeID;

            // move all participants (change session key)
            foreach (var pid in oldSession.PublicParticipants)
            {
                var participantPlayerInfo = NetworkPlayers.GetPlayerInfoByPID(pid);

                if (participantPlayerInfo != null)
                    participantPlayerInfo.GameData().CurrentSession = newSessionKey;
            }

            foreach (var pid in oldSession.Participants)
            {
                var participantPlayerInfo = NetworkPlayers.GetPlayerInfoByPID(pid);

                if (participantPlayerInfo != null)
                    participantPlayerInfo.GameData().CurrentSession = newSessionKey;
            }

            // drop old session
            CustomLogger.LoggerAccessor.LogWarn($"MigrateSession - Auto-deleted session {oldSession.Id}");
            GameSessions.SessionList.Remove(oldSession);

            return Result(newSessionKey);
        }


        [RMCMethod(5)]
        public RMCResult LeaveSession(GameSessionKey gameSessionKey)
        {
            if (Context != null && Context.Client.PlayerInfo != null)
            {
                // Same as AbandonSession
                PlayerInfo? playerInfo = Context.Client.PlayerInfo;
                uint myPlayerId = playerInfo.PID;
                GameSessionData? session = GameSessions.SessionList
                    .FirstOrDefault(x => x.Id == gameSessionKey.m_sessionID &&
                                         x.TypeID == gameSessionKey.m_typeID);

                if (session != null)
                {
                    // send - could be invalid!!!
                    //{
                    //  "notification": {
                    //	"m_pidSource": 25447,	// ???
                    //	"m_uiType": 7004,		// GameSessionEvent
                    //	"m_uiParam1": 539625,	// participantID
                    //	"m_uiParam2": 27,		// gameSessionKey.m_sessionID
                    //	"m_strParam": "",
                    //	"m_uiParam3": 1			// gameSessionKey.m_typeID ??? not sure...
                    //  }
                    //}

                    // send to all session members
                    foreach (var pid in session.Participants)
                    {
                        var qclient = Context.Handler.GetQClientByClientPID(pid);

                        if (qclient != null)
                        {
                            var leaveNotification = new NotificationEvent(NotificationEventsType.GameSessionEvent, 4)
                            {
                                m_pidSource = playerInfo.PID,
                                m_uiParam1 = playerInfo.PID,
                                m_uiParam2 = session.Id,
                                m_strParam = string.Empty,
                                m_uiParam3 = session.TypeID
                            };

                            NotificationQueue.SendNotification(Context.Handler, qclient, leaveNotification);
                        }
                    }

                    GameSessions.UpdateSessionParticipation(playerInfo, null, false);
                }
                else
                    CustomLogger.LoggerAccessor.LogError($"GameSessionService.LeaveSession - no session with id={gameSessionKey.m_sessionID}");
            }

            return Error(0);
        }


        [RMCMethod(6)]
        public RMCResult GetSession(GameSessionKey gameSessionKey)
        {
            GameSessionSearchResult? searchResult = new();

            GameSessionData? session = GameSessions.SessionList.FirstOrDefault(x => x.Id == gameSessionKey.m_sessionID && x.TypeID == gameSessionKey.m_typeID);

            if (session != null)
            {
                searchResult = new GameSessionSearchResult()
                {
                    m_hostPID = session.HostPID,
                    m_hostURLs = session.HostURLs,
                    m_attributes = session.Attributes.Select(x => new GameSessionProperty { ID = x.Key, Value = x.Value }).ToArray(),
                    m_sessionKey = new GameSessionKey()
                    {
                        m_sessionID = session.Id,
                        m_typeID = session.TypeID
                    }
                };
            }

            return Result(searchResult);
        }

        [RMCMethod(7)]
        public RMCResult SearchSessions(uint m_typeID, uint m_queryID, IEnumerable<GameSessionProperty> m_parameters)
        {
            var sessions = GameSessions.SessionList.Where(x => x.TypeID == m_typeID).ToArray();

            var resultList = new List<GameSessionSearchResult>();

            foreach (GameSessionData ses in sessions)
            {
                uint value;

                // cut out *private* sessions completely
                if (ses.Attributes.TryGetValue((uint)GameSessionAttributeType.FreePrivateSlots, out value) && value > 0 ||
                    ses.Attributes.TryGetValue((uint)GameSessionAttributeType.PrivateSlots, out value) && value > 0 ||
                    ses.Attributes.TryGetValue((uint)GameSessionAttributeType.FilledPrivateSlots, out value) && value > 0)
                    continue;

                GameSessionProperty? gameTypeMinParam = m_parameters.FirstOrDefault(x => x.ID == (uint)GameSessionAttributeType.GameTypeMin);
                GameSessionProperty? gameTypeMaxParam = m_parameters.FirstOrDefault(x => x.ID == (uint)GameSessionAttributeType.GameTypeMax);
                GameSessionProperty? totalPublicSlotsParam = m_parameters.FirstOrDefault(x => x.ID == (uint)GameSessionAttributeType.PublicSlots);

                uint sessionGameType = ses.Attributes[(uint)GameSessionAttributeType.GameType];

                // check game mode matches criteria
                // and if there are free slots
                if (gameTypeMinParam != null && gameTypeMaxParam != null && totalPublicSlotsParam != null && sessionGameType >= gameTypeMinParam.Value && sessionGameType <= gameTypeMaxParam.Value &&
                    ses.PublicParticipants.Count < totalPublicSlotsParam.Value)
                {
                    resultList.Add(new GameSessionSearchResult()
                    {
                        m_hostPID = ses.HostPID,
                        m_hostURLs = ses.HostURLs,
                        m_attributes = ses.Attributes.Select(x => new GameSessionProperty { ID = x.Key, Value = x.Value }).ToArray(),
                        m_sessionKey = new GameSessionKey()
                        {
                            m_sessionID = ses.Id,
                            m_typeID = ses.TypeID
                        },
                    });
                }
            }

            return Result(resultList);
        }

        [RMCMethod(8)]
        public RMCResult AddParticipants(GameSessionKey gameSessionKey, IEnumerable<uint> publicParticipantIDs, IEnumerable<uint> privateParticipantIDs)
        {
            GameSessionData? session = GameSessions.SessionList.FirstOrDefault(x => x.IsMatchingKey(gameSessionKey));

            if (session != null)
            {
                foreach (var pid in publicParticipantIDs)
                {
                    session.PublicParticipants.Add(pid);

                    var player = NetworkPlayers.GetPlayerInfoByPID(pid);
                    if (player != null)
                    {
                        GameSessions.UpdateSessionParticipation(player, gameSessionKey, false);
                    }
                }

                foreach (var pid in privateParticipantIDs)
                {
                    session.Participants.Add(pid);

                    var player = NetworkPlayers.GetPlayerInfoByPID(pid);
                    if (player != null)
                    {
                        GameSessions.UpdateSessionParticipation(player, gameSessionKey, true);
                    }
                }

                session.Attributes[(uint)GameSessionAttributeType.FilledPublicSlots] = (uint)session.PublicParticipants.Count;
                session.Attributes[(uint)GameSessionAttributeType.FilledPrivateSlots] = (uint)session.Participants.Count;
            }
            else
                CustomLogger.LoggerAccessor.LogError($"GameSessionService.AddParticipants - no session with id={gameSessionKey.m_sessionID}");

            return Error(0);
        }


        [RMCMethod(9)]
        public RMCResult RemoveParticipants(GameSessionKey gameSessionKey, IEnumerable<uint> participantIDs)
        {
            GameSessionData? session = GameSessions.SessionList.FirstOrDefault(x => x.IsMatchingKey(gameSessionKey));

            if (session != null)
            {
                // TODO: send
                //{
                //  "notification": {
                //	"m_pidSource": 25447,	// ???
                //	"m_uiType": 7004,		// GameSessionEvent
                //	"m_uiParam1": 539625,	// participantID
                //	"m_uiParam2": 27,		// gameSessionKey.m_sessionID
                //	"m_strParam": "",
                //	"m_uiParam3": 1			// gameSessionKey.m_typeID
                //  }
                //}

                foreach (var pid in participantIDs)
                {
                    var player = NetworkPlayers.GetPlayerInfoByPID(pid);
                    if (player != null)
                        GameSessions.UpdateSessionParticipation(player, null, false);
                    else if (GameSessions.RemovePlayerFromSession(session, pid))
                    {
                        CustomLogger.LoggerAccessor.LogWarn($"RemoveParticipants - Auto-deleted session {session.Id}");
                        GameSessions.SessionList.Remove(session);
                    }
                }

                foreach (var pid in participantIDs)
                {
                    var player = NetworkPlayers.GetPlayerInfoByPID(pid);
                    if (player != null)
                        GameSessions.UpdateSessionParticipation(player, null, false);
                    else if (GameSessions.RemovePlayerFromSession(session, pid))
                    {
                        CustomLogger.LoggerAccessor.LogWarn($"RemoveParticipants - Auto-deleted session {session.Id}");
                        GameSessions.SessionList.Remove(session);
                    }
                }

                session.Attributes[(uint)GameSessionAttributeType.FilledPublicSlots] = (uint)session.PublicParticipants.Count;
                session.Attributes[(uint)GameSessionAttributeType.FilledPrivateSlots] = (uint)session.Participants.Count;
            }
            else
                CustomLogger.LoggerAccessor.LogError($"GameSessionService.RemoveParticipants - no session with id={gameSessionKey.m_sessionID}");

            return Error(0);
        }

        [RMCMethod(10)]
        public RMCResult GetParticipantCount(GameSessionKey gameSessionKey, IEnumerable<uint> participantIDs)
        {
            UNIMPLEMENTED();
            return Error(0);
        }

        [RMCMethod(11)]
        public void GetParticipants()
        {
            UNIMPLEMENTED();
        }

        [RMCMethod(12)]
        public void SendInvitation()
        {
            UNIMPLEMENTED();
        }

        [RMCMethod(13)]
        public void GetInvitationReceivedCount()
        {
            UNIMPLEMENTED();
        }

        [RMCMethod(14)]
        public void GetInvitationsReceived()
        {
            UNIMPLEMENTED();
        }

        [RMCMethod(15)]
        public void GetInvitationSentCount()
        {
            UNIMPLEMENTED();
        }

        [RMCMethod(16)]
        public void GetInvitationsSent()
        {
            UNIMPLEMENTED();
        }

        [RMCMethod(17)]
        public void AcceptInvitation()
        {
            UNIMPLEMENTED();
        }

        [RMCMethod(18)]
        public void DeclineInvitation()
        {
            UNIMPLEMENTED();
        }

        [RMCMethod(19)]
        public void CancelInvitation()
        {
            UNIMPLEMENTED();
        }

        [RMCMethod(20)]
        public void SendTextMessage()
        {
            UNIMPLEMENTED();
        }

        [RMCMethod(21)]
        public RMCResult RegisterURLs(IEnumerable<StationURL> stationURLs)
        {
            if (Context != null && Context.Client.PlayerInfo != null)
            {
                PlayerInfo? plInfo = Context.Client.PlayerInfo;
                uint myPlayerId = plInfo.PID;
                GameSessionData? session = GameSessions.SessionList.FirstOrDefault(x => x.HostPID == myPlayerId);

                if (session != null)
                {
                    session.HostURLs.Clear();
                    session.HostURLs.AddRange(stationURLs);
                }
                else
                    CustomLogger.LoggerAccessor.LogError($"GameSessionService.RegisterURLs - no session hosted by pid={myPlayerId}");
            }

            return Error(0);
        }

        [RMCMethod(22)]
        public void JoinSession()
        {
            UNIMPLEMENTED();
        }

        [RMCMethod(23)]
        public RMCResult AbandonSession(GameSessionKey gameSessionKey)
        {
            return LeaveSession(gameSessionKey);
        }

        [RMCMethod(24)]
        public void SearchSessionsWithParticipants()
        {
            UNIMPLEMENTED();
        }

        [RMCMethod(25)]
        public void GetSessions()
        {
            UNIMPLEMENTED();
        }

        [RMCMethod(26)]
        public void GetParticipantsURLs()
        {
            UNIMPLEMENTED();
        }

        [RMCMethod(27)]
        public void MigrateSessionHost()
        {
            UNIMPLEMENTED();
        }

        [RMCMethod(28)]
        public void SplitSession()
        {
            UNIMPLEMENTED();
        }

        [RMCMethod(29)]
        public void SearchSocialSessions()
        {
            UNIMPLEMENTED();
        }

        [RMCMethod(30)]
        public void ReportUnsuccessfulJoinSessions()
        {
            UNIMPLEMENTED();
        }
    }
}
