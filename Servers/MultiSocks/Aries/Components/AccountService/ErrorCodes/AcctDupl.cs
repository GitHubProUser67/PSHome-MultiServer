namespace MultiSocks.Aries.Components.AccountService.ErrorCodes
{
    public class AcctDupl : AbstractMessage
    {
        public override string _Name
        {
            get => "acctdupl";
        }

        public string? OPTS { get; set; }
    }
}
