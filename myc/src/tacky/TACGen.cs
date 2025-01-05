namespace myc
{
    public static class TACGen
    {

        public static TAC_Program Generate(AST_Program program)
        {
            return new(EmitFunction(program.Function));
        }

        static TAC_Function EmitFunction(AST_Function function)
        {
            List<TAC_Instruction> instructions = EmitForStatements(function.Statements);
            return new(function.Identifier.Name, instructions);
        }

        static List<TAC_Instruction> EmitForStatements(List<AST_Statement> statements)
        {
            List<TAC_Instruction> instructions = [];

            var (instr, value) = EmitForExpression(statements[0].Expression);
            instructions.AddRange(instr);
            instructions.Add(new TAC_Return(value));

            return instructions;
        }

        private static (List<TAC_Instruction> instructions, TAC_Value value) EmitForExpression(AST_Factor expression)
        {
            switch (expression)
            {
                case AST_Int constant:
                    return ([], new TAC_Constant(constant.Value));
                case AST_Unary unary:
                    return EmitUnary(unary);
                case AST_Binary binary:
                    return EmitBinary(binary);
                default:
                    throw new ArgumentException("TAC: Unexpected AST Expression type");
            }

        }

        private static (List<TAC_Instruction> instructions, TAC_Value value) EmitUnary(AST_Unary unary)
        {
            var (innerUnaryInstr, unarySrc) = EmitForExpression(unary.Factor);
            TAC_Variable dst = new(Utilities.GenerateUniqueId());
            TAC_UnaryOp unaryOp = ConvertUnaryOp(unary.UnaryOp);
            innerUnaryInstr.Add(new TAC_Unary(unaryOp, unarySrc, dst));
            return (innerUnaryInstr, dst);
        }

        private static TAC_UnaryOp ConvertUnaryOp(AST_UnaryOp unaryOp)
        {
            return unaryOp switch
            {
                AST_Complement => new TAC_Complement(),
                AST_Negate => new TAC_Negate(),
                _ => throw new ArgumentException("Unexpected AST Unary Operation"),
            };
        }

        private static (List<TAC_Instruction> instructions, TAC_Value value) EmitBinary(AST_Binary binary)
        {
            switch (binary.BinaryOp)
            {
                case AST_LogicalAnd:
                    return EmitLogicalAnd();
                case AST_LogicalOr:
                    return EmitLogicalOr();
                default:
                    var (innerBinaryInstr, binarySrc1) = EmitForExpression(binary.LeftFactor);
                    var (innerBinaryInstr2, binarySrc2) = EmitForExpression(binary.RightFactor);

                    innerBinaryInstr.AddRange(innerBinaryInstr2);
                    TAC_Variable binaryDst = new(Utilities.GenerateUniqueId());
                    TAC_BinaryOp binaryOp = ConvertBinaryOp(binary.BinaryOp);
                    innerBinaryInstr.Add(new TAC_Binary(binaryOp, binarySrc1, binarySrc2, binaryDst));

                    return (innerBinaryInstr, binaryDst);
            }
        }

        private static List<TAC_Instruction> EmitLogicalAnd(AST_Binary binaryAnd)
        {
            List<TAC_Instruction> instr = [];

            var (leftInstr, valLeft) = EmitForExpression(binaryAnd.LeftFactor);
            var (rightInstr, valRight) = EmitForExpression(binaryAnd.RightFactor);
            TAC_Label falseLabel = new(Utilities.GenerateUniqueLabel("and_false"));
            TAC_Label endLabel = new(Utilities.GenerateUniqueLabel("and_end"));
            TAC_Variable dst = new(Utilities.GenerateUniqueId());

            instr.AddRange(leftInstr);
            instr.Add(new TAC_JumpIfZero(valLeft,falseLabel));
            instr.AddRange(rightInstr);
            instr.Add(new TAC_JumpIfZero(valRight,falseLabel));
            instr.Add(new TAC_Copy(new TAC_Constant(1), dst));
            instr.Add(new TAC_Jump(endLabel));
            instr.Add(falseLabel);
            instr.Add(new TAC_Copy(new TAC_Constant(0), dst));
            instr.Add(endLabel);
            

            return instr;
        }

        private static List<TAC_Instruction> EmitLogicalOr(AST_Binary binaryOr)
        {
            List<TAC_Instruction> instr = [];
            var (leftBinInstr, valBinLeft) = EmitForExpression(binaryOr.LeftFactor);
            var (rightBinInstr, valBinRight) = EmitForExpression(binaryOr.RightFactor);

            return instr;
        }
        private static TAC_BinaryOp ConvertBinaryOp(AST_BinaryOp binaryOp)
        {
            return binaryOp switch
            {
                AST_Add => new TAC_Add(),
                AST_Subtract => new TAC_Subtract(),
                AST_Multiply => new TAC_Multiply(),
                AST_Divide => new TAC_Divide(),
                AST_Mod => new TAC_Remainder(),
                _ => throw new ArgumentException("Unexpected AST Binary Operation"),
            };
        }
    }
}