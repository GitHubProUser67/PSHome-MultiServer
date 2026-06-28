using Tdf;

namespace Blaze3SDK.Blaze.Stats
{
    public class StatRawValue : TdfUnion
    {
        [TdfUnion(0)]
        private readonly float? mFloatValue;
        public float? FloatValue
        {
            get { return mFloatValue; }
            set { SetValue(value); }
        }

        [TdfUnion(1)]
        private readonly long? mIntValue;
        public long? IntValue
        {
            get { return mIntValue; }
            set { SetValue(value); }
        }

        [TdfUnion(2)]
        private readonly string? mStringValue;
        public string? StringValue
        {
            get { return mStringValue; }
            set { SetValue(value); }
        }
    }
}
