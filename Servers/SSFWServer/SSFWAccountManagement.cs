using CustomLogger;
using Newtonsoft.Json;
using SSFWServer.Helpers.FileHelper;
using System.Text;

namespace SSFWServer
{
    public class SSFWUserData
    {
        public string? Username { get; set; }
        public int LogonCount { get; set; }
        public int IGA { get; set; }
    }

    public class SSFWAccountManagement
    {
        public static int ReadOrMigrateAccount(byte[] extractedData, string? username, string? sessionid, string? key)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(sessionid))
                return -1;

            int logoncount = 1;
            string userprofilefile = $"{SSFWServerConfiguration.SSFWStaticFolder}/SSFW_Accounts/{username}.json";
            string olduserprofilefile = $"{SSFWServerConfiguration.SSFWStaticFolder}/SSFW_Accounts/{sessionid}.json";

            if (File.Exists(olduserprofilefile))
                File.Move(olduserprofilefile, userprofilefile);

            if (File.Exists(userprofilefile))
            {
                string? userprofiledata = FileHelper.ReadAllText(userprofilefile, key);
                if (!string.IsNullOrEmpty(userprofiledata))
                {
                    // Parsing JSON data to SSFWUserData object
                    SSFWUserData? userData = JsonConvert.DeserializeObject<SSFWUserData>(userprofiledata);
                    if (userData != null)
                    {
                        // Modifying the object if needed
                        userData.LogonCount += 1;
                        userData.Username = sessionid;
                        logoncount = userData.LogonCount;
                        File.WriteAllText($"{SSFWServerConfiguration.SSFWStaticFolder}/SSFW_Accounts/{username}.json", JsonConvert.SerializeObject(userData, Formatting.Indented));
                    }
                    else
                    {
                        LoggerAccessor.LogError($"[SSFWLogin] - Profile data couldn't be parsed at this location: {userprofilefile}");

                        logoncount = -1;
                    }
                }
                else
                {
                    LoggerAccessor.LogError($"[SSFWLogin] - Profile data couldn't be read at this location: {userprofilefile}");

                    logoncount = -1;
                }
            }
            else
            {
                Directory.CreateDirectory($"{SSFWServerConfiguration.SSFWStaticFolder}/SSFW_Accounts");

                // Parsing JSON data to SSFWUserData object
                SSFWUserData? userData = JsonConvert.DeserializeObject<SSFWUserData>($"{{\"Username\":\"{sessionid}\",\"LogonCount\":{logoncount},\"IGA\":0}}");
                if (userData != null)
                {
                    LoggerAccessor.LogInfo($"[SSFW] : Account Created - {Encoding.ASCII.GetString(extractedData)} - Session ID : {userData.Username}");
                    LoggerAccessor.LogInfo($"[SSFW] : Account Created - {Encoding.ASCII.GetString(extractedData)} - LogonCount : {userData.LogonCount}");
                    LoggerAccessor.LogInfo($"[SSFW] : Account Created - {Encoding.ASCII.GetString(extractedData)} - IGA : {userData.IGA}");

                    File.WriteAllText($"{SSFWServerConfiguration.SSFWStaticFolder}/SSFW_Accounts/{username}.json", JsonConvert.SerializeObject(userData, Formatting.Indented));
                }
                else
                {
                    LoggerAccessor.LogError("[SSFWLogin] - Profile data couldn't be Deserialized, call a dev!");

                    logoncount = -1;
                }
            }

            return logoncount;
        }

        public static bool CopyAccountProfile(string oldUsername, string newUsername, string oldSessionId, string newSessionId, string? key)
        {
            if (string.IsNullOrEmpty(oldUsername) || string.IsNullOrEmpty(oldSessionId) ||
                string.IsNullOrEmpty(newUsername) || string.IsNullOrEmpty(newSessionId))
                return false;

            string oldUserProfileFile = $"{SSFWServerConfiguration.SSFWStaticFolder}/SSFW_Accounts/{oldUsername}.json";
            string newUserProfileFile = $"{SSFWServerConfiguration.SSFWStaticFolder}/SSFW_Accounts/{newUsername}.json";

            // Check if the old user profile exists
            if (!File.Exists(oldUserProfileFile))
            {
                oldUserProfileFile = $"{SSFWServerConfiguration.SSFWStaticFolder}/SSFW_Accounts/{oldSessionId}.json";

                if (!File.Exists(oldUserProfileFile))
                    return false;
            }

            // Check if the new user profile already exists
            if (File.Exists(newUserProfileFile))
                return false;

            // Copy the old user profile to the new location
            try
            {
                File.Copy(oldUserProfileFile, newUserProfileFile);
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError($"[SSFWLogin] - Error occurred while copying profile: {ex}");

                return false;
            }

            // Read the copied user profile and update session ID
            string? userProfileData = FileHelper.ReadAllText(newUserProfileFile, key);
            if (string.IsNullOrEmpty(userProfileData))
            {
                LoggerAccessor.LogError($"[SSFWLogin] - Profile data couldn't be read at location: {newUserProfileFile}");

                return false;
            }

            SSFWUserData? userData = JsonConvert.DeserializeObject<SSFWUserData>(userProfileData);
            if (userData == null)
            {
                LoggerAccessor.LogError($"[SSFWLogin] - Error parsing user data for user: {newUsername}");

                return false;
            }

            userData.Username = newSessionId;
            userData.IGA = 0; // Reset IGA to prevent exploits.

            // Write back the updated user profile
            try
            {
                File.WriteAllText(newUserProfileFile, JsonConvert.SerializeObject(userData, Formatting.Indented));
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError($"[SSFWLogin] - Error occurred while updating profile: {ex.Message}");

                return false;
            }

            LoggerAccessor.LogInfo($"[SSFW] : Account Profile Copied - From: {oldUsername} ({oldSessionId}) To: {newUsername} ({newSessionId})");

            return true; // Success
        }

        public static bool AccountExists(string username, string sessionid)
        {
            if (File.Exists($"{SSFWServerConfiguration.SSFWStaticFolder}/SSFW_Accounts/{username}.json"))
                return true;

            if (File.Exists($"{SSFWServerConfiguration.SSFWStaticFolder}/SSFW_Accounts/{sessionid}.json"))
                return true;

            return false;
        }
    }
}
