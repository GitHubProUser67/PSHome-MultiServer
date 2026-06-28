namespace WebAPIService.GameServices.PSHOME.JUGGERNAUT.farm.plant
{
    public class plant_getxp
    {
        public static string ProcessGetXp(
            IDictionary<string, string> QueryParameters,
            string apiPath
        )
        {
            if (QueryParameters != null)
            {
                var user = QueryParameters["user"];

                if (!string.IsNullOrEmpty(user))
                {
                    Directory.CreateDirectory($"{apiPath}/juggernaut/farm/User_Data");

                    return File.Exists($"{apiPath}/juggernaut/farm/User_Data/{user}.xml")
                        ? File.ReadAllText($"{apiPath}/juggernaut/farm/User_Data/{user}.xml")
                        : "<xml><found>0</found></xml>";
                }
            }

            return null;
        }
    }
}
