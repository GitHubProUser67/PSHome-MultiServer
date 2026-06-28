using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace DirectiveRewriter
{
    public class FrameworkDirectiveRewriter(
        IEnumerable<string> symbols,
        IEnumerable<string> ignored_symbols
    ) : CSharpSyntaxRewriter
    {
        private readonly DirectiveEvaluator _evaluator = new(symbols, ignored_symbols);

        private class IfState
        {
            public bool Active;
            public bool Taken;
            public bool SkipEntireBlock;
        }

        public override SyntaxNode Visit(SyntaxNode node)
        {
            if (node == null)
                return node;

            var text = node.ToFullString();
            var lines = text.Split('\n').ToList();
            var output = new List<string>();

            var stack = new Stack<IfState>();

            foreach (var line in lines)
            {
                var trimmed = line.Trim();

                if (trimmed.StartsWith("#if"))
                {
                    var expr = trimmed[3..].Trim();

                    bool skip = DirectiveEvaluator.ContainsIgnoredSymbol(expr);
                    bool result = !skip && _evaluator.Evaluate(expr);

                    stack.Push(
                        new IfState
                        {
                            SkipEntireBlock = skip,
                            Active = result,
                            Taken = result,
                        }
                    );

                    if (skip)
                        output.Add(line);

                    continue;
                }

                if (trimmed.StartsWith("#elif"))
                {
                    var state = stack.Pop();
                    var expr = trimmed[5..].Trim();

                    if (state.SkipEntireBlock || DirectiveEvaluator.ContainsIgnoredSymbol(expr))
                    {
                        state.SkipEntireBlock = true;
                        stack.Push(state);

                        output.Add(line);
                        continue;
                    }

                    if (state.Taken)
                    {
                        stack.Push(
                            new IfState
                            {
                                Active = false,
                                Taken = true,
                                SkipEntireBlock = false,
                            }
                        );
                    }
                    else
                    {
                        bool result = _evaluator.Evaluate(expr);

                        stack.Push(
                            new IfState
                            {
                                Active = result,
                                Taken = result,
                                SkipEntireBlock = false,
                            }
                        );
                    }

                    continue;
                }

                if (trimmed.StartsWith("#else"))
                {
                    var state = stack.Pop();

                    if (state.SkipEntireBlock)
                    {
                        stack.Push(state);
                        output.Add(line);
                        continue;
                    }

                    stack.Push(
                        new IfState
                        {
                            Active = !state.Taken,
                            Taken = true,
                            SkipEntireBlock = false,
                        }
                    );

                    continue;
                }

                if (trimmed.StartsWith("#endif"))
                {
                    var state = stack.Pop();

                    if (state.SkipEntireBlock)
                        output.Add(line);

                    continue;
                }

                // inside skipped block → copy verbatim ---
                if (stack.Any(s => s.SkipEntireBlock))
                {
                    output.Add(line);
                    continue;
                }

                if (stack.All(s => s.Active))
                    output.Add(line);
            }

            return SyntaxFactory.ParseSyntaxTree(string.Join("\n", output)).GetRoot();
        }
    }
}
