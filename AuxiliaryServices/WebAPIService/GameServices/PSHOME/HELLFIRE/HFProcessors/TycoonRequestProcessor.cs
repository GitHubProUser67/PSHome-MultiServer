using CustomLogger;
using HttpMultipartParser;
using MultiServerLibrary.HTTP;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using WebAPIService.GameServices.PSHOME.HELLFIRE.Entities.HomeTycoon;
using WebAPIService.GameServices.PSHOME.HELLFIRE.Helpers;
using WebAPIService.GameServices.PSHOME.HELLFIRE.Helpers.Tycoon;

namespace WebAPIService.GameServices.PSHOME.HELLFIRE.HFProcessors
{
    public class TycoonRequestProcessor
    {
        public static int DefaultGiftNumber { get; set; } = 5;

        public static readonly string DefaultBuildings = @"<COM_GD_HighRiseBusiness_B>COM_GD_HighRiseBusiness_B</COM_GD_HighRiseBusiness_B>
                                      <COM_NE_BusinessTower_A>COM_NE_BusinessTower_A</COM_NE_BusinessTower_A>
                                      <COM_NE_HighRiseBusiness_A>COM_NE_HighRiseBusiness_A</COM_NE_HighRiseBusiness_A>
                                      <COM_NE_HighRiseBusiness_B>COM_NE_HighRiseBusiness_B</COM_NE_HighRiseBusiness_B>
                                      <COM_NE_Mall>COM_NE_Mall</COM_NE_Mall>
                                      <COM_NE_Factory>COM_NE_Factory</COM_NE_Factory>
                                      <RES_NE_Estate_A>RES_NE_Estate_A</RES_NE_Estate_A>
                                      <RES_NE_Estate_B>RES_NE_Estate_B</RES_NE_Estate_B>
                                      <RES_NE_HighRiseCondo_A>RES_NE_HighRiseCondo_A</RES_NE_HighRiseCondo_A>
                                      <RES_NE_HighRiseCondo_B>RES_NE_HighRiseCondo_B</RES_NE_HighRiseCondo_B>
                                      <RES_NE_SmApartment_B>RES_NE_SmApartment_B</RES_NE_SmApartment_B>
                                      <RES_NE_LgApartment_B>RES_NE_LgApartment_B</RES_NE_LgApartment_B>
                                      <UTL_NE_ElectricTower>UTL_NE_ElectricTower</UTL_NE_ElectricTower>
                                      <UTL_NE_Highway_A>UTL_NE_Highway_A</UTL_NE_Highway_A>
                                      <UTL_NE_Lake_A>UTL_NE_Lake_A</UTL_NE_Lake_A>
                                      <UTL_NE_Lake_B>UTL_NE_Lake_B</UTL_NE_Lake_B>
                                      <UTL_NE_NovusRecruitStation>UTL_NE_NovusRecruitStation</UTL_NE_NovusRecruitStation>
                                      <UTL_NE_Road_Straight_TreeCover>UTL_NE_Road_Straight_TreeCover</UTL_NE_Road_Straight_TreeCover>
                                      <Park2>Park2</Park2>
                                      <RES_GD_LgApartment_A>RES_GD_LgApartment_A</RES_GD_LgApartment_A>";

        public static readonly Dictionary<int, (int Amount, int Cost, int Sale)> WorkerPackages = new Dictionary<int, (int Amount, int Cost, int Sale)>
        {
            {1, (10, 2, 2)},
            {2, (22, 4, 4)},
            {3, (36, 6, 6)},
            {4, (50, 8, 8)},
            {5, (70, 10, 10)}
        };

        public static readonly Dictionary<string, (int Cost, int Sale)> ServiceCosts = new Dictionary<string, (int Cost, int Sale)>
        {
            { "ChangeTimeOfDay", (2, 1) },
            { "CollectAllRevenue", (1, 1) },
            { "BuySuburb", (175, 150) }
        };

