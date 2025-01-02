namespace myc
{
    public abstract class TAC_Instruction
    {

        public abstract string Print();
    }

    public class TAC_Return(TAC_Value Value) : TAC_Instruction
    {
        public readonly TAC_Value Value = Value;

        public override string Print()
        {
            return string.Format("    return {0}", this.Value.Print());
        }
    }

    public class TAC_Unary(TAC_UnaryOp unaryOp, TAC_Value source, TAC_Value destination) : TAC_Instruction
    {
        public readonly TAC_UnaryOp UnaryOp = unaryOp;
        public readonly TAC_Value Source = source;
        public readonly TAC_Value Destination = destination;

        public override string Print()
        {
            return string.Format("    {0} = {1}{2}", this.Destination.Print(), this.UnaryOp.Print(), this.Source.Print());
        }
    }
}