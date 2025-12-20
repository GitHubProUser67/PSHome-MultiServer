using System;

namespace Horizon.LIBRARY.Database.Models
{
    public class PartyDTO
    {
        public int PartyId { get; set; }
        public int AppId { get; set; }
        public int MinPlayers { get; set; }
        public int MaxPlayers { get; set; }
        public string PartyName { get; set; }
        public string PartyPassword { get; set; }
        public int GenericField1 { get; set; }
        public int GenericField2 { get; set; }
        public int GenericField3 { get; set; }
        public int GenericField4 { get; set; }
        public int GenericField5 { get; set; }
        public int GenericField6 { get; set; }
        public int GenericField7 { get; set; }
        public int GenericField8 { get; set; }
        public string GameHostType { get; set; }
        public string Metadata { get; set; }
        public DateTime? PartyCreateDt { get; set; }
        public DateTime? PartyStartDt { get; set; }
        public DateTime? PartyEndDt { get; set; }
        public bool Destroyed { get; set; }
    }
}