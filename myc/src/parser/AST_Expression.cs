
namespace myc {
    public class AST_Expression : AST_Base
    {
        int? Value;

        public AST_Expression(StringReader tokenStream) : base(tokenStream)
        {
            this.Value = null;
        }

        public override void Parse()
        {
            string? token = GetNextToken();
            string[] expr = token.Split(' ');
            if (
                expr.Length != 2 ||
                expr[0] != TokensEnum.Constant.ToString()
               )
            {
                throw new Exception("Invalid Expression Token");
            };

            //Parse as integer
            this.Value = int.Parse(expr[1]);
        }

        public override string Print()
        {
            return string.Format("{0}",this.Value);
        }
    }
}