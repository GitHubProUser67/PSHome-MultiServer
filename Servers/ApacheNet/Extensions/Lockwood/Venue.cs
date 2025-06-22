using CustomLogger;
using System;
using System.IO;
using System.Net;
using WatsonWebserver.Core;

namespace ApacheNet.Extensions.Lockwood
{
    internal static class Venue
    {
        private static UniqueIDGenerator UniqueIDCounter = new UniqueIDGenerator(0);

        public static void BuildVenuePlugin(WebserverBase server)
        {
            server.Routes.PostAuthentication.Parameter.Add(HttpMethod.GET, "/static/Lockwood/Features/Venue/{scenetype}/{build}/{country}/setDressing.xml", async (ctx) =>
            {
                ctx.Response.StatusCode = (int)HttpStatusCode.OK;
                ctx.Response.ContentType = "text/xml";
                string xmlPath = $"/static/Lockwood/Features/Venue/{ctx.Request.Url.Parameters["scenetype"]}/{ctx.Request.Url.Parameters["build"]}/{ctx.Request.Url.Parameters["country"]}/setDressing.xml";
                string filePath = !ApacheNetServerConfiguration.DomainFolder ? ApacheNetServerConfiguration.HTTPStaticFolder + xmlPath : ApacheNetServerConfiguration.HTTPStaticFolder + $"/{ctx.Request.RetrieveHeaderValue("Host")}" + xmlPath;

                if (File.Exists(filePath))
                {
                    await ctx.Response.Send(File.ReadAllText(filePath));
                    return;
                }

                LoggerAccessor.LogDebug($"[PostAuthParameters] - setDressing data was not found for the Venue, falling back to server file.");

                await ctx.Response.Send(WebAPIService.OHS.LUA2XmlProcessor.TransformLuaTableToXml(@"
                    local maps = {
	                    'colourMap', 'normalMap', 'specularMap', 'envMap', 'emissiveMap', 'colour2Map', 'normal2Map', 'specular2Map'
                    }

                    setDressing = {
		                profiles = {
                            commercePoints = {},
                            Logos = {
                                { profile = 'cloverleaf', icon = 'clover.dds' }, { profile = 'drey', icon = 'drey.dds' },
                                { profile = 'delirious_squid', icon = 'dsquid.dds' }, { profile = 'figment', icon = 'figment.dds' },
                                { profile = 'fool_throttle', icon = 'foolthrottle.dds' }, { profile = 'heart', icon = 'heart.dds' },
                                { profile = 'iron_fusion', icon = 'ironfusion.dds' }, { profile = 'lkwd', icon = 'lkwd.dds' },
                                { profile = 'lkwd_venue', icon = 'dancefloor.dds' }, { profile = 'medusa', icon = 'medusa.dds' },
                                { profile = 'spiral', icon = 'spiral.dds' }, { profile = 'star', icon = 'star.dds' },
                                { profile = 'stitchkin', icon = 'stitchkin.dds' }, { profile = 'unicorn', icon = 'unicorn.dds' },
                                { profile = 'halloween', icon = 'logos.dds' }
                            },
                            Stagertron = {},
			                Runwaytron = { { profile = 'default', icon = 'catwalk.dds' }, { profile = 'stage', icon = 'stage.dds' }, { profile = 'dancefloor', icon = 'venue.dds' } },
			                Customisation = {},
			                Posertrons = {},
			                BackstagePass = {},
                            Music = {
                                { profile = 'Corporate', icon = 'song_00.dds' }, { profile = 'Delirious_Squid', icon = 'song_01.dds' },
                                { profile = 'Drey', icon = 'song_02.dds' }, { profile = 'Fool_Throttle', icon = 'song_03.dds' },
                                { profile = 'IronFusion', icon = 'song_04.dds' }, { profile = 'LKWD', icon = 'song_05.dds' },
                                { profile = 'Medusa', icon = 'song_06.dds' }, { profile = 'Nightclub', icon = 'song_07.dds' },
                                { profile = 'Wings', icon = 'song_08.dds' }, { profile = 'Stitchkins', icon = 'song_09.dds' },
                                { profile = 'default', icon = 'song_no.dds' }
                            }
		                },
		                entities = {
                            ['main_scene'] = {
                                ['Lobby_Gate_Internal']        = {
                                    { colour = '0.350,0.350,0.350,1.000', envMap = 'placeholder_d.dds', colourMap = 'Metal_01_d.dds', normalMap = 'Metal_01_n.dds', specularMap = 'Metal_01_s.dds' },
                                    { colour = '1.000,1.000,1.000,1.000', envMap = 'placeholder_d.dds', colourMap = 'Plaster_01_d.dds', normalMap = 'Plaster_01_n.dds', specularMap = 'Plaster_01_s.dds' },
                                    { colour = '0.800,1.000,0.800,1.000', envMap = 'placeholder_d.dds', colourMap = 'Plaster_01_d.dds', normalMap = 'Plaster_01_n.dds', specularMap = 'Plaster_01_s.dds' },
                                    { colour = '0.400,0.400,0.400,1.000', envMap = 'placeholder_d.dds', colourMap = 'wallpaper_06_d.dds', normalMap = 'wallpaper_06_n.dds', specularMap = 'wallpaper_06_s.dds' },
                                    { colour = '0.600,0.600,0.600,1.000', envMap = 'placeholder_d.dds', colourMap = 'wallpaper_07_d.dds', normalMap = 'wallpaper_07_n.dds', specularMap = 'wallpaper_07_s.dds' },
                                    { colour = '1.000,1.000,1.000,1.000', envMap = 'placeholder_d.dds', colourMap = 'Wood_02_d.dds', normalMap = 'Wood_02_n.dds', specularMap = 'Wood_02_s.dds' },
                                    { colour = '0.745,0.745,0.443,1.000', envMap = 'placeholder_d.dds', colourMap = 'Plaster_01_d.dds', normalMap = 'Plaster_01_n.dds', specularMap = 'Plaster_01_s.dds' },
                                    { colour = '0.500,0.500,0.500,1.000', envMap = 'placeholder_d.dds', colourMap = 'Plaster_01_d.dds', normalMap = 'Plaster_01_n.dds', specularMap = 'Plaster_01_s.dds' },
                                    { colour = '0.804,0.498,0.247,1.000', envMap = 'placeholder_d.dds', colourMap = 'Plaster_01_d.dds', normalMap = 'Plaster_01_n.dds', specularMap = 'Plaster_01_s.dds' },
                                    { colour = '1.000,0.992,0.816,1.000', envMap = 'placeholder_d.dds', colourMap = 'Plaster_01_d.dds', normalMap = 'Plaster_01_n.dds', specularMap = 'Plaster_01_s.dds' },
                                },
                                ['Lobby_Gate_Base']            = {
                                    { colour = '0.200,0.200,0.200,1.000', envMap = 'placeholder_d.dds', colourMap = 'circuit2_d.dds', normalMap = 'Plaster_01_n.dds', specularMap = 'circuit2_s.dds' },
                                    { colour = '0.800,0.800,0.800,1.000', envMap = 'placeholder_d.dds', colourMap = 'Plaster_01_d.dds', normalMap = 'Plaster_01_n.dds', specularMap = 'Plaster_01_s.dds' },
                                    { colour = '1.000,1.000,1.000,1.000', envMap = 'placeholder_d.dds', colourMap = 'Marble_02_d.dds', normalMap = 'Marble_02_n.dds', specularMap = 'Marble_02_s.dds' },
                                    { colour = '0.400,0.400,0.400,1.000', envMap = 'placeholder_d.dds', colourMap = 'Plaster_01_d.dds', normalMap = 'Plaster_01_n.dds', specularMap = 'Plaster_01_s.dds' },
                                    { colour = '0.502,0.000,0.000,1.000', envMap = 'placeholder_d.dds', colourMap = 'Panel_04_d.dds', normalMap = 'Panel_04_n.dds', specularMap = 'Panel_04_s.dds' },
                                    { colour = '1.000,1.000,1.000,1.000', envMap = 'placeholder_d.dds', colourMap = 'Plaster_01_d.dds', normalMap = 'Plaster_01_n.dds', specularMap = 'Plaster_01_s.dds' },
                                    { colour = '0.502,0.000,0.000,1.000', envMap = 'placeholder_d.dds', colourMap = 'Plaster_01_d.dds', normalMap = 'Plaster_01_n.dds', specularMap = 'Plaster_01_s.dds' },
                                    { colour = '0.000,0.052,0.082,1.000', envMap = 'placeholder_d.dds', colourMap = 'Plaster_01_d.dds', normalMap = 'Plaster_01_n.dds', specularMap = 'Plaster_01_s.dds' },
                                    { colour = '0.000,0.000,0.010,1.000', envMap = 'placeholder_d.dds', colourMap = 'Plaster_01_d.dds', normalMap = 'Plaster_01_n.dds', specularMap = 'Plaster_01_s.dds' },
                                    { colour = '1.000,1.000,0.000,1.000', envMap = 'placeholder_d.dds', colourMap = 'Plaster_01_d.dds', normalMap = 'Plaster_01_n.dds', specularMap = 'Plaster_01_s.dds' },
                                },
                                ['Lobby_Main_Walls']           = {
                                    { colour = '0.000,0.147,0.341,1.000', colourMap = 'circuit2_d.dds', normalMap = 'Plaster_01_n.dds', specularMap = 'circuit2_s.dds' },
                                    { colour = '1.000,1.000,1.000,1.000', colourMap = 'Panel_01_d.dds', normalMap = 'Panel_01_n.dds', specularMap = 'Panel_01_s.dds' },
                                    { colour = '1.000,1.000,1.000,1.000', colourMap = 'Wood_02_d.dds', normalMap = 'Wood_02_n.dds', specularMap = 'Wood_02_s.dds' },
                                    { colour = '0.600,0.400,0.800,1.000', colourMap = 'Wallpaper_02_d.dds', normalMap = 'Wallpaper_02_n.dds', specularMap = 'Wallpaper_02_s.dds' },
                                    { colour = '0.863,0.078,0.235,1.000', colourMap = 'mosiac_1_d.dds', normalMap = 'Plaster_01_n.dds', specularMap = 'mosiac_1_s.dds' },
                                    { colour = '1.000,0.900,0.900,1.000', colourMap = 'Panel_04_d.dds', normalMap = 'Panel_04_n.dds', specularMap = 'Panel_04_s.dds' },
                                    { colour = '0.000,0.012,0.082,1.000', colourMap = 'Plaster_01_d.dds', normalMap = 'Plaster_01_n.dds', specularMap = 'Plaster_01_s.dds' },
                                    { colour = '1.000,1.000,1.000,1.000', colourMap = 'panel_05_d.dds', normalMap = 'panel_05_n.dds', specularMap = 'panel_05_s.dds' },
                                    { colour = '1.000,0.653,0.696,1.000', colourMap = 'squid1_d.dds', normalMap = 'Plaster_01_n.dds', specularMap = 'squid1_s.dds' },
                                    { colour = '0.255,0.412,0.882,1.000', colourMap = 'stitchkin4_d.dds', normalMap = 'Plaster_01_n.dds', specularMap = 'stitchkin4_s.dds' },
                                },
                                ['Lobby_Short_Walls']          = {
                                    { colour = '0.000,0.150,1.000,1.000', colourMap = 'Metal_01_d.dds', normalMap = 'Metal_01_n.dds', specularMap = 'Metal_01_s.dds' },
                                    { colour = '1.000,1.000,1.000,1.000', colourMap = 'Panel_01_d.dds', normalMap = 'Panel_01_n.dds', specularMap = 'Panel_01_s.dds' },
                                    { colour = '0.000,0.247,0.341,1.000', colourMap = 'snakeskin_d.dds', normalMap = 'placeholder_n.dds', specularMap = 'snakeskin_s.dds' },
                                    { colour = '0.859,0.647,0.129,1.000', colourMap = 'Plaster_01_d.dds', normalMap = 'Plaster_01_n.dds', specularMap = 'Plaster_01_s.dds' },
                                    { colour = '0.600,0.600,0.600,1.000', colourMap = 'Panel_02_d.dds', normalMap = 'Panel_02_n.dds', specularMap = 'Panel_02_s.dds' },
                                    { colour = '0.800,0.800,0.800,1.000', colourMap = 'circles_1_d.dds', normalMap = 'placeholder_n.dds', specularMap = 'circles_1_s.dds' },
                                    { colour = '0.863,0.078,0.235,1.000', colourMap = 'Plaster_01_d.dds', normalMap = 'Plaster_01_n.dds', specularMap = 'Plaster_01_s.dds' },
                                    { colour = '1.000,1.000,0.000,1.000', colourMap = 'metal_02_d.dds', normalMap = 'metal_02_n.dds', specularMap = 'metal_02_s.dds' },
                                    { colour = '0.200,0.200,0.200,1.000', colourMap = 'squid5_d.dds', normalMap = 'placeholder_n.dds', specularMap = 'placeholder_s.dds' },
                                    { colour = '0.690,0.000,0.129,1.000', colourMap = 'stitchkin1_d.dds', normalMap = 'placeholder_n.dds', specularMap = 'stitchkin1_s.dds' },
                                },
                                ['Lobby_Tall_Walls']           = {
                                    { colour = '0.100,0.100,0.100,1.000', envMap = 'placeholder_d.dds', colourMap = 'futuristicpanels_d.dds', normalMap = 'placeholder_n.dds', specularMap = 'placeholder_s.dds' },
                                    { colour = '0.200,0.200,0.200,1.000', envMap = 'placeholder_d.dds', colourMap = 'Plaster_01_d.dds', normalMap = 'Plaster_01_n.dds', specularMap = 'Plaster_01_s.dds' },
                                    { colour = '0.600,0.600,0.600,1.000', envMap = 'placeholder_d.dds', colourMap = 'Plaster_01_d.dds', normalMap = 'Plaster_01_n.dds', specularMap = 'Plaster_01_s.dds' },
                                    { colour = '0.502,0.000,0.502,1.000', envMap = 'placeholder_d.dds', colourMap = 'Plaster_01_d.dds', normalMap = 'Plaster_01_n.dds', specularMap = 'Plaster_01_s.dds' },
                                    { colour = '0.200,0.549,1.000,1.000', envMap = 'placeholder_d.dds', colourMap = 'Plaster_01_d.dds', normalMap = 'Plaster_01_n.dds', specularMap = 'Plaster_01_s.dds' },
                                    { colour = '0.800,0.800,0.800,1.000', envMap = 'placeholder_d.dds', colourMap = 'Plaster_01_d.dds', normalMap = 'Plaster_01_n.dds', specularMap = 'Plaster_01_s.dds' },
                                    { colour = '0.960,0.960,0.862,1.000', envMap = 'placeholder_d.dds', colourMap = 'Plaster_01_d.dds', normalMap = 'Plaster_01_n.dds', specularMap = 'Plaster_01_s.dds' },
                                    { colour = '0.180,0.000,0.006,1.000', envMap = 'placeholder_d.dds', colourMap = 'Plaster_01_d.dds', normalMap = 'Plaster_01_n.dds', specularMap = 'Plaster_01_s.dds' },
                                    { colour = '0.800,0.692,0.516,1.000', envMap = 'placeholder_d.dds', colourMap = 'circles_1_d.dds', normalMap = 'placeholder_n.dds', specularMap = 'circles_1_s.dds' },
                                    { colour = '0.600,0.000,0.600,1.000', envMap = 'placeholder_d.dds', colourMap = 'Plaster_01_d.dds', normalMap = 'Plaster_01_n.dds', specularMap = 'Plaster_01_s.dds' },
                                },
                                ['Lobby_Gate_External']        = {
                                    { colour = '0.200,0.200,0.200,1.000', colourMap = 'Metal_02_d.dds', normalMap = 'Metal_02_n.dds', specularMap = 'Metal_02_s.dds' },
                                    { colour = '0.000,0.000,0.000,1.000', colourMap = 'Plaster_01_d.dds', normalMap = 'Plaster_01_n.dds', specularMap = 'Plaster_01_s.dds' },
                                    { colour = '0.100,0.100,0.100,1.000', colourMap = 'dots_01_d.dds', normalMap = 'placeholder_n.dds', specularMap = 'dots_01_s.dds' },
                                    { colour = '1.000,0.800,0.800,1.000', colourMap = 'Panel_02_d.dds', normalMap = 'Panel_02_n.dds', specularMap = 'Panel_02_s.dds' },
                                    { colour = '1.000,0.749,0.000,1.000', colourMap = 'mosiacinv_1_d.dds', normalMap = 'placeholder_n.dds', specularMap = 'placeholder_s.dds' },
                                    { colour = '0.900,0.900,0.900,1.000', colourMap = 'dots_01_d.dds', normalMap = 'placeholder_n.dds', specularMap = 'dots_01_s.dds' },
                                    { colour = '0.500,0.500,0.500,1.000', colourMap = 'Plaster_01_d.dds', normalMap = 'Plaster_01_n.dds', specularMap = 'Plaster_01_s.dds' },
                                    { colour = '0.000,0.247,0.341,1.000', colourMap = 'metal_01_d.dds', normalMap = 'metal_01_n.dds', specularMap = 'metal_01_s.dds' },
                                    { colour = '0.100,0.100,0.100,1.000', colourMap = 'Plaster_01_d.dds', normalMap = 'Plaster_01_n.dds', specularMap = 'Plaster_01_s.dds' },
                                    { colour = '0.200,0.200,0.200,1.000', colourMap = 'Plaster_01_d.dds', normalMap = 'Plaster_01_n.dds', specularMap = 'Plaster_01_s.dds' },
                                },
                                ['Booth_1_Colour']             = {
                                    { colour = '1.000,1.000,1.000,1.000', colourMap = 'Panel_01_d.dds', normalMap = 'Panel_01_n.dds', specularMap = 'Panel_01_s.dds' },
                                },
                                ['Booth_1_Back']               = {
                                    { colour = '0.950,0.950,0.950,1.000', envMap = 'placeholder_d.dds', colourMap = 'IronFusion_Logo.dds', normalMap = 'placeholder_n.dds', specularMap = 'placeholder_s.dds' },
                                    { colour = '0.950,0.950,0.950,1.000', envMap = 'placeholder_d.dds', colourMap = 'LKWD_Lion_Red.dds', normalMap = 'placeholder_n.dds', specularMap = 'placeholder_s.dds' },
                                    { colour = '0.950,0.950,0.950,1.000', envMap = 'placeholder_d.dds', colourMap = 'medusa_logo_d.dds', normalMap = 'placeholder_n.dds', specularMap = 'medusa_logo_s.dds' },
                                    { colour = '0.950,0.950,0.950,1.000', envMap = 'placeholder_d.dds', colourMap = 'star_logo_d.dds', normalMap = 'placeholder_n.dds', specularMap = 'placeholder_s.dds' },
                                    { colour = '0.950,0.950,0.950,1.000', envMap = 'placeholder_d.dds', colourMap = 'spiral_logo_d.dds', normalMap = 'placeholder_n.dds', specularMap = 'placeholder_s.dds' },
                                    { colour = '0.950,0.950,0.950,1.000', envMap = 'placeholder_d.dds', colourMap = 'drey_logo_d.dds', normalMap = 'placeholder_n.dds', specularMap = 'placeholder_s.dds' },
                                    { colour = '0.950,0.950,0.950,1.000', envMap = 'placeholder_d.dds', colourMap = 'figment_01_d.dds', normalMap = 'placeholder_n.dds', specularMap = 'placeholder_s.dds' },
                                    { colour = '0.950,0.950,0.950,1.000', envMap = 'placeholder_d.dds', colourMap = 'foolthrottle_logo.dds', normalMap = 'placeholder_n.dds', specularMap = 'placeholder_s.dds' },
                                    { colour = '0.950,0.950,0.950,1.000', envMap = 'placeholder_d.dds', colourMap = 'squid3_d.dds', normalMap = 'placeholder_n.dds', specularMap = 'placeholder_s.dds' },
                                    { colour = '0.950,0.950,0.950,1.000', envMap = 'placeholder_d.dds', colourMap = 'stitchkins_logo_d.dds', normalMap = 'placeholder_n.dds', specularMap = 'placeholder_s.dds' },
                                },
                                ['Booth_2_Colour']             = {
                                    { colour = '1.000,1.000,1.000,1.000', colourMap = 'Panel_01_d.dds', normalMap = 'Panel_01_n.dds', specularMap = 'Panel_01_s.dds' },
                                },
                                ['Booth_2_Back']               = {
                                    { colour = '0.950,0.950,0.950,1.000', envMap = 'placeholder_d.dds', colourMap = 'IronFusion_Logo.dds', normalMap = 'placeholder_n.dds', specularMap = 'placeholder_s.dds' },
                                    { colour = '0.950,0.950,0.950,1.000', envMap = 'placeholder_d.dds', colourMap = 'LKWD_Lion_Red.dds', normalMap = 'placeholder_n.dds', specularMap = 'placeholder_s.dds' },
                                    { colour = '0.950,0.950,0.950,1.000', envMap = 'placeholder_d.dds', colourMap = 'medusa_logo_d.dds', normalMap = 'placeholder_n.dds', specularMap = 'medusa_logo_s.dds' },
                                    { colour = '0.950,0.950,0.950,1.000', envMap = 'placeholder_d.dds', colourMap = 'star_logo_d.dds', normalMap = 'placeholder_n.dds', specularMap = 'placeholder_s.dds' },
                                    { colour = '0.950,0.950,0.950,1.000', envMap = 'placeholder_d.dds', colourMap = 'spiral_logo_d.dds', normalMap = 'placeholder_n.dds', specularMap = 'placeholder_s.dds' },
                                    { colour = '0.950,0.950,0.950,1.000', envMap = 'placeholder_d.dds', colourMap = 'drey_logo_d.dds', normalMap = 'placeholder_n.dds', specularMap = 'placeholder_s.dds' },
                                    { colour = '0.950,0.950,0.950,1.000', envMap = 'placeholder_d.dds', colourMap = 'figment_01_d.dds', normalMap = 'placeholder_n.dds', specularMap = 'placeholder_s.dds' },
                                    { colour = '0.950,0.950,0.950,1.000', envMap = 'placeholder_d.dds', colourMap = 'foolthrottle_logo.dds', normalMap = 'placeholder_n.dds', specularMap = 'placeholder_s.dds' },
                                    { colour = '0.950,0.950,0.950,1.000', envMap = 'placeholder_d.dds', colourMap = 'squid3_d.dds', normalMap = 'placeholder_n.dds', specularMap = 'placeholder_s.dds' },
                                    { colour = '0.950,0.950,0.950,1.000', envMap = 'placeholder_d.dds', colourMap = 'stitchkins_logo_d.dds', normalMap = 'placeholder_n.dds', specularMap = 'placeholder_s.dds' },
                                },
                                ['Booth_3_Colour']             = {
                                    { colour = '1.000,1.000,1.000,1.000', colourMap = 'Panel_01_d.dds', normalMap = 'Panel_01_n.dds', specularMap = 'Panel_01_s.dds' },
                                },
                                ['Booth_3_Back']               = {
                                    { colour = '0.950,0.950,0.950,1.000', envMap = 'placeholder_d.dds', colourMap = 'IronFusion_Logo.dds', normalMap = 'placeholder_n.dds', specularMap = 'placeholder_s.dds' },
                                    { colour = '0.950,0.950,0.950,1.000', envMap = 'placeholder_d.dds', colourMap = 'LKWD_Lion_Red.dds', normalMap = 'placeholder_n.dds', specularMap = 'placeholder_s.dds' },
                                    { colour = '0.950,0.950,0.950,1.000', envMap = 'placeholder_d.dds', colourMap = 'medusa_logo_d.dds', normalMap = 'placeholder_n.dds', specularMap = 'medusa_logo_s.dds' },
                                    { colour = '0.950,0.950,0.950,1.000', envMap = 'placeholder_d.dds', colourMap = 'star_logo_d.dds', normalMap = 'placeholder_n.dds', specularMap = 'placeholder_s.dds' },
                                    { colour = '0.950,0.950,0.950,1.000', envMap = 'placeholder_d.dds', colourMap = 'spiral_logo_d.dds', normalMap = 'placeholder_n.dds', specularMap = 'placeholder_s.dds' },
                                    { colour = '0.950,0.950,0.950,1.000', envMap = 'placeholder_d.dds', colourMap = 'drey_logo_d.dds', normalMap = 'placeholder_n.dds', specularMap = 'placeholder_s.dds' },
                                    { colour = '0.950,0.950,0.950,1.000', envMap = 'placeholder_d.dds', colourMap = 'figment_01_d.dds', normalMap = 'placeholder_n.dds', specularMap = 'placeholder_s.dds' },
                                    { colour = '0.950,0.950,0.950,1.000', envMap = 'placeholder_d.dds', colourMap = 'foolthrottle_logo.dds', normalMap = 'placeholder_n.dds', specularMap = 'placeholder_s.dds' },
                                    { colour = '0.950,0.950,0.950,1.000', envMap = 'placeholder_d.dds', colourMap = 'squid3_d.dds', normalMap = 'placeholder_n.dds', specularMap = 'placeholder_s.dds' },
                                    { colour = '0.950,0.950,0.950,1.000', envMap = 'placeholder_d.dds', colourMap = 'stitchkins_logo_d.dds', normalMap = 'placeholder_n.dds', specularMap = 'placeholder_s.dds' },
                                },
                                ['Booths_Outer']               = {
                                    { colour = '0.000,0.000,0.000,1.000', colourMap = 'placeholder_d.dds', normalMap = 'placeholder_n.dds', specularMap = 'placeholder_s.dds' },
                                },
                                ['Catwalk_Floor']              = {
                                    { colour = '1.000,0.100,0.000,1.000', envMap = 'placeholder_d.dds', colourMap = 'Plaster_01_d.dds', normalMap = 'Plaster_01_n.dds', specularMap = 'Plaster_01_s.dds' },
                                },
                                ['Catwalk_backboard']          = {
                                    { colour = '1.000,1.000,1.000,1.000', colourMap = 'fabric_white_128x128_c.dds', normalMap = 'placeholder_n.dds', specularMap = 'placeholder_s.dds' },
                                },
                                ['Bar_Top_Front']              = {
                                    { colour = '0.000,0.437,0.840,1.000' },
                                    { colour = '1.000,0.100,0.000,1.000' },
                                    { colour = '0.702,0.851,0.161,1.000' },
                                    { colour = '1.000,1.000,0.000,1.000' },
                                    { colour = '0.100,0.229,0.980,1.000' },
                                    { colour = '0.804,0.600,0.000,1.000' },
                                    { colour = '0.789,0.586,0.484,1.000' },
                                    { colour = '0.604,0.400,0.000,1.000' },
                                    { colour = '0.702,0.000,0.502,1.000' },
                                    { colour = '1.000,0.753,0.796,1.000' },
                                },
                                ['Bar_Back']                   = {
                                    { colour = '0.000,0.337,0.741,1.000', colourMap = 'futuristicpanels_d.dds', normalMap = 'placeholder_n.dds', specularMap = 'placeholder_s.dds' },
                                    { colour = '1.000,1.000,1.000,1.000', colourMap = 'Panel_01_d.dds', normalMap = 'Panel_01_n.dds', specularMap = 'Panel_01_s.dds' },
                                    { colour = '0.255,0.412,0.882,1.000', colourMap = 'snakeskin_d.dds', normalMap = 'placeholder_n.dds', specularMap = 'snakeskin_s.dds' },
                                    { colour = '0.720,0.357,0.000,1.000', colourMap = 'Wallpaper_02_d.dds', normalMap = 'Wallpaper_02_n.dds', specularMap = 'Wallpaper_02_s.dds' },
                                    { colour = '0.800,0.800,0.800,1.000', colourMap = 'Panel_02_d.dds', normalMap = 'Panel_02_n.dds', specularMap = 'Panel_02_s.dds' },
                                    { colour = '0.220,0.102,0.051,1.000', colourMap = 'circles_1_d.dds', normalMap = 'placeholder_n.dds', specularMap = 'circles_1_s.dds' },
                                    { colour = '0.498,0.290,0.161,1.000', colourMap = 'Plaster_01_d.dds', normalMap = 'Plaster_01_n.dds', specularMap = 'Plaster_01_s.dds' },
                                    { colour = '0.700,0.000,0.080,1.000', colourMap = 'dots_01_d.dds', normalMap = 'placeholder_n.dds', specularMap = 'dots_01_s.dds' },
                                    { colour = '0.000,0.147,0.601,1.000', colourMap = 'squid5_d.dds', normalMap = 'placeholder_n.dds', specularMap = 'placeholder_s.dds' },
                                    { colour = '1.000,1.000,1.000,1.000', colourMap = 'stitchkinlogotile_d.dds', normalMap = 'placeholder_n.dds', specularMap = 'stitchkinlogotile_s.dds' },
                                },
                                ['Bar_Front']                  = {
                                    { colour = '0.005,0.010,0.030,1.000', envMap = 'placeholder_d.dds', colourMap = 'metal_02_d.dds', normalMap = 'metal_02_n.dds', specularMap = 'metal_02_s.dds' },
                                    { colour = '0.800,0.800,0.800,1.000', envMap = 'placeholder_d.dds', colourMap = 'Plaster_01_d.dds', normalMap = 'Plaster_01_n.dds', specularMap = 'Plaster_01_s.dds' },
                                    { colour = '0.843,0.776,0.682,1.000', envMap = 'placeholder_d.dds', colourMap = 'Plaster_01_d.dds', normalMap = 'Plaster_01_n.dds', specularMap = 'Plaster_01_s.dds' },
                                    { colour = '0.543,0.476,0.382,1.000', envMap = 'placeholder_d.dds', colourMap = 'squid5_d.dds', normalMap = 'placeholder_n.dds', specularMap = 'placeholder_s.dds' },
                                    { colour = '1.000,1.000,1.500,1.000', envMap = 'placeholder_d.dds', colourMap = 'Plaster_01_d.dds', normalMap = 'Plaster_01_n.dds', specularMap = 'Plaster_01_s.dds' },
                                    { colour = '0.050,0.050,0.050,1.000', envMap = 'placeholder_d.dds', colourMap = 'Plaster_01_d.dds', normalMap = 'Plaster_01_n.dds', specularMap = 'Plaster_01_s.dds' },
                                    { colour = '0.643,0.561,0.478,1.000', envMap = 'placeholder_d.dds', colourMap = 'Wallpaper_05_d.dds', normalMap = 'Wallpaper_05_n.dds', specularMap = 'Wallpaper_05_s.dds' },
                                    { colour = '0.000,0.052,0.082,1.000', envMap = 'placeholder_d.dds', colourMap = 'metal_02_d.dds', normalMap = 'metal_02_n.dds', specularMap = 'metal_02_s.dds' },
                                    { colour = '0.200,0.200,0.200,1.000', envMap = 'placeholder_d.dds', colourMap = 'squid4_d.dds', normalMap = 'placeholder_n.dds', specularMap = 'placeholder_s.dds' },
                                    { colour = '0.843,0.776,0.682,1.000', envMap = 'placeholder_d.dds', colourMap = 'stitchkin4_d.dds', normalMap = 'placeholder_n.dds', specularMap = 'stitchkin4_s.dds' },
                                },
                                ['Bar_Floor']                  = {
                                    { colour = '0.500,0.500,0.500,1.000', colourMap = 'metal_02_d.dds', normalMap = 'metal_02_n.dds', specularMap = 'metal_02_s.dds' },
                                    { colour = '0.800,0.800,0.800,1.000', colourMap = 'Marble_02_d.dds', normalMap = 'Marble_02_n.dds', specularMap = 'Marble_02_s.dds' },
                                    { colour = '0.714,0.631,0.522,1.000', colourMap = 'Marble_02_d.dds', normalMap = 'Marble_02_n.dds', specularMap = 'Marble_02_s.dds' },
                                    { colour = '5.000,5.000,5.000,1.000', colourMap = 'tile_01_d.dds', normalMap = 'tile_01_n.dds', specularMap = 'tile_01_s.dds' },
                                    { colour = '1.000,0.298,0.147,1.000', colourMap = 'mosiac_1_d.dds', normalMap = 'placeholder_n.dds', specularMap = 'mosiac_1_s.dds' },
                                    { colour = '1.000,1.000,1.000,1.000', colourMap = 'Panel_04_d.dds', normalMap = 'Panel_04_n.dds', specularMap = 'Panel_04_s.dds' },
                                    { colour = '1.400,1.400,1.400,1.000', colourMap = 'Wood_01_d.dds', normalMap = 'Wood_01_n.dds', specularMap = 'Wood_02_s.dds' },
                                    { colour = '1.000,1.000,1.000,1.000', colourMap = 'Marble_02_d.dds', normalMap = 'Marble_02_n.dds', specularMap = 'Marble_02_s.dds' },
                                    { colour = '0.100,0.229,0.980,1.000', colourMap = 'squid1_d.dds', normalMap = 'placeholder_n.dds', specularMap = 'squid1_s.dds' },
                                    { colour = '0.600,0.000,0.000,1.000', colourMap = 'stitchkin4_d.dds', normalMap = 'placeholder_n.dds', specularMap = 'stitchkin4_s.dds' },
                                },
                                ['Vip_Back']                   = {
                                    { colour = '0.000,0.800,0.800,1.000', colourMap = 'circuit2_d.dds', normalMap = 'placeholder_n.dds', specularMap = 'circuit2_s.dds' },
                                    { colour = '1.000,1.000,1.000,1.000', colourMap = 'Panel_01_d.dds', normalMap = 'Panel_01_n.dds', specularMap = 'Panel_01_s.dds' },
                                    { colour = '0.250,0.650,1.000,1.000', colourMap = 'wallpaper_06_d.dds', normalMap = 'wallpaper_06_n.dds', specularMap = 'wallpaper_06_s.dds' },
                                },
                                ['Vip_Floor_Tile']             = {
                                    { colour = '0.700,0.700,0.700,1.000', colourMap = 'metal_04_d.dds', normalMap = 'metal_04_n.dds', specularMap = 'metal_04_s.dds' },
                                    { colour = '0.700,0.700,0.700,1.000', colourMap = 'tile_01_d.dds', normalMap = 'tile_01_n.dds', specularMap = 'tile_01_s.dds' },
                                    { colour = '0.000,0.110,0.051,1.000', colourMap = 'snakeskin_d.dds', normalMap = 'placeholder_n.dds', specularMap = 'snakeskin_s.dds' },
                                },
                                ['Vip_StairWall']              = {
                                    { colour = '0.600,0.600,0.600,1.000', envMap = 'placeholder_d.dds', colourMap = 'Plaster_01_d.dds', normalMap = 'Plaster_01_n.dds', specularMap = 'Plaster_01_s.dds' },
                                    { colour = '0.800,0.800,0.800,1.000', envMap = 'placeholder_d.dds', colourMap = 'Plaster_01_d.dds', normalMap = 'Plaster_01_n.dds', specularMap = 'Plaster_01_s.dds' },
                                    { colour = '0.400,0.700,0.050,1.000', envMap = 'placeholder_d.dds', colourMap = 'Plaster_01_d.dds', normalMap = 'Plaster_01_n.dds', specularMap = 'Plaster_01_s.dds' },
                                },
                                ['Vip_Cloth']                  = {
                                    { colour = '0.000,0.000,0.500,1.000' },
                                    { colour = '1.000,0.100,0.000,1.000' },
                                    { colour = '0.600,0.900,0.200,1.000' },
                                },
                                ['Backstage_Back_Wall']        = {
                                    { colour = '1.000,1.000,1.000,1.000', colourMap = 'Panel_01_d.dds', normalMap = 'Panel_01_n.dds', specularMap = 'Panel_01_s.dds' },
                                },
                                ['Backstage_Walls']            = {
                                    { colour = '0.800,0.800,0.800,1.000', colourMap = 'Plaster_01_d.dds', normalMap = 'Plaster_01_n.dds', specularMap = 'Plaster_01_s.dds' },
                                },
                                ['Backstage_Ceiling']          = {
                                    { colour = '0.800,0.800,0.800,1.000', colourMap = 'Wood_02_d.dds', normalMap = 'Wood_02_n.dds', specularMap = 'Wood_02_s.dds' },
                                },
                                ['Backstage_Floor']            = {
                                    { colour = '1.000,0.100,0.000,1.000', envMap = 'placeholder_d.dds', colourMap = 'Plaster_01_d.dds', normalMap = 'Plaster_01_n.dds', specularMap = 'Plaster_01_s.dds' },
                                },
                                ['Backstage_Tassles']          = {
                                    { colour = '1.000,0.100,0.000,1.000' },
                                },
                                ['Backstage_wood_dark_veneer'] = {
                                    { colour = '0.800,0.800,0.800,1.000', envMap = 'placeholder_d.dds', colourMap = 'Plaster_01_d.dds', normalMap = 'Plaster_01_n.dds', specularMap = 'Plaster_01_s.dds' },
                                },
                                ['Backstage_wood_dark_veneer2']= {
                                    { colour = '0.100,0.100,0.100,1.000', envMap = 'placeholder_d.dds', colourMap = 'Marble_01_d.dds', normalMap = 'placeholder_n.dds', specularMap = 'Marble_01_s.dds' },
                                },
                            }
                        }
	                }

		            local options = {
			            profiles = setDressing.profiles,
			            entities = {},
		            }
                    local params = {
                        ['lobby'] = {
                                'Lobby_Gate_Base',
                                'Lobby_Main_Walls',
                                'Lobby_Short_Walls',
                                'Lobby_Tall_Walls',
                            },
                            ['runway'] = {
                                'Lobby_Gate_Internal',
                                'Lobby_Gate_External',
                                'Booth_1_Colour',
                                'Booth_1_Back',
                                'Booth_2_Colour',
                                'Booth_2_Back',
                                'Booth_3_Colour',
                                'Booth_3_Back',
                                'Booths_Outer',
                                'Catwalk_Floor',
                                'Catwalk_backboard',
                            },
                            ['bar'] = {
                                'Bar_Top_Front',
                                'Bar_Back',
                                'Bar_Front',
                                'Bar_Floor',
                            },
                            ['lounge'] = {
                                'Vip_Back',
                                'Vip_Floor_Tile',
                                'Vip_StairWall',
                                'Vip_Cloth',
                            },
                            ['vip_lounge'] = {
                                'Backstage_Back_Wall',
                                'Backstage_Walls',
                                'Backstage_Ceiling',
                                'Backstage_Floor',
                                'Backstage_Tassles',
                                'Backstage_wood_dark_veneer',
                                'Backstage_wood_dark_veneer2',
                            },
                    }
                    local dressingDef = {}

		            for entName, entDef in pairs(setDressing.entities) do
                        options.entities[entName] = {}
                        for matName, variants in pairs(entDef) do
                            options.entities[entName][matName] = {}
                            for optionId, optionData in pairs(variants) do
                                options.entities[entName][matName][optionId] = optionData
                            end
                        end
                    end
                    for entName, entDef in pairs(setDressing.entities) do
                        dressingDef[entName] = {}
                        for matName, variants in pairs(entDef) do
                            dressingDef[entName][matName] = {}
                            for optionId, _ in pairs(variants) do
                                dressingDef[entName][matName] = optionId
                            end
                        end
                    end

                    local TableFromInput = {
					    options 	= options,
					    setups 		= {
						     ['default'] = {
							        profiles 	= {
                                        ['commercePoints'] = 'default',
                                        ['Logos'] = 'lkwd',
                                        ['Stagertron'] = 'default',
                                        ['Runwaytron'] = 'default',
                                        ['Customisation'] = 'default',
                                        ['Posertrons'] = 'default',
                                        ['BackstagePass'] = 'default',
                                        ['Music'] = 'LKWD',
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
					    customisationGroups 	= {
                            {
                                name = 'Lobby',
                                camPos = '12.450,8.263,41.139,0.000',
                                lookAt = '-25.000,-6.000,0.000,0.000',
                                lockedIcon = 'locked.dds',
                                lockedOverlay = 'swatch_overlay.dds',
                                entities 	= {
                                    {
                                        name = 'main_scene',
                                        materials = params.lobby
                                    }
                                },
                                themes = {
                                    { id = 1, dressing = { 
                                                entities = { 
                                                    ['main_scene'] = { 
                                                        Lobby_Gate_Base = 1,
                                                        Lobby_Main_Walls = 1,
                                                        Lobby_Short_Walls = 1,
                                                        Lobby_Tall_Walls = 1,
                                                    }
                                                } 
                                              }, profiles = {}, icon = 'ironfusion.dds' },
                                    { id = 2, dressing = { 
                                                entities = { 
                                                    ['main_scene'] = { 
                                                        Lobby_Gate_Base = 2,
                                                        Lobby_Main_Walls = 2,
                                                        Lobby_Short_Walls = 2,
                                                        Lobby_Tall_Walls = 2,
                                                    }
                                                } 
                                              }, profiles = {}, icon = 'lkwd.dds' },
                                    { id = 3, dressing = { 
                                                entities = { 
                                                    ['main_scene'] = { 
                                                        Lobby_Gate_Base = 3,
                                                        Lobby_Main_Walls = 3,
                                                        Lobby_Short_Walls = 3,
                                                        Lobby_Tall_Walls = 3,
                                                    }
                                                } 
                                              }, profiles = {}, icon = 'medusa.dds' },
                                    { id = 4, dressing = { 
                                                entities = { 
                                                    ['main_scene'] = { 
                                                        Lobby_Gate_Base = 4,
                                                        Lobby_Main_Walls = 4,
                                                        Lobby_Short_Walls = 4,
                                                        Lobby_Tall_Walls = 4,
                                                    }
                                                } 
                                              }, profiles = {}, icon = 'star.dds' },
                                    { id = 5, dressing = { 
                                                entities = { 
                                                    ['main_scene'] = { 
                                                        Lobby_Gate_Base = 5,
                                                        Lobby_Main_Walls = 5,
                                                        Lobby_Short_Walls = 5,
                                                        Lobby_Tall_Walls = 5,
                                                    }
                                                } 
                                              }, profiles = {}, icon = 'spiral.dds' },
                                    { id = 6, dressing = { 
                                                entities = { 
                                                    ['main_scene'] = { 
                                                        Lobby_Gate_Base = 6,
                                                        Lobby_Main_Walls = 6,
                                                        Lobby_Short_Walls = 6,
                                                        Lobby_Tall_Walls = 6,
                                                    }
                                                } 
                                              }, profiles = {}, icon = 'drey.dds' },
                                    { id = 7, dressing = { 
                                                entities = { 
                                                    ['main_scene'] = { 
                                                        Lobby_Gate_Base = 7,
                                                        Lobby_Main_Walls = 7,
                                                        Lobby_Short_Walls = 7,
                                                        Lobby_Tall_Walls = 7,
                                                    }
                                                } 
                                              }, profiles = {}, icon = 'figment.dds' },
                                    { id = 8, dressing = { 
                                                entities = { 
                                                    ['main_scene'] = { 
                                                        Lobby_Gate_Base = 8,
                                                        Lobby_Main_Walls = 8,
                                                        Lobby_Short_Walls = 8,
                                                        Lobby_Tall_Walls = 8,
                                                    }
                                                } 
                                              }, profiles = {}, icon = 'foolthrottle.dds' },
                                    { id = 9, dressing = { 
                                                entities = { 
                                                    ['main_scene'] = { 
                                                        Lobby_Gate_Base = 9,
                                                        Lobby_Main_Walls = 9,
                                                        Lobby_Short_Walls = 9,
                                                        Lobby_Tall_Walls = 9,
                                                    }
                                                } 
                                              }, profiles = {}, icon = 'dsquid.dds' },
                                   { id = 10, dressing = { 
                                                entities = { 
                                                    ['main_scene'] = { 
                                                        Lobby_Gate_Base = 10,
                                                        Lobby_Main_Walls = 10,
                                                        Lobby_Short_Walls = 10,
                                                        Lobby_Tall_Walls = 10,
                                                    }
                                                } 
                                              }, profiles = {}, icon = 'stitchkin.dds' } 
                                }
                            },
                            {
                                name = 'Bar',
                                camPos = '-11.943,4.505,10.067,0.000',
                                lookAt = '-14.000,0.000,0.000,0.000',
                                lockedIcon = 'locked.dds',
                                lockedOverlay = 'swatch_overlay.dds',
                                entities 	= {
                                    {
                                        name = 'main_scene',
                                        materials = params.bar
                                    }
                                },
                                themes = {
                                    { id = 1, dressing = { 
                                                entities = { 
                                                    ['main_scene'] = { 
                                                        Bar_Top_Front = 1,
                                                        Bar_Back = 1,
                                                        Bar_Front = 1,
                                                        Bar_Floor = 1,
                                                    }
                                                } 
                                              }, profiles = {}, icon = 'ironfusion.dds' },
                                    { id = 2, dressing = { 
                                                entities = { 
                                                    ['main_scene'] = { 
                                                        Bar_Top_Front = 2,
                                                        Bar_Back = 2,
                                                        Bar_Front = 2,
                                                        Bar_Floor = 2,
                                                    }
                                                } 
                                              }, profiles = {}, icon = 'lkwd.dds' },
                                    { id = 3, dressing = { 
                                                entities = { 
                                                    ['main_scene'] = { 
                                                        Bar_Top_Front = 3,
                                                        Bar_Back = 3,
                                                        Bar_Front = 3,
                                                        Bar_Floor = 3,
                                                    }
                                                } 
                                              }, profiles = {}, icon = 'medusa.dds' },
                                    { id = 4, dressing = { 
                                                entities = { 
                                                    ['main_scene'] = { 
                                                        Bar_Top_Front = 4,
                                                        Bar_Back = 4,
                                                        Bar_Front = 4,
                                                        Bar_Floor = 4,
                                                    }
                                                } 
                                              }, profiles = {}, icon = 'star.dds' },
                                    { id = 5, dressing = { 
                                                entities = { 
                                                    ['main_scene'] = { 
                                                        Bar_Top_Front = 5,
                                                        Bar_Back = 5,
                                                        Bar_Front = 5,
                                                        Bar_Floor = 5,
                                                    }
                                                } 
                                              }, profiles = {}, icon = 'spiral.dds' },
                                    { id = 6, dressing = { 
                                                entities = { 
                                                    ['main_scene'] = { 
                                                        Bar_Top_Front = 6,
                                                        Bar_Back = 6,
                                                        Bar_Front = 6,
                                                        Bar_Floor = 6,
                                                    }
                                                } 
                                              }, profiles = {}, icon = 'drey.dds' },
                                    { id = 7, dressing = { 
                                                entities = { 
                                                    ['main_scene'] = { 
                                                        Bar_Top_Front = 7,
                                                        Bar_Back = 7,
                                                        Bar_Front = 7,
                                                        Bar_Floor = 7,
                                                    }
                                                } 
                                              }, profiles = {}, icon = 'figment.dds' },
                                    { id = 8, dressing = { 
                                                entities = { 
                                                    ['main_scene'] = { 
                                                        Bar_Top_Front = 8,
                                                        Bar_Back = 8,
                                                        Bar_Front = 8,
                                                        Bar_Floor = 8,
                                                    }
                                                } 
                                              }, profiles = {}, icon = 'foolthrottle.dds' },
                                    { id = 9, dressing = { 
                                                entities = { 
                                                    ['main_scene'] = { 
                                                        Bar_Top_Front = 9,
                                                        Bar_Back = 9,
                                                        Bar_Front = 9,
                                                        Bar_Floor = 9,
                                                    }
                                                } 
                                              }, profiles = {}, icon = 'dsquid.dds' },
                                   { id = 10, dressing = { 
                                                entities = { 
                                                    ['main_scene'] = { 
                                                        Bar_Top_Front = 10,
                                                        Bar_Back = 10,
                                                        Bar_Front = 10,
                                                        Bar_Floor = 10,
                                                    }
                                                } 
                                              }, profiles = {}, icon = 'stitchkin.dds' } 
                                }
                            },
                            {
                                name = 'Lounge',
                                camPos = '-10.257,8.656,-8.927,0.000',
                                lookAt = '-18.000,8.000,0.000,0.000',
                                lockedIcon = 'locked.dds',
                                lockedOverlay = 'swatch_overlay.dds',
                                entities 	= {
                                    {
                                        name = 'main_scene',
                                        materials = params.lounge
                                    }
                                },
                                themes = {
                                    { id = 1, dressing = { 
                                                entities = { 
                                                    ['main_scene'] = { 
                                                        Vip_Back = 1,
                                                        Vip_Floor_Tile = 1,
                                                        Vip_StairWall = 1,
                                                        Vip_Cloth = 1,
                                                    }
                                                } 
                                              }, profiles = {}, icon = 'ironfusion.dds' },
                                    { id = 2, dressing = { 
                                                entities = { 
                                                    ['main_scene'] = { 
                                                        Vip_Back = 2,
                                                        Vip_Floor_Tile = 2,
                                                        Vip_StairWall = 2,
                                                        Vip_Cloth = 2,
                                                    }
                                                } 
                                              }, profiles = {}, icon = 'lkwd.dds' },
                                    { id = 3, dressing = { 
                                                entities = { 
                                                    ['main_scene'] = { 
                                                        Vip_Back = 3,
                                                        Vip_Floor_Tile = 3,
                                                        Vip_StairWall = 3,
                                                        Vip_Cloth = 3,
                                                    }
                                                } 
                                              }, profiles = {}, icon = 'medusa.dds' }
                                }
                            },
                            {
                                name = 'VIP Lounge',
                                camPos = '-3.551,5.370,-26.312,0.000',
                                lookAt = '81.000,-4.000,0.000,0.000',
                                lockedIcon = 'locked.dds',
                                lockedOverlay = 'swatch_overlay.dds',
                                entities 	= {
                                    {
                                        name = 'main_scene',
                                        materials = params.vip_lounge
                                    }
                                },
                                themes = {
                                    { id = 1, dressing = { 
                                                entities = { 
                                                    ['main_scene'] = { 
                                                        Backstage_Back_Wall = 1,
                                                        Backstage_Walls = 1,
                                                        Backstage_Ceiling = 1,
                                                        Backstage_Floor = 1,
                                                        Backstage_Tassles = 1,
                                                        Backstage_wood_dark_veneer = 1,
                                                        Backstage_wood_dark_veneer2 = 1,
                                                    }
                                                } 
                                              }, profiles = {}, icon = 'lkwd.dds' },
                                }
                            },
                            {
                                name = 'Runway',
                                camPos = '8.862,6.398,16.067,0.000',
                                lookAt = '0.000,0.000,0.000,0.000',
                                lockedIcon = 'locked.dds',
                                lockedOverlay = 'swatch_overlay.dds',
                                entities 	= {
                                    {
                                        name = 'main_scene',
                                        materials = params.runway
                                    }
                                },
                                profileGroups 	= {
                                    'Music',
                                    'Runwaytron',
                                    'Logos'
                                },
                                 themes = {
                                    { id = 1, dressing = { 
                                                entities = { 
                                                    ['main_scene'] = { 
                                                        Lobby_Gate_Internal = 1,
                                                        Lobby_Gate_External = 1,
                                                        Booth_1_Colour = 1,
                                                        Booth_1_Back = 1,
                                                        Booth_2_Colour = 1,
                                                        Booth_2_Back = 1,
                                                        Booth_3_Colour = 1,
                                                        Booth_3_Back = 1,
                                                        Booths_Outer = 1,
                                                        Catwalk_Floor = 1,
                                                        Catwalk_backboard = 1,
                                                    }
                                                } 
                                              }, profiles = {}, icon = 'ironfusion.dds' },
                                    { id = 2, dressing = { 
                                                entities = { 
                                                    ['main_scene'] = { 
                                                        Lobby_Gate_Internal = 2,
                                                        Lobby_Gate_External = 2,
                                                        Booth_1_Colour = 1,
                                                        Booth_1_Back = 2,
                                                        Booth_2_Colour = 1,
                                                        Booth_2_Back = 2,
                                                        Booth_3_Colour = 1,
                                                        Booth_3_Back = 2,
                                                        Booths_Outer = 1,
                                                        Catwalk_Floor = 1,
                                                        Catwalk_backboard = 1,
                                                    }
                                                } 
                                              }, profiles = {}, icon = 'lkwd.dds' },
                                    { id = 3, dressing = { 
                                                entities = { 
                                                    ['main_scene'] = { 
                                                        Lobby_Gate_Internal = 3,
                                                        Lobby_Gate_External = 3,
                                                        Booth_1_Colour = 1,
                                                        Booth_1_Back = 3,
                                                        Booth_2_Colour = 1,
                                                        Booth_2_Back = 3,
                                                        Booth_3_Colour = 1,
                                                        Booth_3_Back = 3,
                                                        Booths_Outer = 1,
                                                        Catwalk_Floor = 1,
                                                        Catwalk_backboard = 1,
                                                    }
                                                } 
                                              }, profiles = {}, icon = 'medusa.dds' },
                                    { id = 4, dressing = { 
                                                entities = { 
                                                    ['main_scene'] = { 
                                                        Lobby_Gate_Internal = 4,
                                                        Lobby_Gate_External = 4,
                                                        Booth_1_Colour = 1,
                                                        Booth_1_Back = 4,
                                                        Booth_2_Colour = 1,
                                                        Booth_2_Back = 4,
                                                        Booth_3_Colour = 1,
                                                        Booth_3_Back = 5,
                                                        Booths_Outer = 1,
                                                        Catwalk_Floor = 1,
                                                        Catwalk_backboard = 1,
                                                    }
                                                } 
                                              }, profiles = {}, icon = 'star.dds' },
                                    { id = 5, dressing = { 
                                                entities = { 
                                                    ['main_scene'] = { 
                                                        Lobby_Gate_Internal = 5,
                                                        Lobby_Gate_External = 5,
                                                        Booth_1_Colour = 1,
                                                        Booth_1_Back = 5,
                                                        Booth_2_Colour = 1,
                                                        Booth_2_Back = 5,
                                                        Booth_3_Colour = 1,
                                                        Booth_3_Back = 5,
                                                        Booths_Outer = 1,
                                                        Catwalk_Floor = 1,
                                                        Catwalk_backboard = 1,
                                                    }
                                                } 
                                              }, profiles = {}, icon = 'spiral.dds' },
                                    { id = 6, dressing = { 
                                                entities = { 
                                                    ['main_scene'] = { 
                                                        Lobby_Gate_Internal = 6,
                                                        Lobby_Gate_External = 6,
                                                        Booth_1_Colour = 1,
                                                        Booth_1_Back = 6,
                                                        Booth_2_Colour = 1,
                                                        Booth_2_Back = 6,
                                                        Booth_3_Colour = 1,
                                                        Booth_3_Back = 6,
                                                        Booths_Outer = 1,
                                                        Catwalk_Floor = 1,
                                                        Catwalk_backboard = 1,
                                                    }
                                                } 
                                              }, profiles = {}, icon = 'drey.dds' },
                                    { id = 7, dressing = { 
                                                entities = { 
                                                    ['main_scene'] = { 
                                                        Lobby_Gate_Internal = 7,
                                                        Lobby_Gate_External = 7,
                                                        Booth_1_Colour = 1,
                                                        Booth_1_Back = 7,
                                                        Booth_2_Colour = 1,
                                                        Booth_2_Back = 7,
                                                        Booth_3_Colour = 1,
                                                        Booth_3_Back = 7,
                                                        Booths_Outer = 1,
                                                        Catwalk_Floor = 1,
                                                        Catwalk_backboard = 1,
                                                    }
                                                } 
                                              }, profiles = {}, icon = 'figment.dds' },
                                    { id = 8, dressing = { 
                                                entities = { 
                                                    ['main_scene'] = { 
                                                        Lobby_Gate_Internal = 8,
                                                        Lobby_Gate_External = 8,
                                                        Booth_1_Colour = 1,
                                                        Booth_1_Back = 8,
                                                        Booth_2_Colour = 1,
                                                        Booth_2_Back = 8,
                                                        Booth_3_Colour = 1,
                                                        Booth_3_Back = 8,
                                                        Booths_Outer = 1,
                                                        Catwalk_Floor = 1,
                                                        Catwalk_backboard = 1,
                                                    }
                                                } 
                                              }, profiles = {}, icon = 'foolthrottle.dds' },
                                    { id = 9, dressing = { 
                                                entities = { 
                                                    ['main_scene'] = { 
                                                        Lobby_Gate_Internal = 9,
                                                        Lobby_Gate_External = 9,
                                                        Booth_1_Colour = 1,
                                                        Booth_1_Back = 9,
                                                        Booth_2_Colour = 1,
                                                        Booth_2_Back = 9,
                                                        Booth_3_Colour = 1,
                                                        Booth_3_Back = 9,
                                                        Booths_Outer = 1,
                                                        Catwalk_Floor = 1,
                                                        Catwalk_backboard = 1,
                                                    }
                                                } 
                                              }, profiles = {}, icon = 'dsquid.dds' },
                                   { id = 10, dressing = { 
                                                entities = { 
                                                    ['main_scene'] = { 
                                                        Lobby_Gate_Internal = 10,
                                                        Lobby_Gate_External = 10,
                                                        Booth_1_Colour = 1,
                                                        Booth_1_Back = 10,
                                                        Booth_2_Colour = 1,
                                                        Booth_2_Back = 10,
                                                        Booth_3_Colour = 1,
                                                        Booth_3_Back = 10,
                                                        Booths_Outer = 1,
                                                        Catwalk_Floor = 1,
                                                        Catwalk_backboard = 1,
                                                    }
                                                } 
                                              }, profiles = {}, icon = 'stitchkin.dds' } 
                                }
                            }
                        },
					    customisation 			= {
						    profiles 	= { 
                                ['Music'] = { ['name'] = 'Music', ['options'] = { { id = 1 },{ id = 2 },{ id = 3 },{ id = 4 },{ id = 5 },{ id = 6 },{ id = 7 },{ id = 8 },{ id = 9 },{ id = 10 },{ id = 11 } }, pos = '8.862,6.398,16.067,0.000', look = '0.000,0.000,0.000,0.000' },
                                ['Runwaytron'] = { ['name'] = 'Stage', ['options'] = { { id = 1 },{ id = 2 },{ id = 3 } }, pos = '8.862,6.398,16.067,0.000', look = '0.000,0.000,0.000,0.000' },
                                ['Logos'] = { ['name'] = 'Logos', ['options'] = { { id = 1 },{ id = 2 },{ id = 3 },{ id = 4 },{ id = 5 },{ id = 6 },{ id = 7 },{ id = 8 },{ id = 9 },{ id = 10 },{ id = 11 },{ id = 12 },{ id = 13 },{ id = 14 },{ id = 15 }}, pos = '8.862,6.398,16.067,0.000', look = '0.000,5.200,0.000,0.000' },
                            },
						    entities 	= {
                                ['main_scene'] = {
                                    ['Lobby_Gate_Internal']        = { ['default'] = {}, ['name'] = 'Lobby - Entrance - Internal', ['options'] = { { id = 1 },{ id = 2 },{ id = 3 },{ id = 4 },{ id = 5 },{ id = 6 },{ id = 7 },{ id = 8 },{ id = 9 },{ id = 10 } }, pos = '5.287,1.435,13.751,0.000', look = '-11.000,0.000,90.000,0.000' },
                                    ['Lobby_Gate_Base']            = { ['default'] = {}, ['name'] = 'Lobby - Entrance - Base', ['options'] = { { id = 1 },{ id = 2 },{ id = 3 },{ id = 4 },{ id = 5 },{ id = 6 },{ id = 7 },{ id = 8 },{ id = 9 },{ id = 10 } }, pos = '5.239,5.595,30.954,0.000', look = '0.000,-12.500,0.000,0.000' },
                                    ['Lobby_Main_Walls']           = { ['default'] = {}, ['name'] = 'Lobby - Main Walls', ['options'] = { { id = 1 },{ id = 2 },{ id = 3 },{ id = 4 },{ id = 5 },{ id = 6 },{ id = 7 },{ id = 8 },{ id = 9 },{ id = 10 } }, pos = '10.310,2.967,31.814,0.000', look = '-60.000,0.000,0.000,0.000' },
                                    ['Lobby_Short_Walls']          = { ['default'] = {}, ['name'] = 'Lobby - Short Walls', ['options'] = { { id = 1 },{ id = 2 },{ id = 3 },{ id = 4 },{ id = 5 },{ id = 6 },{ id = 7 },{ id = 8 },{ id = 9 },{ id = 10 } }, pos = '-4.559,3.563,29.758,0.000', look = '0.000,0.000,0.000,0.000' },
                                    ['Lobby_Tall_Walls']           = { ['default'] = {}, ['name'] = 'Lobby - Tall Walls', ['options'] = { { id = 1 },{ id = 2 },{ id = 3 },{ id = 4 },{ id = 5 },{ id = 6 },{ id = 7 },{ id = 8 },{ id = 9 },{ id = 10 } }, pos = '-6.541,5.912,27.535,0.000', look = '0.000,3.500,0.000,0.000' },
                                    ['Lobby_Gate_External']        = { ['default'] = {}, ['name'] = 'Lobby - Entrance - External', ['options'] = { { id = 1 },{ id = 2 },{ id = 3 },{ id = 4 },{ id = 5 },{ id = 6 },{ id = 7 },{ id = 8 },{ id = 9 },{ id = 10 } }, pos = '7.540,8.427,11.708,0.000', look = '-11.000,-7.000,90.000,0.000' },
                                    ['Booth_1_Colour']             = { ['default'] = {}, ['name'] = 'Runway - Booth 1 Colour', ['options'] = { { id = 1 },{ id = 2 },{ id = 3 },{ id = 4 },{ id = 5 },{ id = 6 },{ id = 7 },{ id = 8 },{ id = 9 },{ id = 10 } }, pos = '3.377,4.857,-6.149,0.000', look = '38.000,4.000,0.000,0.000' },
                                    ['Booth_1_Back']               = { ['default'] = {}, ['name'] = 'Runway - Booth 1 Wall', ['options'] = { { id = 1 },{ id = 2 },{ id = 3 },{ id = 4 },{ id = 5 },{ id = 6 },{ id = 7 },{ id = 8 },{ id = 9 },{ id = 10 } }, pos = '3.377,4.857,-6.149,0.000', look = '38.000,4.000,0.000,0.000' },
                                    ['Booth_2_Colour']             = { ['default'] = {}, ['name'] = 'Runway - Booth 2 Colour', ['options'] = { { id = 1 },{ id = 2 },{ id = 3 },{ id = 4 },{ id = 5 },{ id = 6 },{ id = 7 },{ id = 8 },{ id = 9 },{ id = 10 } }, pos = '4.388,4.110,2.642,0.000', look = '38.000,4.000,0.000,0.000' },
                                    ['Booth_2_Back']               = { ['default'] = {}, ['name'] = 'Runway - Booth 2 Wall', ['options'] = { { id = 1 },{ id = 2 },{ id = 3 },{ id = 4 },{ id = 5 },{ id = 6 },{ id = 7 },{ id = 8 },{ id = 9 },{ id = 10 } }, pos = '4.388,4.110,2.642,0.000', look = '38.000,4.000,0.000,0.000' },
                                    ['Booth_3_Colour']             = { ['default'] = {}, ['name'] = 'Runway - Booth 2 Colour', ['options'] = { { id = 1 },{ id = 2 },{ id = 3 },{ id = 4 },{ id = 5 },{ id = 6 },{ id = 7 },{ id = 8 },{ id = 9 },{ id = 10 } }, pos = '4.410,4.110,11.466,0.000', look = '38.000,4.000,0.000,0.000' },
                                    ['Booth_3_Back']               = { ['default'] = {}, ['name'] = 'Runway - Booth 3 Wall', ['options'] = { { id = 1 },{ id = 2 },{ id = 3 },{ id = 4 },{ id = 5 },{ id = 6 },{ id = 7 },{ id = 8 },{ id = 9 },{ id = 10 } }, pos = '4.410,4.110,11.466,0.000', look = '38.000,4.000,0.000,0.000' },
                                    ['Booths_Outer']               = { ['default'] = {}, ['name'] = 'Runway - Booths Outer', ['options'] = { { id = 1 },{ id = 2 },{ id = 3 },{ id = 4 },{ id = 5 },{ id = 6 },{ id = 7 },{ id = 8 },{ id = 9 },{ id = 10 } }, pos = '-7.332,3.192,-3.696,0.000', look = '38.000,4.000,0.000,0.000' },
                                    ['Catwalk_Floor']              = { ['default'] = {}, ['name'] = 'Runway - Catwalk Floor', ['options'] = { { id = 1 },{ id = 2 },{ id = 3 },{ id = 4 },{ id = 5 },{ id = 6 },{ id = 7 },{ id = 8 },{ id = 9 },{ id = 10 } }, pos = '7.596,5.559,19.013,0.000', look = '0.000,0.000,0.000,0.000' },
                                    ['Catwalk_backboard']          = { ['default'] = {}, ['name'] = 'Runway - Catwalk Backboard', ['options'] = { { id = 1 },{ id = 2 },{ id = 3 },{ id = 4 },{ id = 5 },{ id = 6 },{ id = 7 },{ id = 8 },{ id = 9 },{ id = 10 } }, pos = '7.596,5.559,19.013,0.000', look = '0.000,0.000,0.000,0.000' },
                                    ['Bar_Top_Front']              = { ['default'] = {}, ['name'] = 'Bar - Front', ['options'] = { { id = 1 },{ id = 2 },{ id = 3 },{ id = 4 },{ id = 5 },{ id = 6 },{ id = 7 },{ id = 8 },{ id = 9 },{ id = 10 } }, pos = '-17.234,1.701,-6.265,0.000', look = '-80.000,0.000,0.000,0.000' },
                                    ['Bar_Back']                   = { ['default'] = {}, ['name'] = 'Bar - Wallpaper', ['options'] = { { id = 1 },{ id = 2 },{ id = 3 },{ id = 4 },{ id = 5 },{ id = 6 },{ id = 7 },{ id = 8 },{ id = 9 },{ id = 10 } }, pos = '-14.563,3.419,-9.750,0.000', look = '-90.000,0.000,0.000,0.000' },
                                    ['Bar_Front']                  = { ['default'] = {}, ['name'] = 'Bar - Wall', ['options'] = { { id = 1 },{ id = 2 },{ id = 3 },{ id = 4 },{ id = 5 },{ id = 6 },{ id = 7 },{ id = 8 },{ id = 9 },{ id = 10 } }, pos = '3.113,3.067,-1.135,0.000', look = '-15.000,4.000,0.000,0.000' },
                                    ['Bar_Floor']                  = { ['default'] = {}, ['name'] = 'Bar - Floor', ['options'] = { { id = 1 },{ id = 2 },{ id = 3 },{ id = 4 },{ id = 5 },{ id = 6 },{ id = 7 },{ id = 8 },{ id = 9 },{ id = 10 } }, pos = '-12.031,4.908,-10.922,0.000', look = '-15.000,0.000,0.000,0.000' },
                                    ['Vip_Back']                   = { ['default'] = {}, ['name'] = 'Lounge - Wall', ['options'] = { { id = 1 },{ id = 2 },{ id = 3 },{ id = 4 },{ id = 5 },{ id = 6 },{ id = 7 },{ id = 8 },{ id = 9 },{ id = 10 } }, pos = '-15.357,7.895,8.958,0.000', look = '-30.000,5.500,0.000,0.000' },
                                    ['Vip_Floor_Tile']             = { ['default'] = {}, ['name'] = 'Lounge - Floor', ['options'] = { { id = 1 },{ id = 2 },{ id = 3 },{ id = 4 },{ id = 5 },{ id = 6 },{ id = 7 },{ id = 8 },{ id = 9 },{ id = 10 } }, pos = '-10.022,9.629,-10.856,0.000', look = '-30.000,0.000,0.000,0.000' },
                                    ['Vip_StairWall']              = { ['default'] = {}, ['name'] = 'Lounge - Stairwell', ['options'] = { { id = 1 },{ id = 2 },{ id = 3 },{ id = 4 },{ id = 5 },{ id = 6 },{ id = 7 },{ id = 8 },{ id = 9 },{ id = 10 } }, pos = '-9.991,9.439,-10.306,0.000', look = '0.000,-9.000,40.000,0.000' },
                                    ['Vip_Cloth']                  = { ['default'] = {}, ['name'] = 'Lounge - Canopy', ['options'] = { { id = 1 },{ id = 2 },{ id = 3 },{ id = 4 },{ id = 5 },{ id = 6 },{ id = 7 },{ id = 8 },{ id = 9 },{ id = 10 } }, pos = '-11.557,8.527,9.810,0.000', look = '-29.050,10.600,0.000,0.000' },
                                    ['Backstage_Back_Wall']        = { ['default'] = {}, ['name'] = 'VIP Lounge - Back Wall', ['options'] = { { id = 1 },{ id = 2 },{ id = 3 },{ id = 4 },{ id = 5 },{ id = 6 },{ id = 7 },{ id = 8 },{ id = 9 },{ id = 10 } }, pos = '11.521,3.411,-31.475,0.000', look = '-100.000,0.000,0.000,0.000' },
                                    ['Backstage_Walls']            = { ['default'] = {}, ['name'] = 'VIP Lounge - Walls', ['options'] = { { id = 1 },{ id = 2 },{ id = 3 },{ id = 4 },{ id = 5 },{ id = 6 },{ id = 7 },{ id = 8 },{ id = 9 },{ id = 10 } }, pos = '-1.506,3.362,-28.657,0.000', look = '-190.000,0.000,0.000,0.000' },
                                    ['Backstage_Ceiling']          = { ['default'] = {}, ['name'] = 'VIP Lounge - Ceiling', ['options'] = { { id = 1 },{ id = 2 },{ id = 3 },{ id = 4 },{ id = 5 },{ id = 6 },{ id = 7 },{ id = 8 },{ id = 9 },{ id = 10 } }, pos = '-3.564,2.697,-27.458,0.000', look = '90.000,30.000,0.000,0.000' },
                                    ['Backstage_Floor']            = { ['default'] = {}, ['name'] = 'VIP Lounge - Floor', ['options'] = { { id = 1 },{ id = 2 },{ id = 3 },{ id = 4 },{ id = 5 },{ id = 6 },{ id = 7 },{ id = 8 },{ id = 9 },{ id = 10 } }, pos = '10.324,4.934,-26.093,0.000', look = '-80.000,-10.000,0.000,0.000' },
                                    ['Backstage_Tassles']          = { ['default'] = {}, ['name'] = 'VIP Lounge - Lightshade', ['options'] = { { id = 1 },{ id = 2 },{ id = 3 },{ id = 4 },{ id = 5 },{ id = 6 },{ id = 7 },{ id = 8 },{ id = 9 },{ id = 10 } }, pos = '11.521,3.411,-31.475,0.000',look = '-100.000,0.000,0.000,0.000' },
                                    ['Backstage_wood_dark_veneer'] = { ['default'] = {}, ['name'] = 'VIP Lounge - Paneling', ['options'] = { { id = 1 },{ id = 2 },{ id = 3 },{ id = 4 },{ id = 5 },{ id = 6 },{ id = 7 },{ id = 8 },{ id = 9 },{ id = 10 } }, pos = '5.802,3.411,-26.511,0.000', look = '-91.000,0.000,0.000,0.000' },
                                    ['Backstage_wood_dark_veneer2']= { ['default'] = {}, ['name'] = 'VIP Lounge - Front Wall', ['options'] = { { id = 1 },{ id = 2 },{ id = 3 },{ id = 4 },{ id = 5 },{ id = 6 },{ id = 7 },{ id = 8 },{ id = 9 },{ id = 10 } }, pos = '-4.359,4.077,-25.937,0.000', look = '0.000,0.000,0.000,0.000' }
                                }
                            }
                        }
				    }

                    return XmlConvert.LuaToXml(TableFromInput, 'lua', 1)
                    "));
            });

            server.Routes.PostAuthentication.Parameter.Add(HttpMethod.GET, "/static/Lockwood/Features/Venue/{scenetype}/{build}/{country}/features.xml", async (ctx) =>
            {
                ctx.Response.StatusCode = (int)HttpStatusCode.OK;
                ctx.Response.ContentType = "text/xml";
                string xmlPath = $"/static/Lockwood/Features/Venue/{ctx.Request.Url.Parameters["scenetype"]}/{ctx.Request.Url.Parameters["build"]}/{ctx.Request.Url.Parameters["country"]}/features.xml";
                string filePath = !ApacheNetServerConfiguration.DomainFolder ? ApacheNetServerConfiguration.HTTPStaticFolder + xmlPath : ApacheNetServerConfiguration.HTTPStaticFolder + $"/{ctx.Request.RetrieveHeaderValue("Host")}" + xmlPath;

                if (File.Exists(filePath))
                {
                    await ctx.Response.Send(File.ReadAllText(filePath));
                    return;
                }

                LoggerAccessor.LogDebug($"[PostAuthParameters] - features data was not found for the Venue, falling back to server file.");

                await ctx.Response.Send(WebAPIService.OHS.LUA2XmlProcessor.TransformLuaTableToXml(@"
                            local TableFromInput = {
                                ['commercePoints'] = {
                                    default = true
                                },
                                ['Logos'] = {
                                    cloverleaf = true,
                                    drey = true,
                                    delirious_squid = true,
                                    figment = true,
                                    fool_throttle = true,
                                    heart = true,
                                    iron_fusion = true,
                                    lkwd = true,
                                    lkwd_venue = true,
                                    medusa = true,
                                    spiral = true,
                                    star = true,
                                    stitchkin = true,
                                    unicorn = true,
                                    halloween = true
                                },
                                ['Stagertron'] = {
                                     default = true
                                },
                                ['Runwaytron'] = {
                                    dancefloor = true,
                                    stage = true,
                                    default = true,
                                },
                                ['Customisation'] = {
                                    default = true
                                },
                                ['Posertrons'] = {
                                    default = true,
                                },
                                ['BackstagePass'] = {
                                    default = true,
                                },
                                ['Music'] = {
                                    default = true,
                                    Stitchkins = true,
                                    Nightclub = true,
                                    IronFusion = true,
                                    LKWD = true,
                                    Medusa = true,
                                    Corporate = true,
                                    Drey = true,
                                    Wings = true,
                                    Fool_Throttle = true,
                                    Delirious_Squid = true,
                                },
				            }

                            return XmlConvert.LuaToXml(TableFromInput, 'lua', 1)
                            "));
            });

            server.Routes.PostAuthentication.Parameter.Add(HttpMethod.GET, "/static/Lockwood/Features/Venue/{scenetype}/{build}/{country}/camPath.xml", async (ctx) =>
            {
                ctx.Response.StatusCode = (int)HttpStatusCode.OK;
                ctx.Response.ContentType = "text/xml";
                string xmlPath = $"/static/Lockwood/Features/Venue/{ctx.Request.Url.Parameters["scenetype"]}/{ctx.Request.Url.Parameters["build"]}/{ctx.Request.Url.Parameters["country"]}/camPath.xml";
                string filePath = !ApacheNetServerConfiguration.DomainFolder ? ApacheNetServerConfiguration.HTTPStaticFolder + xmlPath : ApacheNetServerConfiguration.HTTPStaticFolder + $"/{ctx.Request.RetrieveHeaderValue("Host")}" + xmlPath;

                if (File.Exists(filePath))
                {
                    await ctx.Response.Send(File.ReadAllText(filePath));
                    return;
                }

                LoggerAccessor.LogDebug($"[PostAuthParameters] - camPath data was not found for the Venue, falling back to server file.");

                await ctx.Response.Send(WebAPIService.OHS.LUA2XmlProcessor.TransformLuaTableToXml(@"
                            local TableFromInput = {
                                origin = {
                                    name = 'origin',
                                    pos = '1.981,3.681,11.353,0.000',
                                    links = {""door"", ""bar"", ""runway""}
                                },
                                door = {
                                    name = 'door',
                                    pos = '5.120,1.892,20.704,0.000',
                                    links = {""scene_enter"", ""origin""}
                                },
                                scene_enter = {
                                    name = 'scene_enter',
                                    pos = '13.135,7.156,41.475,0.000',
                                    links = {""door""}
                                },
                                bar = {
                                    name = 'bar',
                                    pos = '-11.973,4.501,10.478,0.000',
                                    links = {""origin"", ""bar_up"", ""backstage"", ""runway""}
                                },
                                bar_up = {
                                    name = 'bar_up',
                                    pos = '-3.557,6.592,-4.290,0.000',
                                    links = {""origin"", ""bar"", ""backstage"", ""runway""}
                                },
                                runway = {
                                    name = 'runway',
                                    pos = '8.762,2.923,-5.979,0.000',
                                    links = {""backstage"", ""bar"", ""bar_up"", ""origin""}
                                },
                                backstage = {
                                    name = 'backstage',
                                    pos = '13.818,2.864,-20.113,0.000',
                                    links = {""backstage_enter"", ""bar"", ""bar_up"", ""runway""}
                                },
                                backstage_enter = {
                                    name = 'backstage_enter',
                                    pos = '10.109,2.864,-22.998,0.000',
                                    links = {""backstage""}
                                }
                            }

                            return XmlConvert.LuaToXml(TableFromInput, 'lua', 1)
                            "));
            });

            server.Routes.PostAuthentication.Parameter.Add(HttpMethod.GET, "/static/Lockwood/Features/Venue/{scenetype}/{build}/{country}/{group_def}/{profile_def}", async (ctx) =>
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
                string xmlPath = $"/static/Lockwood/Features/Venue/{ctx.Request.Url.Parameters["scenetype"]}/{ctx.Request.Url.Parameters["build"]}/{ctx.Request.Url.Parameters["country"]}/{group_def}/{profile_def}";
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
                string filePath = !ApacheNetServerConfiguration.DomainFolder ? ApacheNetServerConfiguration.HTTPStaticFolder + xmlPath : ApacheNetServerConfiguration.HTTPStaticFolder + $"/{ctx.Request.RetrieveHeaderValue("Host")}" + xmlPath;

                if (File.Exists(filePath))
                {
                    await ctx.Response.Send(File.ReadAllText(filePath));
                    return;
                }

                LoggerAccessor.LogDebug($"[PostAuthParameters] - {profile_def} data was not found for the Venue, falling back to server file.");

                switch (group_def)
                {
                    case "Stagertron":
                        await ctx.Response.Send($@"<lua>
	                                <feature_com_context type='num'>{UniqueIDCounter.CreateUniqueID()}</feature_com_context>
                                    <applet>
		                                    <StageEnter_def></StageEnter_def>
                                            <StageExit_def></StageExit_def>
		                                    <Cameratron_def></Cameratron_def>
		                                    <Screenatron_def></Screenatron_def>
		                                    <Stagertron_def></Stagertron_def>
	                                </applet>
                                    <detectors>
                                        <detectorsStageEnter_def></detectorsStageEnter_def>
                                        <detectorsStageExit_def></detectorsStageExit_def>
	                                </detectors>
	                                <feature_root>
                                           <root_cam>
			                                    <rot type='vec'>0,0,0,0</rot>
                                                <scale type='vec'>0,0,0,0</scale>
                                                <pos type='vec'>0,0,0,0</pos>
                                                <applet>
			                                        <name>Cameratron_def</name>
                                                    <override>
                                                        <appletId>Cameratron_Applet</appletId>
                                                        <register>AppletRegister_cameratron.lua</register>
                                                        <params>
                                                        </params>
                                                    </override>
		                                        </applet>
		                                    </root_cam>
                                            <root_screen>
			                                    <rot type='vec'>0,-90,0,-90</rot>
                                                <scale type='vec'>0.8,1.6,1,1</scale>
                                                <pos type='vec'>16.360,4.748,2.837,0</pos>
                                                <applet>
			                                        <name>Screenatron_def</name>
                                                    <override>
                                                        <appletId>Screenatron_Applet</appletId>
                                                        <register>AppletRegister_screenatron.lua</register>
                                                        <params>
                                                            <scale type='num'>2</scale>
                                                            <mode>multiple</mode>
                                                        </params>
                                                    </override>
		                                        </applet>
		                                    </root_screen>
                                            <root_screen_large_0>
			                                    <rot type='vec'>0,-90,0,-90</rot>
                                                <scale type='vec'>0.8,1.6,1,1</scale>
                                                <pos type='vec'>16.360,4.748,-6.308,0</pos>
                                                <applet>
			                                        <name>Screenatron_def</name>
                                                    <override>
                                                        <appletId>Screenatron_Applet</appletId>
                                                        <register>AppletRegister_screenatron.lua</register>
                                                        <params>
                                                            <scale type='num'>2</scale>
                                                            <mode>multiple</mode>
                                                        </params>
                                                    </override>
		                                        </applet>
		                                    </root_screen_large_0>
                                            <root_screen_large_1>
			                                    <rot type='vec'>0,-90,0,-90</rot>
                                                <scale type='vec'>0.8,1.6,1,1</scale>
                                                <pos type='vec'>16.360,4.748,11.750,0</pos>
                                                <applet>
			                                        <name>Screenatron_def</name>
                                                    <override>
                                                        <appletId>Screenatron_Applet</appletId>
                                                        <register>AppletRegister_screenatron.lua</register>
                                                        <params>
                                                            <scale type='num'>2</scale>
                                                            <mode>multiple</mode>
                                                        </params>
                                                    </override>
		                                        </applet>
		                                    </root_screen_large_1>
                                            <root_screen_0>
			                                    <rot type='vec'>0,0,0,0</rot>
                                                <scale type='vec'>0.7,1.1,1,1</scale>
                                                <pos type='vec'>-4.606,3.940,-18.917,0</pos>
                                                <applet>
			                                        <name>Screenatron_def</name>
                                                    <override>
                                                        <appletId>Screenatron_Applet</appletId>
                                                        <register>AppletRegister_screenatron.lua</register>
                                                        <params>
                                                            <scale type='num'>3</scale>
                                                            <voterId type='num'>1</voterId>
                                                            <mode>single</mode>
                                                        </params>
                                                    </override>
		                                        </applet>
		                                    </root_screen_0>
                                            <root_screen_1>
			                                    <rot type='vec'>0,0,0,0</rot>
                                                <scale type='vec'>0.7,1.1,1,1</scale>
                                                <pos type='vec'>8.987,3.940,-18.917,0</pos>
                                                <applet>
			                                        <name>Screenatron_def</name>
                                                    <override>
                                                        <appletId>Screenatron_Applet</appletId>
                                                        <register>AppletRegister_screenatron.lua</register>
                                                        <params>
                                                            <scale type='num'>3</scale>
                                                            <voterId type='num'>3</voterId>
                                                            <mode>single</mode>
                                                        </params>
                                                    </override>
		                                        </applet>
		                                    </root_screen_1>
                                            <!--<root_screen_2>
			                                    <rot type='vec'>0,0,0,0</rot>
                                                <scale type='vec'>0.5,1.1,1,1</scale>
                                                <pos type='vec'>2.128,3.940,-21.718,0</pos>
                                                <applet>
			                                        <name>Screenatron_def</name>
                                                    <override>
                                                        <appletId>Screenatron_Applet</appletId>
                                                        <register>AppletRegister_screenatron.lua</register>
                                                        <params>
                                                            <scale type='num'>3</scale>
                                                            <voterId type='num'>2</voterId>
                                                            <mode>single</mode>
                                                        </params>
                                                    </override>
		                                        </applet>
		                                    </root_screen_2>-->
                                            <root_stage>
			                                    <rot type='vec'>0,0,0,0</rot>
                                                <scale type='vec'>0,0,0,0</scale>
                                                <pos type='vec'>0,0,0,0</pos>
                                                <applet>
			                                        <name>Stagertron_def</name>
                                                    <override>
                                                        <appletId>Stagertron_Applet</appletId>
                                                        <register>AppletRegister_stagertron.lua</register>
                                                        <params>
                                                        </params>
                                                    </override>
		                                        </applet>
		                                    </root_stage>
                                            <root_stage_enter>
                                                <rot type='vec'>0,0,0,0</rot>
                                                <scale type='vec'>0,0,0,0</scale>
                                                <pos type='vec'>7.617,1.532,-20.761,0</pos>
                                                <applet>
                                                    <name>StageEnter_def</name>
                                                    <override>
                                                        <appletId>StageAccess_Applet</appletId>
                                                        <register>AppletRegister_StageAccess.lua</register>
                                                        <params>
                                                            <destination>stage_enter_dest</destination>
                                                            <mode>Entrance</mode>
                                                        </params>
                                                    </override>
                                                </applet>
                                                <detectors>
                                                        <name>detectorsStageEnter_def</name>
                                                        <override>
                                                            <proximity>50000</proximity>
                                                            <homeTarget>
                                                                <localisation>STAGE_ENTER</localisation>
                                                                 <radius type='num'>1.2</radius>
                                                            </homeTarget>
                                                        </override>
                                                </detectors>
                                            </root_stage_enter>
                                            <root_stage_exit>
                                                <rot type='vec'>0,0,0,0</rot>
                                                <scale type='vec'>0,0,0,0</scale>
                                                <pos type='vec'>6.907,1.532,-20.761,0</pos>
                                                <applet>
                                                    <name>StageEnter_def</name>
                                                    <override>
                                                        <appletId>StageAccess_Applet</appletId>
                                                        <register>AppletRegister_StageAccess.lua</register>
                                                        <params>
                                                            <destination>stage_exit_dest</destination>
                                                            <mode>Exit</mode>
                                                        </params>
                                                    </override>
                                                </applet>
                                                <detectors>
                                                        <name>detectorsStageExit_def</name>
                                                        <override>
                                                            <proximity>50000</proximity>
                                                            <homeTarget>
                                                                <localisation>STAGE_LEAVE</localisation>
                                                                <radius type='num'>1.2</radius>
                                                            </homeTarget>
                                                        </override>
                                                </detectors>
                                        </root_stage_exit>
                                        <stage_enter_dest>
                                                <rot type='vec'>0,0,0,0</rot>
                                                <scale type='vec'>0,0,0,0</scale>
                                                <pos type='vec'>6.766,1.922,-20.612,0</pos>
                                        </stage_enter_dest>
                                        <stage_exit_dest>
                                                <rot type='vec'>0,0,0,0</rot>
                                                <scale type='vec'>0,0,0,0</scale>
                                                <pos type='vec'>10.369,1.922,-20.612,0</pos>
                                        </stage_exit_dest>
	                                </feature_root>
                                </lua>");
                        return;
                    case "BackstagePass":
                        await ctx.Response.Send($@"<lua>
	                                <feature_com_context type='num'>{UniqueIDCounter.CreateUniqueID()}</feature_com_context>
                                    <entityGroup>
		                                <EntityGroup_def></EntityGroup_def>
	                                </entityGroup>
	                                <feature_root>
                                        <root_block>
			                                <rot type='vec'>0,0,0,0</rot>
                                            <scale type='vec'>1,1,1,1</scale>
                                            <pos type='vec'>7.731,1.479,-20.632,0</pos>
                                            <entityGroup>
			                                    <name>EntityGroup_def</name>
                                                <override>
                                                    <entities>
                                                        <_>
                                                            <mdl>rope.mdl</mdl>
                                                            <hkx>rope.hkx</hkx>
                                                        </_>
                                                    </entities>
                                                </override>
		                                    </entityGroup>
		                                </root_block>
                                        <root_block_0>
			                                <rot type='vec'>0,0,0,0</rot>
                                            <scale type='vec'>1,1,1,1</scale>
                                            <pos type='vec'>-3.190,1.479,-20.632,0</pos>
                                            <entityGroup>
			                                    <name>EntityGroup_def</name>
                                                <override>
                                                    <entities>
                                                        <_>
                                                            <mdl>rope.mdl</mdl>
                                                            <hkx>rope.hkx</hkx>
                                                        </_>
                                                    </entities>
                                                </override>
		                                    </entityGroup>
		                                </root_block_0>
	                                </feature_root>
                                </lua>");
                        return;
                    case "Customisation":
                        await ctx.Response.Send($@"<lua>
	                                    <feature_com_context type='num'>{UniqueIDCounter.CreateUniqueID()}</feature_com_context>
                                        <applet>
		                                    <Customisation_def></Customisation_def>
	                                    </applet>
                                        <entityGroup>
                                            <EntityGroup_def></EntityGroup_def>
                                        </entityGroup>
                                        <detectors>
		                                    <detectorsCustomisation_def></detectorsCustomisation_def>
	                                    </detectors>
	                                    <feature_root>
                                            <root>
			                                    <rot type='vec'>0,0,0,0</rot>
                                                <scale type='vec'>0,0,0,0</scale>
                                                <pos type='vec'>-2.366,1.790,16.434,0</pos>
                                                <applet>
			                                        <name>Customisation_def</name>
                                                    <override>
                                                        <appletId>ProfileCustomisation_Applet</appletId>
                                                        <register>AppletRegister_ProfileCustomisation.lua</register>
                                                        <params>
                                                            <url>generic_cp</url>
                                                        </params>
                                                    </override>
		                                        </applet>
                                                <detectors>
			                                        <name>detectorsCustomisation_def</name>
                                                    <override>
                                                        <proximity>50000</proximity>
                                                        <homeTarget>
                                                            <localisation>CUSTOMISE</localisation>
                                                            <radius type='num'>2.5</radius>
                                                        </homeTarget>
                                                    </override>
		                                        </detectors>
		                                    </root>
                                            <root_logo>
			                                <rot type='vec'>0,90,0,90</rot>
                                            <scale type='vec'>3.7,3.7,3.7,3.7</scale>
                                            <pos type='vec'>-11.215,6.779,18.593,0</pos>
                                            <entityGroup>
			                                    <name>EntityGroup_def</name>
                                                <override>
                                                    <entities>
                                                        <_>
                                                            <mdl>lkwd_venue.mdl</mdl>
                                                        </_>
                                                    </entities>
                                                </override>
		                                    </entityGroup>
		                                </root_logo>
	                                    </feature_root>
                                    </lua>");
                        return;
                    case "Runwaytron":
                        if (profile_def == "default")
                        {
                            await ctx.Response.Send($@"<lua>
	                                <feature_com_context type='num'>{UniqueIDCounter.CreateUniqueID()}</feature_com_context>
                                    <applet>
		                                <Strutertron_def></Strutertron_def>
		                                <Votertron_def></Votertron_def>
		                                <Votertron_0_def></Votertron_0_def>
		                                <Votertron_1_def></Votertron_1_def>
	                                </applet>
                                    <entityGroup>
                                        <EntityGroup_def></EntityGroup_def>
                                    </entityGroup>
                                    <detectors>
		                                    <detectorsStrut_def></detectorsStrut_def>
		                                    <detectorsVotertron_def></detectorsVotertron_def>
		                                    <detectorsVotertron_0_def></detectorsVotertron_0_def>
		                                    <detectorsVotertron_1_def></detectorsVotertron_1_def>
	                                </detectors>
	                                <feature_root>
                                        <root_catwalk>
			                                <rot type='vec'>0,0,0,0</rot>
                                            <scale type='vec'>1,1,1,1</scale>
                                            <pos type='vec'>0,0,0,0</pos>
                                            <entityGroup>
			                                    <name>EntityGroup_def</name>
                                                <override>
                                                    <entities>
                                                        <_>
                                                            <aniSpd>2.5</aniSpd>
                                                            <ani>set_dressing.ani</ani>
                                                            <mdl>catwalk.mdl</mdl>
                                                            <hkx>catwalk.hkx</hkx>
                                                        </_>
                                                    </entities>
                                                </override>
		                                    </entityGroup>
		                                </root_catwalk>
                                        <root_judge>
			                                <rot type='vec'>0,0,0,0</rot>
                                            <scale type='vec'>1,1,1,1</scale>
                                            <pos type='vec'>2,0,8.077,0</pos>
                                            <entityGroup>
			                                    <name>EntityGroup_def</name>
                                                <override>
                                                    <entities>
                                                        <_>
                                                            <aniSpd>2.5</aniSpd>
                                                            <ani>set_dressing.ani</ani>
                                                            <mdl>judges.mdl</mdl>
                                                            <hkx>judges.hkx</hkx>
                                                        </_>
                                                    </entities>
                                                </override>
		                                    </entityGroup>
		                                </root_judge>
                                        <root_judge_0>
			                                <rot type='vec'>0,0,0,0</rot>
                                            <scale type='vec'>1,1,1,1</scale>
                                            <pos type='vec'>-2.863,0.169,-6.622,0</pos>
                                            <entityGroup>
			                                    <name>EntityGroup_def</name>
                                                <override>
                                                    <entities>
                                                        <_>
                                                            <aniSpd>2.5</aniSpd>
                                                            <ani>set_dressing.ani</ani>
                                                            <mdl>bench.mdl</mdl>
                                                            <hkx>bench.hkx</hkx>
                                                        </_>
                                                    </entities>
                                                </override>
		                                    </entityGroup>
		                                </root_judge_0>
                                        <root_judge_1>
			                                <rot type='vec'>0,0,0,0</rot>
                                            <scale type='vec'>1,1,1,1</scale>
                                            <pos type='vec'>7.117,0.169,-6.622,0</pos>
                                            <entityGroup>
			                                    <name>EntityGroup_def</name>
                                                <override>
                                                    <entities>
                                                        <_>
                                                            <aniSpd>2.5</aniSpd>
                                                            <ani>set_dressing.ani</ani>
                                                            <mdl>bench.mdl</mdl>
                                                            <hkx>bench.hkx</hkx>
                                                        </_>
                                                    </entities>
                                                </override>
		                                    </entityGroup>
		                                </root_judge_1>
                                        <root_struter>
			                                <rot type='vec'>0,0,0,0</rot>
                                            <scale type='vec'>0,0,0,0</scale>
                                            <pos type='vec'>-3.960,1.479,-20.632,0</pos>
                                            <applet>
			                                    <name>Strutertron_def</name>
                                                <override>
                                                    <appletId>Strutertron_Applet</appletId>
                                                    <register>AppletRegister_strutertron.lua</register>
                                                    <params>
                                                        <malePos type='vec'>1.396,1.000,-20.253,0</malePos>
                                                        <femalePos type='vec'>1.396,1.000,-19.087,0</femalePos>
                                                        <destination>catwalk_exit_dest</destination>
                                                        <behavior>WalkTheWalk</behavior>
                                                        <female>votertron_female</female>
                                                        <male>votertron_male</male>
                                                    </params>
                                                </override>
		                                    </applet>
                                            <detectors>
			                                        <name>detectorsStrut_def</name>
                                                    <override>
                                                        <proximity>50000</proximity>
                                                        <homeTarget>
                                                            <localisation>VOTERTRON_JOIN_AS_POSER</localisation>
                                                            <radius type='num'>1.2</radius>
                                                        </homeTarget>
                                                    </override>
		                                    </detectors>
		                                </root_struter>
                                        <root_vote>
			                                <rot type='vec'>0,180,0,180</rot>
                                            <scale type='vec'>1,1,1,1</scale>
                                            <pos type='vec'>0.670,0.501,9.141,0</pos>
                                            <applet>
			                                    <name>Votertron_def</name>
                                                <override>
                                                    <appletId>Votertron_Applet</appletId>
                                                    <register>AppletRegister_Votertron.lua</register>
                                                    <params>
                                                        <male type='vec'>0,0,0,0</male>
                                                        <female type='vec'>0,0,0,0</female>
                                                        <voterId type='num'>1</voterId>
                                                    </params>
                                                </override>
		                                    </applet>
                                            <detectors>
			                                        <name>detectorsVotertron_def</name>
                                                    <override>
                                                        <proximity>50000</proximity>
                                                        <homeTarget>
                                                            <localisation>VOTERTRON_JOIN_AS_VOTER</localisation>
                                                            <radius type='num'>0.8</radius>
                                                        </homeTarget>
                                                    </override>
		                                    </detectors>
		                                </root_vote>
                                        <root_vote_0>
			                                <rot type='vec'>0,180,0,180</rot>
                                            <scale type='vec'>1,1,1,1</scale>
                                            <pos type='vec'>1.944,0.501,9.141,0</pos>
                                            <applet>
			                                    <name>Votertron_0_def</name>
                                                <override>
                                                    <appletId>Votertron_Applet</appletId>
                                                    <register>AppletRegister_Votertron.lua</register>
                                                    <params>
                                                        <male type='vec'>0,0,0,0</male>
                                                        <female type='vec'>0,0,0,0</female>
                                                        <voterId type='num'>2</voterId>
                                                    </params>
                                                </override>
		                                    </applet>
                                            <detectors>
			                                        <name>detectorsVotertron_0_def</name>
                                                    <override>
                                                        <proximity>50000</proximity>
                                                        <homeTarget>
                                                            <localisation>VOTERTRON_JOIN_AS_VOTER</localisation>
                                                            <radius type='num'>0.8</radius>
                                                        </homeTarget>
                                                    </override>
		                                    </detectors>
		                                </root_vote_0>
                                        <root_vote_1>
			                                <rot type='vec'>0,180,0,180</rot>
                                            <scale type='vec'>1,1,1,1</scale>
                                            <pos type='vec'>3.191,0.501,9.141,0</pos>
                                            <applet>
			                                    <name>Votertron_1_def</name>
                                                <override>
                                                    <appletId>Votertron_Applet</appletId>
                                                    <register>AppletRegister_Votertron.lua</register>
                                                    <params>
                                                        <male type='vec'>0,0,0,0</male>
                                                        <female type='vec'>0,0,0,0</female>
                                                        <voterId type='num'>3</voterId>
                                                    </params>
                                                </override>
		                                    </applet>
                                            <detectors>
			                                        <name>detectorsVotertron_1_def</name>
                                                    <override>
                                                        <proximity>50000</proximity>
                                                        <homeTarget>
                                                            <localisation>VOTERTRON_JOIN_AS_VOTER</localisation>
                                                            <radius type='num'>0.8</radius>
                                                        </homeTarget>
                                                    </override>
		                                    </detectors>
		                                </root_vote_1>
                                        <catwalk_exit_dest>
			                                    <rot type='vec'>0,0,0,0</rot>
                                                <scale type='vec'>0,0,0,0</scale>
                                                <pos type='vec'>-4.589,1.849,-20.478,0</pos>
		                                </catwalk_exit_dest>
	                                </feature_root>
                                </lua>");
                            return;
                        }
                        else if (profile_def == "stage")
                        {
                            await ctx.Response.Send($@"<lua>
	                                <feature_com_context type='num'>{UniqueIDCounter.CreateUniqueID()}</feature_com_context>
                                    <applet>
		                                <Votertron_def></Votertron_def>
		                                <Votertron_0_def></Votertron_0_def>
		                                <Votertron_1_def></Votertron_1_def>
	                                </applet>
                                    <entityGroup>
                                        <EntityGroup_def></EntityGroup_def>
                                    </entityGroup>
                                    <detectors>
		                                    <detectorsVotertron_def></detectorsVotertron_def>
		                                    <detectorsVotertron_0_def></detectorsVotertron_0_def>
		                                    <detectorsVotertron_1_def></detectorsVotertron_1_def>
	                                </detectors>
	                                <feature_root>
                                        <root_judge>
			                                <rot type='vec'>0,0,0,0</rot>
                                            <scale type='vec'>1,1,1,1</scale>
                                            <pos type='vec'>2,0,-9.077,0</pos>
                                            <entityGroup>
			                                    <name>EntityGroup_def</name>
                                                <override>
                                                    <entities>
                                                        <_>
                                                            <aniSpd>2.5</aniSpd>
                                                            <ani>set_dressing.ani</ani>
                                                            <mdl>judges.mdl</mdl>
                                                            <hkx>judges.hkx</hkx>
                                                        </_>
                                                    </entities>
                                                </override>
		                                    </entityGroup>
		                                </root_judge>
                                        <root_judge_0>
			                                <rot type='vec'>0,90,0,90</rot>
                                            <scale type='vec'>1,1,1,1</scale>
                                            <pos type='vec'>2,0.169,-1.414,0</pos>
                                            <entityGroup>
			                                    <name>EntityGroup_def</name>
                                                <override>
                                                    <entities>
                                                        <_>
                                                            <aniSpd>2.5</aniSpd>
                                                            <ani>set_dressing.ani</ani>
                                                            <mdl>bench.mdl</mdl>
                                                            <hkx>bench.hkx</hkx>
                                                        </_>
                                                    </entities>
                                                </override>
		                                    </entityGroup>
		                                </root_judge_0>
                                        <root_judge_1>
			                                <rot type='vec'>0,90,0,90</rot>
                                            <scale type='vec'>1,1,1,1</scale>
                                            <pos type='vec'>2,0.169,1.414,0</pos>
                                            <entityGroup>
			                                    <name>EntityGroup_def</name>
                                                <override>
                                                    <entities>
                                                        <_>
                                                            <aniSpd>2.5</aniSpd>
                                                            <ani>set_dressing.ani</ani>
                                                            <mdl>bench.mdl</mdl>
                                                            <hkx>bench.hkx</hkx>
                                                        </_>
                                                    </entities>
                                                </override>
		                                    </entityGroup>
		                                </root_judge_1>
                                        <root_judge_2>
			                                <rot type='vec'>0,90,0,90</rot>
                                            <scale type='vec'>1,1,1,1</scale>
                                            <pos type='vec'>2,0.169,4.414,0</pos>
                                            <entityGroup>
			                                    <name>EntityGroup_def</name>
                                                <override>
                                                    <entities>
                                                        <_>
                                                            <aniSpd>2.5</aniSpd>
                                                            <ani>set_dressing.ani</ani>
                                                            <mdl>bench.mdl</mdl>
                                                            <hkx>bench.hkx</hkx>
                                                        </_>
                                                    </entities>
                                                </override>
		                                    </entityGroup>
		                                </root_judge_2>
                                        <root_vote>
			                                <rot type='vec'>0,180,0,180</rot>
                                            <scale type='vec'>1,1,1,1</scale>
                                            <pos type='vec'>0.670,0.501,-8.021,0</pos>
                                            <applet>
			                                    <name>Votertron_def</name>
                                                <override>
                                                    <appletId>Votertron_Applet</appletId>
                                                    <register>AppletRegister_Votertron.lua</register>
                                                    <params>
                                                        <male type='vec'>0,0,0,0</male>
                                                        <female type='vec'>0,0,0,0</female>
                                                        <voterId type='num'>1</voterId>
                                                    </params>
                                                </override>
		                                    </applet>
                                            <detectors>
			                                        <name>detectorsVotertron_def</name>
                                                    <override>
                                                        <proximity>50000</proximity>
                                                        <homeTarget>
                                                            <localisation>VOTERTRON_JOIN_AS_VOTER</localisation>
                                                            <radius type='num'>0.8</radius>
                                                        </homeTarget>
                                                    </override>
		                                    </detectors>
		                                </root_vote>
                                        <root_vote_0>
			                                <rot type='vec'>0,180,0,180</rot>
                                            <scale type='vec'>1,1,1,1</scale>
                                            <pos type='vec'>1.944,0.501,-8.021,0</pos>
                                            <applet>
			                                    <name>Votertron_0_def</name>
                                                <override>
                                                    <appletId>Votertron_Applet</appletId>
                                                    <register>AppletRegister_Votertron.lua</register>
                                                    <params>
                                                        <male type='vec'>0,0,0,0</male>
                                                        <female type='vec'>0,0,0,0</female>
                                                        <voterId type='num'>2</voterId>
                                                    </params>
                                                </override>
		                                    </applet>
                                            <detectors>
			                                        <name>detectorsVotertron_0_def</name>
                                                    <override>
                                                        <proximity>50000</proximity>
                                                        <homeTarget>
                                                            <localisation>VOTERTRON_JOIN_AS_VOTER</localisation>
                                                            <radius type='num'>0.8</radius>
                                                        </homeTarget>
                                                    </override>
		                                    </detectors>
		                                </root_vote_0>
                                        <root_vote_1>
			                                <rot type='vec'>0,180,0,180</rot>
                                            <scale type='vec'>1,1,1,1</scale>
                                            <pos type='vec'>3.191,0.501,-8.021,0</pos>
                                            <applet>
			                                    <name>Votertron_1_def</name>
                                                <override>
                                                    <appletId>Votertron_Applet</appletId>
                                                    <register>AppletRegister_Votertron.lua</register>
                                                    <params>
                                                        <male type='vec'>0,0,0,0</male>
                                                        <female type='vec'>0,0,0,0</female>
                                                        <voterId type='num'>3</voterId>
                                                    </params>
                                                </override>
		                                    </applet>
                                            <detectors>
			                                        <name>detectorsVotertron_1_def</name>
                                                    <override>
                                                        <proximity>50000</proximity>
                                                        <homeTarget>
                                                            <localisation>VOTERTRON_JOIN_AS_VOTER</localisation>
                                                            <radius type='num'>0.8</radius>
                                                        </homeTarget>
                                                    </override>
		                                    </detectors>
		                                </root_vote_1>
	                                </feature_root>
                                </lua>");
                            return;
                        }
                        else if (profile_def == "dancefloor")
                        {
                            await ctx.Response.Send($@"<lua>
	                                <feature_com_context type='num'>{UniqueIDCounter.CreateUniqueID()}</feature_com_context>
                                    <applet>
		                               <Spotlight_def></Spotlight_def>
	                                </applet>
	                                <feature_root>
                                        <root_spot>
			                                <rot type='vec'>0,0,-90,0</rot>
                                            <scale type='vec'>1,1,1,1</scale>
                                            <pos type='vec'>-0.249,5.610,5.203,0</pos>
                                            <applet>
			                                    <name>Spotlight_def</name>
                                                <override>
                                                    <appletId>Spotlight_Applet</appletId>
                                                    <register>AppletRegister_Spotlight.lua</register>
                                                    <params>
                                                        <mdl>Animated_Light.mdl</mdl>
                                                        <skn>Animated_Light.skn</skn>
                                                        <ani>Animated_Light.ani</ani>
                                                        <aniSpd>1.5</aniSpd>
                                                        <intensity type='vec'>1,1,1,1</intensity>
                                                    </params>
                                                </override>
		                                    </applet>
		                                </root_spot>
                                        <root_spot_0>
			                                <rot type='vec'>0,0,-90,0</rot>
                                            <scale type='vec'>1,1,1,1</scale>
                                            <pos type='vec'>4.278,5.610,5.203,0</pos>
                                            <applet>
			                                    <name>Spotlight_def</name>
                                                <override>
                                                    <appletId>Spotlight_Applet</appletId>
                                                    <register>AppletRegister_Spotlight.lua</register>
                                                    <params>
                                                        <mdl>Animated_Light.mdl</mdl>
                                                        <skn>Animated_Light.skn</skn>
                                                        <ani>Animated_Light.ani</ani>
                                                        <aniSpd>1.5</aniSpd>
                                                        <intensity type='vec'>1,1,1,1</intensity>
                                                    </params>
                                                </override>
		                                    </applet>
		                                </root_spot_0>
                                        <root_spot_1>
			                                <rot type='vec'>0,0,-90,0</rot>
                                            <scale type='vec'>1,1,1,1</scale>
                                            <pos type='vec'>-0.249,5.610,-2.117,0</pos>
                                            <applet>
			                                    <name>Spotlight_def</name>
                                                <override>
                                                    <appletId>Spotlight_Applet</appletId>
                                                    <register>AppletRegister_Spotlight.lua</register>
                                                    <params>
                                                        <mdl>Animated_Light.mdl</mdl>
                                                        <skn>Animated_Light.skn</skn>
                                                        <ani>Animated_Light.ani</ani>
                                                        <aniSpd>1.5</aniSpd>
                                                        <intensity type='vec'>1,1,1,1</intensity>
                                                    </params>
                                                </override>
		                                    </applet>
		                                </root_spot_1>
                                        <root_spot_2>
			                                <rot type='vec'>0,0,-90,0</rot>
                                            <scale type='vec'>1,1,1,1</scale>
                                            <pos type='vec'>4.278,5.610,-2.117,0</pos>
                                            <applet>
			                                    <name>Spotlight_def</name>
                                                <override>
                                                    <appletId>Spotlight_Applet</appletId>
                                                    <register>AppletRegister_Spotlight.lua</register>
                                                    <params>
                                                        <mdl>Animated_Light.mdl</mdl>
                                                        <skn>Animated_Light.skn</skn>
                                                        <ani>Animated_Light.ani</ani>
                                                        <aniSpd>1.5</aniSpd>
                                                        <intensity type='vec'>1,1,1,1</intensity>
                                                    </params>
                                                </override>
		                                    </applet>
		                                </root_spot_2>
	                                </feature_root>
                                </lua>");
                            return;
                        }
                        break;
                    case "Logos":
                        await ctx.Response.Send($@"<lua>
	                                <feature_com_context type='num'>{UniqueIDCounter.CreateUniqueID()}</feature_com_context>
                                    <entityGroup>
		                                <EntityGroup_def></EntityGroup_def>
	                                </entityGroup>
	                                <feature_root>
                                        <root_logo>
			                                <rot type='vec'>0,0,0,0</rot>
                                            <scale type='vec'>3.7,3.7,3.7,3.7</scale>
                                            <pos type='vec'>2.054,4.297,-21.509,0</pos>
                                            <entityGroup>
			                                    <name>EntityGroup_def</name>
                                                <override>
                                                    <entities>
                                                        <_>
                                                            <aniSpd>2.5</aniSpd>
                                                            <ani>set_dressing.ani</ani>
                                                            <mdl>{profile_def}.mdl</mdl>
                                                        </_>
                                                    </entities>
                                                </override>
		                                    </entityGroup>
		                                </root_logo>
                                        <root_logo_0>
			                                <rot type='vec'>0,0,0,0</rot>
                                            <scale type='vec'>2,2,2,2</scale>
                                            <pos type='vec'>-16.650,3.502,-10.845,0</pos>
                                            <entityGroup>
			                                    <name>EntityGroup_def</name>
                                                <override>
                                                    <entities>
                                                        <_>
                                                            <aniSpd>2.5</aniSpd>
                                                            <ani>set_dressing.ani</ani>
                                                            <mdl>{profile_def}.mdl</mdl>
                                                        </_>
                                                    </entities>
                                                </override>
		                                    </entityGroup>
		                                </root_logo_0>
                                        <root_logo_1>
			                                <rot type='vec'>0,0,0,0</rot>
                                            <scale type='vec'>2,2,2,2</scale>
                                            <pos type='vec'>-16.650,7.865,-10.845,0</pos>
                                            <entityGroup>
			                                    <name>EntityGroup_def</name>
                                                <override>
                                                    <entities>
                                                        <_>
                                                            <aniSpd>2.5</aniSpd>
                                                            <ani>set_dressing.ani</ani>
                                                            <mdl>{profile_def}.mdl</mdl>
                                                        </_>
                                                    </entities>
                                                </override>
		                                    </entityGroup>
		                                </root_logo_1>
	                                </feature_root>
                                </lua>");
                        return;
                    case "Music":
                        if (profile_def != "default")
                        {
                            await ctx.Response.Send($@"<lua>
	                                <feature_com_context type='num'>{UniqueIDCounter.CreateUniqueID()}</feature_com_context>
                                    <soundStream>
		                                <{profile_def}></{profile_def}>
		                                <{profile_def}_MUFFLED1></{profile_def}_MUFFLED1>
	                                </soundStream>
	                                <feature_root>
		                                <root>
			                                <rot type='vec'>0,0,0,0</rot>
                                            <scale type='vec'>0,0,0,0</scale>
                                            <pos type='vec'>2.197,2.390,27.509,0</pos>
                                            <soundStream>
			                                    <name>{profile_def}</name>
                                                <override>
                                                    <channel>{profile_def}_channel</channel>
                                                    <stream>stream_{profile_def}</stream>
                                                    <innerRadius type='num'>109</innerRadius>
                                                    <outerRadius type='num'>112</outerRadius>
                                                    <crossfadeTime type='num'>1.5</crossfadeTime>
                                                    <volume type='num'>0.8</volume>
                                                    <canTerminate type='bool'>false</canTerminate>
                                                    <mode>2D</mode>
                                                </override>
		                                    </soundStream>
		                                </root>
                                        <root_muffled>
			                                <rot type='vec'>0,0,0,0</rot>
                                            <scale type='vec'>0,0,0,0</scale>
                                            <pos type='vec'>3.806,3.435,-45.705,0</pos>
                                            <soundStream>
			                                    <name>{profile_def}_MUFFLED1</name>
                                                <override>
                                                    <channel>{profile_def}_MUFFLED1_channel</channel>
                                                    <stream>stream_{profile_def}_MUFFLED1</stream>
                                                    <innerRadius type='num'>24</innerRadius>
                                                    <outerRadius type='num'>24</outerRadius>
                                                    <crossfadeTime type='num'>1.5</crossfadeTime>
                                                    <volume type='num'>0.8</volume>
                                                    <canTerminate type='bool'>false</canTerminate>
                                                    <mode>2D</mode>
                                                </override>
		                                    </soundStream>
		                                </root_muffled>
	                                </feature_root>
                                </lua>");
                            return;
                        }
                        break;
                    case "Posertrons":
                        await ctx.Response.Send($@"<lua>
	                                <feature_com_context type='num'>{UniqueIDCounter.CreateUniqueID()}</feature_com_context>
                                    <applet>
		                                <Posertrons_V2></Posertrons_V2>
	                                </applet>
	                                <feature_root>
                                        <root_custom>
			                                <rot type='vec'>0,-208,0,-208</rot>
                                            <scale type='vec'>1,1,1,1</scale>
                                            <pos type='vec'>-2.596,1.790,16.674,0</pos>
                                            <applet>
			                                    <name>Posertrons_V2</name>
                                                <override>
                                                    <appletId>Posertron_Applet</appletId>
                                                    <register>AppletRegister_posertron.lua</register>
                                                    <params>
                                                        <model>
                                                            <mdl>customize_logo.mdl</mdl>
                                                        </model>
                                                    </params>
                                                </override>
		                                    </applet>
		                                </root_custom>
                                        <root_gift_machine>
			                                <rot type='vec'>0,0,0,0</rot>
                                            <scale type='vec'>1,1,1,1</scale>
                                            <pos type='vec'>-1.864,6.425,16.736,0</pos>
                                            <applet>
			                                    <name>Posertrons_V2</name>
                                                <override>
                                                    <appletId>Posertron_Applet</appletId>
                                                    <register>AppletRegister_posertron.lua</register>
                                                    <params>
                                                        <model>
                                                            <mdl>giftlogo.mdl</mdl>
                                                            <ani>giftlogo.ani</ani>
                                                            <hkx>buybag.hkx</hkx>
                                                            <skn>giftlogo.skn</skn>
                                                        </model>
                                                    </params>
                                                </override>
		                                    </applet>
		                                </root_gift_machine>
                                        <root_commerce>
			                                <rot type='vec'>0,0,0,0</rot>
                                            <scale type='vec'>1,1,1,1</scale>
                                            <pos type='vec'>-6.443,6.425,14.245,0</pos>
                                            <applet>
			                                    <name>Posertrons_V2</name>
                                                <override>
                                                    <appletId>Posertron_Applet</appletId>
                                                    <register>AppletRegister_posertron.lua</register>
                                                    <params>
                                                        <model>
                                                            <mdl>buybag.mdl</mdl>
                                                            <ani>buybag.ani</ani>
                                                            <hkx>buybag.hkx</hkx>
                                                            <skn>buybag.skn</skn>
                                                        </model>
                                                    </params>
                                                </override>
		                                    </applet>
		                                </root_commerce>
	                                </feature_root>
                                </lua>");
                        return;
                    case "commercePoints":
                        await ctx.Response.Send($@"<lua>
	                                    <feature_com_context type='num'>{UniqueIDCounter.CreateUniqueID()}</feature_com_context>
                                        <applet>
		                                    <CommercePoint_def></CommercePoint_def>
	                                    </applet>
                                        <detectors>
		                                    <detectorsCommercePoint_def></detectorsCommercePoint_def>
	                                    </detectors>
	                                    <feature_root>
		                                    <root>
			                                    <rot type='vec'>0,0,0,0</rot>
                                                <scale type='vec'>0,0,0,0</scale>
                                                <pos type='vec'>-6.243,6.924,14.045,0</pos>
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
                    default:
                        LoggerAccessor.LogWarn($"[PostAuthParameters] - group_def definition data was not found for group_def:{group_def}!");
                        break;
                }
                await ctx.Response.Send("<lua></lua>");
            });
        }
    }
}
