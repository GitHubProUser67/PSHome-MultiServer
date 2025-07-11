using System;
using System.IO;
using CustomLogger;
using HttpMultipartParser;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Text;
using MultiServerLibrary.HTTP;
using static WebAPIService.OHS.User;
using System.Linq;

namespace WebAPIService.OHS
{
    public class UserInventory
    {
        public static string AddGlobalItems(byte[] PostData, string ContentType, string directoryPath, string batchparams, int game)
        {
            //int itemCount = 0;

            string dataforohs = null;
            string output = null;

            string boundary = HTTPProcessor.ExtractBoundary(ContentType);

            if (string.IsNullOrEmpty(batchparams))
            {
                if (!string.IsNullOrEmpty(boundary))
                {
                    try
                    {
                        using (MemoryStream ms = new MemoryStream(PostData))
                        {
                            var data = MultipartFormDataParser.Parse(ms, boundary);
                            LoggerAccessor.LogInfo($"[OHS] : Client Version - {data.GetParameterValue("version")}");

                            dataforohs = JaminProcessor.JaminDeFormat(data.GetParameterValue("data"), true, game);
                            ms.Flush();
                        }
                    }
                    catch (Exception ex)
                    {
                        LoggerAccessor.LogError($"Error processing global item: {ex}");
                        dataforohs = JaminProcessor.JaminFormat("{ [\"status\"] = \"fail\" }", game);
                    }
                }
            }
            else
                dataforohs = batchparams;

            try
            {
                if (!string.IsNullOrEmpty(dataforohs))
                {
                    JToken Token = JToken.Parse(dataforohs);

                    object value = Utils.JtokenUtils.GetValueFromJToken(Token, "value");

                    object data = Utils.JtokenUtils.GetValueFromJToken(Token, "data");

                    //object user = DataTypesUtils.GetValueFromJToken(Token, "user");

                    string globaldatastring = directoryPath + "/Globals.json";

                    if (File.Exists(globaldatastring))
                    {
                        string globaldata = File.ReadAllText(globaldatastring);

                        if (!string.IsNullOrEmpty(globaldata))
                        {
                            JObject jObject = JObject.Parse(globaldata);

                            if (jObject != null && value != null)
                            {
                                // Check if the key name already exists in the JSON
                                JToken existingKey = jObject.SelectToken($"$..{data}");

                                if (existingKey != null)
                                    // Update the value of the existing key
                                    existingKey.Replace(JToken.FromObject(value));
                                else if (data != null)
                                {
                                    JToken KeyEntry = jObject["key"];

                                    if (KeyEntry != null)
                                        // Step 2: Add a new entry to the "Key" object
                                        KeyEntry[data] = JToken.FromObject(value);
                                }

                                File.WriteAllText(globaldatastring, jObject.ToString(Formatting.Indented));
                            }
                        }
                    }
                    else if (data != null)
                    {
                        string keystring = data.ToString();

                        if (keystring != null && value != null)
                        {
                            // Create a new profile with the key field
                            OHSGlobalProfile newProfile = new OHSGlobalProfile
                            {
                                Key = new JObject { { keystring, JToken.FromObject(value) } }
                            };

                            File.WriteAllText(globaldatastring, JsonConvert.SerializeObject(newProfile));
                        }
                    }

                    if (value != null)
                        output = LuaUtils.ConvertJTokenToLuaTable(JToken.FromObject(value), true);

                    /*
                    // Process the data and add it to the JSON file
                    string jsonData = dataforohs; //JaminProcessor.JaminFormat(dataforohs, game);
                    WriteToJsonFile(jsonData, Path.Combine(directoryPath, "Global.json"));
                    LoggerAccessor.LogInfo("Successfully added items as globalitems!!");
                    */
                }
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError($"[User] - Json Format Error - {ex}");


            }

            if (!string.IsNullOrEmpty(batchparams))
            {
                if (string.IsNullOrEmpty(output))
                    return null;
                else
                    return output;
            }
            else
            {
                if (string.IsNullOrEmpty(output))
                    dataforohs = JaminProcessor.JaminFormat("{ [\"status\"] = \"fail\" }", game);
                else
                    dataforohs = JaminProcessor.JaminFormat($"{{ [\"status\"] = \"success\", [\"value\"] = {{ {output} }} }}", game);
            }



            return dataforohs;
        }

