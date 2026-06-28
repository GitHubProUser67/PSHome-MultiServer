using CustomLogger;
using HttpMultipartParser;
using MultiServerLibrary.HTTP;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace WebAPIService.GameServices.PSHOME.VEEMEE.wardrobe_wars
{
    internal class Podium
    {
        ///<summary>
        /// First entry must equal 1 to verify <br/>
        /// Second entry is for WW.Bracelet <br/>
        /// Third entry is for refresh timer <br/>
        /// Fourth entry is for display time for screens <br/>
        /// Fifth entry is for Refreshing Score on Kiosk <br/>
        /// Sixth entry is for Refreshing Score on Podium
        /// </summary>
        public static string Verify(byte[] PostData, string ContentType, string apiPath)
        {
            var boundary = HTTPProcessor.ExtractBoundary(ContentType);

            if (!string.IsNullOrEmpty(boundary) && PostData != null)
            {
                using (var ms = new MemoryStream(PostData))
                {
                    var data = MultipartFormDataParser.Parse(ms, boundary);

                    var serverFilePath = $"{apiPath}/VEEMEE/WW-Prod/User_Data";
                    Directory.CreateDirectory(serverFilePath);

                    var territory = data.GetParameterValue("territory");
                    var region = data.GetParameterValue("region");
                    var time = data.GetParameterValue("time");
                    var psnid = data.GetParameterValue("psnid");
                    var language = data.GetParameterValue("language");
                    //if secure url send
                    var bracelet = data.GetParameterValue("bracelet");
                    ms.Flush();
                }

                return "1,http://ww-prod.destinations.scea.com/WardrobeWars/bracelet/,30,60,60,60";
            }

            return null;
        }

        /// <summary>
        /// Entry 1 tells if Podium is active or not
        /// Entry 2 is PSNID
        /// Entry 3 is Score
        /// Entry 4 is id
        /// Entry 5 is localvote
        /// </summary>
        public static string CheckPodium(byte[] PostData, string ContentType, string apiPath)
        {
            var psnNameFromFileName = string.Empty;
            var indexId = -1;

            if (PostData != null)
            {
                var urlEncodedData = HTTPProcessor.ExtractAndSortUrlEncodedPOSTData(PostData);
                var id =
                    urlEncodedData.TryGetValue("id", out var idList) && idList.Count > 0
                        ? idList[0]
                        : string.Empty;
                var previousid =
                    urlEncodedData.TryGetValue("previous", out var previousidList)
                    && previousidList.Count > 0
                        ? previousidList[0]
                        : string.Empty;
                var limitLocal =
                    urlEncodedData.TryGetValue("limitLocal", out var limitLocalList)
                    && limitLocalList.Count > 0
                        ? limitLocalList[0]
                        : string.Empty;
                var territory =
                    urlEncodedData.TryGetValue("territory", out var territoryList)
                    && territoryList.Count > 0
                        ? territoryList[0]
                        : string.Empty;
                var region =
                    urlEncodedData.TryGetValue("region", out var regionList) && regionList.Count > 0
                        ? regionList[0]
                        : string.Empty;
                var time =
                    urlEncodedData.TryGetValue("time", out var timeList) && timeList.Count > 0
                        ? timeList[0]
                        : string.Empty;
                var psnid =
                    urlEncodedData.TryGetValue("psnid", out var psnidList) && psnidList.Count > 0
                        ? psnidList[0]
                        : string.Empty;
                var language =
                    urlEncodedData.TryGetValue("language", out var languageList)
                    && languageList.Count > 0
                        ? languageList[0]
                        : string.Empty;

                //indexId = Convert.ToInt32(id.Split(".").First());

                var serverFilePath =
                    $"{apiPath}/VEEMEE/WW-Prod/User_Data/{DateTime.UtcNow:yyyy-MM-dd}/";

                var IDFromString =
                    previousid == "0" ? 1 : Convert.ToInt32(previousid.Split('.').First());
                var serverScoreFilePath = serverFilePath + $"/Scores/{IDFromString}";

                try
                {
                    if (Directory.Exists(serverFilePath))
                    {
                        var userProfiles = Directory.GetFiles(serverFilePath);

                        var userfileName = Path.GetFileName(userProfiles[IDFromString]);

                        psnNameFromFileName = userfileName;

                        var voteScore = "0";

                        if (Directory.Exists(serverScoreFilePath))
                        {
                            var userScoreProfiles = Directory.GetFiles(serverScoreFilePath);

                            voteScore = File.ReadAllText(
                                serverFilePath + $"/{userScoreProfiles.Contains(psnid)}.txt"
                            );
                        }

                        return $"{DateTime.UtcNow:yyyy-MM-dd}/{psnNameFromFileName},{psnNameFromFileName.Split('_').First()},{voteScore},{indexId},0";
                    }
                    else
                    {
                        LoggerAccessor.LogError(
                            $"[VEEMEE] - Podium: No date directory found for today in path {serverFilePath} !"
                        );
                        return $"0,{psnNameFromFileName.Split('_').First()},0,{id},0";
                    }
                }
                catch (Exception e)
                {
                    LoggerAccessor.LogWarn(
                        $"[VEEMEE] - Podium: Failed to find a image at index {id}"
                    );
                    return $"0,{psnNameFromFileName.Split('_').First()},0,{id},0";
                }
            }

            return null;
        }

        /// <summary>
        /// Entry 1 tells 1 is successful Vote_Successful, 2 is Vote_Failure
        /// Entry 2 is if Vote_Successful send entrant score
        /// </summary>
        public static string RequestVote(byte[] PostData, string ContentType, string apiPath)
        {
            var vote = string.Empty;
            if (PostData != null)
            {
                using (var ms = new MemoryStream(PostData))
                {
                    var urlEncodedData = HTTPProcessor.ExtractAndSortUrlEncodedPOSTData(PostData);
                    var id =
                        urlEncodedData.TryGetValue("id", out var idList) && idList.Count > 0
                            ? idList[0]
                            : string.Empty;
                    var entrant_id =
                        urlEncodedData.TryGetValue("entrant_id", out var entrantList)
                        && entrantList.Count > 0
                            ? entrantList[0]
                            : string.Empty;
                    vote =
                        urlEncodedData.TryGetValue("vote", out var voteList) && voteList.Count > 0
                            ? voteList[0]
                            : string.Empty;
                    var territory =
                        urlEncodedData.TryGetValue("territory", out var territoryList)
                        && territoryList.Count > 0
                            ? territoryList[0]
                            : string.Empty;
                    var region =
                        urlEncodedData.TryGetValue("region", out var regionList)
                        && regionList.Count > 0
                            ? regionList[0]
                            : string.Empty;
                    var time =
                        urlEncodedData.TryGetValue("time", out var timeList) && timeList.Count > 0
                            ? timeList[0]
                            : string.Empty;
                    var psnid =
                        urlEncodedData.TryGetValue("psnid", out var psnidList)
                        && psnidList.Count > 0
                            ? psnidList[0]
                            : string.Empty;
                    var language =
                        urlEncodedData.TryGetValue("language", out var languageList)
                        && languageList.Count > 0
                            ? languageList[0]
                            : string.Empty;
                    var serverFilePath =
                        $"{apiPath}/VEEMEE/WW-Prod/User_Data/{DateTime.UtcNow:yyyy-MM-dd}/Scores/{id}";
                    Directory.CreateDirectory(serverFilePath);

                    File.WriteAllText(
                        serverFilePath + $"/{psnid}_{DateTime.UtcNow:hh-mm-ss}.txt",
                        vote
                    );

                    ms.Flush();
                }

                return $"1,{vote}";
            }

            return null;
        }

        /// <summary>
        /// Entry 1 tells 1 is successful score return, 0 or any other is not able to fetch
        /// Entry 2 is if Vote_Successful send entrant score
        /// Entry 3 is bool localVoted
        /// </summary>
        public static string RequestScore(byte[] PostData, string ContentType, string apiPath)
        {
            if (PostData != null)
            {
                using (var ms = new MemoryStream(PostData))
                {
                    var urlEncodedData = HTTPProcessor.ExtractAndSortUrlEncodedPOSTData(PostData);
                    var id =
                        urlEncodedData.TryGetValue("id", out var idList) && idList.Count > 0
                            ? idList[0]
                            : string.Empty;
                    var entrant_id =
                        urlEncodedData.TryGetValue("entrant_id", out var entrantList)
                        && entrantList.Count > 0
                            ? entrantList[0]
                            : string.Empty;

                    var serverFilePath =
                        $"{apiPath}/VEEMEE/WW-Prod/User_Data/{DateTime.UtcNow:yyyy-MM-dd}/";
                    ms.Flush();
                }

                return "1,10,3";
            }

            return null;
        }

        /// <summary>
        /// Entry 1 ~= WWReward.NO_RETURN = 0
        /// Entry 2 is how many rewards
        /// Entry 3 is for each index, AddRewardTicket
        /// </summary>
        public static string RequestRewards(byte[] PostData, string ContentType, string apiPath)
        {
            var urlEncodedData = HTTPProcessor.ExtractAndSortUrlEncodedPOSTData(PostData);

            var RewardsResponse = string.Empty;

            if (PostData != null)
            {
                using (var ms = new MemoryStream(PostData))
                {
                    var id =
                        urlEncodedData.TryGetValue("id", out var idList) && idList.Count > 0
                            ? idList[0]
                            : string.Empty;
                    var entrant_id =
                        urlEncodedData.TryGetValue("entrant_id", out var entrantList)
                        && entrantList.Count > 0
                            ? entrantList[0]
                            : string.Empty;
                    var serverFilePath = $"{apiPath}/VEEMEE/WW-Prod/Server_Data";
                    Directory.CreateDirectory(serverFilePath);

                    if (File.Exists(serverFilePath + "/Rewards.txt"))
                    {
                        var rewardsTXT = File.ReadAllLines(serverFilePath + "/Rewards.txt");

                        RewardsResponse = $"1,{rewardsTXT.Length},";

                        foreach (var rewardUUID in rewardsTXT)
                        {
                            RewardsResponse += $",{rewardUUID}";
                        }
                    }
                    else
                    {
                        LoggerAccessor.LogWarn(
                            $"[VEEMEE] - Podium: Using fallback Rewards txt! Please provide one in {serverFilePath + "/Rewards.txt"}"
                        );

                        RewardsResponse = $"1,1,DF7977BE-28684CDE-A2FE030B-7416242A";
                    }

                    ms.Flush();
                }

                return RewardsResponse;
            }

            return null;
        }

        /// <summary>
        /// Returns XML for various parts of Screens
        /// </summary>
        public static string RequestScreens(byte[] PostData, string ContentType, string apiPath)
        {
            var screensXML = string.Empty;
            if (PostData != null)
            {
                using (var ms = new MemoryStream(PostData))
                {
                    var urlEncodedData = HTTPProcessor.ExtractAndSortUrlEncodedPOSTData(PostData);
                    var id =
                        urlEncodedData.TryGetValue("id", out var idList) && idList.Count > 0
                            ? idList[0]
                            : string.Empty;
                    var entrant_id =
                        urlEncodedData.TryGetValue("entrant_id", out var entrantList)
                        && entrantList.Count > 0
                            ? entrantList[0]
                            : string.Empty;

                    var serverFilePath = $"{apiPath}/VEEMEE/WW-Prod/Server_Data";
                    Directory.CreateDirectory(serverFilePath);

                    if (File.Exists(serverFilePath + "/Screens.xml"))
                    {
                        screensXML = File.ReadAllText(serverFilePath + "/Screens.xml");
                    }
                    else
                    {
                        LoggerAccessor.LogWarn(
                            $"[VEEMEE] - Podium: Using fallback Screens xml! Please provide one in {serverFilePath + "/Screens.xml"}"
                        );

                        screensXML =
                            @"<Result>
    <!-- Winners -->
    <Winners>
        <Winner>
            <name>NAME_OF_WINNER</name>
            <type>TYPE</type>
            <score>10</score>
            <date></date>
            <tex>TEXTURE</tex>
        </Winner>
    </Winners>
    <!-- theme -->
    <Theme>Any Theme!</Theme>
    <!-- Rewards -->
    <Rewards>
    <!-- Reward Types PARTICIPANT_PRIZE/1 DAILY_WINNER_PRIZE/2 WEEKLY_WINNER_PRIZE/3 MONTHLY_WINNER_PRIZE/4 -->
        <Reward>
            <name>DF7977BE-28684CDE-A2FE030B-7416242A</name>
            <type>1</type>
            <tex>TEXTURE</tex>
        </Reward>
    </Rewards>
    <!-- Screens -->
    <Screens>
    <!-- Screen Types Winner/1 Theme/2 Reward/3 -->
        <Screen id=""1"">
            <Display>
                <type>1</type>
                <index>1</index>
            </Display>
        </Screen>
        
        <Screen id=""2"">
            <Display>
                <type>2</type>
                <index>2</index>
            </Display>
        </Screen>
        
        <Screen id=""3"">
            <Display>
                <type>3</type>
                <index>3</index>
            </Display>
        </Screen>
    </Screens>
</Result>";
                    }

                    ms.Flush();
                }

                return screensXML;
            }

            return null;
        }

        /// <summary>
        /// Entry 1 tells 1 is successful score return, 0 or any other is not able to fetch
        /// Entry 2 is if Vote_Successful send entrant score
        /// Entry 3 is bool localVoted
        /// </summary>
        public static string PostPhotoPart1(byte[] PostData, string ContentType)
        {
            var psnid = string.Empty;
            if (PostData != null)
            {
                using (var ms = new MemoryStream(PostData))
                {
                    var urlEncodedData = HTTPProcessor.ExtractAndSortUrlEncodedPOSTData(PostData);
                    var territory =
                        urlEncodedData.TryGetValue("territory", out var territoryList)
                        && territoryList.Count > 0
                            ? territoryList[0]
                            : string.Empty;
                    var Region =
                        urlEncodedData.TryGetValue("Region", out var regionList)
                        && regionList.Count > 0
                            ? regionList[0]
                            : string.Empty;
                    var time =
                        urlEncodedData.TryGetValue("time", out var timeList) && timeList.Count > 0
                            ? timeList[0]
                            : string.Empty;
                    psnid =
                        urlEncodedData.TryGetValue("psnid", out var psnidList)
                        && psnidList.Count > 0
                            ? psnidList[0]
                            : string.Empty;
                    var bracelet =
                        urlEncodedData.TryGetValue("bracelet", out var braceletList)
                        && braceletList.Count > 0
                            ? braceletList[0]
                            : string.Empty;
                    var secureme =
                        urlEncodedData.TryGetValue("secureme", out var securemeList)
                        && securemeList.Count > 0
                            ? securemeList[0]
                            : string.Empty;
                    ms.Flush();
                }
                //return id
                return $"{psnid}_{DateTime.UtcNow:yyyy-MM-dd}";
            }

            return null;
        }

        //Save Part2
        public static string PostPhotoPart2(
            byte[] PostData,
            string ContentType,
            string apiPath,
            bool isSecure
        )
        {
            var psnid = string.Empty;
            var serverFilePath = $"{apiPath}/VEEMEE/WW-Prod/User_Data/{DateTime.UtcNow:yyyy-MM-dd}";
            Directory.CreateDirectory(serverFilePath);

            var boundary = HTTPProcessor.ExtractBoundary(ContentType);

            if (!string.IsNullOrEmpty(boundary) && PostData != null)
            {
                using (var ms = new MemoryStream(PostData))
                {
                    var data = MultipartFormDataParser.Parse(ms, boundary);

                    var territory = data.GetParameterValue("territory");
                    var Region = data.GetParameterValue("region");
                    var time = data.GetParameterValue("time");
                    psnid = data.GetParameterValue("psnid");

                    if (isSecure)
                    {
                        var bracelet = data.GetParameterValue("bracelet");
                    }
                    else
                    {
                        var secureme = data.GetParameterValue("secureme");
                    }

                    var thefile = data.GetParameterValue("thefile");

                    // Write the file data directly to the file.
                    using (
                        var fileStream = File.Create(
                            serverFilePath + $"/{psnid}_{DateTime.UtcNow:hh-mm-ss}.jpg"
                        )
                    )
                    {
                        data.Files.FirstOrDefault().Data.CopyTo(fileStream);
                    }

                    ms.Flush();
                }
                //return id
                return $"{psnid}_{DateTime.UtcNow:yyyy-MM-dd}";
            }

            return null;
        }

        /// <summary>
        /// Special function to call images from the user submissions for //WardrobeWars/Images/ VEEMEE endpoint
        /// </summary>
        public static byte[] RequestWWImage(string ContentType, string apiPath, string absolutePath)
        {
            byte[] imgSubmission;
            var serverFilePath =
                $"{apiPath}/VEEMEE/WW-Prod/User_Data/{DateTime.UtcNow:yyyy-MM-dd}/{Path.GetFileName(absolutePath)}";

            imgSubmission = File.ReadAllBytes(serverFilePath);

            using (var inputStream = new MemoryStream(imgSubmission))
            {
                // Load the image from the byte array
                using (var image = Image.Load(inputStream))
                {
                    // Target dimensions
                    var targetWidth = 340;
                    var targetHeight = 360;

                    // Resize the image to exactly fit the target size, cropping the sides if necessary
                    image.Mutate(ctx =>
                        ctx.Resize(
                            new ResizeOptions
                            {
                                Mode = ResizeMode.Crop, // Ensures the image is cropped to fit exactly into the target size
                                Size = new Size(targetWidth, targetHeight),
                                Sampler = KnownResamplers.Lanczos3, // High-quality resampling
                            }
                        )
                    );

                    // Save the final image to a MemoryStream
                    using (var outputStream = new MemoryStream())
                    {
                        // Save the image in its original format (you can adjust format here if needed)
                        image.Save(outputStream, image.Metadata.DecodedImageFormat);

                        // Return the byte array
                        return outputStream.ToArray();
                    }
                }
            }
        }
    }
}
