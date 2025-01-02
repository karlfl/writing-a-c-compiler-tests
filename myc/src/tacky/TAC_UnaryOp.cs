namespace myc
{
    public abstract class TAC_UnaryOp { 
        public abstract string Print();
    }

    public class TAC_Complement : TAC_UnaryOp
    {

        public override string Print()
        {
            return "~";
        }

    }

    public class TAC_Negate : TAC_UnaryOp
    {

        public override string Print()
        {
            return "-";
        }
    }
}