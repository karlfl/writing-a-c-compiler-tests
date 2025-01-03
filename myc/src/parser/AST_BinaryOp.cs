namespace myc 
{
    public abstract class AST_BinaryOp : AST_Base
    {
        public abstract string Print();
    }

    public class AST_Add : AST_BinaryOp
    {
        public override string Print()
        {
            return "+";
        }
    }

    public class AST_Subtract : AST_BinaryOp
    {
        public override string Print()
        {
            return "-";
        }
    }

    public class AST_Multiply : AST_BinaryOp
    {
        public override string Print()
        {
            return "*";
        }
    }

    public class AST_Divide : AST_BinaryOp
    {
        public override string Print()
        {
            return @"/";
        }
    }

    public class AST_Mod : AST_BinaryOp
    {
        public override string Print()
        {
            return "%";
        }
    }

    public class AST_LogicalAnd : AST_BinaryOp
    {
        public override string Print()
        {
            return "&&";
        }
    }

    public class AST_LogicalOr : AST_BinaryOp
    {
        public override string Print()
        {
            return "||";
        }
    }

    public class AST_Equal : AST_BinaryOp
    {
        public override string Print()
        {
            return "=";
        }
    }

    public class AST_NotEqual : AST_BinaryOp
    {
        public override string Print()
        {
            return "!=";
        }
    }

    public class AST_LessThan : AST_BinaryOp
    {
        public override string Print()
        {
            return "<";
        }
    }

    public class AST_GreaterThan : AST_BinaryOp
    {
        public override string Print()
        {
            return ">";
        }
    }

    public class AST_LessOrEqual : AST_BinaryOp
    {
        public override string Print()
        {
            return "<=";
        }
    }

    public class AST_GreaterOrEqual : AST_BinaryOp
    {
        public override string Print()
        {
            return ">=";
        }
    }
}