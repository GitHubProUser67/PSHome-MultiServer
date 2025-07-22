using Newtonsoft.Json.Linq;

namespace WebAPIService.GameServices.NDREAMS
{
    public static class NDREAMSProfilesUtils
    {
        // Function to update XP
        public static int UpdateXP(JObject profile, int xpToAdd)
        {
            if (profile.ContainsKey("XP"))
            {
                int? currentXP = profile["XP"]?.Value<int>();

                if (currentXP == null)
                    profile["XP"] = xpToAdd;
                else
                {
                    xpToAdd += currentXP.Value;
                    profile["XP"] = xpToAdd;
                }
            }
            else
                profile.Add("XP", xpToAdd);

            return xpToAdd;
        }

        // Function to update level
        public static (int, int) UpdateLevel(JObject profile, int levelToAdd)
        {
            int PreviousLevel = 1;

            if (profile.ContainsKey("level"))
            {
                int? ExtractedPreviousLevel = profile["level"]?.Value<int>();

                if (ExtractedPreviousLevel != null)
                    PreviousLevel = ExtractedPreviousLevel.Value;

                profile["level"] = levelToAdd;
            }
            else
                profile.Add("level", levelToAdd);

            return (PreviousLevel, levelToAdd);
        }

        public static (int, int) ExtractProfileProperties(string json)
        {
            int xp = 0;
            int level = 1;

            JObject profile = JObject.Parse(json);

            if (profile.ContainsKey("XP"))
            {
                int? currentXP = profile["XP"]?.Value<int>();

                if (currentXP != null)
                    xp = currentXP.Value;
            }

            if (profile.ContainsKey("level"))
            {
                int? currentLevel = profile["level"]?.Value<int>();

                if (currentLevel != null)
                    level = currentLevel.Value;
            }

            return (xp, level);
        }
    }
}
