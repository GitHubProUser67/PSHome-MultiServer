namespace QuazalServer.RDVServices.MissionClass
{
    public class Mission
    {
        public uint mID;
        public string? mCriteria;
        public uint mOasisName = 70870;
        public uint mOasisDescription = 70870;
        public uint mOasisRequirement = 70870;
        public uint mOasisDebrief = 70870;
        public byte mMinLevel;
        public byte mMaxLevel;
        public byte mMinParty;
        public byte mCommandoRequired;
        public byte mReconRequired;
        public byte mSpecialistRequired;
        public byte mFlags;
        public uint mAssetId;
    }
}
