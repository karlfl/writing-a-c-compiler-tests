namespace myc 
{
    public abstract class AST_BitwiseOp : AST_Base
    {
        public abstract string Print();
    }

    public class AST_AND : AST_BitwiseOp
    {
        public override string Print()
        {
            return "&";
        }
    }

    public class AST_OR : AST_BitwiseOp
    {
        public override string Print()
        {
            return "|";
        }
    }

    public class AST_XOR : AST_BitwiseOp
    {
        public override string Print()
        {
            return "^";
        }
    }

    public class AST_LeftShift : AST_BitwiseOp
    {
        public override string Print()
        {
            return "<<";
        }
    }

    public class AST_RightShift : AST_BitwiseOp
    {
        public override string Print()
        {
            return ">>";
        }
    }
}