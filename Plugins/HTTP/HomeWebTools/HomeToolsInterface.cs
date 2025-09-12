using CastleLibrary.Sony.Edge;
using EndianTools;
using HomeTools.AFS;
using HomeTools.BARFramework;
using HomeTools.CDS;
using HomeTools.ChannelID;
using HomeTools.Crypto;
using HomeTools.UnBAR;
using HttpMultipartParser;
using MultiServerLibrary.Extension;
using MultiServerLibrary.HTTP;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using WebAPIService;

namespace HomeWebTools
{
    public class HomeToolsInterface
    {
        public static (byte[], string)? MakeBarSdat(string ConvertersFolder, string TempDirPath, Stream PostData, string ContentType)
        {
            (byte[], string)? output = null;
            List<(byte[], string)?> TasksResult = new List<(byte[], string)?>();

            if (PostData != null && !string.IsNullOrEmpty(ContentType))
            {
                string maindir = TempDirPath + $"MakeBarSdat/{WebAPIsUtils.GenerateDynamicCacheFolder(WebAPIsUtils.GetCurrentDateTime())}";
                Directory.CreateDirectory(maindir);
                string boundary = HTTPProcessor.ExtractBoundary(ContentType);
                if (!string.IsNullOrEmpty(boundary))
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        PostData.CopyTo(ms);
                        ms.Position = 0;
                        ushort SDATVersion = 4;
                        ushort cdnMode = 0;
                        string filename = string.Empty;
                        string leanzlib = string.Empty;
                        string encrypt = string.Empty;
                        string version2 = string.Empty;
                        string bigendian = string.Empty;
                        string optimizeassets = string.Empty;
                        string assetsparams = string.Empty;
                        int TimeStamp = (int)DateTime.Now.ToFileTime();
                        var data = MultipartFormDataParser.Parse(ms, boundary);
                        string mode = data.GetParameterValue("mode");
                        string options = data.GetParameterValue("options");
                        try
                        {
                            TimeStamp = Convert.ToInt32(data.GetParameterValue("TimeStamp"), 16);
                        }
                        catch
                        {
                            // Not Important
                        }
                        try
                        {
                            leanzlib = data.GetParameterValue("leanzlib");
                        }
                        catch
                        {
                            // Not Important
                        }
                        try
                        {
                            encrypt = data.GetParameterValue("encrypt");
                        }
                        catch
                        {
                            // Not Important
                        }
                        try
                        {
                            version2 = data.GetParameterValue("version2");
                        }
                        catch
                        {
                            // Not Important
                        }
                        try
                        {
                            bigendian = data.GetParameterValue("bigendian");
                        }
                        catch
                        {
                            // Not Important
                        }
                        try
                        {
                            optimizeassets = data.GetParameterValue("optimizeassets");
                        }
                        catch
                        {
                            // Not Important
                        }
                        try
                        {
                            assetsparams = data.GetParameterValue("assetsparams");
                        }
                        catch
                        {
                            // Not Important
                        }
                        try
                        {
                            SDATVersion = ushort.Parse(data.GetParameterValue("sdatversion"));
                        }
                        catch
                        {
                            // Not Important
                        }
                        try
                        {
                            cdnMode = ushort.Parse(data.GetParameterValue("cdnmode"));
                        }
                        catch
                        {
                            // Not Important
                        }
                        switch (options)
                        {
                            case "cdn1":
                                options = ToolsImplementation.base64CDNKey1;
                                break;
                            case "cdn2":
                                options = ToolsImplementation.base64CDNKey2;
                                break;
                            default:
                                options = ToolsImplementation.base64DefaultSharcKey;
                                break;
                        }
                        foreach (FilePart multipartfile in data.Files)
                        {
                            using (Stream filedata = multipartfile.Data)
                            {
                                filedata.Position = 0;

                                // Find the number of bytes in the stream
                                int contentLength = (int)filedata.Length;

                                // Create a byte array
                                byte[] buffer = new byte[contentLength];

                                // Read the contents of the memory stream into the byte array
                                filedata.Read(buffer, 0, contentLength);

                                filename = multipartfile.FileName.TrimBeforeExtension();

                                string guid = WebAPIsUtils.GenerateDynamicCacheFolder(filename);

                                string tempdir = $"{maindir}/{guid}";

                                string unzipdir = tempdir + $"/unzip";

                                string rebardir = tempdir + $"/archive";

                                string zipfile = tempdir + $"/{filename}";

                                if (Directory.Exists(tempdir))
                                    Directory.Delete(tempdir, true);

                                Directory.CreateDirectory(rebardir);

                                File.WriteAllBytes(zipfile, buffer);

                                WebAPIsUtils.UncompressFile(zipfile, unzipdir);

                                filename = filename.Substring(0, filename.Length - 4).ToUpper();

                                IEnumerable<string> enumerable = Directory.EnumerateFiles(unzipdir, "*.*", SearchOption.AllDirectories);
                                BARArchive? bararchive = null;
                                bool optimizeassetsBool = optimizeassets == "on";
                                if (version2 == "on")
                                {
                                    if (bigendian == "on")
                                        bararchive = new BARArchive(ConvertersFolder, string.Format("{0}/{1}.SHARC", rebardir, filename), unzipdir, 0, TimeStamp, true, true, options, optimizeassetsBool);
                                    else
                                        bararchive = new BARArchive(ConvertersFolder, string.Format("{0}/{1}.SHARC", rebardir, filename), unzipdir, 0, TimeStamp, true, false, options, optimizeassetsBool);
                                }
                                else
                                {
                                    if (encrypt == "on")
                                    {
                                        if (bigendian == "on")
                                            bararchive = new BARArchive(ConvertersFolder, string.Format("{0}/{1}.BAR", rebardir, filename), unzipdir, cdnMode, TimeStamp, true, true, string.Empty, optimizeassetsBool);
                                        else
                                            bararchive = new BARArchive(ConvertersFolder, string.Format("{0}/{1}.BAR", rebardir, filename), unzipdir, cdnMode, TimeStamp, true, false, string.Empty, optimizeassetsBool);
                                    }
                                    else
                                    {
                                        if (bigendian == "on")
                                            bararchive = new BARArchive(ConvertersFolder, string.Format("{0}/{1}.BAR", rebardir, filename), unzipdir, 0, TimeStamp, false, true, string.Empty, optimizeassetsBool);
                                        else
                                            bararchive = new BARArchive(ConvertersFolder, string.Format("{0}/{1}.BAR", rebardir, filename), unzipdir, 0, TimeStamp, false, false, string.Empty, optimizeassetsBool);
                                    }
                                    if (leanzlib == "on")
                                    {
                                        bararchive.BARHeader.Flags = ArchiveFlags.Bar_Flag_LeanZLib;
                                        bararchive.DefaultCompression = CompressionMethod.ZLib;
                                    }
                                    else
                                        bararchive.BARHeader.Flags = ArchiveFlags.Bar_Flag_ZTOC;
                                }
#if NET6_0_OR_GREATER
                                bararchive.ImageMagickParams = assetsparams;
#endif
                                bararchive.AllowWhitespaceInFilenames = true;

                                foreach (string path in enumerable)
                                {
                                    bararchive.AddFile(Path.Combine(unzipdir, path));
                                }

                                // Create a text file to write the paths to
                                StreamWriter writer = new StreamWriter(unzipdir + @"/files.txt");

                                // Get all files in the directory and its immediate subdirectories
                                string[] files = Directory.GetFiles(unzipdir, "*.*", SearchOption.AllDirectories);

                                // Loop through the files and write their paths to the text file
                                foreach (string file in files)
                                {
                                    string relativePath = "file=\"" + file.Replace(unzipdir + @"\", string.Empty) + "\"";
                                    writer.WriteLine(relativePath.Replace(@"\", "/"));
                                }

                                writer.Close();

                                bararchive.AddFile(unzipdir + @"/files.txt");

                                bararchive.CreateManifest();

                                bararchive.Save();

                                bararchive = null;

                                if (mode == "sdat")
                                {
                                    if (version2 == "on")
                                        RunUnBAR.RunEncrypt(/*TempDirPath,*/ rebardir + $"/{filename}.SHARC", rebardir + $"/{filename.ToLower()}.sdat"/*, SDATVersion*/);
                                    else
                                        RunUnBAR.RunEncrypt(/*TempDirPath,*/ rebardir + $"/{filename}.BAR", rebardir + $"/{filename.ToLower()}.sdat"/*, SDATVersion*/);

                                    using (FileStream zipStream = new FileStream(rebardir + $"/{filename}_Rebar.zip", FileMode.Create))
                                    {
                                        using (ZipArchive archive = new ZipArchive(zipStream, ZipArchiveMode.Create))
                                        {
                                            // Add the first file to the archive
                                            ZipArchiveEntry entry1 = archive.CreateEntry($"{filename.ToLower()}.sdat");
                                            using (Stream entryStream = entry1.Open())
                                            {
                                                using (FileStream fileStream = new FileStream(rebardir + $"/{filename.ToLower()}.sdat", FileMode.Open))
                                                {
                                                    fileStream.CopyTo(entryStream);
                                                    fileStream.Flush();
                                                }
                                                entryStream.Flush();
                                            }

                                            if (version2 == "on")
                                            {
                                                // Add the second file to the archive
                                                ZipArchiveEntry entry2 = archive.CreateEntry($"{filename}.sharc.map");
                                                using (Stream entryStream = entry2.Open())
                                                {
                                                    using (FileStream fileStream = new FileStream(rebardir + $"/{filename}.sharc.map", FileMode.Open))
                                                    {
                                                        fileStream.CopyTo(entryStream);
                                                        fileStream.Flush();
                                                    }
                                                    entryStream.Flush();
                                                }
                                            }
                                            else
                                            {
                                                // Add the second file to the archive
                                                ZipArchiveEntry entry2 = archive.CreateEntry($"{filename}.bar.map");
                                                using (Stream entryStream = entry2.Open())
                                                {
                                                    using (FileStream fileStream = new FileStream(rebardir + $"/{filename}.bar.map", FileMode.Open))
                                                    {
                                                        fileStream.CopyTo(entryStream);
                                                        fileStream.Flush();
                                                    }
                                                    entryStream.Flush();
                                                }
                                            }
                                        }
                                    }

                                    TasksResult.Add((File.ReadAllBytes(rebardir + $"/{filename}_Rebar.zip"), $"{filename}_Rebar.zip"));
                                }
                                else
                                {
                                    using (FileStream zipStream = new FileStream(rebardir + $"/{filename}_Rebar.zip", FileMode.Create))
                                    {
                                        using (ZipArchive archive = new ZipArchive(zipStream, ZipArchiveMode.Create))
                                        {
                                            if (version2 == "on")
                                            {
                                                // Add the first file to the archive
                                                ZipArchiveEntry entry1 = archive.CreateEntry($"{filename}.SHARC");
                                                using (Stream entryStream = entry1.Open())
                                                {
                                                    using (FileStream fileStream = new FileStream(rebardir + $"/{filename}.SHARC", FileMode.Open))
                                                    {
                                                        fileStream.CopyTo(entryStream);
                                                        fileStream.Flush();
                                                    }
                                                    entryStream.Flush();
                                                }

                                                // Add the second file to the archive
                                                ZipArchiveEntry entry2 = archive.CreateEntry($"{filename}.sharc.map");
                                                using (Stream entryStream = entry2.Open())
                                                {
                                                    using (FileStream fileStream = new FileStream(rebardir + $"/{filename}.sharc.map", FileMode.Open))
                                                    {
                                                        fileStream.CopyTo(entryStream);
                                                        fileStream.Flush();
                                                    }
                                                    entryStream.Flush();
                                                }
                                            }
                                            else
                                            {
                                                // Add the first file to the archive
                                                ZipArchiveEntry entry1 = archive.CreateEntry($"{filename}.BAR");
                                                using (Stream entryStream = entry1.Open())
                                                {
                                                    using (FileStream fileStream = new FileStream(rebardir + $"/{filename}.BAR", FileMode.Open))
                                                    {
                                                        fileStream.CopyTo(entryStream);
                                                        fileStream.Flush();
                                                    }
                                                    entryStream.Flush();
                                                }

                                                // Add the second file to the archive
                                                ZipArchiveEntry entry2 = archive.CreateEntry($"{filename}.bar.map");
                                                using (Stream entryStream = entry2.Open())
                                                {
                                                    using (FileStream fileStream = new FileStream(rebardir + $"/{filename}.bar.map", FileMode.Open))
                                                    {
                                                        fileStream.CopyTo(entryStream);
                                                        fileStream.Flush();
                                                    }
                                                    entryStream.Flush();
                                                }
                                            }
                                        }
                                    }

                                    TasksResult.Add((File.ReadAllBytes(rebardir + $"/{filename}_Rebar.zip"), $"{filename}_Rebar.zip"));
                                }

                                if (Directory.Exists(tempdir))
                                    Directory.Delete(tempdir, true);

                                filedata.Flush();
                            }
                        }
                        ms.Flush();
                    }
                }


                if (Directory.Exists(maindir))
                    Directory.Delete(maindir, true);
            }

            if (TasksResult.Count > 0)
            {
                // Create a memory stream to hold the zip file content
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    // Create a ZipArchive in memory
                    using (ZipArchive archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                    {
                        foreach (var item in TasksResult)
                        {
                            if (item.HasValue)
                            {
                                // Add files or content to the zip archive
                                if (item.Value.Item1 != null)
                                    WebAPIsUtils.AddFileToZip(archive, item.Value.Item2, new MemoryStream(item.Value.Item1));
                            }
                        }
                    }

                    memoryStream.Position = 0;

                    output = (memoryStream.ToArray(), $"MakeBarSdat_Results.zip");
                }
            }

            return output;
        }

        public static async Task<(byte[], string)?> UnBar(string TempDirPath, Stream PostData, string ContentType, string HelperStaticFolder)
        {
            (byte[], string)? output = null;
            List<(byte[], string)?> TasksResult = new List<(byte[], string)?>();

            if (PostData != null && !string.IsNullOrEmpty(ContentType))
            {
                string maindir = TempDirPath + $"UnBar/{WebAPIsUtils.GenerateDynamicCacheFolder(WebAPIsUtils.GetCurrentDateTime())}";
                Directory.CreateDirectory(maindir);
                string boundary = HTTPProcessor.ExtractBoundary(ContentType);
                if (!string.IsNullOrEmpty(boundary))
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        PostData.CopyTo(ms);
                        ms.Position = 0;
                        ushort cdnMode = 0;
                        string filename = string.Empty;
                        string ogfilename = string.Empty;
                        string subfolder = string.Empty;
                        string bruteforce = string.Empty;
                        var data = MultipartFormDataParser.Parse(ms, boundary);
                        string prefix = data.GetParameterValue("prefix");
                        try
                        {
                            subfolder = data.GetParameterValue("subfolder");
                        }
                        catch
                        {
                            // Not Important
                        }
                        try
                        {
                            bruteforce = data.GetParameterValue("bruteforce");
                        }
                        catch
                        {
                            // Not Important
                        }
                        try
                        {
                            cdnMode = ushort.Parse(data.GetParameterValue("cdnmode"));
                        }
                        catch
                        {
                            // Not Important
                        }
                        foreach (FilePart multipartfile in data.Files.Where(file => file.FileName.EndsWith(".bar", StringComparison.InvariantCultureIgnoreCase)
                        || file.FileName.EndsWith(".sharc", StringComparison.InvariantCultureIgnoreCase) || file.FileName.EndsWith(".sdat", StringComparison.InvariantCultureIgnoreCase)
                        || file.FileName.EndsWith(".dat", StringComparison.InvariantCultureIgnoreCase) || file.FileName.EndsWith(".zip", StringComparison.InvariantCultureIgnoreCase)))
                        {
                            using (Stream filedata = multipartfile.Data)
                            {
                                filedata.Position = 0;

                                // Find the number of bytes in the stream
                                int contentLength = (int)filedata.Length;

                                // Create a byte array
                                byte[] buffer = new byte[contentLength];

                                // Read the contents of the memory stream into the byte array
                                filedata.Read(buffer, 0, contentLength);

                                filename = multipartfile.FileName.TrimBeforeExtension();

                                string mapfilepath = filename + ".map";

                                string tempdir = $"{maindir}/{WebAPIsUtils.GenerateDynamicCacheFolder(filename)}";

                                string unbardir = tempdir + $"/unbar";

                                string barfile = tempdir + $"/{filename}";

                                FilePart? mapmultipartfile = data.Files.Where(file => file.FileName.Equals(mapfilepath, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();

                                Directory.CreateDirectory(unbardir);

                                File.WriteAllBytes(barfile, buffer);

                                if (mapmultipartfile != null)
                                {
                                    using (Stream mapfiledata = mapmultipartfile.Data)
                                    {
                                        mapfiledata.Position = 0;

                                        // Find the number of bytes in the stream
                                        contentLength = (int)mapfiledata.Length;

                                        // Create a byte array
                                        buffer = new byte[contentLength];

                                        // Read the contents of the memory stream into the byte array
                                        mapfiledata.Read(buffer, 0, contentLength);

                                        File.WriteAllBytes(tempdir + $"/{mapfilepath}", buffer);

                                        mapfiledata.Flush();
                                    }
                                }

                                if (filename.EndsWith(".bar", StringComparison.InvariantCultureIgnoreCase) || filename.EndsWith(".dat", StringComparison.InvariantCultureIgnoreCase))
                                {
                                    await RunUnBAR.Run(TempDirPath, barfile, unbardir, false, cdnMode);
                                    ogfilename = filename;
                                    filename = filename.Substring(0, filename.Length - 4).ToUpper();
                                }
                                else if (filename.EndsWith(".sharc", StringComparison.InvariantCultureIgnoreCase))
                                {
                                    await RunUnBAR.Run(TempDirPath, barfile, unbardir, false, 0);
                                    ogfilename = filename;
                                    filename = filename.Substring(0, filename.Length - 6).ToUpper();
                                }
                                else if (filename.EndsWith(".sdat", StringComparison.InvariantCultureIgnoreCase))
                                {
                                    await RunUnBAR.Run(TempDirPath, barfile, unbardir, true, cdnMode);
                                    ogfilename = filename;
                                    filename = filename.Substring(0, filename.Length - 5).ToUpper();
                                }
                                else if (filename.EndsWith(".zip", StringComparison.InvariantCultureIgnoreCase))
                                {
                                    WebAPIsUtils.UncompressFile(barfile, unbardir);
                                    ogfilename = filename;
                                    filename = filename.Substring(0, filename.Length - 4).ToUpper();
                                }
                                else
                                {
                                    if (Directory.Exists(tempdir))
                                        Directory.Delete(tempdir, true);

                                    filedata.Flush();
                                    continue;
                                }

                                AFSMapper? map = new AFSMapper();

                                if (Directory.Exists(unbardir + $"/{filename}") && (ogfilename.EndsWith(".bar", StringComparison.InvariantCultureIgnoreCase) || ogfilename.EndsWith(".sharc", StringComparison.InvariantCultureIgnoreCase) || ogfilename.EndsWith(".sdat", StringComparison.InvariantCultureIgnoreCase)))
                                {
                                    if (subfolder == "on")
                                    {
                                        foreach (var dircursor in Directory.GetDirectories(unbardir + $"/{filename}"))
                                        {
                                            int fileCount = Directory.GetFiles(dircursor).Length;

                                            if (fileCount > 0)
                                                await map.MapperStart(dircursor, HelperStaticFolder, prefix, bruteforce);
                                        }
                                    }
                                    else
                                    {
                                        int fileCount = Directory.GetFiles(unbardir + $"/{filename}").Length;

                                        if (fileCount > 0)
                                            await map.MapperStart(unbardir + $"/{filename}", HelperStaticFolder, prefix, bruteforce);
                                    }

                                    ZipFile.CreateFromDirectory(unbardir + $"/{filename}", tempdir + $"/{filename}_Mapped.zip");

                                    TasksResult.Add((File.ReadAllBytes(tempdir + $"/{filename}_Mapped.zip"), $"{filename}_Mapped.zip"));
                                }
                                else if (Directory.Exists(unbardir + $"/{filename}"))
                                {
                                    if (subfolder == "on")
                                    {
                                        foreach (var dircursor in Directory.GetDirectories(unbardir + $"/{filename}"))
                                        {
                                            int fileCount = Directory.GetFiles(dircursor).Length;

                                            if (fileCount > 0)
                                                await map.MapperStart(dircursor, HelperStaticFolder, prefix, bruteforce);
                                        }
                                    }
                                    else
                                    {
                                        int fileCount = Directory.GetFiles(unbardir + $"/{filename}").Length;

                                        if (fileCount > 0)
                                            await map.MapperStart(unbardir + $"/{filename}", HelperStaticFolder, prefix, bruteforce);
                                    }

                                    ZipFile.CreateFromDirectory(unbardir + $"/{filename}", tempdir + $"/{filename}_Mapped.zip");

                                    TasksResult.Add((File.ReadAllBytes(tempdir + $"/{filename}_Mapped.zip"), $"{filename}_Mapped.zip"));
                                }
                                else
                                {
                                    if (subfolder == "on")
                                    {
                                        foreach (var dircursor in Directory.GetDirectories(unbardir))
                                        {
                                            int fileCount = Directory.GetFiles(dircursor).Length;

                                            if (fileCount > 0)
                                                await map.MapperStart(dircursor, HelperStaticFolder, prefix, bruteforce);
                                        }
                                    }
                                    else
                                    {
                                        int fileCount = Directory.GetFiles(unbardir).Length;

                                        if (fileCount > 0)
                                            await map.MapperStart(unbardir, HelperStaticFolder, prefix, bruteforce);
                                    }

                                    ZipFile.CreateFromDirectory(unbardir, tempdir + $"/{filename}_Mapped.zip");

                                    TasksResult.Add((File.ReadAllBytes(tempdir + $"/{filename}_Mapped.zip"), $"{filename}_Mapped.zip"));
                                }

                                map = null;

                                if (Directory.Exists(tempdir))
                                    Directory.Delete(tempdir, true);

                                filedata.Flush();
                            }
                        }
                        ms.Flush();
                    }
                }

                if (Directory.Exists(maindir))
                    Directory.Delete(maindir, true);
            }

            if (TasksResult.Count > 0)
            {
                // Create a memory stream to hold the zip file content
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    // Create a ZipArchive in memory
                    using (ZipArchive archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                    {
                        foreach (var item in TasksResult)
                        {
                            if (item.HasValue)
                            {
                                // Add files or content to the zip archive
                                if (item.Value.Item1 != null)
                                    WebAPIsUtils.AddFileToZip(archive, item.Value.Item2, new MemoryStream(item.Value.Item1));
                            }
                        }
                    }

                    memoryStream.Position = 0;

                    output = (memoryStream.ToArray(), $"UnBar_Results.zip");
                }
            }

            return output;
        }

        public static (byte[], string)? CDS(Stream PostData, string ContentType)
        {
            (byte[], string)? output = null;
            List<(byte[], string)?> TasksResult = new List<(byte[], string)?>();

            if (PostData != null && !string.IsNullOrEmpty(ContentType))
            {
                string boundary = HTTPProcessor.ExtractBoundary(ContentType);
                if (!string.IsNullOrEmpty(boundary))
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        PostData.CopyTo(ms);
                        ms.Position = 0;
                        int i = 0;
                        ushort cdnMode = 0;
                        string filename = string.Empty;
                        string decrypt = string.Empty;
                        var data = MultipartFormDataParser.Parse(ms, boundary);
                        string sha1 = data.GetParameterValue("sha1");
                        try
                        {
                            decrypt = data.GetParameterValue("decrypt");
                        }
                        catch
                        {
                            // Not Important
                        }
                        try
                        {
                            cdnMode = ushort.Parse(data.GetParameterValue("cdnmode"));
                        }
                        catch
                        {
                            // Not Important
                        }
                        foreach (FilePart multipartfile in data.Files)
                        {
                            using (Stream filedata = multipartfile.Data)
                            {
                                filedata.Position = 0;

                                // Find the number of bytes in the stream
                                int contentLength = (int)filedata.Length;

                                // Create a byte array
                                byte[] buffer = new byte[contentLength];

                                // Read the contents of the memory stream into the byte array
                                filedata.Read(buffer, 0, contentLength);

                                filename = multipartfile.FileName;

                                if (!string.IsNullOrEmpty(sha1) && sha1.Length >= 16)
                                {
                                    byte[] ProcessedFileBytes = CDSProcess.CDSEncrypt_Decrypt(buffer, sha1.Substring(0, 16), cdnMode);

                                    if (ProcessedFileBytes != null)
                                        TasksResult.Add((ProcessedFileBytes, Path.GetFileNameWithoutExtension(filename) + $"_decrypted{Path.GetExtension(filename)}"));
                                }
                                else
                                {
                                    byte[] ProcessedFileBytes = CDSProcess.CDSEncrypt_Decrypt(buffer, NetHasher.DotNetHasher.ComputeSHA1String(buffer).Substring(0, 16).ToUpper(), cdnMode);

                                    if (ProcessedFileBytes != null)
                                        TasksResult.Add((ProcessedFileBytes, Path.GetFileNameWithoutExtension(filename) + $"_encrypted{Path.GetExtension(filename)}"));
                                }

                                i++;
                                filedata.Flush();
                            }
                        }
                        ms.Flush();
                    }
                }
            }

