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

        private static (List<TAC_Instruction> instructions, TAC_Value value) EmitForExpression(AST_Expression expression)
        {
            switch (expression)
            {
                case AST_Constant constant:
                    return ([], new TAC_Constant(constant.Value));
                case AST_Unary unary:
                    var (innerInstr, src) = EmitForExpression(unary.Expression);
                    TAC_Variable dst = new(Utilities.GenerateUniqueId());
                    TAC_UnaryOp unaryOp = unary.UnaryOp switch
                    {
                        AST_Complement => new TAC_Complement(),
                        AST_Negate => new TAC_Negate(),
                        _ => throw new ArgumentException("Unexpected AST Unary Operation"),
                    };
                    innerInstr.Add(new TAC_Unary(unaryOp, src, dst));
                    return (innerInstr, dst);
                default:
                    throw new ArgumentException("TAC: Unexpected AST Expression type");
            }
        }
    }
}