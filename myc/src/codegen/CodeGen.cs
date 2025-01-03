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
                case TAC_Binary tBinary:
                    List<ASM_Instruction> binInstr = ConvertBinary(tBinary);
                    instructions.AddRange(binInstr);
                    break;
                default:
                    throw new ArgumentException(string.Format("Unexpected TAC Instruction Type: {0}", instruction.GetType().Name));
            }

            return instructions;
        }

        private static List<ASM_Instruction> ConvertBinary(TAC_Binary tBinary)
        {
            List<ASM_Instruction> binInstr = [];
            switch (tBinary.BinaryOp)
            {
                case TAC_Divide:
                    ASM_Operand divSrc1 = ConvertValue(tBinary.Source1);
                    ASM_Operand divSrc2 = ConvertValue(tBinary.Source2);
                    ASM_Operand divDst = ConvertValue(tBinary.Destination);
                    binInstr.Add(new ASM_Mov(divSrc1, new ASM_Register(new ASM_AX())));
                    binInstr.Add(new ASM_Cdq());
                    binInstr.Add(new ASM_Idiv(divSrc2));
                    binInstr.Add(new ASM_Mov(new ASM_Register(new ASM_AX()), divDst));
                    break;
                case TAC_Remainder:
                    ASM_Operand remSrc1 = ConvertValue(tBinary.Source1);
                    ASM_Operand remSrc2 = ConvertValue(tBinary.Source2);
                    ASM_Operand remDst = ConvertValue(tBinary.Destination);
                    binInstr.Add(new ASM_Mov(remSrc1, new ASM_Register(new ASM_AX())));
                    binInstr.Add(new ASM_Cdq());
                    binInstr.Add(new ASM_Idiv(remSrc2));
                    binInstr.Add(new ASM_Mov(new ASM_Register(new ASM_DX()), remDst));
                    break;
                case TAC_Add:
                case TAC_Subtract:
                case TAC_Multiply:
                    ASM_BinaryOp binOpr = ConvertBinaryOp(tBinary.BinaryOp);
                    ASM_Operand binSrc1 = ConvertValue(tBinary.Source1);
                    ASM_Operand binSrc2 = ConvertValue(tBinary.Source2);
                    ASM_Operand binDst = ConvertValue(tBinary.Destination);
                    binInstr.Add(new ASM_Mov(binSrc1, binDst));
                    binInstr.Add(new ASM_Binary(binOpr, binSrc2, binDst));
                    break;
                default:
                    throw new ArgumentException(string.Format("Unexpected TAC Binary Type: {0}", tBinary.GetType().Name));
            }

            return binInstr;

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

        private static ASM_BinaryOp ConvertBinaryOp(TAC_BinaryOp binaryOp)
        {
            return binaryOp switch
            {
                TAC_Add => new ASM_BinaryAdd(),
                TAC_Subtract => new ASM_BinarySub(),
                TAC_Multiply => new ASM_BinaryMult(),
                _ => throw new ArgumentException(string.Format("Unexpected TAC BinaryOp Type: {0}", binaryOp.GetType().Name)),
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