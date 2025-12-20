using Horizon.RT.Common;

namespace Horizon.MUM.Models
{
    public class ChannelListFilter
    {
        public uint FieldID = 0;
        public MediusLobbyFilterMaskLevelType FilterMaskLevel = 0;
        public ulong BaselineValue = 0;
        public MediusComparisonOperator ComparisonOperator = MediusComparisonOperator.EQUAL_TO;
        public MediusLobbyFilterType LobbyFilterType = MediusLobbyFilterType.FILTER_EQUALS;

        public bool IsMatch(Channel channel)
        {
            if (channel == null)
                return false;

            #region FilterField
            switch (FilterMaskLevel)
            {
                case MediusLobbyFilterMaskLevelType.MediusLobbyFilterMaskLevel0: return true;
                case MediusLobbyFilterMaskLevelType.MediusLobbyFilterMaskLevel1: return ComparisonOperator.Compare(BaselineValue, channel.GenericField1);
                case MediusLobbyFilterMaskLevelType.MediusLobbyFilterMaskLevel2: return ComparisonOperator.Compare(BaselineValue, channel.GenericField2);
                case MediusLobbyFilterMaskLevelType.MediusLobbyFilterMaskLevel3: return ComparisonOperator.Compare(BaselineValue, channel.GenericField3);
                case MediusLobbyFilterMaskLevelType.MediusLobbyFilterMaskLevel4: return ComparisonOperator.Compare(BaselineValue, channel.GenericField4);
                case MediusLobbyFilterMaskLevelType.MediusLobbyFilterMaskLevel12: return ComparisonOperator.Compare(BaselineValue, channel.GenericField1 & channel.GenericField2);

                default: return false;
            }
            #endregion
        }
    }
}