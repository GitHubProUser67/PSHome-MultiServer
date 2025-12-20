using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.GameReporting
{
    [TdfStruct]
    public struct GetGameReportView
    {
        
        [TdfMember("MAXR")]
        public uint mMaxRows;
        
        /// <summary>
        /// Max String Length: 64
        /// </summary>
        [TdfMember("NAME")]
        [StringLength(64)]
        public string mName;
        
        /// <summary>
        /// Max String Length: 32
        /// </summary>
        [TdfMember("QVAR")]
        public List<string> mQueryVarValues;
        
    }
}
