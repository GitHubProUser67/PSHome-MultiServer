using System.Text.RegularExpressions;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using MultiServerLibrary.Extension;

namespace MultiServerLibrary.HTTP
{
    public class UrlAnalyzer
    {
        private static object _Lock = new object();
        public static IEnumerable<FileSystemInfo> fileSystemCache = null;

        /// <summary>
        /// Generates a assending list of potential matching Urls from a given text.
        /// <para>Génère une liste d'Urls potentiel depuis un text donné.</para>
        /// <param name="inputUrl">The text to compare against.</param>
        /// <param name="HttpRootFolder">The root folder of the http server.</param>
        /// <param name="minSimilarity">The minimum cosine similarity threshold for including a URL in the suggestions.</param>
        /// <param name="MaxResults">The maximum amount of Urls in the result.</param>
        /// </summary>
        /// <returns>A list of nullable strings.</returns>
        public static List<string> GenerateUrlsSuggestions(string inputUrl, string HttpRootFolder,
            double minSimilarity, int MaxResults = 20)
        {
            if (fileSystemCache == null || string.IsNullOrEmpty(inputUrl) || string.IsNullOrEmpty(HttpRootFolder)
                || !fileSystemCache.Any())
                return null;

            HttpRootFolder = HttpRootFolder.Replace("\\", "/");

            // Calculate similarity between inputText and given list of Urls using cosine similarity
            return fileSystemCache
                .AsParallel()
                .AsUnordered()
                .WithDegreeOfParallelism(Environment.ProcessorCount)
#if NET5_0_OR_GREATER
                .Where(entry => inputUrl.Replace("\\", "/").Split('/').Any(segment => entry.FullName.Replace("\\", "/")
                .Contains(segment, StringComparison.InvariantCultureIgnoreCase)))
#else
                .Where(entry => inputUrl.Replace("\\", "/").Split('/').Any(segment => entry.FullName.ToUpper()
                .Replace("\\", "/").Contains(segment.ToUpper())))
#endif
                .Select(entry =>
                {
                    if (File.Exists(entry.FullName))
                        return entry.FullName.Replace("\\", "/").Replace(HttpRootFolder, string.Empty);
                    else if (Directory.Exists(entry.FullName))
                        return entry.FullName.Replace("\\", "/").Replace(HttpRootFolder, string.Empty) + "/";
                    else
                        return null;
                })
                .Where(url => url != null)
                .Select(url => new { Url = url, Similarity = EnhancedSimilarity(inputUrl, url) })
                .Where(x => x.Similarity >= minSimilarity) // Filter URLs based on similarity threshold
                .OrderByDescending(x => x.Similarity)
                .Select(x => x.Url)
                .Take(MaxResults)
                .ToList();
        }

        /// <summary>
        /// Searches for files in the fileSystemCache that contain a word similar to the given input word.
        /// </summary>
        /// <param name="inputWord">The word to search for.</param>
        /// <param name="minSimilarity">The minimum cosine similarity threshold for including a file in the results.</param>
        /// <param name="MaxResults">The maximum number of files in the result.</param>
        /// <returns>A list of nullable strings representing the paths of the matching files.</returns>
        public static List<string> SearchFilesContainingWordInPath(string inputWord, string extension,
            double minSimilarity, int MaxResults = 20)
        {
            if (fileSystemCache == null || string.IsNullOrEmpty(inputWord) || !fileSystemCache.Any())
                return null;

            // Prepare the extension filter, allowing for wildcards
#if NET5_0_OR_GREATER
            string normalizedExtension = extension.Trim().Replace("*", string.Empty);
#else
            string normalizedExtension = extension.Trim().ToLower().Replace("*", string.Empty);
#endif
            return fileSystemCache
                .AsParallel()
                .AsUnordered()
                .WithDegreeOfParallelism(Environment.ProcessorCount)
#if NET5_0_OR_GREATER
                .Where(x => x.FullName.EndsWith(normalizedExtension, StringComparison.InvariantCultureIgnoreCase))
#else
                .Where(x => x.FullName.ToLower().EndsWith(normalizedExtension))
#endif
                .Where(x => EnhancedSimilarity(x.FullName, inputWord) >= minSimilarity)
                .Select(x => x.FullName)
                .Take(MaxResults)
                .ToList();
        }

