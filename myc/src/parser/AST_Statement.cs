using System.Runtime.CompilerServices;
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
            return string.Format("return({0})", this.Expression.Print());
        }
    }

    public class AST_Expression(AST_Factor expression) : AST_Statement
    {
        public readonly AST_Factor Expression = expression;

        public override string Print()
        {
            return string.Format("{0}", this.Expression.Print());
        }
    }

    public class AST_If(AST_Factor condition, AST_Statement thenStmt, AST_Statement? elseStmt) : AST_Statement
    {
        public readonly AST_Factor Condition = condition;
        public readonly AST_Statement ThenStatement = thenStmt;
        public readonly AST_Statement? ElseStatement = elseStmt;
        public override string Print()
        {
            StringBuilder output = new();
            output.AppendLine(string.Format("if ({0})", this.Condition.Print()));
            output.AppendLine(string.Format("{{\n{0}\n}}", this.ThenStatement.Print()));
            if (ElseStatement != null)
            {
                output.AppendLine("else");
                output.AppendLine(string.Format("{{\n{0}\n}}", this.ElseStatement.Print()));
            }
            return output.ToString();
        }
    }

    public class AST_Null() : AST_Statement
    {
        public override string Print()
        {
            return "null";
        }
    }
}