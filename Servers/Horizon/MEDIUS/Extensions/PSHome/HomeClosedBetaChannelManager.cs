using System.Text.RegularExpressions;
using Horizon.MUM.Models;
using Horizon.RT.Common;
using MultiServerLibrary.Extension.NET;

namespace Horizon.MEDIUS.Extensions.PSHome
{
    public partial class HomeClosedBetaChannelManager
    {
        private static bool initiated = false;

        private static readonly ConcurrentList<int> chatChannelIds = new() { 1 };

        public static void InitiateBetaChannelsId(string sceneListPath)
        {
            if (initiated)
                return;

            if (File.Exists(sceneListPath))
            {
                lock (chatChannelIds)
                {
                    chatChannelIds.Clear();

                    try
                    {
                        // Use Regex to extract all ChannelID="..."
                        foreach (Match match in MyRegex().Matches(File.ReadAllText(sceneListPath)))
                        {
                            if (int.TryParse(match.Groups[1].Value, out var id))
                            {
#if DEBUG
                                CustomLogger.LoggerAccessor.LogInfo(
                                    $"[HomeClosedBetaChannelManager] - Adding chat channel with id:{id}."
                                );
#endif
                                chatChannelIds.Add(id);
                            }
                        }

                        initiated = true;
                    }
                    catch (Exception ex)
                    {
                        CustomLogger.LoggerAccessor.LogWarn(
                            $"[HomeClosedBetaChannelManager] - The Closed beta scenelist located at: {sceneListPath} is not in the right format (Exception:{ex}), make sure to insert a closed beta scenelist."
                        );
                    }
                }
            }
            else
                CustomLogger.LoggerAccessor.LogWarn(
                    $"[HomeClosedBetaChannelManager] - The Closed beta scenelist located at: {sceneListPath} was not found.\nNOTE: You need a closed beta scenelist to make early Home revisions work properly, they expect the chat channels to be created in advance."
                );
        }

        public static async Task GenerateOrUpdateChatChannels(int ApplicationId)
        {
            if (!initiated)
                return;

            foreach (var channelId in chatChannelIds)
            {
                var currentChannel = Program.MUMManager.GetChannelByChannelId(
                    channelId,
                    ApplicationId
                );

                if (currentChannel == null)
                {
                    await Program.MUMManager.AddChannel(
                        new Channel(channelId, ApplicationId, 113)
                        {
                            Name = "HomeLobby",
                            Type = ChannelType.Lobby,
                            GenericField2 = (ulong)channelId,
                            GenericField3 = 1,
                            MinPlayers = 0,
                            MaxPlayers = 32,
                            GenericFieldLevel =
                                MediusWorldGenericFieldLevelType.MediusWorldGenericFieldLevel23,
                        }
                    );
                }
            }
        }

        [GeneratedRegex(@"ChannelID\s*=\s*""(\d+)""")]
        private static partial Regex MyRegex();
    }
}
