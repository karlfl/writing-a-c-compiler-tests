namespace myc
{
    public static class CodeGen
    {
        public static ASM_Program Generate(AST_Program program)
        {
            ASM_Program prog = new(ConvertFunction(program.Function));
            return prog;
        }

        private static ASM_Function ConvertFunction(AST_Function function) {
            List<ASM_Instruction> instr = [];
            foreach(AST_Statement stmt in function.Statements){
                instr.AddRange(ConvertStatement(stmt));
            }

            return new(function.Identifier.Name, instr);
        }

        private static List<ASM_Instruction> ConvertStatement(AST_Statement statement){
            List<ASM_Instruction> instructions = []; 
            
            switch (statement.Instruction)
            {
                case TokensEnum.KWReturn:
                    ASM_Operand value = ConvertExpression_Int(statement.Expression);
                    instructions.Add(new ASM_Mov(value, new ASM_Register()));
                    instructions.Add(new ASM_Ret());
                    break;
                default:
                    throw new ArgumentException("Unexpected AST Instruction Type");
            }

            return instructions;
        }

        private static ASM_Operand ConvertExpression_Int (AST_Expression expression) {
            return new ASM_Imm(expression.Value);
        }

    }
}