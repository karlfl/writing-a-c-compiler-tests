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

    public class TAC_Binary(TAC_BinaryOp binaryOp, TAC_Value source1, TAC_Value source2, TAC_Value destination) : TAC_Instruction
    {
        public readonly TAC_BinaryOp BinaryOp = binaryOp;
        public readonly TAC_Value Source1 = source1;
        public readonly TAC_Value Source2 = source2;
        public readonly TAC_Value Destination = destination;

        public override string Print()
        {
            return string.Format("    {0} = ({1} {2} {3})", this.Destination.Print(), this.Source1.Print(), this.BinaryOp.Print(), this.Source2.Print() );
        }
    }

    public class TAC_Copy(TAC_Value source, TAC_Value destination) : TAC_Instruction
    {
        public readonly TAC_Value Source = source;
        public readonly TAC_Value Destination = destination;

        public override string Print()
        {
            return string.Format("    Copy ({0}, {1})", this.Source.Print(), this.Destination.Print() );
        }
    }

    public class TAC_Jump(TAC_Label target) : TAC_Instruction
    {
        public readonly TAC_Label Target = target;

        public override string Print()
        {
            return string.Format("    JumpZero {0})", this.Target.Print() );
        }
    }


    public class TAC_JumpIfZero(TAC_Value condition, TAC_Label target) : TAC_Instruction
    {
        public readonly TAC_Value Condition = condition;
        public readonly TAC_Label Target = target;

        public override string Print()
        {
            return string.Format("    JumpIfZero({0}, {1})", this.Condition.Print(), this.Target.Print() );
        }
    }

    public class TAC_JumpNotZero(TAC_Value condition, TAC_Label target) : TAC_Instruction
    {
        public readonly TAC_Value Condition = condition;
        public readonly TAC_Label Target = target;

        public override string Print()
        {
            return string.Format("    JumpNotZero({0}, {1})", this.Condition.Print(), this.Target.Print() );
        }
    }

    public class TAC_Label(string identifier) : TAC_Instruction
    {
        public readonly string Identifier = identifier;

        public override string Print()
        {
            return string.Format("Label({0})", this.Identifier);
        }
    }

}