using System.IO;
using MultiServerLibrary.HTTP;
using CustomLogger;
using HttpMultipartParser;
using System.Text;
using System;

namespace WebAPIService.GameServices.PSHOME.PREMIUMAGENCY
{
    public class Trigger
    {
        public static string getEventTriggerRequestPOST(byte[] PostData, string ContentType, string workpath, string eventId)
        {
            #region MultipartFormDataParser
            string nid = string.Empty;
            string readcnt = string.Empty;

            string boundary = HTTPProcessor.ExtractBoundary(ContentType);

            using (MemoryStream ms = new MemoryStream(PostData))
            {
                var data = MultipartFormDataParser.Parse(ms, boundary);

                nid = data.GetParameterValue("nid");
                readcnt = data.GetParameterValue("readcnt");

                ms.Flush();
            }
            #endregion

            #region ConfirmTriggerPaths

            string basaraHomeSquareTriggerPath = $"{workpath}/eventController/collabo_iln/Triggers/";
            string Christmas2010 = $"{workpath}/eventController/Christmas/2010/Triggers/";
            string GeorgiaTriggerPath = $"{workpath}/eventController/Georgia/Triggers/";
            string homecafeDJMusicTriggerPath = $"{workpath}/eventController/hc/DJMusic/Triggers/";
            string homecafeGalleryTriggerPath = $"{workpath}/eventController/hc/hc_gallery/Triggers/";
            string homecafeRollyCafe1FTriggerPath = $"{workpath}/eventController/hc/RollyCafe1F/Triggers/";
            string halloween2010TriggerPath = $"{workpath}/eventController/Halloween/2010/Triggers/";
            string iDOLMASTERSLiveEventTriggerPath = $"{workpath}/eventController/iDOLMASTERs/LiveEvent/Triggers/";
            string iDOLMASTERSSPMoveToSpecialVenueTriggerPath = $"{workpath}/eventController/iDOLMASTERs/SPMoveToSpecialVenue/Triggers/";
            string j_liargame2TriggerPath = $"{workpath}/eventController/j_liargame2/Triggers/";
            string MikuliveJackTriggerPath = $"{workpath}/eventController/MikuLiveJack/Triggers/";
            string MikuLiveJukeboxTriggerPath = $"{workpath}/eventController/MikuLiveJukebox/Triggers/";
            string MikuliveEventTriggerPath = $"{workpath}/eventController/MikuLiveEvent/Triggers/";
            string PrinnyJackTriggerPath = $"{workpath}/eventController/PrinnyJack/Triggers/";
            string RollyJukeboxTriggerPath = $"{workpath}/eventController/RollyJukebox/Triggers/";
            string SCEAsiaChristmas2010OBJTrigger = $"{workpath}/eventController/Christmas/2010/OBJTrig/Triggers/";
            string SCEAsiaChristmas2010SnowFall = $"{workpath}/eventController/Christmas/2010/Snowfall/Triggers/";
            string SonyAquariumRelocatorTriggerPath = $"{workpath}/eventController/SonyAquarium/Relocator/Triggers/";
            string SonyAquariumVideoConfigTriggerPath = $"{workpath}/eventController/SonyAquarium/VideoConfig/Triggers/";

            #endregion

            DateTime currentTime = DateTime.Now;

            switch (eventId)
            {
                #region MikuLiveJack QA
                case "63":
                    string MikuliveJackQATriggerPath = MikuliveJackTriggerPath + "qagetEventTrigger.xml";
                    if (File.Exists(MikuliveJackQATriggerPath))
                    {
                        LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - GetEventTrigger sent for QA MikuLiveJack {eventId}!");
                        string res = File.ReadAllText(MikuliveJackQATriggerPath);
                        return "<xml>\r\n\t" +
                             "<result type=\"int\">1</result>\r\n\t" +
                             "<description type=\"text\">Success</description>\r\n\t" +
                             "<error_no type=\"int\">0</error_no>\r\n\t" +
                             "<error_message type=\"text\">None</error_message>\r\n\r\n\t" +
                            $"<trigger_count type=\"int\">{readcnt}</trigger_count>\r\n\t" +
                             $"{res}\r\n" +
                             "</xml>";
                    }
                    else
                    {
                        LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - GetEventTrigger FALLBACK sent for QA MikuLiveJack {eventId}! Expected path {MikuliveJackQATriggerPath}");
                        return "<xml>\r\n" +
                            "<result type=\"int\">1</result>\r\n" +
                            "<description type=\"text\">Success</description>\r\n" +
                            "<error_no type=\"int\">0</error_no>\r\n" +
                            "<error_message type=\"text\">None</error_message>\r\n" +
                            $"<trigger_count type=\"int\">{readcnt}</trigger_count>\r\n\t" +
                            "<trigger_time>\r\n\t\t" +
                            "<trigger_id type=\"int\">1</trigger_id>\r\n" +
                            "\r\n\t\t<!-- Event Start Trigger --> \r\n\t\t" +
                            "<start_year type=\"int\">2024</start_year>\r\n\t\t" +
                            "<start_month type=\"int\">01</start_month>\r\n\t\t" +
                            "<start_day type=\"int\">15</start_day>\r\n\t\t" +
                            "<start_hour type=\"int\">00</start_hour>\r\n\t\t" +
                            "<start_minutes type=\"int\">00</start_minutes>\r\n\t\t" +
                            "<start_second type=\"int\">00</start_second>\r\n\r\n\t\t" +
                            "<end_year type=\"int\">2124</end_year>\r\n\t\t" +
                            "<end_month type=\"int\">02</end_month>\r\n\t\t" +
                            "<end_day type=\"int\">20</end_day>\r\n\t\t" +
                            "<end_hour type=\"int\">05</end_hour>\r\n\t\t" +
                            "<end_minutes type=\"int\">45</end_minutes>\r\n\t\t" +
                            "<end_second type=\"int\">0</end_second>\r\n\r\n\t\t" +
                            "<trigger_flag type=\"int\">1</trigger_flag>\r\n\t" +
                            "</trigger_time>\r\n" +
                            "</xml>";
                    }
                #endregion

                #region MikuLiveJack LOCAL
                case "76":
                    string MikuliveJackLOCALTriggerPath = MikuliveJackTriggerPath + "localgetEventTrigger.xml";
                    if (File.Exists(MikuliveJackLOCALTriggerPath))
                    {
                        LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - GetEventTrigger sent for LOCAL MikuLiveJack {eventId}!");
                        string res = File.ReadAllText(MikuliveJackLOCALTriggerPath);
                        return "<xml>\r\n\t" +
                             "<result type=\"int\">1</result>\r\n\t" +
                             "<description type=\"text\">Success</description>\r\n\t" +
                             "<error_no type=\"int\">0</error_no>\r\n\t" +
                             "<error_message type=\"text\">None</error_message>\r\n\r\n\t" +
                            $"<trigger_count type=\"int\">{readcnt}</trigger_count>\r\n\t" +
                             $"{res}\r\n" +
                             "</xml>";
                    }
                    else
                    {
                        LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - GetEventTrigger FALLBACK sent for LOCAL MikuLiveJack {eventId}! Expected path {MikuliveJackLOCALTriggerPath}");
                        return "<xml>\r\n" +
                            "<result type=\"int\">1</result>\r\n" +
                            "<description type=\"text\">Success</description>\r\n" +
                            "<error_no type=\"int\">0</error_no>\r\n" +
                            "<error_message type=\"text\">None</error_message>\r\n" +
                            $"<trigger_count type=\"int\">{readcnt}</trigger_count>\r\n\t" +
                            "<trigger_time>\r\n\t\t" +
                            "<trigger_id type=\"int\">1</trigger_id>\r\n" +
                            "\r\n\t\t<!-- Event Start Trigger --> \r\n\t\t" +
                            "<start_year type=\"int\">2024</start_year>\r\n\t\t" +
                            "<start_month type=\"int\">01</start_month>\r\n\t\t" +
                            "<start_day type=\"int\">15</start_day>\r\n\t\t" +
                            "<start_hour type=\"int\">00</start_hour>\r\n\t\t" +
                            "<start_minutes type=\"int\">00</start_minutes>\r\n\t\t" +
                            "<start_second type=\"int\">00</start_second>\r\n\r\n\t\t" +
                            "<end_year type=\"int\">2124</end_year>\r\n\t\t" +
                            "<end_month type=\"int\">02</end_month>\r\n\t\t" +
                            "<end_day type=\"int\">20</end_day>\r\n\t\t" +
                            "<end_hour type=\"int\">05</end_hour>\r\n\t\t" +
                            "<end_minutes type=\"int\">45</end_minutes>\r\n\t\t" +
                            "<end_second type=\"int\">0</end_second>\r\n\r\n\t\t" +
                            "<trigger_flag type=\"int\">1</trigger_flag>\r\n\t" +
                            "</trigger_time>\r\n" +
                            "</xml>";
                    }
                #endregion

                #region Rolly Jukebox PUBLIC
                case "80":
                    string RollyJukeboxPUBLICTriggerPath = RollyJukeboxTriggerPath + "getEventTrigger.xml";
                    if (File.Exists(RollyJukeboxPUBLICTriggerPath))
                    {
                        LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - GetEventTrigger sent for PUBLIC Rolly Music Survey {eventId}!");
                        string res = File.ReadAllText(RollyJukeboxPUBLICTriggerPath);
                        return "<xml>\r\n\t" +
                             "<result type=\"int\">1</result>\r\n\t" +
                             "<description type=\"text\">Success</description>\r\n\t" +
                             "<error_no type=\"int\">0</error_no>\r\n\t" +
                             "<error_message type=\"text\">None</error_message>\r\n\r\n\t" +
                            $"<trigger_count type=\"int\">{readcnt}</trigger_count>\r\n\t" +
                             $"{res}\r\n" +
                             "</xml>";
                    }
                    else
                    {
                        LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - GetEventTrigger FALLBACK sent for PUBLIC Rolly Music Survey {eventId}! Expected path {RollyJukeboxPUBLICTriggerPath}");
                        return "<xml>\r\n" +
                            "<result type=\"int\">1</result>\r\n" +
                            "<description type=\"text\">Success</description>\r\n" +
                            "<error_no type=\"int\">0</error_no>\r\n" +
                            "<error_message type=\"text\">None</error_message>\r\n" +
                            $"<trigger_count type=\"int\">{readcnt}</trigger_count>\r\n\t" +
                            "<trigger_time>\r\n\t\t" +
                            "<trigger_id type=\"int\">1</trigger_id>\r\n" +
                            "\r\n\t\t<!-- Event Start Trigger --> \r\n\t\t" +
                            "<start_year type=\"int\">2024</start_year>\r\n\t\t" +
                            "<start_month type=\"int\">01</start_month>\r\n\t\t" +
                            "<start_day type=\"int\">15</start_day>\r\n\t\t" +
                            "<start_hour type=\"int\">00</start_hour>\r\n\t\t" +
                            "<start_minutes type=\"int\">00</start_minutes>\r\n\t\t" +
                            "<start_second type=\"int\">00</start_second>\r\n\r\n\t\t" +
                            "<end_year type=\"int\">2124</end_year>\r\n\t\t" +
                            "<end_month type=\"int\">02</end_month>\r\n\t\t" +
                            "<end_day type=\"int\">20</end_day>\r\n\t\t" +
                            "<end_hour type=\"int\">05</end_hour>\r\n\t\t" +
                            "<end_minutes type=\"int\">45</end_minutes>\r\n\t\t" +
                            "<end_second type=\"int\">0</end_second>\r\n\r\n\t\t" +
                            "<trigger_flag type=\"int\">1</trigger_flag>\r\n\t" +
                            "</trigger_time>\r\n" +
                            "</xml>";
                    }
                #endregion

                #region MikuLiveJack PUBLIC
                case "90":
                    string MikuliveJackPUBLICTriggerPath = MikuliveJackTriggerPath + "getEventTrigger.xml";
                    if (File.Exists(MikuliveJackPUBLICTriggerPath))
                    {
                        LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - GetEventTrigger sent for PUBLIC MikuLiveJack {eventId}!");
                        string res = File.ReadAllText(MikuliveJackPUBLICTriggerPath);
                        return "<xml>\r\n\t" +
                             "<result type=\"int\">1</result>\r\n\t" +
                             "<description type=\"text\">Success</description>\r\n\t" +
                             "<error_no type=\"int\">0</error_no>\r\n\t" +
                             "<error_message type=\"text\">None</error_message>\r\n\r\n\t" +
                            $"<trigger_count type=\"int\">{readcnt}</trigger_count>\r\n\t" +
                             $"{res}\r\n" +
                             "</xml>";
                    }
                    else
                    {
                        LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - GetEventTrigger FALLBACK sent for PUBLIC MikuLiveJack {eventId}! Expected path {MikuliveJackPUBLICTriggerPath}");
                        return "<xml>\r\n" +
                            "<result type=\"int\">1</result>\r\n" +
                            "<description type=\"text\">Success</description>\r\n" +
                            "<error_no type=\"int\">0</error_no>\r\n" +
                            "<error_message type=\"text\">None</error_message>\r\n" +
                            $"<trigger_count type=\"int\">{readcnt}</trigger_count>\r\n\t" +
                            "<trigger_time>\r\n\t\t" +
                            "<trigger_id type=\"int\">1</trigger_id>\r\n" +
                            "\r\n\t\t<!-- Event Start Trigger --> \r\n\t\t" +
                            "<start_year type=\"int\">2024</start_year>\r\n\t\t" +
                            "<start_month type=\"int\">01</start_month>\r\n\t\t" +
                            "<start_day type=\"int\">15</start_day>\r\n\t\t" +
                            "<start_hour type=\"int\">00</start_hour>\r\n\t\t" +
                            "<start_minutes type=\"int\">00</start_minutes>\r\n\t\t" +
                            "<start_second type=\"int\">00</start_second>\r\n\r\n\t\t" +
                            "<end_year type=\"int\">2124</end_year>\r\n\t\t" +
                            "<end_month type=\"int\">02</end_month>\r\n\t\t" +
                            "<end_day type=\"int\">20</end_day>\r\n\t\t" +
                            "<end_hour type=\"int\">05</end_hour>\r\n\t\t" +
                            "<end_minutes type=\"int\">45</end_minutes>\r\n\t\t" +
                            "<end_second type=\"int\">0</end_second>\r\n\r\n\t\t" +
                            "<trigger_flag type=\"int\">1</trigger_flag>\r\n\t" +
                            "</trigger_time>\r\n" +
                            "</xml>";
                    }
                #endregion

                #region MikuLiveJukebox PUBLIC
                case "91":
                    string MikuLiveJukeboxPUBLICTriggerPath = MikuLiveJukeboxTriggerPath + "getEventTrigger.xml";
                    if (File.Exists(MikuLiveJukeboxPUBLICTriggerPath))
                    {
                        LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - GetEventTrigger sent for PUBLIC MikuLiveJukebox {eventId}!");
                        string res = File.ReadAllText(MikuLiveJukeboxPUBLICTriggerPath);
                        return "<xml>\r\n\t" +
                             "<result type=\"int\">1</result>\r\n\t" +
                             "<description type=\"text\">Success</description>\r\n\t" +
                             "<error_no type=\"int\">0</error_no>\r\n\t" +
                             "<error_message type=\"text\">None</error_message>\r\n\r\n\t" +
                             $"<trigger_count type=\"int\">{readcnt}</trigger_count>\r\n\t" +
                             $"{res}\r\n" +
                             "</xml>";
                    }
                    else
                    {
                        LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - GetEventTrigger FALLBACK sent for PUBLIC MikuLiveJukebox {eventId}! Expected path {MikuLiveJukeboxPUBLICTriggerPath}");
                        return "<xml>\r\n" +
                            "<result type=\"int\">1</result>\r\n" +
                            "<description type=\"text\">Success</description>\r\n" +
                            "<error_no type=\"int\">0</error_no>\r\n" +
                            "<error_message type=\"text\">None</error_message>\r\n" +
                            $"<trigger_count type=\"int\">{readcnt}</trigger_count>\r\n\t" +
                            "<trigger_time>\r\n\t\t" +
                            "<trigger_id type=\"int\">1</trigger_id>\r\n" +
                            "\r\n\t\t<!-- Event Start Trigger --> \r\n\t\t" +
                            "<start_year type=\"int\">2024</start_year>\r\n\t\t" +
                            "<start_month type=\"int\">01</start_month>\r\n\t\t" +
                            "<start_day type=\"int\">15</start_day>\r\n\t\t" +
                            "<start_hour type=\"int\">00</start_hour>\r\n\t\t" +
                            "<start_minutes type=\"int\">00</start_minutes>\r\n\t\t" +
                            "<start_second type=\"int\">00</start_second>\r\n\r\n\t\t" +
                            "<end_year type=\"int\">2124</end_year>\r\n\t\t" +
                            "<end_month type=\"int\">02</end_month>\r\n\t\t" +
                            "<end_day type=\"int\">20</end_day>\r\n\t\t" +
                            "<end_hour type=\"int\">05</end_hour>\r\n\t\t" +
                            "<end_minutes type=\"int\">45</end_minutes>\r\n\t\t" +
                            "<end_second type=\"int\">0</end_second>\r\n\r\n\t\t" +
                            "<trigger_flag type=\"int\">1</trigger_flag>\r\n\t" +
                            "</trigger_time>\r\n" +
                            "</xml>";
                    }
                #endregion

                #region PrinnyJack PUBLIC
                case "93":
                    string PrinnyJackPUBLICTriggerPath = PrinnyJackTriggerPath + "getEventTrigger.xml";
                    if (File.Exists(PrinnyJackPUBLICTriggerPath))
                    {
                        LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - GetEventTrigger sent for PUBLIC PrinnyJack {eventId}!");
                        string res = File.ReadAllText(PrinnyJackPUBLICTriggerPath);
                        return "<xml>\r\n\t" +
                             "<result type=\"int\">1</result>\r\n\t" +
                             "<description type=\"text\">Success</description>\r\n\t" +
                             "<error_no type=\"int\">0</error_no>\r\n\t" +
                             "<error_message type=\"text\">None</error_message>\r\n\r\n\t" +
                             $"<trigger_count type=\"int\">{readcnt}</trigger_count>\r\n\t" +
                             $"{res}\r\n" +
                             "</xml>";
                    }
                    else
                    {
                        LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - GetEventTrigger FALLBACK sent for PUBLIC PrinnyJack {eventId}! Expected path {PrinnyJackPUBLICTriggerPath}");
                        return "<xml>\r\n" +
                            "<result type=\"int\">1</result>\r\n" +
                            "<description type=\"text\">Success</description>\r\n" +
                            "<error_no type=\"int\">0</error_no>\r\n" +
                            "<error_message type=\"text\">None</error_message>\r\n" +
                            $"<trigger_count type=\"int\">{readcnt}</trigger_count>\r\n\t" +
                            "<trigger_time>\r\n\t\t" +
                            "<trigger_id type=\"int\">1</trigger_id>\r\n" +
                            "\r\n\t\t<!-- Event Start Trigger --> \r\n\t\t" +
                            "<start_year type=\"int\">2024</start_year>\r\n\t\t" +
                            "<start_month type=\"int\">01</start_month>\r\n\t\t" +
                            "<start_day type=\"int\">15</start_day>\r\n\t\t" +
                            "<start_hour type=\"int\">00</start_hour>\r\n\t\t" +
                            "<start_minutes type=\"int\">00</start_minutes>\r\n\t\t" +
                            "<start_second type=\"int\">00</start_second>\r\n\r\n\t\t" +
                            "<end_year type=\"int\">2124</end_year>\r\n\t\t" +
                            "<end_month type=\"int\">02</end_month>\r\n\t\t" +
                            "<end_day type=\"int\">20</end_day>\r\n\t\t" +
                            "<end_hour type=\"int\">05</end_hour>\r\n\t\t" +
                            "<end_minutes type=\"int\">45</end_minutes>\r\n\t\t" +
                            "<end_second type=\"int\">0</end_second>\r\n\r\n\t\t" +
                            "<trigger_flag type=\"int\">1</trigger_flag>\r\n\t" +
                            "</trigger_time>\r\n" +
                            "</xml>";
                    }
                #endregion

                #region MikuLiveEvent PUBLIC
                case "95":
                    string MikuliveEventPublicTriggerPath = MikuliveEventTriggerPath + "getEventTrigger.xml";
                    if (File.Exists(MikuliveEventPublicTriggerPath))
                    {
                        LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - GetEventTrigger sent for PUBLIC MikuLiveEvent {eventId}!");
                        string res = File.ReadAllText(MikuliveEventPublicTriggerPath);
                        return "<xml>\r\n\t" +
                             "<result type=\"int\">1</result>\r\n\t" +
                             "<description type=\"text\">Success</description>\r\n\t" +
                             "<error_no type=\"int\">0</error_no>\r\n\t" +
                             "<error_message type=\"text\">None</error_message>\r\n\r\n\t" +
                             $"<trigger_count type=\"int\">{readcnt}</trigger_count>\r\n\t" +
                             $"{res}\r\n" +
                             "</xml>";
                    }
                    else
                    {
                        LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - GetEventTrigger FALLBACK sent for PUBLIC MikuLiveEvent {eventId}! Expected path {MikuliveEventPublicTriggerPath}");
                        return "<xml>\r\n" +
                            "<result type=\"int\">1</result>\r\n" +
                            "<description type=\"text\">Success</description>\r\n" +
                            "<error_no type=\"int\">0</error_no>\r\n" +
                            "<error_message type=\"text\">None</error_message>\r\n" +
                            $"<trigger_count type=\"int\">{readcnt}</trigger_count>\r\n\t" +
                            "<trigger_time>\r\n\t\t" +
                            "<trigger_id type=\"int\">1</trigger_id>\r\n" +
                            "\r\n\t\t<!-- Event Start Trigger --> \r\n\t\t" +
                            "<start_year type=\"int\">2024</start_year>\r\n\t\t" +
                            "<start_month type=\"int\">01</start_month>\r\n\t\t" +
                            "<start_day type=\"int\">15</start_day>\r\n\t\t" +
                            "<start_hour type=\"int\">00</start_hour>\r\n\t\t" +
                            "<start_minutes type=\"int\">00</start_minutes>\r\n\t\t" +
                            "<start_second type=\"int\">00</start_second>\r\n\r\n\t\t" +
                            "<end_year type=\"int\">2124</end_year>\r\n\t\t" +
                            "<end_month type=\"int\">02</end_month>\r\n\t\t" +
                            "<end_day type=\"int\">20</end_day>\r\n\t\t" +
                            "<end_hour type=\"int\">05</end_hour>\r\n\t\t" +
                            "<end_minutes type=\"int\">45</end_minutes>\r\n\t\t" +
                            "<end_second type=\"int\">0</end_second>\r\n\r\n\t\t" +
                            "<trigger_flag type=\"int\">1</trigger_flag>\r\n\t" +
                            "</trigger_time>\r\n" +
                            "</xml>";
                    }
                #endregion

                #region Georgia PUBLIC
                case "111":
                    string GeorgiaPUblicTriggerPath = GeorgiaTriggerPath + "getEventTrigger.xml";
                    if (File.Exists(GeorgiaPUblicTriggerPath))
                    {
                        LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - GetEventTrigger sent for PUBLIC Georgia {eventId}!");
                        string res = File.ReadAllText(GeorgiaPUblicTriggerPath);
                        return "<xml>\r\n\t" +
                             "<result type=\"int\">1</result>\r\n\t" +
                             "<description type=\"text\">Success</description>\r\n\t" +
                             "<error_no type=\"int\">0</error_no>\r\n\t" +
                             "<error_message type=\"text\">None</error_message>\r\n\r\n\t" +
                             $"<trigger_count type=\"int\">{readcnt}</trigger_count>\r\n\t" +
                             $"{res}\r\n" +
                             "</xml>";
                    }
                    else
                    {
                        LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - GetEventTrigger FALLBACK sent for PUBLIC Georgia {eventId}! Expected path {GeorgiaPUblicTriggerPath}");
                        return "<xml>\r\n" +
                            "<result type=\"int\">1</result>\r\n" +
                            "<description type=\"text\">Success</description>\r\n" +
                            "<error_no type=\"int\">0</error_no>\r\n" +
                            "<error_message type=\"text\">None</error_message>\r\n" +
                            $"<trigger_count type=\"int\">{readcnt}</trigger_count>\r\n\t" +
                            "<trigger_time>\r\n\t\t" +
                            "<trigger_id type=\"int\">1</trigger_id>\r\n" +
                            "\r\n\t\t<!-- Event Start Trigger --> \r\n\t\t" +
                            "<start_year type=\"int\">2024</start_year>\r\n\t\t" +
                            "<start_month type=\"int\">01</start_month>\r\n\t\t" +
                            "<start_day type=\"int\">15</start_day>\r\n\t\t" +
                            "<start_hour type=\"int\">00</start_hour>\r\n\t\t" +
                            "<start_minutes type=\"int\">00</start_minutes>\r\n\t\t" +
                            "<start_second type=\"int\">00</start_second>\r\n\r\n\t\t" +
                            "<end_year type=\"int\">2124</end_year>\r\n\t\t" +
                            "<end_month type=\"int\">02</end_month>\r\n\t\t" +
                            "<end_day type=\"int\">20</end_day>\r\n\t\t" +
                            "<end_hour type=\"int\">05</end_hour>\r\n\t\t" +
                            "<end_minutes type=\"int\">45</end_minutes>\r\n\t\t" +
                            "<end_second type=\"int\">0</end_second>\r\n\r\n\t\t" +
                            "<trigger_flag type=\"int\">1</trigger_flag>\r\n\t" +
                            "</trigger_time>\r\n" +
                            "</xml>";
                    }
                #endregion

                #region iDOLMASTERs
                case "148":
                    string iDOLMASTERSLiveEventPUBLICTriggerPath = iDOLMASTERSLiveEventTriggerPath + "getEventTrigger.xml";
                    if (File.Exists(iDOLMASTERSLiveEventPUBLICTriggerPath))
                    {
                        LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - GetEventTrigger sent for PUBLIC iDOLMASTERs LiveStage {eventId}!");
                        string res = File.ReadAllText(iDOLMASTERSLiveEventPUBLICTriggerPath);
                        return "<xml>\r\n\t" +
                             "<result type=\"int\">1</result>\r\n\t" +
                             "<description type=\"text\">Success</description>\r\n\t" +
                             "<error_no type=\"int\">0</error_no>\r\n\t" +
                             "<error_message type=\"text\">None</error_message>\r\n\r\n\t" +
                             $"<trigger_count type=\"int\">{readcnt}</trigger_count>\r\n\t" +
                             $"{res}\r\n" +
                             "</xml>";
                    }
                    else
                    {
                        LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - GetEventTrigger FALLBACK sent for PUBLIC iDOLMASTERs LiveStage {eventId}! Expected path {iDOLMASTERSLiveEventPUBLICTriggerPath}");
                        return "<xml>\r\n" +
                            "<result type=\"int\">1</result>\r\n" +
                            "<description type=\"text\">Success</description>\r\n" +
                            "<error_no type=\"int\">0</error_no>\r\n" +
                            "<error_message type=\"text\">None</error_message>\r\n" +
                            $"<trigger_count type=\"int\">{readcnt}</trigger_count>\r\n\t" +
                            "<trigger_time>\r\n\t\t" +
                            "<trigger_id type=\"int\">1</trigger_id>\r\n" +
                            "\r\n\t\t<!-- Event Start Trigger --> \r\n\t\t" +
                            "<start_year type=\"int\">2024</start_year>\r\n\t\t" +
                            "<start_month type=\"int\">01</start_month>\r\n\t\t" +
                            "<start_day type=\"int\">15</start_day>\r\n\t\t" +
                            "<start_hour type=\"int\">00</start_hour>\r\n\t\t" +
                            "<start_minutes type=\"int\">00</start_minutes>\r\n\t\t" +
                            "<start_second type=\"int\">00</start_second>\r\n\r\n\t\t" +
                            "<end_year type=\"int\">2124</end_year>\r\n\t\t" +
                            "<end_month type=\"int\">02</end_month>\r\n\t\t" +
                            "<end_day type=\"int\">20</end_day>\r\n\t\t" +
                            "<end_hour type=\"int\">05</end_hour>\r\n\t\t" +
                            "<end_minutes type=\"int\">45</end_minutes>\r\n\t\t" +
                            "<end_second type=\"int\">0</end_second>\r\n\r\n\t\t" +
                            "<trigger_flag type=\"int\">1</trigger_flag>\r\n\t" +
                            "</trigger_time>\r\n" +
                            "</xml>";
                    }
                #endregion

                #region iDOLM@STERs SPMoveToSpecialVenue PUBLIC
                case "150":
                    string iDOLMASTERSSPMoveToSpecialVenuePUBLICTriggerPath = iDOLMASTERSSPMoveToSpecialVenueTriggerPath + "getEventTrigger.xml";
                    if (File.Exists(iDOLMASTERSSPMoveToSpecialVenuePUBLICTriggerPath))
                    {
                        LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - GetEventTrigger sent for PUBLIC THE IDOLM@STER SP Move to special live venue {eventId}!");
                        string res = File.ReadAllText(iDOLMASTERSSPMoveToSpecialVenuePUBLICTriggerPath);
                        return "<xml>\r\n\t" +
                             "<result type=\"int\">1</result>\r\n\t" +
                             "<description type=\"text\">Success</description>\r\n\t" +
                             "<error_no type=\"int\">0</error_no>\r\n\t" +
                             "<error_message type=\"text\">None</error_message>\r\n\r\n\t" +
                             $"<trigger_count type=\"int\">{readcnt}</trigger_count>\r\n\t" +
                             $"{res}\r\n" +
                             "</xml>";
                    }
                    else
                    {
                        LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - GetEventTrigger FALLBACK sent for PUBLIC THE IDOLM@STER SP Move to special live venue {eventId}! Expected path {iDOLMASTERSSPMoveToSpecialVenuePUBLICTriggerPath}");
                        return "<xml>\r\n" +
                            "<result type=\"int\">1</result>\r\n" +
                            "<description type=\"text\">Success</description>\r\n" +
                            "<error_no type=\"int\">0</error_no>\r\n" +
                            "<error_message type=\"text\">None</error_message>\r\n" +
                            $"<trigger_count type=\"int\">{readcnt}</trigger_count>\r\n\t" +
                            "<trigger_time>\r\n\t\t" +
                            "<trigger_id type=\"int\">1</trigger_id>\r\n" +
                            "\r\n\t\t<!-- Event Start Trigger --> \r\n\t\t" +
                            "<start_year type=\"int\">2024</start_year>\r\n\t\t" +
                            "<start_month type=\"int\">01</start_month>\r\n\t\t" +
                            "<start_day type=\"int\">15</start_day>\r\n\t\t" +
                            "<start_hour type=\"int\">00</start_hour>\r\n\t\t" +
                            "<start_minutes type=\"int\">00</start_minutes>\r\n\t\t" +
                            "<start_second type=\"int\">00</start_second>\r\n\r\n\t\t" +
                            "<end_year type=\"int\">2124</end_year>\r\n\t\t" +
                            "<end_month type=\"int\">02</end_month>\r\n\t\t" +
                            "<end_day type=\"int\">20</end_day>\r\n\t\t" +
                            "<end_hour type=\"int\">05</end_hour>\r\n\t\t" +
                            "<end_minutes type=\"int\">45</end_minutes>\r\n\t\t" +
                            "<end_second type=\"int\">0</end_second>\r\n\r\n\t\t" +
                            "<trigger_flag type=\"int\">1</trigger_flag>\r\n\t" +
                            "</trigger_time>\r\n" +
                            "</xml>";
                    }
                #endregion

                #region iDOLM@STERs SPMoveToSpecialVenue QA
                case "155":
                    string iDOLMASTERSSPMoveToSpecialVenueQATriggerPath = iDOLMASTERSSPMoveToSpecialVenueTriggerPath + "qagetEventTrigger.xml";
                    if (File.Exists(iDOLMASTERSSPMoveToSpecialVenueQATriggerPath))
                    {
                        LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - GetEventTrigger sent for QA THE IDOLM@STER SP Move to special live venue {eventId}!");
                        string res = File.ReadAllText(iDOLMASTERSSPMoveToSpecialVenueQATriggerPath);
                        return "<xml>\r\n\t" +
                             "<result type=\"int\">1</result>\r\n\t" +
                             "<description type=\"text\">Success</description>\r\n\t" +
                             "<error_no type=\"int\">0</error_no>\r\n\t" +
                             "<error_message type=\"text\">None</error_message>\r\n\r\n\t" +
                             $"<trigger_count type=\"int\">{readcnt}</trigger_count>\r\n\t" +
                             $"{res}\r\n" +
                             "</xml>";
                    }
                    else
                    {
                        LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - GetEventTrigger FALLBACK sent for QA THE IDOLM@STER SP Move to special live venue {eventId}! Expected path {iDOLMASTERSSPMoveToSpecialVenueQATriggerPath}");
                        return "<xml>\r\n" +
                            "<result type=\"int\">1</result>\r\n" +
                            "<description type=\"text\">Success</description>\r\n" +
                            "<error_no type=\"int\">0</error_no>\r\n" +
                            "<error_message type=\"text\">None</error_message>\r\n" +
                            $"<trigger_count type=\"int\">{readcnt}</trigger_count>\r\n\t" +
                            "<trigger_time>\r\n\t\t" +
                            "<trigger_id type=\"int\">1</trigger_id>\r\n" +
                            "\r\n\t\t<!-- Event Start Trigger --> \r\n\t\t" +
                            "<start_year type=\"int\">2024</start_year>\r\n\t\t" +
                            "<start_month type=\"int\">01</start_month>\r\n\t\t" +
                            "<start_day type=\"int\">15</start_day>\r\n\t\t" +
                            "<start_hour type=\"int\">00</start_hour>\r\n\t\t" +
                            "<start_minutes type=\"int\">00</start_minutes>\r\n\t\t" +
                            "<start_second type=\"int\">00</start_second>\r\n\r\n\t\t" +
                            "<end_year type=\"int\">2124</end_year>\r\n\t\t" +
                            "<end_month type=\"int\">02</end_month>\r\n\t\t" +
                            "<end_day type=\"int\">20</end_day>\r\n\t\t" +
                            "<end_hour type=\"int\">05</end_hour>\r\n\t\t" +
                            "<end_minutes type=\"int\">45</end_minutes>\r\n\t\t" +
                            "<end_second type=\"int\">0</end_second>\r\n\r\n\t\t" +
                            "<trigger_flag type=\"int\">1</trigger_flag>\r\n\t" +
                            "</trigger_time>\r\n" +
                            "</xml>";
                    }
                #endregion

                #region iDOLM@STERs SPMoveToSpecialVenue LOCAL
                case "172":
                    string iDOLMASTERSSPMoveToSpecialVenueLOCALTriggerPath = iDOLMASTERSSPMoveToSpecialVenueTriggerPath + "localgetEventTrigger.xml";
                    if (File.Exists(iDOLMASTERSSPMoveToSpecialVenueLOCALTriggerPath))
                    {
                        LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - GetEventTrigger sent for LOCAL THE IDOLM@STER SP Move to special live venue {eventId}!");
                        string res = File.ReadAllText(iDOLMASTERSSPMoveToSpecialVenueLOCALTriggerPath);
                        return "<xml>\r\n\t" +
                             "<result type=\"int\">1</result>\r\n\t" +
                             "<description type=\"text\">Success</description>\r\n\t" +
                             "<error_no type=\"int\">0</error_no>\r\n\t" +
                             "<error_message type=\"text\">None</error_message>\r\n\r\n\t" +
                             $"<trigger_count type=\"int\">{readcnt}</trigger_count>\r\n\t" +
                             $"{res}\r\n" +
                             "</xml>";
                    }
                    else
                    {
                        LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - GetEventTrigger FALLBACK sent for LOCAL THE IDOLM@STER SP Move to special live venue {eventId}! Expected path {iDOLMASTERSSPMoveToSpecialVenueLOCALTriggerPath}");
                        return "<xml>\r\n" +
                            "<result type=\"int\">1</result>\r\n" +
                            "<description type=\"text\">Success</description>\r\n" +
                            "<error_no type=\"int\">0</error_no>\r\n" +
                            "<error_message type=\"text\">None</error_message>\r\n" +
                            $"<trigger_count type=\"int\">{readcnt}</trigger_count>\r\n\t" +
                            "<trigger_time>\r\n\t\t" +
                            "<trigger_id type=\"int\">1</trigger_id>\r\n" +
                            "\r\n\t\t<!-- Event Start Trigger --> \r\n\t\t" +
                            "<start_year type=\"int\">2024</start_year>\r\n\t\t" +
                            "<start_month type=\"int\">01</start_month>\r\n\t\t" +
                            "<start_day type=\"int\">15</start_day>\r\n\t\t" +
                            "<start_hour type=\"int\">00</start_hour>\r\n\t\t" +
                            "<start_minutes type=\"int\">00</start_minutes>\r\n\t\t" +
                            "<start_second type=\"int\">00</start_second>\r\n\r\n\t\t" +
                            "<end_year type=\"int\">2124</end_year>\r\n\t\t" +
                            "<end_month type=\"int\">02</end_month>\r\n\t\t" +
                            "<end_day type=\"int\">20</end_day>\r\n\t\t" +
                            "<end_hour type=\"int\">05</end_hour>\r\n\t\t" +
                            "<end_minutes type=\"int\">45</end_minutes>\r\n\t\t" +
                            "<end_second type=\"int\">0</end_second>\r\n\r\n\t\t" +
                            "<trigger_flag type=\"int\">1</trigger_flag>\r\n\t" +
                            "</trigger_time>\r\n" +
                            "</xml>";
                    }
                #endregion

                #region homecafe Rolly1F LOCAL
                case "174":
                    string homecafeRollyCafe1FLOCALTriggerPath = homecafeRollyCafe1FTriggerPath + "localgetEventTrigger.xml";
                    if (File.Exists(homecafeRollyCafe1FLOCALTriggerPath))
                    {
                        LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - GetEventTrigger sent for LOCAL RollyCafe1F {eventId}!");
                        string res = File.ReadAllText(homecafeRollyCafe1FLOCALTriggerPath);
                        return "<xml>\r\n\t" +
                             "<result type=\"int\">1</result>\r\n\t" +
                             "<description type=\"text\">Success</description>\r\n\t" +
                             "<error_no type=\"int\">0</error_no>\r\n\t" +
                             "<error_message type=\"text\">None</error_message>\r\n\r\n\t" +
                             $"<trigger_count type=\"int\">{readcnt}</trigger_count>\r\n\t" +
                             $"{res}\r\n" +
                             "</xml>";
                    }
                    else
                    {
                        LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - GetEventTrigger FALLBACK sent for LOCAL RollyCafe1F {eventId}! Expected path {homecafeRollyCafe1FLOCALTriggerPath}");
                        return "<xml>\r\n" +
                            "<result type=\"int\">1</result>\r\n" +
                            "<description type=\"text\">Success</description>\r\n" +
                            "<error_no type=\"int\">0</error_no>\r\n" +
                            "<error_message type=\"text\">None</error_message>\r\n" +
                            $"<trigger_count type=\"int\">{readcnt}</trigger_count>\r\n\t" +
                            "<trigger_time>\r\n\t\t" +
                            "<trigger_id type=\"int\">1</trigger_id>\r\n" +
                            "\r\n\t\t<!-- Event Start Trigger --> \r\n\t\t" +
                            "<start_year type=\"int\">2024</start_year>\r\n\t\t" +
                            "<start_month type=\"int\">01</start_month>\r\n\t\t" +
                            "<start_day type=\"int\">15</start_day>\r\n\t\t" +
                            "<start_hour type=\"int\">00</start_hour>\r\n\t\t" +
                            "<start_minutes type=\"int\">00</start_minutes>\r\n\t\t" +
                            "<start_second type=\"int\">00</start_second>\r\n\r\n\t\t" +
                            "<end_year type=\"int\">2124</end_year>\r\n\t\t" +
                            "<end_month type=\"int\">02</end_month>\r\n\t\t" +
                            "<end_day type=\"int\">20</end_day>\r\n\t\t" +
                            "<end_hour type=\"int\">05</end_hour>\r\n\t\t" +
                            "<end_minutes type=\"int\">45</end_minutes>\r\n\t\t" +
                            "<end_second type=\"int\">0</end_second>\r\n\r\n\t\t" +
                            "<trigger_flag type=\"int\">1</trigger_flag>\r\n\t" +
                            "</trigger_time>\r\n" +
                            "</xml>";
                    }
                #endregion

                #region homecafe Rolly1F LOCAL
                case "179":
                    string homecafeRollyCafe1FQATriggerPath = homecafeRollyCafe1FTriggerPath + "qagetEventTrigger.xml";
                    if (File.Exists(homecafeRollyCafe1FQATriggerPath))
                    {
                        LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - GetEventTrigger sent for QA RollyCafe1F {eventId}!");
                        string res = File.ReadAllText(homecafeRollyCafe1FQATriggerPath);
                        return "<xml>\r\n\t" +
                             "<result type=\"int\">1</result>\r\n\t" +
                             "<description type=\"text\">Success</description>\r\n\t" +
                             "<error_no type=\"int\">0</error_no>\r\n\t" +
                             "<error_message type=\"text\">None</error_message>\r\n\r\n\t" +
                             $"<trigger_count type=\"int\">{readcnt}</trigger_count>\r\n\t" +
                             $"{res}\r\n" +
                             "</xml>";
                    }
                    else
                    {
                        LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - GetEventTrigger FALLBACK sent for QA RollyCafe1F {eventId}! Expected path {homecafeRollyCafe1FQATriggerPath}");
                        return "<xml>\r\n" +
                            "<result type=\"int\">1</result>\r\n" +
                            "<description type=\"text\">Success</description>\r\n" +
                            "<error_no type=\"int\">0</error_no>\r\n" +
                            "<error_message type=\"text\">None</error_message>\r\n" +
                            $"<trigger_count type=\"int\">{readcnt}</trigger_count>\r\n\t" +
                            "<trigger_time>\r\n\t\t" +
                            "<trigger_id type=\"int\">1</trigger_id>\r\n" +
                            "\r\n\t\t<!-- Event Start Trigger --> \r\n\t\t" +
                            "<start_year type=\"int\">2024</start_year>\r\n\t\t" +
                            "<start_month type=\"int\">01</start_month>\r\n\t\t" +
                            "<start_day type=\"int\">15</start_day>\r\n\t\t" +
                            "<start_hour type=\"int\">00</start_hour>\r\n\t\t" +
                            "<start_minutes type=\"int\">00</start_minutes>\r\n\t\t" +
                            "<start_second type=\"int\">00</start_second>\r\n\r\n\t\t" +
                            "<end_year type=\"int\">2124</end_year>\r\n\t\t" +
                            "<end_month type=\"int\">02</end_month>\r\n\t\t" +
                            "<end_day type=\"int\">20</end_day>\r\n\t\t" +
                            "<end_hour type=\"int\">05</end_hour>\r\n\t\t" +
                            "<end_minutes type=\"int\">45</end_minutes>\r\n\t\t" +
                            "<end_second type=\"int\">0</end_second>\r\n\r\n\t\t" +
                            "<trigger_flag type=\"int\">1</trigger_flag>\r\n\t" +
                            "</trigger_time>\r\n" +
                            "</xml>";
                    }
                #endregion

                #region Basara HomeSquare Collab Local/QA
                case "180":
                    string basaraHomeSquareLOCALTriggerPath = basaraHomeSquareTriggerPath + "localgetEventTrigger.xml";
                    if (File.Exists(basaraHomeSquareLOCALTriggerPath))
                    {
                        LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - GetEventTrigger sent for LOCAL Basara {eventId}!");
                        string res = File.ReadAllText(basaraHomeSquareLOCALTriggerPath);
                        return "<xml>\r\n\t" +
                             "<result type=\"int\">1</result>\r\n\t" +
                             "<description type=\"text\">Success</description>\r\n\t" +
                             "<error_no type=\"int\">0</error_no>\r\n\t" +
                             "<error_message type=\"text\">None</error_message>\r\n\r\n\t" +
                             $"<trigger_count type=\"int\">{readcnt}</trigger_count>\r\n\t" +
                             $"{res}\r\n" +
                             "</xml>";
                    }
                    else
                    {
                        LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - GetEventTrigger FALLBACK sent for LOCAL Basara {eventId}! Expected path {basaraHomeSquareLOCALTriggerPath}");
                        return "<xml>\r\n" +
                            "<result type=\"int\">1</result>\r\n" +
                            "<description type=\"text\">Success</description>\r\n" +
                            "<error_no type=\"int\">0</error_no>\r\n" +
                            "<error_message type=\"text\">None</error_message>\r\n" +
                            $"<trigger_count type=\"int\">{readcnt}</trigger_count>\r\n\t" +
                            "<trigger_time>\r\n\t\t" +
                            "<trigger_id type=\"int\">1</trigger_id>\r\n" +
                            "\r\n\t\t<!-- Event Start Trigger --> \r\n\t\t" +
                            "<start_year type=\"int\">2024</start_year>\r\n\t\t" +
                            "<start_month type=\"int\">01</start_month>\r\n\t\t" +
                            "<start_day type=\"int\">15</start_day>\r\n\t\t" +
                            "<start_hour type=\"int\">00</start_hour>\r\n\t\t" +
                            "<start_minutes type=\"int\">00</start_minutes>\r\n\t\t" +
                            "<start_second type=\"int\">00</start_second>\r\n\r\n\t\t" +
                            "<end_year type=\"int\">2124</end_year>\r\n\t\t" +
                            "<end_month type=\"int\">02</end_month>\r\n\t\t" +
                            "<end_day type=\"int\">20</end_day>\r\n\t\t" +
                            "<end_hour type=\"int\">05</end_hour>\r\n\t\t" +
                            "<end_minutes type=\"int\">45</end_minutes>\r\n\t\t" +
                            "<end_second type=\"int\">0</end_second>\r\n\r\n\t\t" +
                            "<trigger_flag type=\"int\">1</trigger_flag>\r\n\t" +
                            "</trigger_time>\r\n" +
                            "</xml>";
                    }

                case "192":
                    string basaraHomeSquareQATriggerPath = basaraHomeSquareTriggerPath + "qagetEventTrigger.xml";
                    if (File.Exists(basaraHomeSquareQATriggerPath))
                    {
                        LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - GetEventTrigger sent for QA Basara {eventId}!");
                        string res = File.ReadAllText(basaraHomeSquareQATriggerPath);
                        return "<xml>\r\n\t" +
                             "<result type=\"int\">1</result>\r\n\t" +
                             "<description type=\"text\">Success</description>\r\n\t" +
                             "<error_no type=\"int\">0</error_no>\r\n\t" +
                             "<error_message type=\"text\">None</error_message>\r\n\r\n\t" +
                             $"<trigger_count type=\"int\">{readcnt}</trigger_count>\r\n\t" +
                             $"{res}\r\n" +
                             "</xml>";
                    }
                    else
                    {
                        LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - GetEventTrigger FALLBACK sent for QA Basara {eventId}! Expected path {basaraHomeSquareQATriggerPath}");
                        return "<xml>\r\n" +
                            "<result type=\"int\">1</result>\r\n" +
                            "<description type=\"text\">Success</description>\r\n" +
                            "<error_no type=\"int\">0</error_no>\r\n" +
                            "<error_message type=\"text\">None</error_message>\r\n" +
                            $"<trigger_count type=\"int\">{readcnt}</trigger_count>\r\n\t" +
                            "<trigger_time>\r\n\t\t" +
                            "<trigger_id type=\"int\">1</trigger_id>\r\n" +
                            "\r\n\t\t<!-- Event Start Trigger --> \r\n\t\t" +
                            "<start_year type=\"int\">2024</start_year>\r\n\t\t" +
                            "<start_month type=\"int\">01</start_month>\r\n\t\t" +
                            "<start_day type=\"int\">15</start_day>\r\n\t\t" +
                            "<start_hour type=\"int\">00</start_hour>\r\n\t\t" +
                            "<start_minutes type=\"int\">00</start_minutes>\r\n\t\t" +
                            "<start_second type=\"int\">00</start_second>\r\n\r\n\t\t" +
                            "<end_year type=\"int\">2124</end_year>\r\n\t\t" +
                            "<end_month type=\"int\">02</end_month>\r\n\t\t" +
                            "<end_day type=\"int\">20</end_day>\r\n\t\t" +
                            "<end_hour type=\"int\">05</end_hour>\r\n\t\t" +
                            "<end_minutes type=\"int\">45</end_minutes>\r\n\t\t" +
                            "<end_second type=\"int\">0</end_second>\r\n\r\n\t\t" +
                            "<trigger_flag type=\"int\">1</trigger_flag>\r\n\t" +
                            "</trigger_time>\r\n" +
                            "</xml>";
                    }

                #endregion

                #region homecafe DJMusic
                case "197":
                    string homecafeDJMusicPublicTriggerPath = homecafeDJMusicTriggerPath + "getEventTrigger.xml";
                    if (File.Exists(homecafeDJMusicPublicTriggerPath))
                    {
                        LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - GetEventTrigger sent for PUBLIC DJMusic {eventId}!");
                        string res = File.ReadAllText(homecafeDJMusicPublicTriggerPath);
                        return "<xml>\r\n\t" +
                             "<result type=\"int\">1</result>\r\n\t" +
                             "<description type=\"text\">Success</description>\r\n\t" +
                             "<error_no type=\"int\">0</error_no>\r\n\t" +
                             "<error_message type=\"text\">None</error_message>\r\n\r\n\t" +
                             $"<trigger_count type=\"int\">{readcnt}</trigger_count>\r\n\t" +
                             $"{res}\r\n" +
                             "</xml>";
                    }
                    else
                    {
                        LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - GetEventTrigger FALLBACK sent for PUBLIC DJMusic {eventId}! Expected path {homecafeDJMusicPublicTriggerPath}");
                        return "<xml>\r\n" +
                            "<result type=\"int\">1</result>\r\n" +
                            "<description type=\"text\">Success</description>\r\n" +
                            "<error_no type=\"int\">0</error_no>\r\n" +
                            "<error_message type=\"text\">None</error_message>\r\n" +
                            $"<trigger_count type=\"int\">{readcnt}</trigger_count>\r\n\t" +
                            "<trigger_time>\r\n\t\t" +
                            "<trigger_id type=\"int\">1</trigger_id>\r\n" +
                            "\r\n\t\t<!-- Event Start Trigger --> \r\n\t\t" +
                            "<start_year type=\"int\">2024</start_year>\r\n\t\t" +
                            "<start_month type=\"int\">01</start_month>\r\n\t\t" +
                            "<start_day type=\"int\">15</start_day>\r\n\t\t" +
                            "<start_hour type=\"int\">00</start_hour>\r\n\t\t" +
                            "<start_minutes type=\"int\">00</start_minutes>\r\n\t\t" +
                            "<start_second type=\"int\">00</start_second>\r\n\r\n\t\t" +
                            "<end_year type=\"int\">2124</end_year>\r\n\t\t" +
                            "<end_month type=\"int\">02</end_month>\r\n\t\t" +
                            "<end_day type=\"int\">20</end_day>\r\n\t\t" +
                            "<end_hour type=\"int\">05</end_hour>\r\n\t\t" +
                            "<end_minutes type=\"int\">45</end_minutes>\r\n\t\t" +
                            "<end_second type=\"int\">0</end_second>\r\n\r\n\t\t" +
                            "<trigger_flag type=\"int\">1</trigger_flag>\r\n\t" +
                            "</trigger_time>\r\n" +
                            "</xml>";
                    }
                #endregion

                #region homecafe RollyCafe1F PUBLIC
                case "201":
                    string homecafeRollyCafe1FPUBLICTriggerPath = homecafeRollyCafe1FTriggerPath + "getEventTrigger.xml";
                    if (File.Exists(homecafeRollyCafe1FPUBLICTriggerPath))
                    {
                        LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - GetEventTrigger sent for PUBLIC RollyCafe1F {eventId}!");
                        string res = File.ReadAllText(homecafeRollyCafe1FPUBLICTriggerPath);
                        return "<xml>\r\n\t" +
                             "<result type=\"int\">1</result>\r\n\t" +
                             "<description type=\"text\">Success</description>\r\n\t" +
                             "<error_no type=\"int\">0</error_no>\r\n\t" +
                             "<error_message type=\"text\">None</error_message>\r\n\r\n\t" +
                             $"<trigger_count type=\"int\">{readcnt}</trigger_count>\r\n\t" +
                             $"{res}\r\n" +
                             "</xml>";
                    }
                    else
                    {
                        LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - GetEventTrigger FALLBACK sent for PUBLIC RollyCafe1F {eventId}! Expected path {homecafeRollyCafe1FPUBLICTriggerPath}");
                        return "<xml>\r\n" +
                            "<result type=\"int\">1</result>\r\n" +
                            "<description type=\"text\">Success</description>\r\n" +
                            "<error_no type=\"int\">0</error_no>\r\n" +
                            "<error_message type=\"text\">None</error_message>\r\n" +
                            $"<trigger_count type=\"int\">{readcnt}</trigger_count>\r\n\t" +
                            "<trigger_time>\r\n\t\t" +
                            "<trigger_id type=\"int\">1</trigger_id>\r\n" +
                            "\r\n\t\t<!-- Event Start Trigger --> \r\n\t\t" +
                            "<start_year type=\"int\">2024</start_year>\r\n\t\t" +
                            "<start_month type=\"int\">01</start_month>\r\n\t\t" +
                            "<start_day type=\"int\">15</start_day>\r\n\t\t" +
                            "<start_hour type=\"int\">00</start_hour>\r\n\t\t" +
                            "<start_minutes type=\"int\">00</start_minutes>\r\n\t\t" +
                            "<start_second type=\"int\">00</start_second>\r\n\r\n\t\t" +
                            "<end_year type=\"int\">2124</end_year>\r\n\t\t" +
                            "<end_month type=\"int\">02</end_month>\r\n\t\t" +
                            "<end_day type=\"int\">20</end_day>\r\n\t\t" +
                            "<end_hour type=\"int\">05</end_hour>\r\n\t\t" +
                            "<end_minutes type=\"int\">45</end_minutes>\r\n\t\t" +
                            "<end_second type=\"int\">0</end_second>\r\n\r\n\t\t" +
                            "<trigger_flag type=\"int\">1</trigger_flag>\r\n\t" +
                            "</trigger_time>\r\n" +
                            "</xml>";
                    }
                #endregion

                #region Basara HomeSquare Collab PUBLIC
                case "210":
                    string basaraHomeSquarePUBLICTriggerPath = basaraHomeSquareTriggerPath + "getEventTrigger.xml";
                    if (File.Exists(basaraHomeSquarePUBLICTriggerPath))
                    {
                        LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - GetEventTrigger sent for PUBLIC Basara {eventId}!");
                        string res = File.ReadAllText(basaraHomeSquarePUBLICTriggerPath);
                        return "<xml>\r\n\t" +
                             "<result type=\"int\">1</result>\r\n\t" +
                             "<description type=\"text\">Success</description>\r\n\t" +
                             "<error_no type=\"int\">0</error_no>\r\n\t" +
                             "<error_message type=\"text\">None</error_message>\r\n\r\n\t" +
                             $"<trigger_count type=\"int\">{readcnt}</trigger_count>\r\n\t" +
                             $"{res}\r\n" +
                             "</xml>";
                    }
                    else
                    {
                        LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - GetEventTrigger FALLBACK sent for PUBLIC Basara {eventId}! Expected path {basaraHomeSquarePUBLICTriggerPath}");
                        return "<xml>\r\n" +
                            "<result type=\"int\">1</result>\r\n" +
                            "<description type=\"text\">GET_TRIGGER</description>\r\n" +
                            "<error_no type=\"int\">0</error_no>\r\n" +
                            "<error_message type=\"text\">None</error_message>\r\n" +
                            $"<trigger_count type=\"int\">{readcnt}</trigger_count>\r\n\t" +
                            "<trigger_time>\r\n\t\t" +
                            "<is_active type=\"bool\">true</is_active>\r\n" +
                            "<trigger_id type=\"int\">1</trigger_id>\r\n" +
                            "\r\n\t\t<!-- Event Start Trigger --> \r\n\t\t" +
                            $"<start_year type=\"int\">{currentTime.ToString("yyyy")}</start_year>\r\n\t\t" +
                            $"<start_month type=\"int\">{currentTime.ToString("MM")}</start_month>\r\n\t\t" +
                            $"<start_day type=\"int\">{currentTime.ToString("dd")}</start_day>\r\n\t\t" +
                            $"<start_hour type=\"int\">{currentTime.ToString("HH")}</start_hour>\r\n\t\t" +
                            $"<start_minutes type=\"int\">{currentTime.AddMinutes(2).ToString("mm")}</start_minutes>\r\n\t\t" +
                            $"<start_second type=\"int\">{currentTime.ToString("ss")}</start_second>\r\n\r\n\t\t" +
                            $"<end_year type=\"int\">{currentTime.AddYears(1).ToString("yyyy")}</end_year>\r\n\t\t" +
                            $"<end_month type=\"int\">{currentTime.ToString("MM")}</end_month>\r\n\t\t" +
                            $"<end_day type=\"int\">{currentTime.ToString("dd")}</end_day>\r\n\t\t" +
                            $"<end_hour type=\"int\">{currentTime.ToString("HH")}</end_hour>\r\n\t\t" +
                            $"<end_minutes type=\"int\">{currentTime.ToString("mm")}</end_minutes>\r\n\t\t" +
                            $"<end_second type=\"int\">{currentTime.ToString("ss")}</end_second>\r\n\r\n\t\t" +
                            "<trigger_flag type=\"int\">1</trigger_flag>\r\n\t" +
                            "</trigger_time>\r\n" +
                            "<trigger_time>\r\n\t\t" +
                            "<is_active type=\"bool\">true</is_active>\r\n" +
                            "<trigger_id type=\"int\">2</trigger_id>\r\n" +
                            "\r\n\t\t<!-- Event Start Trigger --> \r\n\t\t" +
                            $"<start_year type=\"int\">{currentTime.ToString("yyyy")}</start_year>\r\n\t\t" +
                            $"<start_month type=\"int\">{currentTime.ToString("MM")}</start_month>\r\n\t\t" +
                            $"<start_day type=\"int\">{currentTime.ToString("dd")}</start_day>\r\n\t\t" +
                            $"<start_hour type=\"int\">{currentTime.ToString("HH")}</start_hour>\r\n\t\t" +
                            $"<start_minutes type=\"int\">{currentTime.AddMinutes(2).ToString("mm")}</start_minutes>\r\n\t\t" +
                            $"<start_second type=\"int\">{currentTime.ToString("ss")}</start_second>\r\n\r\n\t\t" +
                            $"<end_year type=\"int\">{currentTime.ToString("yyyy")}</end_year>\r\n\t\t" +
                            $"<end_month type=\"int\">{currentTime.ToString("MM")}</end_month>\r\n\t\t" +
                            $"<end_day type=\"int\">{currentTime.AddDays(30).ToString("dd")}</end_day>\r\n\t\t" +
                            $"<end_hour type=\"int\">{currentTime.ToString("HH")}</end_hour>\r\n\t\t" +
                            $"<end_minutes type=\"int\">{currentTime.ToString("mm")}</end_minutes>\r\n\t\t" +
                            $"<end_second type=\"int\">{currentTime.ToString("ss")}</end_second>\r\n\r\n\t\t" +
                            "<trigger_flag type=\"int\">1</trigger_flag>\r\n\t" +
                            "</trigger_time>\r\n" +
                            "</xml>";
                    }
                #endregion

                #region homecafe hc_gallery PUBLIC
                case "264":
                    string homecafeGalleryPUBLICTriggerPath = homecafeGalleryTriggerPath + "getEventTrigger.xml";
                    if (File.Exists(homecafeGalleryPUBLICTriggerPath))
                    {
                        LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - GetEventTrigger sent for PUBLIC homecafe Gallery {eventId}!");
                        string res = File.ReadAllText(homecafeGalleryPUBLICTriggerPath);
                        return "<xml>\r\n\t" +
                             "<result type=\"int\">1</result>\r\n\t" +
                             "<description type=\"text\">Success</description>\r\n\t" +
                             "<error_no type=\"int\">0</error_no>\r\n\t" +
                             "<error_message type=\"text\">None</error_message>\r\n\r\n\t" +
                             $"<trigger_count type=\"int\">{readcnt}</trigger_count>\r\n\t" +
                             $"{res}\r\n" +
                             "</xml>";
                    }
                    else
                    {
                        LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - GetEventTrigger FALLBACK sent for PUBLIC homecafe Gallery {eventId}! Expected path {homecafeGalleryPUBLICTriggerPath}");
                        return "<xml>\r\n" +
                            "<result type=\"int\">1</result>\r\n" +
                            "<description type=\"text\">Success</description>\r\n" +
                            "<error_no type=\"int\">0</error_no>\r\n" +
                            "<error_message type=\"text\">None</error_message>\r\n" +
                            $"<trigger_count type=\"int\">{readcnt}</trigger_count>\r\n\t" +
                            "<trigger_time>\r\n\t\t" +
                            "<trigger_id type=\"int\">1</trigger_id>\r\n" +
                            "\r\n\t\t<!-- Event Start Trigger --> \r\n\t\t" +
                            "<start_year type=\"int\">2024</start_year>\r\n\t\t" +
                            "<start_month type=\"int\">01</start_month>\r\n\t\t" +
                            "<start_day type=\"int\">15</start_day>\r\n\t\t" +
                            "<start_hour type=\"int\">00</start_hour>\r\n\t\t" +
                            "<start_minutes type=\"int\">00</start_minutes>\r\n\t\t" +
                            "<start_second type=\"int\">00</start_second>\r\n\r\n\t\t" +
                            "<end_year type=\"int\">2124</end_year>\r\n\t\t" +
                            "<end_month type=\"int\">02</end_month>\r\n\t\t" +
                            "<end_day type=\"int\">20</end_day>\r\n\t\t" +
                            "<end_hour type=\"int\">05</end_hour>\r\n\t\t" +
                            "<end_minutes type=\"int\">45</end_minutes>\r\n\t\t" +
                            "<end_second type=\"int\">0</end_second>\r\n\r\n\t\t" +
                            "<trigger_flag type=\"int\">1</trigger_flag>\r\n\t" +
                            "</trigger_time>\r\n" +
                            "</xml>";
                    }
                #endregion

                #region j_liargame2 POST
                case "296":
                    string j_liargame2POSTPUBLICTriggerPath = j_liargame2TriggerPath + "getEventTrigger_POST.xml";
                    if (File.Exists(j_liargame2POSTPUBLICTriggerPath))
                    {
                        LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - GetEventTrigger sent for POST evid PUBLIC LiarGame2 {eventId}!");
                        string res = File.ReadAllText(j_liargame2POSTPUBLICTriggerPath);
                        return "<xml>\r\n\t" +
                             "<result type=\"int\">1</result>\r\n\t" +
                             "<description type=\"text\">Success</description>\r\n\t" +
                             "<error_no type=\"int\">0</error_no>\r\n\t" +
                             "<error_message type=\"text\">None</error_message>\r\n\r\n\t" +
                             $"<trigger_count type=\"int\">{readcnt}</trigger_count>\r\n\t" +
                             $"{res}\r\n" +
                             "</xml>";
                    }
                    else
                    {
                        LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - GetEventTrigger FALLBACK sent for POST evid PUBLIC LiarGame2 {eventId}! Expected path {j_liargame2POSTPUBLICTriggerPath}");
                        return "<xml>\r\n" +
                            "<result type=\"int\">1</result>\r\n" +
                            "<description type=\"text\">Success</description>\r\n" +
                            "<error_no type=\"int\">0</error_no>\r\n" +
                            "<error_message type=\"text\">None</error_message>\r\n" +
                            $"<trigger_count type=\"int\">{readcnt}</trigger_count>\r\n\t" +
                            "<trigger_time>\r\n\t\t" +
                            "<trigger_id type=\"int\">1</trigger_id>\r\n" +
                            "\r\n\t\t<!-- Event Start Trigger --> \r\n\t\t" +
                            "<start_year type=\"int\">2024</start_year>\r\n\t\t" +
                            "<start_month type=\"int\">01</start_month>\r\n\t\t" +
                            "<start_day type=\"int\">15</start_day>\r\n\t\t" +
                            "<start_hour type=\"int\">00</start_hour>\r\n\t\t" +
                            "<start_minutes type=\"int\">00</start_minutes>\r\n\t\t" +
                            "<start_second type=\"int\">00</start_second>\r\n\r\n\t\t" +
                            "<end_year type=\"int\">2124</end_year>\r\n\t\t" +
                            "<end_month type=\"int\">02</end_month>\r\n\t\t" +
                            "<end_day type=\"int\">20</end_day>\r\n\t\t" +
                            "<end_hour type=\"int\">05</end_hour>\r\n\t\t" +
                            "<end_minutes type=\"int\">45</end_minutes>\r\n\t\t" +
                            "<end_second type=\"int\">0</end_second>\r\n\r\n\t\t" +
                            "<trigger_flag type=\"int\">1</trigger_flag>\r\n\t" +
                            "</trigger_time>\r\n" +
                            "</xml>";
                    }
                #endregion

                #region j_liargame2 CALL
                case "297":
                    string j_liargame2CALLPUBLICTriggerPath = j_liargame2TriggerPath + "getEventTrigger_CALL.xml";
                    if (File.Exists(j_liargame2CALLPUBLICTriggerPath))
                    {
                        LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - GetEventTrigger sent for CALL evid PUBLIC LiarGame2 {eventId}!");
                        string res = File.ReadAllText(j_liargame2CALLPUBLICTriggerPath);
                        return "<xml>\r\n\t" +
                             "<result type=\"int\">1</result>\r\n\t" +
                             "<description type=\"text\">Success</description>\r\n\t" +
                             "<error_no type=\"int\">0</error_no>\r\n\t" +
                             "<error_message type=\"text\">None</error_message>\r\n\r\n\t" +
                             $"<trigger_count type=\"int\">{readcnt}</trigger_count>\r\n\t" +
                             $"{res}\r\n" +
                             "</xml>";
                    }
                    else
                    {
                        LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - GetEventTrigger FALLBACK sent for CALL evid PUBLIC LiarGame2 {eventId}! Expected path {j_liargame2CALLPUBLICTriggerPath}");
                        return "<xml>\r\n" +
                            "<result type=\"int\">1</result>\r\n" +
                            "<description type=\"text\">Success</description>\r\n" +
                            "<error_no type=\"int\">0</error_no>\r\n" +
                            "<error_message type=\"text\">None</error_message>\r\n" +
                            $"<trigger_count type=\"int\">{readcnt}</trigger_count>\r\n\t" +
                            "<trigger_time>\r\n\t\t" +
                            "<trigger_id type=\"int\">1</trigger_id>\r\n" +
                            "\r\n\t\t<!-- Event Start Trigger --> \r\n\t\t" +
                            "<start_year type=\"int\">2024</start_year>\r\n\t\t" +
                            "<start_month type=\"int\">01</start_month>\r\n\t\t" +
                            "<start_day type=\"int\">15</start_day>\r\n\t\t" +
                            "<start_hour type=\"int\">00</start_hour>\r\n\t\t" +
                            "<start_minutes type=\"int\">00</start_minutes>\r\n\t\t" +
                            "<start_second type=\"int\">00</start_second>\r\n\r\n\t\t" +
                            "<end_year type=\"int\">2124</end_year>\r\n\t\t" +
                            "<end_month type=\"int\">02</end_month>\r\n\t\t" +
                            "<end_day type=\"int\">20</end_day>\r\n\t\t" +
                            "<end_hour type=\"int\">05</end_hour>\r\n\t\t" +
                            "<end_minutes type=\"int\">45</end_minutes>\r\n\t\t" +
                            "<end_second type=\"int\">0</end_second>\r\n\r\n\t\t" +
                            "<trigger_flag type=\"int\">1</trigger_flag>\r\n\t" +
                            "</trigger_time>\r\n" +
                            "</xml>";
                    }
                #endregion

                #region j_liargame2 CALL
                case "298":
                    string halloween2010TriggerPathPublic = halloween2010TriggerPath + "getEventTrigger.xml";
                    if (File.Exists(halloween2010TriggerPathPublic))
                    {
                        LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - GetEventTrigger sent for evid PUBLIC Halloween 2010 {eventId}!");
                        string res = File.ReadAllText(halloween2010TriggerPathPublic);
                        return "<xml>\r\n\t" +
                             "<result type=\"int\">1</result>\r\n\t" +
                             "<description type=\"text\">Success</description>\r\n\t" +
                             "<error_no type=\"int\">0</error_no>\r\n\t" +
                             "<error_message type=\"text\">None</error_message>\r\n\r\n\t" +
                             $"<trigger_count type=\"int\">{readcnt}</trigger_count>\r\n\t" +
                             $"{res}\r\n" +
                             "</xml>";
                    }
                    else
                    {
                        LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - GetEventTrigger FALLBACK sent for evid PUBLIC Halloween 2010 {eventId}! Expected path {halloween2010TriggerPathPublic}");
                        return "<xml>\r\n" +
                            "<result type=\"int\">1</result>\r\n" +
                            "<description type=\"text\">Success</description>\r\n" +
                            "<error_no type=\"int\">0</error_no>\r\n" +
                            "<error_message type=\"text\">None</error_message>\r\n" +
                            $"<trigger_count type=\"int\">{readcnt}</trigger_count>\r\n\t" +
                            "<trigger_time>\r\n\t\t" +
                            "<trigger_id type=\"int\">1</trigger_id>\r\n" +
                            "\r\n\t\t<!-- Event Start Trigger --> \r\n\t\t" +
                            "<start_year type=\"int\">2024</start_year>\r\n\t\t" +
                            "<start_month type=\"int\">01</start_month>\r\n\t\t" +
                            "<start_day type=\"int\">15</start_day>\r\n\t\t" +
                            "<start_hour type=\"int\">00</start_hour>\r\n\t\t" +
                            "<start_minutes type=\"int\">00</start_minutes>\r\n\t\t" +
                            "<start_second type=\"int\">00</start_second>\r\n\r\n\t\t" +
                            "<end_year type=\"int\">2124</end_year>\r\n\t\t" +
                            "<end_month type=\"int\">02</end_month>\r\n\t\t" +
                            "<end_day type=\"int\">20</end_day>\r\n\t\t" +
                            "<end_hour type=\"int\">05</end_hour>\r\n\t\t" +
                            "<end_minutes type=\"int\">45</end_minutes>\r\n\t\t" +
                            "<end_second type=\"int\">0</end_second>\r\n\r\n\t\t" +
                            "<trigger_flag type=\"int\">1</trigger_flag>\r\n\t" +
                            "</trigger_time>\r\n" +
                            "</xml>";
                    }
                #endregion

                #region Sony Aquarium Relocator PUBLIC
                case "346":
                    string SonyAquariumRelocatorPUBLICTriggerPath = SonyAquariumRelocatorTriggerPath + "getEventTrigger.xml";
                    if (File.Exists(SonyAquariumRelocatorPUBLICTriggerPath))
                    {
                        LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - GetEventTrigger sent for PUBLIC Sony Aquarium HomeSquare Relocator {eventId}!");
                        string res = File.ReadAllText(SonyAquariumRelocatorPUBLICTriggerPath);
                        return "<xml>\r\n\t" +
                             "<result type=\"int\">1</result>\r\n\t" +
                             "<description type=\"text\">Success</description>\r\n\t" +
                             "<error_no type=\"int\">0</error_no>\r\n\t" +
                             "<error_message type=\"text\">None</error_message>\r\n\r\n\t" +
                             $"<trigger_count type=\"int\">{readcnt}</trigger_count>\r\n\t" +
                             $"{res}\r\n" +
                             "</xml>";
                    }
                    else
                    {
                        LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - GetEventTrigger FALLBACK sent for PUBLIC Sony Aquarium HomeSquare Relocator {eventId}! Expected path {SonyAquariumRelocatorPUBLICTriggerPath}");
                        return "<xml>\r\n" +
                            "<result type=\"int\">1</result>\r\n" +
                            "<description type=\"text\">Success</description>\r\n" +
                            "<error_no type=\"int\">0</error_no>\r\n" +
                            "<error_message type=\"text\">None</error_message>\r\n" +
                            $"<trigger_count type=\"int\">{readcnt}</trigger_count>\r\n\t" +
                            "<trigger_time>\r\n\t\t" +
                            "<trigger_id type=\"int\">1</trigger_id>\r\n" +
                            "\r\n\t\t<!-- Event Start Trigger --> \r\n\t\t" +
                            "<start_year type=\"int\">2024</start_year>\r\n\t\t" +
                            "<start_month type=\"int\">01</start_month>\r\n\t\t" +
                            "<start_day type=\"int\">15</start_day>\r\n\t\t" +
                            "<start_hour type=\"int\">00</start_hour>\r\n\t\t" +
                            "<start_minutes type=\"int\">00</start_minutes>\r\n\t\t" +
                            "<start_second type=\"int\">00</start_second>\r\n\r\n\t\t" +
                            "<end_year type=\"int\">2124</end_year>\r\n\t\t" +
                            "<end_month type=\"int\">02</end_month>\r\n\t\t" +
                            "<end_day type=\"int\">20</end_day>\r\n\t\t" +
                            "<end_hour type=\"int\">05</end_hour>\r\n\t\t" +
                            "<end_minutes type=\"int\">45</end_minutes>\r\n\t\t" +
                            "<end_second type=\"int\">0</end_second>\r\n\r\n\t\t" +
                            "<trigger_flag type=\"int\">1</trigger_flag>\r\n\t" +
                            "</trigger_time>\r\n" +
                            "</xml>";
                    }
                #endregion

                #region Sony Aquarium VideoConfig PUBLIC
                case "349":
                    if (File.Exists($"{workpath}/eventController/SonyAquarium/VideoConfig/getEventTrigger.xml"))
                        return File.ReadAllText($"{workpath}/eventController/SonyAquarium/VideoConfig/getEventTrigger.xml");
                    LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - GetEventTrigger sent for PUBLIC Sony Aquarium VideoConfig {eventId}!");

                    string SonyAquariumVideoConfigPUBLICTriggerPath = SonyAquariumVideoConfigTriggerPath + "getEventTrigger.xml";
                    if (File.Exists(SonyAquariumVideoConfigPUBLICTriggerPath))
                    {
                        LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - GetEventTrigger sent for PUBLIC Sony Aquarium VideoConfig {eventId}!");
                        string res = File.ReadAllText(SonyAquariumVideoConfigPUBLICTriggerPath);
                        return "<xml>\r\n\t" +
                             "<result type=\"int\">1</result>\r\n\t" +
                             "<description type=\"text\">Success</description>\r\n\t" +
                             "<error_no type=\"int\">0</error_no>\r\n\t" +
                             "<error_message type=\"text\">None</error_message>\r\n\r\n\t" +
                             $"<trigger_count type=\"int\">{readcnt}</trigger_count>\r\n\t" +
                             $"{res}\r\n" +
                             "</xml>";
                    }
                    else
                    {
                        LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - GetEventTrigger FALLBACK sent for PUBLIC Sony Aquarium VideoConfig {eventId}! Expected path {SonyAquariumVideoConfigPUBLICTriggerPath}");
                        return "<xml>\r\n" +
                            "<result type=\"int\">1</result>\r\n" +
                            "<description type=\"text\">Success</description>\r\n" +
                            "<error_no type=\"int\">0</error_no>\r\n" +
                            "<error_message type=\"text\">None</error_message>\r\n" +
                            $"<trigger_count type=\"int\">{readcnt}</trigger_count>\r\n\t" +
                            "<trigger_time>\r\n\t\t" +
                            "<trigger_id type=\"int\">1</trigger_id>\r\n" +
                            "\r\n\t\t<!-- Event Start Trigger --> \r\n\t\t" +
                            "<start_year type=\"int\">2024</start_year>\r\n\t\t" +
                            "<start_month type=\"int\">01</start_month>\r\n\t\t" +
                            "<start_day type=\"int\">15</start_day>\r\n\t\t" +
                            "<start_hour type=\"int\">00</start_hour>\r\n\t\t" +
                            "<start_minutes type=\"int\">00</start_minutes>\r\n\t\t" +
                            "<start_second type=\"int\">00</start_second>\r\n\r\n\t\t" +
                            "<end_year type=\"int\">2124</end_year>\r\n\t\t" +
                            "<end_month type=\"int\">02</end_month>\r\n\t\t" +
                            "<end_day type=\"int\">20</end_day>\r\n\t\t" +
                            "<end_hour type=\"int\">05</end_hour>\r\n\t\t" +
                            "<end_minutes type=\"int\">45</end_minutes>\r\n\t\t" +
                            "<end_second type=\"int\">0</end_second>\r\n\r\n\t\t" +
                            "<trigger_flag type=\"int\">1</trigger_flag>\r\n\t" +
                            "</trigger_time>\r\n" +
                            "</xml>";
                    }
                #endregion

                #region SCEAsia Xmas 2010 Obj Trigger PUBLIC
                case "305":
                    string SCEAsiaXmasObjTriggerPUBLICTriggerPath = SCEAsiaChristmas2010OBJTrigger + "getEventTrigger.xml";
                    if (File.Exists(SCEAsiaXmasObjTriggerPUBLICTriggerPath))
                    {
                        LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - GetEventTrigger sent for PUBLIC SCEAsia Christmas 2010 OBJ Trigger {eventId}!");
                        string res = File.ReadAllText(SCEAsiaXmasObjTriggerPUBLICTriggerPath);
                        return "<xml>\r\n\t" +
                             "<result type=\"int\">1</result>\r\n\t" +
                             "<description type=\"text\">Success</description>\r\n\t" +
                             "<error_no type=\"int\">0</error_no>\r\n\t" +
                             "<error_message type=\"text\">None</error_message>\r\n\r\n\t" +
                            $"<trigger_count type=\"int\">{readcnt}</trigger_count>\r\n\t" +
                             $"{res}\r\n" +
                             "</xml>";
                    }
                    else
                    {
                        LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - GetEventTrigger FALLBACK sent for SCEAsia Christmas 2010 OBJ Trigger {eventId}! Expected path {SCEAsiaXmasObjTriggerPUBLICTriggerPath}");
                        return "<xml>\r\n" +
                            "<result type=\"int\">1</result>\r\n" +
                            "<description type=\"text\">Success</description>\r\n" +
                            "<error_no type=\"int\">0</error_no>\r\n" +
                            "<error_message type=\"text\">None</error_message>\r\n" +
                            $"<trigger_count type=\"int\">{readcnt}</trigger_count>\r\n\t" +
                            "<trigger_time>\r\n\t\t" +
                            "<trigger_id type=\"int\">1</trigger_id>\r\n" +
                            "\r\n\t\t<!-- Event Start Trigger --> \r\n\t\t" +
                            "<start_year type=\"int\">2024</start_year>\r\n\t\t" +
                            "<start_month type=\"int\">01</start_month>\r\n\t\t" +
                            "<start_day type=\"int\">15</start_day>\r\n\t\t" +
                            "<start_hour type=\"int\">00</start_hour>\r\n\t\t" +
                            "<start_minutes type=\"int\">00</start_minutes>\r\n\t\t" +
                            "<start_second type=\"int\">00</start_second>\r\n\r\n\t\t" +
                            "<end_year type=\"int\">2124</end_year>\r\n\t\t" +
                            "<end_month type=\"int\">02</end_month>\r\n\t\t" +
                            "<end_day type=\"int\">20</end_day>\r\n\t\t" +
                            "<end_hour type=\"int\">05</end_hour>\r\n\t\t" +
                            "<end_minutes type=\"int\">45</end_minutes>\r\n\t\t" +
                            "<end_second type=\"int\">0</end_second>\r\n\r\n\t\t" +
                            "<trigger_flag type=\"int\">1</trigger_flag>\r\n\t" +
                            "</trigger_time>\r\n" +
                            "</xml>";
                    }
                #endregion

                #region SCEAsia Xmas 2010 Snow Fall PUBLIC
                case "306":
                    string SCEAsiaXmasSnowFallPUBLICTriggerPath = SCEAsiaChristmas2010SnowFall + "getEventTrigger.xml";
                    if (File.Exists(SCEAsiaXmasSnowFallPUBLICTriggerPath))
                    {
                        LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - GetEventTrigger sent for PUBLIC SCEAsia Christmas 2010 Snowfall {eventId}!");
                        string res = File.ReadAllText(SCEAsiaXmasSnowFallPUBLICTriggerPath);
                        return "<xml>\r\n\t" +
                             "<result type=\"int\">1</result>\r\n\t" +
                             "<description type=\"text\">Success</description>\r\n\t" +
                             "<error_no type=\"int\">0</error_no>\r\n\t" +
                             "<error_message type=\"text\">None</error_message>\r\n\r\n\t" +
                            $"<trigger_count type=\"int\">{readcnt}</trigger_count>\r\n\t" +
                             $"{res}\r\n" +
                             "</xml>";
                    }
                    else
                    {
                        LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - GetEventTrigger FALLBACK sent for PUBLIC SCEAsia Christmas 2010 Snowfall {eventId}! Expected path {SCEAsiaXmasSnowFallPUBLICTriggerPath}");
                        return "<xml>\r\n" +
                            "<result type=\"int\">1</result>\r\n" +
                            "<description type=\"text\">Success</description>\r\n" +
                            "<error_no type=\"int\">0</error_no>\r\n" +
                            "<error_message type=\"text\">None</error_message>\r\n" +
                            $"<trigger_count type=\"int\">{readcnt}</trigger_count>\r\n\t" +
                            "<trigger_time>\r\n\t\t" +
                            "<trigger_id type=\"int\">1</trigger_id>\r\n" +
                            "\r\n\t\t<!-- Event Start Trigger --> \r\n\t\t" +
                            "<start_year type=\"int\">2024</start_year>\r\n\t\t" +
                            "<start_month type=\"int\">01</start_month>\r\n\t\t" +
                            "<start_day type=\"int\">15</start_day>\r\n\t\t" +
                            "<start_hour type=\"int\">00</start_hour>\r\n\t\t" +
                            "<start_minutes type=\"int\">00</start_minutes>\r\n\t\t" +
                            "<start_second type=\"int\">00</start_second>\r\n\r\n\t\t" +
                            "<end_year type=\"int\">2124</end_year>\r\n\t\t" +
                            "<end_month type=\"int\">02</end_month>\r\n\t\t" +
                            "<end_day type=\"int\">20</end_day>\r\n\t\t" +
                            "<end_hour type=\"int\">05</end_hour>\r\n\t\t" +
                            "<end_minutes type=\"int\">45</end_minutes>\r\n\t\t" +
                            "<end_second type=\"int\">0</end_second>\r\n\r\n\t\t" +
                            "<trigger_flag type=\"int\">1</trigger_flag>\r\n\t" +
                            "</trigger_time>\r\n" +
                            "</xml>";
                    }
                #endregion

                #region Japan Christmas 2010 PUBLIC
                case "309":
                    string JapanChristmas2010PUBLICTriggerPath = Christmas2010 + "getEventTrigger.xml";
                    if (File.Exists(JapanChristmas2010PUBLICTriggerPath))
                    {
                        LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - GetEventTrigger sent for PUBLIC Christmas 2010 {eventId}!");
                        string res = File.ReadAllText(JapanChristmas2010PUBLICTriggerPath);
                        return "<xml>\r\n\t" +
                             "<result type=\"int\">1</result>\r\n\t" +
                             "<description type=\"text\">Success</description>\r\n\t" +
                             "<error_no type=\"int\">0</error_no>\r\n\t" +
                             "<error_message type=\"text\">None</error_message>\r\n\r\n\t" +
                            $"<trigger_count type=\"int\">{readcnt}</trigger_count>\r\n\t" +
                             $"{res}\r\n" +
                             "</xml>";
                    }
                    else
                    {
                        LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - GetEventTrigger FALLBACK sent for PUBLIC Christmas 2010 {eventId}! Expected path {JapanChristmas2010PUBLICTriggerPath}");
                        return "<xml>\r\n" +
                            "<result type=\"int\">1</result>\r\n" +
                            "<description type=\"text\">Success</description>\r\n" +
                            "<error_no type=\"int\">0</error_no>\r\n" +
                            "<error_message type=\"text\">None</error_message>\r\n" +
                            $"<trigger_count type=\"int\">{readcnt}</trigger_count>\r\n\t" +
                            "<trigger_time>\r\n\t\t" +
                            "<trigger_id type=\"int\">1</trigger_id>\r\n" +
                            "\r\n\t\t<!-- Event Start Trigger --> \r\n\t\t" +
                            "<start_year type=\"int\">2024</start_year>\r\n\t\t" +
                            "<start_month type=\"int\">01</start_month>\r\n\t\t" +
                            "<start_day type=\"int\">15</start_day>\r\n\t\t" +
                            "<start_hour type=\"int\">00</start_hour>\r\n\t\t" +
                            "<start_minutes type=\"int\">00</start_minutes>\r\n\t\t" +
                            "<start_second type=\"int\">00</start_second>\r\n\r\n\t\t" +
                            "<end_year type=\"int\">2124</end_year>\r\n\t\t" +
                            "<end_month type=\"int\">02</end_month>\r\n\t\t" +
                            "<end_day type=\"int\">20</end_day>\r\n\t\t" +
                            "<end_hour type=\"int\">05</end_hour>\r\n\t\t" +
                            "<end_minutes type=\"int\">45</end_minutes>\r\n\t\t" +
                            "<end_second type=\"int\">0</end_second>\r\n\r\n\t\t" +
                            "<trigger_flag type=\"int\">1</trigger_flag>\r\n\t" +
                            "</trigger_time>\r\n" +
                            "</xml>";
                    }
                #endregion



                default:
                    {
                        LoggerAccessor.LogError($"[PREMIUMAGENCY] - GetEventTrigger unhandled for eventId {eventId} | POSTDATA: \n{Encoding.UTF8.GetString(PostData)}");
                        return null;
                    }
            }

        }

