namespace myc
{
    public static class CodeGen
    {
        public static ASM_Program Generate(TAC_Program program)
        {
            ASM_Program prog = new(ConvertFunction(program.Function));
            return prog;
        }

        private static ASM_Function ConvertFunction(TAC_Function function)
        {
            List<ASM_Instruction> instr = [];
            foreach (TAC_Instruction stmt in function.Instructions)
            {
                instr.AddRange(ConvertInstruction(stmt));
            }

            return new(function.Name, instr);
        }

        private static List<ASM_Instruction> ConvertInstruction(TAC_Instruction instruction)
        {
            List<ASM_Instruction> instructions = [];

            switch (instruction)
            {
                case TAC_Return tReturn:
                    ASM_Operand retValue = ConvertValue(tReturn.Value);
                    instructions.Add(new ASM_Mov(retValue, new ASM_Register(new ASM_AX())));
                    instructions.Add(new ASM_Ret());
                    break;
                case TAC_Unary tUnary:
                    ASM_UnaryOp opr = ConvertUnaryOp(tUnary.UnaryOp);
                    ASM_Operand src = ConvertValue(tUnary.Source);
                    ASM_Operand dst = ConvertValue(tUnary.Destination);
                    instructions.Add(new ASM_Mov(src, dst));
                    instructions.Add(new ASM_Unary(opr, dst));
                    break;

                default:
                    throw new ArgumentException(string.Format("Unexpected TAC Instruction Type: {0}", instruction.GetType().Name));
            }

            return instructions;
        }

        private static ASM_UnaryOp ConvertUnaryOp(TAC_UnaryOp unaryOp)
        {
            return unaryOp switch
            {
                TAC_Complement => new ASM_UnaryNot(),
                TAC_Negate => new ASM_UnaryNeg(),
                _ => throw new ArgumentException(string.Format("Unexpected TAC UnaryOp Type: {0}", unaryOp.GetType().Name)),
            };
        }

        private static ASM_Operand ConvertValue(TAC_Value expression)
        {
            return expression switch
            {
                TAC_Constant cnst => new ASM_Imm(cnst.Value),
                TAC_Variable value => new ASM_Pseudo(value.Name),
                _ => throw new ArgumentException(string.Format("Unexpected TAC Value Type: {0}", expression.GetType().Name)),
            };
        }

    }
}