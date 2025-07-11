using MultiServerLibrary.HTTP;
using System;
using System.IO;
using System.Threading.Tasks;
using WatsonWebserver.Core;
using System.Net;
using ApacheNet.PluginManager;

namespace HomeWebTools
{
    public class HTTPPluginReceiver : HTTPPlugin
    {
        // TODO, improve the plugin params, by ideally passing the server config directly.
        private string APIStaticFolder = string.Empty;
        private string ConvertersStaticFolder = Directory.GetCurrentDirectory() + "/static/converters";

        public Task HTTPStartPlugin(string param, ushort port)
        {
            APIStaticFolder = param + "/!HomeTools";

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
                        case "POST":

                            switch (HTTPProcessor.ExtractDirtyProxyPath(request.RetrieveHeaderValue("Referer")) + HTTPProcessor.ProcessQueryString(HTTPProcessor.DecodeUrl(request.Url.RawWithQuery)))
                            {
                                #region LibSecure HomeTools
                                case "/!HomeTools/MakeBarSdat/":
                                    (byte[]?, string)? makeres = HomeToolsInterface.MakeBarSdat(ConvertersStaticFolder, Path.GetTempPath(), new MemoryStream(request.DataAsBytes), request.ContentType);
                                    if (makeres != null)
                                    {
                                        response.Headers.Add("Date", DateTime.Now.ToString("r"));
                                        response.Headers.Add("Content-disposition", $"attachment; filename={makeres.Value.Item2}");
                                        response.StatusCode = (int)HttpStatusCode.OK;
                                        response.ContentType = "text/plain";
                                        sent = response.Send(makeres.Value.Item1).Result;
                                    }
                                    else
                                    {
                                        response.StatusCode = (int)HttpStatusCode.InternalServerError;
                                        response.ContentType = "text/plain";
                                        sent = response.Send().Result;
                                    }
                                    break;
                                case "/!HomeTools/UnBar/":
                                    (byte[]?, string)? unbarres = HomeToolsInterface.UnBar(Path.GetTempPath(), new MemoryStream(request.DataAsBytes), request.ContentType, APIStaticFolder + "/HelperFiles").Result;
                                    if (unbarres != null)
                                    {
                                        response.Headers.Add("Date", DateTime.Now.ToString("r"));
                                        response.Headers.Add("Content-disposition", $"attachment; filename={unbarres.Value.Item2}");
                                        response.StatusCode = (int)HttpStatusCode.OK;
                                        response.ContentType = "text/plain";
                                        sent = response.Send(unbarres.Value.Item1).Result;
                                    }
                                    else
                                    {
                                        response.StatusCode = (int)HttpStatusCode.InternalServerError;
                                        response.ContentType = "text/plain";
                                        sent = response.Send().Result;
                                    }
                                    break;
                                case "/!HomeTools/CDS/":
                                    (byte[]?, string)? cdsres = HomeToolsInterface.CDS(new MemoryStream(request.DataAsBytes), request.ContentType);
                                    if (cdsres != null)
                                    {
                                        response.Headers.Add("Date", DateTime.Now.ToString("r"));
                                        response.Headers.Add("Content-disposition", $"attachment; filename={cdsres.Value.Item2}");
                                        response.StatusCode = (int)HttpStatusCode.OK;
                                        response.ContentType = "text/plain";
                                        sent = response.Send(cdsres.Value.Item1).Result;
                                    }
                                    else
                                    {
                                        response.StatusCode = (int)HttpStatusCode.InternalServerError;
                                        response.ContentType = "text/plain";
                                        sent = response.Send().Result;
                                    }
                                    break;
                                case "/!HomeTools/CDSBruteforce/":
                                    (byte[]?, string)? cdsbruteres = HomeToolsInterface.CDSBruteforce(new MemoryStream(request.DataAsBytes), request.ContentType);
                                    if (cdsbruteres != null)
                                    {
                                        response.Headers.Add("Date", DateTime.Now.ToString("r"));
                                        response.Headers.Add("Content-disposition", $"attachment; filename={cdsbruteres.Value.Item2}");
                                        response.StatusCode = (int)HttpStatusCode.OK;
                                        response.ContentType = "text/plain";
                                        sent = response.Send(cdsbruteres.Value.Item1).Result;
                                    }
                                    else
                                    {
                                        response.StatusCode = (int)HttpStatusCode.InternalServerError;
                                        response.ContentType = "text/plain";
                                        sent = response.Send().Result;
                                    }
                                    break;
                                case "/!HomeTools/HCDBUnpack/":
                                    (byte[]?, string)? hcdbres = HomeToolsInterface.HCDBUnpack(new MemoryStream(request.DataAsBytes), request.ContentType);
                                    if (hcdbres != null)
                                    {
                                        response.Headers.Add("Date", DateTime.Now.ToString("r"));
                                        response.Headers.Add("Content-disposition", $"attachment; filename={hcdbres.Value.Item2}");
                                        response.StatusCode = (int)HttpStatusCode.OK;
                                        response.ContentType = "text/plain";
                                        sent = response.Send(hcdbres.Value.Item1).Result;
                                    }
                                    else
                                    {
                                        response.StatusCode = (int)HttpStatusCode.InternalServerError;
                                        response.ContentType = "text/plain";
                                        sent = response.Send().Result;
                                    }
                                    break;
                                case "/!HomeTools/TicketList/":
                                    (byte[]?, string)? ticketlistres = HomeToolsInterface.TicketList(new MemoryStream(request.DataAsBytes), request.ContentType);
                                    if (ticketlistres != null)
                                    {
                                        response.Headers.Add("Date", DateTime.Now.ToString("r"));
                                        response.Headers.Add("Content-disposition", $"attachment; filename={ticketlistres.Value.Item2}");
                                        response.StatusCode = (int)HttpStatusCode.OK;
                                        response.ContentType = "text/plain";
                                        sent = response.Send(ticketlistres.Value.Item1).Result;
                                    }
                                    else
                                    {
                                        response.StatusCode = (int)HttpStatusCode.InternalServerError;
                                        response.ContentType = "text/plain";
                                        sent = response.Send().Result;
                                    }
                                    break;
                                case "/!HomeTools/INF/":
                                    (byte[]?, string)? infres = HomeToolsInterface.INF(new MemoryStream(request.DataAsBytes), request.ContentType);
                                    if (infres != null)
                                    {
                                        response.Headers.Add("Date", DateTime.Now.ToString("r"));
                                        response.Headers.Add("Content-disposition", $"attachment; filename={infres.Value.Item2}");
                                        response.StatusCode = (int)HttpStatusCode.OK;
                                        response.ContentType = "text/plain";
                                        sent = response.Send(infres.Value.Item1).Result;
                                    }
                                    else
                                    {
                                        response.StatusCode = (int)HttpStatusCode.InternalServerError;
                                        response.ContentType = "text/plain";
                                        sent = response.Send().Result;
                                    }
                                    break;
                                case "/!HomeTools/ProcessProfanity/":
                                    (byte[]?, string)? profres = HomeToolsInterface.ProcessProfanity(new MemoryStream(request.DataAsBytes), request.ContentType);
                                    if (profres != null)
                                    {
                                        response.Headers.Add("Date", DateTime.Now.ToString("r"));
                                        response.Headers.Add("Content-disposition", $"attachment; filename={profres.Value.Item2}");
                                        response.StatusCode = (int)HttpStatusCode.OK;
                                        response.ContentType = "text/plain";
                                        sent = response.Send(profres.Value.Item1).Result;
                                    }
                                    else
                                    {
                                        response.StatusCode = (int)HttpStatusCode.InternalServerError;
                                        response.ContentType = "text/plain";
                                        sent = response.Send().Result;
                                    }
                                    break;
                                case "/!HomeTools/ChannelID/":
                                    string? channelres = HomeToolsInterface.ChannelID(new MemoryStream(request.DataAsBytes), request.ContentType);
                                    if (!string.IsNullOrEmpty(channelres))
                                    {
                                        response.Headers.Add("Date", DateTime.Now.ToString("r"));
                                        response.StatusCode = (int)HttpStatusCode.OK;
                                        response.ContentType = "text/plain";
                                        sent = response.Send(channelres).Result;
                                    }
                                    else
                                    {
                                        response.StatusCode = (int)HttpStatusCode.InternalServerError;
                                        response.ContentType = "text/plain";
                                        sent = response.Send().Result;
                                    }
                                    break;
                                case "/!HomeTools/SceneID/":
                                    string? sceneres = HomeToolsInterface.SceneID(new MemoryStream(request.DataAsBytes), request.ContentType);
                                    if (!string.IsNullOrEmpty(sceneres))
                                    {
                                        response.Headers.Add("Date", DateTime.Now.ToString("r"));
                                        response.StatusCode = (int)HttpStatusCode.OK;
                                        response.ContentType = "text/plain";
                                        sent = response.Send(sceneres).Result;
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
