using QuazalServer.QNetZ.Attributes;
using QuazalServer.QNetZ.Interfaces;
using QuazalServer.RDVServices.DDL.Models;

namespace QuazalServer.RDVServices.GameServices.PCDriverServices
{
    /// <summary>
    /// Hermes Game Info protocol
    /// </summary>
    [RMCService((ushort)RMCProtocolId.GameInfoService)]
    class GameInfoService : RMCServiceBase
    {
        // files which server can renturn
        private static readonly string[] FileList = { "OnlineConfig.ini" };

        [RMCMethod(5)]
        public RMCResult GetFileInfoList(int indexStart, int numElements, string stringSearch)
        {
            List<PersistentInfo> fileList = new();

            if (!string.IsNullOrEmpty(stringSearch))
            {
                var directoryPath =
                    QuazalServerConfiguration.QuazalStaticFolder + "/StaticFiles/DriverPC";

                if (stringSearch == "*")
                {
                    foreach (var name in FileList.Skip(indexStart).Take(numElements))
                    {
                        var path = Path.Combine(directoryPath, name);

                        if (!File.Exists(path))
                            continue;

                        fileList.Add(
                            new PersistentInfo
                            {
                                m_name = name,
                                m_size = (uint)new FileInfo(path).Length,
                            }
                        );
                    }
                }
                else
                {
                    if (File.Exists(Path.Combine(directoryPath, stringSearch)))
                    {
                        fileList.Add(
                            new PersistentInfo
                            {
                                m_name = stringSearch,
                                m_size = (uint)
                                    new FileInfo(Path.Combine(directoryPath, stringSearch)).Length,
                            }
                        );
                    }
                }
            }

            return Result(fileList);
        }
    }
}
