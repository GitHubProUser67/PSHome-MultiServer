using CustomLogger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAPIService
{
    public static class CSVUtils
    {
        /// <summary>
        /// Appends or updates CSV lines in the file.
        /// If a line with matching clanId (1st column) AND username (4th column) exists,
        /// it will be replaced with the first provided csvLine. Additional lines are appended.
        /// Automatically writes the header line only if the file is new or empty.
        /// </summary>
        /// <param name="filePath">Full path to the CSV file.</param>
        /// <param name="csvLines">One or more complete CSV lines to add/update.</param>
        /// <param name="headerLine">Optional: The header line to write if the file is new/empty.</param>
        public static async Task AppendOrUpdateCsvLinesAsync(
            string filePath,
            IEnumerable<string> csvLines,
            bool delete,
            string headerLine = null)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("File path cannot be null or empty.", nameof(filePath));
            if (csvLines == null)
                throw new ArgumentNullException(nameof(csvLines));
            
            var inputLines = csvLines
                .Where(line => line != null)
                .Select(line => line.TrimEnd('\r', '\n'))
                .Where(line => !string.IsNullOrWhiteSpace(line))
                .ToList();

            if (inputLines.Count == 0)
                return;

            bool updated = false;
            string targetClanId = string.Empty;
            string targetClanName = string.Empty;
            string targetUsername = string.Empty;

            Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);

            string[] allLines = Array.Empty<string>();
            bool fileExists = File.Exists(filePath);
            bool isNewFile = !fileExists || new FileInfo(filePath).Length == 0;

            if (!isNewFile)
                // Read all existing lines
                allLines = await File.ReadAllLinesAsync(filePath, Encoding.UTF8).ConfigureAwait(false);

            var resultLines = new List<string>(allLines);

            // Take the first input line for potential update (the one containing clanId and username)
            string primaryLine = inputLines[0];
            var primaryFields = ParseCsvLine(primaryLine);

            switch (filePath.Split("/").LastOrDefault())
            {
                case "ClanMembersList.csv":
                    {
                        targetClanId = primaryFields[0].Trim();
                        targetUsername = primaryFields[3].Trim();

                        for (int i = 0; i < resultLines.Count; i++)
                        {
                            // Skip header if you have one (optional – adjust if your header shouldn't be checked)
                            var fields = ParseCsvLine(resultLines[i]);
                            if (fields.Length >= 4 &&
                                fields[0].Trim() == targetClanId &&
                                fields[3].Trim() == targetUsername)
                            {
                                // Replace the existing line
                                resultLines[i] = primaryLine;
                                updated = true;
                                break;
                            }
                        }

                        if (!updated)
                        {
                            // No match found → append the primary line
                            resultLines.Add(primaryLine);
                        }
                    }
                    break;
                case "ClanList.csv":
                    targetClanId = primaryFields[0].Trim();
                    targetClanName = primaryFields[1].Trim();

                    for (int i = 0; i < resultLines.Count; i++)
                    {
                        // Skip header if you have one (optional – adjust if your header shouldn't be checked)
                        var fields = ParseCsvLine(resultLines[i]);
                        if (fields.Length >= 4 &&
                            fields[0].Trim() == targetClanId &&
                            fields[1].Trim() == targetClanName)
                        {
                            // Replace the existing line
                            resultLines[i] = primaryLine;
                            updated = true;
                            break;
                        }
                    }

                    if (!updated)
                        // No match found → append the primary line
                        resultLines.Add(primaryLine);
                    break;
                case "ClanBlacklist.csv":
                    targetClanId = primaryFields[0].Trim();
                    targetUsername = primaryFields[1].Trim();

                    for (int i = 0; i < resultLines.Count; i++)
                    {
                        // Skip header if you have one (optional – adjust if your header shouldn't be checked)
                        var fields = ParseCsvLine(resultLines[i]);
                        if (fields.Length >= 4 &&
                            fields[0].Trim() == targetClanId &&
                            fields[1].Trim() == targetUsername)
                        {
                            if (delete)
                                resultLines[i] = string.Empty;
                            else
                                resultLines[i] = primaryLine;

                            updated = true;
                            break;
                        }
                    }

                    if (!updated)
                        // No match found → append the primary line
                        resultLines.Add(primaryLine);
                    break;
                default:
                    // Fallback: if primary line doesn't have enough fields, just append it
                    resultLines.Add(primaryLine); break;
            }

            // Append any additional lines beyond the first one
            for (int i = 1; i < inputLines.Count; i++)
                resultLines.Add(inputLines[i]);

            // Write header only if file is brand new and header provided
            if (isNewFile && !string.IsNullOrWhiteSpace(headerLine))
                resultLines.Insert(0, headerLine.TrimEnd('\r', '\n'));

            try
            {
                await File.WriteAllLinesAsync(filePath, resultLines, Encoding.UTF8).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to write CSV file '{filePath}': {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Simple CSV line parser that handles quoted fields and commas inside quotes.
        /// Does not use a full CSV library to keep dependencies minimal.
        /// </summary>
        private static string[] ParseCsvLine(string line)
        {
            var fields = new List<string>();
            bool inQuotes = false;
            var field = new StringBuilder();

            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];

                if (c == '"')
                {
                    if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
                    {
                        // Escaped quote
                        field.Append('"');
                        i++; // skip next quote
                    }
                    else
                        inQuotes = !inQuotes;
                }
                else if (c == ',' && !inQuotes)
                {
                    fields.Add(field.ToString());
                    field.Clear();
                }
                else
                    field.Append(c);
            }

            fields.Add(field.ToString()); // last field
            return fields.Select(f => f.Trim()).ToArray();
        }

        /// <summary>
        /// Convenience overload: Append a single pre-formatted CSV line.
        /// </summary>
        public static Task AppendCsvLineAsync(string filePath, string csvLine, bool delete, string headerLine = null)
        {
            return AppendOrUpdateCsvLinesAsync(filePath, new[] { csvLine }, delete, headerLine);
        }

        /// <summary>
        /// Synchronous wrappers (use async where possible to avoid deadlocks)
        /// </summary>
        public static void AppendCsvLines(string filePath, IEnumerable<string> csvLines, bool delete, string headerLine = null)
        {
            AppendOrUpdateCsvLinesAsync(filePath, csvLines, delete, headerLine).GetAwaiter().GetResult();
        }

        public static void AppendCsvLine(string filePath, string csvLine, bool delete, string headerLine = null)
        {
            AppendCsvLineAsync(filePath, csvLine, delete, headerLine).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Reads all lines from CSV
        /// </summary>
        public static async Task<string[]> ReadAllLinesAsync(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                    return Array.Empty<string>();

                return await File.ReadAllLinesAsync(filePath).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError(ex, "Failed to read CSV: {FilePath}", filePath);
            }

            return Array.Empty<string>();
        }
    }
}
