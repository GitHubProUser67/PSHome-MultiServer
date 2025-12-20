using CustomLogger;
using HttpMultipartParser;
using MultiServerLibrary.HTTP;
using System;
using System.IO;
using System.Text;

namespace WebAPIService.GameServices.PSHOME.CDM
{
    internal class User
    {
        public static string HandleGame(byte[] PostData, string ContentType, string workpath, string absolutePath)
        {
            string pubListPath = $"{workpath}/CDM/User";
            string publisherId = absolutePath.Split("/")[3];
            string gameId = absolutePath.Split("/")[4];
            string filePath = $"{pubListPath}/{publisherId}/{gameId}";
			string gameXMLPath = filePath + "/game.xml";

            if (File.Exists(gameXMLPath))
            {
                return "<xml>\r\n\t" +
                    "<status>success</status>\r\n" +
                    $"{File.ReadAllText(gameXMLPath)}\r\n" +
                    "</xml>";
            }
            else
            {
                LoggerAccessor.LogError($"[CDM] - Publisher Game failed with expected path {filePath}!");

                return "<xml>\r\n\t" +
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
            }
        }

        public static string HandleSpace(byte[] PostData, string ContentType, string workpath, string absolutePath)
        {
            string pubListPath = $"{workpath}/CDM/space/";
            string spacePlayerIsIn = absolutePath.Split("/")[5];
            string region = absolutePath.Split("/")[6];
            string npAge = absolutePath.Split("/")[8];
            string filePath = $"{pubListPath}/{spacePlayerIsIn}/{region}";
			string spaceXML = filePath + "/space.xml";
						
            if (File.Exists(spaceXML))
            {
                return "<xml>\r\n\t" +
                    "<status>success</status>\r\n" +
                    $"{File.ReadAllText(spaceXML)}\r\n" +
                    "</xml>";
            }
            else
                LoggerAccessor.LogError($"[CDM] - User Space failed with expected path {filePath}!");

            return "<xml>" +
                "<status>fail</status>" +
                "</xml>";
        }

        public static string HandleUserSync(byte[] PostData, string ContentType, string workpath, string absolutePath)
        {
            string status;
            string userSync = string.Empty;
            string boundary = HTTPProcessor.ExtractBoundary(ContentType);
			
            using (MemoryStream ms = new MemoryStream(PostData))
                userSync = MultipartFormDataParser.Parse(ms, boundary).GetParameterValue("sync");

            string pubListPath = $"{workpath}/CDM/{absolutePath}";
			
            Directory.CreateDirectory(pubListPath);
			
            string filePath = $"{pubListPath}/UserSyncData.json";

            try
            {
                File.WriteAllBytes(filePath, Encoding.UTF8.GetBytes(userSync));
                status = "<xml>\r\n\t" +
                    "<status>success</status>\r\n" +
                    "</xml>";
            }
			catch (Exception e)
			{
				
                LoggerAccessor.LogError($"[CDM] User Sync JSON write failed with exception {e}");

                status = "<xml>\r\n\t" +
                    "<status>fail</status>\r\n" +
                    "</xml>";
            }

            return status;
        }

    }
}