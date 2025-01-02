namespace myc
{
    public class ASM_Function
    {
        public readonly string Name;
        public List<ASM_Instruction> Instructions;

        public ASM_Function(string name, List<ASM_Instruction> instructions)
        {
            this.Name = name;
            this.Instructions = instructions;
        }
    }
}