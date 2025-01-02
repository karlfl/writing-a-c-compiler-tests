namespace myc
{
    public abstract class TAC_Value { 
        public abstract string Print();
    }

    public class TAC_Constant(int Value) : TAC_Value
    {
        public readonly int Value = Value;

        public override string Print()
        {
            return string.Format("{0}", this.Value);
        }
}

    public class TAC_Variable(string name) : TAC_Value
    {
        public readonly string Name = name;

        public override string Print()
        {
            return string.Format("{0}", this.Name);
        }
    }
}