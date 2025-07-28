using CustomLogger;
using System;
using System.IO;
using System.Net;
using WatsonWebserver.Core;
using WebAPIService.GameServices.OHS;

namespace ApacheNet.BuildIn.RouteHandlers.GameRoutes.Lockwood
{
    internal static class IslandDevelopment
    {
        private static UniqueIDGenerator UniqueIDCounter = new UniqueIDGenerator(0);

        public static void BuildIslandPlugin(WebserverBase server)
        {
            server.Routes.PostAuthentication.Parameter.Add(HttpMethod.GET, "/static/Lockwood/Features/IslandDevelopment/Hub/{build}/{country}/setDressing.xml", async (ctx) =>
            {
                ctx.Response.StatusCode = (int)HttpStatusCode.OK;
                ctx.Response.ContentType = "text/xml";

                string xmlPath = $"/static/Lockwood/Features/IslandDevelopment/Hub/{ctx.Request.Url.Parameters["build"]}/{ctx.Request.Url.Parameters["country"]}/setDressing.xml";
                string filePath = ApacheNetServerConfiguration.HTTPStaticFolder + xmlPath;

                if (File.Exists(filePath))
                {
                    await ctx.Response.Send(File.ReadAllText(filePath));
                    return;
                }

                LoggerAccessor.LogDebug($"[PostAuthParameters] - setDressing data was not found for IslandDevelopment scene:Hub, falling back to server file.");

                await ctx.Response.Send(LUA2XmlProcessor.TransformLuaTableToXml(@"
                            local maps = {
	                            'colourMap', 'normalMap', 'specularMap', 'envMap', 'emissiveMap', 'colour2Map', 'normal2Map', 'specular2Map'
                            }

                            setDressing = {
		                        profiles = {
                                    'commercePoints',
                                    'Twitter',
                                    'Gate',
                                    'LKWDLife'
		                        },
		                        entities = {}
	                        }

		                    local options = {
			                    profiles = {},
			                    entities = {},
		                    }
		                    local dressingDef = {}

		                    if setDressing then
			                    for _, name in ipairs(setDressing.profiles) do
				                    options.profiles[name] = {}
			                    end
		                    end
		                    if setDressing then
                                for entName, entDef in pairs(setDressing.entities) do
                                    options.entities[entName] = {}
                                    for matName, variants in pairs(entDef) do
                                        options.entities[entName][matName] = {}
                                        for optionId, optionData in pairs(variants) do
                                            options.entities[entName][matName][optionId] = optionData
                                        end
                                    end
                                end
                            end
                            if setDressing then
                                for entName, entDef in pairs(setDressing.entities) do
                                    dressingDef[entName] = {}
                                    for matName, variants in pairs(entDef) do
                                        dressingDef[entName][matName] = {}
                                        for optionId, _ in pairs(variants) do
                                            dressingDef[entName][matName] = optionId
                                        end
                                    end
                                end
                            end

                            local TableFromInput = {
					            options 	= options,
					            setups 		= {
						            ['default'] = {
							            profiles 	= {
                                            ['commercePoints'] = 'default',
                                            ['Gate'] = 'default',
                                            ['LKWDLife'] = 'default',
                                            ['Twitter'] = 'Lockwood_Ltd',
                                        },
							            dressing 	= {
								            entities 	= dressingDef,
							            },
						            },
					            },
					           schedule 	= {
						            january 	= {'default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default'},
						            february 	= {'default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default'},
						            march 		= {'default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default'},
						            april 		= {'default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default'},
						            may 		= {'default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default'},
						            june 		= {'default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default'},
						            july 		= {'default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default'},
						            august 		= {'default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default'},
						            september 	= {'default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default'},
						            october 	= {'default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default'},
						            november 	= {'default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default'},
						            december 	= {'default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default'},
					            },
					            customisationGroups 	= {},
					            customisation 			= {
						            profiles 	= {},
						            entities 	= {},
					            }
				            }

                            return XmlConvert.LuaToXml(TableFromInput, 'lua', 1)
                            "));
            });

            server.Routes.PostAuthentication.Parameter.Add(HttpMethod.GET, "/static/Lockwood/Features/IslandDevelopment/{sceneIdent}/{scenetype}/{build}/{country}/setDressing.xml", async (ctx) =>
            {
                string? sceneIdent = ctx.Request.Url.Parameters["sceneIdent"];
                if (string.IsNullOrEmpty(sceneIdent))
                {
                    ctx.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    ctx.Response.ContentType = "text/plain";
                    await ctx.Response.Send();
                    return;
                }
                string xmlPath = $"/static/Lockwood/Features/IslandDevelopment/{sceneIdent}/{ctx.Request.Url.Parameters["scenetype"]}/{ctx.Request.Url.Parameters["build"]}/{ctx.Request.Url.Parameters["country"]}/setDressing.xml";
                string filePath = ApacheNetServerConfiguration.HTTPStaticFolder + xmlPath;

                if (File.Exists(filePath))
                {
                    ctx.Response.StatusCode = (int)HttpStatusCode.OK;
                    ctx.Response.ContentType = "text/xml";
                    await ctx.Response.Send(File.ReadAllText(filePath));
                    return;
                }

                LoggerAccessor.LogDebug($"[PostAuthParameters] - setDressing data was not found for IslandDevelopment scene:{sceneIdent}, falling back to server file.");

                switch (sceneIdent)
                {
                    case "Lounge":
                    case "Beach":
                    case "Hideaway":
                    case "Forest":
                    case "Yacht":
                    case "IceYacht":
                        ctx.Response.StatusCode = (int)HttpStatusCode.OK;
                        ctx.Response.ContentType = "text/xml";
                        await ctx.Response.Send(LUA2XmlProcessor.TransformLuaTableToXml(@"
                            local maps = {
	                            'colourMap', 'normalMap', 'specularMap', 'envMap', 'emissiveMap', 'colour2Map', 'normal2Map', 'specular2Map'
                            }

                            setDressing = {
		                        profiles = {
			                        'Telescope',
                                    --'Dolphins',
                                    'Starfish',
                                    'Iguana',
                                    'Gecko',
                                    'FirePit',
                                    'SFX',
                                    'commercePoints',
                                    'Posertrons',
                                    'Fish',
                                    'Splashertron',
                                    'Gate',
                                    'LKWDLife'
		                        },
		                        entities = {}
	                        }

		                    local options = {
			                    profiles = {},
			                    entities = {},
		                    }
		                    local dressingDef = {}

		                    if setDressing then
			                    for _, name in ipairs(setDressing.profiles) do
				                    options.profiles[name] = {}
			                    end
		                    end
		                    if setDressing then
                                for entName, entDef in pairs(setDressing.entities) do
                                    options.entities[entName] = {}
                                    for matName, variants in pairs(entDef) do
                                        options.entities[entName][matName] = {}
                                        for optionId, optionData in pairs(variants) do
                                            options.entities[entName][matName][optionId] = optionData
                                        end
                                    end
                                end
                            end
                            if setDressing then
                                for entName, entDef in pairs(setDressing.entities) do
                                    dressingDef[entName] = {}
                                    for matName, variants in pairs(entDef) do
                                        dressingDef[entName][matName] = {}
                                        for optionId, _ in pairs(variants) do
                                            dressingDef[entName][matName] = optionId
                                        end
                                    end
                                end
                            end

                            local TableFromInput = {
					            options 	= options,
					            setups 		= {
						            ['default'] = {
							            profiles 	= {
                                            ['Telescope'] = 'default',
                                            ['FirePit'] = 'default',
                                            ['SFX'] = 'default',
                                            ['commercePoints'] = 'default',
                                            ['Splashertron'] = 'default',
                                            ['Gate'] = 'default',
                                            ['LKWDLife'] = 'default',
                                            ['Posertrons'] = 'default',
                                            ['Fish'] = 'both',
                                            ['Starfish'] = 'beach',
                                            ['Iguana'] = 'beach',
                                            ['Gecko'] = 'beach',
                                        },
							            dressing 	= {
								            entities 	= dressingDef,
							            },
						            },
					            },
					           schedule 	= {
						            january 	= {'default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default'},
						            february 	= {'default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default'},
						            march 		= {'default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default'},
						            april 		= {'default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default'},
						            may 		= {'default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default'},
						            june 		= {'default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default'},
						            july 		= {'default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default'},
						            august 		= {'default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default'},
						            september 	= {'default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default'},
						            october 	= {'default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default'},
						            november 	= {'default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default'},
						            december 	= {'default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default','default'},
					            },
					            customisationGroups 	= {},
					            customisation 			= {
						            profiles 	= {},
						            entities 	= {},
					            }
				            }

                            return XmlConvert.LuaToXml(TableFromInput, 'lua', 1)
                            "));
                        return;
                    default:
                        LoggerAccessor.LogWarn($"[PostAuthParameters] - setDressing definition data was not found for scene:{sceneIdent}!");
                        break;
                }
                ctx.Response.StatusCode = (int)HttpStatusCode.NotFound;
                await ctx.Response.Send();
            });

            server.Routes.PostAuthentication.Parameter.Add(HttpMethod.GET, "/static/Lockwood/Features/IslandDevelopment/Hub/{build}/{country}/features.xml", async (ctx) =>
            {
                ctx.Response.StatusCode = (int)HttpStatusCode.OK;
                ctx.Response.ContentType = "text/xml";

                string xmlPath = $"/static/Lockwood/Features/IslandDevelopment/Hub/{ctx.Request.Url.Parameters["build"]}/{ctx.Request.Url.Parameters["country"]}/features.xml";
                string filePath = ApacheNetServerConfiguration.HTTPStaticFolder + xmlPath;

                if (File.Exists(filePath))
                {
                    await ctx.Response.Send(File.ReadAllText(filePath));
                    return;
                }

                LoggerAccessor.LogDebug($"[PostAuthParameters] - features data was not found for IslandDevelopment scene:Hub, falling back to server file.");

                await ctx.Response.Send(LUA2XmlProcessor.TransformLuaTableToXml(@"
                            local TableFromInput = {
                                ['commercePoints'] = {
                                    default = true
                                },
                                ['Gate'] = {
                                    default = true
                                },
                                ['LKWDLife'] = {
                                    default = true
                                },
                                ['Twitter'] = {
                                    Lockwood_Ltd = true
                                },
				            }

                            return XmlConvert.LuaToXml(TableFromInput, 'lua', 1)
                            "));
            });

            server.Routes.PostAuthentication.Parameter.Add(HttpMethod.GET, "/static/Lockwood/Features/IslandDevelopment/{sceneIdent}/{scenetype}/{build}/{country}/features.xml", async (ctx) =>
            {
                string? sceneIdent = ctx.Request.Url.Parameters["sceneIdent"];
                if (string.IsNullOrEmpty(sceneIdent))
                {
                    ctx.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    ctx.Response.ContentType = "text/plain";
                    await ctx.Response.Send();
                    return;
                }
                string xmlPath = $"/static/Lockwood/Features/IslandDevelopment/{sceneIdent}/{ctx.Request.Url.Parameters["scenetype"]}/{ctx.Request.Url.Parameters["build"]}/{ctx.Request.Url.Parameters["country"]}/features.xml";
                string filePath = ApacheNetServerConfiguration.HTTPStaticFolder + xmlPath;

                if (File.Exists(filePath))
                {
                    ctx.Response.StatusCode = (int)HttpStatusCode.OK;
                    ctx.Response.ContentType = "text/xml";
                    await ctx.Response.Send(File.ReadAllText(filePath));
                    return;
                }

                LoggerAccessor.LogDebug($"[PostAuthParameters] - features data was not found for IslandDevelopment scene:{sceneIdent}, falling back to server file.");

                switch (sceneIdent)
                {
                    case "Hub":
                    case "Lounge":
                    case "Beach":
                    case "Hideaway":
                    case "Forest":
                    case "Yacht":
                    case "IceYacht":
                        ctx.Response.StatusCode = (int)HttpStatusCode.OK;
                        ctx.Response.ContentType = "text/xml";
                        await ctx.Response.Send(LUA2XmlProcessor.TransformLuaTableToXml(@"
                            local TableFromInput = {
                                ['Telescope'] = {
                                    default = true
                                },
                                ['FirePit'] = {
                                    default = true
                                },
                                ['SFX'] = {
                                    default = true
                                },
                                ['commercePoints'] = {
                                    default = true
                                },
                                ['Splashertron'] = {
                                    default = true
                                },
                                ['Gate'] = {
                                    default = true
                                },
                                ['Posertrons'] = {
                                    default = true
                                },
                                ['LKWDLife'] = {
                                    default = true
                                },
                                ['Fish'] = {
                                    both = true
                                },
                                ['Starfish'] = {
                                    beach = true
                                },
                                ['Iguana'] = {
                                    beach = true
                                },
                                ['Gecko'] = {
                                    beach = true
                                },
				            }

                            return XmlConvert.LuaToXml(TableFromInput, 'lua', 1)
                            "));
                        return;
                    default:
                        LoggerAccessor.LogWarn($"[PostAuthParameters] - features definition data was not found for scene:{sceneIdent}!");
                        break;
                }
                ctx.Response.StatusCode = (int)HttpStatusCode.NotFound;
                await ctx.Response.Send();
            });

            server.Routes.PostAuthentication.Parameter.Add(HttpMethod.GET, "/static/Lockwood/Features/IslandDevelopment/Hub/{build}/{country}/camPath.xml", async (ctx) =>
            {
                ctx.Response.StatusCode = (int)HttpStatusCode.OK;
                ctx.Response.ContentType = "text/xml";
                string xmlPath = $"/static/Lockwood/Features/IslandDevelopment/Hub/{ctx.Request.Url.Parameters["build"]}/{ctx.Request.Url.Parameters["country"]}/camPath.xml";
                string filePath = ApacheNetServerConfiguration.HTTPStaticFolder + xmlPath;

                if (File.Exists(filePath))
                {
                    await ctx.Response.Send(File.ReadAllText(filePath));
                    return;
                }

                LoggerAccessor.LogDebug($"[PostAuthParameters] - camPath data was not found for IslandDevelopment scene:Hub, falling back to server file.");

                await ctx.Response.Send("<lua></lua>");
            });

            server.Routes.PostAuthentication.Parameter.Add(HttpMethod.GET, "/static/Lockwood/Features/IslandDevelopment/{sceneIdent}/{scenetype}/{build}/{country}/camPath.xml", async (ctx) =>
            {
                ctx.Response.StatusCode = (int)HttpStatusCode.OK;
                ctx.Response.ContentType = "text/xml";
                string xmlPath = $"/static/Lockwood/Features/IslandDevelopment/{ctx.Request.Url.Parameters["sceneIdent"]}/{ctx.Request.Url.Parameters["scenetype"]}/{ctx.Request.Url.Parameters["build"]}/{ctx.Request.Url.Parameters["country"]}/camPath.xml";
                string filePath = ApacheNetServerConfiguration.HTTPStaticFolder + xmlPath;

                if (File.Exists(filePath))
                {
                    await ctx.Response.Send(File.ReadAllText(filePath));
                    return;
                }

                LoggerAccessor.LogDebug($"[PostAuthParameters] - camPath data was not found for IslandDevelopment scene:Hub, falling back to server file.");

                await ctx.Response.Send("<lua></lua>");
            });

            server.Routes.PostAuthentication.Parameter.Add(HttpMethod.GET, "/static/Lockwood/Features/IslandDevelopment/Hub/{build}/{country}/effects.xml", async (ctx) =>
            {
                ctx.Response.StatusCode = (int)HttpStatusCode.OK;
                ctx.Response.ContentType = "text/xml";
                string xmlPath = $"/static/Lockwood/Features/IslandDevelopment/Hub/{ctx.Request.Url.Parameters["build"]}/{ctx.Request.Url.Parameters["country"]}/effects.xml";
                string filePath = ApacheNetServerConfiguration.HTTPStaticFolder + xmlPath;

                if (File.Exists(filePath))
                {
                    await ctx.Response.Send(File.ReadAllText(filePath));
                    return;
                }

                LoggerAccessor.LogDebug($"[PostAuthParameters] - effects data was not found for IslandDevelopment scene:Hub, falling back to server file.");

                await ctx.Response.Send("<lua></lua>");
            });

            server.Routes.PostAuthentication.Parameter.Add(HttpMethod.GET, "/static/Lockwood/Features/IslandDevelopment/{sceneIdent}/{scenetype}/{build}/{country}/effects.xml", async (ctx) =>
            {
                ctx.Response.StatusCode = (int)HttpStatusCode.OK;
                ctx.Response.ContentType = "text/xml";
                string xmlPath = $"/static/Lockwood/Features/IslandDevelopment/{ctx.Request.Url.Parameters["sceneIdent"]}/{ctx.Request.Url.Parameters["scenetype"]}/{ctx.Request.Url.Parameters["build"]}/{ctx.Request.Url.Parameters["country"]}/effects.xml";
                string filePath = ApacheNetServerConfiguration.HTTPStaticFolder + xmlPath;

                if (File.Exists(filePath))
                {
                    await ctx.Response.Send(File.ReadAllText(filePath));
                    return;
                }

                LoggerAccessor.LogDebug($"[PostAuthParameters] - effects data was not found for IslandDevelopment scene:{ctx.Request.Url.Parameters["sceneIdent"]}, falling back to server file.");

                await ctx.Response.Send("<lua></lua>");
            });

            server.Routes.PostAuthentication.Parameter.Add(HttpMethod.GET, "/static/Lockwood/Features/IslandDevelopment/Hub/{build}/{country}/{group_def}/{profile_def}", async (ctx) =>
            {
                string? group_def = ctx.Request.Url.Parameters["group_def"];
                string? profile_def = ctx.Request.Url.Parameters["profile_def"];
                if (string.IsNullOrEmpty(group_def) || string.IsNullOrEmpty(profile_def))
                {
                    ctx.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    ctx.Response.ContentType = "text/plain";
                    await ctx.Response.Send();
                    return;
                }
                const string param_str = ".param_group";
                if (!profile_def.EndsWith(param_str))
                {
                    LoggerAccessor.LogWarn($"[PostAuthParameters] - profile_def definition path was invalid!");
                    ctx.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    ctx.Response.ContentType = "text/plain";
                    await ctx.Response.Send();
                    return;
                }
                string xmlPath = $"/static/Lockwood/Features/IslandDevelopment/Hub/{ctx.Request.Url.Parameters["build"]}/{ctx.Request.Url.Parameters["country"]}/{group_def}/{profile_def}";
                profile_def = profile_def.Substring(0, profile_def.Length - param_str.Length);
                if (string.IsNullOrEmpty(profile_def))
                {
                    LoggerAccessor.LogWarn($"[PostAuthParameters] - profile_def definition project was invalid!");
                    ctx.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    ctx.Response.ContentType = "text/plain";
                    await ctx.Response.Send();
                    return;
                }
                ctx.Response.StatusCode = (int)HttpStatusCode.OK;
                ctx.Response.ContentType = "text/xml";
                string filePath = ApacheNetServerConfiguration.HTTPStaticFolder + xmlPath;

                if (File.Exists(filePath))
                {
                    await ctx.Response.Send(File.ReadAllText(filePath));
                    return;
                }

                LoggerAccessor.LogDebug($"[PostAuthParameters] - {profile_def} data was not found for IslandDevelopment scene:Hub, falling back to server file.");

                switch (group_def)
                {
                    case "Gate":
                        await ctx.Response.Send($@"<lua>
	                                    <feature_com_context type='num'>{UniqueIDCounter.CreateUniqueID()}</feature_com_context>
                                        <applet>
		                                    <UUIDGate_def></UUIDGate_def>
	                                    </applet>
                                        <detectors>
		                                    <detectorsGate_def></detectorsGate_def>
	                                    </detectors>
	                                    <feature_root>
		                                    <root_0>
			                                    <rot type='vec'>0,0,0,0</rot>
                                                <scale type='vec'>0,0,0,0</scale>
                                                <pos type='vec'>-28.084,0.288,-1.781,0</pos>
                                                <applet>
			                                        <name>UUIDGate_def</name>
                                                    <override>
                                                        <appletId>UUIDBlocker_Applet</appletId>
                                                        <register>AppletRegister_UUIDBlocker.lua</register>
                                                        <params>
                                                            <commerceUrl>generic_cp</commerceUrl>
                                                            <entry>DreamHideawayEntitlements</entry>
                                                        </params>
                                                    </override>
		                                        </applet>
                                                <detectors>
			                                        <name>detectorsGate_def</name>
                                                    <override>
                                                        <proximity>1.8</proximity>
                                                        <homeTarget>
                                                            <localisation>ENTER</localisation>
                                                            <radius type='num'>1.8</radius>
                                                        </homeTarget>
                                                    </override>
		                                        </detectors>
		                                    </root_0>
                                            <root_1>
			                                    <rot type='vec'>0,0,0,0</rot>
                                                <scale type='vec'>0,0,0,0</scale>
                                                <pos type='vec'>-19.238,0.498,-8.339,0</pos>
                                                <applet>
			                                        <name>UUIDGate_def</name>
                                                    <override>
                                                        <appletId>UUIDBlocker_Applet</appletId>
                                                        <register>AppletRegister_UUIDBlocker.lua</register>
                                                        <params>
                                                            <commerceUrl>generic_cp</commerceUrl>
                                                            <entry>DreamYachtArcticEntitlements</entry>
                                                        </params>
                                                    </override>
		                                        </applet>
                                                <detectors>
			                                        <name>detectorsGate_def</name>
                                                    <override>
                                                        <proximity>1.8</proximity>
                                                        <homeTarget>
                                                            <localisation>ENTER</localisation>
                                                            <radius type='num'>1.8</radius>
                                                        </homeTarget>
                                                    </override>
		                                        </detectors>
		                                    </root_1>
                                            <root_2>
			                                    <rot type='vec'>0,0,0,0</rot>
                                                <scale type='vec'>0,0,0,0</scale>
                                                <pos type='vec'>-3.312,0.475,-12.799,0</pos>
                                                <applet>
			                                        <name>UUIDGate_def</name>
                                                    <override>
                                                        <appletId>UUIDBlocker_Applet</appletId>
                                                        <register>AppletRegister_UUIDBlocker.lua</register>
                                                        <params>
                                                            <commerceUrl>generic_cp</commerceUrl>
                                                            <entry>DreamYachtEntitlements</entry>
                                                        </params>
                                                    </override>
		                                        </applet>
                                                <detectors>
			                                        <name>detectorsGate_def</name>
                                                    <override>
                                                        <proximity>1.8</proximity>
                                                        <homeTarget>
                                                            <localisation>ENTER</localisation>
                                                            <radius type='num'>1.8</radius>
                                                        </homeTarget>
                                                    </override>
		                                        </detectors>
		                                    </root_2>
                                            <root_3>
			                                    <rot type='vec'>0,0,0,0</rot>
                                                <scale type='vec'>0,0,0,0</scale>
                                                <pos type='vec'>12.603,0.345,-7.121,0</pos>
                                                <applet>
			                                        <name>UUIDGate_def</name>
                                                    <override>
                                                        <appletId>UUIDBlocker_Applet</appletId>
                                                        <register>AppletRegister_UUIDBlocker.lua</register>
                                                        <params>
                                                            <commerceUrl>generic_cp</commerceUrl>
                                                            <entry>DreamForestEntitlements</entry>
                                                        </params>
                                                    </override>
		                                        </applet>
                                                <detectors>
			                                        <name>detectorsGate_def</name>
                                                    <override>
                                                        <proximity>1.8</proximity>
                                                        <homeTarget>
                                                            <localisation>ENTER</localisation>
                                                            <radius type='num'>1.8</radius>
                                                        </homeTarget>
                                                    </override>
		                                        </detectors>
		                                    </root_3>
                                            <root_4>
			                                    <rot type='vec'>0,0,0,0</rot>
                                                <scale type='vec'>0,0,0,0</scale>
                                                <pos type='vec'>14.815,0.345,9.345,0</pos>
                                                <applet>
			                                        <name>UUIDGate_def</name>
                                                    <override>
                                                        <appletId>UUIDBlocker_Applet</appletId>
                                                        <register>AppletRegister_UUIDBlocker.lua</register>
                                                        <params>
                                                            <commerceUrl>generic_cp</commerceUrl>
                                                            <entry>DreamIslandEntitlements</entry>
                                                        </params>
                                                    </override>
		                                        </applet>
                                                <detectors>
			                                        <name>detectorsGate_def</name>
                                                    <override>
                                                        <proximity>1.8</proximity>
                                                        <homeTarget>
                                                            <localisation>ENTER</localisation>
                                                            <radius type='num'>1.8</radius>
                                                        </homeTarget>
                                                    </override>
		                                        </detectors>
		                                    </root_4>
	                                    </feature_root>
                                    </lua>");
                        return;
                    case "commercePoints":
                        await ctx.Response.Send($@"<lua>
	                                    <feature_com_context type='num'>{UniqueIDCounter.CreateUniqueID()}</feature_com_context>
                                        <applet>
		                                    <CommercePoint0_def></CommercePoint0_def>
		                                    <CommercePoint1_def></CommercePoint1_def>
		                                    <CommercePoint2_def></CommercePoint2_def>
		                                    <CommercePoint3_def></CommercePoint3_def>
		                                    <CommercePoint4_def></CommercePoint4_def>
		                                    <CommercePoint5_def></CommercePoint5_def>
	                                    </applet>
                                        <detectors>
		                                    <detectorsCommercePoint0_def></detectorsCommercePoint0_def>
		                                    <detectorsCommercePoint1_def></detectorsCommercePoint1_def>
		                                    <detectorsCommercePoint2_def></detectorsCommercePoint2_def>
		                                    <detectorsCommercePoint3_def></detectorsCommercePoint3_def>
		                                    <detectorsCommercePoint4_def></detectorsCommercePoint4_def>
		                                    <detectorsCommercePoint5_def></detectorsCommercePoint5_def>
	                                    </detectors>
	                                    <feature_root>
		                                    <root_0>
			                                    <rot type='vec'>0,0,0,0</rot>
                                                <scale type='vec'>0,0,0,0</scale>
                                                <pos type='vec'>-24.965,-0.343,-2.876,0</pos>
                                                <applet>
			                                        <name>CommercePoint0_def</name>
                                                    <override>
                                                        <appletId>CommercePoint_Applet</appletId>
                                                        <register>AppletRegister_commercePoint.lua</register>
                                                        <params>
                                                            <url>generic_cp</url>
                                                        </params>
                                                    </override>
		                                        </applet>
                                                <detectors>
			                                        <name>detectorsCommercePoint0_def</name>
                                                    <override>
                                                        <proximity>1.5</proximity>
                                                        <homeTarget>
                                                            <localisation>COMMERCE_POINT</localisation>
                                                            <radius type='num'>1.5</radius>
                                                        </homeTarget>
                                                    </override>
		                                        </detectors>
		                                    </root_0>
                                            <root_1>
			                                    <rot type='vec'>0,0,0,0</rot>
                                                <scale type='vec'>0,0,0,0</scale>
                                                <pos type='vec'>-15.722,-0.220,-6.339,0</pos>
                                                <applet>
			                                        <name>CommercePoint1_def</name>
                                                    <override>
                                                        <appletId>CommercePoint_Applet</appletId>
                                                        <register>AppletRegister_commercePoint.lua</register>
                                                        <params>
                                                            <url>generic_cp</url>
                                                        </params>
                                                    </override>
		                                        </applet>
                                                <detectors>
			                                        <name>detectorsCommercePoint1_def</name>
                                                    <override>
                                                        <proximity>1.5</proximity>
                                                        <homeTarget>
                                                            <localisation>COMMERCE_POINT</localisation>
                                                            <radius type='num'>1.5</radius>
                                                        </homeTarget>
                                                    </override>
		                                        </detectors>
		                                    </root_1>
                                            <root_2>
			                                    <rot type='vec'>0,0,0,0</rot>
                                                <scale type='vec'>0,0,0,0</scale>
                                                <pos type='vec'>-1.522,-0.000,-10.033,0</pos>
                                                <applet>
			                                        <name>CommercePoint2_def</name>
                                                    <override>
                                                        <appletId>CommercePoint_Applet</appletId>
                                                        <register>AppletRegister_commercePoint.lua</register>
                                                        <params>
                                                            <url>generic_cp</url>
                                                        </params>
                                                    </override>
		                                        </applet>
                                                <detectors>
			                                        <name>detectorsCommercePoint2_def</name>
                                                    <override>
                                                        <proximity>1.5</proximity>
                                                        <homeTarget>
                                                            <localisation>COMMERCE_POINT</localisation>
                                                            <radius type='num'>1.5</radius>
                                                        </homeTarget>
                                                    </override>
		                                        </detectors>
		                                    </root_2>
                                            <root_3>
			                                    <rot type='vec'>0,0,0,0</rot>
                                                <scale type='vec'>0,0,0,0</scale>
                                                <pos type='vec'>9.425,-0.117,-6.281,0</pos>
                                                <applet>
			                                        <name>CommercePoint3_def</name>
                                                    <override>
                                                        <appletId>CommercePoint_Applet</appletId>
                                                        <register>AppletRegister_commercePoint.lua</register>
                                                        <params>
                                                            <url>generic_cp</url>
                                                        </params>
                                                    </override>
		                                        </applet>
                                                <detectors>
			                                        <name>detectorsCommercePoint3_def</name>
                                                    <override>
                                                        <proximity>1.5</proximity>
                                                        <homeTarget>
                                                            <localisation>COMMERCE_POINT</localisation>
                                                            <radius type='num'>1.5</radius>
                                                        </homeTarget>
                                                    </override>
		                                        </detectors>
		                                    </root_3>
                                            <root_4>
			                                    <rot type='vec'>0,0,0,0</rot>
                                                <scale type='vec'>0,0,0,0</scale>
                                                <pos type='vec'>11.891,-0.253,8.002,0</pos>
                                                <applet>
			                                        <name>CommercePoint4_def</name>
                                                    <override>
                                                        <appletId>CommercePoint_Applet</appletId>
                                                        <register>AppletRegister_commercePoint.lua</register>
                                                        <params>
                                                            <url>generic_cp</url>
                                                        </params>
                                                    </override>
		                                        </applet>
                                                <detectors>
			                                        <name>detectorsCommercePoint4_def</name>
                                                    <override>
                                                        <proximity>1.5</proximity>
                                                        <homeTarget>
                                                            <localisation>COMMERCE_POINT</localisation>
                                                            <radius type='num'>1.5</radius>
                                                        </homeTarget>
                                                    </override>
		                                        </detectors>
		                                    </root_4>
	                                    </feature_root>
                                    </lua>");
                        return;
                    case "Twitter":
                        await ctx.Response.Send($@"<lua>
	                                    <feature_com_context type='num'>{UniqueIDCounter.CreateUniqueID()}</feature_com_context>
                                        <applet>
		                                    <Twitter_def></Twitter_def>
	                                    </applet>
	                                    <feature_root>
		                                    <root>
			                                    <rot type='vec'>0,-27,0,-27</rot>
                                                <scale type='vec'>1.7,1.7,1.6,1.6</scale>
                                                <pos type='vec'>5.645,2.632,-8.286,0</pos>
                                                <applet>
			                                        <name>Twitter_def</name>
                                                    <override>
                                                        <appletId>Twitter_Applet</appletId>
                                                        <register>AppletRegister_Twitter.lua</register>
                                                        <params>
                                                            <account>{profile_def}</account>
                                                        </params>
                                                    </override>
		                                        </applet>
		                                    </root>
	                                    </feature_root>
                                    </lua>");
                        return;
                    default:
                        LoggerAccessor.LogWarn($"[PostAuthParameters] - group_def definition data was not found for group_def:{group_def}!");
                        break;
                }

                await ctx.Response.Send("<lua></lua>");
            });

            server.Routes.PostAuthentication.Parameter.Add(HttpMethod.GET, "/static/Lockwood/Features/IslandDevelopment/{sceneIdent}/{scenetype}/{build}/{country}/{group_def}/{profile_def}", async (ctx) =>
            {
                string? group_def = ctx.Request.Url.Parameters["group_def"];
                string? profile_def = ctx.Request.Url.Parameters["profile_def"];
                string? sceneIdent = ctx.Request.Url.Parameters["sceneIdent"];
                if (string.IsNullOrEmpty(sceneIdent) || string.IsNullOrEmpty(group_def) || string.IsNullOrEmpty(profile_def))
                {
                    ctx.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    ctx.Response.ContentType = "text/plain";
                    await ctx.Response.Send();
                    return;
                }
                const string param_str = ".param_group";
                if (!profile_def.EndsWith(param_str))
                {
                    LoggerAccessor.LogWarn($"[PostAuthParameters] - profile_def definition path was invalid!");
                    ctx.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    ctx.Response.ContentType = "text/plain";
                    await ctx.Response.Send();
                    return;
                }
                string xmlPath = $"/static/Lockwood/Features/IslandDevelopment/{sceneIdent}/{ctx.Request.Url.Parameters["scenetype"]}/{ctx.Request.Url.Parameters["build"]}/{ctx.Request.Url.Parameters["country"]}/{group_def}/{profile_def}";
                profile_def = profile_def.Substring(0, profile_def.Length - param_str.Length);
                if (string.IsNullOrEmpty(profile_def))
                {
                    LoggerAccessor.LogWarn($"[PostAuthParameters] - profile_def definition project was invalid!");
                    ctx.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    ctx.Response.ContentType = "text/plain";
                    await ctx.Response.Send();
                    return;
                }
                ctx.Response.StatusCode = (int)HttpStatusCode.OK;
                ctx.Response.ContentType = "text/xml";
                string filePath = ApacheNetServerConfiguration.HTTPStaticFolder + xmlPath;

                if (File.Exists(filePath))
                {
                    await ctx.Response.Send(File.ReadAllText(filePath));
                    return;
                }

                LoggerAccessor.LogDebug($"[PostAuthParameters] - {profile_def} data was not found for IslandDevelopment scene:{sceneIdent}, falling back to server file.");

                switch (sceneIdent)
                {
                    case "Hub":
                    case "Lounge":
                    case "Beach":
                    case "Hideaway":
                    case "Forest":
                    case "Yacht":
                    case "IceYacht":
                        switch (group_def)
                        {
                            case "Telescope":
                                if (sceneIdent == "Beach")
                                {
                                    await ctx.Response.Send($@"<lua>
	                                    <feature_com_context type='num'>{UniqueIDCounter.CreateUniqueID()}</feature_com_context>
                                        <applet>
		                                    <Telescope_def></Telescope_def>
	                                    </applet>
                                        <entityGroup>
		                                    <EntityGroup_def></EntityGroup_def>
	                                    </entityGroup>
                                        <detectors>
		                                    <detectorsTelescope_def></detectorsTelescope_def>
	                                    </detectors>
	                                    <feature_root>
                                            <root>
			                                    <rot type='vec'>0,-100,0,-100</rot>
                                                <scale type='vec'>0.1,0.1,0.1,0.1</scale>
                                                <pos type='vec'>-67.666,3.989,-28.205,0</pos>
                                                <applet>
			                                        <name>Telescope_def</name>
                                                    <override>
                                                        <appletId>Telescope_Applet</appletId>
                                                        <register>AppletRegister_telescope.lua</register>
                                                        <params>
                                                            <lensOffset type='vec'>-67.666,3.989,-28.205,0</lensOffset>
                                                        </params>
                                                    </override>
		                                        </applet>
                                                <detectors>
			                                        <name>detectorsTelescope_def</name>
                                                    <override>
                                                        <proximity>5</proximity>
                                                        <homeTarget>
                                                            <localisation>TELESCOPE</localisation>
                                                            <radius type='num'>4.5</radius>
                                                        </homeTarget>
                                                    </override>
		                                        </detectors>
		                                    </root>
                                            <root_telescope>
			                                    <rot type='vec'>0,-190,0,-190</rot>
                                                <scale type='vec'>1,1,1,1</scale>
                                                <pos type='vec'>-67.069,2.211,-27.122,0</pos>
                                                <entityGroup>
			                                        <name>EntityGroup_def</name>
                                                    <override>
                                                        <entities>
                                                            <_>
                                                                <mdl>Telescope.mdl</mdl>
                                                                <hkx>Telescope.hkx</hkx>
                                                            </_>
                                                        </entities>
                                                    </override>
		                                        </entityGroup>
		                                    </root_telescope>
	                                    </feature_root>
                                    </lua>");
                                    return;
                                }
                                break;
                            case "commercePoints":
                                if (sceneIdent == "Beach")
                                {
                                    await ctx.Response.Send($@"<lua>
	                                    <feature_com_context type='num'>{UniqueIDCounter.CreateUniqueID()}</feature_com_context>
                                        <applet>
		                                    <CommercePointLux_def></CommercePointLux_def>
		                                    <CommercePoint_def></CommercePoint_def>
	                                    </applet>
                                        <detectors>
		                                    <detectorsCommercePointLux_def></detectorsCommercePointLux_def>
		                                    <detectorsCommercePoint_def></detectorsCommercePoint_def>
	                                    </detectors>
	                                    <feature_root>
		                                    <root_lux>
			                                    <rot type='vec'>0,0,0,0</rot>
                                                <scale type='vec'>0,0,0,0</scale>
                                                <pos type='vec'>-0.748,1.956,-41.419,0</pos>
                                                <applet>
			                                        <name>CommercePointLux_def</name>
                                                    <override>
                                                        <appletId>CommercePoint_Applet</appletId>
                                                        <register>AppletRegister_commercePoint.lua</register>
                                                        <params>
                                                            <url>exclusive_cp</url>
                                                        </params>
                                                    </override>
		                                        </applet>
                                                <detectors>
			                                        <name>detectorsCommercePointLux_def</name>
                                                    <override>
                                                        <proximity>1.5</proximity>
                                                        <homeTarget>
                                                            <localisation>COMMERCE_POINT</localisation>
                                                            <radius type='num'>1.5</radius>
                                                        </homeTarget>
                                                    </override>
		                                        </detectors>
		                                    </root_lux>
                                            <root>
			                                    <rot type='vec'>0,0,0,0</rot>
                                                <scale type='vec'>0,0,0,0</scale>
                                                <pos type='vec'>-3.458,1.956,-46.998,0</pos>
                                                <applet>
			                                        <name>CommercePoint_def</name>
                                                    <override>
                                                        <appletId>CommercePoint_Applet</appletId>
                                                        <register>AppletRegister_commercePoint.lua</register>
                                                        <params>
                                                            <url>generic_cp</url>
                                                        </params>
                                                    </override>
		                                        </applet>
                                                <detectors>
			                                        <name>detectorsCommercePoint_def</name>
                                                    <override>
                                                        <proximity>1.5</proximity>
                                                        <homeTarget>
                                                            <localisation>COMMERCE_POINT</localisation>
                                                            <radius type='num'>1.5</radius>
                                                        </homeTarget>
                                                    </override>
		                                        </detectors>
		                                    </root>
	                                    </feature_root>
                                    </lua>");
                                    return;
                                }
                                break;
                            case "Posertrons":
                                if (sceneIdent == "Beach")
                                {
                                    await ctx.Response.Send($@"<lua>
	                                    <feature_com_context type='num'>{UniqueIDCounter.CreateUniqueID()}</feature_com_context>
                                        <applet>
		                                    <Posertrons_V2></Posertrons_V2>
	                                    </applet>
	                                    <feature_root>
		                                    <root_lux_commerce>
			                                    <rot type='vec'>0,20,0,20</rot>
                                                <scale type='vec'>1,1,1,1</scale>
                                                <pos type='vec'>-0.748,1.956,-41.419,0</pos>
                                                <applet>
			                                        <name>Posertrons_V2</name>
                                                    <override>
                                                        <appletId>Posertron_Applet</appletId>
                                                        <register>AppletRegister_posertron.lua</register>
                                                        <params>
                                                            <model>
                                                                <mdl>buybagExclusive.mdl</mdl>
                                                                <skn>buybagExclusive.skn</skn>
                                                                <ani>buybagExclusive.ani</ani>
                                                                <hkx>buybag.hkx</hkx>
                                                            </model>
                                                        </params>
                                                    </override>
		                                        </applet>
		                                    </root_lux_commerce>
                                            <root_commerce>
			                                    <rot type='vec'>0,20,0,20</rot>
                                                <scale type='vec'>1,1,1,1</scale>
                                                <pos type='vec'>-3.458,1.956,-46.998,0</pos>
                                                <applet>
			                                        <name>Posertrons_V2</name>
                                                    <override>
                                                        <appletId>Posertron_Applet</appletId>
                                                        <register>AppletRegister_posertron.lua</register>
                                                        <params>
                                                            <model>
                                                                <mdl>buybag.mdl</mdl>
                                                                <skn>buybag.skn</skn>
                                                                <ani>buybag.ani</ani>
                                                                <hkx>buybag.hkx</hkx>
                                                            </model>
                                                        </params>
                                                    </override>
		                                        </applet>
		                                    </root_commerce>
	                                    </feature_root>
                                    </lua>");
                                    return;
                                }
                                break;
                            case "Gate":
                                if (sceneIdent == "Lounge")
                                {
                                    await ctx.Response.Send($@"<lua>
	                                    <feature_com_context type='num'>{UniqueIDCounter.CreateUniqueID()}</feature_com_context>
                                        <applet>
		                                    <UUIDGate_def></UUIDGate_def>
	                                    </applet>
                                        <entityGroup>
		                                    <EntityGroup_def></EntityGroup_def>
	                                    </entityGroup>
                                        <detectors>
		                                    <detectorsGate_def></detectorsGate_def>
	                                    </detectors>
	                                    <feature_root>
		                                    <root>
			                                    <rot type='vec'>0,0,0,0</rot>
                                                <scale type='vec'>0,0,0,0</scale>
                                                <pos type='vec'>6.910,-0.2,23.100,0</pos>
                                                <applet>
			                                        <name>UUIDGate_def</name>
                                                    <override>
                                                        <appletId>UUIDGate_Applet</appletId>
                                                        <register>AppletRegister_UUIDGate.lua</register>
                                                        <params>
                                                            <commerceUrl>generic_cp</commerceUrl>
                                                            <entry>DreamApartmentEntitlements</entry>
                                                            <destination>gate_dest</destination>
                                                        </params>
                                                    </override>
		                                        </applet>
                                                <detectors>
			                                        <name>detectorsGate_def</name>
                                                    <override>
                                                        <proximity>1.8</proximity>
                                                        <homeTarget>
                                                            <localisation>ENTER</localisation>
                                                            <radius type='num'>1.8</radius>
                                                        </homeTarget>
                                                    </override>
		                                        </detectors>
		                                    </root>
                                            <root_gate>
			                                    <rot type='vec'>0,15,0,15</rot>
                                                <scale type='vec'>1,1,1,1</scale>
                                                <pos type='vec'>6.910,-0.2,22.800,0</pos>
                                                <entityGroup>
			                                        <name>EntityGroup_def</name>
                                                    <override>
                                                        <entities>
                                                            <_>
                                                                <mdl>lock.mdl</mdl>
                                                                <skn>lock.skn</skn>
                                                                <ani>lock.ani</ani>
                                                            </_>
                                                        </entities>
                                                    </override>
		                                        </entityGroup>
		                                    </root_gate>
                                            <gate_dest>
			                                    <rot type='vec'>0,0,0,0</rot>
                                                <scale type='vec'>0,0,0,0</scale>
                                                <pos type='vec'>6.752,0,21.779,0</pos>
		                                    </gate_dest>
	                                    </feature_root>
                                    </lua>");
                                    return;
                                }
                                break;
                            case "Splashertron":
                                if (sceneIdent == "Hideaway")
                                {
                                    await ctx.Response.Send($@"<lua>
	                                    <feature_com_context type='num'>{UniqueIDCounter.CreateUniqueID()}</feature_com_context>
                                        <applet>
		                                    <Splashertron_def></Splashertron_def>
	                                    </applet>
                                        <detectors>
		                                    <detectorsSplash_def></detectorsSplash_def>
	                                    </detectors>
	                                    <feature_root>
		                                    <root>
			                                    <rot type='vec'>0,0,0,0</rot>
                                                <scale type='vec'>0,0,0,0</scale>
                                                <pos type='vec'>119.739,2.246,-140.552,0</pos>
                                                <applet>
			                                        <name>Splashertron_def</name>
                                                    <override>
                                                        <appletId>Splashertron_Applet</appletId>
                                                        <register>AppletRegister_splashertron.lua</register>
                                                        <params>
                                                            <collision>seabed.hkx</collision>
                                                            <waterLevel type='num'>1.2</waterLevel>
                                                            <destination>swim_dest</destination>
                                                        </params>
                                                    </override>
		                                        </applet>
                                                <detectors>
			                                        <name>detectorsSplash_def</name>
                                                    <override>
                                                        <proximity>10000</proximity>
                                                        <homeTarget>
                                                            <localisation>SWIM</localisation>
                                                            <radius type='num'>2</radius>
                                                        </homeTarget>
                                                    </override>
		                                        </detectors>
		                                    </root>
                                            <swim_dest>
			                                    <rot type='vec'>0,0,0,0</rot>
                                                <scale type='vec'>0,0,0,0</scale>
                                                <pos type='vec'>119.739,2.246,-136.120,0</pos>
		                                    </swim_dest>
	                                    </feature_root>
                                    </lua>");
                                    return;
                                }
                                else if (sceneIdent == "Forest")
                                {
                                    await ctx.Response.Send($@"<lua>
                                         <feature_com_context type='num'>{UniqueIDCounter.CreateUniqueID()}</feature_com_context>
                                         <applet>
                                             <Splashertron_def></Splashertron_def>
                                         </applet>
                                         <detectors>
                                             <detectorsSplash_def></detectorsSplash_def>
                                         </detectors>
                                         <feature_root>
                                             <root>
                                                 <rot type='vec'>0,0,0,0</rot>
                                                 <scale type='vec'>0,0,0,0</scale>
                                                 <pos type='vec'>-86.489,-1,-69.773,0</pos>
                                                 <applet>
                                                     <name>Splashertron_def</name>
                                                     <override>
                                                         <appletId>Splashertron_Applet</appletId>
                                                         <register>AppletRegister_splashertron.lua</register>
                                                         <params>
                                                             <collision>seabed.hkx</collision>
                                                             <waterLevel type='num'>-4.5</waterLevel>
                                                             <destination>swim_dest</destination>
                                                         </params>
                                                     </override>
                                                 </applet>
                                                 <detectors>
                                                     <name>detectorsSplash_def</name>
                                                     <override>
                                                         <proximity>10000</proximity>
                                                         <homeTarget>
                                                             <localisation>SWIM</localisation>
                                                             <radius type='num'>4</radius>
                                                         </homeTarget>
                                                     </override>
                                                 </detectors>
                                             </root>
                                             <swim_dest>
                                                 <rot type='vec'>0,0,0,0</rot>
                                                 <scale type='vec'>0,0,0,0</scale>
                                                 <pos type='vec'>-91,-3.285,-69.773,0</pos>
                                             </swim_dest>
                                         </feature_root>
                                     </lua>");
                                    return;
                                }
                                else if (sceneIdent == "Lounge")
                                {
                                    await ctx.Response.Send($@"<lua>
	                                    <feature_com_context type='num'>{UniqueIDCounter.CreateUniqueID()}</feature_com_context>
                                        <applet>
		                                    <Splashertron_def></Splashertron_def>
	                                    </applet>
	                                    <feature_root>
		                                    <root>
			                                    <rot type='vec'>0,0,0,0</rot>
                                                <scale type='vec'>0,0,0,0</scale>
                                                <pos type='vec'>0,0,0,0</pos>
                                                <applet>
			                                        <name>Splashertron_def</name>
                                                    <override>
                                                        <appletId>Splashertron_Applet</appletId>
                                                        <register>AppletRegister_splashertron.lua</register>
                                                        <params>
                                                            <collision>seabed.hkx</collision>
                                                            <waterLevel type='num'>-1</waterLevel>
                                                        </params>
                                                    </override>
		                                        </applet>
		                                    </root>
	                                    </feature_root>
                                    </lua>");
                                    return;
                                }
                                else if (sceneIdent == "Beach")
                                {
                                    await ctx.Response.Send($@"<lua>
	                                    <feature_com_context type='num'>{UniqueIDCounter.CreateUniqueID()}</feature_com_context>
                                        <applet>
		                                    <Splashertron_def></Splashertron_def>
	                                    </applet>
	                                    <feature_root>
		                                    <root>
			                                    <rot type='vec'>0,0,0,0</rot>
                                                <scale type='vec'>0,0,0,0</scale>
                                                <pos type='vec'>0,0,0,0</pos>
                                                <applet>
			                                        <name>Splashertron_def</name>
                                                    <override>
                                                        <appletId>Splashertron_Applet</appletId>
                                                        <register>AppletRegister_splashertron.lua</register>
                                                        <params>
                                                            <collision>seabed.hkx</collision>
                                                            <waterLevel type='num'>1.3</waterLevel>
                                                        </params>
                                                    </override>
		                                        </applet>
		                                    </root>
	                                    </feature_root>
                                    </lua>");
                                    return;
                                }
                                break;
                            default:
                                LoggerAccessor.LogWarn($"[PostAuthParameters] - group_def definition data was not found for group_def:{group_def}!");
                                break;
                        }
                        break;
                    default:
                        LoggerAccessor.LogWarn($"[PostAuthParameters] - group_def definition data was not found for scene:{sceneIdent}!");
                        break;
                }
                await ctx.Response.Send("<lua></lua>");
            });
        }
    }
}
