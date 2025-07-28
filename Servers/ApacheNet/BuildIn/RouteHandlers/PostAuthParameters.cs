using ApacheNet.BuildIn.RouteHandlers.GameRoutes.Lockwood;
using System.Net;
using WatsonWebserver.Core;

namespace ApacheNet.BuildIn.RouteHandlers
{
    internal static class PostAuthParameters
    {
        public static void Build(WebserverBase server)
        {
            #region PS Home
            if (ApacheNetServerConfiguration.EnableBuiltInPlugins)
            {
                server.Routes.PostAuthentication.Parameter.Add(HttpMethod.GET, "/objects/D2CDD8B2-DE444593-A64C68CB-0B5EDE23/{id}.xml", async (ctx) =>
                {
                    string? QuizID = ctx.Request.Url.Parameters["id"];

                    if (!string.IsNullOrEmpty(QuizID))
                    {
                        ctx.Response.StatusCode = (int)HttpStatusCode.OK;
                        ctx.Response.ContentType = "text/xml";
                        await ctx.Response.Send("<Root></Root>"); // TODO - Figure out this complicated LUAC in object : D2CDD8B2-DE444593-A64C68CB-0B5EDE23
                    }
                    else
                    {
                        ctx.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                        ctx.Response.ContentType = "text/plain";
                        await ctx.Response.Send();
                    }
                });

                Sodium.BuildSodiumPlugin(server);
                Sodium.BuildSodiumBlimpPlugin(server);
                SodiumObjectives.BuildSodiumObjectivesPlugin(server);
                Sodium.BuildSodium2Plugin(server);

                Venue.BuildVenuePlugin(server);

                IslandDevelopment.BuildIslandPlugin(server);
            }
            #endregion
        }
    }
}