        #region getEventTriggerEx POST
        public static string getEventTriggerExRequestPOST(byte[] PostData, string ContentType, string workpath, string eventId)
        {
            #region MultiPartFormDataParser
            string tday = string.Empty;

            string boundary = HTTPProcessor.ExtractBoundary(ContentType);
            if (!string.IsNullOrEmpty(boundary) && PostData != null)
            {
                using (MemoryStream ms = new MemoryStream(PostData))
                {
                    tday = MultipartFormDataParser.Parse(ms, boundary).GetParameterValue("tday");

                    ms.Flush();
                }
            }
            #endregion

            switch (eventId)
            {
                #region Spring2013
                case "342":
                    string Spring2013 = $"{workpath}/eventController/Spring/2013/getEventTriggerEx.xml";
                    if (File.Exists(Spring2013))
                    {
                        LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - GetEventTriggerEx FOUND for POST evid PUBLIC Spring2013 {eventId}!");
                        string res = File.ReadAllText(Spring2013);
                        return "<xml>\r\n\t" +
                             "<result type=\"int\">1</result>\r\n\t" +
                             "<description type=\"text\">Success</description>\r\n\t" +
                             "<error_no type=\"int\">0</error_no>\r\n\t" +
                             "<error_message type=\"text\">None</error_message>\r\n\r\n\t" +
                             $"{res}\r\n" +
                             "</xml>";
                    }
                    else
                    {
                        LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - GetEventTriggerEx FALLBACK for POST evid PUBLIC Spring2013 {eventId}! Expected path {Spring2013}");

                        return "<xml>\r\n" +
                            "<result type=\"int\">1</result>\r\n" +
                            "<description type=\"text\">Success</description>\r\n" +
                            "<error_no type=\"int\">0</error_no>\r\n" +
                            "<error_message type=\"text\">None</error_message>\r\n" +
                            "<trigger_count type=\"int\">200</trigger_count>\r\n" +
                            "<is_active type=\"bool\">true</is_active>\r\n" +
                            "<trigger_id type=\"int\">1</trigger_id>\r\n\r\n" +
                            "<start_year type=\"int\">2023</start_year>\r\n" +
                            "<start_month type=\"int\">09</start_month>\r\n" +
                            "<start_day type=\"int\">12</start_day>\r\n" +
                            "<start_hour type=\"int\">07</start_hour>\r\n" +
                            "<start_minutes type=\"int\">30</start_minutes>\r\n" +
                            "<start_second type=\"int\">0</start_second>\r\n\r\n" +
                            "<end_year type=\"int\">2024</end_year>\r\n" +
                            "<end_month type=\"int\">09</end_month>\r\n" +
                            "<end_day type=\"int\">17</end_day>\r\n" +
                            "<end_hour type=\"int\">00</end_hour>\r\n" +
                            "<end_minutes type=\"int\">30</end_minutes>\r\n" +
                            "<end_second type=\"int\">0</end_second>\r\n\r\n" +
                            "<trigger_flag type=\"int\">1</trigger_flag>\r\n\r\n" +
                            "</xml>";
                    }
                #endregion

                default:
                    {
                        LoggerAccessor.LogError($"[PREMIUMAGENCY] - GetEventTriggerEx unhandled for eventId {eventId} | POSTDATA: \n{Encoding.UTF8.GetString(PostData)}");
                        return null;
                    }
            }
        }
        #endregion

