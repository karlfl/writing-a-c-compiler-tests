namespace myc {
    public class AST_Expression : AST_Base
    {
        public readonly int Value;

        public AST_Expression(int value)
        {
            this.Value = value;
        }

        public string Print()
        {
            return string.Format("{0}",this.Value);
        }
    }
}