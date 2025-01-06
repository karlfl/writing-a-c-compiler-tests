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
                    instructions.AddRange(ConvertUnary(tUnary));
                    break;
                case TAC_Binary tBinary:
                    instructions.AddRange(ConvertBinary(tBinary));
                    break;
                case TAC_Jump tJump:
                    instructions.Add(new ASM_Jmp(tJump.Target.Identifier));
                    break;
                case TAC_JumpIfZero tJumpZ:
                    ASM_Operand zeroCond = ConvertValue(tJumpZ.Condition);
                    instructions.Add(new ASM_Cmp(new ASM_Imm(0), zeroCond));
                    instructions.Add(new ASM_JmpCC(new ASM_EQ(), tJumpZ.Target.Identifier));
                    break;
                case TAC_JumpNotZero tJumpNZ:
                    ASM_Operand notZCond = ConvertValue(tJumpNZ.Condition);
                    instructions.Add(new ASM_Cmp(new ASM_Imm(0), notZCond));
                    instructions.Add(new ASM_JmpCC(new ASM_NE(), tJumpNZ.Target.Identifier));
                    break;
                case TAC_Copy tCopy:
                    ASM_Operand src = ConvertValue(tCopy.Source);
                    ASM_Operand dst = ConvertValue(tCopy.Destination);
                    instructions.Add(new ASM_Mov(src, dst));
                    break;
                case TAC_Label tLabel:
                    instructions.Add(new ASM_Label(tLabel.Identifier));
                    break;

                default:
                    throw new ArgumentException(string.Format("Unexpected TAC Instruction Type: {0}", instruction.GetType().Name));
            }

            return instructions;
        }

        private static List<ASM_Instruction> ConvertUnary(TAC_Unary tUnary)
        {
            List<ASM_Instruction> instr = [];

            ASM_Operand src = ConvertValue(tUnary.Source);
            ASM_Operand dst = ConvertValue(tUnary.Destination);

            switch (tUnary.UnaryOp)
            {
                case TAC_Not:
                    instr.Add(new ASM_Cmp(new ASM_Imm(0), src));
                    instr.Add(new ASM_Mov(new ASM_Imm(0), dst));
                    instr.Add(new ASM_SetCC(new ASM_EQ(), dst));
                    break;
                default:
                    ASM_UnaryOp opr = ConvertUnaryOp(tUnary.UnaryOp);
                    instr.Add(new ASM_Mov(src, dst));
                    instr.Add(new ASM_Unary(opr, dst));
                    break;
            }

            return instr;
        }

        private static List<ASM_Instruction> ConvertBinary(TAC_Binary tBinary)
        {
            List<ASM_Instruction> instr = [];

            ASM_Operand src1 = ConvertValue(tBinary.Source1);
            ASM_Operand src2 = ConvertValue(tBinary.Source2);
            ASM_Operand dst = ConvertValue(tBinary.Destination);

            switch (tBinary.BinaryOp)
            {
                case TAC_Divide:
                    instr.Add(new ASM_Mov(src1, new ASM_Register(new ASM_AX())));
                    instr.Add(new ASM_Cdq());
                    instr.Add(new ASM_Idiv(src2));
                    instr.Add(new ASM_Mov(new ASM_Register(new ASM_AX()), dst));
                    break;
                case TAC_Remainder:
                    instr.Add(new ASM_Mov(src1, new ASM_Register(new ASM_AX())));
                    instr.Add(new ASM_Cdq());
                    instr.Add(new ASM_Idiv(src2));
                    instr.Add(new ASM_Mov(new ASM_Register(new ASM_DX()), dst));
                    break;
                case TAC_Add:
                case TAC_Subtract:
                case TAC_Multiply:
                    ASM_BinaryOp binOpr = ConvertBinaryOp(tBinary.BinaryOp);
                    instr.Add(new ASM_Mov(src1, dst));
                    instr.Add(new ASM_Binary(binOpr, src2, dst));
                    break;
                case TAC_Equal:
                case TAC_NotEqual:
                case TAC_LessThan:
                case TAC_LessOrEqual:
                case TAC_GreaterThan:
                case TAC_GreaterOrEqual:
                    ASM_CondCode condCode = ConvertCondCode(tBinary.BinaryOp);
                    instr.Add(new ASM_Cmp(src2, src1));
                    instr.Add(new ASM_Mov(new ASM_Imm(0), dst));
                    instr.Add(new ASM_SetCC(condCode, dst));
                    break;
                default:
                    throw new ArgumentException(string.Format("Unexpected TAC Binary Type: {0}", tBinary.GetType().Name));
            }

            return instr;

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

        private static ASM_CondCode ConvertCondCode(TAC_BinaryOp binaryOp)
        {
            return binaryOp switch
            {
                TAC_Equal => new ASM_EQ(),
                TAC_NotEqual => new ASM_NE(),
                TAC_LessThan => new ASM_LT(),
                TAC_LessOrEqual => new ASM_LE(),
                TAC_GreaterThan => new ASM_GT(),
                TAC_GreaterOrEqual => new ASM_GE(),
                _ => throw new ArgumentException(string.Format("Cannot Convert Condition Code: Unexpected TAC BinaryOp Type: {0}", binaryOp.GetType().Name)),

            };
        }

    }
}