namespace myc
{
    public class ASM_Function
    {
        public readonly string Name;
        public readonly List<ASM_Instruction> Body;

        public ASM_Function(string name, List<ASM_Instruction> instructions)
        {
            this.Name = name;
            this.Body = instructions;
        }
    }
}