namespace MultiSocks.Aries.Components.AuthService.News
{
    public class NucleusNewsOut : AbstractMessage
    {
        public override string _Name
        {
            get => "news";
        }
        public string? NUCLEUS_CREATE_URL { get; set; } =
            "\"http://gos.ea.com/easo/editorial/common/2008/nucleus/create.jsp\"";
        public string? NUCLEUS_ADDED_URL { get; set; } =
            "\"http://gos.ea.com/easo/editorial/common/2008/nucleus/added.jsp\"";
        public string? NUCLEUS_INCOMPLETE_URL { get; set; } =
            "\"http://gos.ea.com/easo/editorial/common/2008/nucleus/incomplete.jsp\"";
        public string? NUCLEUS_CREATE_INFO_URL { get; set; } =
            "\"http://gos.ea.com/easo/editorial/common/2008/nucleus/create_info.jsp\"";
        public string? NUCLEUS_DUPACCT_INFO_URL { get; set; } =
            "\"http://gos.ea.com/easo/editorial/common/2008/nucleus/dupacct.jsp\"";
        public string? NUCLEUS_DEACTIVATED_INFO_URL { get; set; } =
            "\"http://gos.ea.com/easo/editorial/common/2008/nucleus/desactived.jsp\"";
    }
}
