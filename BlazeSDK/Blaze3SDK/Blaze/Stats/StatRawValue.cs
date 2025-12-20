using Tdf;

namespace Blaze3SDK.Blaze.Stats
{
	public class StatRawValue : TdfUnion
	{

		[TdfUnion(0)]
		private float? mFloatValue;
		public float? FloatValue { get { return mFloatValue; } set { SetValue(value); } }

		[TdfUnion(1)]
		private long? mIntValue;
		public long? IntValue { get { return mIntValue; } set { SetValue(value); } }

		[TdfUnion(2)]
		private string? mStringValue;
		public string? StringValue { get { return mStringValue; } set { SetValue(value); } }

	}
}
