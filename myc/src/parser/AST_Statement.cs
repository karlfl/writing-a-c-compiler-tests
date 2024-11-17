

using System.Text;

namespace myc
{
    public class AST_Statement : AST_Base
    {
        AST_Expression expression;


        public AST_Statement(StringReader tokenStream) : base(tokenStream)
        {
            this.expression = new(tokenStream);
        }

        public override void Parse(){
            Expect(TokensEnum.KWReturn);
            this.expression.Parse();
            Expect(TokensEnum.Semicolon);
        }

        public override string Print()
        {
            StringBuilder output = new();
            output.AppendLine("Return(");
            output.AppendLine(string.Format("\t\t\tConstant({0})",this.expression.Print()));
            output.AppendLine("\t\t)");
            return output.ToString();
        }
    }
}