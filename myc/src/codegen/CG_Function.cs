namespace myc
{
    public class CG_Function
    {
        readonly AST_Function Function;
        readonly List<CG_Instruction> Instructions;

        public CG_Function(AST_Function function)
        {
            this.Function = function;
        }
    }
}