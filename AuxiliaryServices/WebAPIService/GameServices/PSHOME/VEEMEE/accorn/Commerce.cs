namespace WebAPIService.GameServices.PSHOME.VEEMEE.accorn
{
    public static class Commerce
    {
        public static string Get_Count()
        {
            var counter = new VEEMEELoginCounter();
            var returnstring = Processor.Sign(
                $"{{\"count\":{counter.GetLoginCount("Voodooperson05")}}}"
            );
            return returnstring;
        }

        public static string Get_Ownership()
        {
            return Processor.Sign("{\"owner\":\"Voodooperson05\"}");
        }

        private class VEEMEELoginCounter
        {
            private readonly Dictionary<string, int> loginCounts;

            public VEEMEELoginCounter()
            {
                loginCounts = [];
            }

            public void ProcessLogin(string username)
            {
                if (loginCounts.TryGetValue(username, out var value))
                    loginCounts[username] = ++value;
                else
                    loginCounts.Add(username, 1);
            }

            public int GetLoginCount(string username)
            {
                return loginCounts.TryGetValue(username, out var value) ? value : 0;
            }
        }
    }
}
