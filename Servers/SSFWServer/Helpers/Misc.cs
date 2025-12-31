namespace SSFWServer.Helpers
{
    public class Misc
    {
        
        // Sandbox Environments
        public static List<string> homeEnvs = new()
        {
            "cprod", "cprodts", "cpreprod", "cpreprodb",
            "rc-qa", "rcdev", "rc-dev", "cqa-e",
            "cqa-a", "cqa-j", "cqa-h", "cqab-e",
            "cqab-a", "cqab-j", "cqab-h", "qcqa-e",
            "qcqa-a", "qcqa-j", "qcqa-h", "qcpreprod",
            "qcqab-e", "qcqab-a", "qcqab-j", "qcqab-h",
            "qcpreprodb", "coredev", "core-dev", "core-qa",
            "cdev", "cdev2", "cdev3", "cdeva", "cdevb", "cdevc",
            "nonprod1", "nonprod2", "nonprod3", "prodsp"
        };

        /// <summary>
        /// Extract a portion of a string winthin boundaries.
        /// <para>Extrait une portion d'un string entre des limites.</para>
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <param name="startToRemove">The amount of characters to remove from the left to the right.</param>
        /// <param name="endToRemove">The amount of characters to remove from the right to the left.</param>
        /// <returns>A string.</returns>
        public static string? ExtractPortion(string input, int startToRemove, int endToRemove)
        {
            if (input.Length < startToRemove + endToRemove)
                return null;

            return input[startToRemove..][..^endToRemove];
        }
    }

    public class SSFWUserData
    {
        public string? Username { get; set; }
        public int LogonCount { get; set; }
        public int IGA { get; set; }
    }
}
