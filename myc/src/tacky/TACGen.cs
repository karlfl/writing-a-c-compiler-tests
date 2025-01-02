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
                    var (innerUnaryInstr, unarySrc) = EmitForExpression(unary.Factor);
                    TAC_Variable dst = new(Utilities.GenerateUniqueId());
                    TAC_UnaryOp unaryOp = ConvertUnaryOp(unary.UnaryOp);
                    innerUnaryInstr.Add(new TAC_Unary(unaryOp, unarySrc, dst));
                    return (innerUnaryInstr, dst);
                case AST_Binary binary:
                    var (innerBinaryInstr, binarySrc1) = EmitForExpression(binary.LeftFactor);
                    var (innerBinaryInstr2, binarySrc2) = EmitForExpression(binary.RightFactor);
                    innerBinaryInstr.AddRange(innerBinaryInstr2);
                    TAC_Variable binaryDst = new(Utilities.GenerateUniqueId());
                    TAC_BinaryOp binaryOp = ConvertBinaryOp(binary.BinaryOp);
                    innerBinaryInstr.Add(new TAC_Binary(binaryOp, binarySrc1, binarySrc2, binaryDst));
                    return (innerBinaryInstr, binaryDst);

                default:
                    throw new ArgumentException("TAC: Unexpected AST Expression type");
            }

            static TAC_BinaryOp ConvertBinaryOp(AST_BinaryOp binaryOp)
            {
                return binaryOp switch
                {
                    AST_Add => new TAC_Add(),
                    AST_Subtract => new TAC_Subtract(),
                    AST_Multiply => new TAC_Multiply(),
                    AST_Divide => new TAC_Divide(),
                    AST_Mod => new TAC_Remainder(),
                    _ => throw new ArgumentException("Unexpected AST Unary Operation"),
                };
            }
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
    }
}