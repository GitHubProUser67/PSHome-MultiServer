using Alcatraz.Context;
using Alcatraz.Context.Entities;
using Alcatraz.DTO.Helpers;
using AlcatrazService.DTO;
using CustomLogger;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using QuazalServer.QNetZ.DDL;
using QuazalServer.RDVServices.DDL.Models;

namespace RDVServices
{
	public static class DBHelper
	{
		public static MainDbContext? GetDbContext(string serviceClass)
		{
            bool initiateSharedUbiPlayers = false;
            string connectionString;
            MainDbContext? retCtx = null;

            switch (serviceClass)
			{
                case "PCGFRSServices":
                case "PCDriverServices":
                case "PCUbisoftServices":
					connectionString = $"{Program.configDir}/Quazal/Database/Uplay.sqlite";

					Directory.CreateDirectory(Path.GetDirectoryName(connectionString)!);

                    retCtx = new MainDbContext(MainDbContext.OnContextBuilding(new DbContextOptionsBuilder<MainDbContext>(), 0, $"Data Source={connectionString}").Options);

                    retCtx.Database.Migrate();

                    initiateSharedUbiPlayers = true;
                    break;
                case "PS3RaymanLegendsServices":
                case "PS3GFRSServices":
                case "PS3DriverServices":
                case "PS3UbisoftServices":
                    connectionString = $"{Program.configDir}/Quazal/Database/Uplay_PS3.sqlite";

                    Directory.CreateDirectory(Path.GetDirectoryName(connectionString)!);

                    retCtx = new MainDbContext(MainDbContext.OnContextBuilding(new DbContextOptionsBuilder<MainDbContext>(), 0, $"Data Source={connectionString}").Options);

                    retCtx.Database.Migrate();

                    initiateSharedUbiPlayers = true;
                    break;
                case "v2Services":
                    connectionString = $"{Program.configDir}/Quazal/Database/RendezVous_v2.sqlite";

                    Directory.CreateDirectory(Path.GetDirectoryName(connectionString)!);

                    retCtx = new MainDbContext(MainDbContext.OnContextBuilding(new DbContextOptionsBuilder<MainDbContext>(), 0, $"Data Source={connectionString}").Options);

                    retCtx.Database.Migrate();

                    break;
                default:
					LoggerAccessor.LogWarn($"[DbHelper] - Unknwon: {serviceClass} Class passed to the database!");
					break;
			}

            if (retCtx != null)
                InitiateDefaultPlayers(retCtx, initiateSharedUbiPlayers);

            return retCtx;
		}

        private static void InitiateDefaultPlayers(MainDbContext context, bool initiateSharedUbiPlayers)
        {
            if (!context.Users.Any())
            {
                // Add dummy user with starting ID == 1000 and two guest accounts.
                context.Users.Add(new User()
                {
                    Id = 1000,
                    Username = "dummy",
                    PlayerNickName = "dummy",
                    Password = "dummy",
                    RewardFlags = 0,
                });
                context.SaveChanges();

                if (initiateSharedUbiPlayers)
                {
                    User newUser = new User()
                    {
                        Username = "AAAABBBB",
                        PlayerNickName = "AAAABBBB",
                        Password = "tmp",
                    };

                    try
                    {
                        context.Users.Add(newUser);
                        context.SaveChanges();
                    }
                    catch
                    {
                        LoggerAccessor.LogError($"[DBHelper] - Unable to add default ubi {newUser.Username} user (internal error)");
                        return;
                    }

                    // update password as user Id is acquired
                    newUser.Password = SecurePasswordHasher.Hash($"{newUser.Id}-CCCCDDDD");
                    context.SaveChanges();

                    newUser = new User()
                    {
                        Username = "sam_the_fisher",
                        PlayerNickName = "sam_the_fisher",
                        Password = "tmp",
                    };

                    try
                    {
                        context.Users.Add(newUser);
                        context.SaveChanges();
                    }
                    catch
                    {
                        LoggerAccessor.LogError($"[DBHelper] - Unable to add default ubi {newUser.Username} user (internal error)");
                        return;
                    }

                    // update password as user Id is acquired
                    newUser.Password = SecurePasswordHasher.Hash($"{newUser.Id}-password1234");
                    context.SaveChanges();
                }
            }
        }

        public static bool RegisterUplayUser(string serviceClass, UserRegisterModel model)
        {
            using (MainDbContext? context = GetDbContext(serviceClass))
            {
                if (context == null)
                    return false;

                if (string.IsNullOrWhiteSpace(model.Username))
                    return false;

                if (string.IsNullOrWhiteSpace(model.PlayerNickName))
                    return false;

                if (string.IsNullOrWhiteSpace(model.Password))
                    return false;

                var newUser = new User()
                {
                    Username = model.Username,
                    PlayerNickName = model.PlayerNickName,
                    Password = "tmp",
                };

                if (context.Users.Any(x => x.Username == model.Username || x.PlayerNickName == model.PlayerNickName))
                    return false;

                try
                {
                    context.Users.Add(newUser);
                    context.SaveChanges();
                }
                catch
                {
                    return false;
                }

                // update password as user Id is acquired
                {
                    newUser.Password = SecurePasswordHasher.Hash($"{newUser.Id}-{model.Password}");
                    context.SaveChanges();
                }

                return true;
            }
        }

