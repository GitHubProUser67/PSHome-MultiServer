namespace QuazalServer.RDVServices.DDL.Models.SparkService
{
    public class SparkStats
    {
        // Fields based on a2 offsets seen in C++
        public int id { get; set; }         // a2 + 4
        public string name { get; set; }        // a2 + 8
        public int unk { get; set; }      // a2 + 0xC

        // Three float arrays (dynamic vectors)
        public List<float> vec1 { get; set; } // a2 + 0x10
        public List<float> vec2 { get; set; } // a2 + 0x20
        public List<float> vec3 { get; set; } // a2 + 0x30
    }
}
