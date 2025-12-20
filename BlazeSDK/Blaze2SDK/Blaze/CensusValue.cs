using System.ComponentModel.DataAnnotations;
using Tdf;

namespace Blaze3SDK.Blaze
{
    public class CensusValue : TdfUnion
    {

        [TdfUnion(0)]
        private uint? mNumValue;
        public uint? NumValue { get { return mNumValue; } set { SetValue(value); } }


        [TdfUnion(1)]
        [StringLength(512)]
        private string? mStringValue;
        /// <summary>
        /// Max String Length: 512
        /// </summary>
        public string? StringValue { get { return mStringValue; } set { SetValue(value); } }

    }
}