        /// <summary>
        /// Combines cosine similarity with Levenshtein similarity for enhanced similarity checking.
        /// </summary>
        /// <param name="text1">The first text.</param>
        /// <param name="text2">The second text.</param>
        /// <returns>The combined similarity score between the two texts.</returns>
        private static double EnhancedSimilarity(string text1, string text2)
        {
            return (CosineSimilarity(text1, text2) + LevenshteinSimilarity(text1, text2)) / 2.0;
        }

        /// <summary>
        /// Calculates the cosine between 2 string.
        /// <para>Calcule le Cosinus entre 2 string.</para>
        /// <param name="text1">The first text.</param>
        /// <param name="text2">The second text.</param>
        /// </summary>
        /// <returns>A double.</returns>
        public static double CosineSimilarity(string text1, string text2)
        {
            if (string.IsNullOrEmpty(text2)) return 0;

            // Tokenize text
            Regex regex = new Regex(@"\W+");

            // Calculate word frequency
            Dictionary<string, int> freq1 = regex.Split(text1.ToLower()).Where(token => token.Length > 0)
                .GroupBy(word => word).ToDictionary(g => g.Key, g => g.Count());
            Dictionary<string, int> freq2 = regex.Split(text2.ToLower()).Where(token => token.Length > 0)
                .GroupBy(word => word).ToDictionary(g => g.Key, g => g.Count());

            // Calculate magnitude
            double magnitude1 = Math.Sqrt(freq1.Values.Sum(count => count * count));
            double magnitude2 = Math.Sqrt(freq2.Values.Sum(count => count * count));

            // Calculate cosine similarity
            if (magnitude1 == 0 || magnitude2 == 0)
                return 0;

            // Calculate dot product and return.
            return freq1.Keys.Intersect(freq2.Keys).Sum(word => freq1[word] * freq2[word]) / (magnitude1 * magnitude2);
        }

        /// <summary>
        /// Calculates the Levenshtein similarity between two strings.
        /// </summary>
        /// <param name="text1">The first text.</param>
        /// <param name="text2">The second text.</param>
        /// <returns>The normalized Levenshtein similarity between the two texts.</returns>
        private static double LevenshteinSimilarity(string text1, string text2)
        {
            int maxLength = Math.Max(text1.Length, text2.Length);
            if (maxLength == 0) return 1.0; // Both strings are empty
            return 1.0 - (double)LevenshteinDistance(text1, text2) / maxLength;
        }

        /// <summary>
        /// Calculates the Levenshtein distance between two strings.
        /// </summary>
        /// <param name="text1">The first text.</param>
        /// <param name="text2">The second text.</param>
        /// <returns>The Levenshtein distance between the two texts.</returns>
        private static int LevenshteinDistance(string text1, string text2)
        {
            int[,] matrix = new int[text1.Length + 1, text2.Length + 1];

            for (int i = 0; i <= text1.Length; i++)
                matrix[i, 0] = i;

            for (int j = 0; j <= text2.Length; j++)
                matrix[0, j] = j;

            for (int i = 1; i <= text1.Length; i++)
            {
                for (int j = 1; j <= text2.Length; j++)
                {
                    int cost = text2[j - 1] == text1[i - 1] ? 0 : 1;
                    matrix[i, j] = new[] {
                        matrix[i - 1, j] + 1,
                        matrix[i, j - 1] + 1,
                        matrix[i - 1, j - 1] + cost
                    }.Min();
                }
            }

            return matrix[text1.Length, text2.Length];
        }

        public static bool IsAvailable()
        {
            return !Monitor.IsEntered(_Lock);
        }

        public static void ScheduledfileSystemUpdate(object state)
        {
            if (state != null)
            {
                string HttpRootFolder = (string)state;

                if (Directory.Exists(HttpRootFolder))
                {
                    CustomLogger.LoggerAccessor.LogWarn("[UrlAnalyzer] - Caching fileSystem structure, this might longer on very slow hard drives...");

                    lock (_Lock)
                        fileSystemCache = new DirectoryInfo(HttpRootFolder).AllFilesAndFoldersLinq();

                    CustomLogger.LoggerAccessor.LogInfo("[UrlAnalyzer] - fileSystem Cache has been actualized and is ready!");
                }
            }
        }
    }
}
