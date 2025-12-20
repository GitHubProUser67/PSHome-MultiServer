using QuazalServer.QNetZ.Attributes;
using QuazalServer.QNetZ.Interfaces;
using QuazalServer.RDVServices.DDL.Models;
using QuazalServer.QNetZ;
using QuazalServer.RDVServices.RMC;

namespace QuazalServer.RDVServices.GameServices.PCDriverServices
{
    [RMCService((ushort)RMCProtocolId.RichPresenceService)]
    public class RichPresenceService : RMCServiceBase
    {
        [RMCMethod(1)]
        public RMCResult SetPresence(int phraseId, QNetZ.DDL.qBuffer argument)
        {
            if (Context != null && Context.Client.PlayerInfo != null)
            {
                PlayerInfo? plInfo = Context.Client.PlayerInfo;
                PresenceElement? presence = plInfo.GameData().CurrentPresence;

                //QLog.WriteLine(1, $"Presence set to {phraseId}, {argument.data}");

                if (presence == null)
                {
                    presence = new();
                    plInfo.GameData().CurrentPresence = presence;
                }

                presence.principalId = plInfo.PID;
                presence.phraseId = phraseId;
                presence.argument = argument;
                presence.isConnected = true;
            }

            return Error(0);
        }

        [RMCMethod(2)]
        public RMCResult GetPresence(IEnumerable<uint> pids)
        {
            var presenceResult = new List<PresenceElement>();

            foreach (uint principalId in pids)
            {
                PlayerInfo? playerInfo = NetworkPlayers.GetPlayerInfoByPID(principalId);
                if (playerInfo != null && playerInfo.GameData().CurrentPresence != null)
                    presenceResult.Add(playerInfo.GameData().CurrentPresence);
                else
                {
                    presenceResult.Add(new PresenceElement()
                    {
                        phraseId = 2,
                        isConnected = false,
                        principalId = principalId,
                        argument = new QNetZ.DDL.qBuffer()
                    });
                }
            }

            return Result(presenceResult);
        }
    }
}
