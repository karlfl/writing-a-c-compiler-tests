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
            return string.Format("return({0})", Expression.Print());
        }
    }

    public class AST_Expression(AST_Factor expression) : AST_Statement
    {
        public readonly AST_Factor Expression = expression;

        public override string Print()
        {
            return string.Format("{0}", Expression.Print());
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
            output.AppendLine(string.Format("if ({0})", Condition.Print()));
            output.AppendLine(string.Format("{{\n{0}\n}}", ThenStatement.Print()));
            if (ElseStatement != null)
            {
                output.AppendLine("else");
                output.AppendLine(string.Format("{{\n{0}\n}}", ElseStatement.Print()));
            }
            return output.ToString();
        }
    }

    public class AST_Compound(AST_Block block) : AST_Statement
    {
        public readonly AST_Block Block = block;
        public override string Print()
        {
            return string.Format("{0}", Block.Print());
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