namespace myc
{
    public class ASM_Operand { }

    public class ASM_Imm : ASM_Operand
    {
        public int Value;
        public ASM_Imm(int value)
        {
            this.Value = value;
        }
    }

    public class ASM_Register : ASM_Operand 
    { }
}
