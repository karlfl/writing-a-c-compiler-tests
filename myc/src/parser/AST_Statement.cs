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

    public class AST_Break(string id) : AST_Statement
    {
        public readonly string Identifier = id;
        public override string Print()
        {
            return string.Format("break-{0};",Identifier);
        }
    }

    public class AST_Continue(string id) : AST_Statement
    {
        public readonly string Identifier = id;
        public override string Print()
        {
            return string.Format("continue-{0};",Identifier);
        }
    }

    public class AST_While(AST_Factor condition, AST_Statement body, string id) : AST_Statement
    {
        public readonly AST_Factor Condition = condition;
        public readonly AST_Statement Body = body;
        public readonly string Identifier = id;
        public override string Print()
        {
            StringBuilder output = new();
            output.AppendLine(string.Format("while-{0} ({1})", Identifier, Condition.Print()));
            output.AppendLine(string.Format("{{\n{0}\n}}", Body.Print()));
            return output.ToString();
        }
    }

    public class AST_DoWhile(AST_Statement body, AST_Factor condition, string id) : AST_Statement
    {
        public readonly AST_Statement Body = body;
        public readonly AST_Factor Condition = condition;
        public readonly string Identifier = id;
        public override string Print()
        {
            StringBuilder output = new();
            output.AppendLine(string.Format("do-{0}\n{{\n{1}\n}}", Identifier, Body.Print()));
            output.AppendLine(string.Format("while ({0})", Condition.Print()));
            return output.ToString();
        }
    }

    public class AST_For(AST_ForInit initialization, AST_Factor? condition, AST_Factor? post, AST_Statement body, string id) : AST_Statement
    {
        public readonly AST_ForInit Initialization = initialization;
        public readonly AST_Factor? Condition = condition;
        public readonly AST_Factor? Post = post;
        public readonly AST_Statement Body = body;
        public readonly string Identifier = id;
        public override string Print()
        {
            StringBuilder output = new();
            output.AppendLine(string.Format("for-{0} ({1}, {2}, {3}", Identifier, Initialization.Print(), Condition?.Print(), Post?.Print()));
            output.AppendLine(string.Format("{{\n{0}\n}}", Body.Print()));
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