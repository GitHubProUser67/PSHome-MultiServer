//Dr Gadgit from the Code project http://www.codeproject.com/Articles/893791/DLNA-made-easy-and-Play-To-for-any-device
using MultiServerLibrary.Extension;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace DLNAService
{
    #region HelperDNL
    public static class HelperDLNA
    {
        public static string MakeRequest(string Method, string Url, int ContentLength, string SOAPAction, string IP, int Port)
        {
            // Make a request that is sent out to the DLNA server on the LAN using TCP
            string R = Method.ToUpper() + " /" + Url + " HTTP/1.1\r\n";
            R += "Cache-Control: no-cache\r\n";
            R += "Connection: Close\r\n";
            R += "Pragma: no-cache\r\n";
            R += "Host: " + IP + ":" + Port + "\r\n";
            R += "User-Agent: Microsoft-Windows/6.3 UPnP/1.0 Microsoft-DLNA DLNADOC/1.50\r\n";
            R += "FriendlyName.DLNA.ORG: " + Environment.MachineName + "\r\n";
            if (ContentLength > 0)
            {
                R += "Content-Length: " + ContentLength + "\r\n";
                R += "Content-Type: text/xml; charset=\"utf-8\"\r\n";
            }
            if (SOAPAction.Length > 0)
                R += "SOAPAction: \"" + SOAPAction + "\"\r\n";
            return R + "\r\n";
        }

        public static Socket MakeSocket(string ip, int port)
        {
            // Just returns a TCP socket ready to use
            IPEndPoint IPWeb = new IPEndPoint(IPAddress.Parse(ip), port);
            Socket SocWeb = new Socket(IPWeb.Address.AddressFamily, SocketType.Stream, ProtocolType.Tcp)
            {
                ReceiveTimeout = 6000
            };
            SocWeb.Connect(IPWeb);
            return SocWeb;
        }

        public static string ReadSocket(Socket Soc, bool CloseOnExit, ref int ReturnCode)
        {
            // We have some data to read on the socket 
            ReturnCode = 0;
            int Count = 0;
            int ContentLength = 0;
            int HeadLength = 0;
            byte[] buffer = new byte[8000];
            using (MemoryStream ms = new MemoryStream())
            {
                Thread.Sleep(300);
                while (Count < 8)
                {
                    Count++;
                    if (Soc.Available > 0)
                    {
                        int Size = Soc.Receive(buffer);
                        string Head = Encoding.UTF32.GetString(buffer).ToLower();
                        if (ContentLength == 0 && Head.IndexOf("\r\n\r\n") > -1 && Head.IndexOf("content-length:") > -1)
                        {
                            // We have a contant length so we can test if we have all the page data.
                            HeadLength = Head.LastIndexOf("\r\n\r\n");
                            string StrCL = Head.ChopOffBefore("content-length:").ChopOffAfter("\r\n");
                            _ = int.TryParse(StrCL, out ContentLength);
                        }
                        ms.Write(buffer, 0, Size);
                        if (ContentLength > 0 && ms.Length >= HeadLength + ContentLength)
                        {
                            if (CloseOnExit) Soc.Close();
                            return Encoding.UTF8.GetString(ms.ToArray());
                        }
                    }
                    Thread.Sleep(200);
                }
                if (CloseOnExit) Soc.Close();
                string HTML = Encoding.UTF8.GetString(ms.ToArray());
                string Code = HTML.ChopOffBefore("HTTP/1.1").Trim().ChopOffAfter(" ");
                _ = int.TryParse(Code, out ReturnCode);
                return HTML;
            }
        }
    }
    #endregion

    #region DLNAService
    public class DLNAService
    {
        // Each DLNA server might offer several services so this class makes them easyer to read but the one we are looking for to use is AVTransport
        public string controlURL = string.Empty;
        public string Scpdurl = string.Empty;
        public string EventSubURL = string.Empty;
        public string ServiceType = string.Empty;
        public string ServiceID = string.Empty;

        public DLNAService(string HTML)
        {
            HTML = HTML.ChopOffBefore("<service>").Replace("url>/", "url>").Trim();
            HTML = HTML.Replace("URL>/", "URL>");
            if (HTML.ToLower().IndexOf("<servicetype>") > -1)
                ServiceType = HTML.ChopOffBefore("<servicetype>").ChopOffAfter("</servicetype>").Trim();
            if (HTML.ToLower().IndexOf("<serviceid>") > -1)
                ServiceID = HTML.ChopOffBefore("<serviceid>").ChopOffAfter("</serviceid>").Trim();
            if (HTML.ToLower().IndexOf("<controlurl>") > -1)
                controlURL = HTML.ChopOffBefore("<controlurl>").ChopOffAfter("</controlurl>").Trim();
            if (HTML.ToLower().IndexOf("<scpdurl>") > -1)
                Scpdurl = HTML.ChopOffBefore("<scpdurl>").ChopOffAfter("</scpdurl>").Trim();
            if (HTML.ToLower().IndexOf("<eventsuburl>") > -1)
                EventSubURL = HTML.ChopOffBefore("<eventsuburl>").ChopOffAfter("</eventsuburl>").Trim();
        }

        public static Dictionary<string, DLNAService> ReadServices(string HTML)
        {
            Dictionary<string, DLNAService> Dic = new Dictionary<string, DLNAService>();
            HTML = HTML.ChopOffBefore("<serviceList>").ChopOffAfter("</serviceList>").Replace("</service>", "¬");
            foreach (string Line in HTML.Split('¬'))
            {
                if (Line.Length > 20)
                {
                    DLNAService S = new DLNAService(Line);
                    Dic.Add(S.ServiceID, S);
                }
            }
            return Dic;
        }
    }
    #endregion

    public class DLNADevice
    {
        private int NoPlayCount = 0;
        private int PlayListPointer = 0;
        private Dictionary<int, string> PlayListQueue = new Dictionary<int, string>();
        public string ControlURL = string.Empty;
        public bool Connected = false;
        public int ReturnCode = 0;
        public int Port = 0;
        public string IP = string.Empty;
        public string Location = string.Empty;
        public string Server = string.Empty;
        public string USN = string.Empty;
        public string ST = string.Empty;
        public string SMP = string.Empty;
        public string HTML = string.Empty;
        public string FriendlyName = string.Empty;
        public Dictionary<string, DLNAService> Services = null;

        private readonly string XMLHead = "<?xml version=\"1.0\"?>\r\n" +
            "<SOAP-ENV:Envelope xmlns:SOAP-ENV=\"http://schemas.xmlsoap.org/soap/envelope/\" SOAP-ENV:encodingStyle=\"http://schemas.xmlsoap.org/soap/encoding/\">\r\n" +
            "<SOAP-ENV:Body>\r\n";
        private readonly string XMLFoot = "</SOAP-ENV:Body>\r\n" +
            "</SOAP-ENV:Envelope>\r\n";

        public bool IsConnected()
        {
            // Will send a request to the DLNA server and then see if we get a valid reply
            Connected = false;
            try
            {
                Socket SocWeb = HelperDLNA.MakeSocket(IP, Port);
                SocWeb.Send(Encoding.UTF8.GetBytes(HelperDLNA.MakeRequest("GET", SMP, 0, string.Empty, IP, Port)), SocketFlags.None);
                HTML = HelperDLNA.ReadSocket(SocWeb, true, ref ReturnCode);
                if (ReturnCode != 200) return false;
                Services = DLNAService.ReadServices(HTML);
                if (HTML.ToLower().IndexOf("<friendlyname>") > -1)
                    FriendlyName = HTML.ChopOffBefore("<friendlyName>").ChopOffAfter("</friendlyName>").Trim();
                foreach (DLNAService S in Services.Values)
                {
                    if (S.ServiceType.ToLower().IndexOf("avtransport:1") > -1) // avtransport is the one we will be using to control the device
                    {
                        ControlURL = S.controlURL;
                        Connected = true;
                        return true;
                    }
                }
            }
            catch { ;}
            return false;
        }


        public string TryToPlayFile(string UrlToPlay)
        {
            if (!Connected) Connected = IsConnected(); // Someone might have turned the TV Off !
            if (!Connected) return "#ERROR# Not connected";
            try
            {
                if (Services != null)
                {
                    foreach (DLNAService S in Services.Values)
                    {
                        if (S.ServiceType.ToLower().IndexOf("avtransport:1") > -1)
                        {//is the service we are using so upload the file and then start playing
                            string AddPlay = UploadFileToPlay(S.controlURL, UrlToPlay);
                            if (ReturnCode != 200) return "#ERROR# Cannot upload file";
                            string PlayNow = StartPlay(S.controlURL, 0);
                            if (ReturnCode == 200) return "OK"; else return "#ERROR Starting";
                        }
                    }
                }
                return "#ERROR# Could not find avtransport:1";
            }
            catch (Exception ex) { return "#ERROR# " + ex.Message;}
            
        }

        public string GetPosition()
        {
            // Returns the current position for the track that is playing on the DLNA server
            return GetPosition(ControlURL);
        }

        private string GetPosition(string ControlURL)
        {
            // Returns the current position for the track that is playing on the DLNA server
            string XML = XMLHead + "<m:GetPositionInfo xmlns:m=\"urn:schemas-upnp-org:service:AVTransport:1\"><InstanceID xmlns:dt=\"urn:schemas-microsoft-com:datatypes\" dt:dt=\"ui4\">0</InstanceID></m:GetPositionInfo>" + XMLFoot + "\r\n";
            Socket SocWeb = HelperDLNA.MakeSocket(IP, Port);
            SocWeb.Send(Encoding.UTF8.GetBytes(HelperDLNA.MakeRequest("POST", ControlURL, XML.Length, "urn:schemas-upnp-org:service:AVTransport:1#GetPositionInfo", IP, Port) + XML), SocketFlags.None);
            return HelperDLNA.ReadSocket(SocWeb, true, ref ReturnCode);
        }

        public string Desc()
        {
            // Gets a description of the DLNA server
            string XML= "<DIDL-Lite xmlns:dc=\"http://purl.org/dc/elements/1.1/\" xmlns:upnp=\"urn:schemas-upnp-org:metadata-1-0/upnp/\" xmlns:r=\"urn:schemas-rinconnetworks-com:metadata-1-0/\" xmlns=\"urn:schemas-upnp-org:metadata-1-0/DIDL-Lite/\">\r\n";
            XML+= "<item>\r\n";
            XML+="<dc:title>Capital Edinburgh "  + DateTime.Now.Millisecond + "</dc:title>\r\n";
            XML+= "<upnp:class>object.item.audioItem.audioBroadcast</upnp:class>\r\n";
            XML+= "<desc id=\"cdudn\" nameSpace=\"urn:schemas-rinconnetworks-com:metadata-1-0/\">SA_RINCON65031_</desc>\r\n";
            XML+= "</item>\r\n";
            XML += "</DIDL-Lite>\r\n";
            return XML;
        }

        public string StartPlay(int Instance)
        {
            // Start playing the new upload film or music track 
            if (!Connected) Connected = IsConnected();
            if (!Connected) return "#ERROR# Not connected";
            return StartPlay(ControlURL, Instance);
        }

        private string StartPlay(string ControlURL, int Instance)
        {
            // Start playing the new upload film or music track 
            string XML = XMLHead;
            XML += "<u:Play xmlns:u=\"urn:schemas-upnp-org:service:AVTransport:1\"><InstanceID>"+ Instance + "</InstanceID><Speed>1</Speed></u:Play>\r\n";
            XML += XMLFoot + "\r\n";
            Socket SocWeb = HelperDLNA.MakeSocket(IP, Port);
            SocWeb.Send(Encoding.UTF8.GetBytes(HelperDLNA.MakeRequest("POST", ControlURL, XML.Length, "urn:schemas-upnp-org:service:AVTransport:1#Play", IP, Port) + XML), SocketFlags.None);
            return HelperDLNA.ReadSocket(SocWeb, true, ref ReturnCode);
        }

        public string StopPlay(bool ClearQueue)
        {
            // If we are playing music tracks and not just a movie then clear our queue of tracks
            if (!Connected) Connected = IsConnected();
            if (!Connected) return "#ERROR# Not connected";
            if (ClearQueue)
            {
                PlayListQueue = new Dictionary<int, string>();
                PlayListPointer = 0;
            }
            return  StopPlay(ControlURL , 0);
        }


        private string StopPlay(string ControlURL, int Instance)
        {
            // Called to stop playing a movie or a music track
            string XML = XMLHead;
            XML += "<u:Stop xmlns:u=\"urn:schemas-upnp-org:service:AVTransport:1\"><InstanceID>" + Instance + "</InstanceID></u:Stop>\r\n";
            XML += XMLFoot + "\r\n";
            Socket SocWeb = HelperDLNA.MakeSocket(IP, Port);
            SocWeb.Send(Encoding.UTF8.GetBytes(HelperDLNA.MakeRequest("POST", ControlURL, XML.Length, "urn:schemas-upnp-org:service:AVTransport:1#Stop", IP, Port) + XML), SocketFlags.None);
            return HelperDLNA.ReadSocket(SocWeb, true, ref ReturnCode);
        }

        private string Pause(string ControlURL, int Instance)
        {
            // Called to pause playing a movie or a music track
            string XML = XMLHead;
            XML += "<u:Pause xmlns:u=\"urn:schemas-upnp-org:service:AVTransport:1\"><InstanceID>" + Instance + "</InstanceID></u:Pause>\r\n";
            XML += XMLFoot + "\r\n";
            Socket SocWeb = HelperDLNA.MakeSocket(IP, Port);
            SocWeb.Send(Encoding.UTF8.GetBytes(HelperDLNA.MakeRequest("POST", ControlURL, XML.Length, "urn:schemas-upnp-org:service:AVTransport:1#Pause", IP, Port) + XML), SocketFlags.None);
            return HelperDLNA.ReadSocket(SocWeb, true, ref ReturnCode);
        }

        public int PlayPreviousQueue()
        {
            // Play the previous track in our queue, we don't care if the current track has not completed or not, just do it
            PlayListPointer--;
            if (PlayListQueue.Count == 0) return 0;
            if (PlayListPointer == 0)
                PlayListPointer = PlayListQueue.Count;
            string Url = PlayListQueue[PlayListPointer];
            StopPlay(false);
            TryToPlayFile(Url);
            return 310;
        }

        public int PlayNextQueue(bool Force)
        {
            // Play the next track in our queue but only if the current track is about to end or unless we are being forced  
            if (Force)
            {
                // Looks like someone has pressed the next track button
                PlayListPointer++;
                if (PlayListQueue.Count == 0) return 0;
                if (PlayListPointer > PlayListQueue.Count)
                    PlayListPointer = 1;
                string Url = PlayListQueue[PlayListPointer];
                StopPlay(false);
                TryToPlayFile(Url); // Just play it
                NoPlayCount = 0;
                return 310; // Just guess for now how long the track is
            }
            else
            {
                string HTMLPosition = GetPosition();
                if (HTMLPosition.Length < 50) return 0;
                int RTime = TotalSeconds(HTMLPosition.ChopOffBefore("<RelTime>").ChopOffAfter("</RelTime>").Substring(2));
                int TTime = TotalSeconds(HTMLPosition.ChopOffBefore("<TrackDuration>").ChopOffAfter("</TrackDuration>").Substring(2));
                if (RTime < 3 || TTime < 2)
                {
                    NoPlayCount++;
                    if (NoPlayCount > 3)
                    {
                        StopPlay(false);
                        return PlayNextQueue(true); // Force the next track to start because the current track is about to end
                    }
                    else
                        return 0;

                }
                int SecondsToPlay = TTime - RTime - 5;
                if (SecondsToPlay < 0) SecondsToPlay = 0; // Just a safeguard
                if (SecondsToPlay <10)
                {
                    // Current track is about to end so wait a few seconds and then force the next track in our queue to play
                    Thread.Sleep((SecondsToPlay * 1000) +100);
                    return PlayNextQueue(true);
                }
                return SecondsToPlay; // Will have to wait to be polled again before playing the next track in our queue
            }
        }

        private int TotalSeconds(string Value)
        {
            // Convert the time left for the track to play back to seconds
            try
            {
                Value = Value.ChopOffAfter(".");
                return int.Parse(Value.Split(':')[0]) * 60 + int.Parse(Value.Split(':')[1]);
            }
            catch {;}
            return 0;
        }

        public bool AddToQueue(string UrlToPlay, ref bool NewTrackPlaying)
        {
            // We add music tracks to a play list queue and then we poll the server so we know when to send the next track in the queue to play
            if (!Connected) Connected = IsConnected();
            if (!Connected) return false;
            foreach (string Url in PlayListQueue.Values)
            {
                if (Url.ToLower() == UrlToPlay.ToLower())
                    return false;
            }
            PlayListQueue.Add(PlayListQueue.Count + 1, UrlToPlay);
            if (!NewTrackPlaying)
            {
                PlayListPointer = PlayListQueue.Count + 1;
                StopPlay(false);
                TryToPlayFile(UrlToPlay);
                NewTrackPlaying = true;
            }
            return false;
        }

        private string UploadFileToPlay(string ControlURL, string UrlToPlay)
        {
            // Later we will send a message to the DLNA server to start the file playing
            string XML = XMLHead;
            XML += "<u:SetAVTransportURI xmlns:u=\"urn:schemas-upnp-org:service:AVTransport:1\">\r\n";
            XML += "<InstanceID>0</InstanceID>\r\n";
            XML += "<CurrentURI>" + UrlToPlay.Replace(" ", "%20") + "</CurrentURI>\r\n";
            XML += "<CurrentURIMetaData>" + Desc() + "</CurrentURIMetaData>\r\n";
            XML += "</u:SetAVTransportURI>\r\n";
            XML += XMLFoot + "\r\n";
            Socket SocWeb = HelperDLNA.MakeSocket(IP, Port);
            SocWeb.Send(Encoding.UTF8.GetBytes(HelperDLNA.MakeRequest("POST", ControlURL, XML.Length, "urn:schemas-upnp-org:service:AVTransport:1#SetAVTransportURI", IP, Port) + XML), SocketFlags.None);
            return HelperDLNA.ReadSocket(SocWeb, true, ref ReturnCode);
        }

        public DLNADevice(string url)
        {
            // Constructor like "http://192.168.0.41:7676/smp_14_"
            IP = url.ChopOffBefore("http://").ChopOffAfter(":");
            SMP = url.ChopOffBefore(IP).ChopOffBefore("/");
            _ = int.TryParse(url.ChopOffBefore(IP).ChopOffBefore(":").ChopOffAfter("/"), out Port);
        }

        public DLNADevice(string ip, int port, string smp)
        {
            // Constructor
            IP = ip;
            Port = port;
            SMP = smp;
        }
    }
}