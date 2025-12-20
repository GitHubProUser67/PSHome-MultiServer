using System.Collections.Generic;
using WebAPIService.GameServices.PSHOME.VEEMEE;

namespace WebAPIService.GameServices.PSHOME.VEEMEE.accorn
{
    public static class Commerce
    {
        public static string Get_Count()
        {
            VEEMEELoginCounter counter = new VEEMEELoginCounter();
            string returnstring = Processor.Sign($"{{\"count\":{counter.GetLoginCount("Voodooperson05")}}}");
            counter = null;
            return returnstring;
        }

        public static string Get_Ownership()
        {
            return Processor.Sign("{\"owner\":\"Voodooperson05\"}");
        }

        private class VEEMEELoginCounter
        {
            private Dictionary<string, int> loginCounts;

            public VEEMEELoginCounter()
            {
                loginCounts = new Dictionary<string, int>();
            }

            public void ProcessLogin(string username)
            {
                if (loginCounts.TryGetValue(username, out int value))
                    loginCounts[username] = ++value;
                else
                    loginCounts.Add(username, 1);
            }

            public int GetLoginCount(string username)
            {
                if (loginCounts.TryGetValue(username, out int value))
                    return value;

                return 0;
            }
        }
    }
}
