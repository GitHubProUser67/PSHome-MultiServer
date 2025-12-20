using Tdf;

namespace Blaze3SDK.Blaze.GameManager
{
	public class GameSetupReason : TdfUnion
	{

		[TdfUnion(0)]
		private DatalessSetupContext? mDatalessSetupContext;
		public DatalessSetupContext? DatalessSetupContext { get { return mDatalessSetupContext; } set { SetValue(value); } }

		[TdfUnion(1)]
		private ResetDedicatedServerSetupContext? mResetDedicatedServerSetupContext;
		public ResetDedicatedServerSetupContext? ResetDedicatedServerSetupContext { get { return mResetDedicatedServerSetupContext; } set { SetValue(value); } }

		[TdfUnion(2)]
		private IndirectJoinGameSetupContext? mIndirectJoinGameSetupContext;
		public IndirectJoinGameSetupContext? IndirectJoinGameSetupContext { get { return mIndirectJoinGameSetupContext; } set { SetValue(value); } }

		[TdfUnion(3)]
		private MatchmakingSetupContext? mMatchmakingSetupContext;
		public MatchmakingSetupContext? MatchmakingSetupContext { get { return mMatchmakingSetupContext; } set { SetValue(value); } }

		[TdfUnion(4)]
		private IndirectMatchmakingSetupContext? mIndirectMatchmakingSetupContext;
		public IndirectMatchmakingSetupContext? IndirectMatchmakingSetupContext { get { return mIndirectMatchmakingSetupContext; } set { SetValue(value); } }

	}
}