        public static string ProcessMainPHP(byte[] PostData, string ContentType, string PHPSessionID, string WorkPath, bool https)
        {
            if (PostData == null || string.IsNullOrEmpty(ContentType))
                return null;

            string Command = string.Empty;
            string UserID = string.Empty;
            string DisplayName = string.Empty;
            string TownID = string.Empty;
            string InstanceID = string.Empty;
            string Region = string.Empty;
            string NumPlayers = string.Empty;
            string boundary = HTTPProcessor.ExtractBoundary(ContentType);

            if (boundary != null)
            {
                using (MemoryStream ms = new MemoryStream(PostData))
                {
                    var data = MultipartFormDataParser.Parse(ms, boundary);
                    Command = data.GetParameterValue("Command");
                    try
                    {
                        UserID = data.GetParameterValue("UserID");
                    }
                    catch
                    {
                        // Not Important.
                    }
                    try
                    {
                        DisplayName = data.GetParameterValue("DisplayName");
                    }
                    catch
                    {
                        // Not Important.
                    }
                    try
                    {
                        TownID = data.GetParameterValue("TownID");
                    }
                    catch
                    {
                        // Not Important.
                    }
                    try
                    {
                        InstanceID = data.GetParameterValue("InstanceID").Split('.')[0]; // Strip decimal.
                    }
                    catch
                    {
                        // Not Important.
                    }
                    try
                    {
                        Region = data.GetParameterValue("Region");
                    }
                    catch
                    {
                        // Not Important.
                    }
                    try
                    {
                        NumPlayers = data.GetParameterValue("NumPlayers");
                    }
                    catch
                    {
                        // Not Important.
                    }
                }

                if (!string.IsNullOrEmpty(Command))
                {
                    int i = 0;
                    string ServerFilesPath = $"{WorkPath}/HomeTycoon/Server_Data";
                    string userDataPath = $"{WorkPath}/HomeTycoon/User_Data/{UserID}";
                    string userConfigFilePath = userDataPath + $"/Profile.xml";
#if DEBUG
                    LoggerAccessor.LogInfo($"[TycoonRequestProcessor] - Client issued command:{Command}.");
#endif

                    // Ensure the town Instance lookup is initialized before doing anything.
                    TownInstance.EnsureIndexLoadedAsync(WorkPath).Wait();

                    switch (Command)
                    {
                        case "VersionCheck":
                            return $"<Response><URL>{(https ? "https" : "http")}://game2.hellfiregames.com/HomeTycoon</URL></Response>";
                        case "RequestNPTicket":
                            return NPTicket.RequestNPTicket(PostData, boundary, true); // We enable cross-save as friend fetching will be broken if not.
                        case "RequestDefaultTownInstance":
                            return TownInstance.RequestDefaultTownInstance();
                        case "RequestTownInstance":
                            return TownInstance.RequestTownInstance(UserID, DisplayName, TownID, WorkPath);
                        case "RequestTown":
                            Thread.Sleep(3000); // Why is that in here? Because the game is so bugged that responding too fast makes it crash (busy loading building ressources).
                            return TownInstance.RequestTown(InstanceID, WorkPath);
                        case "HasUser":
                        case "RequestUser":
                        case "RequestVisitingUser":
                            if (File.Exists(userConfigFilePath))
                                return $"<Response>{File.ReadAllText(userConfigFilePath)}</Response>";
                            else
                                return $"<Response>{User.DefaultHomeTycoonProfile}</Response>";
                        case "RequestUserTowns":
                            {
                                StringBuilder townsNameBuilder = new StringBuilder("<Response>");

                                foreach (string cityName in TownInstance.RequestTownsName(UserID, WorkPath))
                                {
                                    townsNameBuilder.Append($"<{cityName}><TownID>{TownInstance.TownNameToID(cityName)}</TownID></{cityName}>");
                                }

                                townsNameBuilder.Append("</Response>");

                                return townsNameBuilder.ToString();
                            }
                        case "RequestTowns":
                            return TownInstance.RequestTowns(PostData, boundary, UserID, DisplayName, WorkPath);
                        case "QueryMotd":
                            string motdFilePath = ServerFilesPath + "/MOTD.xml";

                            if (File.Exists(motdFilePath))
                                return $"<Response>{File.ReadAllText(motdFilePath)}</Response>";
                            else
                            {
                                return @"<Response>
                                            <Motd>

                                                <en>
                                                Welcome to Home Tycoon!

                                                Ready to begin building your new city? Your assistant will get you started, and you can
                                                access everything you need from your Mayor's Desk by pressing the button.

                                                Earn free rewards by visiting the Train Station from your Mayor's Desk, where you'll be
                                                able to visit other Home Tycoon cities, hang out with friends, and maybe even get your
                                                town onto the leaderboards!

                                                Have fun building your dream city, Mayor!
                                                </en>

                                                <es>
                                                ¡Bienvenido a Home Tycoon!

                                                ¿Listo para comenzar a construir tu nueva ciudad? Tu asistente te ayudará a empezar y
                                                podrás acceder a todo lo que necesitas desde el Escritorio del Alcalde presionando el botón.

                                                Obtén recompensas gratis visitando la Estación de Tren desde el Escritorio del Alcalde,
                                                donde podrás visitar otras ciudades, pasar el rato con amigos y quizás llevar tu ciudad a las clasificaciones.

                                                ¡Diviértete construyendo tu ciudad soñada, Alcalde!
                                                </es>

                                                <fr>
                                                Bienvenue dans Home Tycoon !

                                                Prêt à commencer à construire votre nouvelle ville ? Votre assistant vous aidera à démarrer
                                                et vous pourrez accéder à tout ce dont vous avez besoin depuis le Bureau du Maire en appuyant sur le bouton.

                                                Gagnez des récompenses gratuites en visitant la Gare depuis le Bureau du Maire, où vous
                                                pourrez visiter d'autres villes, passer du temps avec vos amis et peut-être même placer votre ville au classement !

                                                Amusez-vous à construire la ville de vos rêves, Monsieur/Madame le Maire !
                                                </fr>

                                                <de>
                                                Willkommen bei Home Tycoon!

                                                Bereit, deine neue Stadt zu bauen? Dein Assistent hilft dir beim Start und du kannst alles
                                                vom Bürgermeister-Schreibtisch aus erreichen, indem du die Taste drückst.

                                                Verdiene kostenlose Belohnungen, indem du den Bahnhof vom Bürgermeister-Schreibtisch aus besuchst,
                                                andere Städte erkundest, Freunde triffst und vielleicht deine Stadt in die Bestenliste bringst!

                                                Viel Spaß beim Aufbau deiner Traumstadt, Bürgermeister!
                                                </de>

                                                <zh>
                                                欢迎来到《Home Tycoon》！

                                                准备开始建设你的新城市了吗？你的助手会帮助你起步，你可以通过按下按钮从市长办公桌访问所需的一切。

                                                访问市长办公桌的火车站可获得免费奖励，你可以参观其他 Home Tycoon 城市、与朋友一起玩，
                                                甚至让你的城市登上排行榜！

                                                祝你建设梦想之城愉快，市长！
                                                </zh>

                                                <ja>
                                                Home Tycoonへようこそ！

                                                新しい街づくりを始める準備はできましたか？アシスタントがスタートをサポートし、ボタンを押すことで
                                                市長デスクから必要なすべてにアクセスできます。

                                                市長デスクから鉄道駅を訪れると無料報酬を獲得できます。他のHome Tycoonの街を訪れたり、
                                                友達と遊んだり、あなたの街がランキングに載るかもしれません！

                                                理想の街づくりを楽しんでください、市長！
                                                </ja>

                                                <ko>
                                                Home Tycoon에 오신 것을 환영합니다!

                                                새로운 도시 건설을 시작할 준비가 되었나요? 보조가 시작을 도와드리며 버튼을 눌러
                                                시장 책상에서 필요한 모든 것에 접근할 수 있습니다.

                                                시장 책상에서 기차역을 방문하면 무료 보상을 받을 수 있고, 다른 도시를 방문하고 친구들과 놀며
                                                도시를 순위에 올릴 수도 있습니다!

                                                꿈의 도시를 건설하는 즐거움을 느껴보세요, 시장님!
                                                </ko>

                                                <ru>
                                                Добро пожаловать в Home Tycoon!

                                                Готовы начать строить ваш новый город? Ваш помощник поможет вам начать, и вы сможете получить доступ
                                                ко всему необходимому с рабочего стола мэра, нажав кнопку.

                                                Зарабатывайте бесплатные награды, посещая вокзал с рабочего стола мэра, посещайте другие города,
                                                встречайтесь с друзьями и, возможно, поднимите свой город в таблице лидеров!

                                                Удачи в строительстве города мечты, мэр!
                                                </ru>

                                                <pt>
                                                Bem-vindo ao Home Tycoon!

                                                Pronto para começar a construir sua nova cidade? Seu assistente vai te ajudar e você pode acessar tudo
                                                o que precisa na Mesa do Prefeito pressionando o botão.

                                                Ganhe recompensas grátis ao visitar a Estação de Trem na Mesa do Prefeito, visite outras cidades,
                                                encontre amigos e coloque sua cidade no ranking!

                                                Divirta-se construindo sua cidade dos sonhos, Prefeito!
                                                </pt>

                                                <ar>
                                                مرحبًا بك في Home Tycoon!

                                                هل أنت مستعد لبدء بناء مدينتك الجديدة؟ سيساعدك مساعدك في البداية، ويمكنك الوصول إلى كل ما تحتاجه
                                                من مكتب العمدة بالضغط على الزر.

                                                احصل على مكافآت مجانية من خلال زيارة محطة القطار من مكتب العمدة، حيث يمكنك زيارة مدن أخرى،
                                                وقضاء الوقت مع الأصدقاء، وربما إيصال مدينتك إلى لوائح المتصدرين!

                                                استمتع ببناء مدينتك الحلم، أيها العمدة!
                                                </ar>

                                            </Motd>
                                        </Response>";
                            };
                        case "QueryServerGlobals":
#if DEBUG
                            return "<Response><GlobalHard>1</GlobalHard><GlobalWrinkles>1</GlobalWrinkles></Response>";
#else
                            return "<Response><GlobalHard>0</GlobalHard><GlobalWrinkles>0</GlobalWrinkles></Response>";
#endif
                        case "QueryPrices":
                            {
                                // Make everything free for now.
                                const int Cost = 0;
                                const int Sale = 0;

                                StringBuilder pricesBuilder = new StringBuilder("<Response><Buildings>");

                                foreach (string building in TycoonFileList.BuildingsFilenames)
                                {
                                    string buildingWithoutXml = building.Replace(".xml", string.Empty);
                                    pricesBuilder.Append($"<{buildingWithoutXml}><Cost>{Cost}</Cost><Sale>{Sale}</Sale></{buildingWithoutXml}>");
                                }

                                pricesBuilder.Append("</Buildings><Vehicles>");

                                foreach (string vehicle in TycoonFileList.VehiclesFilenames)
                                {
                                    string vehicleWithoutXml = vehicle.Replace(".xml", string.Empty);
                                    pricesBuilder.Append($"<{vehicleWithoutXml}><Cost>{Cost}</Cost><Sale>{Sale}</Sale></{vehicleWithoutXml}>");
                                }

                                pricesBuilder.Append("</Vehicles><Expansions>");

                                foreach (string expension in TycoonFileList.ExpensionFilenames)
                                {
                                    pricesBuilder.Append($"<{expension}><Cost>{Cost}</Cost><Sale>{Sale}</Sale></{expension}>");
                                }

                                pricesBuilder.Append("</Expansions><WorkerPackages>");

                                lock (WorkerPackages)
                                {
                                    for (i = 1; i <= WorkerPackages.Count; i++)
                                    {
                                        var pkg = WorkerPackages[i];
                                        pricesBuilder.Append($"<{i}><Amount>{pkg.Amount}</Amount><Cost>{pkg.Cost}</Cost><Sale>{pkg.Sale}</Sale></{i}>");
                                    }
                                }

                                pricesBuilder.Append("</WorkerPackages><Services>");

                                lock (ServiceCosts)
                                {
                                    foreach (var service in ServiceCosts)
                                    {
                                        pricesBuilder.Append(
                                            $"<{service.Key}><Cost>{service.Value.Cost}</Cost><Sale>{service.Value.Sale}</Sale></{service.Key}>");
                                    }
                                }

                                pricesBuilder.Append("</Services></Response>");

                                return pricesBuilder.ToString();
                            }
                        case "QueryBoosters":
                            return "<Response>" +
                                "<Booster>" +
                                "<Type>1</Type><Value>1</Value><Param>1</Param>" +
                                "<UUID>7A8BC3DB-399F4457-8117F099-D9F5D132</UUID><Reward>true</Reward>" +
                                "</Booster>" +
                                "</Response>";
                        case "QueryHoldbacks":

                            // DateTime unused in the script (only for logging).
                            DateTime now = DateTime.UtcNow;
                            StringBuilder holdbacksBuilder = new StringBuilder("<Response>");

                            foreach (var building in TycoonHoldbacks.Buildings)
                            {
                                holdbacksBuilder.Append(
                                    $"<{i}><Type>Building</Type><Name>{building}</Name><Date>{now}</Date></{i}>");

                                i++;
                            }

                            foreach (var building in TycoonHoldbacks.ExpansionPacks)
                            {
                                holdbacksBuilder.Append(
                                    $"<{i}><Type>ExpansionPack</Type><Name>{building}</Name><Date>{now}</Date></{i}>");

                                i++;
                            }

                            foreach (var building in TycoonHoldbacks.Vehicles)
                            {
                                holdbacksBuilder.Append(
                                    $"<{i}><Type>Vehicle</Type><Name>{building}</Name><Date>{now}</Date></{i}>");

                                i++;
                            }

                            holdbacksBuilder.Append("</Response>");

                            return holdbacksBuilder.ToString();
                        case "QueryRewards":
                            string serverRewardsFilePath = ServerFilesPath + $"/Server_Rewards.xml";

                            if (File.Exists(serverRewardsFilePath))
                                return $"<Response>{File.ReadAllText(serverRewardsFilePath)}</Response>";
                            else
                                return @"<Response>
                                <Reward>
                                    <Name>HT_T_Shirt</Name>
                                    <SCEA type=""table"">
                                       <UUID>3EC5EAB2-5C5D4379-AE88BD53-A2B26938</UUID>
                                       <UUID>ADED875A-724E40C7-BF874795-6ACEF9E3</UUID>
                                    </SCEA>
                                    <SCEE type=""table"">
                                       <UUID>3EC5EAB2-5C5D4379-AE88BD53-A2B26938</UUID>
                                       <UUID>ADED875A-724E40C7-BF874795-6ACEF9E3</UUID>
                                    </SCEE>
                                    <SCEJ type=""table"">
                                       <UUID>3EC5EAB2-5C5D4379-AE88BD53-A2B26938</UUID>
                                       <UUID>ADED875A-724E40C7-BF874795-6ACEF9E3</UUID>
                                    </SCEJ>
                                </Reward>

                                <Reward>
                                    <Name>Construction_Hat</Name>
                                    <SCEA type=""table"">
                                       <UUID>E13282E0-662243D5-9FFE2923-DE784629</UUID>
                                       <UUID>B5BBC99A-C47C45AF-8E9E394C-571098A5</UUID>
                                    </SCEA>
                                    <SCEE type=""table"">
                                       <UUID>E13282E0-662243D5-9FFE2923-DE784629</UUID>
                                       <UUID>B5BBC99A-C47C45AF-8E9E394C-571098A5</UUID>
                                    </SCEE>
                                    <SCEJ type=""table"">
                                       <UUID>E13282E0-662243D5-9FFE2923-DE784629</UUID>
                                       <UUID>B5BBC99A-C47C45AF-8E9E394C-571098A5</UUID>
                                    </SCEJ>
                                </Reward>

                                <Reward>
                                    <Name>TU_Shirt</Name>
                                    <SCEA type=""table"">
                                       <UUID>5A8D5748-86974730-AFAEDD07-1762E91D</UUID>
                                    </SCEA>
                                    <SCEE type=""table"">
                                       <UUID>5A8D5748-86974730-AFAEDD07-1762E91D</UUID>
                                    </SCEE>
                                    <SCEJ type=""table"">
                                       <UUID>5A8D5748-86974730-AFAEDD07-1762E91D</UUID>
                                    </SCEJ>
                                </Reward>

                                <Reward>
                                    <Name>GS_Shirt</Name>
                                    <SCEA type=""table"">
                                       <UUID>5A8D5748-86974730-AFAEDD07-1762E91D</UUID>
                                       <UUID>4E24376E-2C82472E-94762AFE-92684A8C</UUID>
                                    </SCEA>
                                    <SCEE type=""table"">
                                       <UUID>5A8D5748-86974730-AFAEDD07-1762E91D</UUID>
                                       <UUID>4E24376E-2C82472E-94762AFE-92684A8C</UUID>
                                    </SCEE>
                                    <SCEJ type=""table"">
                                       <UUID>5A8D5748-86974730-AFAEDD07-1762E91D</UUID>
                                       <UUID>4E24376E-2C82472E-94762AFE-92684A8C</UUID>
                                    </SCEJ>
                                </Reward>

                                <Reward>
                                    <Name>Fire_Helmet</Name>
                                    <SCEA type=""table"">
                                       <UUID>2F0AAC71-78844481-81D0CA67-D45825BF</UUID>
                                       <UUID>160F4886-9A1243E8-855E6DBA-19F47246</UUID>
                                    </SCEA>
                                    <SCEE type=""table"">
                                       <UUID>2F0AAC71-78844481-81D0CA67-D45825BF</UUID>
                                       <UUID>160F4886-9A1243E8-855E6DBA-19F47246</UUID>
                                    </SCEE>
                                    <SCEJ type=""table"">
                                       <UUID>2F0AAC71-78844481-81D0CA67-D45825BF</UUID>
                                       <UUID>160F4886-9A1243E8-855E6DBA-19F47246</UUID>
                                    </SCEJ>
                                </Reward>

                                <Reward>
                                    <Name>Stop_Sign</Name>
                                    <SCEA type=""table"">
                                        <UUID>00455F91-55174086-B2B98B2A-B1F3644D</UUID>
                                    </SCEA>
                                    <SCEE type=""table"">
                                        <UUID>00455F91-55174086-B2B98B2A-B1F3644D</UUID>
                                    </SCEE>
                                    <SCEJ type=""table"">
                                        <UUID>00455F91-55174086-B2B98B2A-B1F3644D</UUID>
                                    </SCEJ>
                                </Reward>

                                <Reward>
                                    <Name>Police_Hat</Name>
                                    <SCEA type=""table"">
                                        <UUID>A553749A-297849B0-9FB73F2D-65A8372C</UUID>
                                        <UUID>1731F810-F13948C5-B4BD0B99-4776B29C</UUID>
                                    </SCEA>
                                    <SCEE type=""table"">
                                        <UUID>A553749A-297849B0-9FB73F2D-65A8372C</UUID>
                                        <UUID>1731F810-F13948C5-B4BD0B99-4776B29C</UUID>
                                    </SCEE>
                                    <SCEJ type=""table"">
                                        <UUID>A553749A-297849B0-9FB73F2D-65A8372C</UUID>
                                        <UUID>1731F810-F13948C5-B4BD0B99-4776B29C</UUID>
                                    </SCEJ>
                                </Reward>

                                <Reward>
                                    <Name>Police_Shirt</Name>
                                    <SCEA type=""table"">
                                        <UUID>6F73F579-68914063-8CEC298A-6229F961</UUID>
                                        <UUID>C2864B65-7ED74C96-950ACFA6-956FBAC8</UUID>
                                    </SCEA>
                                    <SCEE type=""table"">
                                        <UUID>6F73F579-68914063-8CEC298A-6229F961</UUID>
                                        <UUID>C2864B65-7ED74C96-950ACFA6-956FBAC8</UUID>
                                    </SCEE>
                                    <SCEJ type=""table"">
                                        <UUID>6F73F579-68914063-8CEC298A-6229F961</UUID>
                                        <UUID>C2864B65-7ED74C96-950ACFA6-956FBAC8</UUID>
                                    </SCEJ>
                                </Reward>

                                <Reward>
                                    <Name>Head_Mirror</Name>
                                    <SCEA type=""table"">
                                        <UUID>87D4951A-2A8C4139-B310F037-FA75D803</UUID>
                                        <UUID>0A25BFC8-872C44F9-A9EB507D-6CCED420</UUID>
                                    </SCEA>
                                    <SCEE type=""table"">
                                        <UUID>87D4951A-2A8C4139-B310F037-FA75D803</UUID>
                                        <UUID>0A25BFC8-872C44F9-A9EB507D-6CCED420</UUID>
                                    </SCEE>
                                    <SCEJ type=""table"">
                                        <UUID>87D4951A-2A8C4139-B310F037-FA75D803</UUID>
                                        <UUID>0A25BFC8-872C44F9-A9EB507D-6CCED420</UUID>
                                    </SCEJ>
                                </Reward>

                                <Reward>
                                    <Name>Zombie_Briefcase</Name>
                                    <SCEA type=""table"">
                                        <UUID>51630718-06794A57-B1C07AAB-351A9450</UUID>
                                        <UUID>B1377361-FD3B458F-82137B59-4668A41D</UUID>
                                    </SCEA>
                                    <SCEE type=""table"">
                                        <UUID>51630718-06794A57-B1C07AAB-351A9450</UUID>
                                        <UUID>B1377361-FD3B458F-82137B59-4668A41D</UUID>
                                    </SCEE>
                                    <SCEJ type=""table"">
                                        <UUID>51630718-06794A57-B1C07AAB-351A9450</UUID>
                                        <UUID>B1377361-FD3B458F-82137B59-4668A41D</UUID>
                                    </SCEJ>
                                </Reward>

                                <Reward>
                                    <Name>Money_Bag</Name>
                                    <SCEA type=""table"">
                                        <UUID>3752C3E6-8C0B49BB-B56C9CEA-8EF72704</UUID>
                                        <UUID>665048D1-B4234C01-A55EC3B0-9A6A24FC</UUID>
                                    </SCEA>
                                    <SCEE type=""table"">
                                        <UUID>3752C3E6-8C0B49BB-B56C9CEA-8EF72704</UUID>
                                        <UUID>665048D1-B4234C01-A55EC3B0-9A6A24FC</UUID>
                                    </SCEE>
                                    <SCEJ type=""table"">
                                        <UUID>3752C3E6-8C0B49BB-B56C9CEA-8EF72704</UUID>
                                        <UUID>665048D1-B4234C01-A55EC3B0-9A6A24FC</UUID>
                                    </SCEJ>
                                </Reward>

                                <Reward>
                                    <Name>Classy_Cane</Name>
                                    <SCEA type=""table"">
                                        <UUID>07BF333A-5B5A4C94-970F8AA6-9AA52BB3</UUID>
                                        <UUID>6DF457F3-28824F1D-95711D87-2DBE5953</UUID>
                                    </SCEA>
                                    <SCEE type=""table"">
                                        <UUID>07BF333A-5B5A4C94-970F8AA6-9AA52BB3</UUID>
                                        <UUID>6DF457F3-28824F1D-95711D87-2DBE5953</UUID>
                                    </SCEE>
                                    <SCEJ type=""table"">
                                        <UUID>07BF333A-5B5A4C94-970F8AA6-9AA52BB3</UUID>
                                        <UUID>6DF457F3-28824F1D-95711D87-2DBE5953</UUID>
                                    </SCEJ>
                                </Reward>

                                <Reward>
                                    <Name>Pith_Helmet</Name>
                                    <SCEA type=""table"">
                                        <UUID>C7FAD181-CFE14883-B558AC79-529DF0C1</UUID>
                                        <UUID>1CBC11C1-2AC74BC7-ADA4ABFB-5BF1BF4E</UUID>
                                    </SCEA>
                                    <SCEE type=""table"">
                                        <UUID>C7FAD181-CFE14883-B558AC79-529DF0C1</UUID>
                                        <UUID>1CBC11C1-2AC74BC7-ADA4ABFB-5BF1BF4E</UUID>
                                    </SCEE>
                                    <SCEJ type=""table"">
                                        <UUID>C7FAD181-CFE14883-B558AC79-529DF0C1</UUID>
                                        <UUID>1CBC11C1-2AC74BC7-ADA4ABFB-5BF1BF4E</UUID>
                                    </SCEJ>
                                </Reward>

                                <Reward>
                                    <Name>Bear_Companion</Name>
                                    <SCEA type=""table"">
                                        <UUID>0F251E61-13774374-8B028F67-73FF8D4D</UUID>
                                    </SCEA>
                                    <SCEE type=""table"">
                                        <UUID>0F251E61-13774374-8B028F67-73FF8D4D</UUID>
                                    </SCEE>
                                    <SCEJ type=""table"">
                                        <UUID>0F251E61-13774374-8B028F67-73FF8D4D</UUID>
                                    </SCEJ>
                                </Reward>

                                <Reward>
                                    <Name>Science_Gloves</Name>
                                    <SCEA type=""table"">
                                        <UUID>A99BAA9D-1E734B87-947DBE36-87CA6471</UUID>
                                        <UUID>9AE73E6B-962A4BF0-8EC5C0C6-0FFE28DC</UUID>
                                    </SCEA>
                                    <SCEE type=""table"">
                                        <UUID>A99BAA9D-1E734B87-947DBE36-87CA6471</UUID>
                                        <UUID>9AE73E6B-962A4BF0-8EC5C0C6-0FFE28DC</UUID>
                                    </SCEE>
                                    <SCEJ type=""table"">
                                        <UUID>A99BAA9D-1E734B87-947DBE36-87CA6471</UUID>
                                        <UUID>9AE73E6B-962A4BF0-8EC5C0C6-0FFE28DC</UUID>
                                    </SCEJ>
                                </Reward>


                                <Reward>
                                    <Name>Sonja_Portrait</Name>
                                    <SCEA type=""table"">
                                        <UUID>8866F55D-77E94B79-931173AF-C859F433</UUID>
                                    </SCEA>
                                    <SCEE type=""table"">
                                        <UUID>8866F55D-77E94B79-931173AF-C859F433</UUID>
                                    </SCEE>
                                    <SCEJ type=""table"">
                                        <UUID>8866F55D-77E94B79-931173AF-C859F433</UUID>
                                    </SCEJ>
                                </Reward>

                                <Reward>
                                    <Name>Science_Goggles</Name>
                                    <SCEA type=""table"">
                                        <UUID>23022989-4CFE4808-BCFCABEE-44F6239E</UUID>
                                        <UUID>87DC7757-835B46CB-B552E3E8-E9ED1D70</UUID>
                                    </SCEA>
                                    <SCEE type=""table"">
                                        <UUID>23022989-4CFE4808-BCFCABEE-44F6239E</UUID>
                                        <UUID>87DC7757-835B46CB-B552E3E8-E9ED1D70</UUID>
                                    </SCEE>
                                    <SCEJ type=""table"">
                                        <UUID>23022989-4CFE4808-BCFCABEE-44F6239E</UUID>
                                        <UUID>87DC7757-835B46CB-B552E3E8-E9ED1D70</UUID>
                                    </SCEJ>
                                </Reward>

                                <Reward>
                                    <Name>Geiger_Counter</Name>
                                    <SCEA type=""table"">
                                        <UUID>FBAA938E-2ECF4B0A-B489E637-4D35E100</UUID>
                                        <UUID>E6D20C76-FC87402E-9FA05F49-C64183AE</UUID>
                                    </SCEA>
                                    <SCEE type=""table"">
                                        <UUID>FBAA938E-2ECF4B0A-B489E637-4D35E100</UUID>
                                        <UUID>E6D20C76-FC87402E-9FA05F49-C64183AE</UUID>
                                    </SCEE>
                                    <SCEJ type=""table"">
                                        <UUID>FBAA938E-2ECF4B0A-B489E637-4D35E100</UUID>
                                        <UUID>E6D20C76-FC87402E-9FA05F49-C64183AE</UUID>
                                    </SCEJ>
                                </Reward>

                                <Reward>
                                    <Name>Hazmat_Suit</Name>
                                    <SCEA type=""table"">
                                        <UUID>F79F3FE5-CB6048C4-AA60F7AC-2DCB2284</UUID>
                                        <UUID>CF8233A4-9D0A42C8-A31C7E91-8283E369</UUID>
                                    </SCEA>
                                    <SCEE type=""table"">
                                        <UUID>F79F3FE5-CB6048C4-AA60F7AC-2DCB2284</UUID>
                                        <UUID>CF8233A4-9D0A42C8-A31C7E91-8283E369</UUID>
                                    </SCEE>
                                    <SCEJ type=""table"">
                                        <UUID>F79F3FE5-CB6048C4-AA60F7AC-2DCB2284</UUID>
                                        <UUID>CF8233A4-9D0A42C8-A31C7E91-8283E369</UUID>
                                    </SCEJ>
                                </Reward>

                                <Reward>
                                    <Name>Briefcase</Name>
                                    <SCEA type=""table"">
                                        <UUID>AB63CC1E-27994EA0-AABD000B-E913153C</UUID>
                                    </SCEA>
                                    <SCEE type=""table"">
                                        <UUID>AB63CC1E-27994EA0-AABD000B-E913153C</UUID>
                                    </SCEE>
                                    <SCEJ type=""table"">
                                        <UUID>AB63CC1E-27994EA0-AABD000B-E913153C</UUID>
                                    </SCEJ>
                                </Reward>

                                <Reward>
                                    <Name>Magnus_Portrait</Name>
                                    <SCEA type=""table"">
                                        <UUID>2D79FF0B-E8984797-AD501B97-60FABECB</UUID>
                                    </SCEA>
                                    <SCEE type=""table"">
                                        <UUID>2D79FF0B-E8984797-AD501B97-60FABECB</UUID>
                                    </SCEE>
                                    <SCEJ type=""table"">
                                        <UUID>2D79FF0B-E8984797-AD501B97-60FABECB</UUID>
                                    </SCEJ>
                                </Reward>

                                <Reward>
                                    <Name>Mayor_Pants</Name>
                                    <SCEA type=""table"">
                                        <UUID>D0A3B7EB-6E4441B1-AC500603-549B08D7</UUID>
                                    </SCEA>
                                    <SCEE type=""table"">
                                        <UUID>D0A3B7EB-6E4441B1-AC500603-549B08D7</UUID>
                                    </SCEE>
                                    <SCEJ type=""table"">
                                        <UUID>D0A3B7EB-6E4441B1-AC500603-549B08D7</UUID>
                                    </SCEJ>
                                </Reward>

                                <Reward>
                                    <Name>Medic_Helmet</Name>
                                    <SCEA type=""table"">
                                        <UUID>83B0054B-94854708-93F79807-B2C6E822</UUID>
                                        <UUID>D3D58063-C1E94046-BA95179D-664FB50A</UUID>
                                    </SCEA>
                                    <SCEE type=""table"">
                                        <UUID>83B0054B-94854708-93F79807-B2C6E822</UUID>
                                        <UUID>D3D58063-C1E94046-BA95179D-664FB50A</UUID>
                                    </SCEE>
                                    <SCEJ type=""table"">
                                        <UUID>83B0054B-94854708-93F79807-B2C6E822</UUID>
                                        <UUID>D3D58063-C1E94046-BA95179D-664FB50A</UUID>
                                    </SCEJ>
                                </Reward>

                                <Reward>
                                    <Name>Police_Helmet</Name>
                                    <SCEA type=""table"">
                                        <UUID>FBD8A0B0-C66F451C-8B292AF3-786E2138</UUID>
                                        <UUID>00DC914D-3F054B62-A9ED25DE-0B81E05D</UUID>
                                    </SCEA>
                                    <SCEE type=""table"">
                                        <UUID>FBD8A0B0-C66F451C-8B292AF3-786E2138</UUID>
                                        <UUID>00DC914D-3F054B62-A9ED25DE-0B81E05D</UUID>
                                    </SCEE>
                                    <SCEJ type=""table"">
                                        <UUID>FBD8A0B0-C66F451C-8B292AF3-786E2138</UUID>
                                        <UUID>00DC914D-3F054B62-A9ED25DE-0B81E05D</UUID>
                                    </SCEJ>
                                </Reward>

                                <Reward>
                                    <Name>Mayor_Vest</Name>
                                    <SCEA type=""table"">
                                        <UUID>FBD8A0B0-C66F451C-8B292AF3-786E2138</UUID>
                                        <UUID>70A97492-2B0A420F-A8838A9E-877F8D9D</UUID>
                                    </SCEA>
                                    <SCEE type=""table"">
                                        <UUID>FBD8A0B0-C66F451C-8B292AF3-786E2138</UUID>
                                        <UUID>70A97492-2B0A420F-A8838A9E-877F8D9D</UUID>
                                    </SCEE>
                                    <SCEJ type=""table"">
                                        <UUID>FBD8A0B0-C66F451C-8B292AF3-786E2138</UUID>
                                        <UUID>70A97492-2B0A420F-A8838A9E-877F8D9D</UUID>
                                    </SCEJ>
                                </Reward>

                                <Reward>
                                    <Name>Balloon_Cart</Name>
                                    <SCEA type=""table"">
                                        <UUID>A39BF49F-5C944139-8D2C36A9-5A9B86CD</UUID>
                                    </SCEA>
                                    <SCEE type=""table"">
                                        <UUID>A39BF49F-5C944139-8D2C36A9-5A9B86CD</UUID>
                                    </SCEE>
                                    <SCEJ type=""table"">
                                        <UUID>A39BF49F-5C944139-8D2C36A9-5A9B86CD</UUID>
                                    </SCEJ>
                                </Reward>

                                <Reward>
                                    <Name>News_Helmet</Name>
                                    <SCEA type=""table"">
                                        <UUID>725040AC-812C4C79-86CBB69C-1CC0C512</UUID>
                                        <UUID>C06A2E46-A34D4467-9CB460F4-732E394A</UUID>
                                    </SCEA>
                                    <SCEE type=""table"">
                                        <UUID>725040AC-812C4C79-86CBB69C-1CC0C512</UUID>
                                        <UUID>C06A2E46-A34D4467-9CB460F4-732E394A</UUID>
                                    </SCEE>
                                    <SCEJ type=""table"">
                                        <UUID>725040AC-812C4C79-86CBB69C-1CC0C512</UUID>
                                        <UUID>C06A2E46-A34D4467-9CB460F4-732E394A</UUID>
                                    </SCEJ>
                                </Reward>

                                <Reward>
                                    <Name>Carnival_Tent</Name>
                                    <SCEA type=""table"">
                                        <UUID>2B17E6D9-BF104CF5-94DDF655-C28209FE</UUID>
                                    </SCEA>
                                    <SCEE type=""table"">
                                        <UUID>2B17E6D9-BF104CF5-94DDF655-C28209FE</UUID>
                                    </SCEE>
                                    <SCEJ type=""table"">
                                        <UUID>2B17E6D9-BF104CF5-94DDF655-C28209FE</UUID>
                                    </SCEJ>
                                </Reward>

                                <Reward>
                                    <Name>Popcorn_Cart</Name>
                                    <SCEA type=""table"">
                                        <UUID>80D47F2A-CFC44160-886AF5B7-0515292A</UUID>
                                    </SCEA>
                                    <SCEE type=""table"">
                                        <UUID>80D47F2A-CFC44160-886AF5B7-0515292A</UUID>
                                    </SCEE>
                                    <SCEJ type=""table"">
                                        <UUID>80D47F2A-CFC44160-886AF5B7-0515292A</UUID>
                                    </SCEJ>
                                </Reward>

                                <Reward>
                                    <Name>Fortune_Teller</Name>
                                    <SCEA type=""table"">
                                        <UUID>D1C6E9B7-A8824EEF-85A63FA8-6B06A318</UUID>
                                    </SCEA>
                                    <SCEE type=""table"">
                                        <UUID>D1C6E9B7-A8824EEF-85A63FA8-6B06A318</UUID>
                                    </SCEE>
                                    <SCEJ type=""table"">
                                        <UUID>D1C6E9B7-A8824EEF-85A63FA8-6B06A318</UUID>
                                    </SCEJ>
                                </Reward>

                                <Reward>
                                    <Name>Floating_Balloons</Name>
                                    <SCEA type=""table"">
                                        <UUID>F7C99A22-80904B69-95754823-CC2051F8</UUID>
                                    </SCEA>
                                    <SCEE type=""table"">
                                        <UUID>F7C99A22-80904B69-95754823-CC2051F8</UUID>
                                    </SCEE>
                                    <SCEJ type=""table"">
                                        <UUID>F7C99A22-80904B69-95754823-CC2051F8</UUID>
                                    </SCEJ>
                                </Reward>
								
								<Reward>
                                    <Name>Traffic_Cone_Hat</Name>
                                    <SCEA type=""table"">
                                        <UUID>763AE6B4-9F954624-B9A3ED23-9E008FBA</UUID>
										<UUID>6D256C0C-6C9D445C-A2BA531E-C6602A11</UUID>
                                    </SCEA>
                                    <SCEE type=""table"">
                                        <UUID>763AE6B4-9F954624-B9A3ED23-9E008FBA</UUID>
										<UUID>6D256C0C-6C9D445C-A2BA531E-C6602A11</UUID>
                                    </SCEE>
                                    <SCEJ type=""table"">
                                        <UUID>763AE6B4-9F954624-B9A3ED23-9E008FBA</UUID>
										<UUID>6D256C0C-6C9D445C-A2BA531E-C6602A11</UUID>
                                    </SCEJ>
                                </Reward>

                                </Response>";
                        case "QueryGifts":
                            string giftOverrideFile = userDataPath + $"/Gifts.xml";

                            if (File.Exists(giftOverrideFile))
                                return $"<Response>{File.ReadAllText(giftOverrideFile)}</Response>";
                            else
                                return $"<Response><Gift>{DefaultGiftNumber}</Gift></Response>";
                        case "UpdateTownTime":
                            return TownProcessor.UpdateTownTime(UserID, TownID, WorkPath);
                        case "UpdateTownPlayers":
                            return TownProcessor.UpdateTownPlayers(UserID, TownID, NumPlayers, WorkPath);
                        case "UpdateInstance":
                            // Seems to do nothing, no idea what this is for, only send parameter: InstanceID
                            return "<Response></Response>";
                        case "AddVisitor":
                            return TownProcessor.HandleVisitors(PostData, boundary, UserID, WorkPath, Command);
                        case "GetVisitors":
                            return TownProcessor.HandleVisitors(PostData, boundary, UserID, WorkPath, Command);
                        case "ClearVisitors":
                            return TownProcessor.HandleVisitors(PostData, boundary, UserID, WorkPath, Command);
                        case "CreateSuburb":
                            return TownInstance.CreateSuburp(UserID, WorkPath);
                        case "SavePostcard":
                            return PostCards.HandleUpload(PostData, boundary, UserID, WorkPath);
                        case "SpendCoins":
                        case "UpdateUser":
                        case "SetPrivacy":
                        case "AddActivity":
                        case "RemoveActivity":
                        case "AddUnlocked":
                        case "RemoveUnlocked":
                        case "AddVehicle":
                        case "RemoveVehicle":
                        case "AddInventory":
                        case "AddFlag":
                        case "AddMission":
                        case "AddExpansion":
                        case "CompleteMission":
                        case "AddMissionToJournal":
                        case "RemoveMissionFromJournal":
                        case "AddDialog":
                        case "CompleteDialog":
                            return User.UpdateUserHomeTycoon(PostData, boundary, UserID, WorkPath, Command);
                        case "UnlockDefault":
                            return $"<Response>{DefaultBuildings}</Response>";
                        case "CreateBuilding":
                            return TownProcessor.CreateBuilding(PostData, boundary, UserID, WorkPath);
                        case "UpdateBuildings":
                            return TownProcessor.UpdateBuildings(PostData, boundary, UserID, WorkPath);
                        case "RemoveBuilding":
                            return TownProcessor.RemoveBuilding(PostData, boundary, UserID, WorkPath);
                        case "GlobalPopulationLeaderboard":
                            return Leaderboards.GetGlobalPopulationLeaderboard(PostData, boundary, UserID, WorkPath);
                        case "GlobalRevenueCollectedLeaderboard":
                            return Leaderboards.GetGlobalRevenueCollectedLeaderboard(PostData, boundary, UserID, WorkPath);
                        //Debug functions in lua commented out
                        case "DeleteCity":
                            return "<Response></Response>";
                        case "DeleteUser":
                            return "<Response></Response>";
                        case "ClearMissionRevenueCollected":
                            return "<Response></Response>";
                        default:
                            LoggerAccessor.LogWarn($"[TycoonRequestProcessor] - Client Requested an unknown Home Tycoon Command, please report as issue on GITHUB : {Command}");
                            return "<Response></Response>";
                    }
                }
            }

            return null;
        }
    }
}
