using System.Text;

namespace myc
{
    public class AST_Statement : AST_Base
    {
        public readonly TokensEnum Instruction;
        public readonly AST_Expression Expression;


        public AST_Statement(TokensEnum instruction, AST_Expression expression)
        {
            this.Instruction = instruction;
            this.Expression = expression;
        }

        public string Print()
        {
            StringBuilder output = new();
            output.AppendLine("Return(");
            output.AppendLine(string.Format("            {0}",this.Expression.Print()));
            output.AppendLine("        )");
            return output.ToString();
        }
    }
}