using Newtonsoft.Json.Linq;

namespace WebAPIService.GameServices.PSHOME.NDREAMS
{
    public static class NDREAMSProfilesUtils
    {
        // Function to update XP
        public static int UpdateXP(JObject profile, int xpToAdd)
        {
            if (profile.ContainsKey("XP"))
            {
                var currentXP = profile["XP"]?.Value<int>();

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
            var PreviousLevel = 1;

            if (profile.ContainsKey("level"))
            {
                var ExtractedPreviousLevel = profile["level"]?.Value<int>();

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
            var xp = 0;
            var level = 1;

            var profile = JObject.Parse(json);

            if (profile.ContainsKey("XP"))
            {
                var currentXP = profile["XP"]?.Value<int>();

                if (currentXP != null)
                    xp = currentXP.Value;
            }

            if (profile.ContainsKey("level"))
            {
                var currentLevel = profile["level"]?.Value<int>();

                if (currentLevel != null)
                    level = currentLevel.Value;
            }

            return (xp, level);
        }
    }
}
