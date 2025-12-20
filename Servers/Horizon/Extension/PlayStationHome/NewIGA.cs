using Horizon.DME;
using Horizon.DME.Models;

namespace Horizon.Extension.Extension.PlayStationHome
{
    public class NewIGA
    {
		// This was used as an attack, so we handle it server side preferably (except admins), see https://www.youtube.com/watch?v=lBo0eMH4GAo at 1.52.39 for more details.
        private static readonly byte[] KickCMD = new byte[] { 0x02, 0x0B, 0x00, 0x01, 0x00, 0x10, 0x64, 0x00, 0x00, 0x0B, 0xFF, 0xFF, 0xFF, 0xAB, 0xFF, 0xFF, 0xFF, 0xFF, 0x30, 0x32, 0x00, 0x00 };
        private static readonly byte[] ReleaseCMD = new byte[] { 0x02, 0x0B, 0x00, 0x01, 0x00, 0x10, 0x64, 0x00, 0x00, 0x0B, 0xFF, 0xFF, 0xFF, 0xAB, 0xFF, 0xFF, 0xFF, 0xFF, 0x30, 0x37, 0x00, 0x00 };
        private static readonly byte[] MuteCMD = new byte[] { 0x02, 0x0B, 0x00, 0x01, 0x00, 0x10, 0x64, 0x00, 0x00, 0x0c, 0xFF, 0xFF, 0xFF, 0xAB, 0xFF, 0xFF, 0xFF, 0xFF, 0x30, 0x37, 0x03, 0x00 };
        private static readonly byte[] MuteNFreezeCMD = new byte[] { 0x02, 0x0B, 0x00, 0x01, 0x00, 0x10, 0x64, 0x00, 0x00, 0x0c, 0xFF, 0xFF, 0xFF, 0xAB, 0xFF, 0xFF, 0xFF, 0xFF, 0x30, 0x37, 0x02, 0x00 };
        private static readonly byte[] FreezeCMD = new byte[] { 0x02, 0x0B, 0x00, 0x01, 0x00, 0x10, 0x64, 0x00, 0x00, 0x0c, 0xFF, 0xFF, 0xFF, 0xAB, 0xFF, 0xFF, 0xFF, 0xFF, 0x30, 0x37, 0x01, 0x00 };

        public static string KickClient(short DmeId, int WorldId, int DmeWorldId, bool retail)
        {
            DMEObject? homeDmeServer = retail ? DmeClass.TcpServer.GetServerPerAppId(20374) : DmeClass.TcpServer.GetServerPerAppId(20371);
            if (homeDmeServer != null && homeDmeServer.DmeWorld != null)
            {
                World? worldToSearchIn = homeDmeServer.DmeWorld.GetWorldById(WorldId, DmeWorldId);
                var client = worldToSearchIn?.Clients.FirstOrDefault(c => c.DmeId == DmeId);
                if (client != null)
                {
                    _ = Task.Run(() =>
                    {
                        byte[] payload = new byte[KickCMD.Length];
                        Array.Copy(KickCMD, 0, payload, 0, payload.Length);
                        payload[6] = client.mumClient.ProtocolVersion;
                        worldToSearchIn!.SendTcpAppSingle(homeDmeServer, DmeId, payload);
                    });
                    return $"{DmeId} was kicked successfully in world: {worldToSearchIn!.WorldId}!";
                }

                return $"{DmeId} was not found in a valid World!";
            }

            return "Home doesn't have any world populated!";
        }

        public static string ReleaseClient(short DmeId, int WorldId, int DmeWorldId, bool retail)
        {
            DMEObject? homeDmeServer = retail ? DmeClass.TcpServer.GetServerPerAppId(20374) : DmeClass.TcpServer.GetServerPerAppId(20371);
            if (homeDmeServer != null && homeDmeServer.DmeWorld != null)
            {
                World? worldToSearchIn = homeDmeServer.DmeWorld.GetWorldById(WorldId, DmeWorldId);
                var client = worldToSearchIn?.Clients.FirstOrDefault(c => c.DmeId == DmeId);
                if (client != null)
                {
                    _ = Task.Run(() => 
                    {
                        byte[] payload = new byte[ReleaseCMD.Length];
                        Array.Copy(ReleaseCMD, 0, payload, 0, payload.Length);
                        payload[6] = client.mumClient.ProtocolVersion;
                        worldToSearchIn!.SendTcpAppSingle(homeDmeServer, DmeId, payload);
                    });
                    return $"{DmeId} was released successfully in world: {worldToSearchIn!.WorldId}!";
                }

                return $"{DmeId} was not found in a valid World!";
            }

            return "Home doesn't have any world populated!";
        }

