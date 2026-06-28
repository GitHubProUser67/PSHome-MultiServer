using CustomLogger;
using HttpMultipartParser;
using MultiServerLibrary.HTTP;

namespace WebAPIService.GameServices.PSHOME.VEEMEE.accorn
{
    public static class Stats
    {
        public static string GetConfig(
            bool get,
            byte[] PostData,
            string ContentType,
            string apiPath
        )
        {
            if (!get)
            {
                var id = string.Empty;
                var boundary = HTTPProcessor.ExtractBoundary(ContentType);

                if (!string.IsNullOrEmpty(boundary) && PostData != null)
                {
                    using (var ms = new MemoryStream(PostData))
                    {
                        id = MultipartFormDataParser.Parse(ms, boundary).GetParameterValue("id");
                        ms.Flush();
                    }
                }

                LoggerAccessor.LogInfo($"[VEEMEE] - Getconfig values : id|{id}");
            }

            return File.Exists($"{apiPath}/VEEMEE/Acorn_Medow/stats_config.json")
                    ? Processor.Sign(
                        File.ReadAllText($"{apiPath}/VEEMEE/Acorn_Medow/stats_config.json")
                    )
                : File.Exists($"{apiPath}/VEEMEE/nml/stats_config.xml")
                    ? Processor.Sign(File.ReadAllText($"{apiPath}/VEEMEE/nml/stats_config.xml"))
                : null;
        }

        public static string Crash(byte[] PostData, string ContentType, string apiPath)
        {
            var corehook = string.Empty;
            var territory = string.Empty;
            var region = string.Empty;
            var psnid = string.Empty;
            var scene = string.Empty;
            var sceneid = string.Empty;
            var scenetime = string.Empty;
            var sceneowner = string.Empty;
            var owner = string.Empty;
            var owned = string.Empty;
            var crash = string.Empty;
            var numplayers = string.Empty;
            var numpeople = string.Empty;
            var objectid = string.Empty;
            var objectname = string.Empty;
            var boundary = HTTPProcessor.ExtractBoundary(ContentType);

            if (!string.IsNullOrEmpty(boundary) && PostData != null)
            {
                using (var ms = new MemoryStream(PostData))
                {
                    var data = MultipartFormDataParser.Parse(ms, boundary);

                    corehook = data.GetParameterValue("corehook");

                    territory = data.GetParameterValue("territory");

                    region = data.GetParameterValue("region");

                    psnid = data.GetParameterValue("psnid");

                    scene = data.GetParameterValue("scene");

                    sceneid = data.GetParameterValue("sceneid");

                    scenetime = data.GetParameterValue("scenetime");

                    sceneowner = data.GetParameterValue("sceneowner");

                    owner = data.GetParameterValue("owner");

                    owned = data.GetParameterValue("owned");

                    crash = data.GetParameterValue("crash");

                    numplayers = data.GetParameterValue("numplayers");

                    numpeople = data.GetParameterValue("numpeople");

                    objectid = data.GetParameterValue("objectid");

                    objectname = data.GetParameterValue("objectname");

                    ms.Flush();
                }

                LoggerAccessor.LogWarn(
                    $"[VEEMEE] : A Client Crash Happened - Details : corehook|{corehook} - territory|{territory} - region|{region} - psnid|{psnid}"
                        + $" - scene|{scene} - sceneid|{sceneid} - scenetime|{scenetime} - sceneowner|{sceneowner} - owner|{owner} - owned|{owned} - crash|{crash} -"
                        + $" numplayers|{numplayers} - numpeople|{numpeople} - objectid|{objectid} - objectname|{objectname}"
                );
            }

            return File.Exists($"{apiPath}/VEEMEE/Acorn_Medow/stats_config.json")
                ? Processor.Sign(
                    File.ReadAllText($"{apiPath}/VEEMEE/Acorn_Medow/stats_config.json")
                )
                : null;
        }

        public static string Usage(byte[] PostData, string ContentType)
        {
            var usage = string.Empty;
            var boundary = HTTPProcessor.ExtractBoundary(ContentType);

            if (!string.IsNullOrEmpty(boundary) && PostData != null)
            {
                using (var ms = new MemoryStream(PostData))
                {
                    var data = MultipartFormDataParser.Parse(ms, boundary);

                    usage = data.GetParameterValue("usage");

                    ms.Flush();
                }

                return Processor.Sign(usage);
            }

            return null;
        }
    }
}
