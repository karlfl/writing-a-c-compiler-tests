using System.Diagnostics;

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
            List<TAC_Instruction> instructions = EmitForStatements(function.Body);
            //Extra return;
            instructions.Add(new TAC_Return(new TAC_Constant(0)));

            return new(function.Identifier.Name, instructions);
        }

        static List<TAC_Instruction> EmitForStatements(List<AST_BlockItem> blockItems)
        {
            List<TAC_Instruction> instructions = [];

            foreach (AST_BlockItem blockItem in blockItems)
            {
                switch (blockItem)
                {
                    case AST_BlockStatement statement:
                        switch (statement.Statement)
                        {
                            case AST_Return aReturn:
                                var (instr, value) = EmitForExpression(aReturn.Expression);
                                instructions.AddRange(instr);
                                instructions.Add(new TAC_Return(value));
                                break;
                            case AST_Expression aExp:
                                // evaluate expression but don't use result.
                                var (expInstr, _) = EmitForExpression(aExp.Expression);
                                instructions.AddRange(expInstr);
                                break;
                            case AST_Null:
                                break;
                            default:
                                throw new ArgumentException("TAC: Unexpected AST Expression type");
                        }
                        break;
                    case AST_BlockDeclaration declaration:
                        break;
                    default:
                        throw new ArgumentException("TAC: Unexpected AST Block Item type");
                }
            }

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
                AST_Not => new TAC_Not(),
                _ => throw new ArgumentException("Unexpected AST Unary Operation"),
            };
        }

        private static (List<TAC_Instruction>, TAC_Value) EmitBinary(AST_Binary binary)
        {
            return binary.BinaryOp switch
            {
                AST_LogicalAnd => EmitLogicalAnd(binary),
                AST_LogicalOr => EmitLogicalOr(binary),
                _ => EmitBinaryOp(binary),
            };
        }

        private static (List<TAC_Instruction>, TAC_Value) EmitBinaryOp(AST_Binary binary)
        {
            List<TAC_Instruction> instr = [];

            var (leftInstr, leftSrc) = EmitForExpression(binary.LeftFactor);
            var (rightInstr, rightSrc) = EmitForExpression(binary.RightFactor);
            TAC_Variable dstName = new(Utilities.GenerateUniqueId());
            TAC_BinaryOp binaryOp = ConvertBinaryOp(binary.BinaryOp);

            instr.AddRange(leftInstr);
            instr.AddRange(rightInstr);
            instr.Add(new TAC_Binary(binaryOp, leftSrc, rightSrc, dstName));

            return (instr, dstName);
        }

        private static (List<TAC_Instruction>, TAC_Variable) EmitLogicalAnd(AST_Binary binaryAnd)
        {
            List<TAC_Instruction> instr = [];

            var (leftCond, valLeft) = EmitForExpression(binaryAnd.LeftFactor);
            var (rightCond, valRight) = EmitForExpression(binaryAnd.RightFactor);
            TAC_Label falseLabel = new(Utilities.GenerateUniqueLabel("and_false"));
            TAC_Label endLabel = new(Utilities.GenerateUniqueLabel("and_end"));
            TAC_Variable dst = new(Utilities.GenerateUniqueId());

            instr.AddRange(leftCond);
            instr.Add(new TAC_JumpIfZero(valLeft, falseLabel));
            instr.AddRange(rightCond);
            instr.Add(new TAC_JumpIfZero(valRight, falseLabel));
            instr.Add(new TAC_Copy(new TAC_Constant(1), dst));
            instr.Add(new TAC_Jump(endLabel));
            instr.Add(falseLabel);
            instr.Add(new TAC_Copy(new TAC_Constant(0), dst));
            instr.Add(endLabel);


            return (instr, dst);
        }

        private static (List<TAC_Instruction>, TAC_Variable) EmitLogicalOr(AST_Binary binaryOr)
        {
            List<TAC_Instruction> instr = [];

            var (leftCond, valLeft) = EmitForExpression(binaryOr.LeftFactor);
            var (rightCond, valRight) = EmitForExpression(binaryOr.RightFactor);
            TAC_Label trueLabel = new(Utilities.GenerateUniqueLabel("or_true"));
            TAC_Label endLabel = new(Utilities.GenerateUniqueLabel("or_end"));
            TAC_Variable dst = new(Utilities.GenerateUniqueId());

            instr.AddRange(leftCond);
            instr.Add(new TAC_JumpNotZero(valLeft, trueLabel));
            instr.AddRange(rightCond);
            instr.Add(new TAC_JumpNotZero(valRight, trueLabel));
            instr.Add(new TAC_Copy(new TAC_Constant(0), dst));
            instr.Add(new TAC_Jump(endLabel));
            instr.Add(trueLabel);
            instr.Add(new TAC_Copy(new TAC_Constant(1), dst));
            instr.Add(endLabel);

            return (instr, dst);
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
                AST_Equal => new TAC_Equal(),
                AST_NotEqual => new TAC_NotEqual(),
                AST_LessThan => new TAC_LessThan(),
                AST_LessOrEqual => new TAC_LessOrEqual(),
                AST_GreaterThan => new TAC_GreaterThan(),
                AST_GreaterOrEqual => new TAC_GreaterOrEqual(),
                _ => throw new ArgumentException("Unexpected AST Binary Operation"),
            };
        }
    }
}