        public static string MuteClient(short DmeId, int WorldId, int DmeWorldId, bool retail)
        {
            DMEObject? homeDmeServer = retail ? DmeClass.TcpServer.GetServerPerAppId(20374) : DmeClass.TcpServer.GetServerPerAppId(20371);
            if (homeDmeServer != null && homeDmeServer.DmeWorld != null)
            {
                World? worldToSearchIn = homeDmeServer.DmeWorld.GetWorldById(WorldId, DmeWorldId);
                var client = worldToSearchIn?.Clients.FirstOrDefault(c => c.DmeId == DmeId);
                if (client != null)
                {
                    _ = Task.Run(() => 
                    {
                        byte[] payload = new byte[MuteCMD.Length];
                        Array.Copy(MuteCMD, 0, payload, 0, payload.Length);
                        payload[6] = client.mumClient.ProtocolVersion;
                        worldToSearchIn!.SendTcpAppSingle(homeDmeServer, DmeId, payload);
                    });
                    return $"{DmeId} was muted successfully in world: {worldToSearchIn!.WorldId}!";
                }

                return $"{DmeId} was not found in a valid World!";
            }

            return "Home doesn't have any world populated!";
        }

        public static string MuteAndFreezeClient(short DmeId, int WorldId, int DmeWorldId, bool retail)
        {
            DMEObject? homeDmeServer = retail ? DmeClass.TcpServer.GetServerPerAppId(20374) : DmeClass.TcpServer.GetServerPerAppId(20371);
            if (homeDmeServer != null && homeDmeServer.DmeWorld != null)
            {
                World? worldToSearchIn = homeDmeServer.DmeWorld.GetWorldById(WorldId, DmeWorldId);
                var client = worldToSearchIn?.Clients.FirstOrDefault(c => c.DmeId == DmeId);
                if (client != null)
                {
                    _ = Task.Run(() => 
                    {
                        byte[] payload = new byte[MuteNFreezeCMD.Length];
                        Array.Copy(MuteNFreezeCMD, 0, payload, 0, payload.Length);
                        payload[6] = client.mumClient.ProtocolVersion;
                        worldToSearchIn!.SendTcpAppSingle(homeDmeServer, DmeId, payload);
                    });
                    return $"{DmeId} was muted and frozen successfully in world: {worldToSearchIn!.WorldId}!";
                }

                return $"{DmeId} was not found in a valid World!";
            }

            return "Home doesn't have any world populated!";
        }

        public static string FreezeClient(short DmeId, int WorldId, int DmeWorldId, bool retail)
        {
            DMEObject? homeDmeServer = retail ? DmeClass.TcpServer.GetServerPerAppId(20374) : DmeClass.TcpServer.GetServerPerAppId(20371);
            if (homeDmeServer != null && homeDmeServer.DmeWorld != null)
            {
                World? worldToSearchIn = homeDmeServer.DmeWorld.GetWorldById(WorldId, DmeWorldId);
                var client = worldToSearchIn?.Clients.FirstOrDefault(c => c.DmeId == DmeId);
                if (client != null)
                {
                    _ = Task.Run(() =>
                    {
                        byte[] payload = new byte[FreezeCMD.Length];
                        Array.Copy(FreezeCMD, 0, payload, 0, payload.Length);
                        payload[6] = client.mumClient.ProtocolVersion;
                        worldToSearchIn!.SendTcpAppSingle(homeDmeServer, DmeId, payload);
                    });
                    return $"{DmeId} was frozen successfully in world: {worldToSearchIn!.WorldId}!";
                }

                return $"{DmeId} was not found in a valid World!";
            }

            return "Home doesn't have any world populated!";
        }
    }
}
