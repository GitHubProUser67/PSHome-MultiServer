using System;
using System.Collections.Generic;
using System.IO;
using MultiServerLibrary.Extension;
using System.Threading.Tasks;
using System.Text;
using System.Web;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MultiServerLibrary.HTTP
{
    public static class FileStructureFormater
    {
        private static int ProcessorCount = Environment.ProcessorCount;
        private static bool IsSingleProcessor => ProcessorCount == 1;

        private static int ProcessorCountLeft = IsSingleProcessor ? ProcessorCount : (int)Math.Floor(ProcessorCount / 2.0);
        private static int ProcessorCountRight = IsSingleProcessor ? ProcessorCount : ProcessorCount - ProcessorCountLeft;

        public static async Task<string> GetFileStructureAsync(string rootDirectory, string rootDirectoryUrl, int ServerPort, bool html, bool allowNestedReports, bool detailedReport, Dictionary<string, string> mimeTypesDic)
        {
            FileNode node = null;

            try
            {
                node = await CreateFileNodeAsync(rootDirectory, rootDirectoryUrl, allowNestedReports, detailedReport, mimeTypesDic).ConfigureAwait(false);
                FileStructure structure = new FileStructure() { Root = node };

                if (html)
                    return CreateFileNodeHtmlString(structure, Path.GetFileName(rootDirectory), ServerPort, mimeTypesDic);
                else
                    return JsonSerializer.Serialize(structure,
                      new JsonSerializerOptions
                      {
                          WriteIndented = true,
                          DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                      });
            }
            catch (Exception ex)
            {
                CustomLogger.LoggerAccessor.LogError($"[FileStructureFormater] - GetFileStructureAsJson - Thrown an exception: {ex}");
            }
            finally
            {
                if (node != null)
                {
                    node.Childrens.Clear();
                    node.Childrens = null;
                }
            }

            return null;
        }

        private static async Task<FileNode> CreateFileNodeAsync(string directoryPath, string httpdirectoryrequest, bool allowNestedReports, bool detailedReport, Dictionary<string, string> mimeTypesDic)
        {
            try
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(directoryPath);
                FileNode fileNode;

                if (detailedReport)
                    fileNode = new FileNode()
                    {
                        Name = directoryInfo.Name,
                        Link = httpdirectoryrequest,
                        Type = "Directory",
                        CreationDate = directoryInfo.CreationTimeUtc,
                        Size = directoryInfo.GetLength(),
                        LastWriteTime = directoryInfo.LastWriteTimeUtc,
                        Childrens = new ConcurrentList<FileNode>()
                    };
                else
                    fileNode = new FileNode()
                    {
                        Name = directoryInfo.Name,
                        Link = httpdirectoryrequest,
                        Type = "Directory",
                        CreationDate = null,
                        Size = null,
                        LastWriteTime = directoryInfo.LastWriteTimeUtc,
                        Childrens = new ConcurrentList<FileNode>()
                    };
#if NET6_0_OR_GREATER
                await Task.WhenAll(
                    Parallel.ForEachAsync(directoryInfo.GetFiles(), new ParallelOptions { MaxDegreeOfParallelism = ProcessorCountLeft }, async (file, cancellationToken) =>
                    {
                        if (!file.IsHidden())
                        {
                            string ImgLink = null;
                            string DescriptorText = null;
                            string mimetype = HTTPProcessor.GetMimeType(Path.GetExtension(file.FullName), mimeTypesDic);

                            // List of possible web image extensions
                            string[] imageExtensions = new string[] { "jpeg", "jpg", "png", "gif", "bmp", "tiff" };

                            foreach (string extension in imageExtensions)
                            {
                                if (File.Exists(Path.Combine(Path.GetDirectoryName(file.FullName), $"{Path.GetFileNameWithoutExtension(file.FullName)}_pic.{extension}")))
                                {
                                    ImgLink = $"{httpdirectoryrequest}/{Path.GetFileNameWithoutExtension(file.FullName)}_pic.{extension}";
                                    break;
                                }
                            }

                            if (File.Exists(Path.Combine(Path.GetDirectoryName(file.FullName), $"{Path.GetFileNameWithoutExtension(file.FullName)}_description.txt")))
                                DescriptorText = await File.ReadAllTextAsync(
                                    Path.Combine(Path.GetDirectoryName(file.FullName),
                                        $"{Path.GetFileNameWithoutExtension(file.FullName)}_description.txt"), cancellationToken).ConfigureAwait(false);
                            else if (File.Exists(Path.Combine(Path.GetDirectoryName(file.FullName), $"{Path.GetFileNameWithoutExtension(file.FullName)}_description.EdgeZlib")))
                                DescriptorText = Encoding.UTF8.GetString(
                                    SonyEdge.Zlib.EdgeZlibDecompress(
                                        await File.ReadAllBytesAsync(
                                            Path.Combine(Path.GetDirectoryName(file.FullName),
                                                $"{Path.GetFileNameWithoutExtension(file.FullName)}_description.EdgeZlib"), cancellationToken).ConfigureAwait(false)));
                            else if (File.Exists(Path.Combine(Path.GetDirectoryName(file.FullName), $"{Path.GetFileNameWithoutExtension(file.FullName)}_desc.txt")))
                                DescriptorText = await File.ReadAllTextAsync(
                                    Path.Combine(Path.GetDirectoryName(file.FullName),
                                        $"{Path.GetFileNameWithoutExtension(file.FullName)}_desc.txt"), cancellationToken).ConfigureAwait(false);
                            else if (File.Exists(Path.Combine(Path.GetDirectoryName(file.FullName), $"{Path.GetFileNameWithoutExtension(file.FullName)}_desc.EdgeZlib")))
                                DescriptorText = Encoding.UTF8.GetString(
                                    SonyEdge.Zlib.EdgeZlibDecompress(
                                        await File.ReadAllBytesAsync(
                                            Path.Combine(Path.GetDirectoryName(file.FullName),
                                                $"{Path.GetFileNameWithoutExtension(file.FullName)}_desc.EdgeZlib"), cancellationToken).ConfigureAwait(false)));

                            switch (mimetype)
                            {
                                case "text/plain":
                                case "text/xml":
                                case "application/xml":
                                case "application/json":
                                    bool isUtf8 = false;
                                    try
                                    {
                                        isUtf8 = new Utf8Checker().Check(file.FullName);
                                    }
                                    catch
                                    {
                                    }
                                    fileNode.Childrens.Add(new FileNode
                                    {
                                        Link = $"{httpdirectoryrequest}/{file.Name}",
                                        Content = isUtf8 ? await File.ReadAllTextAsync(file.FullName, cancellationToken).ConfigureAwait(false) : null,
                                        Image = ImgLink,
                                        Descriptor = DescriptorText,
                                        Name = file.Name,
                                        Type = mimetype,
                                        Size = detailedReport ? file.Length : null,
                                        LastWriteTime = file.LastWriteTime,
                                        CreationDate = detailedReport ? file.CreationTimeUtc : null
                                    });
                                    break;
                                default:
                                    fileNode.Childrens.Add(new FileNode
                                    {
                                        Link = $"{httpdirectoryrequest}/{file.Name}",
                                        Image = ImgLink,
                                        Descriptor = DescriptorText,
                                        Name = file.Name,
                                        Type = mimetype,
                                        Size = detailedReport ? file.Length : null,
                                        LastWriteTime = file.LastWriteTime,
                                        CreationDate = detailedReport ? file.CreationTimeUtc : null
                                    });
                                    break;
                            }
                        }
                    }),
                    Parallel.ForEachAsync(directoryInfo.GetDirectories(), new ParallelOptions { MaxDegreeOfParallelism = ProcessorCountRight }, async (subdirectory, cancellationToken) =>
                    {
                        if (!subdirectory.IsHidden())
                        {
                            if (allowNestedReports)
                            {
                                // Recursively process subdirectories
                                fileNode.Childrens.Add(await CreateFileNodeAsync(
                                    subdirectory.FullName,
                                    $"{httpdirectoryrequest}/{subdirectory.Name}",
                                    true,
                                    detailedReport,
                                    mimeTypesDic
                                ).ConfigureAwait(false));
                            }
                            else
                            {
                                fileNode.Childrens.Add(new FileNode
                                {
                                    Link = httpdirectoryrequest + $"/{subdirectory.Name}",
                                    Name = subdirectory.Name,
                                    Type = "Directory",
                                    CreationDate = detailedReport ? subdirectory.CreationTimeUtc : null,
                                    Size = detailedReport ? directoryInfo.GetLength() : null,
                                    LastWriteTime = directoryInfo.LastWriteTimeUtc
                                });
                            }
                        }
                    })
                ).ConfigureAwait(false);
#else
                Parallel.ForEach(directoryInfo.GetFiles(), new ParallelOptions { MaxDegreeOfParallelism = ProcessorCountLeft }, async (file, cancellationToken) =>
                {
                    if (!file.IsHidden())
                    {
                        string ImgLink = null;
                        string DescriptorText = null;
                        string mimetype = HTTPProcessor.GetMimeType(Path.GetExtension(file.FullName), mimeTypesDic);

                        // List of possible web image extensions
                        string[] imageExtensions = new string[] { "jpeg", "jpg", "png", "gif", "bmp", "tiff" };

                        foreach (string extension in imageExtensions)
                        {
                            if (File.Exists(Path.Combine(Path.GetDirectoryName(file.FullName), $"{Path.GetFileNameWithoutExtension(file.FullName)}_pic.{extension}")))
                            {
                                ImgLink = $"{httpdirectoryrequest}/{Path.GetFileNameWithoutExtension(file.FullName)}_pic.{extension}";
                                break;
                            }
                        }

                        if (File.Exists(Path.Combine(Path.GetDirectoryName(file.FullName), $"{Path.GetFileNameWithoutExtension(file.FullName)}_description.txt")))
                            DescriptorText = File.ReadAllText(
                                Path.Combine(Path.GetDirectoryName(file.FullName),
                                    $"{Path.GetFileNameWithoutExtension(file.FullName)}_description.txt"));
                        else if (File.Exists(Path.Combine(Path.GetDirectoryName(file.FullName), $"{Path.GetFileNameWithoutExtension(file.FullName)}_description.EdgeZlib")))
                            DescriptorText = Encoding.UTF8.GetString(
                                await CompressionLibrary.Edge.Zlib.EdgeZlibDecompress(
                                    File.ReadAllBytes(
                                        Path.Combine(Path.GetDirectoryName(file.FullName),
                                            $"{Path.GetFileNameWithoutExtension(file.FullName)}_description.EdgeZlib"))).ConfigureAwait(false));
                        else if (File.Exists(Path.Combine(Path.GetDirectoryName(file.FullName), $"{Path.GetFileNameWithoutExtension(file.FullName)}_desc.txt")))
                            DescriptorText = File.ReadAllText(
                                Path.Combine(Path.GetDirectoryName(file.FullName),
                                    $"{Path.GetFileNameWithoutExtension(file.FullName)}_desc.txt"));
                        else if (File.Exists(Path.Combine(Path.GetDirectoryName(file.FullName), $"{Path.GetFileNameWithoutExtension(file.FullName)}_desc.EdgeZlib")))
                            DescriptorText = Encoding.UTF8.GetString(
                                await CompressionLibrary.Edge.Zlib.EdgeZlibDecompress(
                                    File.ReadAllBytes(
                                        Path.Combine(Path.GetDirectoryName(file.FullName),
                                            $"{Path.GetFileNameWithoutExtension(file.FullName)}_desc.EdgeZlib"))).ConfigureAwait(false));

                        switch (mimetype)
                        {
                            case "text/plain":
                            case "text/xml":
                            case "application/xml":
                            case "application/json":
                                bool isUtf8 = false;
                                try
                                {
                                    isUtf8 = new Utf8Checker().Check(file.FullName);
                                }
                                catch
                                {
                                }
                                if (detailedReport)
                                    fileNode.Childrens.Add(new FileNode
                                    {
                                        Link = $"{httpdirectoryrequest}/{file.Name}",
                                        Content = isUtf8 ? File.ReadAllText(file.FullName) : null,
                                        Image = ImgLink,
                                        Descriptor = DescriptorText,
                                        Name = file.Name,
                                        Type = mimetype,
                                        Size = file.Length,
                                        LastWriteTime = file.LastWriteTime,
                                        CreationDate = file.CreationTimeUtc
                                    });
                                else
                                    fileNode.Childrens.Add(new FileNode
                                    {
                                        Link = $"{httpdirectoryrequest}/{file.Name}",
                                        Content = isUtf8 ? File.ReadAllText(file.FullName) : null,
                                        Image = ImgLink,
                                        Descriptor = DescriptorText,
                                        Name = file.Name,
                                        Type = mimetype,
                                        Size = null,
                                        LastWriteTime = file.LastWriteTime,
                                        CreationDate = null
                                    });
                                break;
                            default:
                                if (detailedReport)
                                    fileNode.Childrens.Add(new FileNode
                                    {
                                        Link = $"{httpdirectoryrequest}/{file.Name}",
                                        Image = ImgLink,
                                        Descriptor = DescriptorText,
                                        Name = file.Name,
                                        Type = mimetype,
                                        Size = file.Length,
                                        LastWriteTime = file.LastWriteTime,
                                        CreationDate = file.CreationTimeUtc
                                    });
                                else
                                    fileNode.Childrens.Add(new FileNode
                                    {
                                        Link = $"{httpdirectoryrequest}/{file.Name}",
                                        Image = ImgLink,
                                        Descriptor = DescriptorText,
                                        Name = file.Name,
                                        Type = mimetype,
                                        Size = null,
                                        LastWriteTime = file.LastWriteTime,
                                        CreationDate = null
                                    });
                                break;
                        }
                    }
                });
                Parallel.ForEach(directoryInfo.GetDirectories(), new ParallelOptions { MaxDegreeOfParallelism = ProcessorCountRight }, async (subdirectory, cancellationToken) =>
                {
                    if (!subdirectory.IsHidden())
                    {
                        if (allowNestedReports)
                        {
                            // Recursively process subdirectories
                            fileNode.Childrens.Add(await CreateFileNodeAsync(
                                subdirectory.FullName,
                                $"{httpdirectoryrequest}/{subdirectory.Name}",
                                true,
                                detailedReport,
                                mimeTypesDic
                            ).ConfigureAwait(false));
                        }
                        else if (detailedReport)
                        {
                            fileNode.Childrens.Add(new FileNode
                            {
                                Link = httpdirectoryrequest + $"/{subdirectory.Name}",
                                Name = subdirectory.Name,
                                Type = "Directory",
                                CreationDate = subdirectory.CreationTimeUtc,
                                Size = directoryInfo.GetLength(),
                                LastWriteTime = directoryInfo.LastWriteTimeUtc
                            });
                        }
                        else
                        {
                            fileNode.Childrens.Add(new FileNode
                            {
                                Link = httpdirectoryrequest + $"/{subdirectory.Name}",
                                Name = subdirectory.Name,
                                Type = "Directory",
                                CreationDate = null,
                                Size = null,
                                LastWriteTime = directoryInfo.LastWriteTimeUtc
                            });
                        }
                    }
                });
#endif
                return fileNode;
            }
            catch (Exception ex)
            {
                CustomLogger.LoggerAccessor.LogError($"[FileStructureFormater] - CreateFileNode - Thrown an exception: {ex}");
            }

            return null;
        }

        public static string CreateFileNodeHtmlString(FileStructure structure, string title, int ServerPort, Dictionary<string, string> mimeTypesDic)
        {
            const string htmlStartData = @"
                <style>
                    body {
                        font-family: 'Arial', sans-serif;
                        background-color: #f4f4f4;
                        margin: 0;
                        padding: 0;
                        color: #333;
                    }
                    h1 {
                        background-color: #007BFF;
                        color: white;
                        padding: 20px;
                        margin: 0;
                        text-align: center;
                    }
                    .header {
                        background-color: #007BFF;
                        color: white;
                        padding: 20px;
                        margin: 0;
                        text-align: center;
                        display: flex;
                        align-items: center;
                        justify-content: center;
                        position: relative;
                    }
                    .back-button {
                        position: absolute;
                        left: 20px;
                        background: white;
                        border: none;
                        font-size: 16px;
                        cursor: pointer;
                        padding: 10px;
                        border-radius: 5px;
                    }
                    .file-node {
                        background-color: white;
                        border-radius: 8px;
                        margin: 10px 0;
                        padding: 15px;
                        box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
                        transition: transform 0.3s ease, box-shadow 0.3s ease;
                    }
                    .file-node:hover {
                        transform: translateY(-5px);
                        box-shadow: 0 8px 12px rgba(0, 0, 0, 0.2);
                    }
                    .file-node b {
                        color: #007BFF;
                        font-size: 1.1em;
                    }
                    .file-node i {
                        color: #666;
                        font-size: 0.9em;
                    }
                    .file-node p {
                        font-size: 0.95em;
                        color: #444;
                    }
                    .file-node ul {
                        padding-left: 20px;
                    }
                    .file-node ul li {
                        margin: 8px 0;
                    }
                    .file-node .file-size {
                        color: #28a745;
                        font-weight: bold;
                    }
                    .file-node .level-0 {
                        background-color: #e9f7fe;
                    }
                    .file-node .level-1 {
                        background-color: #f9f9f9;
                    }
                    .file-node .level-2 {
                        background-color: #ffffff;
                    }
                    .file-node .level-3 {
                        background-color: #f1f8e9;
                    }
                    .file-content {
                        background-color: #f9f9f9;
                        padding: 10px;
                        border-radius: 8px;
                        margin-top: 10px;
                        font-family: 'Courier New', Courier, monospace;
                        font-size: 0.95em;
                        white-space: pre-wrap;
                        word-wrap: break-word;
                        max-height: 300px;
                        overflow-y: auto;
                        line-height: 1.5;
                    }
                    .file-container {
                        max-width: 100%;
                        overflow: hidden;
                        display: flex;
                        justify-content: center;
                        align-items: center;
                        margin: 10px 0;
                    }
                    .file-image {
                        max-width: 100%;
                        height: auto;
                        margin-top: 10px;
                        border-radius: 8px;
                    }
                    .file-video, .file-audio {
                        max-width: 100%;
                        max-height: 400px; /* Prevent oversized videos */
                        border-radius: 8px;
                    }
                    .pdf-container {
                        max-width: 100%;
                        overflow: auto;
                        border: 1px solid #ddd;
                        border-radius: 8px;
                    }
                    .file-pdf {
                        width: 100%;
                        height: 600px;
                    }
                    .hidden {
                        display: none;
                    }
                </style>
                <script>
                    function goBack() {
                        window.history.back();
                    }
                    function toggleFolder(id) {
                        var element = document.getElementById(id);
                        if (element) {
                            element.classList.toggle('hidden');
                        }
                    }
                </script>
                </head><body>";

            StringBuilder sb = new StringBuilder($"<html><head><div class='header'><button class='back-button' onclick='goBack()'>&larr; Back</button><h1>{htmlStartData}{HttpUtility.HtmlEncode(title)}</h1></div>");
            sb.Append(GenerateFileNodeHtml(structure?.Root, 0, ServerPort, mimeTypesDic));
            sb.Append("</body></html>");
            return sb.ToString();
        }

        private static string GenerateFileNodeHtml(FileNode node, int level, int ServerPort, Dictionary<string, string> mimeTypesDic)
        {
            if (node == null)
                return "<h3>FileNode was null!</h3>";

            // Apply a unique class for each level to distinguish the appearance
            string levelClass;
            switch (level)
            {
                case 0:
                    levelClass = "level-0";
                    break;
                case 1:
                    levelClass = "level-1";
                    break;
                case 2:
                    levelClass = "level-2";
                    break;
                default:
                    levelClass = "level-3";
                    break;
            }

            bool isRoot = level == 0;
            bool hasChildrens = node.Childrens != null && node.Childrens.Count > 0;
            string nodeId = Guid.NewGuid().ToString();

            // Create the basic structure for the current node
            StringBuilder sb = new StringBuilder($"<div class='file-node {levelClass}'>");

            if (!isRoot && hasChildrens && node.Type == "Directory")
                sb.AppendLine($"<button class='toggle-btn' onclick=\"toggleFolder('{nodeId}')\">[+]</button>");

            // Make the file/directory name clickable by wrapping it with an <a> tag
            sb.AppendLine($"<b><a href='{(node.Type == "Directory" ? HttpUtility.HtmlEncode(node.Link + "?directory=on") : HttpUtility.HtmlEncode(node.Link))}' style='color: #007BFF; text-decoration: none;'>{HttpUtility.HtmlEncode(node.Name)}</a></b> - <span style='color: #007BFF;'>{node.Type}</span>");
            if (node.CreationDate != null)
                sb.AppendLine($"<i>Created: {node.CreationDate}</i><br>");
            sb.AppendLine($"<i>Last modified: {node.LastWriteTime}</i><br>");

            if (!string.IsNullOrEmpty(node.Image))
                sb.AppendLine($"<img src='{HttpUtility.HtmlEncode(node.Image)}' class='file-image' alt='{HttpUtility.HtmlEncode(Path.GetFileName(node.Image))}'>");
            if (!string.IsNullOrEmpty(node.Descriptor))
                sb.AppendLine($"<p>{HttpUtility.HtmlEncode(node.Descriptor)}</p>");

            if (node.Size.HasValue)
            {
                double size = node.Size.Value;
                string formattedSize;

                if (size >= 1_073_741_824) // 1 GB
                    formattedSize = $"{size / 1_073_741_824:0.##} GB";
                else if (size >= 1_048_576) // 1 MB
                    formattedSize = $"{size / 1_048_576:0.##} MB";
                else if (size >= 1024) // 1 KB
                    formattedSize = $"{size / 1024:0.##} KB";
                else
                    formattedSize = $"{size} bytes";

                sb.AppendLine($"<p class='file-size'>Size: {formattedSize}</p>");
            }

            if (!string.IsNullOrEmpty(node.Content))
            {
                sb.AppendLine("<div class='file-content'>");
                sb.AppendLine($"<pre>{HttpUtility.HtmlEncode(node.Content)}</pre>");
                sb.AppendLine("</div>");
            }
            else if (node.Type.StartsWith("image/"))
                sb.AppendLine($"<img src='{HttpUtility.HtmlEncode(node.Link)}' class='file-image' alt='{HttpUtility.HtmlEncode(node.Name)}'>");
            else if (node.Type.StartsWith("video/"))
            {
                string fileFormat = HTTPProcessor.GetExtensionFromMime(node.Type, mimeTypesDic).Substring(1).ToLower();
                bool isWebVideoFriendly = fileFormat == "mp4" || fileFormat == "webm";
                sb.AppendLine("<div class='file-container'>");
                if (isWebVideoFriendly)
                {
                    sb.AppendLine("<video controls class='file-video'>");
                    sb.AppendLine($"<source src='{HttpUtility.HtmlEncode(node.Link)}' type='{node.Type}'>");
                    sb.AppendLine("Your browser does not support the video tag.");
                    sb.AppendLine("</video>");
                }
                else
                {
                    string mediaInfoResult = HTTPProcessor.RequestURLGET($"http://localhost:{ServerPort}/media/info?item={Uri.EscapeDataString(new Uri(node.Link).AbsolutePath)}");
                    if (!string.IsNullOrEmpty(mediaInfoResult))
                    {
                        double? vBitrate = PlayerData.GetVBitRate(mediaInfoResult);
                        double? vFrameRate = PlayerData.GetVFrameRate(mediaInfoResult);
                        sb.AppendLine("<p style='color: red;'>This video format is not supported for direct streaming. To watch it using your browser, Please use this link instead :</p>");
                        sb.AppendLine($"<a href='{HttpUtility.HtmlEncode(node.Link)}?offset=&vbitrate={(vBitrate == null ? "NaN" : vBitrate.ToString().Replace(",", "."))}&vtranscode=true&vframerate={(vFrameRate == null ? string.Empty : vFrameRate.ToString().Replace(",", "."))}&format={fileFormat}' download>Download Transcoded Video</a>");
                    }
                }
                sb.AppendLine("</div>");
            }
            else if (node.Type.StartsWith("audio/"))
            {
                sb.AppendLine("<div class='file-container'>");
                sb.AppendLine("<audio controls class='file-audio'>");
                sb.AppendLine($"<source src='{HttpUtility.HtmlEncode(node.Link)}' type='{node.Type}'>");
                sb.AppendLine("Your browser does not support the audio tag.");
                sb.AppendLine("</audio>");
                sb.AppendLine("</div>");
            }
            else if (node.Type == "application/pdf")
            {
                sb.AppendLine("<div class='file-container pdf-container'>");
                sb.AppendLine($"<iframe src='{HttpUtility.HtmlEncode(node.Link)}' class='file-pdf'></iframe>");
                sb.AppendLine("</div>");
            }

            if (hasChildrens)
            {
                sb.AppendLine($"<ul id='{nodeId}' class='{(isRoot ? "" : "hidden")}' > ");
                foreach (var child in node.Childrens.OrderBy(x => x.Name))
                {
                    sb.AppendLine("<li>");
                    sb.Append(GenerateFileNodeHtml(child, level + 1, ServerPort, mimeTypesDic)); // Recursive call for child nodes
                    sb.AppendLine("</li>");
                }
                sb.AppendLine("</ul>");
            }

            sb.AppendLine("</div>");

            return sb.ToString();
        }
    }

    public class FileNode
    {
        public string Name { get; set; }
        public string Link { get; set; }
        public string Image { get; set; }
        public string Descriptor { get; set; }
        public string Content { get; set; }
        public string Type { get; set; }
        public long? Size { get; set; }
        public DateTime? CreationDate { get; set; }
        public DateTime LastWriteTime { get; set; }
        public ConcurrentList<FileNode> Childrens { get; set; }

        public override string ToString()
        {
            string childrenInfo = Childrens != null && Childrens.Count > 0
                ? $"Children Count: {Childrens.Count}"
                : "No children";

            return $"FileNode: {Name}\n" +
                   $"Link: {Link}\n" +
                   $"Image: {Image}\n" +
                   $"Descriptor: {Descriptor}\n" +
                   $"Content: {Content}\n" +
                   $"Type: {Type}\n" +
                   $"Size: {Size?.ToString() ?? "N/A"}\n" +
                   $"Creation Date: {CreationDate}\n" +
                   $"Last Write-Time: {LastWriteTime}\n" +
                   $"{childrenInfo}";
        }
    }

    public class FileStructure
    {
        public FileNode Root { get; set; }
    }
}
