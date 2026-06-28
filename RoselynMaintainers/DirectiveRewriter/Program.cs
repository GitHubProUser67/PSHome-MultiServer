using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;

namespace DirectiveRewriter
{
    internal partial class Program
    {
        private static readonly List<string> _IgnoredDirectives = [];

        static async Task Main()
        {
            var solutionPath = FindSolutionFile();

            if (solutionPath == null)
            {
                Console.WriteLine("No .sln or .slnx file found in parent directories.");
                return;
            }

            Console.WriteLine($"Using solution: {solutionPath}");

            LoadIgnoredDirectivesFromSolution(solutionPath);

            Console.WriteLine("Ignored directives:");
            Console.WriteLine(string.Join(", ", _IgnoredDirectives));

            var workspace = MSBuildWorkspace.Create();
            var solution = await workspace.OpenSolutionAsync(solutionPath).ConfigureAwait(false);

            Console.WriteLine($"Projects found: {solution.Projects.Count()}");

            var currentAssemblyPath = Assembly.GetExecutingAssembly().Location;

            foreach (var project in solution.Projects)
            {
                if (project.Language != LanguageNames.CSharp)
                    continue;

                if (project.FilePath == null)
                    continue;

                if (IsCurrentProject(project, currentAssemblyPath))
                {
                    Console.WriteLine($"Skipping self project: {project.Name}");
                    continue;
                }

                await ProcessProject(project).ConfigureAwait(false);
            }

            Console.WriteLine("Done.");
        }

        static string? FindSolutionFile()
        {
            var dir = new DirectoryInfo(AppContext.BaseDirectory);

            while (dir != null)
            {
                var sln = dir.GetFiles("*.sln").FirstOrDefault();
                if (sln != null)
                    return sln.FullName;

                dir = dir.Parent;
            }

            dir = new DirectoryInfo(AppContext.BaseDirectory);

            while (dir != null)
            {
                var sln = dir.GetFiles("*.slnx").FirstOrDefault();
                if (sln != null)
                    return sln.FullName;

                dir = dir.Parent;
            }

            return null;
        }

        static async Task ProcessProject(Project project)
        {
            var symbols = project.ParseOptions?.PreprocessorSymbolNames ?? [];

            Console.WriteLine($"Processing project: {project.Name}");

            foreach (var document in project.Documents)
            {
                if (document.SourceCodeKind != SourceCodeKind.Regular)
                    continue;

                var root = await document.GetSyntaxRootAsync().ConfigureAwait(false);
                if (root == null)
                    continue;

                if (document.FilePath != null)
                {
                    var originalText = root.ToFullString();

                    var newText = new FrameworkDirectiveRewriter(symbols, _IgnoredDirectives)
                        .Visit(root)
                        .ToFullString();

                    if (!string.Equals(originalText, newText, StringComparison.Ordinal))
                    {
                        File.WriteAllText(document.FilePath, newText);
                        Console.WriteLine($"Processed: {document.FilePath}");
                    }
                }
            }
        }

        static bool IsCurrentProject(Project project, string assemblyPath)
        {
            var assemblyDir = Path.GetDirectoryName(assemblyPath);
            if (assemblyDir == null)
                return false;

            var projectDir = Directory.GetParent(assemblyDir)?.Parent?.Parent?.FullName;
            if (projectDir == null)
                return false;

            return Path.GetFullPath(project.FilePath)
                .StartsWith(projectDir, StringComparison.OrdinalIgnoreCase);
        }

        static void LoadIgnoredDirectivesFromSolution(string solutionPath)
        {
            var solutionDir = Path.GetDirectoryName(solutionPath);
            if (solutionDir == null)
                return;

            foreach (
                var csproj in Directory.GetFiles(
                    solutionDir,
                    "*.csproj",
                    SearchOption.AllDirectories
                )
            )
                ExtractDefineConstants(File.ReadAllText(csproj));
        }

        static void ExtractDefineConstants(string csprojText)
        {
            foreach (Match comment in MyRegex().Matches(csprojText))
                ExtractFromPattern(comment.Groups[1].Value);

            ExtractFromPattern(csprojText);
        }

        static void ExtractFromPattern(string text)
        {
            var match = MyRegex1().Match(text);

            if (!match.Success)
                return;

            var values = match.Groups[1].Value;

            foreach (var symbol in values.Split(';', StringSplitOptions.RemoveEmptyEntries))
            {
                var trimmed = symbol.Trim();

                // ignore MSBuild variables like $(DefineConstants)
                if (trimmed.StartsWith("$(") && trimmed.EndsWith(")"))
                    continue;

                // basic safety filter
                if (string.IsNullOrWhiteSpace(trimmed))
                    continue;

                if (!_IgnoredDirectives.Contains(trimmed))
                    _IgnoredDirectives.Add(trimmed);
            }
        }

        [GeneratedRegex(@"<!--([\s\S]*?)-->")]
        private static partial Regex MyRegex();

        [GeneratedRegex(@"<DefineConstants>(.*?)</DefineConstants>")]
        private static partial Regex MyRegex1();
    }
}
