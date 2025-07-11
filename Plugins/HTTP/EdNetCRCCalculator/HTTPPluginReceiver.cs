using MultiServerLibrary.HTTP;
using System;
using System.Threading.Tasks;
using WatsonWebserver.Core;
using System.Net;
using ApacheNet.PluginManager;
using EdNetService.CRC;

namespace EdNetCRCCalculator
{
    public class HTTPPluginReceiver : HTTPPlugin
    {
        public Task HTTPStartPlugin(string param, ushort port)
        {
            return Task.CompletedTask;
        }

        public object? ProcessPluginMessage(object obj)
        {
            if (obj is HttpContextBase ctx)
            {
                HttpRequestBase request = ctx.Request;
                HttpResponseBase response = ctx.Response;

                bool sent = false;

                if (!string.IsNullOrEmpty(request.Url.RawWithQuery))
                {
                    switch (request.Method.ToString())
                    {
                        case "GET":

                            switch (HTTPProcessor.ExtractDirtyProxyPath(request.RetrieveHeaderValue("Referer")) + HTTPProcessor.ProcessQueryString(HTTPProcessor.DecodeUrl(request.Url.RawWithQuery)))
                            {
                                #region EdNet CRC Tools
                                case "/!EdNet/GetCRC/":
                                    if (request.QuerystringExists("str"))
                                    {
                                        response.Headers.Add("Date", DateTime.Now.ToString("r"));
                                        response.StatusCode = (int)HttpStatusCode.OK;
                                        response.ContentType = "text/plain";
                                        sent = response.Send(Utils.GetCRCFromStringHexadecimal(request.RetrieveQueryValue("str").Replace("\"", string.Empty))).Result;
                                    }
                                    else
                                    {
                                        response.StatusCode = (int)HttpStatusCode.InternalServerError;
                                        response.ContentType = "text/plain";
                                        sent = response.Send().Result;
                                    }
                                    break;
                                    #endregion
                            }

                            break;
                    }
                }

                return sent;
            }

            return null;
        }
    }
}
