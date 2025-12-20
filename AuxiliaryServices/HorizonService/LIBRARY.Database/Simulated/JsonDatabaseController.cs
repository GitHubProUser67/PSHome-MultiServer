using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace HorizonService.LIBRARY.Database.Simulated
{
    public class JsonDatabaseController
    {
        public static void WriteToJsonFile(string directoryPath, string Type, string Value, bool IsBanned = false)
        {
            if (string.IsNullOrEmpty(directoryPath))
                return;

            try
            {
                Directory.CreateDirectory(directoryPath + $"/MediusSimulatedDatabase/{Type}");

                File.WriteAllText(directoryPath + $"/MediusSimulatedDatabase/{Type}/{Value}.json", JsonConvert.SerializeObject(new Dictionary<string, bool> { { "IsBanned", IsBanned } }));
            }
            catch (Exception ex)
            {
                CustomLogger.LoggerAccessor.LogError($"[JsonDatabaseController] - WriteToJsonFile errored out with exception: {ex}");
            }
        }

        public static (string, bool) ReadFromJsonFile(string directoryPath, string Type, string Value)
        {
            if (string.IsNullOrEmpty(directoryPath))
                return ("INVALIDFOLDER", false);

            try
            {
                if (File.Exists(directoryPath + $"/MediusSimulatedDatabase/{Type}/{Value}.json"))
                {
                    string json = File.ReadAllText(directoryPath + $"/MediusSimulatedDatabase/{Type}/{Value}.json");

                    Dictionary<string, bool> JsonOutput = JsonConvert.DeserializeObject<Dictionary<string, bool>>(json);

                    if (JsonOutput != null && JsonOutput.ContainsKey("IsBanned"))
                        return ("OK", JsonOutput["IsBanned"]);
                }
            }
            catch (Exception ex)
            {
                CustomLogger.LoggerAccessor.LogError($"[JsonDatabaseController] - ReadFromJsonFile errored out with exception: {ex}");
            }

            return ("NOTFOUND", false);
        }
    }
}
