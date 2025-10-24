using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HomeTools.AFS
{
    public class AFSRegexProcessor
    {
        public static ConcurrentBag<MappedList> ScanForSceneListPaths(string sourceFile)
        {
            return ReturnMappedListFromFile(new List<RegexPatterns>()
              {
                    new RegexPatterns() {
                        type = ".scene",
                        pattern = "(?<=\\b(?<=config=\"))[^\"]*.scene"
                    }
              }, sourceFile);
        }

        public static ConcurrentBag<MappedList> ScanForString(string sourceFileContent)
        {
            ConcurrentBag<RegexPatterns> regexPatternsList = new ConcurrentBag<RegexPatterns>();

            Parallel.ForEach(Regex.Matches(sourceFileContent, "(?<=\\b(?<=source=\"|src=\"|href=\"|file=\"|filename=\"|efx_filename=\"|texture\\s=\\s\"|spriteTexture\\s=\\s\"))[^\"]*\\.[^\\\"]*").Cast<Match>(),
                new ParallelOptions { MaxDegreeOfParallelism = Utils.ThreadLimiter.NumOfThreadsAvailable }, match => {
                string extension = Path.GetExtension(match.Value);
                string pattern = $"(?<=\\b(?<=source=\"|src=\"|href=\"|file=\"|filename=\"|efx_filename=\"|texture\\s=\\s\"|spriteTexture\\s=\\s\"))[^\"]*{extension}";
                // Check if any existing pattern matches pattern
                if (!regexPatternsList.Any(p => p.type == extension && p.pattern == pattern))
                {
                    regexPatternsList.Add(new RegexPatterns
                    {
                        type = extension,
                        pattern = pattern
                    });
                }
            });

            Parallel.ForEach(Regex.Matches(sourceFileContent, "([\\w-\\s]+\\\\)+[\\w-\\s]+\\.[\\w]+").Cast<Match>(), new ParallelOptions { MaxDegreeOfParallelism = Utils.ThreadLimiter.NumOfThreadsAvailable }, match => {
                string extension = Path.GetExtension(match.Value);
                string pattern = $"([\\w-\\s]+\\\\)+[\\w-\\s]+\\{extension}";
                // Check if any existing pattern matches pattern2
                if (!regexPatternsList.Any(p => p.type == extension && p.pattern == pattern))
                {
                    regexPatternsList.Add(new RegexPatterns
                    {
                        type = extension,
                        pattern = pattern
                    });
                }
            });

            return ReturnMappedList(regexPatternsList, sourceFileContent);
        }

        public static ConcurrentBag<MappedList> ReturnMappedListFromFile(List<RegexPatterns> regexPatternsList, string sourceFile)
        {
            string input = string.Empty;
            ConcurrentBag<MappedList> mappedListList = new ConcurrentBag<MappedList>();

            using (StreamReader streamReader = File.OpenText(sourceFile))
            {
                input = streamReader.ReadToEnd();
                streamReader.Close();
            }

            Parallel.ForEach(regexPatternsList, new ParallelOptions { MaxDegreeOfParallelism = Utils.ThreadLimiter.NumOfThreadsAvailable }, regexPatterns =>
            {
                if (!string.IsNullOrEmpty(regexPatterns.pattern))
                {
                    Parallel.ForEach(Regex.Matches(input, regexPatterns.pattern).OfType<Match>(), match => {
                        if (!mappedListList.Contains(new MappedList()
                        {
                            type = regexPatterns.type,
                            file = match.Value
                        }))
                            mappedListList.Add(new MappedList()
                            {
                                type = regexPatterns.type,
                                file = match.Value
                            });
                    });
                }
            });

            return mappedListList;
        }

        public static ConcurrentBag<MappedList> ReturnMappedList(ConcurrentBag<RegexPatterns> regexPatternsList, string sourceFileContent)
        {
            ConcurrentBag<MappedList> mappedListList = new ConcurrentBag<MappedList>();

            Parallel.ForEach(regexPatternsList, new ParallelOptions { MaxDegreeOfParallelism = Utils.ThreadLimiter.NumOfThreadsAvailable }, regexPatterns =>
            {
                if (!string.IsNullOrEmpty(regexPatterns.pattern))
                {
                    Parallel.ForEach(Regex.Matches(sourceFileContent, regexPatterns.pattern).OfType<Match>(), match => {
                        if (!mappedListList.Contains(new MappedList()
                        {
                            type = regexPatterns.type,
                            file = match.Value
                        }))
                            mappedListList.Add(new MappedList()
                            {
                                type = regexPatterns.type,
                                file = match.Value
                            });
                    });
                }
            });

            return mappedListList;
        }
    }

    public class MappedList
    {
        public string type;
        public string file;
    }

    public class RegexPatterns
    {
        public string type;
        public string pattern;
    }
}
