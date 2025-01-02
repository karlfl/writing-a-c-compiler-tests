namespace myc
{
    public abstract class TAC_BinaryOp { 
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
    }}