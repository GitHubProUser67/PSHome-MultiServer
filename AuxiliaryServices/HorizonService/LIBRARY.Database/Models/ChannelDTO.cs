namespace Horizon.LIBRARY.Database.Models
{
    public class ChannelDTO
    {
        public int Id { get; set; }
        public int AppId { get; set; }
        public int MediusVersion { get; set; }
        public string Name { get; set; }
        public int MaxPlayers { get; set; }
        public ulong GenericField1 { get; set; }
        public ulong GenericField2 { get; set; }
        public ulong GenericField3 { get; set; }
        public ulong GenericField4 { get; set; }
        public int GenericFieldFilter { get; set; }
    }
}