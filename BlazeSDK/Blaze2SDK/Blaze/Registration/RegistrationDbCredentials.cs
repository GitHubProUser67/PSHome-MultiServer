using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze2SDK.Blaze.Registration
{
    [TdfStruct]
    public struct RegistrationDbCredentials
    {
        
        /// <summary>
        /// Max String Length: 128
        /// </summary>
        [TdfMember("HOST")]
        [StringLength(128)]
        public string mDbHost;
        
        /// <summary>
        /// Max String Length: 64
        /// </summary>
        [TdfMember("NAME")]
        [StringLength(64)]
        public string mDbInstanceName;
        
        /// <summary>
        /// Max String Length: 64
        /// </summary>
        [TdfMember("PASS")]
        [StringLength(64)]
        public string mDbPassword;
        
        [TdfMember("PORT")]
        public uint mDbPort;
        
        /// <summary>
        /// Max String Length: 64
        /// </summary>
        [TdfMember("SCHM")]
        [StringLength(64)]
        public string mDbSchema;
        
        /// <summary>
        /// Max String Length: 64
        /// </summary>
        [TdfMember("USER")]
        [StringLength(64)]
        public string mDbUser;
        
    }
}