            if (TasksResult.Count > 0)
            {
                // Create a memory stream to hold the zip file content
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    // Create a ZipArchive in memory
                    using (ZipArchive archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                    {
                        foreach (var item in TasksResult)
                        {
                            if (item.HasValue)
                            {
                                // Add files or content to the zip archive
                                if (item.Value.Item1 != null)
                                    WebAPIsUtils.AddFileToZip(archive, item.Value.Item2, new MemoryStream(item.Value.Item1));
                            }
                        }
                    }

                    memoryStream.Position = 0;

                    output = (memoryStream.ToArray(), $"CDS_Results.zip");
                }
            }

            return output;
        }

        public static (byte[], string)? CDSBruteforce(Stream PostData, string ContentType)
        {
            (byte[], string)? output = null;
            List<(byte[], string)?> TasksResult = new List<(byte[], string)?>();

            if (PostData != null && !string.IsNullOrEmpty(ContentType))
            {
                string boundary = HTTPProcessor.ExtractBoundary(ContentType);
                if (!string.IsNullOrEmpty(boundary))
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        PostData.CopyTo(ms);
                        ms.Position = 0;
                        int i = 0;
                        ushort cdnMode = 0;
                        string filename = string.Empty;
                        var data = MultipartFormDataParser.Parse(ms, boundary);
                        try
                        {
                            cdnMode = ushort.Parse(data.GetParameterValue("cdnmode"));
                        }
                        catch
                        {
                            // Not Important
                        }
                        foreach (FilePart multipartfile in data.Files)
                        {
                            using (Stream filedata = multipartfile.Data)
                            {
                                filedata.Position = 0;

                                // Find the number of bytes in the stream
                                int contentLength = (int)filedata.Length;

                                // Create a byte array
                                byte[] buffer = new byte[contentLength];

                                // Read the contents of the memory stream into the byte array
                                filedata.Read(buffer, 0, contentLength);

                                filename = multipartfile.FileName;

                                BruteforceProcess? proc = new BruteforceProcess(buffer);

                                if (filename.ToLower().Contains(".hcdb"))
                                    TasksResult.Add((proc.StartBruteForce(cdnMode, 1), filename));
                                else if (filename.ToLower().Contains(".bar"))
                                    TasksResult.Add((proc.StartBruteForce(cdnMode, 2), filename));
                                else
                                    TasksResult.Add((proc.StartBruteForce(cdnMode), filename));

                                proc = null;

                                i++;
                                filedata.Flush();
                            }
                        }
                        ms.Flush();
                    }
                }
            }

            if (TasksResult.Count > 0)
            {
                // Create a memory stream to hold the zip file content
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    // Create a ZipArchive in memory
                    using (ZipArchive archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                    {
                        foreach (var item in TasksResult)
                        {
                            if (item.HasValue)
                            {
                                // Add files or content to the zip archive
                                if (item.Value.Item1 != null)
                                    WebAPIsUtils.AddFileToZip(archive, item.Value.Item2, new MemoryStream(item.Value.Item1));
                            }
                        }
                    }

                    memoryStream.Position = 0;

                    output = (memoryStream.ToArray(), $"CDSBruteforce_Results.zip");
                }
            }

            return output;
        }

        public static (byte[], string)? HCDBUnpack(Stream PostData, string ContentType)
        {
            (byte[], string)? output = null;
            List<(byte[], string)?> TasksResult = new List<(byte[], string)?>();

            if (PostData != null && !string.IsNullOrEmpty(ContentType))
            {
                string boundary = HTTPProcessor.ExtractBoundary(ContentType);
                if (!string.IsNullOrEmpty(boundary))
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        PostData.CopyTo(ms);
                        ms.Position = 0;
                        int i = 0;
                        string filename = string.Empty;
                        foreach (FilePart multipartfile in MultipartFormDataParser.Parse(ms, boundary).Files)
                        {
                            using (Stream filedata = multipartfile.Data)
                            {
                                filedata.Position = 0;

                                // Find the number of bytes in the stream
                                int contentLength = (int)filedata.Length;

                                // Create a byte array
                                byte[] buffer = new byte[contentLength];

                                // Read the contents of the memory stream into the byte array
                                filedata.Read(buffer, 0, contentLength);

                                filename = multipartfile.FileName;

                                byte[] DecompressedData = LZMA.SegmentsDecompress(buffer);

                                if (DecompressedData != null)
                                    TasksResult.Add((DecompressedData, $"{Path.GetFileNameWithoutExtension(filename)}.sql"));

                                i++;
                                filedata.Flush();
                            }
                        }
                        ms.Flush();
                    }
                }
            }

            if (TasksResult.Count > 0)
            {
                // Create a memory stream to hold the zip file content
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    // Create a ZipArchive in memory
                    using (ZipArchive archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                    {
                        foreach (var item in TasksResult)
                        {
                            if (item.HasValue)
                            {
                                // Add files or content to the zip archive
                                if (item.Value.Item1 != null)
                                    WebAPIsUtils.AddFileToZip(archive, item.Value.Item2, new MemoryStream(item.Value.Item1));
                            }
                        }
                    }

                    memoryStream.Position = 0;

                    output = (memoryStream.ToArray(), $"HCDBUnpack_Results.zip");
                }
            }

            return output;
        }

        public static (byte[], string)? TicketList(Stream PostData, string ContentType)
        {
            (byte[], string)? output = null;
            List<(byte[], string)?> TasksResult = new List<(byte[], string)?>();

            if (PostData != null && !string.IsNullOrEmpty(ContentType))
            {
                string boundary = HTTPProcessor.ExtractBoundary(ContentType);
                if (!string.IsNullOrEmpty(boundary))
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        PostData.CopyTo(ms);
                        ms.Position = 0;
                        int i = 0;
                        string filename = string.Empty;
                        var data = MultipartFormDataParser.Parse(ms, boundary);
                        string version1 = string.Empty;
                        try
                        {
                            version1 = data.GetParameterValue("version1");
                        }
                        catch
                        {
                            // Not Important
                        }
                        foreach (FilePart multipartfile in data.Files)
                        {
                            using (Stream filedata = multipartfile.Data)
                            {
                                filedata.Position = 0;

                                // Find the number of bytes in the stream
                                int contentLength = (int)filedata.Length;

                                // Create a byte array
                                byte[] buffer = new byte[contentLength];

                                // Read the contents of the memory stream into the byte array
                                filedata.Read(buffer, 0, contentLength);

                                filename = multipartfile.FileName;

                                if (buffer.Length > 8 && buffer[0] == 0xBE && buffer[1] == 0xE5 && buffer[2] == 0xBE && buffer[3] == 0xE5
                                     && buffer[4] == 0x00 && buffer[5] == 0x00 && buffer[6] == 0x00 && buffer[7] == 0x01 && version1 == "on")
                                {
                                    byte[] ProcessedFileBytes = new byte[buffer.Length - 8];
                                    Buffer.BlockCopy(buffer, 8, ProcessedFileBytes, 0, ProcessedFileBytes.Length);
                                    ProcessedFileBytes = ToolsImplementation.ProcessCrypt_Decrypt(ProcessedFileBytes, ToolsImplementation.TicketListV1Key, ToolsImplementation.TicketListV1IV.ShadowCopy(), 1);
                                    if (ProcessedFileBytes != null)
                                        TasksResult.Add((ProcessedFileBytes, $"{filename}_Decrypted.lst"));
                                }
                                else if (version1 == "on")
                                    TasksResult.Add((ByteUtils.CombineByteArray(new byte[] { 0xBE, 0xE5, 0xBE, 0xE5, 0x00, 0x00, 0x00, 0x01 }, ToolsImplementation.ProcessCrypt_Decrypt(buffer, ToolsImplementation.TicketListV1Key, ToolsImplementation.TicketListV1IV.ShadowCopy(), 1))
                                            , $"{filename}_Encrypted.lst"));
                                else if (buffer.Length > 8 && buffer[0] == 0xBE && buffer[1] == 0xE5 && buffer[2] == 0xBE && buffer[3] == 0xE5
                                    && buffer[4] == 0x00 && buffer[5] == 0x00 && buffer[6] == 0x00 && buffer[7] == 0x00)
                                {
                                    byte[] ProcessedFileBytes = new byte[buffer.Length - 8];
                                    Buffer.BlockCopy(buffer, 8, ProcessedFileBytes, 0, ProcessedFileBytes.Length);
                                    ProcessedFileBytes = ToolsImplementation.ProcessCrypt_Decrypt(ProcessedFileBytes, ToolsImplementation.TicketListV0Key, ToolsImplementation.TicketListV0IV.ShadowCopy(), 1);
                                    if (ProcessedFileBytes != null)
                                        TasksResult.Add((ProcessedFileBytes, $"{filename}_Decrypted.lst"));
                                }
                                else
                                    TasksResult.Add((ByteUtils.CombineByteArray(new byte[] { 0xBE, 0xE5, 0xBE, 0xE5, 0x00, 0x00, 0x00, 0x00 }, ToolsImplementation.ProcessCrypt_Decrypt(buffer, ToolsImplementation.TicketListV0Key, ToolsImplementation.TicketListV0IV.ShadowCopy(), 1))
                                            , $"{filename}_Encrypted.lst"));

                                i++;
                                filedata.Flush();
                            }
                        }
                        ms.Flush();
                    }
                }
            }

            if (TasksResult.Count > 0)
            {
                // Create a memory stream to hold the zip file content
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    // Create a ZipArchive in memory
                    using (ZipArchive archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                    {
                        foreach (var item in TasksResult)
                        {
                            if (item.HasValue)
                            {
                                // Add files or content to the zip archive
                                if (item.Value.Item1 != null)
                                    WebAPIsUtils.AddFileToZip(archive, item.Value.Item2, new MemoryStream(item.Value.Item1));
                            }
                        }
                    }

                    memoryStream.Position = 0;

                    output = (memoryStream.ToArray(), $"TicketList_Results.zip");
                }
            }

            return output;
        }

        public static (byte[], string)? INF(Stream PostData, string ContentType)
        {
            (byte[], string)? output = null;
            List<(byte[], string)?> TasksResult = new List<(byte[], string)?>();

            if (PostData != null && !string.IsNullOrEmpty(ContentType))
            {
                string boundary = HTTPProcessor.ExtractBoundary(ContentType);
                if (!string.IsNullOrEmpty(boundary))
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        PostData.CopyTo(ms);
                        ms.Position = 0;
                        int i = 0;
                        string filename = string.Empty;
                        foreach (FilePart multipartfile in MultipartFormDataParser.Parse(ms, boundary).Files)
                        {
                            using (Stream filedata = multipartfile.Data)
                            {
                                filedata.Position = 0;

                                // Find the number of bytes in the stream
                                int contentLength = (int)filedata.Length;

                                // Create a byte array
                                byte[] buffer = new byte[contentLength];

                                // Read the contents of the memory stream into the byte array
                                filedata.Read(buffer, 0, contentLength);

                                filename = multipartfile.FileName;

                                if (buffer[0] == 0x00 && buffer[1] == 0x00 && buffer[2] == 0x00 && buffer[3] == 0x01)
                                {
                                    buffer = ToolsImplementation.RemovePaddingPrefix(buffer);

                                    byte[] decryptedfilebytes = LIBSECURE.Crypt_Decrypt(buffer, ToolsImplementation.MetaDataV1IVA, 8);

                                    if (decryptedfilebytes != null)
                                        TasksResult.Add((decryptedfilebytes, $"{filename}_Decrypted.bin"));
                                }
                                else if (buffer[0] == 0xBE && buffer[1] == 0xE5 && buffer[2] == 0xBE && buffer[3] == 0xE5)
                                {
                                    byte[] encryptedfilebytes = LIBSECURE.Crypt_Decrypt(buffer, ToolsImplementation.MetaDataV1IVA, 8);

                                    if (encryptedfilebytes != null)
                                        TasksResult.Add((ToolsImplementation.ApplyPaddingPrefix(encryptedfilebytes, false), $"{filename}_Encrypted.bin"));
                                }

                                i++;
                                filedata.Flush();
                            }
                        }
                        ms.Flush();
                    }
                }
            }

            if (TasksResult.Count > 0)
            {
                // Create a memory stream to hold the zip file content
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    // Create a ZipArchive in memory
                    using (ZipArchive archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                    {
                        foreach (var item in TasksResult)
                        {
                            if (item.HasValue)
                            {
                                // Add files or content to the zip archive
                                if (item.Value.Item1 != null)
                                    WebAPIsUtils.AddFileToZip(archive, item.Value.Item2, new MemoryStream(item.Value.Item1));
                            }
                        }
                    }

                    memoryStream.Position = 0;

                    output = (memoryStream.ToArray(), $"INF_Results.zip");
                }
            }

            return output;
        }

        public static (byte[], string)? ProcessProfanity(Stream PostData, string ContentType)
        {
            (byte[], string)? output = null;
            List<(byte[], string)?> TasksResult = new List<(byte[], string)?>();

            if (PostData != null && !string.IsNullOrEmpty(ContentType))
            {
                string boundary = HTTPProcessor.ExtractBoundary(ContentType);
                if (!string.IsNullOrEmpty(boundary))
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        PostData.CopyTo(ms);
                        ms.Position = 0;
                        int i = 0;
                        foreach (FilePart multipartfile in MultipartFormDataParser.Parse(ms, boundary).Files)
                        {
                            using (Stream filedata = multipartfile.Data)
                            {
                                filedata.Position = 0;

                                // Find the number of bytes in the stream
                                int contentLength = (int)filedata.Length;

                                // Create a byte array
                                byte[] buffer = new byte[contentLength];

                                // Read the contents of the memory stream into the byte array
                                filedata.Read(buffer, 0, contentLength);

                                try
                                {
                                    TasksResult.Add((EndianUtils.EndianSwap(ProfanityFilterSecurity.PF_DeCipher(buffer)), multipartfile.FileName));
                                }
                                catch (Exception ex)
                                {
                                    CustomLogger.LoggerAccessor.LogError($"[HomeToolsInterface] - ProcessProfanity - thrown an exception: {ex}");
                                }

                                i++;
                                filedata.Flush();
                            }
                        }
                        ms.Flush();
                    }
                }
            }

            if (TasksResult.Count > 0)
            {
                // Create a memory stream to hold the zip file content
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    // Create a ZipArchive in memory
                    using (ZipArchive archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                    {
                        foreach (var item in TasksResult)
                        {
                            if (item.HasValue)
                            {
                                // Add files or content to the zip archive
                                if (item.Value.Item1 != null)
                                    WebAPIsUtils.AddFileToZip(archive, item.Value.Item2, new MemoryStream(item.Value.Item1));
                            }
                        }
                    }

                    memoryStream.Position = 0;

                    output = (memoryStream.ToArray(), $"Profanity_Results.zip");
                }
            }

            return output;
        }

        public static string? ChannelID(Stream PostData, string ContentType)
        {
            string? res = null;

            if (PostData != null && !string.IsNullOrEmpty(ContentType))
            {
                string boundary = HTTPProcessor.ExtractBoundary(ContentType);
                if (!string.IsNullOrEmpty(boundary))
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        PostData.CopyTo(ms);
                        ms.Position = 0;
                        int sceneid = 0;
                        string newerhome = string.Empty;
                        var data = MultipartFormDataParser.Parse(ms, boundary);
                        try
                        {
                            sceneid = int.Parse(data.GetParameterValue("sceneid"));
                        }
                        catch
                        {
                            // Not Important
                        }
                        try
                        {
                            newerhome = data.GetParameterValue("newerhome");
                        }
                        catch
                        {
                            // Not Important
                        }
                        if (newerhome == "on")
                        {
                            SceneKey? sceneKey = SIDKeyGenerator.Instance.GenerateNewerType(Convert.ToUInt16(sceneid));
                            res = sceneKey.ToString();
                            sceneKey = null;
                        }
                        else
                        {
                            SceneKey? sceneKey = SIDKeyGenerator.Instance.Generate(Convert.ToUInt16(sceneid));
                            res = sceneKey.ToString();
                            sceneKey = null;
                        }
                        ms.Flush();
                    }
                }
            }

            return res;
        }

        public static string? SceneID(Stream PostData, string ContentType)
        {
            string? res = null;

            if (PostData != null && !string.IsNullOrEmpty(ContentType))
            {
                string boundary = HTTPProcessor.ExtractBoundary(ContentType);
                if (!string.IsNullOrEmpty(boundary))
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        PostData.CopyTo(ms);
                        ms.Position = 0;
                        string newerhome = string.Empty;
                        var data = MultipartFormDataParser.Parse(ms, boundary);
                        string channelid = data.GetParameterValue("channelid");
                        try
                        {
                            newerhome = data.GetParameterValue("newerhome");
                        }
                        catch
                        {

                        }
                        if (newerhome == "on")
                        {
                            SceneKey? sceneKey = new SceneKey(channelid);
                            try
                            {
                                SIDKeyGenerator.VerifyNewerKey(sceneKey);
                                res = SIDKeyGenerator.Instance.ExtractSceneIDNewerType(sceneKey).ToString();
                            }
                            catch (SceneKeyException)
                            {
                                res = "Invalid ChannelID";
                            }
                            catch
                            {
                                // Not Important
                            }
                            sceneKey = null;
                        }
                        else
                        {
                            SceneKey? sceneKey = new SceneKey(channelid);
                            try
                            {
                                SIDKeyGenerator.Verify(sceneKey);
                                res = SIDKeyGenerator.Instance.ExtractSceneID(sceneKey).ToString();
                            }
                            catch (SceneKeyException)
                            {
                                res = "Invalid ChannelID";
                            }
                            catch
                            {
                                // Not Important
                            }
                            sceneKey = null;
                        }
                        ms.Flush();
                    }
                }
            }

            return res;
        }
    }
}
