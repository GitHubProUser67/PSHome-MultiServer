using HttpMultipartParser;
using System.IO;

namespace WebAPIService.GameServices.PSHOME.HELLFIRE.Helpers.Tycoon
{
    internal class PostCards
    {
        public static string HandleUpload(byte[] PostData, string boundary, string UserID, string WorkPath)
        {
            const string screenShotFileName = "screenshot.jpg";

            byte[] jpgBuffer = null;
            string TownID = string.Empty;

            if (PostData != null && !string.IsNullOrEmpty(boundary))
            {
                using (MemoryStream ms = new MemoryStream(PostData))
                {
                    var data = MultipartFormDataParser.Parse(ms, boundary);
                    try
                    {
                        TownID = data.GetParameterValue("TownID");
                    }
                    catch
                    {
                        // Not a town picture.
                    }
                    foreach (var file in data.Files)
                    {
                        using (Stream filedata = file.Data)
                        {
                            filedata.Position = 0;

                            // Find the number of bytes in the stream
                            int contentLength = (int)filedata.Length;

                            // Create a byte array
                            byte[] buffer = new byte[contentLength];

                            // Read the contents of the memory stream into the byte array
                            filedata.Read(buffer, 0, contentLength);

                            if (file.FileName == screenShotFileName)
                                jpgBuffer = buffer;

                            filedata.Flush();
                        }
                    }
                    ms.Flush();
                }

                if (IsValidJpeg(jpgBuffer))
                {
                    if (!string.IsNullOrEmpty(TownID))
                    {
                        string townsDirPath = $"{WorkPath}/HomeTycoon/TownsData/{UserID}";

                        Directory.CreateDirectory(townsDirPath);

                        _ = File.WriteAllBytesAsync(townsDirPath + $"/{TownID}{screenShotFileName.Substring(screenShotFileName.Length - 4)}", jpgBuffer);
                    }
                    else
                    {
                        string townsDirPath = $"{WorkPath}/HomeTycoon/User_Data/{UserID}";

                        Directory.CreateDirectory(townsDirPath);

                        _ = File.WriteAllBytesAsync(townsDirPath + $"/{screenShotFileName}", jpgBuffer);
                    }
                }
            }

            return "<Response></Response>";
        }

        private static bool IsValidJpeg(byte[] data)
        {
            // JPEG magic numbers
            byte[] jpegHeader = { 0xFF, 0xD8 };
            byte[] jpegFooter = { 0xFF, 0xD9 };

            if (data == null || data.Length < 4)
                return false;

            return data[0] == jpegHeader[0] &&
                   data[1] == jpegHeader[1] &&
                   data[data.Length - 2] == jpegFooter[0] &&
                   data[data.Length - 1] == jpegFooter[1];
        }

    }
}
