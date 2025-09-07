using CustomLogger;
using MultiServerLibrary.HTTP;
using HttpMultipartParser;
using System;
using System.IO;
using System.Text;

namespace WebAPIService.GameServices.CDM
{
    internal class User
    {
        public static string handleGame(byte[] PostData, string ContentType, string workpath, string absolutePath)
        {
            string pubListPath = $"{workpath}/CDM/User";

            Directory.CreateDirectory(pubListPath);
            string filePath = $"{pubListPath}/game.xml";
            if (File.Exists(filePath))
            {
                string res = File.ReadAllText(filePath);

                string resourceXML = "<xml>\r\n\t" +
                    "<status>success</status>\r\n" +
                    $"{res}\r\n" +
                    "</xml>";

                return resourceXML;
            }
            else
            {
                LoggerAccessor.LogWarn($"[CDM] - Publisher Game failed with expected path {filePath}!");

                string resourceXML = "<xml>\r\n\t" +
                    "<status>success</status>\r\n" +
                    $"<publisher_game>\r\n" +
                    $"    <publisher_id>14</publisher_id>\r\n" +
                    $"    <games>\r\n" +
                    $"        <game>\r\n" +
                    $"            <attributes>\r\n" +
                    $"                <id>10</id>\r\n" +
                    $"            </attributes>\r\n" +
                    $"            <game_data />\r\n" +
                    $"        </game>\r\n" +
                    $"    </games>\r\n" +
                    $"    <inventories>\r\n" +
                    $"        <inventory>\r\n" +
                    $"            <attributes>\r\n" +
                    $"                <id>1</id>\r\n" +
                    $"            </attributes>\r\n" +
                    $"            <item>Test</item>\r\n" +
                    $"            <quantity>1</quantity>\r\n" +
                    $"        </inventory>\r\n" +
                    $"        <!-- Additional inventory entries go here -->\r\n" +
                    $"    </inventories>\r\n" +
                    $"</publisher_game>\r\n" +
                    $"<publisher_game>\r\n" +
                    $"    <publisher_id>13</publisher_id>\r\n" +
                    $"    <games>\r\n" +
                    $"        <game>\r\n" +
                    $"            <attributes>\r\n" +
                    $"                <id>6</id>\r\n" +
                    $"            </attributes>\r\n" +
                    $"            <game_data />\r\n" +
                    $"        </game>\r\n" +
                    $"    </games>\r\n" +
                    $"    <inventories>\r\n" +
                    $"        <inventory>\r\n" +
                    $"            <attributes>\r\n" +
                    $"                <id>1</id>\r\n" +
                    $"            </attributes>\r\n" +
                    $"            <item>Test</item>\r\n" +
                    $"            <quantity>1</quantity>\r\n" +
                    $"        </inventory>\r\n" +
                    $"        <!-- Additional inventory entries go here -->\r\n" +
                    $"    </inventories>\r\n" +
                    $"</publisher_game>\r\n" +
                    "</xml>";

                return resourceXML;
            }
        }

        public static string handleSpace(byte[] PostData, string ContentType, string workpath, string absolutePath)
        {
            string pubListPath = $"{workpath}/CDM/{absolutePath}";

            Directory.CreateDirectory(pubListPath);
            string filePath = $"{pubListPath}/space.xml";
            if (File.Exists(filePath))
            {
                string res = File.ReadAllText(filePath);

                string resourceXML = "<xml>\r\n\t" +
                    "<status>success</status>\r\n" +
                    $"{res}\r\n" +
                    "</xml>";

                return resourceXML;
            }
            else
                LoggerAccessor.LogWarn($"[CDM] - User Space failed with expected path {filePath}!");

            return "<xml>" +
                "<status>fail</status>" +
                "</xml>";
        }

        public static string handleUserSync(byte[] PostData, string ContentType, string workpath, string absolutePath)
        {
            string status;
            string userSync = string.Empty;

            string boundary = HTTPProcessor.ExtractBoundary(ContentType);
            using (MemoryStream ms = new MemoryStream(PostData))
            {
                var data = MultipartFormDataParser.Parse(ms, boundary);

                userSync = data.GetParameterValue("sync");

                ms.Flush();
            }
#if DEBUG
            LoggerAccessor.LogInfo($"[CDM] User Sync - Received: \n{userSync}");
#endif
            string pubListPath = $"{workpath}/CDM/{absolutePath}";
            Directory.CreateDirectory(pubListPath);
            string filePath = $"{pubListPath}/UserSyncData.json";

            try
            {
                File.WriteAllText(filePath, userSync);
                status = "<xml>\r\n\t" +
                    "<status>success</status>\r\n" +
                    "</xml>";
            } catch (Exception e) {
                LoggerAccessor.LogWarn($"[CDM] User Sync JSON write failed with exception {e}");

                status = "<xml>\r\n\t" +
                    "<status>fail</status>\r\n" +
                    "</xml>";
            }

            return status;
        }

    }
}