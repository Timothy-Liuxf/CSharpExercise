using JsonUtils.Frontend.AST;

namespace JsonUtils.Formatter
{
    public sealed class Formatter
    {
        public string Format()
        {
            return this.result ?? FormatOnce();
        }

        private string FormatOnce()
        {
            this.ast.Accept(this.helper);
            var eol = Environment.NewLine;
            var result = string.Join(eol, this.ast.StringAttribute!) + eol;
            return this.result = result;
        }

        public Formatter(ASTNode ast)
        {
            this.ast = ast;
            this.helper = new();
        }

        private ASTNode ast;
        private FormatHelper helper;
        private string? result;
    }
}
