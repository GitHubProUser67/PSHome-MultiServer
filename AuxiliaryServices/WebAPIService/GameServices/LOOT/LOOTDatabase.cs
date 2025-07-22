using CustomLogger;
using MultiServerLibrary.HTTP;
using HttpMultipartParser;
using System;
using System.IO;
using System.Text;

namespace WebAPIService.GameServices.LOOT
{
    public class LOOTDatabase
    {
        public static string ProcessDatabaseRequest(byte[] PostData, string ContentType, string WorkPath)
        {
            string boundary = HTTPProcessor.ExtractBoundary(ContentType);

            if (!string.IsNullOrEmpty(boundary))
            {
                byte[] ticketData = null;

                using (MemoryStream ms = new MemoryStream(PostData))
                {
                    var data = MultipartFormDataParser.Parse(ms, boundary);

                    string action = data.GetParameterValue("action");
                    string psn_acct_name = data.GetParameterValue("psn_acct_name");
                    string guid = data.GetParameterValue("guid");
                    string serialized_data = data.GetParameterValue("serialized_data");
                    string protocol_version = data.GetParameterValue("protocol_version");

                    string LOOTDBPlayerSavePath = $"{WorkPath}/LOOT/SS3/{guid}";

                    foreach (var file in data.Files)
                    {
                        using (Stream filedata = file.Data)
                        {
                            filedata.Position = 0;

                            // Find the number of bytes in the stream
                            int contentLength = (int)filedata.Length;

                            // Create a byte array
                            byte[] buffer = new byte[contentLength];

                            // Read the contents of the memory stream into the byte array
                            filedata.Read(buffer, 0, contentLength);

                            if (file.FileName == "ticket.bin")
                                ticketData = buffer;

                            filedata.Flush();
                        }
                    }

                    if (ticketData != null)
                    {
                        // Extract the desired portion of the binary data
                        byte[] extractedData = new byte[0x63 - 0x54 + 1];

                        // Copy it
                        Array.Copy(ticketData, 0x54, extractedData, 0, extractedData.Length);

                        // Convert 0x00 bytes to 0x20 so we pad as space.
                        for (int i = 0; i < extractedData.Length; i++)
                        {
                            if (extractedData[i] == 0x00)
                                extractedData[i] = 0x20;
                        }

                        // Convert the modified data to a string
                        string psnname = Encoding.ASCII.GetString(extractedData).Replace(" ", string.Empty);

#if DEBUG
                        LoggerAccessor.LogInfo($"[LOOT] ProtocolVersion: {protocol_version} - Player {psn_acct_name} Action detected as: {action} GUID: {guid} Serialized_Data {serialized_data} ");
#endif

                        switch (action)
                        {
                            case "200.000000": // AddAchievementEarned
                                break;
                            case "201.000000": // GetAchievementDetails
                                break;
                            case "206.000000": // GetUserAchievementList
                                break;
                            case "207.000000": // GetUserAchievementCount
                                break;

                            //Save Current Status and send it to DB
                            case "501":
                                {
                                    switch (guid)
                                    {
                                        //SS3 Director's Chair
                                        case "1BDF949A-0A804B07-9C4C7BAE-46C15AAE":
                                            {
                                                Directory.CreateDirectory(LOOTDBPlayerSavePath);
                                                string profilePath = $"{LOOTDBPlayerSavePath}/{psn_acct_name}.cache";

                                                try
                                                {
                                                    if (File.Exists(profilePath))
                                                    {
                                                        LoggerAccessor.LogInfo($"[LOOT] - Detected existing player data for {guid}, overwriting!");
                                                        File.WriteAllText(profilePath, serialized_data);

                                                        // #Floor:1,EX_ENTITY,0,0,0,0,0,0,false,,false,StageFloor_Plywood.mdl#Canvas:1:L,EX_ENTITY,-9.388,15.5,-12.464,0,0,0,true,,false,StageCanvas_WhiteL.mdl#Canvas:1:C,EX_ENTITY,0.706,7.2691,-21.788,0,0,0,true,,false,StageCanvas_White.mdl#Canvas:1:R,EX_ENTITY,11.3,12.955,-12.8,0,0,0,true,,false,StageCanvas_WhiteR.mdl

                                                        return "<response>\r\n" +
                                                            "    <code>200</code>\r\n" +
                                                            "    <message>ok</message>\r\n" +
                                                            "    <version>1</version>\r\n" +
                                                           $"    <body>{serialized_data}</body>\r\n" +
                                                            "</response>";
                                                    }
                                                    else
                                                    {
                                                        LoggerAccessor.LogInfo($"[LOOT] - New player with no saved for {guid}! Saving for FIRST TIME!");
                                                        File.WriteAllText(profilePath, serialized_data);

                                                        return "<response>\r\n" +
                                                            "    <code>200</code>\r\n" +
                                                            "    <message>ok</message>\r\n" +
                                                            "    <version>1</version>\r\n" +
                                                           $"    <body>{serialized_data}</body>\r\n" +
                                                            "</response>";
                                                    }
                                                }
                                                catch (Exception ex)
                                                {
                                                    LoggerAccessor.LogError($"[LOOT] - Encountered a fatal exception trying to save! Here is the details:\n{ex}");
                                                    return "<response>\r\n" +
                                                            "    <code>500</code>\r\n" +
                                                            "    <message>Internal server error</message>\r\n" +
                                                            "    <version>1</version>\r\n" +
                                                           $"    <body></body>\r\n" +
                                                            "</response>";
                                                }

                                            }

                                        //SS3 Floodlight
                                        case "F810F551-01B540C0-B25A372C-E657524C":
                                            {
                                                Directory.CreateDirectory(LOOTDBPlayerSavePath);
                                                string profilePath = $"{LOOTDBPlayerSavePath}/{psn_acct_name}.cache";

                                                try
                                                {
                                                    if (File.Exists(profilePath))
                                                    {
                                                        LoggerAccessor.LogInfo($"[LOOT] - Detected existing player data for {guid}, overwriting!");
                                                        File.WriteAllText(profilePath, serialized_data);

                                                        // #RichardLight:1,RICHARD_LIGHT,0.25098,0.67843,0.90196,10,1,10,0,14,9.6883,265.09,1,30,12.7,3.2,15.5,1,2.5,3.5#RichardLight:2,RICHARD_LIGHT,0.88235,0.88235,0.88235,8,1,5,0,0,10.677,0.85705,5,30,5.7,5,6.2,1,2.5,13.5#RichardLight:3,RIG_LIGHT,0.98039,0.23529,1,15,1,0,5.5,4,0,0,0,0,0,0,0,0,0,0,0

                                                        return "<response>\r\n" +
                                                            "    <code>200</code>\r\n" +
                                                            "    <message>ok</message>\r\n" +
                                                            "    <version>1</version>\r\n" +
                                                           $"    <body>{serialized_data}</body>\r\n" +
                                                            "</response>";
                                                    }
                                                    else
                                                    {
                                                        LoggerAccessor.LogInfo($"[LOOT] - New player with no saved for {guid}! Saving for FIRST TIME!");
                                                        File.WriteAllText(profilePath, serialized_data);

                                                        return "<response>\r\n" +
                                                            "    <code>200</code>\r\n" +
                                                            "    <message>ok</message>\r\n" +
                                                            "    <version>1</version>\r\n" +
                                                           $"    <body>{serialized_data}</body>\r\n" +
                                                            "</response>";
                                                    }
                                                }
                                                catch (Exception ex)
                                                {
                                                    LoggerAccessor.LogError($"[LOOT] - Encountered a fatal exception trying to save! Here is the details:\n{ex}");
                                                    return "<response>\r\n" +
                                                            "    <code>500</code>\r\n" +
                                                            "    <message>Internal server error</message>\r\n" +
                                                            "    <version>1</version>\r\n" +
                                                           $"    <body></body>\r\n" +
                                                            "</response>";
                                                }

                                            }

                                        //SS3 SpotLight
                                        case "6E5532C3-D87E43BA-AC2695CB-555A96A3":
                                            {
                                                Directory.CreateDirectory(LOOTDBPlayerSavePath);
                                                string profilePath = $"{LOOTDBPlayerSavePath}/{psn_acct_name}.cache";

                                                try
                                                {
                                                    if (File.Exists(profilePath))
                                                    {
                                                        LoggerAccessor.LogInfo($"[LOOT] - Detected existing player data for {guid}, overwriting!");
                                                        File.WriteAllText(profilePath, serialized_data);

                                                        // #RichardLight:1,RICHARD_LIGHT,1.349,1.7765,2,10,1,-10,1.35,-14,10.512,262.12,1,30,-12.7,3.2,-15.5,1,2.5,-13.5#RichardLight:2,RICHARD_LIGHT,0.88235,0.88235,0.88235,8,1,5,0,0,10.677,0.85705,5,30,5.7,5,6.2,1,2.5,-13.5#RichardLight:3,RIG_LIGHT,0.98039,0.23529,1,15,1,0,5.5,-4,0,0,0,0,0,0,0,0,0,0,0

                                                        return "<response>\r\n" +
                                                            "    <code>200</code>\r\n" +
                                                            "    <message>ok</message>\r\n" +
                                                            "    <version>1</version>\r\n" +
                                                           $"    <body>{serialized_data}</body>\r\n" +
                                                            "</response>";
                                                    }
                                                    else
                                                    {
                                                        LoggerAccessor.LogInfo($"[LOOT] - New player with no saved for {guid}! Saving for FIRST TIME!");
                                                        File.WriteAllText(profilePath, serialized_data);

                                                        return "<response>\r\n" +
                                                            "    <code>200</code>\r\n" +
                                                            "    <message>ok</message>\r\n" +
                                                            "    <version>1</version>\r\n" +
                                                           $"    <body>{serialized_data}</body>\r\n" +
                                                            "</response>";
                                                    }
                                                }
                                                catch (Exception ex)
                                                {
                                                    LoggerAccessor.LogError($"[LOOT] - Encountered a fatal exception trying to save! Here is the details:\n{ex}");
                                                    return "<response>\r\n" +
                                                            "    <code>500</code>\r\n" +
                                                            "    <message>Internal server error</message>\r\n" +
                                                            "    <version>1</version>\r\n" +
                                                           $"    <body></body>\r\n" +
                                                            "</response>";
                                                }

                                            }

                                        //Camera - Stage
                                        case "6E0B532B-07194299-A80FC531-39C893BB":
                                            {
                                                Directory.CreateDirectory(LOOTDBPlayerSavePath);
                                                string profilePath = $"{LOOTDBPlayerSavePath}/{psn_acct_name}.cache";

                                                try
                                                {
                                                    if (File.Exists(profilePath))
                                                    {
                                                        LoggerAccessor.LogInfo($"[LOOT] - Detected existing player data for {guid}, overwriting!");
                                                        File.WriteAllText(profilePath, serialized_data);

                                                        // #1,RICHARD_CAMERA,3.0275,1.5,-11.296,0,180,0,76.515#2,RICHARD_CAMERA,5.655,1.5,-5.9686,0,180,0,49.847#3,RICHARD_CAMERA,-5.0921,1.5,-9.2886,0,180,0,49.847

                                                        return "<response>\r\n" +
                                                            "    <code>200</code>\r\n" +
                                                            "    <message>ok</message>\r\n" +
                                                            "    <version>1</version>\r\n" +
                                                           $"    <body>{serialized_data}</body>\r\n" +
                                                            "</response>";
                                                    }
                                                    else
                                                    {
                                                        LoggerAccessor.LogInfo($"[LOOT] - New player with no saved for {guid}! Saving for FIRST TIME!");
                                                        File.WriteAllText(profilePath, serialized_data);

                                                        return "<response>\r\n" +
                                                            "    <code>200</code>\r\n" +
                                                            "    <message>ok</message>\r\n" +
                                                            "    <version>1</version>\r\n" +
                                                           $"    <body>{serialized_data}</body>\r\n" +
                                                            "</response>";
                                                    }
                                                }
                                                catch (Exception ex)
                                                {
                                                    LoggerAccessor.LogError($"[LOOT] - Encountered a fatal exception trying to save! Here is the details:\n{ex}");
                                                    return "<response>\r\n" +
                                                            "    <code>500</code>\r\n" +
                                                            "    <message>Internal server error</message>\r\n" +
                                                            "    <version>1</version>\r\n" +
                                                           $"    <body></body>\r\n" +
                                                            "</response>";
                                                }

                                            }

                                        default:
                                            {
                                                LoggerAccessor.LogWarn($"[LOOT] - Unhandled GET DB Status with object {guid} please report to GITHUB");
                                            }
                                            break;
                                    }
                                }
                                break;
                            //Fetch Current Status from DB
                            case "502":
                                {
                                    switch (guid)
                                    {
                                        //SS3 Director's Chair
                                        case "1BDF949A-0A804B07-9C4C7BAE-46C15AAE":
                                            {
                                                Directory.CreateDirectory(LOOTDBPlayerSavePath);
                                                string profilePath = $"{LOOTDBPlayerSavePath}/{psn_acct_name}.cache";

                                                try
                                                {
                                                    if (File.Exists(profilePath))
                                                    {
                                                        LoggerAccessor.LogInfo($"[LOOT] - Detected existing player data for {guid}, sending!");
                                                        string playerData = File.ReadAllText(profilePath);

                                                        return "<response>\r\n" +
                                                            "    <code>200</code>\r\n" +
                                                            "    <message>ok</message>\r\n" +
                                                            "    <version>1</version>\r\n" +
                                                           $"    <body>{playerData}</body>\r\n" +
                                                            "</response>";
                                                    }
                                                    else
                                                    {
                                                        LoggerAccessor.LogInfo($"[LOOT] - New player with no saved for {guid}! Using Default!");

                                                        return "<response>\r\n" +
                                                            "    <code>200</code>\r\n" +
                                                            "    <message>ok</message>\r\n" +
                                                            "    <version>1</version>\r\n" +
                                                            "    <body>#Floor:1,EX_ENTITY,0,0,0,0,0,0,false,,false,StageFloor_Plywood.mdl#Canvas:1:L,EX_ENTITY,-9.388,15.5,-12.464,0,0,0,true,,false,StageCanvas_WhiteL.mdl#Canvas:1:C,EX_ENTITY,0.706,7.2691,-21.788,0,0,0,true,,false,StageCanvas_White.mdl#Canvas:1:R,EX_ENTITY,11.3,12.955,-12.8,0,0,0,true,,false,StageCanvas_WhiteR.mdl</body>\r\n" +
                                                            "</response>";
                                                    }
                                                }
                                                catch (Exception ex)
                                                {
                                                    LoggerAccessor.LogError($"[LOOT] - Encountered a fatal exception trying to save! Here is the details:\n{ex}");
                                                    return "<response>\r\n" +
                                                            "    <code>500</code>\r\n" +
                                                            "    <message>Internal server error</message>\r\n" +
                                                            "    <version>1</version>\r\n" +
                                                           $"    <body></body>\r\n" +
                                                            "</response>";
                                                }

                                            }

                                        //SS3 Floodlight
                                        case "F810F551-01B540C0-B25A372C-E657524C":
                                            {
                                                Directory.CreateDirectory(LOOTDBPlayerSavePath);
                                                string profilePath = $"{LOOTDBPlayerSavePath}/{psn_acct_name}.cache";

                                                try
                                                {
                                                    if (File.Exists(profilePath))
                                                    {
                                                        LoggerAccessor.LogInfo($"[LOOT] - Detected existing player data for {guid}, sending!");
                                                        string playerData = File.ReadAllText(profilePath);

                                                        return "<response>\r\n" +
                                                            "    <code>200</code>\r\n" +
                                                            "    <message>ok</message>\r\n" +
                                                            "    <version>1</version>\r\n" +
                                                           $"    <body>{playerData}</body>\r\n" +
                                                            "</response>";
                                                    }
                                                    else
                                                    {
                                                        LoggerAccessor.LogInfo($"[LOOT] - New player with no saved for {guid}! Using Default!");

                                                        return "<response>\r\n" +
                                                            "    <code>200</code>\r\n" +
                                                            "    <message>ok</message>\r\n" +
                                                            "    <version>1</version>\r\n" +
                                                            "    <body>#RichardLight:1,RICHARD_LIGHT,0.25098,0.67843,0.90196,10,1,10,0,14,9.6883,265.09,1,30,12.7,3.2,15.5,1,2.5,3.5#RichardLight:2,RICHARD_LIGHT,0.88235,0.88235,0.88235,8,1,5,0,0,10.677,0.85705,5,30,5.7,5,6.2,1,2.5,13.5#RichardLight:3,RIG_LIGHT,0.98039,0.23529,1,15,1,0,5.5,4,0,0,0,0,0,0,0,0,0,0,0</body>\r\n" +
                                                            "</response>";
                                                    }
                                                }
                                                catch (Exception ex)
                                                {
                                                    LoggerAccessor.LogError($"[LOOT] - Encountered a fatal exception trying to save! Here is the details:\n{ex}");
                                                    return "<response>\r\n" +
                                                            "    <code>500</code>\r\n" +
                                                            "    <message>Internal server error</message>\r\n" +
                                                            "    <version>1</version>\r\n" +
                                                           $"    <body></body>\r\n" +
                                                            "</response>";
                                                }
                                            }
                                        //SS3 SpotLight
                                        case "6E5532C3-D87E43BA-AC2695CB-555A96A3":
                                            {
                                                Directory.CreateDirectory(LOOTDBPlayerSavePath);
                                                string profilePath = $"{LOOTDBPlayerSavePath}/{psn_acct_name}.cache";

                                                try
                                                {
                                                    if (File.Exists(profilePath))
                                                    {
                                                        LoggerAccessor.LogInfo($"[LOOT] - Detected existing player data for {guid}, sending!");
                                                        string playerData = File.ReadAllText(profilePath);

                                                        return "<response>\r\n" +
                                                            "    <code>200</code>\r\n" +
                                                            "    <message>ok</message>\r\n" +
                                                            "    <version>1</version>\r\n" +
                                                           $"    <body>{playerData}</body>\r\n" +
                                                            "</response>";
                                                    }
                                                    else
                                                    {
                                                        LoggerAccessor.LogInfo($"[LOOT] - New player with no saved for {guid}! Using Default!");

                                                        /// <summary>
                                                        /// Commented out SpotLight data works, but it doesn't handle well in LUA to activate minigame, so we leave default for now.
                                                        /// </summary>
                                                        /// #RichardLight:1,RICHARD_LIGHT,1.349,1.7765,2,10,1,-10,1.35,-14,10.512,262.12,1,30,-12.7,3.2,-15.5,1,2.5,-13.5#RichardLight:2,RICHARD_LIGHT,0.88235,0.88235,0.88235,8,1,5,0,0,10.677,0.85705,5,30,5.7,5,6.2,1,2.5,-13.5#RichardLight:3,RIG_LIGHT,0.98039,0.23529,1,15,1,0,5.5,-4,0,0,0,0,0,0,0,0,0,0,0
                                                        return "<response>\r\n" +
                                                            "    <code>200</code>\r\n" +
                                                            "    <message>ok</message>\r\n" +
                                                            "    <version>1</version>\r\n" +
                                                            "    <body></body>\r\n" +
                                                            "</response>";
                                                    }
                                                }
                                                catch (Exception ex)
                                                {
                                                    LoggerAccessor.LogError($"[LOOT] - Encountered a fatal exception trying to save! Here is the details:\n{ex}");
                                                    return "<response>\r\n" +
                                                            "    <code>500</code>\r\n" +
                                                            "    <message>Internal server error</message>\r\n" +
                                                            "    <version>1</version>\r\n" +
                                                           $"    <body></body>\r\n" +
                                                            "</response>";
                                                }

                                            }
                                        //Camera - Stage
                                        case "6E0B532B-07194299-A80FC531-39C893BB":
                                            {
                                                Directory.CreateDirectory(LOOTDBPlayerSavePath);
                                                string profilePath = $"{LOOTDBPlayerSavePath}/{psn_acct_name}.cache";

                                                try
                                                {
                                                    if (File.Exists(profilePath))
                                                    {
                                                        LoggerAccessor.LogInfo($"[LOOT] - Detected existing player data for {guid}, sending!");
                                                        string playerData = File.ReadAllText(profilePath);

                                                        return "<response>\r\n" +
                                                            "    <code>200</code>\r\n" +
                                                            "    <message>ok</message>\r\n" +
                                                            "    <version>1</version>\r\n" +
                                                           $"    <body>{playerData}</body>\r\n" +
                                                            "</response>";
                                                    }
                                                    else
                                                    {
                                                        LoggerAccessor.LogInfo($"[LOOT] - New player with no saved for {guid}! Using Default!");

                                                        return "<response>\r\n" +
                                                            "    <code>200</code>\r\n" +
                                                            "    <message>ok</message>\r\n" +
                                                            "    <version>1</version>\r\n" +
                                                            "    <body>#1,RICHARD_CAMERA,3.0275,1.5,-11.296,0,180,0,76.515#2,RICHARD_CAMERA,5.655,1.5,-5.9686,0,180,0,49.847#3,RICHARD_CAMERA,-5.0921,1.5,-9.2886,0,180,0,49.847</body>\r\n" +
                                                            "</response>";
                                                    }
                                                }
                                                catch (Exception ex)
                                                {
                                                    LoggerAccessor.LogError($"[LOOT] - Encountered a fatal exception trying to save! Here is the details:\n{ex}");
                                                    return "<response>\r\n" +
                                                            "    <code>500</code>\r\n" +
                                                            "    <message>Internal server error</message>\r\n" +
                                                            "    <version>1</version>\r\n" +
                                                           $"    <body></body>\r\n" +
                                                            "</response>";
                                                }
                                            }

                                        default:
                                            {
                                                LoggerAccessor.LogWarn($"[LOOT] - Unhandled UPLOAD DB Status with object {guid} please report to GITHUB");
                                            }
                                            break;
                                    }

                                }
                                break;
                            case "604.000000": // UNK
                                break;
                            default:
                                {
                                    LoggerAccessor.LogWarn($"[LOOT] - Unhandled Action {action} please report to GITHUB");
                                }
                                break;

                        }
                    }
                    ms.Flush();
                }
            }

            return null;
        }
    }
}
