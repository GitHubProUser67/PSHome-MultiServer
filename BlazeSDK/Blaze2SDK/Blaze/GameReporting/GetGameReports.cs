using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.GameReporting
{
    [TdfStruct]
    public struct GetGameReports
    {
        
        [TdfMember("MGRR")]
        public uint mMaxGameReport;
        
        /// <summary>
        /// Max String Length: 64
        /// </summary>
        [TdfMember("QNAM")]
        [StringLength(64)]
        public string mQueryName;
        
        /// <summary>
        /// Max String Length: 32
        /// </summary>
        [TdfMember("QVAR")]
        public List<string> mQueryVarValues;
        
    }
}
