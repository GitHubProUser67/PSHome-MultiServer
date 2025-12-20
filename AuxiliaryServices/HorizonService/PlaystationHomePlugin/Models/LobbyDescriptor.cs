namespace HorizonService.PlaystationHomePlugin.Models
{
    public class LobbyDescriptor
    {
        public string Type { get; set; }
        public string Code1 { get; set; }
        public string Code2 { get; set; }
        public string Code3 { get; set; }
        public string Code4 { get; set; }
        public string Description { get; set; }
        public string Id { get; set; }

        public static LobbyDescriptor Parse(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                CustomLogger.LoggerAccessor.LogError("[LobbyDescriptor] - Input string cannot be null or empty.");
                return null;
            }

            string[] parts = name.Split('|');

            if (parts.Length != 7)
            {
                CustomLogger.LoggerAccessor.LogError("[LobbyDescriptor] - Input string does not have the correct format.");
                return null;
            }

            return new LobbyDescriptor
            {
                Type = parts[0],
                Code1 = parts[1],
                Code2 = parts[2],
                Code3 = parts[3],
                Code4 = parts[4],
                Description = parts[5],
                Id = parts[6]
            };
        }

        // Optional: override ToString for easy display
        public override string ToString()
        {
            return $"Type: {Type}, Codes: {Code1}-{Code2}-{Code3}-{Code4}, Description: {Description}, Id: {Id}";
        }

    }
}
