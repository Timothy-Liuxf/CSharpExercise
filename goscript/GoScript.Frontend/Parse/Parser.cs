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

            if (tokens.TryPeekPunctuator(PunctuatorType.LBrace, out _))
            {
                return ParseCompoundStmt();
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
            return ParseLogicalOrExpr();
        }

        private Expression ParseLogicalOrExpr()
        {
            var expr = ParseLogicalAndExpr();
            while (tokens.TryMatchPunctuator(PunctuatorType.Or, out var op))
            {
                var rExpr = ParseLogicalAndExpr();
                expr = new LogicalOrExpr(expr, rExpr, op!.Location);
            }
            return expr;
        }

        private Expression ParseLogicalAndExpr()
        {
            var expr = ParseAdditiveExpr();
            while (tokens.TryMatchPunctuator(PunctuatorType.And, out var op))
            {
                var rExpr = ParseAdditiveExpr();
                expr = new LogicalAndExpr(expr, rExpr, op!.Location);
            }
            return expr;
        }

        private Expression ParseAdditiveExpr()
        {
            var expr = ParseMultiplicativeExpr();
            while (tokens.TryMatchPunctuator(PunctuatorType.Add, out var op)
                || tokens.TryMatchPunctuator(PunctuatorType.Sub, out op))
            {
                var rExpr = ParseMultiplicativeExpr();
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

        private Expression ParseMultiplicativeExpr()
        {
            var expr = ParseUnaryExpr();
            while (tokens.TryMatchPunctuator(PunctuatorType.Star, out var op)
                || tokens.TryMatchPunctuator(PunctuatorType.Div, out op)
                || tokens.TryMatchPunctuator(PunctuatorType.Mod, out op))
            {
                var rExpr = ParseUnaryExpr();
                expr = new MultiplicativeExpr(expr, rExpr, op!.Type switch
                {
                    PunctuatorType.Star => MultiplicativeExpr.OperatorType.Mul,
                    PunctuatorType.Div => MultiplicativeExpr.OperatorType.Div,
                    PunctuatorType.Mod => MultiplicativeExpr.OperatorType.Mod,
                    _ => throw new InternalErrorException("Unexpected operator type."),
                },
                op.Location);
            }
            return expr;
        }

        private Expression ParseUnaryExpr()
        {
            if (tokens.TryMatchPunctuator(PunctuatorType.Sub, out var negOp))
            {
                var expr = ParseUnaryExpr();
                return new UnaryExpr(UnaryExpr.OperatorType.Neg, expr, negOp!.Location);
            }
            if (tokens.TryMatchPunctuator(PunctuatorType.Not, out var notOp))
            {
                var expr = ParseUnaryExpr();
                return new UnaryExpr(UnaryExpr.OperatorType.Not, expr, notOp!.Location);
            }
            return ParsePrimaryExpr();
        }

        private Expression ParsePrimaryExpr()
        {
            if (tokens.TryMatchIdentifier(out var identifier))
            {
                return new IdExpr(identifier!.Name, identifier.Location);
            }
            if (tokens.TryMatchLiteral(LiteralType.IntegerLiteral, out var integerLiteral))
            {
                return new IntegerConstantExpr((integerLiteral as IntegerLiteral)!.Value);
            }
            if (tokens.TryMatchLiteral(LiteralType.BoolLiteral, out var boolLiteral))
            {
                return new BoolConstantExpr((boolLiteral as BoolLiteral)!.Value);
            }
            if (tokens.TryMatchPunctuator(PunctuatorType.LParen, out _))
            {
                var expr = ParseExpression();
                tokens.MatchPunctuator(PunctuatorType.RParen);
                return expr;
            }
            throw new NotImplementedException($"ParsePrimaryExpr at {tokens.CurrentToken?.Location.ToString() ?? "EOF"}.");
        }

        private VarDecl ParseVarDecl()
        {
            var location = tokens.MatchKeyword(KeywordType.Var).Location;
            var identifiers = new List<string>() { tokens.MatchIdentifier().Name };
            while (tokens.TryMatchPunctuator(PunctuatorType.Comma, out _))
            {
                identifiers.Add(tokens.MatchIdentifier().Name);
            }

            Keyword? typeKeyword;
            if (!tokens.TryMatchTypeKeyword(out typeKeyword)
                || tokens.TryMatchPunctuator(PunctuatorType.Assign, out _))
            {
                if (typeKeyword is null)
                {
                    tokens.MatchPunctuator(PunctuatorType.Assign);
                }

                var initExprs = new List<Expression>() { ParseExpression() };
                while (tokens.TryMatchPunctuator(PunctuatorType.Comma, out _))
                {
                    initExprs.Add(ParseExpression());
                }

                if (typeKeyword is null)
                {
                    return new VarDecl(identifiers, initExprs, location);
                }
                else
                {
                    return new VarDecl(identifiers, Keyword.GetKeywordString(typeKeyword.Type)!, initExprs, location);
                }
            }
            return new VarDecl(identifiers, Keyword.GetKeywordString(typeKeyword!.Type)!, location);
        }

        private CompoundStmt ParseCompoundStmt()
        {
            var location = tokens.MatchPunctuator(PunctuatorType.LBrace).Location;
            tokens.MatchNewline();
            var statements = new List<Statement>();
            while (!tokens.TryMatchPunctuator(PunctuatorType.RBrace, out _))
            {
                statements.Add(ParseStatement());
            }
            tokens.MatchNewline();
            return new CompoundStmt(statements, location);
        }

        private readonly TokenReader tokens;

        public Parser(TokenReader tokens)
        {
            this.tokens = tokens;
        }
    }
}