        public static string GetGlobalItems(byte[] PostData, string ContentType, string directorypath, string batchparams, int game)
        {
            string dataforohs = null;
            string output = null;

            if (string.IsNullOrEmpty(batchparams))
            {
                string boundary = HTTPProcessor.ExtractBoundary(ContentType);

                if (!string.IsNullOrEmpty(boundary))
                {
                    using (MemoryStream ms = new MemoryStream(PostData))
                    {
                        var data = MultipartFormDataParser.Parse(ms, boundary);
                        LoggerAccessor.LogInfo($"[OHS] : Client Version - {data.GetParameterValue("version")}");
                        dataforohs = JaminProcessor.JaminDeFormat(data.GetParameterValue("data"), true, game);
                        ms.Flush();
                    }
                }
            }

            try
            {
                string globalinvdatastring = directorypath + "/Global_Variables.json";

                if (File.Exists(globalinvdatastring))
                {
                    string filedata = File.ReadAllText(globalinvdatastring);

                    //if (string.IsNullOrEmpty(filedata))

                    output = "{ " + LuaUtils.ConvertJTokenToLuaTable(JToken.Parse(filedata), true) + " }";
                    LoggerAccessor.LogWarn($"[UserInventory] GetGlobalItems - {output}");
                }
                else
                {
                    LoggerAccessor.LogError($"[UserInventory] GetGlobalItems - File Not Found in this dir: {globalinvdatastring} \nSending Default!");

                }
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError($"[UserInventory] GetGlobalItems - Json Format Error - {ex}");
            }

            if (!string.IsNullOrEmpty(batchparams))
            {
                if (string.IsNullOrEmpty(output))
                    return null;
                else
                    return output;
            }
            else
            {
                if (string.IsNullOrEmpty(output))
                    dataforohs = JaminProcessor.JaminFormat("{ [\"status\"] = \"fail\" }", game);
                else
                    dataforohs = JaminProcessor.JaminFormat($"{{ [\"status\"] = \"success\", [\"value\"] = {{ {output} }} }}", game);
            }

            return dataforohs;
        }

