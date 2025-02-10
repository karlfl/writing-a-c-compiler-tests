namespace myc
{
    public abstract class TAC_BinaryOp
    {
        public abstract string Print();
    }

    public class TAC_Add : TAC_BinaryOp
    {

        public override string Print()
        {
            return "+";
        }

    }

    public class TAC_Subtract : TAC_BinaryOp
    {

        public override string Print()
        {
            return "-";
        }
    }

    public class TAC_Multiply : TAC_BinaryOp
    {

        public override string Print()
        {
            return "*";
        }
    }
    public class TAC_Divide : TAC_BinaryOp
    {

        public override string Print()
        {
            return "/";
        }
    }
     public class TAC_Remainder : TAC_BinaryOp
    {

        public override string Print()
        {
            return "%";
        }
    }
    public class TAC_AND : TAC_BinaryOp
    {

        public override string Print()
        {
            return "&";
        }
    }
    public class TAC_OR : TAC_BinaryOp
    {

        public override string Print()
        {
            return "|";
        }
    }
    public class TAC_XOR : TAC_BinaryOp
    {

        public override string Print()
        {
            return "^";
        }
    }
    public class TAC_LeftShift : TAC_BinaryOp
    {

        public override string Print()
        {
            return "<<";
        }
    }
    public class TAC_RightShift : TAC_BinaryOp
    {

        public override string Print()
        {
            return ">>";
        }
    }
   public class TAC_Equal : TAC_BinaryOp
    {

        public override string Print()
        {
            return "=";
        }
    }
    public class TAC_NotEqual : TAC_BinaryOp
    {

        public override string Print()
        {
            return "!=";
        }
    }
    public class TAC_LessThan : TAC_BinaryOp
    {

        public override string Print()
        {
            return "<";
        }
    }
    public class TAC_LessOrEqual : TAC_BinaryOp
    {

        public override string Print()
        {
            return "<=";
        }
    }
    public class TAC_GreaterThan : TAC_BinaryOp
    {

        public override string Print()
        {
            return ">";
        }
    }
    public class TAC_GreaterOrEqual : TAC_BinaryOp
    {

        public override string Print()
        {
            return ">=";
        }
    }
}