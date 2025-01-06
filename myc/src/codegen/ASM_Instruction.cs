using System.Text;

namespace myc
{
    public class ASM_Instruction
    {
    }

    public class ASM_Mov(ASM_Operand source, ASM_Operand destination) : ASM_Instruction
    {
        public readonly ASM_Operand Source = source;
        public readonly ASM_Operand Destination = destination;
    }

    public class ASM_Unary(ASM_UnaryOp unaryOp, ASM_Operand operand) : ASM_Instruction
    {
        public readonly ASM_UnaryOp UnaryOp = unaryOp;
        public readonly ASM_Operand Operand = operand;
    }

    public class ASM_Binary(ASM_BinaryOp binaryOp, ASM_Operand operand1, ASM_Operand operand2) : ASM_Instruction
    {
        public readonly ASM_BinaryOp BinaryOp = binaryOp;
        public readonly ASM_Operand Operand1 = operand1;
        public readonly ASM_Operand Operand2 = operand2;
    }

    public class ASM_Idiv(ASM_Operand operand) : ASM_Instruction
    {
        public readonly ASM_Operand Operand = operand;
    }

    public class ASM_Cdq() : ASM_Instruction { }

    public class ASM_Cmp(ASM_Operand operand1, ASM_Operand operand2) : ASM_Instruction
    {
        public readonly ASM_Operand Operand1 = operand1;
        public readonly ASM_Operand Operand2 = operand2;
    }

    public class ASM_Jmp(string identifier) : ASM_Instruction
    {
        public readonly string Identifier = identifier;
    }

    public class ASM_JmpCC(ASM_CondCode condCode, string identifier) : ASM_Instruction
    {
        public readonly ASM_CondCode ConditionCode = condCode;
        public readonly string Identifier = identifier;
    }

    public class ASM_SetCC(ASM_CondCode condCode, ASM_Operand operand) : ASM_Instruction
    {
        public readonly ASM_CondCode ConditionCode = condCode;
        public readonly ASM_Operand Operand = operand;
    }

    public class ASM_Label(string identifier) : ASM_Instruction
    {
        public readonly string Identifier = identifier;
    }

    public class ASM_AllocateStack(int size) : ASM_Instruction
    {
        public readonly int Size = size;
    }

    public class ASM_Ret : ASM_Instruction
    {
        public ASM_Ret() { }
    }
}