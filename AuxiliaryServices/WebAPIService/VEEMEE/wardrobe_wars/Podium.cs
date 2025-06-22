using CustomLogger;
using NetworkLibrary.HTTP;
using HttpMultipartParser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
#if NET6_0_OR_GREATER
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
#endif
namespace WebAPIService.VEEMEE.wardrobe_wars
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
            string territory = string.Empty;
            string region = string.Empty;
            string time = string.Empty;
            string psnid = string.Empty;
            string language = string.Empty;
            string bracelet = string.Empty;

            string boundary = HTTPProcessor.ExtractBoundary(ContentType);

            if (!string.IsNullOrEmpty(boundary) && PostData != null)
            {
                using (MemoryStream ms = new MemoryStream(PostData))
                {
                    var data = MultipartFormDataParser.Parse(ms, boundary);

                    string serverFilePath = $"{apiPath}/VEEMEE/WW-Prod/User_Data";
                    Directory.CreateDirectory(serverFilePath);

                    territory = data.GetParameterValue("territory");
                    region = data.GetParameterValue("region");
                    time = data.GetParameterValue("time");
                    psnid = data.GetParameterValue("psnid");
                    language = data.GetParameterValue("language");
                    //if secure url send
                    bracelet = data.GetParameterValue("bracelet");
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
            string id = string.Empty;
            string previousid = string.Empty;
            string limitLocal = string.Empty;
            string territory = string.Empty;
            string region = string.Empty;
            string time = string.Empty;
            string psnid = string.Empty;
            string language = string.Empty;
            string psnNameFromFileName = string.Empty;
            int indexId = -1;

            if (PostData != null)
            {
                Dictionary<string, List<string>> urlEncodedData = HTTPProcessor.ExtractAndSortUrlEncodedPOSTData(PostData);
                id = urlEncodedData.TryGetValue("id", out var idList) && idList.Count > 0 ? idList[0] : string.Empty;
                previousid = urlEncodedData.TryGetValue("previous", out var previousidList) && previousidList.Count > 0 ? previousidList[0] : string.Empty;
                limitLocal = urlEncodedData.TryGetValue("limitLocal", out var limitLocalList) && limitLocalList.Count > 0 ? limitLocalList[0] : string.Empty;
                territory = urlEncodedData.TryGetValue("territory", out var territoryList) && territoryList.Count > 0 ? territoryList[0] : string.Empty;
                region = urlEncodedData.TryGetValue("region", out var regionList) && regionList.Count > 0 ? regionList[0] : string.Empty;
                time = urlEncodedData.TryGetValue("time", out var timeList) && timeList.Count > 0 ? timeList[0] : string.Empty;
                psnid = urlEncodedData.TryGetValue("psnid", out var psnidList) && psnidList.Count > 0 ? psnidList[0] : string.Empty;
                language = urlEncodedData.TryGetValue("language", out var languageList) && languageList.Count > 0 ? languageList[0] : string.Empty;

                //indexId = Convert.ToInt32(id.Split(".").First());

                string serverFilePath = $"{apiPath}/VEEMEE/WW-Prod/User_Data/{DateTime.UtcNow.ToString("yyyy-MM-dd")}/";

                int IDFromString = 0;
                if (previousid == "0")
                {
                    IDFromString = 1;
                }
                else
                {
                    IDFromString = Convert.ToInt32(previousid.Split('.').First());
                }

                string serverScoreFilePath = serverFilePath + $"/Scores/{IDFromString}";

                try {

                    if (Directory.Exists(serverFilePath))
                    {
                        string[] userProfiles = Directory.GetFiles(serverFilePath);

                        string userfileName = Path.GetFileName(userProfiles[IDFromString]);

                        psnNameFromFileName = userfileName;

                        string voteScore = "0";

                        if (Directory.Exists(serverScoreFilePath))
                        {
                            string[] userScoreProfiles = Directory.GetFiles(serverScoreFilePath);

                            voteScore = File.ReadAllText(serverFilePath + $"/{userScoreProfiles.Contains(psnid)}.txt");
                        }

                        return $"{DateTime.UtcNow.ToString("yyyy-MM-dd")}/{psnNameFromFileName},{psnNameFromFileName.Split('_').First()},{voteScore},{indexId},0";
                    }
                    else
                    {
                        LoggerAccessor.LogError($"[VEEMEE] - Podium: No date directory found for today in path {serverFilePath} !");
                        return $"0,{psnNameFromFileName.Split('_').First()},0,{id},0";
                    }
                } catch (Exception e)
                {
                    LoggerAccessor.LogWarn($"[VEEMEE] - Podium: Failed to find a image at index {id}");
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
            string id = string.Empty;
            string entrant_id = string.Empty;
            string vote = string.Empty;
            string territory = string.Empty;
            string region = string.Empty;
            string time = string.Empty;
            string psnid = string.Empty;
            string language = string.Empty;

            if (PostData != null)
            {
                using (MemoryStream ms = new MemoryStream(PostData))
                {
                    Dictionary<string, List<string>> urlEncodedData = HTTPProcessor.ExtractAndSortUrlEncodedPOSTData(PostData);
                    id = urlEncodedData.TryGetValue("id", out var idList) && idList.Count > 0 ? idList[0] : string.Empty;
                    entrant_id = urlEncodedData.TryGetValue("entrant_id", out var entrantList) && entrantList.Count > 0 ? entrantList[0] : string.Empty;
                    vote = urlEncodedData.TryGetValue("vote", out var voteList) && voteList.Count > 0 ? voteList[0] : string.Empty;
                    territory = urlEncodedData.TryGetValue("territory", out var territoryList) && territoryList.Count > 0 ? territoryList[0] : string.Empty;
                    region = urlEncodedData.TryGetValue("region", out var regionList) && regionList.Count > 0 ? regionList[0] : string.Empty;
                    time = urlEncodedData.TryGetValue("time", out var timeList) && timeList.Count > 0 ? timeList[0] : string.Empty;
                    psnid = urlEncodedData.TryGetValue("psnid", out var psnidList) && psnidList.Count > 0 ? psnidList[0] : string.Empty;
                    language = urlEncodedData.TryGetValue("language", out var languageList) && languageList.Count > 0 ? languageList[0] : string.Empty;

                    string serverFilePath = $"{apiPath}/VEEMEE/WW-Prod/User_Data/{DateTime.UtcNow.ToString("yyyy-MM-dd")}/Scores/{id}";
                    Directory.CreateDirectory(serverFilePath);

                    File.WriteAllText(serverFilePath + $"/{psnid}_{DateTime.UtcNow.ToString("hh-mm-ss")}.txt", vote);

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
            string id = string.Empty;
            string entrant_id = string.Empty;
            string config = string.Empty;
            string product = string.Empty;

            if (PostData != null)
            {
                using (MemoryStream ms = new MemoryStream(PostData))
                {
                    Dictionary<string, List<string>> urlEncodedData = HTTPProcessor.ExtractAndSortUrlEncodedPOSTData(PostData);
                    id = urlEncodedData.TryGetValue("id", out var idList) && idList.Count > 0 ? idList[0] : string.Empty;
                    entrant_id = urlEncodedData.TryGetValue("entrant_id", out var entrantList) && entrantList.Count > 0 ? entrantList[0] : string.Empty;

                    string serverFilePath = $"{apiPath}/VEEMEE/WW-Prod/User_Data/{DateTime.UtcNow.ToString("yyyy-MM-dd")}/";
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
            string[] rewardsTXT = null;
            string id = string.Empty;
            string entrant_id = string.Empty;

            Dictionary<string, List<string>> urlEncodedData = HTTPProcessor.ExtractAndSortUrlEncodedPOSTData(PostData);

            string RewardsResponse = string.Empty;

            if (PostData != null)
            {
                using (MemoryStream ms = new MemoryStream(PostData))
                {
                    id = urlEncodedData.TryGetValue("id", out var idList) && idList.Count > 0 ? idList[0] : string.Empty;
                    entrant_id = urlEncodedData.TryGetValue("entrant_id", out var entrantList) && entrantList.Count > 0 ? entrantList[0] : string.Empty;

                    string serverFilePath = $"{apiPath}/VEEMEE/WW-Prod/Server_Data";
                    Directory.CreateDirectory(serverFilePath);

                    if (File.Exists(serverFilePath + "/Rewards.txt"))
                    {
                        rewardsTXT = File.ReadAllLines(serverFilePath + "/Rewards.txt");

                        RewardsResponse = $"1,{rewardsTXT.Length},";

                        foreach (string rewardUUID in rewardsTXT)
                        {
                            RewardsResponse = RewardsResponse + $",{rewardUUID}";
                        }

                    }
                    else
                    {
                        LoggerAccessor.LogWarn($"[VEEMEE] - Podium: Using fallback Rewards txt! Please provide one in {serverFilePath + "/Rewards.txt"}");

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

            string screensXML = string.Empty;
            string id = string.Empty;
            string entrant_id = string.Empty;


            if (PostData != null)
            {
                using (MemoryStream ms = new MemoryStream(PostData))
                {

                    Dictionary<string, List<string>> urlEncodedData = HTTPProcessor.ExtractAndSortUrlEncodedPOSTData(PostData);
                    id = urlEncodedData.TryGetValue("id", out var idList) && idList.Count > 0 ? idList[0] : string.Empty;
                    entrant_id = urlEncodedData.TryGetValue("entrant_id", out var entrantList) && entrantList.Count > 0 ? entrantList[0] : string.Empty;

                    string serverFilePath = $"{apiPath}/VEEMEE/WW-Prod/Server_Data";
                    Directory.CreateDirectory(serverFilePath);
                    
                    if(File.Exists(serverFilePath + "/Screens.xml"))
                    {
                        screensXML = File.ReadAllText(serverFilePath + "/Screens.xml");
                    } else
                    {
                        LoggerAccessor.LogWarn($"[VEEMEE] - Podium: Using fallback Screens xml! Please provide one in {serverFilePath + "/Screens.xml"}");

                        screensXML = @"<Result>
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
            string territory = string.Empty;
            string Region = string.Empty;
            string time = string.Empty;
            string psnid = string.Empty;
            string bracelet = string.Empty;
            string secureme = string.Empty;

            if (PostData != null)
            {
                using (MemoryStream ms = new MemoryStream(PostData))
                {
                    Dictionary<string, List<string>> urlEncodedData = HTTPProcessor.ExtractAndSortUrlEncodedPOSTData(PostData);
                    territory = urlEncodedData.TryGetValue("territory", out var territoryList) && territoryList.Count > 0 ? territoryList[0] : string.Empty;
                    Region = urlEncodedData.TryGetValue("Region", out var regionList) && regionList.Count > 0 ? regionList[0] : string.Empty;
                    time = urlEncodedData.TryGetValue("time", out var timeList) && timeList.Count > 0 ? timeList[0] : string.Empty;
                    psnid = urlEncodedData.TryGetValue("psnid", out var psnidList) && psnidList.Count > 0 ? psnidList[0] : string.Empty;
                    bracelet = urlEncodedData.TryGetValue("bracelet", out var braceletList) && braceletList.Count > 0 ? braceletList[0] : string.Empty;
                    secureme = urlEncodedData.TryGetValue("secureme", out var securemeList) && securemeList.Count > 0 ? securemeList[0] : string.Empty;

                    ms.Flush();
                }
                //return id
                return $"{psnid}_{DateTime.UtcNow.ToString("yyyy-MM-dd")}";
            }

            return null;
        }

        //Save Part2
        public static string PostPhotoPart2(byte[] PostData, string ContentType, string apiPath, bool isSecure)
        {
            string territory = string.Empty;
            string Region = string.Empty;
            string time = string.Empty;
            string psnid = string.Empty;
            string bracelet = string.Empty;
            string secureme = string.Empty;
            string thefile = string.Empty;

            string serverFilePath = $"{apiPath}/VEEMEE/WW-Prod/User_Data/{DateTime.UtcNow.ToString("yyyy-MM-dd")}";
            Directory.CreateDirectory(serverFilePath);

            string boundary = HTTPProcessor.ExtractBoundary(ContentType);

            if (!string.IsNullOrEmpty(boundary) && PostData != null)
            {
                using (MemoryStream ms = new MemoryStream(PostData))
                {
                    var data = MultipartFormDataParser.Parse(ms, boundary);

                    territory = data.GetParameterValue("territory");
                    Region = data.GetParameterValue("region");
                    time = data.GetParameterValue("time");
                    psnid = data.GetParameterValue("psnid");

                    if(isSecure)
                    {
                        bracelet = data.GetParameterValue("bracelet");
                    } else
                    {
                        secureme = data.GetParameterValue("secureme");
                    }
                    thefile = data.GetParameterValue("thefile");

                    // Write the file data directly to the file.
                    using (var fileStream = File.Create(serverFilePath + $"/{psnid}_{DateTime.UtcNow.ToString("hh-mm-ss")}.jpg"))
                    {
                        data.Files.FirstOrDefault().Data.CopyTo(fileStream);
                    }

                    ms.Flush();
                }
                //return id
                return $"{psnid}_{DateTime.UtcNow.ToString("yyyy-MM-dd")}";
            }

            return null;
        }
#if NET6_0_OR_GREATER
        /// <summary>
        /// Special function to call images from the user submissions for //WardrobeWars/Images/ VEEMEE endpoint
        /// </summary>
        public static byte[] RequestWWImage(string ContentType, string apiPath, string absolutePath)
        {
            byte[] imgSubmission;
            string serverFilePath = $"{apiPath}/VEEMEE/WW-Prod/User_Data/{DateTime.UtcNow.ToString("yyyy-MM-dd")}/{Path.GetFileName(absolutePath)}";

            imgSubmission = File.ReadAllBytes(serverFilePath);

            using (MemoryStream inputStream = new MemoryStream(imgSubmission))
            {
                // Load the image from the byte array
                using (Image image = Image.Load(inputStream))
                {
                    // Target dimensions
                    int targetWidth = 340;
                    int targetHeight = 360;

                    // Resize the image to exactly fit the target size, cropping the sides if necessary
                    image.Mutate(ctx => ctx.Resize(new ResizeOptions
                    {
                        Mode = ResizeMode.Crop, // Ensures the image is cropped to fit exactly into the target size
                        Size = new Size(targetWidth, targetHeight),
                        Sampler = KnownResamplers.Lanczos3 // High-quality resampling
                    }));

                    // Save the final image to a MemoryStream
                    using (MemoryStream outputStream = new MemoryStream())
                    {
                        // Save the image in its original format (you can adjust format here if needed)
                        image.Save(outputStream, image.Metadata.DecodedImageFormat);

                        // Return the byte array
                        return outputStream.ToArray();
                    }
                }
            }
        }
#endif
    }
}
