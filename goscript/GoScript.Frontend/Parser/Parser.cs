using GoScript.Frontend.AST;
using GoScript.Frontend.Lexer;

namespace GoScript.Frontend.Parser
{
    internal class Parser
    {
        private bool parsed = false;

        public IEnumerable<ASTNode> Parse()
        {
            if (parsed)
            {
                throw new InternalErrorException("Already parsed.");
            }

            parsed = true;
            while (tokens.CurrentToken != null)
            {
                yield return ParseStatement();
            }
        }

        private Statement ParseStatement()
        {
            if (tokens.TryMatchPunctuator(PunctuatorType.Semicolon, out _))
            {
                tokens.MatchNewline();
                return new EmptyStmt();
            }

            if (tokens.TryMatchNewline(out _))
            {
                return new EmptyStmt();
            }

            if (tokens.TryPeekKeyword(KeywordType.Var, out _))
            {
                var varDecl = ParseVarDecl();
                tokens.TryMatchPunctuator(PunctuatorType.Semicolon, out _);
                tokens.MatchNewline();
                return varDecl;
            }

            var expr = ParseExpression();
            if (tokens.TryMatchPunctuator(PunctuatorType.Semicolon, out _))
            {
                tokens.MatchNewline();
                return new SingleStmt(expr, true);
            }
            else
            {
                tokens.MatchNewline();
                return new SingleStmt(expr, false);
            }
        }

        private Expression ParseExpression()
        {
            return ParseAddExpr();
        }

        private Expression ParseAddExpr()
        {
            var expr = ParsePrimaryExpr();
            while (tokens.TryMatchPunctuator(PunctuatorType.Add, out _))
            {
                var rExpr = ParsePrimaryExpr();
                expr = new AddExpr(expr, rExpr);
            }
            return expr;
        }

        private Expression ParsePrimaryExpr()
        {
            if (tokens.TryMatchIdentifier(out var identifier))
            {
                return new IdExpr(identifier!.Name);
            }
            if (tokens.TryMatchLiteral(LiteralType.IntegerLiteral, out var literal))
            {
                var integerLiteral = (literal as IntegerLiteral)!;
                return new IntegerRValueExpr(integerLiteral.Value);
            }
            throw new NotImplementedException("ParsePrimaryExpr");
        }

        private VarDecl ParseVarDecl()
        {
            tokens.MatchKeyword(KeywordType.Var);
            var name = tokens.MatchIdentifier();
            if (tokens.TryMatchTypeKeyword(out var typeKeyword))
            {
                if (tokens.TryMatchPunctuator(PunctuatorType.Assign, out _))
                {
                    var initExpr = ParseExpression();
                    return new VarDecl(name.Name, Keyword.GetKeywordString(typeKeyword!.Type)!, initExpr);
                }
                return new VarDecl(name.Name, Keyword.GetKeywordString(typeKeyword!.Type)!);
            }
            {
                tokens.MatchPunctuator(PunctuatorType.Assign);
                var initExpr = ParseExpression();
                return new VarDecl(name.Name, initExpr);
            }
        }

        private readonly TokenReader tokens;

        public Parser(TokenReader tokens)
        {
            this.tokens = tokens;
        }
    }
}