        public static string UpdateUserInventory(byte[] PostData, string ContentType, string directorypath, string batchparams, int game)
        {
            string dataforohs = null;
            string output = null;

            if (string.IsNullOrEmpty(batchparams))
            {
                string boundary = HTTPProcessor.ExtractBoundary(ContentType);

                if (!string.IsNullOrEmpty(boundary))
                {
                    using (MemoryStream ms = new MemoryStream(PostData))
                    {
                        var data = MultipartFormDataParser.Parse(ms, boundary);
                        LoggerAccessor.LogInfo($"[OHS] : Client Version - {data.GetParameterValue("version")}");
                        dataforohs = JaminProcessor.JaminDeFormat(data.GetParameterValue("data"), true, game);
                        ms.Flush();
                    }
                }
            }
            else
                dataforohs = batchparams;

            try
            {
                if (!string.IsNullOrEmpty(dataforohs))
                {
                    // Deserialize the JSON data into a JObject
                    JObject jObject = JsonConvert.DeserializeObject<JObject>(dataforohs);

                    string user = jObject?.Value<string>("user");
                    string region = jObject?.Value<string>("region");

                    StringBuilder resultBuilder = new StringBuilder();

                    string inventorypath = directorypath + $"/User_Inventory/{user}_{region}/";

                    if (Directory.Exists(inventorypath))
                    {
                        JToken invName = jObject?.Value<string>("inventory_name");
                        string fileName = inventorypath + invName + ".json";
                        JArray invItemsToChange = jObject?.Value<JArray>("changes");

                        //JArray invItemsToChange = JArray.Parse(inventoryChanges);

                        foreach (string key in invName.Select(v => (string)v))
                        {
                            if (File.Exists(fileName))
                            {
                                string invFileData = File.ReadAllText(fileName);

                                if (!string.IsNullOrEmpty(invFileData))
                                {
                                    JObject existingFileJson = JObject.Parse(invFileData);

                                    // Check if the invName already exists in the JSON
                                    JToken existingKey = existingFileJson.SelectToken($"$..{invName}");

                                    if (existingKey != null && invItemsToChange != null)
                                        // Update the value of the existing key
                                        existingKey.Replace(JToken.FromObject(invItemsToChange));
                                    else if (existingKey == null && invItemsToChange != null)
                                    {
                                        JToken KeyEntry = existingKey["key"];

                                        if (KeyEntry != null)
                                            // Step 2: Add a new entry to the "Key" object
                                            KeyEntry[existingKey] = JToken.FromObject(invItemsToChange);
                                    }

                                    existingFileJson.Add(invItemsToChange);

                                    File.WriteAllText(inventorypath, existingFileJson.ToString(Formatting.Indented));
                                }

                                if (invItemsToChange != null)
                                {
                                    if (JToken.FromObject(invItemsToChange).Type == JTokenType.String)
                                        // Handle string type
                                        output = "\"" + JToken.FromObject(invItemsToChange).ToString() + "\"";
                                    else if (JToken.FromObject(invItemsToChange).Type == JTokenType.Integer)
                                        // Handle integer type
                                        output = JToken.FromObject(invItemsToChange).ToString();
                                    else if (JToken.FromObject(invItemsToChange).Type == JTokenType.Float)
                                        // Handle integer type
                                        output = JToken.FromObject(invItemsToChange).ToString();
                                    else if (JToken.FromObject(invItemsToChange).Type == JTokenType.Array)
                                        // Handle array type
                                        output = LuaUtils.ConvertJTokenToLuaTable(JToken.FromObject(invItemsToChange), false);
                                    else if (JToken.FromObject(invItemsToChange).Type == JTokenType.Boolean)
                                        // Handle boolean type
                                        output = JToken.FromObject(invItemsToChange).ToObject<bool>() ? "true" : "false";
                                }

                                output = LuaUtils.ConvertJTokenToLuaTable(JToken.Parse(invFileData), false);
                            }
                            else
                            {
                                string invCh = (string)invItemsToChange;

                                if (!string.IsNullOrEmpty(invCh))
                                {
                                    FileStream fs = File.Create(fileName);
#if NET5_0_OR_GREATER
                                    fs.Write((byte[])invItemsToChange);
#else
                                    byte[] invItemsToChangeBytes = (byte[])invItemsToChange;
                                    fs.Write(invItemsToChangeBytes, 0, invItemsToChangeBytes.Length);
#endif
                                    fs.Close();
                                    fs.Dispose();

                                    output = LuaUtils.ConvertJTokenToLuaTable(JArray.Parse(invCh), false);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError($"[User] - Json Format Error - {ex}");
            }

            if (!string.IsNullOrEmpty(batchparams))
            {
                if (string.IsNullOrEmpty(output))
                    return null;
                else
                    return output;
            }
            else
            {
                if (string.IsNullOrEmpty(output))
                    dataforohs = JaminProcessor.JaminFormat("{ [\"status\"] = \"fail\" }", game);
                else
                    dataforohs = JaminProcessor.JaminFormat($"{{ [\"status\"] = \"success\", [\"value\"] = {output} }}", game);
            }

            return dataforohs;
        }

        public static string GetUserInventory(byte[] PostData, string ContentType, string directorypath, string batchparams, int game)
        {
            string dataforohs = null;

            if (string.IsNullOrEmpty(batchparams))
            {
                string boundary = HTTPProcessor.ExtractBoundary(ContentType);

                if (!string.IsNullOrEmpty(boundary))
                {
                    using (MemoryStream ms = new MemoryStream(PostData))
                    {
                        var data = MultipartFormDataParser.Parse(ms, boundary);
                        LoggerAccessor.LogInfo($"[OHS] : Client Version - {data.GetParameterValue("version")}");
                        dataforohs = JaminProcessor.JaminDeFormat(data.GetParameterValue("data"), true, game);
                        ms.Flush();
                    }
                }
            }
            else
                dataforohs = batchparams;

            try
            {
                if (!string.IsNullOrEmpty(dataforohs))
                {
                    // Deserialize the JSON data into a JObject
                    JObject jObject = JsonConvert.DeserializeObject<JObject>(dataforohs);

                    if (jObject != null)
                    {
                        string user = jObject.Value<string>("user");
                        string region = jObject.Value<string>("region");

                        if (!string.IsNullOrEmpty(user) && !string.IsNullOrEmpty(region))
                        {
                            string inventorypath = directorypath + $"/User_Inventory/{user}_{region}/";

                            StringBuilder resultBuilder = new StringBuilder();

                            if (Directory.Exists(inventorypath))
                            {
                                JToken keyToken = jObject.GetValue("inventory_names");

                                if (keyToken != null)
                                {
                                    foreach (string key in keyToken.Select(v => (string)v))
                                    {
                                        if (File.Exists(inventorypath + key + ".json"))
                                        {
                                            string inventorydata = File.ReadAllText(inventorypath + key + ".json");

                                            if (inventorydata != null)
                                            {
                                                string datafrominventory = LuaUtils.ConvertJTokenToLuaTable(JObject.Parse(inventorydata), false);

                                                if (resultBuilder.Length == 0)
                                                    resultBuilder.Append($"{{ [\"{key}\"] = {datafrominventory}");
                                                else
                                                    resultBuilder.Append($", [\"{key}\"] = {datafrominventory}");
                                            }
                                        }
                                        else
                                        {
                                            if (resultBuilder.Length == 0)
                                                resultBuilder.Append($"{{ [\"{key}\"] = {{ }}");
                                            else
                                                resultBuilder.Append($", [\"{key}\"] = {{ }}");
                                        }
                                    }
                                }
                            }
                            else
                            {
                                Directory.CreateDirectory(inventorypath);

                                JToken keyToken = jObject.GetValue("inventory_names");

                                if (keyToken != null)
                                {
                                    foreach (string key in keyToken)
                                    {
                                        if (resultBuilder.Length == 0)
                                            resultBuilder.Append($"{{ [\"{key}\"] = {{ }}");
                                        else
                                            resultBuilder.Append($", [\"{key}\"] = {{ }}");
                                    }
                                }
                            }

                            if (resultBuilder.Length != 0)
                            {
                                resultBuilder.Append(" }");
                                dataforohs = resultBuilder.ToString();
                            }
                            else
                                dataforohs = string.Empty;

                            resultBuilder = null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError($"[UserInventory] - Json Format Error - {ex}");
            }

            if (!string.IsNullOrEmpty(batchparams))
            {
                if (string.IsNullOrEmpty(dataforohs))
                    return null;
                else
                    return dataforohs;
            }
            else
            {
                if (string.IsNullOrEmpty(dataforohs))
                    dataforohs = JaminProcessor.JaminFormat("{ [\"status\"] = \"fail\" }", game);
                else
                    dataforohs = JaminProcessor.JaminFormat($"{{ [\"status\"] = \"success\", [\"value\"] = {dataforohs} }}", game);
            }

            return dataforohs;
        }


        public static void WriteToJsonFile(string jsonData, string filePath)
        {
            try
            {
                // Create the directory if it doesn't exist
                string directoryPath = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                //LoggerAccessor.LogError($"GLOBAL FILE PATH CHECK {filePath}");

                // Read existing data from the file, if it exists
                string existingData = File.Exists(filePath) ? File.ReadAllText(filePath) : string.Empty;

                // Merge the existing data with the new data
                string mergedData = MergeJsonData(existingData, jsonData);

                // Write the merged data back to the file
                File.WriteAllText(filePath, mergedData);
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError($"Error writing to JSON file: {ex}");
            }
        }

        private static string MergeJsonData(string existingData, string newData)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(existingData))
                {
                    // If no existing data, return the new data as is
                    return newData;
                }

                // Parse existing and new data as JObjects
                JObject existingObject = JObject.Parse(existingData);
                JObject newObject = JObject.Parse(newData);

                // Merge the objects
                existingObject.Merge(newObject, new JsonMergeSettings
                {
                    MergeArrayHandling = MergeArrayHandling.Union
                });

                // Convert the merged object back to a JSON string
                return existingObject.ToString();
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError($"Error merging JSON data: {ex}");
                return existingData;
            }
        }

    }

}
