using CustomLogger;
using System.IO;
using System.Net;
using WatsonWebserver.Core;
using WebAPIService.GameServices.OHS;

namespace ApacheNet.BuildIn.RouteHandlers.GameRoutes.Lockwood
{
    internal static class SodiumObjectives
    {
        public static void BuildSodiumObjectivesPlugin(WebserverBase server)
        {
            server.Routes.PostAuthentication.Parameter.Add(HttpMethod.GET, "/webassets/Sodium/objectives/data/{version}/{project}/{xmldef}", async (ctx) =>
            {
                string? project = ctx.Request.Url.Parameters["project"];
                if (string.IsNullOrEmpty(project))
                {
                    ctx.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    ctx.Response.ContentType = "text/plain";
                    await ctx.Response.Send();
                    return;
                }
                string version = ctx.Request.Url.Parameters["version"] ?? "7";
                string xmldef = ctx.Request.Url.Parameters["xmldef"] ?? "SCEA";

                string xmlPath = $"/webassets/Sodium/objectives/data/{version}/{project}/{ctx.Request.Url.Parameters["xmldef"]}";
                string filePath = ApacheNetServerConfiguration.HTTPStaticFolder + xmlPath;

                if (File.Exists(filePath))
                {
                    ctx.Response.StatusCode = (int)HttpStatusCode.OK;
                    ctx.Response.ContentType = "text/xml";
                    await ctx.Response.Send(File.ReadAllText(filePath));
                    return;
                }

                LoggerAccessor.LogDebug($"[PostAuthParameters] - Sodium objectives regional {project} definition data was not found, falling back to static file.");

                switch (project)
                {
                    case "region":
                        ctx.Response.StatusCode = (int)HttpStatusCode.OK;
                        ctx.Response.ContentType = "text/xml";
#if DEBUG
                        bool debug = true;
#else
                        bool debug = false;
#endif
                        await ctx.Response.Send(LUA2XmlProcessor.TransformLuaTableToXml($@"
                            local TableFromInput = {{
                                REGION = {{
                                    sodium_debug = {{
                                        {xmldef.Replace(".xml", string.Empty)} = {debug}
                                    }}
                                }}
                            }}

                            return Encode(TableFromInput, 4, 4)
                            "));
                        return;
                    case "lang":
                        ctx.Response.StatusCode = (int)HttpStatusCode.OK;
                        ctx.Response.ContentType = "text/xml";
                        await ctx.Response.Send(LUA2XmlProcessor.TransformLuaTableToXml($@"
                            local TableFromInput = {{
                                LOCALIZATION = {{
                                    sodium_1 = {{
                                        title = ""What Did It Ever Do To You?"",
                                        hints = {{
                                            ""-""
                                        }},
                                        description = ""Stomp your first robotic scorpion""
                                    }},
                                    sodium_8 = {{
                                        title = ""Your orders, my drinks"",
                                        hints = {{
                                            ""-""
                                        }},
                                        description = ""Served a drink at scorpio's bar""
                                    }},
                                    sodium_9 = {{
                                        title = ""A taste of success"",
                                        hints = {{
                                            ""-""
                                        }},
                                        description = ""Had a drink at scorpio's bar""
                                    }},
                                    sodium_10 = {{
                                        title = ""Salt Trainee"",
                                        hints = {{
                                            ""-""
                                        }},
                                        description = ""Play a round of the salt trainer""
                                    }},
                                    sodium_11 = {{
                                        title = ""Rock Muncher"",
                                        hints = {{
                                            ""-""
                                        }},
                                        description = ""Collect 250 resources""
                                    }},
                                    sodium_12 = {{
                                        title = ""Salt Greenhorn"",
                                        hints = {{
                                            ""-""
                                        }},
                                        description = ""Complete level 9""
                                    }},
                                    sodium_13 = {{
                                        title = ""Salt Master"",
                                        hints = {{
                                            ""-""
                                        }},
                                        description = ""Complete level 40""
                                    }},
                                    sodium_14 = {{
                                        title = ""Talked to VICKIE"",
                                        hints = {{
                                            ""If you are reading this, you already completed it!""
                                        }},
                                        description = ""See, robots aren't scary!""
                                    }},
                                    sodium_16 = {{
                                        title = ""Salt Trainee Master"",
                                        hints = {{
                                            ""-""
                                        }},
                                        description = ""Reach a score of at least 10 in the trainer""
                                    }},
                                    sodium_17 = {{
                                        title = ""Salt Trainee Guru"",
                                        hints = {{
                                            ""-""
                                        }},
                                        description = ""Reach a score of at least 15 in the trainer""
                                    }},
                                    sodium_18 = {{
                                        title = ""Pew Pew Pew"",
                                        hints = {{
                                            ""-""
                                        }},
                                        description = ""Play the salt shooter game""
                                    }},
                                    sodium_19 = {{
                                        title = ""Lock and Load"",
                                        hints = {{
                                            ""-""
                                        }},
                                        description = ""Reach level 50""
                                    }},
                                    sodium_20 = {{
                                        title = ""Twinkle Toes"",
                                        hints = {{
                                            ""-""
                                        }},
                                        description = ""Stomp 20 robotic scorpions without being stung in a single session.""
                                    }},
                                    sodium_22 = {{
                                        title = ""Mmmmm, Poison"",
                                        hints = {{
                                            ""-""
                                        }},
                                        description = ""Expose yourself to the scorpion's sting while wearing boots""
                                    }},
                                    sodium_23 = {{
                                        title = ""Supersonic Soles"",
                                        hints = {{
                                            ""-""
                                        }},
                                        description = ""Stomp 30 robotic scorpions within 60 seconds.""
                                    }},
                                    sodium_24 = {{
                                        title = ""Scorpion Dodger"",
                                        hints = {{
                                            ""-""
                                        }},
                                        description = ""Achieve a multiplier score of 4.""
                                    }},
                                    sodium_25 = {{
                                        title = ""Scorpion Scraper"",
                                        hints = {{
                                            ""-""
                                        }},
                                        description = ""Achieve a multiplier score of 5.""
                                    }},
                                    sodium_26 = {{
                                        title = ""Scorpion Overlord"",
                                        hints = {{
                                            ""-""
                                        }},
                                        description = ""Achieve a multiplier score of 6.""
                                    }},
                                    sodium_31 = {{
                                        title = ""Papers, Please"",
                                        hints = {{
                                            ""-""
                                        }},
                                        description = ""Entered the VIP area at Scorpio's""
                                    }},
                                    sodium_39 = {{
                                        title = ""Pincer Movement"",
                                        hints = {{
                                            ""-""
                                        }},
                                        description = ""Stomp 50 robotic scorpions in a single session""
                                    }},
                                    sodium_40 = {{
                                        title = ""Supersonic Soles"",
                                        hints = {{
                                            ""-""
                                        }},
                                        description = ""Stomp 200 robotic scorpions in a single session""
                                    }},
                                    sodium_41 = {{
                                        title = ""Supersonic Soles"",
                                        hints = {{
                                            ""-""
                                        }},
                                        description = ""Stomp 500 robotic scorpions in a single session""
                                    }},
                                    sodium_44 = {{
                                        title = ""Welcome to Sodium One"",
                                        hints = {{
                                            ""Welcome to Sodium One""
                                        }},
                                        description = ""Automatically Rewarded""
                                    }},
                                    sodium_45 = {{
                                        title = ""Friendship is magic"",
                                        hints = {{
                                            ""-""
                                        }},
                                        description = ""Be invited by a friend to Sodium One""
                                    }},
                                    sodium_46 = {{
                                        title = ""For the greater good"",
                                        hints = {{
                                            ""-""
                                        }},
                                        description = ""Invite a friend to Sodium One""
                                    }},
                                    sodium_47 = {{
                                        title = ""Worth your salt"",
                                        hints = {{
                                            ""-""
                                        }},
                                        description = ""Complete level 50""
                                    }},
                                    sodium_48 = {{
                                        title = ""Salt Apprentice"",
                                        hints = {{
                                            ""-""
                                        }},
                                        description = ""Complete level 3""
                                    }},
                                    sodium_49 = {{
                                        title = ""Salt Intern"",
                                        hints = {{
                                            ""-""
                                        }},
                                        description = ""Complete level 5""
                                    }},
                                    sodium_50 = {{
                                        title = ""Salt Professional"",
                                        hints = {{
                                            ""-""
                                        }},
                                        description = ""Complete level 30""
                                    }},
                                    sodium_51 = {{
                                        title = ""Salt Legend"",
                                        hints = {{
                                            ""-""
                                        }},
                                        description = ""Complete level 51""
                                    }},
                                    sodium_64 = {{
                                        title = ""This gatorade tastes weird!"",
                                        hints = {{
                                            ""-""
                                        }},
                                        description = ""Take your first drink of vetoxade""
                                    }},
                                    sodium_66 = {{
                                        title = ""I <![CDATA[ <3 ]]> Vickie!"",
                                        hints = {{
                                            ""-""
                                        }},
                                        description = ""Own the vickie statue and a pair of vickie headphones""
                                    }},
                                    sodium_72 = {{
                                        title = ""Bird on a Wire"",
                                        hints = {{
                                            ""-""
                                        }},
                                        description = ""Reach the sky!""
                                    }},
                                    sodium_81 = {{
                                        title = ""Joining The Fold"",
                                        hints = {{
                                            ""-""
                                        }},
                                        description = ""Obtain the Light Assault Drone ornament""
                                    }},
                                    sodium_82 = {{
                                        title = ""Salt Shooter Looter 1"",
                                        hints = {{
                                            ""-""
                                        }},
                                        description = ""Obtain the Sand Stalker ornament""
                                    }},
                                    sodium_83 = {{
                                        title = ""Salt Shooter Looter 2"",
                                        hints = {{
                                            ""-""
                                        }},
                                        description = ""Obtain the Sand Skater ornament""
                                    }},
                                    sodium_84 = {{
                                        title = ""Salt Shooter Looter 3"",
                                        hints = {{
                                            ""-""
                                        }},
                                        description = ""Obtain the Skimmer Assault Drone ornament""
                                    }},
                                    sodium_85 = {{
                                        title = ""Salt Shooter Looter 4"",
                                        hints = {{
                                            ""-""
                                        }},
                                        description = ""Obtain the Heavy LRM Drone ornament""
                                    }},
                                    sodium_86 = {{
                                        title = ""Salt Shooter Looter 5"",
                                        hints = {{
                                            ""-""
                                        }},
                                        description = ""Obtain the SodiumOne Mini MKII Hover Tank ornament""
                                    }},
                                    sodium_92 = {{
                                        title = ""88 m/s in the sky"",
                                        hints = {{
                                            ""-""
                                        }},
                                        description = ""Reach 88 m/s with the blimp""
                                    }},
                                    sodium_93 = {{
                                        title = ""Leave blimp flying"",
                                        hints = {{
                                            ""-""
                                        }},
                                        description = ""Leave the blimp flying commands""
                                    }},
                                    sodium_97 = {{
                                        title = ""Scorpion Stomp Newbie"",
                                        hints = {{
                                            ""Stomp scorpions to reach this objective!""
                                        }},
                                        description = ""Obtain the level 1 stomper boots""
                                    }},
                                    sodium_98 = {{
                                        title = ""111,111,111"",
                                        hints = {{
                                            ""-""
                                        }},
                                        description = ""Collectively stomp 111,111,111 Scorpions""
                                    }},
                                    {(version == "v2.3" ? @"sodium_100 = {
                                        title = ""Scorpion Stomp Apprentice"",
                                        hints = {
                                            ""Stomp scorpions to reach this objective!""
                                        },
                                        description = ""Obtain the level 2 stomper boots""
                                    }" : @"sodium_100 = {
                                        title = ""Initiate Customization flying skills"",
                                        hints = {
                                            ""-""
                                        },
                                        description = ""Change the blimp skin to anything other than basic""
                                    }")},
                                    {(version == "v2.3" ? @"ssodium_101 = {
                                        title = ""Scorpion Stomp Master"",
                                        hints = {
                                            ""Stomp scorpions to reach this objective!""
                                        },
                                        description = ""Obtain the level 3 stomper boots""
                                    }" : @"sodium_101 = {
                                        title = ""Teleporter on the fly"",
                                        hints = {
                                            ""-""
                                        },
                                        description = ""Use telepad in the blimp""
                                    }")},
                                    {(version == "v2.3" ? @"sodium_102 = {
                                        title = ""Scorpion Stomp Tycoon"",
                                        hints = {
                                            ""Stomp scorpions to reach this objective!""
                                        },
                                        description = ""Obtain the level 4 stomper boots""
                                    }" : @"sodium_102 = {
                                        title = ""Geyser Man"",
                                        hints = {
                                            ""-""
                                        },
                                        description = ""Focus of geyser for 10 seconds""
                                    }")},
                                    {(version == "v2.3" ? @"sodium_103 = {
                                        title = ""Scorpion Stomp Legend"",
                                        hints = {
                                            ""Stomp scorpions to reach this objective!""
                                        },
                                        description = ""Obtain the level 5 stomper boots""
                                    }" : @"sodium_103 = {
                                        title = ""Teleporter on the fly (with style)"",
                                        hints = {
                                            ""Any homester will know what needs to be done...""
                                        },
                                        description = ""Use telepad in the blimp in a specific way""
                                    }")},
                                    sodium_104 = {{
                                        title = ""Pro Blimp Driver"",
                                        hints = {{
                                            ""Stomp scorpions to reach this objective!""
                                        }},
                                        description = ""Fly whilst driving with the blimp""
                                    }},
                                    sodium_106 = {{
                                        title = ""Archeological Beginning"",
                                        hints = {{
                                            ""-""
                                        }},
                                        description = ""Activate your first thumper""
                                    }},
                                    sodium_107 = {{
                                        title = ""Old School Blimper"",
                                        hints = {{
                                            ""-""
                                        }},
                                        description = ""Use the sodium one skin on the blimp""
                                    }},
                                    sodium_111 = {{
                                        title = ""Rock Dancer"",
                                        hints = {{
                                            ""-""
                                        }},
                                        description = ""Dance on a rock 10 times in one session""
                                    }},
                                    sodium_112 = {{
                                        title = ""Unsafe mind"",
                                        hints = {{
                                            ""-""
                                        }},
                                        description = ""Return dehydrated 10 times in one session""
                                    }},
                                    sodium_113 = {{
                                        title = ""Good Haul"",
                                        hints = {{
                                            ""-""
                                        }},
                                        description = ""Collect all rewards on the blimp space""
                                    }},
                                    sodium_115 = {{
                                        title = ""20 successful digs in one session"",
                                        hints = {{
                                            ""-""
                                        }},
                                        description = ""Obtain 20 successful digs in one session""
                                    }},
                                    sodium_116 = {{
                                        title = ""50 successful digs in one session"",
                                        hints = {{
                                            ""-""
                                        }},
                                        description = ""Obtain 50 successful digs in one session""
                                    }},
                                    sodium_150 = {{
                                        title = ""5 Drinks Served"",
                                        hints = {{
                                            ""-""
                                        }},
                                        description = ""Served 5 drinks at scorpio's bar""
                                    }},
                                    sodium_151 = {{
                                        title = ""25 Drinks Served"",
                                        hints = {{
                                            ""-""
                                        }},
                                        description = ""Served 25 drinks at scorpio's bar""
                                    }},
                                    sodium_152 = {{
                                        title = ""100 Drinks Served"",
                                        hints = {{
                                            ""-""
                                        }},
                                        description = ""Served 100 drinks at scorpio's bar""
                                    }},
                                    sodium_153 = {{
                                        title = ""500 Drinks Served"",
                                        hints = {{
                                            ""-""
                                        }},
                                        description = ""Served 500 drinks at scorpio's bar""
                                    }},
                                    sodium_154 = {{
                                        title = ""1000 Drinks Served"",
                                        hints = {{
                                            ""-""
                                        }},
                                        description = ""Served 1000 drinks at scorpio's bar""
                                    }},
                                }}
                            }}

                            return Encode(TableFromInput, 4, 4)
                            "));
                        return;
                    default:
                        LoggerAccessor.LogWarn($"[PostAuthParameters] - Sodium objectives regional definition data was not found for project:{project}!");
                        break;
                }
                ctx.Response.StatusCode = (int)HttpStatusCode.NotFound;
                await ctx.Response.Send();
            });

            server.Routes.PostAuthentication.Parameter.Add(HttpMethod.GET, "/webassets/Sodium/objectives/data/{version}/{project}", async (ctx) =>
            {
                string? project = ctx.Request.Url.Parameters["project"];
                if (string.IsNullOrEmpty(project))
                {
                    ctx.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    ctx.Response.ContentType = "text/plain";
                    await ctx.Response.Send();
                    return;
                }
                string version = ctx.Request.Url.Parameters["version"] ?? "7";

                string xmlPath = $"/webassets/Sodium/objectives/data/{version}/{project}";
                string filePath = ApacheNetServerConfiguration.HTTPStaticFolder + xmlPath;

                if (File.Exists(filePath))
                {
                    ctx.Response.StatusCode = (int)HttpStatusCode.OK;
                    ctx.Response.ContentType = "text/xml";
                    await ctx.Response.Send(File.ReadAllText(filePath));
                    return;
                }

                LoggerAccessor.LogDebug($"[PostAuthParameters] - Sodium objectives {project} definition data was not found, falling back to static file.");

                switch (project)
                {
                    case "ObjectiveOrder.xml":
                        ctx.Response.StatusCode = (int)HttpStatusCode.OK;
                        ctx.Response.ContentType = "text/xml";
                        await ctx.Response.Send(LUA2XmlProcessor.TransformLuaTableToXml(@"
                            local TableFromInput = {
                                ORDER = {
                                    ""sodium_44"", ""sodium_14"", ""sodium_45"", ""sodium_46"", ""sodium_66"", ""sodium_1"", 
                                    ""sodium_20"", ""sodium_22"", ""sodium_23"", ""sodium_24"", ""sodium_25"", ""sodium_26"", 
                                    ""sodium_39"", ""sodium_40"", ""sodium_41"", ""sodium_97"", ""sodium_100"", ""sodium_101"", 
                                    ""sodium_102"", ""sodium_103"", ""sodium_98"", ""sodium_8"", ""sodium_9"", ""sodium_31"", 
                                    ""sodium_64"", ""sodium_150"", ""sodium_151"", ""sodium_152"", ""sodium_153"", ""sodium_154"", 
                                    ""sodium_18"", ""sodium_11"", ""sodium_48"", ""sodium_49"", ""sodium_12"", ""sodium_50"", 
                                    ""sodium_13"", ""sodium_19"", ""sodium_47"", ""sodium_51"", ""sodium_72"", ""sodium_81"",
                                    ""sodium_82"", ""sodium_83"", ""sodium_84"", ""sodium_85"", ""sodium_86"", ""sodium_10"",
                                    ""sodium_16"", ""sodium_17"", ""sodium_92"", ""sodium_93"", ""sodium_104"", ""sodium_107"",
                                    ""sodium_115"", ""sodium_116"", ""sodium_111"", ""sodium_113"", ""sodium_112"", ""sodium_106"",
                                }
                            }

                            return Encode(TableFromInput, 4, 4)
                            "));
                        return;
                    case "ObjectiveDefs.xml":
                        ctx.Response.StatusCode = (int)HttpStatusCode.OK;
                        ctx.Response.ContentType = "text/xml";
                        await ctx.Response.Send(LUA2XmlProcessor.TransformLuaTableToXml($@"local TableFromInput = {{
                              OBJECTIVES = {{
                                  sodium_1 = {{
                                      credits = 10,
                                      img_incmplt = ""https://www.outso-srv1.com/webassets/Sodium/objectives/images/{version}/stomp_incomplete.dds"",
                                      img_cmplt = ""https://www.outso-srv1.com/webassets/Sodium/objectives/images/{version}/stomp_complete.dds"",
                                      project = ""scorpion_game"",
                                      obtype = ""EVENT"",
                                      current_key = ""totalKills"",
                                      unlock_target = false,
                                  }},
                                  sodium_8 = {{
                                      credits = 10,
                                      img_incmplt = ""https://www.outso-srv1.com/webassets/Sodium/objectives/images/{version}/bar_incomplete.dds"",
                                      img_cmplt = ""https://www.outso-srv1.com/webassets/Sodium/objectives/images/{version}/bar_complete.dds"",
                                      project = ""desert_quench"",
                                      obtype = ""EVENT"",
                                      unlock_target = false,
                                  }},
                                  sodium_9 = {{
                                      credits = 10,
                                      img_incmplt = ""https://www.outso-srv1.com/webassets/Sodium/objectives/images/{version}/bar_incomplete.dds"",
                                      img_cmplt = ""https://www.outso-srv1.com/webassets/Sodium/objectives/images/{version}/bar_complete.dds"",
                                      project = ""desert_quench"",
                                      obtype = ""EVENT"",
                                      unlock_target = false,
                                  }},
                                  sodium_10 = {{
                                      credits = 50,
                                      img_incmplt = ""https://www.outso-srv1.com/webassets/Sodium/objectives/images/{version}/shooter_incomplete.dds"",
                                      img_cmplt = ""https://www.outso-srv1.com/webassets/Sodium/objectives/images/{version}/shooter_complete.dds"",
                                      project = ""shooter_game"",
                                      obtype = ""EVENT"",
                                      unlock_target = false,
                                  }},
                                  sodium_11 = {{
                                      credits = 150,
                                      img_incmplt = ""https://www.outso-srv1.com/webassets/Sodium/objectives/images/{version}/shooter_incomplete.dds"",
                                      img_cmplt = ""https://www.outso-srv1.com/webassets/Sodium/objectives/images/{version}/shooter_complete.dds"",
                                      project = ""shooter_game"",
                                      obtype = ""EVENT"",
                                      unlock_target = false,
                                  }},
                                  sodium_12 = {{
                                      credits = 3000,
                                      img_incmplt = ""https://www.outso-srv1.com/webassets/Sodium/objectives/images/{version}/shooter_incomplete.dds"",
                                      img_cmplt = ""https://www.outso-srv1.com/webassets/Sodium/objectives/images/{version}/shooter_complete.dds"",
                                      project = ""shooter_game"",
                                      obtype = ""EVENT"",
                                      unlock_target = false,
                                  }},
                                  sodium_13 = {{
                                      credits = 50000,
                                      img_incmplt = ""https://www.outso-srv1.com/webassets/Sodium/objectives/images/{version}/shooter_incomplete.dds"",
                                      img_cmplt = ""https://www.outso-srv1.com/webassets/Sodium/objectives/images/{version}/shooter_complete.dds"",
                                      project = ""shooter_game"",
                                      obtype = ""EVENT"",
                                      unlock_target = false,
                                  }},
                                  sodium_14 = {{
                                      credits = 10,
                                      img_incmplt = ""https://www.outso-srv1.com/webassets/Sodium/objectives/images/{version}/sodium_incomplete.dds"",
                                      img_cmplt = ""https://www.outso-srv1.com/webassets/Sodium/objectives/images/{version}/sodium_complete.dds"",
                                      project = ""npc"",
                                      obtype = ""EVENT"",
                                      unlock_target = false,
                                  }},
                                  sodium_16 = {{
                                      credits = 1000,
                                      img_incmplt = ""https://www.outso-srv1.com/webassets/Sodium/objectives/images/{version}/shooter_incomplete.dds"",
                                      img_cmplt = ""https://www.outso-srv1.com/webassets/Sodium/objectives/images/{version}/shooter_complete.dds"",
                                      project = ""shooter_game"",
                                      obtype = ""EVENT"",
                                      unlock_target = false,
                                  }},
                                  sodium_17 = {{
                                      credits = 5000,
                                      img_incmplt = ""https://www.outso-srv1.com/webassets/Sodium/objectives/images/{version}/shooter_incomplete.dds"",
                                      img_cmplt = ""https://www.outso-srv1.com/webassets/Sodium/objectives/images/{version}/shooter_complete.dds"",
                                      project = ""shooter_game"",
                                      obtype = ""EVENT"",
                                      unlock_target = false,
                                  }},
                                  sodium_18 = {{
                                      credits = 50,
                                      img_incmplt = ""https://www.outso-srv1.com/webassets/Sodium/objectives/images/{version}/shooter_incomplete.dds"",
                                      img_cmplt = ""https://www.outso-srv1.com/webassets/Sodium/objectives/images/{version}/shooter_complete.dds"",
                                      project = ""shooter_game"",
                                      obtype = ""EVENT"",
                                      unlock_target = false,
                                  }},
                                  sodium_19 = {{
                                      rewards = {{
                                          ""53309DBD-77B946AB-94B97CF7-506B5069"",
                                      }},
                                      img_incmplt = ""https://www.outso-srv1.com/webassets/Sodium/objectives/images/{version}/shooter_incomplete.dds"",
                                      img_cmplt = ""https://www.outso-srv1.com/webassets/Sodium/objectives/images/{version}/shooter_complete.dds"",
                                      project = ""shooter_game"",
                                      obtype = ""EVENT"",
                                      credits = 75000,
                                      unlock_target = false,
                                  }},
                                  sodium_20 = {{
                                      credits = 1500,
                                      img_incmplt = ""https://www.outso-srv1.com/webassets/Sodium/objectives/images/{version}/stomp_incomplete.dds"",
                                      img_cmplt = ""https://www.outso-srv1.com/webassets/Sodium/objectives/images/{version}/stomp_complete.dds"",
                                      project = ""scorpion_game"",
                                      obtype = ""EVENT"",
                                      unlock_target = false,
                                  }},
                                  sodium_22 = {{
                                      credits = 5000,
                                      img_incmplt = ""https://www.outso-srv1.com/webassets/Sodium/objectives/images/{version}/stomp_incomplete.dds"",
                                      img_cmplt = ""https://www.outso-srv1.com/webassets/Sodium/objectives/images/{version}/stomp_complete.dds"",
                                      project = ""scorpion_game"",
                                      obtype = ""EVENT"",
                                      unlock_target = 1,
                                  }},
                                  sodium_23 = {{
                                      credits = 5000,
                                      img_incmplt = ""https://www.outso-srv1.com/webassets/Sodium/objectives/images/{version}/stomp_incomplete.dds"",
                                      img_cmplt = ""https://www.outso-srv1.com/webassets/Sodium/objectives/images/{version}/stomp_complete.dds"",
                                      project = ""scorpion_game"",
                                      obtype = ""EVENT"",
                                      unlock_target = false,
                                  }},
                                  sodium_24 = {{
                                      credits = 150,
                                      img_incmplt = ""https://www.outso-srv1.com/webassets/Sodium/objectives/images/{version}/stomp_incomplete.dds"",
                                      img_cmplt = ""https://www.outso-srv1.com/webassets/Sodium/objectives/images/{version}/stomp_complete.dds"",
                                      project = ""scorpion_game"",
                                      obtype = ""EVENT"",
                                  }},
                                  sodium_25 = {{
                                      credits = 300,
                                      img_incmplt = ""https://www.outso-srv1.com/webassets/Sodium/objectives/images/{version}/stomp_incomplete.dds"",
                                      img_cmplt = ""https://www.outso-srv1.com/webassets/Sodium/objectives/images/{version}/stomp_complete.dds"",
                                      project = ""scorpion_game"",
                                      obtype = ""EVENT"",
                                      current_key = ""multiplierLevel"",
                                      unlock_target = 5,
                                  }},
                                  sodium_26 = {{
                                      credits = 500,
                                      img_incmplt = ""https://www.outso-srv1.com/webassets/Sodium/objectives/images/{version}/stomp_incomplete.dds"",
                                      img_cmplt = ""https://www.outso-srv1.com/webassets/Sodium/objectives/images/{version}/stomp_complete.dds"",
                                      project = ""scorpion_game"",
                                      obtype = ""EVENT"",
                                      current_key = ""multiplierLevel"",
                                      unlock_target = 6,
                                  }},
                                  sodium_31 = {{
                                      credits = 1000,
                                      img_incmplt = ""https://www.outso-srv1.com/webassets/Sodium/objectives/images/{version}/bar_incomplete.dds"",
                                      img_cmplt = ""https://www.outso-srv1.com/webassets/Sodium/objectives/images/{version}/bar_complete.dds"",
                                      project = ""desert_quench"",
                                      obtype = ""EVENT"",
                                      unlock_target = false,
                                  }},
                                  sodium_39 = {{
                                      credits = 1000,
                                      img_incmplt = ""https://www.outso-srv1.com/webassets/Sodium/objectives/images/{version}/stomp_incomplete.dds"",
                                      img_cmplt = ""https://www.outso-srv1.com/webassets/Sodium/objectives/images/{version}/stomp_complete.dds"",
                                      project = ""scorpion_game"",
                                      obtype = ""EVENT"",
                                      unlock_target = 1,
                                  }},
                                  sodium_40 = {{
                                      credits = 1000,
                                      img_incmplt = ""https://www.outso-srv1.com/webassets/Sodium/objectives/images/{version}/stomp_incomplete.dds"",
                                      img_cmplt = ""https://www.outso-srv1.com/webassets/Sodium/objectives/images/{version}/stomp_complete.dds"",
                                      project = ""scorpion_game"",
                                      obtype = ""EVENT"",
                                      unlock_target = 1,
                                  }},
                                  sodium_41 = {{
                                      credits = 1000,
                                      img_incmplt = ""https://www.outso-srv1.com/webassets/Sodium/objectives/images/{version}/stomp_incomplete.dds"",
                                      img_cmplt = ""https://www.outso-srv1.com/webassets/Sodium/objectives/images/{version}/stomp_complete.dds"",
                                      project = ""scorpion_game"",
                                      obtype = ""EVENT"",
                                      unlock_target = 1,
                                  }},
                                  sodium_44 = {{
                                      rewards = {{
                                          ""3DCA3C46-AAE54E72-BCB30EAC-3F58F795"",
                                      }},
                                      img_incmplt = ""https://www.outso-srv1.com/webassets/Sodium/objectives/images/{version}/vickie_incomplete.dds"",
                                      img_cmplt = ""https://www.outso-srv1.com/webassets/Sodium/objectives/images/{version}/vickie_complete.dds"",
                                      project = ""npc"",
                                      obtype = ""EVENT"",
                                      credits = 1000,
                                      unlock_target = false,
                                  }},
                                  sodium_45 = {{
                                      credits = 50,
                                      img_incmplt = ""https://www.outso-srv1.com/webassets/Sodium/objectives/images/{version}/sodium_incomplete.dds"",
                                      img_cmplt = ""https://www.outso-srv1.com/webassets/Sodium/objectives/images/{version}/sodium_complete.dds"",
                                      project = ""npc"",
                                      obtype = ""EVENT"",
                                      unlock_target = false,
                                  }},
                                  sodium_46 = {{
                                      credits = 50,
                                      img_incmplt = ""https://www.outso-srv1.com/webassets/Sodium/objectives/images/{version}/sodium_incomplete.dds"",
                                      img_cmplt = ""https://www.outso-srv1.com/webassets/Sodium/objectives/images/{version}/sodium_complete.dds"",
                                      project = ""npc"",
                                      obtype = ""EVENT"",
                                      unlock_target = false,
                                  }},
                                  sodium_47 = {{
                                      credits = 100000,
                                      img_incmplt = ""https://www.outso-srv1.com/webassets/Sodium/objectives/images/{version}/shooter_incomplete.dds"",
                                      img_cmplt = ""https://www.outso-srv1.com/webassets/Sodium/objectives/images/{version}/shooter_complete.dds"",
                                      project = ""shooter_game"",
                                      obtype = ""EVENT"",
                                      unlock_target = false,
                                  }},
                                  sodium_48 = {{
                                      rewards = {{
                                          ""D5920D88-5D614595-8C31E578-491684D3"",
                                      }},
                                      img_incmplt = ""https://www.outso-srv1.com/webassets/Sodium/objectives/images/{version}/shooter_incomplete.dds"",
                                      img_cmplt = ""https://www.outso-srv1.com/webassets/Sodium/objectives/images/{version}/shooter_complete.dds"",
                                      project = ""shooter_game"",
                                      obtype = ""EVENT"",
                                      credits = 500,
                                      unlock_target = false,
                                  }},
                                  sodium_49 = {{
                                      rewards = {{
                                          ""E6F6DD5A-847B4214-B7F3C17A-3BFFF076"",
                                      }},
                                      img_incmplt = ""https://www.outso-srv1.com/webassets/Sodium/objectives/images/{version}/shooter_incomplete.dds"",
                                      img_cmplt = ""https://www.outso-srv1.com/webassets/Sodium/objectives/images/{version}/shooter_complete.dds"",
                                      project = ""shooter_game"",
                                      obtype = ""EVENT"",
                                      credits = 1500,
                                      unlock_target = false,
                                  }},
                                  sodium_50 = {{
                                      rewards = {{
                                          ""7BE0909E-FECD4FDB-A117ADAC-39699691"",
                                      }},
                                      img_incmplt = ""https://www.outso-srv1.com/webassets/Sodium/objectives/images/{version}/shooter_incomplete.dds"",
                                      img_cmplt = ""https://www.outso-srv1.com/webassets/Sodium/objectives/images/{version}/shooter_complete.dds"",
                                      project = ""shooter_game"",
                                      obtype = ""EVENT"",
                                      credits = 20000,
                                      unlock_target = false,
                                  }},
                                  sodium_51 = {{
                                      credits = 150000,
                                      img_incmplt = ""https://www.outso-srv1.com/webassets/Sodium/objectives/images/{version}/shooter_incomplete.dds"",
                                      img_cmplt = ""https://www.outso-srv1.com/webassets/Sodium/objectives/images/{version}/shooter_complete.dds"",
                                      project = ""shooter_game"",
                                      obtype = ""EVENT"",
                                      unlock_target = false,
                                  }},
                                  sodium_64 = {{
                                      credits = 10,
                                      img_incmplt = ""https://www.outso-srv1.com/webassets/Sodium/objectives/images/{version}/vending_incomplete.dds"",
                                      img_cmplt = ""https://www.outso-srv1.com/webassets/Sodium/objectives/images/{version}/vending_complete.dds"",
                                      project = ""vending_machine"",
                                      obtype = ""EVENT"",
                                      unlock_target = false,
                                  }},
                                  sodium_66 = {{
                                      credits = 1000,
                                      img_incmplt = ""https://www.outso-srv1.com/webassets/Sodium/objectives/images/{version}/vickie_incomplete.dds"",
                                      img_cmplt = ""https://www.outso-srv1.com/webassets/Sodium/objectives/images/{version}/vickie_complete.dds"",
                                      project = ""npc"",
                                      obtype = ""EVENT"",
                                      unlock_target = 1,
                                  }},
                                  sodium_72 = {{
                                      credits = 10000,
                                      img_incmplt = ""https://www.outso-srv1.com/webassets/Sodium/objectives/images/{version}/vickie_incomplete.dds"",
                                      img_cmplt = ""https://www.outso-srv1.com/webassets/Sodium/objectives/images/{version}/vickie_complete.dds"",
                                      project = ""desert_quench"",
                                      obtype = ""EVENT"",
                                      unlock_target = false,
                                  }},
                                  sodium_81 = {{
                                      credits = 1000,
                                      img_incmplt = ""https://www.outso-srv1.com/webassets/Sodium/objectives/images/{version}/shooter_incomplete.dds"",
                                      img_cmplt = ""https://www.outso-srv1.com/webassets/Sodium/objectives/images/{version}/shooter_complete.dds"",
                                      project = ""shooter_game"",
                                      obtype = ""EVENT"",
                                      unlock_target = false,
                                  }},
                                  sodium_82 = {{
                                      credits = 1000,
                                      img_incmplt = ""https://www.outso-srv1.com/webassets/Sodium/objectives/images/{version}/shooter_incomplete.dds"",
                                      img_cmplt = ""https://www.outso-srv1.com/webassets/Sodium/objectives/images/{version}/shooter_complete.dds"",
                                      project = ""shooter_game"",
                                      obtype = ""EVENT"",
                                      unlock_target = false,
                                  }},
                                  sodium_83 = {{
                                      credits = 1000,
                                      img_incmplt = ""https://www.outso-srv1.com/webassets/Sodium/objectives/images/{version}/shooter_incomplete.dds"",
                                      img_cmplt = ""https://www.outso-srv1.com/webassets/Sodium/objectives/images/{version}/shooter_complete.dds"",
                                      project = ""shooter_game"",
                                      obtype = ""EVENT"",
                                      unlock_target = false,
                                  }},
                                  sodium_84 = {{
                                      credits = 1000,
                                      img_incmplt = ""https://www.outso-srv1.com/webassets/Sodium/objectives/images/{version}/shooter_incomplete.dds"",
                                      img_cmplt = ""https://www.outso-srv1.com/webassets/Sodium/objectives/images/{version}/shooter_complete.dds"",
                                      project = ""shooter_game"",
                                      obtype = ""EVENT"",
                                      unlock_target = false,
                                  }},
                                  sodium_85 = {{
                                      credits = 1000,
                                      img_incmplt = ""https://www.outso-srv1.com/webassets/Sodium/objectives/images/{version}/shooter_incomplete.dds"",
                                      img_cmplt = ""https://www.outso-srv1.com/webassets/Sodium/objectives/images/{version}/shooter_complete.dds"",
                                      project = ""shooter_game"",
                                      obtype = ""EVENT"",
                                      unlock_target = false,
                                  }},
                                  sodium_86 = {{
                                      credits = 1000,
                                      img_incmplt = ""https://www.outso-srv1.com/webassets/Sodium/objectives/images/{version}/shooter_incomplete.dds"",
                                      img_cmplt = ""https://www.outso-srv1.com/webassets/Sodium/objectives/images/{version}/shooter_complete.dds"",
                                      project = ""shooter_game"",
                                      obtype = ""EVENT"",
                                      unlock_target = false,
                                  }},
                                  sodium_92 = {{
                                      img_cmplt = ""https://www.outso-srv1.com/webassets/Sodium/objectives/images/{version}/blimp_complete.dds"",
                                      img_incmplt = ""https://www.outso-srv1.com/webassets/Sodium/objectives/images/{version}/blimp_incomplete.dds"",
                                      credits = 10,
                                      project = ""sodium_blimp"",
                                      obtype = ""EVENT"",
                                      unlock_target = false,
                                  }},
                                  sodium_93 = {{
                                      img_cmplt = ""https://www.outso-srv1.com/webassets/Sodium/objectives/images/{version}/blimp_complete.dds"",
                                      img_incmplt = ""https://www.outso-srv1.com/webassets/Sodium/objectives/images/{version}/blimp_incomplete.dds"",
                                      credits = 10,
                                      project = ""sodium_blimp"",
                                      obtype = ""EVENT"",
                                      unlock_target = false,
                                  }},
                                  sodium_97 = {{
                                      img_cmplt = ""https://www.outso-srv1.com/webassets/Sodium/objectives/images/{version}/stomp_complete.dds"",
                                      rewards = {{
                                          ""2DB00408-A1A34DD2-A90CC1C5-BD3D0762"",
                                          ""6576A366-B43148FA-9092AB60-A43B5104"",
                                      }},
                                      img_incmplt = ""https://www.outso-srv1.com/webassets/Sodium/objectives/images/{version}/stomp_incomplete.dds"",
                                      credits = 50000,
                                      target = 25,
                                      project = ""scorpion_game"",
                                      obtype = ""COLLECTION"",
                                      current_key = ""totalKills"",
                                      unlock_target = 0,
                                  }},
                                  sodium_98 = {{
                                      img_cmplt = ""https://www.outso-srv1.com/webassets/Sodium/objectives/images/{version}/stomp_complete.dds"",
                                      rewards = {{
                                          ""036B47FA-7CBE484C-B4309B77-F20A4CB3"",
                                          ""162DC30D-67F3404D-A76C8350-49415ED8"",
                                      }},
                                      img_incmplt = ""https://www.outso-srv1.com/webassets/Sodium/objectives/images/{version}/stomp_incomplete.dds"",
                                      credits = 7500,
                                      target = 111111111,
                                      project = ""scorpion_game"",
                                      obtype = ""COMM"",
                                      current_key = ""totalKills"",
                                      unlock_target = false,
                                  }},
                                  {(version == "v2.3" ? $@"sodium_100 = {{
                                      img_cmplt = ""https://www.outso-srv1.com/webassets/Sodium/objectives/images/{version}/stomp_complete.dds"",
                                      rewards = {{
                                          ""E913FC08-D7FB4412-A08793FB-DDC9E47A"",
                                          ""81243ABF-C9D6457B-871CF2C4-0FA4A687"",
                                      }},
                                      img_incmplt = ""https://www.outso-srv1.com/webassets/Sodium/objectives/images/{version}/stomp_incomplete.dds"",
                                      credits = 5500,
                                      target = 100,
                                      project = ""scorpion_game"",
                                      obtype = ""COLLECTION"",
                                      current_key = ""totalKills"",
                                      unlock_target = 25,
                                  }},
                                  sodium_101 = {{
                                      img_cmplt = ""https://www.outso-srv1.com/webassets/Sodium/objectives/images/{version}/stomp_complete.dds"",
                                      rewards = {{
                                          ""C212815C-92474496-AEAA4286-4583DA98"",
                                          ""E8396A6C-78C842B0-97A3843A-659C5FC3"",
                                      }},
                                      img_incmplt = ""https://www.outso-srv1.com/webassets/Sodium/objectives/images/{version}/stomp_incomplete.dds"",
                                      credits = 6000,
                                      target = 1000,
                                      project = ""scorpion_game"",
                                      obtype = ""COLLECTION"",
                                      current_key = ""totalKills"",
                                      unlock_target = 100,
                                  }},
                                  sodium_102 = {{
                                      img_cmplt = ""https://www.outso-srv1.com/webassets/Sodium/objectives/images/{version}/stomp_complete.dds"",
                                      rewards = {{
                                          ""9ACCC1EB-315F4990-A9D7990D-72B345AE"",
                                          ""F8471241-67384755-A249B2D9-FD25EF9D"",
                                      }},
                                      img_incmplt = ""https://www.outso-srv1.com/webassets/Sodium/objectives/images/{version}/stomp_incomplete.dds"",
                                      credits = 6500,
                                      target = 10000,
                                      project = ""scorpion_game"",
                                      obtype = ""COLLECTION"",
                                      current_key = ""totalKills"",
                                      unlock_target = 1000,
                                  }},
                                  sodium_103 = {{
                                      img_cmplt = ""https://www.outso-srv1.com/webassets/Sodium/objectives/images/{version}/stomp_complete.dds"",
                                      rewards = {{
                                          ""538FED1A-740D46FB-82001B80-9EE88209"",
                                          ""D6E00450-70B447C9-96F49BD6-BB6936FC"",
                                      }},
                                      img_incmplt = ""https://www.outso-srv1.com/webassets/Sodium/objectives/images/{version}/stomp_incomplete.dds"",
                                      credits = 7000,
                                      target = 50000,
                                      project = ""scorpion_game"",
                                      obtype = ""COLLECTION"",
                                      current_key = ""totalKills"",
                                      unlock_target = 10000,
                                  }}" : $@"sodium_100 = {{
                                      img_cmplt = ""https://www.outso-srv1.com/webassets/Sodium/objectives/images/{version}/blimp_complete.dds"",
                                      img_incmplt = ""https://www.outso-srv1.com/webassets/Sodium/objectives/images/{version}/blimp_incomplete.dds"",
                                      credits = 50,
                                      project = ""sodium_blimp"",
                                      obtype = ""EVENT"",
                                      unlock_target = false,
                                  }},
                                  sodium_101 = {{
                                      img_cmplt = ""https://www.outso-srv1.com/webassets/Sodium/objectives/images/{version}/blimp_complete.dds"",
                                      img_incmplt = ""https://www.outso-srv1.com/webassets/Sodium/objectives/images/{version}/blimp_incomplete.dds"",
                                      credits = 10,
                                      project = ""sodium_blimp"",
                                      obtype = ""EVENT"",
                                      unlock_target = false,
                                  }},
                                  sodium_102 = {{
                                      img_cmplt = ""https://www.outso-srv1.com/webassets/Sodium/objectives/images/{version}/blimp_complete.dds"",
                                      img_incmplt = ""https://www.outso-srv1.com/webassets/Sodium/objectives/images/{version}/blimp_incomplete.dds"",
                                      credits = 50,
                                      project = ""sodium_blimp"",
                                      obtype = ""EVENT"",
                                      unlock_target = false,
                                  }},
                                  sodium_103 = {{
                                      img_cmplt = ""https://www.outso-srv1.com/webassets/Sodium/objectives/images/{version}/blimp_complete.dds"",
                                      img_incmplt = ""https://www.outso-srv1.com/webassets/Sodium/objectives/images/{version}/blimp_incomplete.dds"",
                                      credits = 10,
                                      project = ""sodium_blimp"",
                                      obtype = ""EVENT"",
                                      unlock_target = false,
                                  }}")},
                                  sodium_106 = {{
                                      img_cmplt = ""https://www.outso-srv1.com/webassets/Sodium/objectives/images/{version}/blimp_complete.dds"",
                                      img_incmplt = ""https://www.outso-srv1.com/webassets/Sodium/objectives/images/{version}/blimp_incomplete.dds"",
                                      credits = 5,
                                      project = ""sodium_blimp"",
                                      obtype = ""EVENT"",
                                      unlock_target = false,
                                  }},
                                  sodium_107 = {{
                                      img_cmplt = ""https://www.outso-srv1.com/webassets/Sodium/objectives/images/{version}/blimp_complete.dds"",
                                      img_incmplt = ""https://www.outso-srv1.com/webassets/Sodium/objectives/images/{version}/blimp_incomplete.dds"",
                                      credits = 10,
                                      project = ""sodium_blimp"",
                                      obtype = ""EVENT"",
                                      unlock_target = false,
                                  }},
                                  sodium_111 = {{
                                      img_cmplt = ""https://www.outso-srv1.com/webassets/Sodium/objectives/images/{version}/blimp_complete.dds"",
                                      img_incmplt = ""https://www.outso-srv1.com/webassets/Sodium/objectives/images/{version}/blimp_incomplete.dds"",
                                      credits = 550,
                                      project = ""sodium_blimp"",
                                      obtype = ""EVENT"",
                                      unlock_target = false,
                                  }},
                                  sodium_112 = {{
                                      img_cmplt = ""https://www.outso-srv1.com/webassets/Sodium/objectives/images/{version}/blimp_complete.dds"",
                                      img_incmplt = ""https://www.outso-srv1.com/webassets/Sodium/objectives/images/{version}/blimp_incomplete.dds"",
                                      credits = 5,
                                      project = ""sodium_blimp"",
                                      obtype = ""EVENT"",
                                      unlock_target = false,
                                  }},
                                  sodium_113 = {{
                                      img_cmplt = ""https://www.outso-srv1.com/webassets/Sodium/objectives/images/{version}/blimp_complete.dds"",
                                      img_incmplt = ""https://www.outso-srv1.com/webassets/Sodium/objectives/images/{version}/blimp_incomplete.dds"",
                                      credits = 10000,
                                      project = ""sodium_blimp"",
                                      obtype = ""EVENT"",
                                      unlock_target = false,
                                  }},
                                  sodium_115 = {{
                                      img_cmplt = ""https://www.outso-srv1.com/webassets/Sodium/objectives/images/{version}/blimp_complete.dds"",
                                      img_incmplt = ""https://www.outso-srv1.com/webassets/Sodium/objectives/images/{version}/blimp_incomplete.dds"",
                                      credits = 1000,
                                      project = ""sodium_blimp"",
                                      obtype = ""EVENT"",
                                      unlock_target = false,
                                  }},
                                  sodium_116 = {{
                                      img_cmplt = ""https://www.outso-srv1.com/webassets/Sodium/objectives/images/{version}/blimp_complete.dds"",
                                      img_incmplt = ""https://www.outso-srv1.com/webassets/Sodium/objectives/images/{version}/blimp_incomplete.dds"",
                                      credits = 3500,
                                      project = ""sodium_blimp"",
                                      obtype = ""EVENT"",
                                      unlock_target = false,
                                  }},
                                  sodium_150 = {{
                                      img_cmplt = ""https://www.outso-srv1.com/webassets/Sodium/objectives/images/{version}/bar_complete.dds"",
                                      credits = 250,
                                      img_incmplt = ""https://www.outso-srv1.com/webassets/Sodium/objectives/images/{version}/bar_incomplete.dds"",
                                      rewards = {{
                                          ""F63C2F5E-8D55497D-BC606BEC-0CB3C696"",
                                          ""FF62B544-0B394747-818875A7-2A4F7698"",
                                      }},
                                      current_key = ""totalDrinksServed"",
                                      project = ""desert_quench"",
                                      obtype = ""COLLECTION"",
                                      target = 5,
                                      unlock_target = 0,
                                  }},
                                  sodium_151 = {{
                                      img_cmplt = ""https://www.outso-srv1.com/webassets/Sodium/objectives/images/{version}/bar_complete.dds"",
                                      credits = 500,
                                      img_incmplt = ""https://www.outso-srv1.com/webassets/Sodium/objectives/images/{version}/bar_incomplete.dds"",
                                      rewards = {{
                                          ""5EB5B2D0-8870493D-BDC48D0E-119AC360"",
                                          ""E0BB102A-E0454C68-B180E81C-BBFB811F"",
                                      }},
                                      current_key = ""totalDrinksServed"",
                                      project = ""desert_quench"",
                                      obtype = ""COLLECTION"",
                                      target = 25,
                                      unlock_target = 5,
                                  }},
                                  sodium_152 = {{
                                      img_cmplt = ""https://www.outso-srv1.com/webassets/Sodium/objectives/images/{version}/bar_complete.dds"",
                                      credits = 750,
                                      img_incmplt = ""https://www.outso-srv1.com/webassets/Sodium/objectives/images/{version}/bar_incomplete.dds"",
                                      rewards = {{
                                          ""0D261560-B0E94608-B04F9CA9-5D619683"",
                                          ""EA4957BA-CCA54DC9-A17E0835-1E8F207B"",
                                      }},
                                      current_key = ""totalDrinksServed"",
                                      project = ""desert_quench"",
                                      obtype = ""COLLECTION"",
                                      target = 100,
                                      unlock_target = 25,
                                  }},
                                  sodium_153 = {{
                                      img_cmplt = ""https://www.outso-srv1.com/webassets/Sodium/objectives/images/{version}/bar_complete.dds"",
                                      credits = 1000,
                                      img_incmplt = ""https://www.outso-srv1.com/webassets/Sodium/objectives/images/{version}/bar_incomplete.dds"",
                                      rewards = {{
                                          ""87F3B1F5-5D6B4E31-A42FA18A-AF7AA0F0"",
                                          ""910564B3-6F5D4204-94FBC652-F535AF11"",
                                      }},
                                      current_key = ""totalDrinksServed"",
                                      project = ""desert_quench"",
                                      obtype = ""COLLECTION"",
                                      target = 500,
                                      unlock_target = 100,
                                  }},
                                  sodium_154 = {{
                                      img_cmplt = ""https://www.outso-srv1.com/webassets/Sodium/objectives/images/{version}/bar_complete.dds"",
                                      credits = 1500,
                                      img_incmplt = ""https://www.outso-srv1.com/webassets/Sodium/objectives/images/{version}/bar_incomplete.dds"",
                                      rewards = {{
                                          ""2CAB3C68-45294E0C-8FE820E7-611E9C5D"",
                                          ""6E7E2581-0F244F28-9DABA308-9B66EDFF"",
                                      }},
                                      current_key = ""totalDrinksServed"",
                                      project = ""desert_quench"",
                                      obtype = ""COLLECTION"",
                                      target = 1000,
                                      unlock_target = 500,
                                  }},
                              }}
                            }}

                            return Encode(TableFromInput, 4, 4)
                            "));
                        return;
                    default:
                        LoggerAccessor.LogWarn($"[PostAuthParameters] - Sodium objectives definition data was not found for project:{project}!");
                        break;
                }
                ctx.Response.StatusCode = (int)HttpStatusCode.NotFound;
                await ctx.Response.Send();
            });
        }
    }
}
