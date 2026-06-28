namespace DirectiveRewriter
{
    public class DirectiveEvaluator
    {
        private readonly HashSet<string> _symbols;

        private static readonly List<string> IgnoredSymbols = ["DEBUG", "TRACE", "true", "false"];

        public DirectiveEvaluator(IEnumerable<string> symbols, IEnumerable<string> ignored_symbols)
        {
            _symbols = [.. symbols];
            IgnoredSymbols.AddRange(ignored_symbols);
        }

        public static bool ContainsIgnoredSymbol(string expression)
        {
            return Tokenize(expression).Any(IgnoredSymbols.Contains);
        }

        public bool Evaluate(string expression)
        {
            var tokens = Tokenize(expression);
            int index = 0;
            return ParseOr(tokens, ref index);
        }

        private bool ParseOr(List<string> tokens, ref int i)
        {
            bool left = ParseAnd(tokens, ref i);
            while (i < tokens.Count && tokens[i] == "||")
            {
                i++;
                left = left || ParseAnd(tokens, ref i);
            }
            return left;
        }

        private bool ParseAnd(List<string> tokens, ref int i)
        {
            bool left = ParseUnary(tokens, ref i);
            while (i < tokens.Count && tokens[i] == "&&")
            {
                i++;
                left = left && ParseUnary(tokens, ref i);
            }
            return left;
        }

        private bool ParseUnary(List<string> tokens, ref int i)
        {
            if (tokens[i] == "!")
            {
                i++;
                return !ParsePrimary(tokens, ref i);
            }
            return ParsePrimary(tokens, ref i);
        }

        private bool ParsePrimary(List<string> tokens, ref int i)
        {
            if (tokens[i] == "(")
            {
                i++;
                var val = ParseOr(tokens, ref i);
                i++; // skip ')'
                return val;
            }

            return _symbols.Contains(tokens[i++]);
        }

        private static List<string> Tokenize(string expr)
        {
            return
            [
                .. expr.Replace("(", " ( ")
                    .Replace(")", " ) ")
                    .Replace("&&", " && ")
                    .Replace("||", " || ")
                    .Replace("!", " ! ")
                    .Split(' ', StringSplitOptions.RemoveEmptyEntries),
            ];
        }
    }
}
