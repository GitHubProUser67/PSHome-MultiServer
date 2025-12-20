using QuazalServer.QNetZ;
using QuazalServer.QNetZ.Attributes;
using QuazalServer.QNetZ.DDL;
using QuazalServer.QNetZ.Interfaces;
using QuazalServer.RDVServices.DDL.Models;
using QuazalServer.RDVServices.RMC;
using System.Net;

namespace QuazalServer.RDVServices.GameServices.v2Services
{
    [RMCService((ushort)RMCProtocolId.NATTraversalService)]
    public class NATTraversalService : RMCServiceBase
    {
        [RMCMethod(1)]
        public RMCResult RequestProbeInitiation(IEnumerable<StationURL> urlTargetList)
        {
            // urlTargetList contains all player urls (basicmcnally given by MatchMakingService.GetSessionURLs)
            // Server sends InitiateProbe to all players in that url with those URLs
            // Then clients communicate with each other...
            foreach (StationURL? urlTarget in urlTargetList)
            {
                QClient? qclient = Context?.Handler.GetQClientByEndPoint(new(IPAddress.Parse(urlTarget.Address), urlTarget.Parameters["port"]));

                if (qclient != null)
                {
                    QClient? thisClient = Context?.Client;
                    if (thisClient != null && thisClient.PlayerInfo != null)
                    {
                        StationURL thisClientURL = new(
                            "prudp",
                            thisClient.Endpoint.Address.ToString(),
                            new Dictionary<string, int>() {
                                { "port", thisClient.Endpoint.Port },
                                { "RVCID", (int)thisClient.PlayerInfo.RVCID }
                            });

                        SendRMCCall(qclient, (ushort)RMCProtocolId.NATTraversalService, 2, thisClientURL);
                    }
                }
            }

            return Error(0);
        }

        [RMCMethod(2)]
        public RMCResult InitiateProbe(StationURL urlStationToProbe)
        {
            UNIMPLEMENTED();
            return Error(0);
        }

        [RMCMethod(3)]
        public RMCResult RequestProbeInitiationExt(IEnumerable<StationURL> urlTargetList, StationURL urlStationToProbe)
        {
            UNIMPLEMENTED();
            return Error(0);
        }

        [RMCMethod(4)]
        public RMCResult ReportNATTraversalResult(uint cid, bool result)
        {
            UNIMPLEMENTED();
            return Error(0);
        }

        [RMCMethod(5)]
        public RMCResult ReportNATProperties(uint natmapping, uint natfiltering, uint rtt)
        {
            UNIMPLEMENTED();
            return Error(0);
        }

        [RMCMethod(6)]
        public RMCResult GetRelaySignatureKey()
        {
            UNIMPLEMENTED();
            return Result(new RelaySignatureKey());
        }

        [RMCMethod(7)]
        public RMCResult ReportNATTraversalResultDetail(uint cid, bool result, int detail, uint rtt)
        {
            UNIMPLEMENTED();
            return Error(0);
        }

    }
}
