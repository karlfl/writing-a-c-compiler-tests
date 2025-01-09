using System.Text;

namespace myc
{
    public abstract class AST_Statement : AST_Base
    {
        public abstract string Print();
    }
    public class AST_Return(AST_Factor expression) : AST_Statement
    {
        public readonly AST_Factor Expression = expression;

        public override string Print()
        {
            StringBuilder output = new();
            output.AppendLine("Return(");
            output.AppendLine(string.Format("            {0}", this.Expression.Print()));
            output.AppendLine("        )");
            return output.ToString();
        }
    }

    public class AST_Expression(AST_Factor expression) : AST_Statement
    {
        public readonly AST_Factor Expression = expression;

        public override string Print()
        {
            StringBuilder output = new();
            output.AppendLine("Return(");
            output.AppendLine(string.Format("            {0}", this.Expression.Print()));
            output.AppendLine("        )");
            return output.ToString();
        }
    }

    public class AST_Null() : AST_Statement {
        public override string Print()
        {
            return "null";
        }
    }
}