using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HomeTools.AFS
{
    public class AFSMapper : IDisposable
    {
        private static readonly string UUIDRegexModel = @"[0-9a-fA-F]{8}-[0-9a-fA-F]{8}-[0-9a-fA-F]{8}-[0-9a-fA-F]{8}";
        private Dictionary<string, string> MappedAFSHashesCache = null;
        private bool disposedValue;

        public async Task MapperStart(string CurrentFolder, string MapperHelperFolder, string prefix, string BruteforceUUIDs)
        {
           bool hasHelperFolder = InitAFSMappedList(MapperHelperFolder);

            Match objectmatch = new Regex(UUIDRegexModel).Match(CurrentFolder);

            if (objectmatch.Success) // We first map the corresponding object.
            {
                string Objectprefix = $"objects/{objectmatch.Groups[0].Value}/";

                foreach (string ObjectMetaDataRelativePath in new List<string>() { $"{Objectprefix}object.xml", $"{Objectprefix}resources.xml", $"{Objectprefix}localisation.xml" })
                {
                    string text = AFSHash.EscapeString(ObjectMetaDataRelativePath);
                    string CrcHash = new AFSHash(text).Value.ToString("X8");

                    // Search for files with names matching the CRC hash, regardless of the extension
                    foreach (string filePath in Directory.GetFiles(CurrentFolder)
                      .Where(path => new Regex($"(?:0X)?{CrcHash}(?:\\.\\w+)?$").IsMatch(Path.GetFileNameWithoutExtension(path)))
                      .ToArray())
                    {
                        // Already mapped.
                        if (!File.Exists(filePath))
                            continue;

                        string NewfilePath = (CurrentFolder + $"/{text}").ToUpper();
                        string destinationDirectory = Path.GetDirectoryName(NewfilePath);
                        string fileContent = File.ReadAllText(filePath);

                        if (!string.IsNullOrEmpty(destinationDirectory) && !Directory.Exists(destinationDirectory))
                            Directory.CreateDirectory(destinationDirectory.ToUpper());

                        if (!File.Exists(NewfilePath))
                            File.Move(filePath, NewfilePath);

                        await SubHashMapBatch(CurrentFolder, Objectprefix, fileContent).ConfigureAwait(false);
                    }
                }
            }

            if (BruteforceUUIDs == "on" && hasHelperFolder && File.Exists(MapperHelperFolder + "/uuid_helper.txt"))
            {
                // Open the file for reading
                using (StreamReader reader = new StreamReader(MapperHelperFolder + "/uuid_helper.txt"))
                {
                    string line = null;

                    // Read and display lines from the file until the end of the file is reached
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (!string.IsNullOrEmpty(line))
                        {
                            objectmatch = new Regex(UUIDRegexModel).Match(line);

                            if (objectmatch.Success) // We first map the corresponding object.
                            {
                                string Objectprefix = $"objects/{objectmatch.Groups[0].Value}/";

                                foreach (string ObjectMetaDataRelativePath in new List<string>() { $"{Objectprefix}object.xml", $"{Objectprefix}resources.xml", $"{Objectprefix}localisation.xml" })
                                {
                                    string text = AFSHash.EscapeString(ObjectMetaDataRelativePath);
                                    string CrcHash = new AFSHash(text).Value.ToString("X8");

                                    // Search for files with names matching the CRC hash, regardless of the extension
                                    foreach (string filePath in Directory.GetFiles(CurrentFolder)
                                      .Where(path => new Regex($"(?:0X)?{CrcHash}(?:\\.\\w+)?$").IsMatch(Path.GetFileNameWithoutExtension(path)))
                                      .ToArray())
                                    {
                                        // Already mapped.
                                        if (!File.Exists(filePath))
                                            continue;

                                        string NewfilePath = (CurrentFolder + $"/{text}").ToUpper();
                                        string destinationDirectory = Path.GetDirectoryName(NewfilePath);
                                        string fileContent = File.ReadAllText(filePath);

                                        if (!string.IsNullOrEmpty(destinationDirectory) && !Directory.Exists(destinationDirectory))
                                            Directory.CreateDirectory(destinationDirectory.ToUpper());

                                        if (!File.Exists(NewfilePath))
                                            File.Move(filePath, NewfilePath);

                                        await SubHashMapBatch(CurrentFolder, Objectprefix, fileContent).ConfigureAwait(false);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            foreach (string CrcHash in MappedAFSHashesCache.Keys)
            {
                // Search for files with names matching the CRC hash, regardless of the extension
                foreach (string filePath in Directory.GetFiles(CurrentFolder)
                  .Where(path => new Regex($"(?:0X)?{CrcHash}(?:\\.\\w+)?$").IsMatch(Path.GetFileNameWithoutExtension(path)))
                  .ToArray())
                {
                    // Already mapped.
                    if (!File.Exists(filePath))
                        continue;

                    string NewfilePath = (CurrentFolder + $"/{MappedAFSHashesCache[CrcHash]}").ToUpper();
                    string destinationDirectory = Path.GetDirectoryName(NewfilePath);
                    string fileContent = File.ReadAllText(filePath);

                    if (!string.IsNullOrEmpty(destinationDirectory) && !Directory.Exists(destinationDirectory))
                        Directory.CreateDirectory(destinationDirectory.ToUpper());

                    if (!File.Exists(NewfilePath))
                        File.Move(filePath, NewfilePath);

                    await SubHashMapBatch(CurrentFolder, prefix, fileContent).ConfigureAwait(false);
                }
            }
        }

        public static async Task SubHashMapBatch(string CurrentFolder, string prefix, string FileContent)
        {
            if (!string.IsNullOrEmpty(FileContent))
            {
                foreach (MappedList match in new List<MappedList>(AFSRegexProcessor.ScanForString(FileContent)))
                {
                    if (!string.IsNullOrEmpty(match.file))
                    {
                        string text = AFSHash.EscapeString(match.file);

                        if (text.ToLower().EndsWith(".atmos"))
                        {
                            string cdatapath = text.Remove(text.Length - 6) + ".cdata"; // Eboot does this by default.

                            // Search for files with names matching the CRC hash, regardless of the extension
                            foreach (string filePath in Directory.GetFiles(CurrentFolder)
                              .Where(path => new Regex($"(?:0X)?{new AFSHash(cdatapath).Value.ToString("X8")}(?:\\.\\w+)?$").IsMatch(Path.GetFileNameWithoutExtension(path)))
                              .ToArray())
                            {
                                // Already mapped.
                                if (!File.Exists(filePath))
                                    continue;

                                string NewfilePath = (CurrentFolder + $"/{cdatapath}").ToUpper();
                                string destinationDirectory = Path.GetDirectoryName(NewfilePath);

                                if (!string.IsNullOrEmpty(destinationDirectory) && !Directory.Exists(destinationDirectory))
                                    Directory.CreateDirectory(destinationDirectory.ToUpper());

                                if (!File.Exists(NewfilePath))
                                    File.Move(filePath, NewfilePath);
                            }
                        }

                        // Search for files with names matching the CRC hash, regardless of the extension
                        foreach (string filePath in Directory.GetFiles(CurrentFolder)
                          .Where(path => new Regex($"(?:0X)?{new AFSHash(prefix + text).Value.ToString("X8")}(?:\\.\\w+)?$").IsMatch(Path.GetFileNameWithoutExtension(path)))
                          .ToArray())
                        {
                            // Already mapped.
                            if (!File.Exists(filePath))
                                continue;

                            string NewfilePath = (CurrentFolder + $"/{text}").ToUpper();
                            string NewfilePathLow = NewfilePath.ToLower();
                            string destinationDirectory = Path.GetDirectoryName(NewfilePath);
                            string fileContent = File.ReadAllText(filePath);

                            if (!string.IsNullOrEmpty(destinationDirectory) && !Directory.Exists(destinationDirectory))
                                Directory.CreateDirectory(destinationDirectory.ToUpper());

                            if (!File.Exists(NewfilePath))
                                File.Move(filePath, NewfilePath);

                            if (NewfilePathLow.EndsWith(".mdl") || NewfilePathLow.EndsWith(".atmos")
                            || NewfilePathLow.EndsWith(".efx") || NewfilePathLow.EndsWith(".xml") || NewfilePathLow.EndsWith(".scene")
                            || NewfilePathLow.EndsWith(".map") || NewfilePathLow.EndsWith(".lua") || NewfilePathLow.EndsWith(".luac")
                            || NewfilePathLow.EndsWith(".unknown") || NewfilePathLow.EndsWith(".txt"))
                                await SubHashMapBatch(CurrentFolder, prefix, fileContent).ConfigureAwait(false);
                        }
                    }
                }
            }
        }

        public bool InitAFSMappedList(string MapperHelperFolder)
        {
            bool dirExists = Directory.Exists(MapperHelperFolder);
            if (!dirExists)
                return false;

            var MappedAFSHashesCache = new ConcurrentDictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            // Static entries (optimization from original code)
            MappedAFSHashesCache.TryAdd("4E545585", "\\PLACEHOLDER_N.DDS");
            MappedAFSHashesCache.TryAdd("4EE3523A", "\\PLACEHOLDER_S.DDS");
            MappedAFSHashesCache.TryAdd("696E72D6", "\\HATBUBBLE.DDS");
            MappedAFSHashesCache.TryAdd("D3A7AF9F", "\\__$manifest$__");
            MappedAFSHashesCache.TryAdd("EDFBFAE9", "\\FILES.TXT");

            var tasks = new List<Task>();

            string coreDataPath = Path.Combine(MapperHelperFolder, "core_data_mapper_helper.txt");
            if (File.Exists(coreDataPath))
            {
                tasks.Add(Task.Run(() =>
                {
                    foreach (var line in File.ReadLines(coreDataPath))
                    {
                        if (!line.Contains(':')) continue;
                        string[] elements = line.Split(':');
                        if (elements.Length == 2)
                            MappedAFSHashesCache.AddOrUpdate(elements[0], elements[1], (k, v) => elements[1]);
                    }
                }));
            }

            string sceneFilePath = Path.Combine(MapperHelperFolder, "scene_file_mapper_helper.txt");
            if (File.Exists(sceneFilePath))
            {
                tasks.Add(Task.Run(() =>
                {
                    foreach (var line in File.ReadLines(sceneFilePath))
                    {
                        if (!line.Contains(':')) continue;
                        string[] elements = line.Split(':');
                        if (elements.Length == 2)
                            MappedAFSHashesCache.AddOrUpdate(elements[0], elements[1], (k, v) => elements[1]);
                    }
                }));
            }

            string coredataXmlPath = Path.Combine(MapperHelperFolder, "CoredataHelper.xml");
            if (File.Exists(coredataXmlPath))
            {
                tasks.Add(Task.Run(() =>
                {
                    foreach (var match in new List<MappedList>(AFSRegexProcessor.ScanForString(File.ReadAllText(coredataXmlPath))))
                    {
                        if (string.IsNullOrEmpty(match.file)) continue;
                        string text = AFSHash.EscapeString(match.file);
                        MappedAFSHashesCache.AddOrUpdate(new AFSHash(text).Value.ToString("X8"), text, (k, v) => text);
                    }
                }));
            }

            string sceneListPath = Path.Combine(MapperHelperFolder, "SceneList.xml");
            if (File.Exists(sceneListPath))
            {
                tasks.Add(Task.Run(() =>
                {
                    foreach (var match in new List<MappedList>(AFSRegexProcessor.ScanForSceneListPaths(sceneListPath)))
                    {
                        if (string.IsNullOrEmpty(match.file)) continue;
                        string text = AFSHash.EscapeString(match.file);
                        MappedAFSHashesCache.AddOrUpdate(new AFSHash(text).Value.ToString("X8"), text, (k, v) => text);
                    }
                }));
            }

            Task.WaitAll(tasks.ToArray());

            this.MappedAFSHashesCache = MappedAFSHashesCache.ToDictionary(kv => kv.Key, kv => kv.Value);

            return true;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _ = Task.Run(() =>
                    {
                        MappedAFSHashesCache?.Clear();
                        MappedAFSHashesCache = null;
                    });
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Ne changez pas ce code. Placez le code de nettoyage dans la méthode 'Dispose(bool disposing)'
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
