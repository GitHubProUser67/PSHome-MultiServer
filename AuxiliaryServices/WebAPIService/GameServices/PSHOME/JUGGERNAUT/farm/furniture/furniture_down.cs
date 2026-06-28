namespace WebAPIService.GameServices.PSHOME.JUGGERNAUT.farm.furniture
{
    public class furniture_down
    {
        public static string ProcessDown(
            IDictionary<string, string> QueryParameters,
            string apiPath
        )
        {
            if (QueryParameters != null)
            {
                var user = QueryParameters["user"];
                var layout = QueryParameters["layout"];

                if (!string.IsNullOrEmpty(user) && !string.IsNullOrEmpty(layout))
                {
                    return File.Exists($"{apiPath}/juggernaut/farm/User_Data/{user}/{layout}.xml")
                        ? File.ReadAllText(
                            $"{apiPath}/juggernaut/farm/User_Data/{user}/{layout}.xml"
                        )
                        : string.Empty;
                }
            }

            return null;
        }
    }
}
