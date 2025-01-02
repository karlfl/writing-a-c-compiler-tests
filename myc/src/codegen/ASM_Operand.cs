namespace myc
{
    public class ASM_Operand { }

    public class ASM_Imm(int value) : ASM_Operand
    {
        public int Value = value;
    }

    public class ASM_Register(ASM_RegisterValue register) : ASM_Operand
    {
        public ASM_RegisterValue Register = register;
    }

    public class ASM_Pseudo(string identifier) : ASM_Operand
    {
        public string Identifier = identifier;
    }

    public class ASM_Stack(int stack) : ASM_Operand
    {
        public int Stack = stack;
    }}
