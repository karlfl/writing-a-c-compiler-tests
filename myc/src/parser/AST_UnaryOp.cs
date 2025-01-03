namespace myc
{
    public abstract class AST_UnaryOp : AST_Base
    {
        public abstract string Print();

    }


    public class AST_Complement : AST_UnaryOp
    {
        public override string Print()
        {
            return "~";
        }

    }

    public class AST_Negate : AST_UnaryOp
    {
        public override string Print()
        {
            return "-";
        }
    }

    public class AST_Not : AST_UnaryOp
    {
        public override string Print()
        {
            return "!";
        }
    }}