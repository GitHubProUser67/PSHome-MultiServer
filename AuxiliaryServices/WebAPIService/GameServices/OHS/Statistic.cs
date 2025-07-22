using System;
using MultiServerLibrary.HTTP;
using CustomLogger;
using HttpMultipartParser;
using System.IO;

namespace WebAPIService.GameServices.OHS
{
    public class Statistic
    {
        public static string Set(byte[] PostData, string ContentType)
        {
            string dataforohs = null;

            string boundary = HTTPProcessor.ExtractBoundary(ContentType);

            if (!string.IsNullOrEmpty(boundary))
            {
                using (MemoryStream ms = new MemoryStream(PostData))
                {
                    var data = MultipartFormDataParser.Parse(ms, boundary);
                    try
                    {
                        LoggerAccessor.LogInfo($"[OHS] : Client Version - {data.GetParameterValue("version")}");
                    }
                    catch (Exception)
                    {
                        // Not Important.
                    }
                    try
                    {
                        dataforohs = JaminProcessor.JaminDeFormat(data.GetParameterValue("data"), true, 0);
                    }
                    catch (Exception ex)
                    {
                        LoggerAccessor.LogWarn($"[OHS] : Client issued Statistics with an unknown body format, report this to GITHUB: {ex}");
                    }
                    ms.Flush();
                }
            }

            if (!string.IsNullOrEmpty(dataforohs))
            {
                LoggerAccessor.LogInfo($"[OHS] : Client issued Statistics - {dataforohs}");
                return JaminProcessor.JaminFormat("{ [\"status\"] = \"success\" }", 0);
            }
            else
                return JaminProcessor.JaminFormat("{ [\"status\"] = \"fail\" }", 0);
        }


        public static string HeatmapTracker(byte[] PostData, string ContentType)
        {
            string dataforohs = null;

            string boundary = HTTPProcessor.ExtractBoundary(ContentType);

            if (!string.IsNullOrEmpty(boundary))
            {
                using (MemoryStream ms = new MemoryStream(PostData))
                {
                    var data = MultipartFormDataParser.Parse(ms, boundary);
                    try
                    {
                        LoggerAccessor.LogInfo($"[OHS] : Client Version - {data.GetParameterValue("version")}");
                    }
                    catch (Exception)
                    {
                        // Not Important.
                    }

                    try
                    {
#if DEBUG
                        dataforohs = JaminProcessor.JaminDeFormat(data.GetParameterValue("data"), true, 0);
                        LoggerAccessor.LogInfo($"[OHS] Heatmap Tracker Data : {dataforohs}");
#endif
                    }
                    catch (Exception ex)
                    {
                        LoggerAccessor.LogWarn($"[OHS] : Client issued Heatmap Tracker with an unknown body format, report this to GITHUB: {ex}");
                    }

                    ms.Flush();
                }
            }

            if (!string.IsNullOrEmpty(dataforohs))
            {
                LoggerAccessor.LogInfo($"[OHS] : Client issued Heatmap Tracker - {dataforohs}");
                return JaminProcessor.JaminFormat("{ [\"status\"] = \"success\" }", 0);
            }
            else
                return JaminProcessor.JaminFormat("{ [\"status\"] = \"fail\" }", 0);
        }

        public static string PointsTracker(byte[] PostData, string ContentType)
        {
            string dataforohs = null;

            string boundary = HTTPProcessor.ExtractBoundary(ContentType);

            if (!string.IsNullOrEmpty(boundary))
            {
                using (MemoryStream ms = new MemoryStream(PostData))
                {
                    var data = MultipartFormDataParser.Parse(ms, boundary);
                    try
                    {
                        LoggerAccessor.LogInfo($"[OHS] : Client Version - {data.GetParameterValue("version")}");
                    }
                    catch (Exception)
                    {
                        // Not Important.
                    }

                    try
                    {
#if DEBUG
                        dataforohs = JaminProcessor.JaminDeFormat(data.GetParameterValue("data"), true, 0);
                        LoggerAccessor.LogInfo($"[OHS] Points Tracker Data : {dataforohs}");
#endif
                    }
                    catch (Exception ex)
                    {
                        LoggerAccessor.LogWarn($"[OHS] : Client issued Points Tracker with an unknown body format, report this to GITHUB: {ex}");
                    }

                    ms.Flush();
                }
            }

            if (!string.IsNullOrEmpty(dataforohs))
            {
                LoggerAccessor.LogInfo($"[OHS] : Client issued Points Tracker - {dataforohs}");
                return JaminProcessor.JaminFormat("{ [\"status\"] = \"success\" }", 0);
            }
            else
                return JaminProcessor.JaminFormat("{ [\"status\"] = \"fail\" }", 0);
        }

    }
}