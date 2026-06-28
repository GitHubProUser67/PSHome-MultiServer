using System.Xml;
using CustomLogger;
using HttpMultipartParser;
using MultiServerLibrary.HTTP;

namespace WebAPIService.GameServices.PSHOME.OUWF
{
    public class OuWFList
    {
        public static string List(byte[] PostData, string ContentType)
        {
            var boundary = HTTPProcessor.ExtractBoundary(ContentType);

            using (var ms = new MemoryStream(PostData))
            {
                var data = MultipartFormDataParser.Parse(ms, boundary);

                var instanceId = Convert.ToInt32(data.GetParameterValue("instanceId"));
                var vers = data.GetParameterValue("version");
                var path = data.GetParameterValue("path");

                LoggerAccessor.LogInfo(
                    $"[OuWF] - Requested List with instanceId {instanceId} | version {vers} | path {path}"
                );

                var xml = GenerateXml(path);
                ms.Flush();

                return xml;
            }
        }

        static string GenerateXml(string directoryPath)
        {
            //string xml = "";

            var dirs = Directory.GetDirectories(directoryPath);
            var files = Directory.GetFiles(directoryPath);

            var xmlDoc = new XmlDocument();
            var root = xmlDoc.CreateElement("list");

            var dirsElement = xmlDoc.CreateElement("dirs");
            foreach (var dir in dirs)
            {
                var dirElement = xmlDoc.CreateElement("dir");

                //string HDKBuildRoot = "H:/HDK186/Build/";

                //string editDir = GetRelativePath(HDKBuildRoot, dir);

                //string backSlashEdit;

                /*
                if(editDir.Contains("\\"))
                {
                    backSlashEdit = editDir.Replace("\\", "/");

                    string pathTrim = GetRelativePath(directoryPath, backSlashEdit);
                    dirElement.InnerText = pathTrim;
                } else {
                    string pathEditTrim = editDir.Replace(directoryPath, "");
                    dirElement.InnerText = pathEditTrim;
                }
                */

                //string pathEditTrim = editDir.Replace(directoryPath, "");

                var partialPath = SplitPath(dir, directoryPath);

                dirElement.InnerText = partialPath;

                dirsElement.AppendChild(dirElement);
            }

            var filesElement = xmlDoc.CreateElement("files");
            if (files.Length == 0)
            {
                foreach (var file in files)
                {
                    var fileInfo = new FileInfo(file);

                    var fileElement = xmlDoc.CreateElement("file");

                    var filePathTrimmed = fileInfo.Name.Replace(directoryPath, "");

                    fileElement.SetAttribute("value", filePathTrimmed);

                    fileElement.SetAttribute("size", fileInfo.Length.ToString());
                    fileElement.SetAttribute("read_only", fileInfo.IsReadOnly.ToString());
                    filesElement.AppendChild(fileElement);
                }

                root.AppendChild(filesElement);
            }

            root.AppendChild(dirsElement);
            xmlDoc.AppendChild(root);

            //xmlDoc.Save(xml);
            LoggerAccessor.LogInfo($"[OuWF] - List Response: {xmlDoc.OuterXml}");
            return xmlDoc.OuterXml;
        }

        static string GetRelativePath(string rootPath, string fullPath)
        {
            if (Path.GetPathRoot(rootPath) != Path.GetPathRoot(fullPath))
            {
                throw new InvalidOperationException("Paths must have the same root.");
            }

            var rootUri = new Uri(rootPath);
            var fullUri = new Uri(fullPath);

            return Uri.UnescapeDataString(
                rootUri
                    .MakeRelativeUri(fullUri)
                    .ToString()
                    .Replace('/', Path.DirectorySeparatorChar)
            );
        }

        static string SplitPath(string directory, string baseDirectory)
        {
            // Split the directory path into components
            var directoryComponents = directory.Split(Path.DirectorySeparatorChar);
            var baseDirectoryComponents = baseDirectory.Split(Path.DirectorySeparatorChar);

            // Find the common base directory components
            var commonLength = Math.Min(directoryComponents.Length, baseDirectoryComponents.Length);
            for (var i = 0; i < commonLength; i++)
            {
                if (directoryComponents[i] != baseDirectoryComponents[i])
                {
                    commonLength = i;
                    break;
                }
            }

            // Concatenate the remaining directory components
            var partialPathComponents = new string[directoryComponents.Length - commonLength];
            Array.Copy(
                directoryComponents,
                commonLength,
                partialPathComponents,
                0,
                partialPathComponents.Length
            );

            return string.Join(Path.DirectorySeparatorChar.ToString(), partialPathComponents);
        }
    }
}
