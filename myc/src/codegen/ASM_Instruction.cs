namespace myc
{
    public class ASM_Instruction
    {
    }

    public class ASM_Ret : ASM_Instruction 
    {
        public ASM_Ret(){}
    }

    public class ASM_Mov : ASM_Instruction 
    {
        public readonly ASM_Operand Source;
        public readonly ASM_Operand Destination;
        public ASM_Mov(ASM_Operand source, ASM_Operand destination)
        {
            this.Source = source;
            this.Destination = destination;
        }
    }
}