        public static bool RegisterUser(string serviceClass, string userName, string password, uint PID, string? NickName = null)
        {
            using (MainDbContext? context = GetDbContext(serviceClass))
            {
                if (context != null)
                {
                    bool HasNickName = !string.IsNullOrEmpty(NickName);

                    if (!context.Users.Any(x => x.Username == userName || x.Id == PID || (HasNickName && x.PlayerNickName == NickName)))
                    {
                        User dbUser = new User() { Id = PID, Username = userName };

                        if (HasNickName)
                            dbUser.PlayerNickName = NickName;

                        dbUser.Password = password;

                        try
                        {
                            context.Users.Add(dbUser);
                            context.SaveChanges();

                            return true;
                        }
                        catch (Exception ex)
                        {
                            LoggerAccessor.LogError($"[DBHelper] - An assertion was thrown while adding User:{dbUser} to the database. (Exception: {ex})");
                        }
                    }
                }
            }

            return false;
        }

        public static bool RegisterUserWithExtraData(string serviceClass, string userName, string password, uint PID, AnyData<PlayerData> oPublicData, AnyData<AccountInfoPrivateData> oPrivateData, string? NickName = null)
        {
            using (MainDbContext? context = GetDbContext(serviceClass))
            {
                if (context != null)
                {
                    bool HasNickName = !string.IsNullOrEmpty(NickName);

                    if (!context.Users.Any(x => x.Username == userName || x.Id == PID || (HasNickName && x.PlayerNickName == NickName)))
                    {
                        User dbUser = new User() { Id = PID, Username = userName };

                        if (HasNickName)
                            dbUser.PlayerNickName = NickName;

                        dbUser.Password = password;

                        using (MemoryStream ms = new())
                        using (MemoryStream ms1 = new())
                        {
                            oPublicData.Write(ms);
                            oPrivateData.Write(ms1);

                            dbUser.PublicData = ms.ToArray();
                            dbUser.PrivateData = ms1.ToArray();
                        }

                        try
                        {
                            context.Users.Add(dbUser);
                            context.SaveChanges();

                            return true;
                        }
                        catch (Exception ex)
                        {
                            LoggerAccessor.LogError($"[DBHelper] - An assertion was thrown while adding User:{dbUser} to the database. (Exception: {ex})");
                        }
                    }
                }
            }

            return false;
        }

        public static bool UpdateUbiTokensDataByUserName(string serviceClass, string userName, int numOfTokens)
        {
            using (MainDbContext? context = GetDbContext(serviceClass))
            {
                User? user = context?.Users
                    .SingleOrDefault(x => x.Username == userName);

                if (user != null)
                {
                    user.UbiTokens = numOfTokens;
                    context?.SaveChanges();

                    return true;
                }
            }

            return false;
        }

        public static bool UpdateUbiAccountDataByUserName(string serviceClass, string userName, UbiAccount account)
        {
            using (MainDbContext? context = GetDbContext(serviceClass))
            {
                User? user = context?.Users
                    .SingleOrDefault(x => x.Username == userName);

                if (user != null)
                {
                    user.UbiData = JsonConvert.SerializeObject(account);
                    context?.SaveChanges();

                    return true;
                }
            }

            return false;
        }

        public static int GetUbiTokensDataByUserName(string serviceClass, string userName)
        {
            using (MainDbContext? context = GetDbContext(serviceClass))
            {
                return context?.Users
                    .AsNoTracking()
                    .SingleOrDefault(x => x.Username == userName)?.UbiTokens ?? 0;
            }
        }

        public static UbiAccount? GetUbiAccountDataByUserName(string serviceClass, string userName)
        {
            using (MainDbContext? context = GetDbContext(serviceClass))
            {
                string? ubiData = context?.Users
                    .AsNoTracking()
                    .SingleOrDefault(x => x.Username == userName)?.UbiData;

                if (!string.IsNullOrEmpty(ubiData))
                    return JsonConvert.DeserializeObject<UbiAccount>(ubiData);
            }

            return null;
        }

        public static User? GetUserByUserName(string serviceClass, string userName)
        {
            using (MainDbContext? context = GetDbContext(serviceClass))
            {
                return context?.Users
                    .AsNoTracking()
                    .SingleOrDefault(x => x.Username == userName);
            }
        }

        public static User? GetUserByNickName(string serviceClass, string name)
		{
			using (MainDbContext? context = GetDbContext(serviceClass))
			{
				return context?.Users
					.AsNoTracking()
					.SingleOrDefault(x => x.PlayerNickName == name);
			}
		}

		public static User? GetUserByPID(string serviceClass, uint PID)
		{
			using (MainDbContext? context = GetDbContext(serviceClass))
			{
				return context?.Users
					.AsNoTracking()
					.SingleOrDefault(x => x.Id == PID);
			}
		}
	}
}
