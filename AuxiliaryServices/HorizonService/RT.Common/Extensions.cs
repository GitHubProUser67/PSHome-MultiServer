using System;
using System.Collections.Generic;

namespace Horizon.RT.Common
{
    public static class Extensions
    {
        #region MediusUniverseVariableInformationInfoFilter

        public static bool IsSet(this MediusUniverseVariableInformationInfoFilter value, MediusUniverseVariableInformationInfoFilter filter)
        {
            return (value & filter) == filter;
        }

        #endregion

        #region MediusComparisonOperator

        public static bool Compare(this MediusComparisonOperator op, ulong lhs, ulong rhs)
        {
            switch (op)
            {
                case MediusComparisonOperator.EQUAL_TO: return lhs == rhs;
                case MediusComparisonOperator.GREATER_THAN: return lhs > rhs;
                case MediusComparisonOperator.GREATER_THAN_OR_EQUAL_TO: return lhs >= rhs;
                case MediusComparisonOperator.LESS_THAN: return lhs < rhs;
                case MediusComparisonOperator.LESS_THAN_OR_EQUAL_TO: return lhs <= rhs;
                case MediusComparisonOperator.NOT_EQUALS: return lhs != rhs;
                default: return false;
            }
        }

        #endregion

        #region MediusLobbyFilter

        public static bool Compare(this MediusLobbyFilterType ft, ulong lhs, ulong rhs)
        {
            switch (ft)
            {
                case MediusLobbyFilterType.MediusLobbyFilterEqualsLobby: return true;
                case MediusLobbyFilterType.MediusLobbyFilterEqualsFilter: return lhs == rhs;
                case MediusLobbyFilterType.FILTER_EQUALS: return lhs == rhs;
                default: return false;
            }
        }

        #endregion
    }

    public static class TimeZoneInfoExtensions
    {

        public static string Abbreviation(this TimeZoneInfo Source)
        {

            var TimeZoneMap = new Dictionary<string, string>()
            {
                {"eastern standard time", "est"},
                {"mountain standard time", "mst"},
                {"central standard time", "cst"},
                {"pacific standard time", "pst"},
                {"alaska standard time", "akst"},
                {"hawaii-aleutian standard time", "hast"},
                {"atlantic standard time", "ast"},
                {"greenwich mean time", "gmt"},
                {"central european standard time", "cest"},
                {"indian standard time", "ist"},
                {"japan standard time", "jst"},
                {"australian eastern standard time", "aest"},
                // Add more timezones and acronyms as needed
            };

            return TimeZoneMap[Source.Id.ToLower()].ToUpper();

        }

    }
}