using Horizon.DME.Models;

namespace Horizon.DME.Extension.PlayStationHome
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
                if (worldToSearchIn != null && worldToSearchIn.Clients.Any(client => client.DmeId == DmeId))
                {
                    _ = Task.Run(() => { worldToSearchIn.SendTcpAppSingle(homeDmeServer, DmeId, KickCMD); });
                    return $"{DmeId} was kicked successfully in world: {worldToSearchIn.WorldId}!";
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
                if (worldToSearchIn != null && worldToSearchIn.Clients.Any(client => client.DmeId == DmeId))
                {
                    _ = Task.Run(() => { worldToSearchIn.SendTcpAppSingle(homeDmeServer, DmeId, ReleaseCMD); });
                    return $"{DmeId} was released successfully in world: {worldToSearchIn.WorldId}!";
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
                if (worldToSearchIn != null && worldToSearchIn.Clients.Any(client => client.DmeId == DmeId))
                {
                    _ = Task.Run(() => { worldToSearchIn.SendTcpAppSingle(homeDmeServer, DmeId, MuteCMD); });
                    return $"{DmeId} was muted successfully in world: {worldToSearchIn.WorldId}!";
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
                if (worldToSearchIn != null && worldToSearchIn.Clients.Any(client => client.DmeId == DmeId))
                {
                    _ = Task.Run(() => { worldToSearchIn.SendTcpAppSingle(homeDmeServer, DmeId, MuteNFreezeCMD); });
                    return $"{DmeId} was muted and frozen successfully in world: {worldToSearchIn.WorldId}!";
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
                if (worldToSearchIn != null && worldToSearchIn.Clients.Any(client => client.DmeId == DmeId))
                {
                    _ = Task.Run(() => { worldToSearchIn.SendTcpAppSingle(homeDmeServer, DmeId, FreezeCMD); });
                    return $"{DmeId} was frozen successfully in world: {worldToSearchIn.WorldId}!";
                }

                return $"{DmeId} was not found in a valid World!";
            }

            return "Home doesn't have any world populated!";
        }
    }
}