        #region confirmEventTrigger POST
        public static string confirmEventTriggerRequestPOST(byte[] PostData, string ContentType, string workpath, string eventId)
        {
            #region ConfirmTriggerPaths

            string basaraHomeSquareTriggerPath = $"{workpath}/eventController/collabo_iln/Triggers/";
            string Christmas2010TriggerPath = $"{workpath}/eventController/Christmas/2010/Triggers/";
            string GeorgiaTriggerPath = $"{workpath}/eventController/Georgia/Triggers/";
            string homesquareUFO09TriggerPath = $"{workpath}/eventController/UFO09/Triggers/";
            string homecafeDJMusicTriggerPath = $"{workpath}/eventController/hc/DJMusic/Triggers/";
            string homecafeGalleryTriggerPath = $"{workpath}/eventController/hc/hc_gallery/Triggers/";
            string Halloween2010TriggerPath = $"{workpath}/eventController/HalloweenV2/2010/Triggers/";
            string Halloween2010EffectsTriggerPath = $"{workpath}/eventController/HalloweenEffects/2010/Triggers/";
            string homecafeRollyCafe1FTriggerPath = $"{workpath}/eventController/hc/RollyCafe1F/Triggers/";
            string iDOLMASTERSLiveEventTriggerPath = $"{workpath}/eventController/iDOLMASTERs/LiveEvent/Triggers/";
            string iDOLMASTERSEventShopTriggerPath = $"{workpath}/eventController/iDOLMASTERs/EventShop/Triggers/";
            string iDOLMASTERSSPMoveToSpecialVenueTriggerPath = $"{workpath}/eventController/iDOLMASTERs/SPMoveToSpecialVenue/Triggers/";
            string j_liargame2TriggerPath = $"{workpath}/eventController/j_liargame2/Triggers/";
            string MacrossEventShopTriggerPath = $"{workpath}/eventController/Macross/EventShop/Triggers/";
            string MacrossSSFTriggerPath = $"{workpath}/eventController/Macross/SSF/Triggers/";
            string MacrossVF25TriggerPath = $"{workpath}/eventController/Macross/VF25/Triggers/";
            string MikuliveJackTriggerPath = $"{workpath}/eventController/MikuLiveJack/Triggers/";
            string MikuLiveJukeboxTriggerPath = $"{workpath}/eventController/MikuLiveJukebox/Triggers/";
            string MikuliveEventTriggerPath = $"{workpath}/eventController/MikuLiveEvent/Triggers/";
            string RainbowEventTriggerPath = $"{workpath}/eventController/Rainbow/Triggers/";
            string PrinnyJackTriggerPath = $"{workpath}/eventController/PrinnyJack/Triggers/";
            string RollyJukeboxTriggerPath = $"{workpath}/eventController/RollyJukebox/Triggers/";
            string SCEAsiaChristmas2010OBJTrigger = $"{workpath}/eventController/Christmas/2010/OBJTrig/Triggers/";
            string SCEAsiaChristmas2010SnowFall = $"{workpath}/eventController/Christmas/2010/Snowfall/Triggers/";
            string SceasiaDistributionTriggerPath = $"{workpath}/eventController/Distribution/SCEAsia/lounge/Triggers/";
            string SonyAquariumRelocatorTriggerPath = $"{workpath}/eventController/SonyAquarium/Relocator/Triggers/";
            string SonyAquariumVideoConfigTriggerPath = $"{workpath}/eventController/SonyAquarium/VideoConfig/Triggers/";

            #endregion

            LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - ConfirmEventTrigger {eventId}!");
            switch (eventId)
            {

                #region 2009 UFO
                case "53":
                    string homesquareUFO09ProdTriggerPath = homesquareUFO09TriggerPath + "confirmEventTrigger.xml";
                    if (File.Exists(homesquareUFO09ProdTriggerPath))
                    {
                        LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - ConfirmEventTrigger sent for Public UFO 2009 {eventId}!");
                        string res = File.ReadAllText(homesquareUFO09ProdTriggerPath);
                        return "<xml>\r\n\t" +
                             "<result type=\"int\">1</result>\r\n\t" +
                             "<description type=\"text\">Success</description>\r\n\t" +
                             "<error_no type=\"int\">0</error_no>\r\n\t" +
                             "<error_message type=\"text\">None</error_message>\r\n\r\n\t" +
                             $"{res}\r\n" +
                             "</xml>";
                    }
                    else
                    {
                        LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - ConfirmEventTrigger FALLBACK sent for Public UFO 2009 {eventId}! Expected path {homesquareUFO09ProdTriggerPath}");
                        return "<xml>\r\n" +
                            "<result type=\"int\">1</result>\r\n" +
                            "<description type=\"text\">EVENT_TRIGGER</description>\r\n" +
                            "<error_no type=\"int\">0</error_no>\r\n" +
                            "<error_message type=\"text\">None</error_message>\r\n" +
                            $"<trigger_count type=\"int\">1</trigger_count>\r\n\t" +
                            "<trigger_time>\r\n\t\t" +
                            "<trigger_id type=\"int\">1</trigger_id>\r\n" +
                            "\r\n\t\t<!-- Event Start Trigger --> \r\n\t\t" +
                            "<start_year type=\"int\">2024</start_year>\r\n\t\t" +
                            "<start_month type=\"int\">01</start_month>\r\n\t\t" +
                            "<start_day type=\"int\">15</start_day>\r\n\t\t" +
                            "<start_hour type=\"int\">00</start_hour>\r\n\t\t" +
                            "<start_minutes type=\"int\">00</start_minutes>\r\n\t\t" +
                            "<start_second type=\"int\">00</start_second>\r\n\r\n\t\t" +
                            "<end_year type=\"int\">2124</end_year>\r\n\t\t" +
                            "<end_month type=\"int\">02</end_month>\r\n\t\t" +
                            "<end_day type=\"int\">20</end_day>\r\n\t\t" +
                            "<end_hour type=\"int\">05</end_hour>\r\n\t\t" +
                            "<end_minutes type=\"int\">45</end_minutes>\r\n\t\t" +
                            "<end_second type=\"int\">0</end_second>\r\n\r\n\t\t" +
                            "<trigger_flag type=\"int\">1</trigger_flag>\r\n\t" +
                            "</trigger_time>\r\n" +
                            "</xml>";
                    }
                #endregion

                #region MikuliveJack QA
                case "55":
                    string MikuliveJackQATriggerPath = MikuliveJackTriggerPath + "qaconfirmEventTrigger.xml";
                    if (File.Exists(MikuliveJackQATriggerPath))
                    {
                        LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - ConfirmEventTrigger sent for QA MikuLiveJack {eventId}!");
                        string res = File.ReadAllText(MikuliveJackQATriggerPath);
                        return "<xml>\r\n\t" +
                             "<result type=\"int\">1</result>\r\n\t" +
                             "<description type=\"text\">Success</description>\r\n\t" +
                             "<error_no type=\"int\">0</error_no>\r\n\t" +
                             "<error_message type=\"text\">None</error_message>\r\n\r\n\t" +
                             $"{res}\r\n" +
                             "</xml>";
                    }
                    else
                    {
                        LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - ConfirmEventTrigger FALLBACK sent for QA MikuLiveJack {eventId}! Expected path {MikuliveJackQATriggerPath}");
                        return "<xml>\r\n" +
                            "<result type=\"int\">1</result>\r\n" +
                            "<description type=\"text\">Success</description>\r\n" +
                            "<error_no type=\"int\">0</error_no>\r\n" +
                            "<error_message type=\"text\">None</error_message>\r\n" +
                            "<is_active type=\"bool\">true</is_active>\r\n" +
                            "<trigger_id type=\"int\">1</trigger_id>\r\n\r\n" +
                            "<start_year type=\"int\">2023</start_year>\r\n" +
                            "<start_month type=\"int\">09</start_month>\r\n" +
                            "<start_day type=\"int\">12</start_day>\r\n" +
                            "<start_hour type=\"int\">08</start_hour>\r\n" +
                            "<start_minutes type=\"int\">00</start_minutes>\r\n" +
                            "<start_second type=\"int\">0</start_second>\r\n\r\n" +
                            "<end_year type=\"int\">2100</end_year>\r\n" +
                            "<end_month type=\"int\">09</end_month>\r\n" +
                            "<end_day type=\"int\">17</end_day>\r\n" +
                            "<end_hour type=\"int\">00</end_hour>\r\n" +
                            "<end_minutes type=\"int\">30</end_minutes>\r\n" +
                            "<end_second type=\"int\">0</end_second>\r\n\r\n" +
                            "<trigger_flag type=\"int\">999</trigger_flag>\r\n\r\n" +
                            "</xml>";
                    }
                #endregion

                #region MikuLiveEvent QA
                case "63":
                    string MikuliveEventQATriggerPath = MikuliveEventTriggerPath + "qaconfirmEventTrigger.xml";
                    if (File.Exists(MikuliveEventQATriggerPath))
                    {
                        LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - ConfirmEventTrigger sent for QA MikuLiveEvent {eventId}!");
                        string res = File.ReadAllText(MikuliveEventQATriggerPath);
                        return "<xml>\r\n\t" +
                             "<result type=\"int\">1</result>\r\n\t" +
                             "<description type=\"text\">Success</description>\r\n\t" +
                             "<error_no type=\"int\">0</error_no>\r\n\t" +
                             "<error_message type=\"text\">None</error_message>\r\n\r\n\t" +
                             $"{res}\r\n" +
                             "</xml>";
                    }
                    else
                    {
                        LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - ConfirmEventTrigger FALLBACK sent for QA MikuLiveEvent {eventId}! Expected path {MikuliveEventQATriggerPath}");
                        return "<xml>\r\n" +
                            "<result type=\"int\">1</result>\r\n" +
                            "<description type=\"text\">Success</description>\r\n" +
                            "<error_no type=\"int\">0</error_no>\r\n" +
                            "<error_message type=\"text\">None</error_message>\r\n" +
                            "<is_active type=\"bool\">true</is_active>\r\n" +
                            "<trigger_id type=\"int\">1</trigger_id>\r\n\r\n" +
                            "<start_year type=\"int\">2023</start_year>\r\n" +
                            "<start_month type=\"int\">09</start_month>\r\n" +
                            "<start_day type=\"int\">12</start_day>\r\n" +
                            "<start_hour type=\"int\">08</start_hour>\r\n" +
                            "<start_minutes type=\"int\">00</start_minutes>\r\n" +
                            "<start_second type=\"int\">0</start_second>\r\n\r\n" +
                            "<end_year type=\"int\">2100</end_year>\r\n" +
                            "<end_month type=\"int\">09</end_month>\r\n" +
                            "<end_day type=\"int\">17</end_day>\r\n" +
                            "<end_hour type=\"int\">00</end_hour>\r\n" +
                            "<end_minutes type=\"int\">30</end_minutes>\r\n" +
                            "<end_second type=\"int\">0</end_second>\r\n\r\n" +
                            "<trigger_flag type=\"int\">999</trigger_flag>\r\n\r\n" +
                            "</xml>";
                    }
                #endregion

                #region Rainbow QA
                case "72":
                    string RainbowEventQATriggerPath = RainbowEventTriggerPath + "qaconfirmEventTrigger.xml";
                    if (File.Exists(RainbowEventQATriggerPath))
                    {
                        LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - ConfirmEventTrigger sent for QA Rainbow {eventId}!");
                        string res = File.ReadAllText(RainbowEventQATriggerPath);
                        return "<xml>\r\n\t" +
                             "<result type=\"int\">1</result>\r\n\t" +
                             "<description type=\"text\">Success</description>\r\n\t" +
                             "<error_no type=\"int\">0</error_no>\r\n\t" +
                             "<error_message type=\"text\">None</error_message>\r\n\r\n\t" +
                             $"{res}\r\n" +
                             "</xml>";
                    }
                    else
                    {
                        LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - ConfirmEventTrigger FALLBACK sent for QA Rainbow {eventId}! Expected path {RainbowEventQATriggerPath}");
                        return "<xml>\r\n" +
                            "<result type=\"int\">1</result>\r\n" +
                            "<description type=\"text\">Success</description>\r\n" +
                            "<error_no type=\"int\">0</error_no>\r\n" +
                            "<error_message type=\"text\">None</error_message>\r\n" +
                            "<is_active type=\"bool\">true</is_active>\r\n" +
                            "<trigger_id type=\"int\">1</trigger_id>\r\n\r\n" +
                            "<start_year type=\"int\">2023</start_year>\r\n" +
                            "<start_month type=\"int\">09</start_month>\r\n" +
                            "<start_day type=\"int\">12</start_day>\r\n" +
                            "<start_hour type=\"int\">08</start_hour>\r\n" +
                            "<start_minutes type=\"int\">00</start_minutes>\r\n" +
                            "<start_second type=\"int\">0</start_second>\r\n\r\n" +
                            "<end_year type=\"int\">2100</end_year>\r\n" +
                            "<end_month type=\"int\">09</end_month>\r\n" +
                            "<end_day type=\"int\">17</end_day>\r\n" +
                            "<end_hour type=\"int\">00</end_hour>\r\n" +
                            "<end_minutes type=\"int\">30</end_minutes>\r\n" +
                            "<end_second type=\"int\">0</end_second>\r\n\r\n" +
                            "<trigger_flag type=\"int\">999</trigger_flag>\r\n\r\n" +
                            "</xml>";
                    }
                #endregion

                #region MikuLiveEvent LOCAL
                case "76":
                    string MikuliveEventLocalTriggerPath = MikuliveEventTriggerPath + "localconfirmEventTrigger.xml";
                    if (File.Exists(MikuliveEventLocalTriggerPath))
                    {
                        LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - ConfirmEventTrigger sent for LOCAL MikuLiveEvent {eventId}!");
                        string res = File.ReadAllText(MikuliveEventLocalTriggerPath);
                        return "<xml>\r\n\t" +
                             "<result type=\"int\">1</result>\r\n\t" +
                             "<description type=\"text\">Success</description>\r\n\t" +
                             "<error_no type=\"int\">0</error_no>\r\n\t" +
                             "<error_message type=\"text\">None</error_message>\r\n\r\n\t" +
                             $"{res}\r\n" +
                             "</xml>";
                    }
                    else
                    {
                        LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - ConfirmEventTrigger FALLBACK sent for LOCAL MikuLiveEvent {eventId}! Expected path {MikuliveEventLocalTriggerPath}");
                        return "<xml>\r\n" +
                            "<result type=\"int\">1</result>\r\n" +
                            "<description type=\"text\">Success</description>\r\n" +
                            "<error_no type=\"int\">0</error_no>\r\n" +
                            "<error_message type=\"text\">None</error_message>\r\n" +
                            "<is_active type=\"bool\">true</is_active>\r\n" +
                            "<trigger_id type=\"int\">1</trigger_id>\r\n\r\n" +
                            "<start_year type=\"int\">2023</start_year>\r\n" +
                            "<start_month type=\"int\">09</start_month>\r\n" +
                            "<start_day type=\"int\">12</start_day>\r\n" +
                            "<start_hour type=\"int\">08</start_hour>\r\n" +
                            "<start_minutes type=\"int\">00</start_minutes>\r\n" +
                            "<start_second type=\"int\">0</start_second>\r\n\r\n" +
                            "<end_year type=\"int\">2100</end_year>\r\n" +
                            "<end_month type=\"int\">09</end_month>\r\n" +
                            "<end_day type=\"int\">17</end_day>\r\n" +
                            "<end_hour type=\"int\">00</end_hour>\r\n" +
                            "<end_minutes type=\"int\">30</end_minutes>\r\n" +
                            "<end_second type=\"int\">0</end_second>\r\n\r\n" +
                            "<trigger_flag type=\"int\">999</trigger_flag>\r\n\r\n" +
                            "</xml>";
                    }
                #endregion

                #region RollyJukebox PUBLIC
                case "80":
                    string RollyJukeboxPublicTriggerPath = RollyJukeboxTriggerPath + "confirmEventTrigger.xml";
                    if (File.Exists(RollyJukeboxPublicTriggerPath))
                    {
                        LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - ConfirmEventTrigger sent for PUBLIC Rolly Music Survey {eventId}!");
                        string res = File.ReadAllText(RollyJukeboxPublicTriggerPath);
                        return "<xml>\r\n\t" +
                             "<result type=\"int\">1</result>\r\n\t" +
                             "<description type=\"text\">Success</description>\r\n\t" +
                             "<error_no type=\"int\">0</error_no>\r\n\t" +
                             "<error_message type=\"text\">None</error_message>\r\n\r\n\t" +
                             $"{res}\r\n" +
                             "</xml>";
                    }
                    else
                    {
                        LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - ConfirmEventTrigger FALLBACK sent for PUBLIC Rolly Music Survey {eventId}! Expected path {RollyJukeboxPublicTriggerPath}");
                        return "<xml>\r\n" +
                            "<result type=\"int\">1</result>\r\n" +
                            "<description type=\"text\">Success</description>\r\n" +
                            "<error_no type=\"int\">0</error_no>\r\n" +
                            "<error_message type=\"text\">None</error_message>\r\n" +
                            "<is_active type=\"bool\">true</is_active>\r\n" +
                            "<trigger_id type=\"int\">1</trigger_id>\r\n\r\n" +
                            "<start_year type=\"int\">2023</start_year>\r\n" +
                            "<start_month type=\"int\">09</start_month>\r\n" +
                            "<start_day type=\"int\">12</start_day>\r\n" +
                            "<start_hour type=\"int\">08</start_hour>\r\n" +
                            "<start_minutes type=\"int\">00</start_minutes>\r\n" +
                            "<start_second type=\"int\">0</start_second>\r\n\r\n" +
                            "<end_year type=\"int\">2100</end_year>\r\n" +
                            "<end_month type=\"int\">09</end_month>\r\n" +
                            "<end_day type=\"int\">17</end_day>\r\n" +
                            "<end_hour type=\"int\">00</end_hour>\r\n" +
                            "<end_minutes type=\"int\">30</end_minutes>\r\n" +
                            "<end_second type=\"int\">0</end_second>\r\n\r\n" +
                            "<trigger_flag type=\"int\">999</trigger_flag>\r\n\r\n" +
                            "</xml>";
                    }
                #endregion

                #region MikuLiveJack PUBLIC
                case "90":
                    string MikuliveJackPublicTriggerPath = MikuliveJackTriggerPath + "confirmEventTrigger.xml";
                    if (File.Exists(MikuliveJackPublicTriggerPath))
                    {
                        LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - ConfirmEventTrigger sent for PUBLIC MikuLiveJack {eventId}!");
                        string res = File.ReadAllText(MikuliveJackPublicTriggerPath);
                        return "<xml>\r\n\t" +
                             "<result type=\"int\">1</result>\r\n\t" +
                             "<description type=\"text\">Success</description>\r\n\t" +
                             "<error_no type=\"int\">0</error_no>\r\n\t" +
                             "<error_message type=\"text\">None</error_message>\r\n\r\n\t" +
                             $"{res}\r\n" +
                             "</xml>";
                    }
                    else
                    {
                        LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - ConfirmEventTrigger FALLBACK sent for PUBLIC MikuLiveJack {eventId}! Expected path {MikuliveJackPublicTriggerPath}");
                        return "<xml>\r\n" +
                            "<result type=\"int\">1</result>\r\n" +
                            "<description type=\"text\">Success</description>\r\n" +
                            "<error_no type=\"int\">0</error_no>\r\n" +
                            "<error_message type=\"text\">None</error_message>\r\n" +
                            "<is_active type=\"bool\">true</is_active>\r\n" +
                            "<trigger_id type=\"int\">1</trigger_id>\r\n\r\n" +
                            "<start_year type=\"int\">2023</start_year>\r\n" +
                            "<start_month type=\"int\">09</start_month>\r\n" +
                            "<start_day type=\"int\">12</start_day>\r\n" +
                            "<start_hour type=\"int\">08</start_hour>\r\n" +
                            "<start_minutes type=\"int\">00</start_minutes>\r\n" +
                            "<start_second type=\"int\">0</start_second>\r\n\r\n" +
                            "<end_year type=\"int\">2100</end_year>\r\n" +
                            "<end_month type=\"int\">09</end_month>\r\n" +
                            "<end_day type=\"int\">17</end_day>\r\n" +
                            "<end_hour type=\"int\">00</end_hour>\r\n" +
                            "<end_minutes type=\"int\">30</end_minutes>\r\n" +
                            "<end_second type=\"int\">0</end_second>\r\n\r\n" +
                            "<trigger_flag type=\"int\">999</trigger_flag>\r\n\r\n" +
                            "</xml>";
                    }
                #endregion

                #region MikuLiveJukebox PUBLIC
                case "91":
                    string MikuLiveJukeboxPublicTriggerPath = MikuLiveJukeboxTriggerPath + "confirmEventTrigger.xml";
                    if (File.Exists(MikuLiveJukeboxPublicTriggerPath))
                    {
                        LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - ConfirmEventTrigger sent for PUBLIC MikuLiveJukebox {eventId}!");
                        string res = File.ReadAllText(MikuLiveJukeboxPublicTriggerPath);
                        return "<xml>\r\n\t" +
                             "<result type=\"int\">1</result>\r\n\t" +
                             "<description type=\"text\">Success</description>\r\n\t" +
                             "<error_no type=\"int\">0</error_no>\r\n\t" +
                             "<error_message type=\"text\">None</error_message>\r\n\r\n\t" +
                             $"{res}\r\n" +
                             "</xml>";
                    }
                    else
                    {
                        LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - ConfirmEventTrigger FALLBACK sent for PUBLIC MikuLiveJukebox {eventId}! Expected path {MikuLiveJukeboxPublicTriggerPath}");
                        return "<xml>\r\n" +
                            "<result type=\"int\">1</result>\r\n" +
                            "<description type=\"text\">Success</description>\r\n" +
                            "<error_no type=\"int\">0</error_no>\r\n" +
                            "<error_message type=\"text\">None</error_message>\r\n" +
                            "<is_active type=\"bool\">true</is_active>\r\n" +
                            "<trigger_id type=\"int\">1</trigger_id>\r\n\r\n" +
                            "<start_year type=\"int\">2023</start_year>\r\n" +
                            "<start_month type=\"int\">09</start_month>\r\n" +
                            "<start_day type=\"int\">12</start_day>\r\n" +
                            "<start_hour type=\"int\">08</start_hour>\r\n" +
                            "<start_minutes type=\"int\">00</start_minutes>\r\n" +
                            "<start_second type=\"int\">0</start_second>\r\n\r\n" +
                            "<end_year type=\"int\">2100</end_year>\r\n" +
                            "<end_month type=\"int\">09</end_month>\r\n" +
                            "<end_day type=\"int\">17</end_day>\r\n" +
                            "<end_hour type=\"int\">00</end_hour>\r\n" +
                            "<end_minutes type=\"int\">30</end_minutes>\r\n" +
                            "<end_second type=\"int\">0</end_second>\r\n\r\n" +
                            "<trigger_flag type=\"int\">999</trigger_flag>\r\n\r\n" +
                            "</xml>";
                    }
                #endregion

                #region PrinnyJack
                case "93":
                    string PrinnyJackPublicTriggerPath = PrinnyJackTriggerPath + "confirmEventTrigger.xml";
                    if (File.Exists(PrinnyJackPublicTriggerPath))
                    {
                        LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - ConfirmEventTrigger sent for PUBLIC Prinny  {eventId}!");
                        string res = File.ReadAllText(PrinnyJackPublicTriggerPath);
                        return "<xml>\r\n\t" +
                             "<result type=\"int\">1</result>\r\n\t" +
                             "<description type=\"text\">Success</description>\r\n\t" +
                             "<error_no type=\"int\">0</error_no>\r\n\t" +
                             "<error_message type=\"text\">None</error_message>\r\n\r\n\t" +
                             $"{res}\r\n" +
                             "</xml>";
                    }
                    else
                    {
                        LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - ConfirmEventTrigger FALLBACK sent for PUBLIC Prinny  {eventId}! Expected path {PrinnyJackPublicTriggerPath}");
                        return "<xml>\r\n" +
                            "<result type=\"int\">1</result>\r\n" +
                            "<description type=\"text\">Success</description>\r\n" +
                            "<error_no type=\"int\">0</error_no>\r\n" +
                            "<error_message type=\"text\">None</error_message>\r\n" +
                            "<is_active type=\"bool\">true</is_active>\r\n" +
                            "<trigger_id type=\"int\">1</trigger_id>\r\n\r\n" +
                            "<start_year type=\"int\">2023</start_year>\r\n" +
                            "<start_month type=\"int\">09</start_month>\r\n" +
                            "<start_day type=\"int\">12</start_day>\r\n" +
                            "<start_hour type=\"int\">08</start_hour>\r\n" +
                            "<start_minutes type=\"int\">00</start_minutes>\r\n" +
                            "<start_second type=\"int\">0</start_second>\r\n\r\n" +
                            "<end_year type=\"int\">2100</end_year>\r\n" +
                            "<end_month type=\"int\">09</end_month>\r\n" +
                            "<end_day type=\"int\">17</end_day>\r\n" +
                            "<end_hour type=\"int\">00</end_hour>\r\n" +
                            "<end_minutes type=\"int\">30</end_minutes>\r\n" +
                            "<end_second type=\"int\">0</end_second>\r\n\r\n" +
                            "<trigger_flag type=\"int\">999</trigger_flag>\r\n\r\n" +
                            "</xml>";
                    }
                #endregion

                #region MikuLiveEvent Public
                case "95":
                    string MikuliveEventPublicTriggerPath = MikuliveEventTriggerPath + "confirmEventTrigger.xml";
                    if (File.Exists(MikuliveEventPublicTriggerPath))
                    {
                        LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - ConfirmEventTrigger sent for PUBLIC MikuLiveEvent {eventId}!");
                        string res = File.ReadAllText(MikuliveEventPublicTriggerPath);
                        return "<xml>\r\n\t" +
                             "<result type=\"int\">1</result>\r\n\t" +
                             "<description type=\"text\">Success</description>\r\n\t" +
                             "<error_no type=\"int\">0</error_no>\r\n\t" +
                             "<error_message type=\"text\">None</error_message>\r\n\r\n\t" +
                             $"{res}\r\n" +
                             "</xml>";
                    }
                    else
                    {
                        LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - ConfirmEventTrigger FALLBACK sent for PUBLIC MikuLiveEvent {eventId}! Expected path {MikuliveEventPublicTriggerPath}");
                        return "<xml>\r\n" +
                            "<result type=\"int\">1</result>\r\n" +
                            "<description type=\"text\">Success</description>\r\n" +
                            "<error_no type=\"int\">0</error_no>\r\n" +
                            "<error_message type=\"text\">None</error_message>\r\n" +
                            "<is_active type=\"bool\">true</is_active>\r\n" +
                            "<trigger_id type=\"int\">1</trigger_id>\r\n\r\n" +
                            "<start_year type=\"int\">2023</start_year>\r\n" +
                            "<start_month type=\"int\">09</start_month>\r\n" +
                            "<start_day type=\"int\">12</start_day>\r\n" +
                            "<start_hour type=\"int\">08</start_hour>\r\n" +
                            "<start_minutes type=\"int\">00</start_minutes>\r\n" +
                            "<start_second type=\"int\">0</start_second>\r\n\r\n" +
                            "<end_year type=\"int\">2100</end_year>\r\n" +
                            "<end_month type=\"int\">09</end_month>\r\n" +
                            "<end_day type=\"int\">17</end_day>\r\n" +
                            "<end_hour type=\"int\">00</end_hour>\r\n" +
                            "<end_minutes type=\"int\">30</end_minutes>\r\n" +
                            "<end_second type=\"int\">0</end_second>\r\n\r\n" +
                            "<trigger_flag type=\"int\">999</trigger_flag>\r\n\r\n" +
                            "</xml>";
                    }
                #endregion

                #region Rainbow PUBLIC
                case "98":
                    string RainbowEventPublicTriggerPath = RainbowEventTriggerPath + "confirmEventTrigger.xml";
                    if (File.Exists(RainbowEventPublicTriggerPath))
                    {
                        LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - ConfirmEventTrigger sent for PUBLIC Rainbow {eventId}!");
                        string res = File.ReadAllText(RainbowEventPublicTriggerPath);
                        return "<xml>\r\n\t" +
                             "<result type=\"int\">1</result>\r\n\t" +
                             "<description type=\"text\">Success</description>\r\n\t" +
                             "<error_no type=\"int\">0</error_no>\r\n\t" +
                             "<error_message type=\"text\">None</error_message>\r\n\r\n\t" +
                             $"{res}\r\n" +
                             "</xml>";
                    }
                    else
                    {
                        LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - ConfirmEventTrigger FALLBACK sent for PUBLIC Rainbow {eventId}! Expected path {RainbowEventPublicTriggerPath}");
                        return "<xml>\r\n" +
                            "<result type=\"int\">1</result>\r\n" +
                            "<description type=\"text\">Success</description>\r\n" +
                            "<error_no type=\"int\">0</error_no>\r\n" +
                            "<error_message type=\"text\">None</error_message>\r\n" +
                            "<is_active type=\"bool\">true</is_active>\r\n" +
                            "<trigger_id type=\"int\">1</trigger_id>\r\n\r\n" +
                            "<start_year type=\"int\">2023</start_year>\r\n" +
                            "<start_month type=\"int\">09</start_month>\r\n" +
                            "<start_day type=\"int\">12</start_day>\r\n" +
                            "<start_hour type=\"int\">08</start_hour>\r\n" +
                            "<start_minutes type=\"int\">00</start_minutes>\r\n" +
                            "<start_second type=\"int\">0</start_second>\r\n\r\n" +
                            "<end_year type=\"int\">2100</end_year>\r\n" +
                            "<end_month type=\"int\">09</end_month>\r\n" +
                            "<end_day type=\"int\">17</end_day>\r\n" +
                            "<end_hour type=\"int\">00</end_hour>\r\n" +
                            "<end_minutes type=\"int\">30</end_minutes>\r\n" +
                            "<end_second type=\"int\">0</end_second>\r\n\r\n" +
                            "<trigger_flag type=\"int\">999</trigger_flag>\r\n\r\n" +
                            "</xml>";
                    }
                #endregion

                #region MikuLiveJack LOCAL
                case "110":
                    string MikuliveJackLocalTriggerPath = MikuliveJackTriggerPath + "localconfirmEventTrigger.xml";
                    if (File.Exists(MikuliveJackLocalTriggerPath))
                    {
                        LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - ConfirmEventTrigger sent for LOCAL MikuLiveJack {eventId}!");
                        string res = File.ReadAllText(MikuliveJackLocalTriggerPath);
                        return "<xml>\r\n\t" +
                             "<result type=\"int\">1</result>\r\n\t" +
                             "<description type=\"text\">Success</description>\r\n\t" +
                             "<error_no type=\"int\">0</error_no>\r\n\t" +
                             "<error_message type=\"text\">None</error_message>\r\n\r\n\t" +
                             $"{res}\r\n" +
                             "</xml>";
                    }
                    else
                    {
                        LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - ConfirmEventTrigger FALLBACK sent for LOCAL MikuLiveJack {eventId}! Expected path {MikuliveJackLocalTriggerPath}");
                        return "<xml>\r\n" +
                            "<result type=\"int\">1</result>\r\n" +
                            "<description type=\"text\">Success</description>\r\n" +
                            "<error_no type=\"int\">0</error_no>\r\n" +
                            "<error_message type=\"text\">None</error_message>\r\n" +
                            "<is_active type=\"bool\">true</is_active>\r\n" +
                            "<trigger_id type=\"int\">1</trigger_id>\r\n\r\n" +
                            "<start_year type=\"int\">2023</start_year>\r\n" +
                            "<start_month type=\"int\">09</start_month>\r\n" +
                            "<start_day type=\"int\">12</start_day>\r\n" +
                            "<start_hour type=\"int\">08</start_hour>\r\n" +
                            "<start_minutes type=\"int\">00</start_minutes>\r\n" +
                            "<start_second type=\"int\">0</start_second>\r\n\r\n" +
                            "<end_year type=\"int\">2100</end_year>\r\n" +
                            "<end_month type=\"int\">09</end_month>\r\n" +
                            "<end_day type=\"int\">17</end_day>\r\n" +
                            "<end_hour type=\"int\">00</end_hour>\r\n" +
                            "<end_minutes type=\"int\">30</end_minutes>\r\n" +
                            "<end_second type=\"int\">0</end_second>\r\n\r\n" +
                            "<trigger_flag type=\"int\">999</trigger_flag>\r\n\r\n" +
                            "</xml>";
                    }
                #endregion

                #region Georgia PUBLIC
                case "111":
                    string GeorgiaPublicTriggerPath = GeorgiaTriggerPath + "confirmEventTrigger.xml";
                    if (File.Exists(GeorgiaPublicTriggerPath))
                    {
                        LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - ConfirmEventTrigger sent for PUBLIC Georgia {eventId}!");
                        string res = File.ReadAllText(GeorgiaPublicTriggerPath);
                        return "<xml>\r\n\t" +
                             "<result type=\"int\">1</result>\r\n\t" +
                             "<description type=\"text\">Success</description>\r\n\t" +
                             "<error_no type=\"int\">0</error_no>\r\n\t" +
                             "<error_message type=\"text\">None</error_message>\r\n\r\n\t" +
                             $"{res}\r\n" +
                             "</xml>";
                    }
                    else
                    {
                        LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - ConfirmEventTrigger FALLBACK sent for PUBLIC Georgia {eventId}! Expected path {GeorgiaPublicTriggerPath}");
                        return "<xml>\r\n" +
                            "<result type=\"int\">1</result>\r\n" +
                            "<description type=\"text\">Success</description>\r\n" +
                            "<error_no type=\"int\">0</error_no>\r\n" +
                            "<error_message type=\"text\">None</error_message>\r\n" +
                            "<is_active type=\"bool\">true</is_active>\r\n" +
                            "<trigger_id type=\"int\">1</trigger_id>\r\n\r\n" +
                            "<start_year type=\"int\">2023</start_year>\r\n" +
                            "<start_month type=\"int\">09</start_month>\r\n" +
                            "<start_day type=\"int\">12</start_day>\r\n" +
                            "<start_hour type=\"int\">08</start_hour>\r\n" +
                            "<start_minutes type=\"int\">00</start_minutes>\r\n" +
                            "<start_second type=\"int\">0</start_second>\r\n\r\n" +
                            "<end_year type=\"int\">2100</end_year>\r\n" +
                            "<end_month type=\"int\">09</end_month>\r\n" +
                            "<end_day type=\"int\">17</end_day>\r\n" +
                            "<end_hour type=\"int\">00</end_hour>\r\n" +
                            "<end_minutes type=\"int\">30</end_minutes>\r\n" +
                            "<end_second type=\"int\">0</end_second>\r\n\r\n" +
                            "<trigger_flag type=\"int\">999</trigger_flag>\r\n\r\n" +
                            "</xml>";
                    }
                #endregion

                #region Rainbow LOCAL
                case "124":
                    string RainbowEventLocalTriggerPath = RainbowEventTriggerPath + "localconfirmEventTrigger.xml";
                    if (File.Exists(RainbowEventLocalTriggerPath))
                    {
                        LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - ConfirmEventTrigger sent for LOCAL Rainbow {eventId}!");
                        string res = File.ReadAllText(RainbowEventLocalTriggerPath);
                        return "<xml>\r\n\t" +
                             "<result type=\"int\">1</result>\r\n\t" +
                             "<description type=\"text\">Success</description>\r\n\t" +
                             "<error_no type=\"int\">0</error_no>\r\n\t" +
                             "<error_message type=\"text\">None</error_message>\r\n\r\n\t" +
                             $"{res}\r\n" +
                             "</xml>";
                    }
                    else
                    {
                        LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - ConfirmEventTrigger FALLBACK sent for LOCAL Rainbow {eventId}! Expected path {RainbowEventLocalTriggerPath}");
                        return "<xml>\r\n" +
                            "<result type=\"int\">1</result>\r\n" +
                            "<description type=\"text\">Success</description>\r\n" +
                            "<error_no type=\"int\">0</error_no>\r\n" +
                            "<error_message type=\"text\">None</error_message>\r\n" +
                            "<is_active type=\"bool\">true</is_active>\r\n" +
                            "<trigger_id type=\"int\">1</trigger_id>\r\n\r\n" +
                            "<start_year type=\"int\">2023</start_year>\r\n" +
                            "<start_month type=\"int\">09</start_month>\r\n" +
                            "<start_day type=\"int\">12</start_day>\r\n" +
                            "<start_hour type=\"int\">08</start_hour>\r\n" +
                            "<start_minutes type=\"int\">00</start_minutes>\r\n" +
                            "<start_second type=\"int\">0</start_second>\r\n\r\n" +
                            "<end_year type=\"int\">2100</end_year>\r\n" +
                            "<end_month type=\"int\">09</end_month>\r\n" +
                            "<end_day type=\"int\">17</end_day>\r\n" +
                            "<end_hour type=\"int\">00</end_hour>\r\n" +
                            "<end_minutes type=\"int\">30</end_minutes>\r\n" +
                            "<end_second type=\"int\">0</end_second>\r\n\r\n" +
                            "<trigger_flag type=\"int\">999</trigger_flag>\r\n\r\n" +
                            "</xml>";
                    }
                #endregion

                #region SCEAsia Distribution QA/PUBLIC
                case "138":
                    string SceasiaDistributionQATriggerPath = SceasiaDistributionTriggerPath + "qaconfirmEventTrigger.xml";
                    if (File.Exists(SceasiaDistributionQATriggerPath))
                    {
                        LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - ConfirmEventTrigger sent for QA Free distribution SCEAsia area lounge MiddleFloor {eventId}!");
                        string res = File.ReadAllText(SceasiaDistributionQATriggerPath);
                        return "<xml>\r\n\t" +
                             "<result type=\"int\">1</result>\r\n\t" +
                             "<description type=\"text\">Success</description>\r\n\t" +
                             "<error_no type=\"int\">0</error_no>\r\n\t" +
                             "<error_message type=\"text\">None</error_message>\r\n\r\n\t" +
                             $"{res}\r\n" +
                             "</xml>";
                    }
                    else
                    {
                        LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - ConfirmEventTrigger FALLBACK sent for QA Free distribution SCEAsia area lounge MiddleFloor {eventId}! Expected path {SceasiaDistributionQATriggerPath}");
                        return "<xml>\r\n" +
                            "<result type=\"int\">1</result>\r\n" +
                            "<description type=\"text\">Success</description>\r\n" +
                            "<error_no type=\"int\">0</error_no>\r\n" +
                            "<error_message type=\"text\">None</error_message>\r\n" +
                            "<is_active type=\"bool\">true</is_active>\r\n" +
                            "<trigger_id type=\"int\">1</trigger_id>\r\n\r\n" +
                            "<start_year type=\"int\">2023</start_year>\r\n" +
                            "<start_month type=\"int\">09</start_month>\r\n" +
                            "<start_day type=\"int\">12</start_day>\r\n" +
                            "<start_hour type=\"int\">08</start_hour>\r\n" +
                            "<start_minutes type=\"int\">00</start_minutes>\r\n" +
                            "<start_second type=\"int\">0</start_second>\r\n\r\n" +
                            "<end_year type=\"int\">2100</end_year>\r\n" +
                            "<end_month type=\"int\">09</end_month>\r\n" +
                            "<end_day type=\"int\">17</end_day>\r\n" +
                            "<end_hour type=\"int\">00</end_hour>\r\n" +
                            "<end_minutes type=\"int\">30</end_minutes>\r\n" +
                            "<end_second type=\"int\">0</end_second>\r\n\r\n" +
                            "<trigger_flag type=\"int\">999</trigger_flag>\r\n\r\n" +
                            "</xml>";
                    }
                case "140":
                    string SceasiaDistributionPublicTriggerPath = SceasiaDistributionTriggerPath + "confirmEventTrigger.xml";
                    if (File.Exists(SceasiaDistributionPublicTriggerPath))
                    {
                        LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - ConfirmEventTrigger sent for PUBLIC Free distribution SCEAsia area lounge MiddleFloor {eventId}!");
                        string res = File.ReadAllText(SceasiaDistributionPublicTriggerPath);
                        return "<xml>\r\n\t" +
                             "<result type=\"int\">1</result>\r\n\t" +
                             "<description type=\"text\">Success</description>\r\n\t" +
                             "<error_no type=\"int\">0</error_no>\r\n\t" +
                             "<error_message type=\"text\">None</error_message>\r\n\r\n\t" +
                             $"{res}\r\n" +
                             "</xml>";
                    }
                    else
                    {
                        LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - ConfirmEventTrigger FALLBACK sent for PUBLIC Free distribution SCEAsia area lounge MiddleFloor {eventId}! Expected path {SceasiaDistributionPublicTriggerPath}");
                        return "<xml>\r\n" +
                            "<result type=\"int\">1</result>\r\n" +
                            "<description type=\"text\">Success</description>\r\n" +
                            "<error_no type=\"int\">0</error_no>\r\n" +
                            "<error_message type=\"text\">None</error_message>\r\n" +
                            "<is_active type=\"bool\">true</is_active>\r\n" +
                            "<trigger_id type=\"int\">1</trigger_id>\r\n\r\n" +
                            "<start_year type=\"int\">2023</start_year>\r\n" +
                            "<start_month type=\"int\">09</start_month>\r\n" +
                            "<start_day type=\"int\">12</start_day>\r\n" +
                            "<start_hour type=\"int\">08</start_hour>\r\n" +
                            "<start_minutes type=\"int\">00</start_minutes>\r\n" +
                            "<start_second type=\"int\">0</start_second>\r\n\r\n" +
                            "<end_year type=\"int\">2100</end_year>\r\n" +
                            "<end_month type=\"int\">09</end_month>\r\n" +
                            "<end_day type=\"int\">17</end_day>\r\n" +
                            "<end_hour type=\"int\">00</end_hour>\r\n" +
                            "<end_minutes type=\"int\">30</end_minutes>\r\n" +
                            "<end_second type=\"int\">0</end_second>\r\n\r\n" +
                            "<trigger_flag type=\"int\">999</trigger_flag>\r\n\r\n" +
                            "</xml>";
                    }
                #endregion

                #region iDOLM@ASTER LiveEvent PUBLIC
                case "148":
                    string iDOLMASTERSLiveEventPublicTriggerPath = iDOLMASTERSLiveEventTriggerPath + "confirmEventTrigger.xml";
                    if (File.Exists(iDOLMASTERSLiveEventPublicTriggerPath))
                    {
                        LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - ConfirmEventTrigger sent for PUBLIC iDOLMASTERs LiveEvent {eventId}!");
                        string res = File.ReadAllText(iDOLMASTERSLiveEventPublicTriggerPath);
                        return "<xml>\r\n\t" +
                             "<result type=\"int\">1</result>\r\n\t" +
                             "<description type=\"text\">Success</description>\r\n\t" +
                             "<error_no type=\"int\">0</error_no>\r\n\t" +
                             "<error_message type=\"text\">None</error_message>\r\n\r\n\t" +
                             $"{res}\r\n" +
                             "</xml>";
                    }
                    else
                    {
                        LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - ConfirmEventTrigger FALLBACK sent for PUBLIC iDOLMASTERs LiveEvent {eventId}! Expected path {iDOLMASTERSLiveEventPublicTriggerPath}");
                        return "<xml>\r\n" +
                            "<result type=\"int\">1</result>\r\n" +
                            "<description type=\"text\">Success</description>\r\n" +
                            "<error_no type=\"int\">0</error_no>\r\n" +
                            "<error_message type=\"text\">None</error_message>\r\n" +
                            "<is_active type=\"bool\">true</is_active>\r\n" +
                            "<trigger_id type=\"int\">1</trigger_id>\r\n\r\n" +
                            "<start_year type=\"int\">2023</start_year>\r\n" +
                            "<start_month type=\"int\">09</start_month>\r\n" +
                            "<start_day type=\"int\">12</start_day>\r\n" +
                            "<start_hour type=\"int\">08</start_hour>\r\n" +
                            "<start_minutes type=\"int\">00</start_minutes>\r\n" +
                            "<start_second type=\"int\">0</start_second>\r\n\r\n" +
                            "<end_year type=\"int\">2100</end_year>\r\n" +
                            "<end_month type=\"int\">09</end_month>\r\n" +
                            "<end_day type=\"int\">17</end_day>\r\n" +
                            "<end_hour type=\"int\">00</end_hour>\r\n" +
                            "<end_minutes type=\"int\">30</end_minutes>\r\n" +
                            "<end_second type=\"int\">0</end_second>\r\n\r\n" +
                            "<trigger_flag type=\"int\">999</trigger_flag>\r\n\r\n" +
                            "</xml>";
                    }
                #endregion

                #region iDOLM@ASTERs EventShop QA
                case "149":
                    string iDOLMASTERSEventShopQATriggerPath = iDOLMASTERSEventShopTriggerPath + "confirmEventTrigger.xml";
                    if (File.Exists(iDOLMASTERSEventShopQATriggerPath))
                    {
                        LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - ConfirmEventTrigger sent for QA iDOLM@STERs Event Shop {eventId}!");
                        string res = File.ReadAllText(iDOLMASTERSEventShopQATriggerPath);
                        return "<xml>\r\n\t" +
                             "<result type=\"int\">1</result>\r\n\t" +
                             "<description type=\"text\">Success</description>\r\n\t" +
                             "<error_no type=\"int\">0</error_no>\r\n\t" +
                             "<error_message type=\"text\">None</error_message>\r\n\r\n\t" +
                             $"{res}\r\n" +
                             "</xml>";
                    }
                    else
                    {
                        LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - ConfirmEventTrigger FALLBACK sent for QA iDOLM@STERs Event Shop {eventId}! Expected path {iDOLMASTERSEventShopQATriggerPath}");
                        return "<xml>\r\n" +
                            "<result type=\"int\">1</result>\r\n" +
                            "<description type=\"text\">Success</description>\r\n" +
                            "<error_no type=\"int\">0</error_no>\r\n" +
                            "<error_message type=\"text\">None</error_message>\r\n" +
                            "<is_active type=\"bool\">true</is_active>\r\n" +
                            "<trigger_id type=\"int\">1</trigger_id>\r\n\r\n" +
                            "<start_year type=\"int\">2023</start_year>\r\n" +
                            "<start_month type=\"int\">09</start_month>\r\n" +
                            "<start_day type=\"int\">12</start_day>\r\n" +
                            "<start_hour type=\"int\">08</start_hour>\r\n" +
                            "<start_minutes type=\"int\">00</start_minutes>\r\n" +
                            "<start_second type=\"int\">0</start_second>\r\n\r\n" +
                            "<end_year type=\"int\">2100</end_year>\r\n" +
                            "<end_month type=\"int\">09</end_month>\r\n" +
                            "<end_day type=\"int\">17</end_day>\r\n" +
                            "<end_hour type=\"int\">00</end_hour>\r\n" +
                            "<end_minutes type=\"int\">30</end_minutes>\r\n" +
                            "<end_second type=\"int\">0</end_second>\r\n\r\n" +
                            "<trigger_flag type=\"int\">999</trigger_flag>\r\n\r\n" +
                            "</xml>";
                    }
                #endregion

                #region iDOLM@STERs SPMoveToSpecialVenue PUBLIC
                case "150":
                    string iDOLMASTERSSPMoveToSpecialVenuePUBLICTriggerPath = iDOLMASTERSSPMoveToSpecialVenueTriggerPath + "localconfirmEventTrigger.xml";
                    if (File.Exists(iDOLMASTERSSPMoveToSpecialVenuePUBLICTriggerPath))
                    {
                        LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - ConfirmEventTrigger sent for PUBLIC THE IDOLM@STER SP Move to special live venue {eventId}!");
                        string res = File.ReadAllText(iDOLMASTERSSPMoveToSpecialVenuePUBLICTriggerPath);
                        return "<xml>\r\n\t" +
                             "<result type=\"int\">1</result>\r\n\t" +
                             "<description type=\"text\">Success</description>\r\n\t" +
                             "<error_no type=\"int\">0</error_no>\r\n\t" +
                             "<error_message type=\"text\">None</error_message>\r\n\r\n\t" +
                             $"{res}\r\n" +
                             "</xml>";
                    }
                    else
                    {
                        LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - ConfirmEventTrigger FALLBACK sent for PUBLIC THE IDOLM@STER SP Move to special live venue {eventId}! Expected path {iDOLMASTERSSPMoveToSpecialVenuePUBLICTriggerPath}");
                        return "<xml>\r\n" +
                            "<result type=\"int\">1</result>\r\n" +
                            "<description type=\"text\">Success</description>\r\n" +
                            "<error_no type=\"int\">0</error_no>\r\n" +
                            "<error_message type=\"text\">None</error_message>\r\n" +
                            "<is_active type=\"bool\">true</is_active>\r\n" +
                            "<trigger_id type=\"int\">1</trigger_id>\r\n\r\n" +
                            "<start_year type=\"int\">2023</start_year>\r\n" +
                            "<start_month type=\"int\">09</start_month>\r\n" +
                            "<start_day type=\"int\">12</start_day>\r\n" +
                            "<start_hour type=\"int\">08</start_hour>\r\n" +
                            "<start_minutes type=\"int\">00</start_minutes>\r\n" +
                            "<start_second type=\"int\">0</start_second>\r\n\r\n" +
                            "<end_year type=\"int\">2100</end_year>\r\n" +
                            "<end_month type=\"int\">09</end_month>\r\n" +
                            "<end_day type=\"int\">17</end_day>\r\n" +
                            "<end_hour type=\"int\">00</end_hour>\r\n" +
                            "<end_minutes type=\"int\">30</end_minutes>\r\n" +
                            "<end_second type=\"int\">0</end_second>\r\n\r\n" +
                            "<trigger_flag type=\"int\">999</trigger_flag>\r\n\r\n" +
                            "</xml>";
                    }
                #endregion

                #region iDOLM@STERs EventShop LOCAL
                case "159":
                    string iDOLMASTERSEventShopLOCALTriggerPath = iDOLMASTERSEventShopTriggerPath + "localconfirmEventTrigger.xml";
                    if (File.Exists(iDOLMASTERSEventShopLOCALTriggerPath))
                    {
                        LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - ConfirmEventTrigger sent for LOCAL iDOLM@STERs Event Shop {eventId}!");
                        string res = File.ReadAllText(iDOLMASTERSEventShopLOCALTriggerPath);
                        return "<xml>\r\n\t" +
                             "<result type=\"int\">1</result>\r\n\t" +
                             "<description type=\"text\">Success</description>\r\n\t" +
                             "<error_no type=\"int\">0</error_no>\r\n\t" +
                             "<error_message type=\"text\">None</error_message>\r\n\r\n\t" +
                             $"{res}\r\n" +
                             "</xml>";
                    }
                    else
                    {
                        LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - ConfirmEventTrigger FALLBACK sent for LOCAL iDOLM@STERs Event Shop {eventId}! Expected path {iDOLMASTERSEventShopLOCALTriggerPath}");
                        return "<xml>\r\n" +
                            "<result type=\"int\">1</result>\r\n" +
                            "<description type=\"text\">Success</description>\r\n" +
                            "<error_no type=\"int\">0</error_no>\r\n" +
                            "<error_message type=\"text\">None</error_message>\r\n" +
                            "<is_active type=\"bool\">true</is_active>\r\n" +
                            "<trigger_id type=\"int\">1</trigger_id>\r\n\r\n" +
                            "<start_year type=\"int\">2023</start_year>\r\n" +
                            "<start_month type=\"int\">09</start_month>\r\n" +
                            "<start_day type=\"int\">12</start_day>\r\n" +
                            "<start_hour type=\"int\">08</start_hour>\r\n" +
                            "<start_minutes type=\"int\">00</start_minutes>\r\n" +
                            "<start_second type=\"int\">0</start_second>\r\n\r\n" +
                            "<end_year type=\"int\">2100</end_year>\r\n" +
                            "<end_month type=\"int\">09</end_month>\r\n" +
                            "<end_day type=\"int\">17</end_day>\r\n" +
                            "<end_hour type=\"int\">00</end_hour>\r\n" +
                            "<end_minutes type=\"int\">30</end_minutes>\r\n" +
                            "<end_second type=\"int\">0</end_second>\r\n\r\n" +
                            "<trigger_flag type=\"int\">999</trigger_flag>\r\n\r\n" +
                            "</xml>";
                    }
                #endregion

                #region homecafe DJMusic PUBLIC
                case "197":
                    string homecafeDJMusicPUBLICTriggerPath = homecafeDJMusicTriggerPath + "confirmEventTrigger.xml";
                    if (File.Exists(homecafeDJMusicPUBLICTriggerPath))
                    {
                        LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - ConfirmEventTrigger sent for PUBLIC homecafe DJMusic {eventId}!");
                        string res = File.ReadAllText(homecafeDJMusicPUBLICTriggerPath);
                        return "<xml>\r\n\t" +
                             "<result type=\"int\">1</result>\r\n\t" +
                             "<description type=\"text\">Success</description>\r\n\t" +
                             "<error_no type=\"int\">0</error_no>\r\n\t" +
                             "<error_message type=\"text\">None</error_message>\r\n\r\n\t" +
                             $"{res}\r\n" +
                             "</xml>";
                    }
                    else
                    {
                        LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - ConfirmEventTrigger FALLBACK sent for PUBLIC homecafe DJMusic {eventId}! Expected path {homecafeDJMusicPUBLICTriggerPath}");
                        return "<xml>\r\n" +
                            "<result type=\"int\">1</result>\r\n" +
                            "<description type=\"text\">Success</description>\r\n" +
                            "<error_no type=\"int\">0</error_no>\r\n" +
                            "<error_message type=\"text\">None</error_message>\r\n" +
                            "<is_active type=\"bool\">true</is_active>\r\n" +
                            "<trigger_id type=\"int\">1</trigger_id>\r\n\r\n" +
                            "<start_year type=\"int\">2023</start_year>\r\n" +
                            "<start_month type=\"int\">09</start_month>\r\n" +
                            "<start_day type=\"int\">12</start_day>\r\n" +
                            "<start_hour type=\"int\">08</start_hour>\r\n" +
                            "<start_minutes type=\"int\">00</start_minutes>\r\n" +
                            "<start_second type=\"int\">0</start_second>\r\n\r\n" +
                            "<end_year type=\"int\">2100</end_year>\r\n" +
                            "<end_month type=\"int\">09</end_month>\r\n" +
                            "<end_day type=\"int\">17</end_day>\r\n" +
                            "<end_hour type=\"int\">00</end_hour>\r\n" +
                            "<end_minutes type=\"int\">30</end_minutes>\r\n" +
                            "<end_second type=\"int\">0</end_second>\r\n\r\n" +
                            "<trigger_flag type=\"int\">999</trigger_flag>\r\n\r\n" +
                            "</xml>";
                    }
                #endregion

                #region homecafe Gallery PUBLIC
                case "264":
                    string homecafeGalleryPUBLICTriggerPath = homecafeGalleryTriggerPath + "confirmEventTrigger.xml";
                    if (File.Exists(homecafeGalleryPUBLICTriggerPath))
                    {
                        LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - ConfirmEventTrigger sent for PUBLIC homecafe Gallery {eventId}!");
                        string res = File.ReadAllText(homecafeGalleryPUBLICTriggerPath);
                        return "<xml>\r\n\t" +
                             "<result type=\"int\">1</result>\r\n\t" +
                             "<description type=\"text\">Success</description>\r\n\t" +
                             "<error_no type=\"int\">0</error_no>\r\n\t" +
                             "<error_message type=\"text\">None</error_message>\r\n\r\n\t" +
                             $"{res}\r\n" +
                             "</xml>";
                    }
                    else
                    {
                        LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - ConfirmEventTrigger FALLBACK sent for PUBLIC homecafe Gallery {eventId}! Expected path {homecafeGalleryPUBLICTriggerPath}");
                        return "<xml>\r\n" +
                            "<result type=\"int\">1</result>\r\n" +
                            "<description type=\"text\">Success</description>\r\n" +
                            "<error_no type=\"int\">0</error_no>\r\n" +
                            "<error_message type=\"text\">None</error_message>\r\n" +
                            "<is_active type=\"bool\">true</is_active>\r\n" +
                            "<trigger_id type=\"int\">1</trigger_id>\r\n\r\n" +
                            "<start_year type=\"int\">2023</start_year>\r\n" +
                            "<start_month type=\"int\">09</start_month>\r\n" +
                            "<start_day type=\"int\">12</start_day>\r\n" +
                            "<start_hour type=\"int\">08</start_hour>\r\n" +
                            "<start_minutes type=\"int\">00</start_minutes>\r\n" +
                            "<start_second type=\"int\">0</start_second>\r\n\r\n" +
                            "<end_year type=\"int\">2100</end_year>\r\n" +
                            "<end_month type=\"int\">09</end_month>\r\n" +
                            "<end_day type=\"int\">17</end_day>\r\n" +
                            "<end_hour type=\"int\">00</end_hour>\r\n" +
                            "<end_minutes type=\"int\">30</end_minutes>\r\n" +
                            "<end_second type=\"int\">0</end_second>\r\n\r\n" +
                            "<trigger_flag type=\"int\">999</trigger_flag>\r\n\r\n" +
                            "</xml>";
                    }
                #endregion

                #region homecafe RollyCafe1F LOCAL
                case "174":
                    string homecafeRollyCafe1FLOCALTriggerPath = homecafeRollyCafe1FTriggerPath + "localconfirmEventTrigger.xml";
                    if (File.Exists(homecafeRollyCafe1FLOCALTriggerPath))
                    {
                        LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - ConfirmEventTrigger sent for LOCAL homecafe RollyCafe1F {eventId}!");
                        string res = File.ReadAllText(homecafeRollyCafe1FLOCALTriggerPath);
                        return "<xml>\r\n\t" +
                             "<result type=\"int\">1</result>\r\n\t" +
                             "<description type=\"text\">Success</description>\r\n\t" +
                             "<error_no type=\"int\">0</error_no>\r\n\t" +
                             "<error_message type=\"text\">None</error_message>\r\n\r\n\t" +
                             $"{res}\r\n" +
                             "</xml>";
                    }
                    else
                    {
                        LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - ConfirmEventTrigger FALLBACK sent for LOCAL homecafe RollyCafe1F {eventId}! Expected path {homecafeRollyCafe1FLOCALTriggerPath}");
                        return "<xml>\r\n" +
                            "<result type=\"int\">1</result>\r\n" +
                            "<description type=\"text\">Success</description>\r\n" +
                            "<error_no type=\"int\">0</error_no>\r\n" +
                            "<error_message type=\"text\">None</error_message>\r\n" +
                            "<is_active type=\"bool\">true</is_active>\r\n" +
                            "<trigger_id type=\"int\">1</trigger_id>\r\n\r\n" +
                            "<start_year type=\"int\">2023</start_year>\r\n" +
                            "<start_month type=\"int\">09</start_month>\r\n" +
                            "<start_day type=\"int\">12</start_day>\r\n" +
                            "<start_hour type=\"int\">08</start_hour>\r\n" +
                            "<start_minutes type=\"int\">00</start_minutes>\r\n" +
                            "<start_second type=\"int\">0</start_second>\r\n\r\n" +
                            "<end_year type=\"int\">2100</end_year>\r\n" +
                            "<end_month type=\"int\">09</end_month>\r\n" +
                            "<end_day type=\"int\">17</end_day>\r\n" +
                            "<end_hour type=\"int\">00</end_hour>\r\n" +
                            "<end_minutes type=\"int\">30</end_minutes>\r\n" +
                            "<end_second type=\"int\">0</end_second>\r\n\r\n" +
                            "<trigger_flag type=\"int\">999</trigger_flag>\r\n\r\n" +
                            "</xml>";
                    }
                #endregion

                #region homecafe RollyCafe1F QA
                case "179":
                    string homecafeRollyCafe1FQATriggerPath = homecafeRollyCafe1FTriggerPath + "qaconfirmEventTrigger.xml";
                    if (File.Exists(homecafeRollyCafe1FQATriggerPath))
                    {
                        LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - ConfirmEventTrigger sent for QA homecafe RollyCafe1F {eventId}!");
                        string res = File.ReadAllText(homecafeRollyCafe1FQATriggerPath);
                        return "<xml>\r\n\t" +
                             "<result type=\"int\">1</result>\r\n\t" +
                             "<description type=\"text\">Success</description>\r\n\t" +
                             "<error_no type=\"int\">0</error_no>\r\n\t" +
                             "<error_message type=\"text\">None</error_message>\r\n\r\n\t" +
                             $"{res}\r\n" +
                             "</xml>";
                    }
                    else
                    {
                        LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - ConfirmEventTrigger FALLBACK sent for QA homecafe RollyCafe1F {eventId}! Expected path {homecafeRollyCafe1FQATriggerPath}");
                        return "<xml>\r\n" +
                            "<result type=\"int\">1</result>\r\n" +
                            "<description type=\"text\">Success</description>\r\n" +
                            "<error_no type=\"int\">0</error_no>\r\n" +
                            "<error_message type=\"text\">None</error_message>\r\n" +
                            "<is_active type=\"bool\">true</is_active>\r\n" +
                            "<trigger_id type=\"int\">1</trigger_id>\r\n\r\n" +
                            "<start_year type=\"int\">2023</start_year>\r\n" +
                            "<start_month type=\"int\">09</start_month>\r\n" +
                            "<start_day type=\"int\">12</start_day>\r\n" +
                            "<start_hour type=\"int\">08</start_hour>\r\n" +
                            "<start_minutes type=\"int\">00</start_minutes>\r\n" +
                            "<start_second type=\"int\">0</start_second>\r\n\r\n" +
                            "<end_year type=\"int\">2100</end_year>\r\n" +
                            "<end_month type=\"int\">09</end_month>\r\n" +
                            "<end_day type=\"int\">17</end_day>\r\n" +
                            "<end_hour type=\"int\">00</end_hour>\r\n" +
                            "<end_minutes type=\"int\">30</end_minutes>\r\n" +
                            "<end_second type=\"int\">0</end_second>\r\n\r\n" +
                            "<trigger_flag type=\"int\">999</trigger_flag>\r\n\r\n" +
                            "</xml>";
                    }
                #endregion

                #region Basara LOCAL
                case "180":
                    string basaraHomeSquareLOCALTriggerPath = basaraHomeSquareTriggerPath + "localconfirmEventTrigger.xml";
                    if (File.Exists(basaraHomeSquareLOCALTriggerPath))
                    {
                        LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - ConfirmEventTrigger sent for LOCAL Basara HomeSquare Collab {eventId}!");
                        string res = File.ReadAllText(basaraHomeSquareLOCALTriggerPath);
                        return "<xml>\r\n\t" +
                             "<result type=\"int\">1</result>\r\n\t" +
                             "<description type=\"text\">Success</description>\r\n\t" +
                             "<error_no type=\"int\">0</error_no>\r\n\t" +
                             "<error_message type=\"text\">None</error_message>\r\n\r\n\t" +
                             $"{res}\r\n" +
                             "</xml>";
                    }
                    else
                    {
                        LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - ConfirmEventTrigger FALLBACK sent for LOCAL Basara HomeSquare Collab {eventId}! Expected path {basaraHomeSquareLOCALTriggerPath}");
                        return "<xml>\r\n" +
                            "<result type=\"int\">1</result>\r\n" +
                            "<description type=\"text\">Success</description>\r\n" +
                            "<error_no type=\"int\">0</error_no>\r\n" +
                            "<error_message type=\"text\">None</error_message>\r\n" +
                            "<is_active type=\"bool\">true</is_active>\r\n" +
                            "<trigger_id type=\"int\">1</trigger_id>\r\n\r\n" +
                            "<start_year type=\"int\">2023</start_year>\r\n" +
                            "<start_month type=\"int\">09</start_month>\r\n" +
                            "<start_day type=\"int\">12</start_day>\r\n" +
                            "<start_hour type=\"int\">08</start_hour>\r\n" +
                            "<start_minutes type=\"int\">00</start_minutes>\r\n" +
                            "<start_second type=\"int\">0</start_second>\r\n\r\n" +
                            "<end_year type=\"int\">2100</end_year>\r\n" +
                            "<end_month type=\"int\">09</end_month>\r\n" +
                            "<end_day type=\"int\">17</end_day>\r\n" +
                            "<end_hour type=\"int\">00</end_hour>\r\n" +
                            "<end_minutes type=\"int\">30</end_minutes>\r\n" +
                            "<end_second type=\"int\">0</end_second>\r\n\r\n" +
                            "<trigger_flag type=\"int\">999</trigger_flag>\r\n\r\n" +
                            "</xml>";
                    }
                #endregion

                #region Macross SSF LOCAL
                case "183":
                    string MacrossSSFLOCALTriggerPath = MacrossSSFTriggerPath + "localconfirmEventTrigger.xml";
                    if (File.Exists(MacrossSSFLOCALTriggerPath))
                    {
                        LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - ConfirmEventTrigger sent for LOCAL Macross SS F/Fifa Relocator {eventId}!");
                        string res = File.ReadAllText(MacrossSSFLOCALTriggerPath);
                        return "<xml>\r\n\t" +
                             "<result type=\"int\">1</result>\r\n\t" +
                             "<description type=\"text\">Success</description>\r\n\t" +
                             "<error_no type=\"int\">0</error_no>\r\n\t" +
                             "<error_message type=\"text\">None</error_message>\r\n\r\n\t" +
                             $"{res}\r\n" +
                             "</xml>";
                    }
                    else
                    {
                        LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - ConfirmEventTrigger FALLBACK sent for LOCAL Macross SS F/Fifa Relocator {eventId}! Expected path {MacrossSSFLOCALTriggerPath}");
                        return "<xml>\r\n" +
                            "<result type=\"int\">1</result>\r\n" +
                            "<description type=\"text\">Success</description>\r\n" +
                            "<error_no type=\"int\">0</error_no>\r\n" +
                            "<error_message type=\"text\">None</error_message>\r\n" +
                            "<is_active type=\"bool\">true</is_active>\r\n" +
                            "<trigger_id type=\"int\">1</trigger_id>\r\n\r\n" +
                            "<start_year type=\"int\">2023</start_year>\r\n" +
                            "<start_month type=\"int\">09</start_month>\r\n" +
                            "<start_day type=\"int\">12</start_day>\r\n" +
                            "<start_hour type=\"int\">08</start_hour>\r\n" +
                            "<start_minutes type=\"int\">00</start_minutes>\r\n" +
                            "<start_second type=\"int\">0</start_second>\r\n\r\n" +
                            "<end_year type=\"int\">2100</end_year>\r\n" +
                            "<end_month type=\"int\">09</end_month>\r\n" +
                            "<end_day type=\"int\">17</end_day>\r\n" +
                            "<end_hour type=\"int\">00</end_hour>\r\n" +
                            "<end_minutes type=\"int\">30</end_minutes>\r\n" +
                            "<end_second type=\"int\">0</end_second>\r\n\r\n" +
                            "<trigger_flag type=\"int\">999</trigger_flag>\r\n\r\n" +
                            "</xml>";
                    }
                #endregion

                #region Basara QA
                case "192":
                    string basaraHomeSquareQATriggerPath = basaraHomeSquareTriggerPath + "qaconfirmEventTrigger.xml";
                    if (File.Exists(basaraHomeSquareQATriggerPath))
                    {
                        LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - ConfirmEventTrigger sent for QA Basara HomeSquare Collab {eventId}!");
                        string res = File.ReadAllText(basaraHomeSquareQATriggerPath);
                        return "<xml>\r\n\t" +
                             "<result type=\"int\">1</result>\r\n\t" +
                             "<description type=\"text\">Success</description>\r\n\t" +
                             "<error_no type=\"int\">0</error_no>\r\n\t" +
                             "<error_message type=\"text\">None</error_message>\r\n\r\n\t" +
                             $"{res}\r\n" +
                             "</xml>";
                    }
                    else
                    {
                        LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - ConfirmEventTrigger FALLBACK sent for QA Basara HomeSquare Collab {eventId}! Expected path {basaraHomeSquareQATriggerPath}");
                        return "<xml>\r\n" +
                            "<result type=\"int\">1</result>\r\n" +
                            "<description type=\"text\">Success</description>\r\n" +
                            "<error_no type=\"int\">0</error_no>\r\n" +
                            "<error_message type=\"text\">None</error_message>\r\n" +
                            "<is_active type=\"bool\">true</is_active>\r\n" +
                            "<trigger_id type=\"int\">1</trigger_id>\r\n\r\n" +
                            "<start_year type=\"int\">2023</start_year>\r\n" +
                            "<start_month type=\"int\">09</start_month>\r\n" +
                            "<start_day type=\"int\">12</start_day>\r\n" +
                            "<start_hour type=\"int\">08</start_hour>\r\n" +
                            "<start_minutes type=\"int\">00</start_minutes>\r\n" +
                            "<start_second type=\"int\">0</start_second>\r\n\r\n" +
                            "<end_year type=\"int\">2100</end_year>\r\n" +
                            "<end_month type=\"int\">09</end_month>\r\n" +
                            "<end_day type=\"int\">17</end_day>\r\n" +
                            "<end_hour type=\"int\">00</end_hour>\r\n" +
                            "<end_minutes type=\"int\">30</end_minutes>\r\n" +
                            "<end_second type=\"int\">0</end_second>\r\n\r\n" +
                            "<trigger_flag type=\"int\">999</trigger_flag>\r\n\r\n" +
                            "</xml>";
                    }
                #endregion

                #region RollyCafe1F PUBLIC
                case "201":
                    string homecafeRollyCafe1FPUBLICTriggerPath = homecafeRollyCafe1FTriggerPath + "confirmEventTrigger.xml";
                    if (File.Exists(homecafeRollyCafe1FPUBLICTriggerPath))
                    {
                        LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - ConfirmEventTrigger sent for PUBLIC homecafe RollyCafe1F {eventId}!");
                        string res = File.ReadAllText(homecafeRollyCafe1FPUBLICTriggerPath);
                        return "<xml>\r\n\t" +
                             "<result type=\"int\">1</result>\r\n\t" +
                             "<description type=\"text\">Success</description>\r\n\t" +
                             "<error_no type=\"int\">0</error_no>\r\n\t" +
                             "<error_message type=\"text\">None</error_message>\r\n\r\n\t" +
                             $"{res}\r\n" +
                             "</xml>";
                    }
                    else
                    {
                        LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - ConfirmEventTrigger FALLBACK sent for PUBLIC homecafe RollyCafe1F {eventId}! Expected path {homecafeRollyCafe1FPUBLICTriggerPath}");
                        return "<xml>\r\n" +
                            "<result type=\"int\">1</result>\r\n" +
                            "<description type=\"text\">Success</description>\r\n" +
                            "<error_no type=\"int\">0</error_no>\r\n" +
                            "<error_message type=\"text\">None</error_message>\r\n" +
                            "<is_active type=\"bool\">true</is_active>\r\n" +
                            "<trigger_id type=\"int\">1</trigger_id>\r\n\r\n" +
                            "<start_year type=\"int\">2023</start_year>\r\n" +
                            "<start_month type=\"int\">09</start_month>\r\n" +
                            "<start_day type=\"int\">12</start_day>\r\n" +
                            "<start_hour type=\"int\">08</start_hour>\r\n" +
                            "<start_minutes type=\"int\">00</start_minutes>\r\n" +
                            "<start_second type=\"int\">0</start_second>\r\n\r\n" +
                            "<end_year type=\"int\">2100</end_year>\r\n" +
                            "<end_month type=\"int\">09</end_month>\r\n" +
                            "<end_day type=\"int\">17</end_day>\r\n" +
                            "<end_hour type=\"int\">00</end_hour>\r\n" +
                            "<end_minutes type=\"int\">30</end_minutes>\r\n" +
                            "<end_second type=\"int\">0</end_second>\r\n\r\n" +
                            "<trigger_flag type=\"int\">999</trigger_flag>\r\n\r\n" +
                            "</xml>";
                    }
                #endregion

                #region Basara PUBLIC
                case "210":
                    string basaraHomeSquarePUBLICTriggerPath = basaraHomeSquareTriggerPath + "confirmEventTrigger.xml";
                    if (File.Exists(basaraHomeSquarePUBLICTriggerPath))
                    {
                        LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - ConfirmEventTrigger sent for PUBLIC Basara HomeSquare Collab {eventId}!");
                        string res = File.ReadAllText(basaraHomeSquarePUBLICTriggerPath);
                        return "<xml>\r\n\t" +
                             "<result type=\"int\">1</result>\r\n\t" +
                             "<description type=\"text\">Success</description>\r\n\t" +
                             "<error_no type=\"int\">0</error_no>\r\n\t" +
                             "<error_message type=\"text\">None</error_message>\r\n\r\n\t" +
                             $"{res}\r\n" +
                             "</xml>";
                    }
                    else
                    {
                        LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - ConfirmEventTrigger FALLBACK sent for PUBLIC Basara HomeSquare Collab {eventId}! Expected path {basaraHomeSquarePUBLICTriggerPath}");
                        return "<xml>\r\n" +
                            "<result type=\"int\">1</result>\r\n" +
                            "<description type=\"text\">Success</description>\r\n" +
                            "<error_no type=\"int\">0</error_no>\r\n" +
                            "<error_message type=\"text\">None</error_message>\r\n" +
                            "<is_active type=\"bool\">true</is_active>\r\n" +
                            "<trigger_id type=\"int\">1</trigger_id>\r\n\r\n" +
                            "<start_year type=\"int\">2023</start_year>\r\n" +
                            "<start_month type=\"int\">09</start_month>\r\n" +
                            "<start_day type=\"int\">12</start_day>\r\n" +
                            "<start_hour type=\"int\">08</start_hour>\r\n" +
                            "<start_minutes type=\"int\">00</start_minutes>\r\n" +
                            "<start_second type=\"int\">0</start_second>\r\n\r\n" +
                            "<end_year type=\"int\">2100</end_year>\r\n" +
                            "<end_month type=\"int\">09</end_month>\r\n" +
                            "<end_day type=\"int\">17</end_day>\r\n" +
                            "<end_hour type=\"int\">00</end_hour>\r\n" +
                            "<end_minutes type=\"int\">30</end_minutes>\r\n" +
                            "<end_second type=\"int\">0</end_second>\r\n\r\n" +
                            "<trigger_flag type=\"int\">999</trigger_flag>\r\n\r\n" +
                            "</xml>";
                    }
                #endregion

                #region Macross SSF QA
                case "275":
                    string MacrossSSFQATriggerPath = MacrossSSFTriggerPath + "qaconfirmEventTrigger.xml";
                    if (File.Exists(MacrossSSFQATriggerPath))
                    {
                        LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - ConfirmEventTrigger sent for QA Macross SS F/Fifa Relocator {eventId}!");
                        string res = File.ReadAllText(MacrossSSFQATriggerPath);
                        return "<xml>\r\n\t" +
                             "<result type=\"int\">1</result>\r\n\t" +
                             "<description type=\"text\">Success</description>\r\n\t" +
                             "<error_no type=\"int\">0</error_no>\r\n\t" +
                             "<error_message type=\"text\">None</error_message>\r\n\r\n\t" +
                             $"{res}\r\n" +
                             "</xml>";
                    }
                    else
                    {
                        LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - ConfirmEventTrigger FALLBACK sent for QA Macross SS F/Fifa Relocator {eventId}! Expected path {MacrossSSFQATriggerPath}");
                        return "<xml>\r\n" +
                            "<result type=\"int\">1</result>\r\n" +
                            "<description type=\"text\">Success</description>\r\n" +
                            "<error_no type=\"int\">0</error_no>\r\n" +
                            "<error_message type=\"text\">None</error_message>\r\n" +
                            "<is_active type=\"bool\">true</is_active>\r\n" +
                            "<trigger_id type=\"int\">1</trigger_id>\r\n\r\n" +
                            "<start_year type=\"int\">2023</start_year>\r\n" +
                            "<start_month type=\"int\">09</start_month>\r\n" +
                            "<start_day type=\"int\">12</start_day>\r\n" +
                            "<start_hour type=\"int\">08</start_hour>\r\n" +
                            "<start_minutes type=\"int\">00</start_minutes>\r\n" +
                            "<start_second type=\"int\">0</start_second>\r\n\r\n" +
                            "<end_year type=\"int\">2100</end_year>\r\n" +
                            "<end_month type=\"int\">09</end_month>\r\n" +
                            "<end_day type=\"int\">17</end_day>\r\n" +
                            "<end_hour type=\"int\">00</end_hour>\r\n" +
                            "<end_minutes type=\"int\">30</end_minutes>\r\n" +
                            "<end_second type=\"int\">0</end_second>\r\n\r\n" +
                            "<trigger_flag type=\"int\">999</trigger_flag>\r\n\r\n" +
                            "</xml>";
                    }
                #endregion

                #region Macross VF25 HS QA
                case "282":
                    string MacrossVF25QATriggerPath = MacrossVF25TriggerPath + "qaconfirmEventTrigger.xml";
                    if (File.Exists(MacrossVF25QATriggerPath))
                    {
                        LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - ConfirmEventTrigger sent for QA Macross VF25 HS {eventId}!");
                        string res = File.ReadAllText(MacrossVF25QATriggerPath);
                        return "<xml>\r\n\t" +
                             "<result type=\"int\">1</result>\r\n\t" +
                             "<description type=\"text\">Success</description>\r\n\t" +
                             "<error_no type=\"int\">0</error_no>\r\n\t" +
                             "<error_message type=\"text\">None</error_message>\r\n\r\n\t" +
                             $"{res}\r\n" +
                             "</xml>";
                    }
                    else
                    {
                        LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - ConfirmEventTrigger FALLBACK sent for QA Macross VF25 HS {eventId}! Expected path {MacrossVF25QATriggerPath}");
                        return "<xml>\r\n" +
                            "<result type=\"int\">1</result>\r\n" +
                            "<description type=\"text\">Success</description>\r\n" +
                            "<error_no type=\"int\">0</error_no>\r\n" +
                            "<error_message type=\"text\">None</error_message>\r\n" +
                            "<is_active type=\"bool\">true</is_active>\r\n" +
                            "<trigger_id type=\"int\">1</trigger_id>\r\n\r\n" +
                            "<start_year type=\"int\">2023</start_year>\r\n" +
                            "<start_month type=\"int\">09</start_month>\r\n" +
                            "<start_day type=\"int\">12</start_day>\r\n" +
                            "<start_hour type=\"int\">08</start_hour>\r\n" +
                            "<start_minutes type=\"int\">00</start_minutes>\r\n" +
                            "<start_second type=\"int\">0</start_second>\r\n\r\n" +
                            "<end_year type=\"int\">2100</end_year>\r\n" +
                            "<end_month type=\"int\">09</end_month>\r\n" +
                            "<end_day type=\"int\">17</end_day>\r\n" +
                            "<end_hour type=\"int\">00</end_hour>\r\n" +
                            "<end_minutes type=\"int\">30</end_minutes>\r\n" +
                            "<end_second type=\"int\">0</end_second>\r\n\r\n" +
                            "<trigger_flag type=\"int\">999</trigger_flag>\r\n\r\n" +
                            "</xml>";
                    }
                #endregion

                #region j_liargame2 POST
                case "296":
                    string j_liargame2POSTTriggerPath = j_liargame2TriggerPath + "confirmEventTrigger_POST.xml";
                    if (File.Exists(j_liargame2POSTTriggerPath))
                    {
                        LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - ConfirmEventTrigger sent for POST evid PUBLIC LiarGame2 {eventId}!");
                        string res = File.ReadAllText(j_liargame2POSTTriggerPath);
                        return "<xml>\r\n\t" +
                             "<result type=\"int\">1</result>\r\n\t" +
                             "<description type=\"text\">Success</description>\r\n\t" +
                             "<error_no type=\"int\">0</error_no>\r\n\t" +
                             "<error_message type=\"text\">None</error_message>\r\n\r\n\t" +
                             $"{res}\r\n" +
                             "</xml>";
                    }
                    else
                    {
                        LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - ConfirmEventTrigger FALLBACK sent for POST evid PUBLIC LiarGame2 {eventId}! Expected path {j_liargame2POSTTriggerPath}");
                        return "<xml>\r\n" +
                            "<result type=\"int\">1</result>\r\n" +
                            "<description type=\"text\">Success</description>\r\n" +
                            "<error_no type=\"int\">0</error_no>\r\n" +
                            "<error_message type=\"text\">None</error_message>\r\n" +
                            "<is_active type=\"bool\">true</is_active>\r\n" +
                            "<trigger_id type=\"int\">1</trigger_id>\r\n\r\n" +
                            "<start_year type=\"int\">2023</start_year>\r\n" +
                            "<start_month type=\"int\">09</start_month>\r\n" +
                            "<start_day type=\"int\">12</start_day>\r\n" +
                            "<start_hour type=\"int\">08</start_hour>\r\n" +
                            "<start_minutes type=\"int\">00</start_minutes>\r\n" +
                            "<start_second type=\"int\">0</start_second>\r\n\r\n" +
                            "<end_year type=\"int\">2100</end_year>\r\n" +
                            "<end_month type=\"int\">09</end_month>\r\n" +
                            "<end_day type=\"int\">17</end_day>\r\n" +
                            "<end_hour type=\"int\">00</end_hour>\r\n" +
                            "<end_minutes type=\"int\">30</end_minutes>\r\n" +
                            "<end_second type=\"int\">0</end_second>\r\n\r\n" +
                            "<trigger_flag type=\"int\">999</trigger_flag>\r\n\r\n" +
                            "</xml>";
                    }
                #endregion

                #region j_liargame2 CALL
                case "297":
                    string j_liargame2CALLTriggerPath = j_liargame2TriggerPath + "confirmEventTrigger_CALL.xml";
                    if (File.Exists(j_liargame2CALLTriggerPath))
                    {
                        LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - ConfirmEventTrigger sent for CALL evid PUBLIC LiarGame2 {eventId}!");
                        string res = File.ReadAllText(j_liargame2CALLTriggerPath);
                        return "<xml>\r\n\t" +
                             "<result type=\"int\">1</result>\r\n\t" +
                             "<description type=\"text\">Success</description>\r\n\t" +
                             "<error_no type=\"int\">0</error_no>\r\n\t" +
                             "<error_message type=\"text\">None</error_message>\r\n\r\n\t" +
                             $"{res}\r\n" +
                             "</xml>";
                    }
                    else
                    {
                        LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - ConfirmEventTrigger FALLBACK sent for CALL evid PUBLIC LiarGame2 {eventId}! Expected path {j_liargame2CALLTriggerPath}");
                        return "<xml>\r\n" +
                            "<result type=\"int\">1</result>\r\n" +
                            "<description type=\"text\">Success</description>\r\n" +
                            "<error_no type=\"int\">0</error_no>\r\n" +
                            "<error_message type=\"text\">None</error_message>\r\n" +
                            "<is_active type=\"bool\">true</is_active>\r\n" +
                            "<trigger_id type=\"int\">1</trigger_id>\r\n\r\n" +
                            "<start_year type=\"int\">2023</start_year>\r\n" +
                            "<start_month type=\"int\">09</start_month>\r\n" +
                            "<start_day type=\"int\">12</start_day>\r\n" +
                            "<start_hour type=\"int\">08</start_hour>\r\n" +
                            "<start_minutes type=\"int\">00</start_minutes>\r\n" +
                            "<start_second type=\"int\">0</start_second>\r\n\r\n" +
                            "<end_year type=\"int\">2100</end_year>\r\n" +
                            "<end_month type=\"int\">09</end_month>\r\n" +
                            "<end_day type=\"int\">17</end_day>\r\n" +
                            "<end_hour type=\"int\">00</end_hour>\r\n" +
                            "<end_minutes type=\"int\">30</end_minutes>\r\n" +
                            "<end_second type=\"int\">0</end_second>\r\n\r\n" +
                            "<trigger_flag type=\"int\">999</trigger_flag>\r\n\r\n" +
                            "</xml>";
                    }
                #endregion

                #region Halloween 2010
                case "299":
                    string Halloween2010TriggerPathAmbiencePublic = Halloween2010EffectsTriggerPath + "confirmEventTrigger.xml";
                    if (File.Exists(Halloween2010TriggerPathAmbiencePublic))
                    {
                        LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - ConfirmEventTrigger sent for evid PUBLIC Halloween 2010 {eventId}!");
                        string res = File.ReadAllText(Halloween2010TriggerPathAmbiencePublic);
                        return "<xml>\r\n\t" +
                             "<result type=\"int\">1</result>\r\n\t" +
                             "<description type=\"text\">Success</description>\r\n\t" +
                             "<error_no type=\"int\">0</error_no>\r\n\t" +
                             "<error_message type=\"text\">None</error_message>\r\n\r\n\t" +
                             $"{res}\r\n" +
                             "</xml>";
                    }
                    else
                    {
                        LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - ConfirmEventTrigger FALLBACK sent for evid PUBLIC Halloween 2010 {eventId}! Expected path {Halloween2010TriggerPathAmbiencePublic}");
                        return "<xml>\r\n" +
                            "<result type=\"int\">1</result>\r\n" +
                            "<description type=\"text\">Success</description>\r\n" +
                            "<error_no type=\"int\">0</error_no>\r\n" +
                            "<error_message type=\"text\">None</error_message>\r\n" +
                            "<is_active type=\"bool\">true</is_active>\r\n" +
                            "<trigger_id type=\"int\">1</trigger_id>\r\n\r\n" +
                            "<start_year type=\"int\">2023</start_year>\r\n" +
                            "<start_month type=\"int\">09</start_month>\r\n" +
                            "<start_day type=\"int\">12</start_day>\r\n" +
                            "<start_hour type=\"int\">08</start_hour>\r\n" +
                            "<start_minutes type=\"int\">00</start_minutes>\r\n" +
                            "<start_second type=\"int\">0</start_second>\r\n\r\n" +
                            "<end_year type=\"int\">2100</end_year>\r\n" +
                            "<end_month type=\"int\">09</end_month>\r\n" +
                            "<end_day type=\"int\">17</end_day>\r\n" +
                            "<end_hour type=\"int\">00</end_hour>\r\n" +
                            "<end_minutes type=\"int\">30</end_minutes>\r\n" +
                            "<end_second type=\"int\">0</end_second>\r\n\r\n" +
                            "<trigger_flag type=\"int\">999</trigger_flag>\r\n\r\n" +
                            "</xml>";
                    }

                case "298":
                    string Halloween2010TriggerPathPublic = Halloween2010TriggerPath + "confirmEventTrigger.xml";
                    if (File.Exists(Halloween2010TriggerPathPublic))
                    {
                        LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - ConfirmEventTrigger sent for evid PUBLIC Halloween 2010 {eventId}!");
                        string res = File.ReadAllText(Halloween2010TriggerPathPublic);
                        return "<xml>\r\n\t" +
                             "<result type=\"int\">1</result>\r\n\t" +
                             "<description type=\"text\">Success</description>\r\n\t" +
                             "<error_no type=\"int\">0</error_no>\r\n\t" +
                             "<error_message type=\"text\">None</error_message>\r\n\r\n\t" +
                             $"{res}\r\n" +
                             "</xml>";
                    }
                    else
                    {
                        LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - ConfirmEventTrigger FALLBACK sent for evid PUBLIC Halloween 2010 {eventId}! Expected path {Halloween2010TriggerPathPublic}");
                        return "<xml>\r\n" +
                            "<result type=\"int\">1</result>\r\n" +
                            "<description type=\"text\">Success</description>\r\n" +
                            "<error_no type=\"int\">0</error_no>\r\n" +
                            "<error_message type=\"text\">None</error_message>\r\n" +
                            "<is_active type=\"bool\">true</is_active>\r\n" +
                            "<trigger_id type=\"int\">1</trigger_id>\r\n\r\n" +
                            "<start_year type=\"int\">2023</start_year>\r\n" +
                            "<start_month type=\"int\">09</start_month>\r\n" +
                            "<start_day type=\"int\">12</start_day>\r\n" +
                            "<start_hour type=\"int\">08</start_hour>\r\n" +
                            "<start_minutes type=\"int\">00</start_minutes>\r\n" +
                            "<start_second type=\"int\">0</start_second>\r\n\r\n" +
                            "<end_year type=\"int\">2100</end_year>\r\n" +
                            "<end_month type=\"int\">09</end_month>\r\n" +
                            "<end_day type=\"int\">17</end_day>\r\n" +
                            "<end_hour type=\"int\">00</end_hour>\r\n" +
                            "<end_minutes type=\"int\">30</end_minutes>\r\n" +
                            "<end_second type=\"int\">0</end_second>\r\n\r\n" +
                            "<trigger_flag type=\"int\">999</trigger_flag>\r\n\r\n" +
                            "</xml>";
                    }
                #endregion

                #region Macross Campaign On-Site Sales Office PUBLIC
                case "331":
                    string MacrossEventShopPUBLICTriggerPath = MacrossEventShopTriggerPath + "confirmEventTrigger.xml";
                    if (File.Exists(MacrossEventShopPUBLICTriggerPath))
                    {
                        LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - ConfirmEventTrigger sent for PUBLIC Macross Campaign on-site Sales Office {eventId}!");
                        string res = File.ReadAllText(MacrossEventShopPUBLICTriggerPath);
                        return "<xml>\r\n\t" +
                             "<result type=\"int\">1</result>\r\n\t" +
                             "<description type=\"text\">Success</description>\r\n\t" +
                             "<error_no type=\"int\">0</error_no>\r\n\t" +
                             "<error_message type=\"text\">None</error_message>\r\n\r\n\t" +
                             $"{res}\r\n" +
                             "</xml>";
                    }
                    else
                    {
                        LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - ConfirmEventTrigger FALLBACK sent for PUBLIC Macross Campaign on-site Sales Office {eventId}! Expected path {MacrossEventShopPUBLICTriggerPath}");
                        return "<xml>\r\n" +
                            "<result type=\"int\">1</result>\r\n" +
                            "<description type=\"text\">Success</description>\r\n" +
                            "<error_no type=\"int\">0</error_no>\r\n" +
                            "<error_message type=\"text\">None</error_message>\r\n" +
                            "<is_active type=\"bool\">true</is_active>\r\n" +
                            "<trigger_id type=\"int\">1</trigger_id>\r\n\r\n" +
                            "<start_year type=\"int\">2023</start_year>\r\n" +
                            "<start_month type=\"int\">09</start_month>\r\n" +
                            "<start_day type=\"int\">12</start_day>\r\n" +
                            "<start_hour type=\"int\">08</start_hour>\r\n" +
                            "<start_minutes type=\"int\">00</start_minutes>\r\n" +
                            "<start_second type=\"int\">0</start_second>\r\n\r\n" +
                            "<end_year type=\"int\">2100</end_year>\r\n" +
                            "<end_month type=\"int\">09</end_month>\r\n" +
                            "<end_day type=\"int\">17</end_day>\r\n" +
                            "<end_hour type=\"int\">00</end_hour>\r\n" +
                            "<end_minutes type=\"int\">30</end_minutes>\r\n" +
                            "<end_second type=\"int\">0</end_second>\r\n\r\n" +
                            "<trigger_flag type=\"int\">999</trigger_flag>\r\n\r\n" +
                            "</xml>";
                    }
                #endregion

                #region Macross SSF PUBLIC
                case "332":
                    string MacrossSSFPUBLICTriggerPath = MacrossSSFTriggerPath + "qaconfirmEventTrigger.xml";
                    if (File.Exists(MacrossSSFPUBLICTriggerPath))
                    {
                        LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - ConfirmEventTrigger sent for PUBLIC Macross SS F/Fifa Relocator {eventId}!");
                        string res = File.ReadAllText(MacrossSSFPUBLICTriggerPath);
                        return "<xml>\r\n\t" +
                             "<result type=\"int\">1</result>\r\n\t" +
                             "<description type=\"text\">Success</description>\r\n\t" +
                             "<error_no type=\"int\">0</error_no>\r\n\t" +
                             "<error_message type=\"text\">None</error_message>\r\n\r\n\t" +
                             $"{res}\r\n" +
                             "</xml>";
                    }
                    else
                    {
                        LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - ConfirmEventTrigger FALLBACK sent for PUBLIC Macross SS F/Fifa Relocator {eventId}! Expected path {MacrossSSFPUBLICTriggerPath}");
                        return "<xml>\r\n" +
                            "<result type=\"int\">1</result>\r\n" +
                            "<description type=\"text\">Success</description>\r\n" +
                            "<error_no type=\"int\">0</error_no>\r\n" +
                            "<error_message type=\"text\">None</error_message>\r\n" +
                            "<is_active type=\"bool\">true</is_active>\r\n" +
                            "<trigger_id type=\"int\">1</trigger_id>\r\n\r\n" +
                            "<start_year type=\"int\">2023</start_year>\r\n" +
                            "<start_month type=\"int\">09</start_month>\r\n" +
                            "<start_day type=\"int\">12</start_day>\r\n" +
                            "<start_hour type=\"int\">08</start_hour>\r\n" +
                            "<start_minutes type=\"int\">00</start_minutes>\r\n" +
                            "<start_second type=\"int\">0</start_second>\r\n\r\n" +
                            "<end_year type=\"int\">2100</end_year>\r\n" +
                            "<end_month type=\"int\">09</end_month>\r\n" +
                            "<end_day type=\"int\">17</end_day>\r\n" +
                            "<end_hour type=\"int\">00</end_hour>\r\n" +
                            "<end_minutes type=\"int\">30</end_minutes>\r\n" +
                            "<end_second type=\"int\">0</end_second>\r\n\r\n" +
                            "<trigger_flag type=\"int\">999</trigger_flag>\r\n\r\n" +
                            "</xml>";
                    }
                #endregion

                #region Macross VF25 PUBLIC
                case "335":
                    string MacrossVF25PUBLICTriggerPath = MacrossVF25TriggerPath + "confirmEventTrigger.xml";
                    if (File.Exists(MacrossVF25PUBLICTriggerPath))
                    {
                        LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - ConfirmEventTrigger sent for PUBLIC Macross VF25 HS {eventId}!");
                        string res = File.ReadAllText(MacrossVF25PUBLICTriggerPath);
                        return "<xml>\r\n\t" +
                             "<result type=\"int\">1</result>\r\n\t" +
                             "<description type=\"text\">Success</description>\r\n\t" +
                             "<error_no type=\"int\">0</error_no>\r\n\t" +
                             "<error_message type=\"text\">None</error_message>\r\n\r\n\t" +
                             $"{res}\r\n" +
                             "</xml>";
                    }
                    else
                    {
                        LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - ConfirmEventTrigger FALLBACK sent for PUBLIC Macross VF25 HS {eventId}! Expected path {MacrossVF25PUBLICTriggerPath}");
                        return "<xml>\r\n" +
                            "<result type=\"int\">1</result>\r\n" +
                            "<description type=\"text\">Success</description>\r\n" +
                            "<error_no type=\"int\">0</error_no>\r\n" +
                            "<error_message type=\"text\">None</error_message>\r\n" +
                            "<is_active type=\"bool\">true</is_active>\r\n" +
                            "<trigger_id type=\"int\">1</trigger_id>\r\n\r\n" +
                            "<start_year type=\"int\">2023</start_year>\r\n" +
                            "<start_month type=\"int\">09</start_month>\r\n" +
                            "<start_day type=\"int\">12</start_day>\r\n" +
                            "<start_hour type=\"int\">08</start_hour>\r\n" +
                            "<start_minutes type=\"int\">00</start_minutes>\r\n" +
                            "<start_second type=\"int\">0</start_second>\r\n\r\n" +
                            "<end_year type=\"int\">2100</end_year>\r\n" +
                            "<end_month type=\"int\">09</end_month>\r\n" +
                            "<end_day type=\"int\">17</end_day>\r\n" +
                            "<end_hour type=\"int\">00</end_hour>\r\n" +
                            "<end_minutes type=\"int\">30</end_minutes>\r\n" +
                            "<end_second type=\"int\">0</end_second>\r\n\r\n" +
                            "<trigger_flag type=\"int\">999</trigger_flag>\r\n\r\n" +
                            "</xml>";
                    }
                #endregion

                #region Sony Aquarium Relocator PUBLIC
                case "346":
                    string SonyAquariumRelocatorPUBLICTriggerPath = SonyAquariumRelocatorTriggerPath + "confirmEventTrigger.xml";
                    if (File.Exists(SonyAquariumRelocatorPUBLICTriggerPath))
                    {
                        LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - ConfirmEventTrigger sent for PUBLIC Sony Aquarium HomeSquare Relocator {eventId}!");
                        string res = File.ReadAllText(SonyAquariumRelocatorPUBLICTriggerPath);
                        return "<xml>\r\n\t" +
                             "<result type=\"int\">1</result>\r\n\t" +
                             "<description type=\"text\">Success</description>\r\n\t" +
                             "<error_no type=\"int\">0</error_no>\r\n\t" +
                             "<error_message type=\"text\">None</error_message>\r\n\r\n\t" +
                             $"{res}\r\n" +
                             "</xml>";
                    }
                    else
                    {
                        LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - ConfirmEventTrigger FALLBACK sent for PUBLIC Sony Aquarium VideoScreens {eventId}! Expected path {SonyAquariumRelocatorPUBLICTriggerPath}");
                        return "<xml>\r\n" +
                            "<result type=\"int\">1</result>\r\n" +
                            "<description type=\"text\">Success</description>\r\n" +
                            "<error_no type=\"int\">0</error_no>\r\n" +
                            "<error_message type=\"text\">None</error_message>\r\n" +
                            "<is_active type=\"bool\">true</is_active>\r\n" +
                            "<trigger_id type=\"int\">1</trigger_id>\r\n\r\n" +
                            "<start_year type=\"int\">2023</start_year>\r\n" +
                            "<start_month type=\"int\">09</start_month>\r\n" +
                            "<start_day type=\"int\">12</start_day>\r\n" +
                            "<start_hour type=\"int\">08</start_hour>\r\n" +
                            "<start_minutes type=\"int\">00</start_minutes>\r\n" +
                            "<start_second type=\"int\">0</start_second>\r\n\r\n" +
                            "<end_year type=\"int\">2100</end_year>\r\n" +
                            "<end_month type=\"int\">09</end_month>\r\n" +
                            "<end_day type=\"int\">17</end_day>\r\n" +
                            "<end_hour type=\"int\">00</end_hour>\r\n" +
                            "<end_minutes type=\"int\">30</end_minutes>\r\n" +
                            "<end_second type=\"int\">0</end_second>\r\n\r\n" +
                            "<trigger_flag type=\"int\">999</trigger_flag>\r\n\r\n" +
                            "</xml>";
                    }
                #endregion

                #region Sony Aquarium VideoConfig/Screens PUBLIC
                case "349":
                    string SonyAquariumVideoConfigPUBLICTriggerPath = SonyAquariumVideoConfigTriggerPath + "confirmEventTrigger.xml";
                    if (File.Exists(SonyAquariumVideoConfigPUBLICTriggerPath))
                    {
                        LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - ConfirmEventTrigger sent for PUBLIC Sony Aquarium VideoScreens {eventId}!");
                        string res = File.ReadAllText(SonyAquariumVideoConfigPUBLICTriggerPath);
                        return "<xml>\r\n\t" +
                             "<result type=\"int\">1</result>\r\n\t" +
                             "<description type=\"text\">Success</description>\r\n\t" +
                             "<error_no type=\"int\">0</error_no>\r\n\t" +
                             "<error_message type=\"text\">None</error_message>\r\n\r\n\t" +
                             $"{res}\r\n" +
                             "</xml>";
                    }
                    else
                    {
                        LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - ConfirmEventTrigger FALLBACK sent for PUBLIC Sony Aquarium VideoScreens {eventId}! Expected path {SonyAquariumVideoConfigPUBLICTriggerPath}");
                        return "<xml>\r\n" +
                            "<result type=\"int\">1</result>\r\n" +
                            "<description type=\"text\">Success</description>\r\n" +
                            "<error_no type=\"int\">0</error_no>\r\n" +
                            "<error_message type=\"text\">None</error_message>\r\n" +
                            "<is_active type=\"bool\">true</is_active>\r\n" +
                            "<trigger_id type=\"int\">1</trigger_id>\r\n\r\n" +
                            "<start_year type=\"int\">2023</start_year>\r\n" +
                            "<start_month type=\"int\">09</start_month>\r\n" +
                            "<start_day type=\"int\">12</start_day>\r\n" +
                            "<start_hour type=\"int\">08</start_hour>\r\n" +
                            "<start_minutes type=\"int\">00</start_minutes>\r\n" +
                            "<start_second type=\"int\">0</start_second>\r\n\r\n" +
                            "<end_year type=\"int\">2100</end_year>\r\n" +
                            "<end_month type=\"int\">09</end_month>\r\n" +
                            "<end_day type=\"int\">17</end_day>\r\n" +
                            "<end_hour type=\"int\">00</end_hour>\r\n" +
                            "<end_minutes type=\"int\">30</end_minutes>\r\n" +
                            "<end_second type=\"int\">0</end_second>\r\n\r\n" +
                            "<trigger_flag type=\"int\">999</trigger_flag>\r\n\r\n" +
                            "</xml>";
                    }
                #endregion

                #region SCEAsia Christmas 2010 OBJ Trigger PUBLIC
                case "305":
                    string SCEAsiaXmasOBJTriggerPUBLICTriggerPath = SCEAsiaChristmas2010OBJTrigger + "confirmEventTrigger.xml";
                    if (File.Exists(SCEAsiaXmasOBJTriggerPUBLICTriggerPath))
                    {
                        LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - ConfirmEventTrigger sent for PUBLIC SCEAsia Christmas 2010 OBJ Trigger {eventId}!");
                        string res = File.ReadAllText(SCEAsiaXmasOBJTriggerPUBLICTriggerPath);
                        return "<xml>\r\n\t" +
                             "<result type=\"int\">1</result>\r\n\t" +
                             "<description type=\"text\">Success</description>\r\n\t" +
                             "<error_no type=\"int\">0</error_no>\r\n\t" +
                             "<error_message type=\"text\">None</error_message>\r\n\r\n\t" +
                             $"{res}\r\n" +
                             "</xml>";
                    }
                    else
                    {
                        LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - ConfirmEventTrigger FALLBACK sent for PUBLIC SCEAsia Christmas 2010 OBJ Trigger {eventId}! Expected path {SCEAsiaXmasOBJTriggerPUBLICTriggerPath}");
                        return "<xml>\r\n" +
                            "<result type=\"int\">1</result>\r\n" +
                            "<description type=\"text\">Success</description>\r\n" +
                            "<error_no type=\"int\">0</error_no>\r\n" +
                            "<error_message type=\"text\">None</error_message>\r\n" +
                            "<is_active type=\"bool\">true</is_active>\r\n" +
                            "<trigger_id type=\"int\">1</trigger_id>\r\n\r\n" +
                            "<start_year type=\"int\">2023</start_year>\r\n" +
                            "<start_month type=\"int\">09</start_month>\r\n" +
                            "<start_day type=\"int\">12</start_day>\r\n" +
                            "<start_hour type=\"int\">08</start_hour>\r\n" +
                            "<start_minutes type=\"int\">00</start_minutes>\r\n" +
                            "<start_second type=\"int\">0</start_second>\r\n\r\n" +
                            "<end_year type=\"int\">2100</end_year>\r\n" +
                            "<end_month type=\"int\">09</end_month>\r\n" +
                            "<end_day type=\"int\">17</end_day>\r\n" +
                            "<end_hour type=\"int\">00</end_hour>\r\n" +
                            "<end_minutes type=\"int\">30</end_minutes>\r\n" +
                            "<end_second type=\"int\">0</end_second>\r\n\r\n" +
                            "<trigger_flag type=\"int\">999</trigger_flag>\r\n\r\n" +
                            "</xml>";
                    }
                #endregion

                #region SCEAsia Christmas 2010 SnowFall PUBLIC
                case "306":
                    string SCEAsiaXmasSnowFallPUBLICTriggerPath = SCEAsiaChristmas2010SnowFall + "confirmEventTrigger.xml";
                    if (File.Exists(SCEAsiaXmasSnowFallPUBLICTriggerPath))
                    {
                        LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - ConfirmEventTrigger sent for PUBLIC SCEAsia Christmas 2010 SnowFall 2010 {eventId}!");
                        string res = File.ReadAllText(SCEAsiaXmasSnowFallPUBLICTriggerPath);
                        return "<xml>\r\n\t" +
                             "<result type=\"int\">1</result>\r\n\t" +
                             "<description type=\"text\">Success</description>\r\n\t" +
                             "<error_no type=\"int\">0</error_no>\r\n\t" +
                             "<error_message type=\"text\">None</error_message>\r\n\r\n\t" +
                             $"{res}\r\n" +
                             "</xml>";
                    }
                    else
                    {
                        LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - ConfirmEventTrigger FALLBACK sent for PUBLIC SCEAsia Christmas 2010 SnowFall 2010 {eventId}! Expected path {SCEAsiaXmasSnowFallPUBLICTriggerPath}");
                        return "<xml>\r\n" +
                            "<result type=\"int\">1</result>\r\n" +
                            "<description type=\"text\">Success</description>\r\n" +
                            "<error_no type=\"int\">0</error_no>\r\n" +
                            "<error_message type=\"text\">None</error_message>\r\n" +
                            "<is_active type=\"bool\">true</is_active>\r\n" +
                            "<trigger_id type=\"int\">1</trigger_id>\r\n\r\n" +
                            "<start_year type=\"int\">2023</start_year>\r\n" +
                            "<start_month type=\"int\">09</start_month>\r\n" +
                            "<start_day type=\"int\">12</start_day>\r\n" +
                            "<start_hour type=\"int\">08</start_hour>\r\n" +
                            "<start_minutes type=\"int\">00</start_minutes>\r\n" +
                            "<start_second type=\"int\">0</start_second>\r\n\r\n" +
                            "<end_year type=\"int\">2100</end_year>\r\n" +
                            "<end_month type=\"int\">09</end_month>\r\n" +
                            "<end_day type=\"int\">17</end_day>\r\n" +
                            "<end_hour type=\"int\">00</end_hour>\r\n" +
                            "<end_minutes type=\"int\">30</end_minutes>\r\n" +
                            "<end_second type=\"int\">0</end_second>\r\n\r\n" +
                            "<trigger_flag type=\"int\">999</trigger_flag>\r\n\r\n" +
                            "</xml>";
                    }
                #endregion

                #region Japan Christmas 2010 PUBLIC
                case "309":
                    string JapanXmas2010PUBLICTriggerPath = Christmas2010TriggerPath + "confirmEventTrigger.xml";
                    if (File.Exists(JapanXmas2010PUBLICTriggerPath))
                    {
                        LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - ConfirmEventTrigger sent for PUBLIC Japan Christmas 2010 {eventId}!");
                        string res = File.ReadAllText(JapanXmas2010PUBLICTriggerPath);
                        return "<xml>\r\n\t" +
                             "<result type=\"int\">1</result>\r\n\t" +
                             "<description type=\"text\">Success</description>\r\n\t" +
                             "<error_no type=\"int\">0</error_no>\r\n\t" +
                             "<error_message type=\"text\">None</error_message>\r\n\r\n\t" +
                             $"{res}\r\n" +
                             "</xml>";
                    }
                    else
                    {
                        LoggerAccessor.LogInfo($"[PREMIUMAGENCY] - ConfirmEventTrigger FALLBACK sent for PUBLIC Japan Christmas 2010  {eventId}! Expected path {JapanXmas2010PUBLICTriggerPath}");
                        return "<xml>\r\n" +
                            "<result type=\"int\">1</result>\r\n" +
                            "<description type=\"text\">Success</description>\r\n" +
                            "<error_no type=\"int\">0</error_no>\r\n" +
                            "<error_message type=\"text\">None</error_message>\r\n" +
                            "<is_active type=\"bool\">true</is_active>\r\n" +
                            "<trigger_id type=\"int\">1</trigger_id>\r\n\r\n" +
                            "<start_year type=\"int\">2023</start_year>\r\n" +
                            "<start_month type=\"int\">09</start_month>\r\n" +
                            "<start_day type=\"int\">12</start_day>\r\n" +
                            "<start_hour type=\"int\">08</start_hour>\r\n" +
                            "<start_minutes type=\"int\">00</start_minutes>\r\n" +
                            "<start_second type=\"int\">0</start_second>\r\n\r\n" +
                            "<end_year type=\"int\">2100</end_year>\r\n" +
                            "<end_month type=\"int\">09</end_month>\r\n" +
                            "<end_day type=\"int\">17</end_day>\r\n" +
                            "<end_hour type=\"int\">00</end_hour>\r\n" +
                            "<end_minutes type=\"int\">30</end_minutes>\r\n" +
                            "<end_second type=\"int\">0</end_second>\r\n\r\n" +
                            "<trigger_flag type=\"int\">999</trigger_flag>\r\n\r\n" +
                            "</xml>";
                    }
                #endregion


                default:
                    {
                        LoggerAccessor.LogError($"[PREMIUMAGENCY] - ConfirmEventTrigger unhandled with eventId {eventId} contact the developers! \nPOSTDATA: \n{Encoding.UTF8.GetString(PostData)}");
                        return null;
                    }
            }

        }

        #endregion
    }
}
