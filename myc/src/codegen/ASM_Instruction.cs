namespace myc
{
    public class ASM_Instruction
    {
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

    public class ASM_Unary : ASM_Instruction
    {
        public readonly ASM_UnaryOp UnaryOp;
        public readonly ASM_Operand Operand;
        public ASM_Unary(ASM_UnaryOp unaryOp, ASM_Operand operand)
        {
            this.UnaryOp = unaryOp;
            this.Operand = operand;
        }
    }

    public class ASM_AllocateStack : ASM_Instruction
    {
        public readonly int Size;
        public ASM_AllocateStack(int size)
        {
            this.Size = size;
        }
    }

    public class ASM_Ret : ASM_Instruction
    {
        public ASM_Ret() { }
    }

}