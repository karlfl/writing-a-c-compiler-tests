namespace myc
{
    public class TAC_Program 
    {
        public readonly TAC_Function Function;

        public TAC_Program(TAC_Function function)
        {
            this.Function = function;
            
        }

        public string Print()
        {
            return Function.Print();
        }
    }
}