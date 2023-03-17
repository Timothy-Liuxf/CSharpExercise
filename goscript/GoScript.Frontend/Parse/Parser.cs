using GoScript.Frontend.AST;
using GoScript.Frontend.Lex;

namespace GoScript.Frontend.Parse
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
                return new SingleStmt(expr, false);
            }
            else
            {
                tokens.MatchNewline();
                return new SingleStmt(expr, true);
            }
        }

        private Expression ParseExpression()
        {
            return ParseAddExpr();
        }

        private Expression ParseAddExpr()
        {
            var expr = ParsePrimaryExpr();
            while (tokens.TryMatchPunctuator(PunctuatorType.Add, out var op)
                || tokens.TryMatchPunctuator(PunctuatorType.Sub, out op))
            {
                var rExpr = ParsePrimaryExpr();
                expr = new AdditiveExpr(expr, rExpr, op!.Type switch
                {
                    PunctuatorType.Add => AdditiveExpr.OperatorType.Add,
                    PunctuatorType.Sub => AdditiveExpr.OperatorType.Sub,
                    _ => throw new InternalErrorException("Unexpected operator type."),
                },
                op.Location);
            }
            return expr;
        }

        private Expression ParsePrimaryExpr()
        {
            if (tokens.TryMatchIdentifier(out var identifier))
            {
                return new IdExpr(identifier!.Name, identifier.Location);
            }
            if (tokens.TryMatchLiteral(LiteralType.IntegerLiteral, out var literal))
            {
                var integerLiteral = (literal as IntegerLiteral)!;
                return new IntegerRValueExpr(integerLiteral.Value);
            }
            if (tokens.TryMatchPunctuator(PunctuatorType.LParen, out _))
            {
                var expr = ParseExpression();
                tokens.MatchPunctuator(PunctuatorType.RParen);
                return expr;
            }
            throw new NotImplementedException("ParsePrimaryExpr");
        }

        private VarDecl ParseVarDecl()
        {
            var location = tokens.MatchKeyword(KeywordType.Var).Location;
            var name = tokens.MatchIdentifier();
            if (tokens.TryMatchTypeKeyword(out var typeKeyword))
            {
                if (tokens.TryMatchPunctuator(PunctuatorType.Assign, out _))
                {
                    var initExpr = ParseExpression();
                    return new VarDecl(name.Name, Keyword.GetKeywordString(typeKeyword!.Type)!, initExpr, location);
                }
                return new VarDecl(name.Name, Keyword.GetKeywordString(typeKeyword!.Type)!, location);
            }
            {
                tokens.MatchPunctuator(PunctuatorType.Assign);
                var initExpr = ParseExpression();
                return new VarDecl(name.Name, initExpr, location);
            }
        }

        private readonly TokenReader tokens;

        public Parser(TokenReader tokens)
        {
            this.tokens = tokens;
        }
    }
}
